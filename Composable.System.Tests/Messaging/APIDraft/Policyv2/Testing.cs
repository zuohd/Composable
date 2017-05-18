﻿using System;
using System.Collections.Generic;
using System.Threading;
using Composable.System;
using Composable.System.Collections.Collections;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable All

namespace Composable.Tests.Messaging.APIDraft.Policyv2
{
    using Composable.System;

    [TestFixture]
    public class TestResetEvent
    {
        [Test]
        public void Test()
        {
            var @event = new ManualResetEventSlim(false);

            @event.Reset();
            @event.Reset();

            @event.Set();
            @event.Wait(1.Milliseconds())
                  .Should()
                  .Be(true);

            @event.Reset();
            @event.Wait(1.Milliseconds())
                  .Should()
                  .Be(false);
        }
    }

    public class Testing
    {
        public void Test()
        {
            var createAccountHandler = new TestMessageHandler<CreateAccountCommand>();
            var accountQueryModelUpdater = new TestMessageHandler<AccountCreatedEvent>();

            var endpoint = new Endpoint(
                CommandHandler.For<CreateAccountCommand>("BE8B06E7-28BB-439D-BDD6-CF7E9454424B", createAccountHandler.Handle),
                EventHandler.For<AccountCreatedEvent>("AD198D3E-5340-4CB3-8BDB-31AFD0C7FC9A", accountQueryModelUpdater.Handle)
                );

            //bus.Send(new CreateAccountCommand)
            createAccountHandler.Started.Wait();
            accountQueryModelUpdater.IsStarted.Should().Be(false);
            createAccountHandler.AllowToComplete.Set();
            accountQueryModelUpdater.Started.Wait();
        }


        class TestingResetEvent
        {
            private readonly ManualResetEventSlim _event = new ManualResetEventSlim(false);
            public void Wait()
            {
                if (!_event.Wait(TimeSpanExtensions.Milliseconds(10)))
                {
                    throw new Exception("Timed out waiting for lock.");
                }
            }

            public void Set() => _event.Set();
            public void Reset() => _event.Reset();
        }


        class ResetEvents
        {
            private readonly Dictionary<string, TestingResetEvent> _manuals = new Dictionary<string, TestingResetEvent>();

            public TestingResetEvent Manual(string name)
            {
                lock (_manuals)
                {
                    return _manuals.GetOrAdd(name, () => new TestingResetEvent());
                }
            }

            public TestingResetEvent Manual(int key) => Manual(key.ToString());
            public TestingResetEvent Manual(Guid key) => Manual(key.ToString());
        }



        //Register a handler implemented like this and you get full insight into when it is invoked, and full control over when it is allowed to complete.
        //This should give us full testability of invokation policies :)
        class TestMessageHandler<T>
        {
            public readonly TestingResetEvent Started = new TestingResetEvent();
            public readonly  TestingResetEvent Completed = new TestingResetEvent();
            public  readonly  TestingResetEvent AllowToComplete = new TestingResetEvent();

            public bool IsStarted = false;
            public bool IsCompleted = false;
            public bool IsRunning => IsStarted && !IsCompleted;


            public void Handle(T message)
            {
                Completed.Reset();
                IsCompleted = false;
                Started.Set();
                IsStarted = true;
                

                AllowToComplete.Wait();
                AllowToComplete.Reset();

                Completed.Set();
                IsCompleted = true;                
                Started.Reset();
                IsStarted = false;                
            }
        }
    }
}