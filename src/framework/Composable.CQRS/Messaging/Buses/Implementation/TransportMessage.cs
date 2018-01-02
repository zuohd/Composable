﻿using System;
using System.Collections.Generic;
using Composable.Contracts;
using Composable.NewtonSoft;
using Composable.System.Reflection;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace Composable.Messaging.Buses.Implementation
{
    static class TransportMessage
    {
        internal enum TransportMessageType
        {
            Event,
            Command,
            Query
        }

        internal class InComing
        {
            public readonly byte[] Client;
            public readonly Guid MessageId;
            readonly string _body;
            readonly string _messageType;

            IMessage _message;

            public IMessage DeserializeMessageAndCacheForNextCall()
            {
                if(_message == null)
                {
                    _message = (IMessage)JsonConvert.DeserializeObject(_body, _messageType.AsType(), JsonSettings.JsonSerializerSettings);


                    Contract.State.Assert(!(_message is IExactlyOnceDeliveryMessage) || MessageId == (_message as IExactlyOnceDeliveryMessage).MessageId);
                }
                return _message;
            }

            InComing(string body, string messageType, byte[] client, Guid messageId)
            {
                _body = body;
                _messageType = messageType;
                Client = client;
                MessageId = messageId;
            }

            public static InComing Receive(RouterSocket socket)
            {
                var receivedMessage = socket.ReceiveMultipartMessage();

                var client = receivedMessage[0].ToByteArray();
                var messageId = new Guid(receivedMessage[1].ToByteArray());
                var messageTypeString = receivedMessage[2].ConvertToString();
                var messageBody = receivedMessage[3].ConvertToString();

                return new InComing(messageBody, messageTypeString, client, messageId);
            }

            public Response.Outgoing CreateFailureResponse(Exception exception) => Response.Outgoing.Failure(this, exception);

            public Response.Outgoing CreateSuccessResponse(object response) => Response.Outgoing.Success(this, response);

            public Response.Outgoing CreatePersistedResponse() => Response.Outgoing.Persisted(this);
        }

        internal class OutGoing
        {
            public bool IsExactlyOnceDeliveryMessage { get; }
            public readonly Guid MessageId;
            public readonly TransportMessageType Type;

            readonly string _messageType;
            readonly string _messageBody;

            public void Send(IOutgoingSocket socket)
            {
                var message = new NetMQMessage(4);
                message.Append(MessageId);
                message.Append(_messageType);
                message.Append(_messageBody);

                socket.SendMultipartMessage(message);
            }

            public static OutGoing Create(IMessage message)
            {
                var messageId = (message as IExactlyOnceDeliveryMessage)?.MessageId ?? Guid.NewGuid();
                var body = JsonConvert.SerializeObject(message, Formatting.Indented, JsonSettings.JsonSerializerSettings);
                return new OutGoing(message.GetType(), messageId, body, GetMessageType(message), message is IExactlyOnceDeliveryMessage);
            }

            static TransportMessageType GetMessageType(IMessage message)
            {
                switch(message) {
                    case IEvent _:
                        return TransportMessageType.Event;
                    case ICommand _:
                        return TransportMessageType.Command;
                    case IQuery _:
                        return TransportMessageType.Query;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            OutGoing(Type messageType, Guid messageId, string messageBody, TransportMessageType type, bool isExactlyOnceDeliveryMessage)
            {
                IsExactlyOnceDeliveryMessage = isExactlyOnceDeliveryMessage;
                Type = type;
                _messageType = messageType.FullName;
                MessageId = messageId;
                _messageBody = messageBody;
            }
        }

        internal class Response
        {
            internal enum ResponseType
            {
                Success,
                Failure,
                Persisted
            }

            static class Constants
            {
                public const string NullString = "NULL";
            }

            internal class Outgoing
            {
                readonly NetMQMessage _response;

                Outgoing(NetMQMessage response) => _response = response;

                public void Send(IOutgoingSocket socket) => socket.SendMultipartMessage(_response);

                public static Outgoing Success(TransportMessage.InComing incoming, object result)
                {
                    var responseMessage = new NetMQMessage();

                    responseMessage.Append(incoming.Client);
                    responseMessage.Append(incoming.MessageId);
                    responseMessage.Append((int)ResponseType.Success);

                    if(result != null)
                    {
                        responseMessage.Append(result.GetType().FullName);
                        responseMessage.Append(JsonConvert.SerializeObject(result, Formatting.Indented, JsonSettings.JsonSerializerSettings));
                    } else
                    {
                        responseMessage.Append(Constants.NullString);
                        responseMessage.Append(Constants.NullString);
                    }
                    return new Outgoing(responseMessage);
                }

                public static Outgoing Failure(TransportMessage.InComing incoming, Exception failure)
                {
                    var response = new NetMQMessage();

                    response.Append(incoming.Client);
                    response.Append(incoming.MessageId);
                    response.Append((int)ResponseType.Failure);

                    return new Outgoing(response);
                }

                public static Outgoing Persisted(InComing incoming)
                {
                    var responseMessage = new NetMQMessage();

                    responseMessage.Append(incoming.Client);
                    responseMessage.Append(incoming.MessageId);
                    responseMessage.Append((int)ResponseType.Persisted);
                    return new Outgoing(responseMessage);
                }
            }

            internal class Incoming
            {
                readonly string _resultJson;
                readonly string _responseType;
                object _result;
                internal ResponseType ResponseType { get; }
                internal Guid RespondingToMessageId { get; }

                public object DeserializeResult()
                {
                    if(_result == null)
                    {
                        if(_resultJson == Constants.NullString)
                        {
                            return null;
                        }
                        _result = JsonConvert.DeserializeObject(_resultJson, _responseType.AsType(), JsonSettings.JsonSerializerSettings);
                    }
                    return _result;
                }

                public static IReadOnlyList<Response.Incoming> ReceiveBatch(IReceivingSocket socket, int batchMaximum)
                {
                    var result = new List<Response.Incoming>();
                    NetMQMessage received = null;
                    while(socket.TryReceiveMultipartMessage(TimeSpan.Zero, ref received))
                    {
                        result.Add(FromMultipartMessage(received));
                    }
                    return result;
                }

                static Incoming FromMultipartMessage(NetMQMessage message)
                {
                    var messageId = new Guid(message[0].ToByteArray());
                    var type = (ResponseType)message[1].ConvertToInt32();

                    switch(type)
                    {
                        case ResponseType.Success:
                            var responseType = message[2].ConvertToString();
                            var responseBody = message[3].ConvertToString();
                            return new Incoming(type: type, respondingToMessageId: messageId, resultJson: responseBody, responseType: responseType);
                        case ResponseType.Failure:
                            return new Incoming(type: type, respondingToMessageId: messageId, resultJson: null, responseType: null);
                        case ResponseType.Persisted:
                            return new Incoming(type: type, respondingToMessageId: messageId, resultJson: null, responseType: null);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                Incoming(ResponseType type, Guid respondingToMessageId, string resultJson, string responseType)
                {
                    _resultJson = resultJson;
                    _responseType = responseType;
                    ResponseType = type;
                    RespondingToMessageId = respondingToMessageId;
                }
            }
        }
    }
}
