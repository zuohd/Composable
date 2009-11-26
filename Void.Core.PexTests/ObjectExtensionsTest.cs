// <copyright file="ObjectExtensionsTest.cs" company="Microsoft">Copyright © Microsoft 2009</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Void;

namespace Void
{
    /// <summary>This class contains parameterized unit tests for ObjectExtensions</summary>
    [PexClass(typeof(ObjectExtensions))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class ObjectExtensionsTest
    {
        /// <summary>Test stub for Do(!!0, Action`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public void Do<T>(T me, Action<T> action)
        {
            ObjectExtensions.Do<T>(me, action);
            // TODO: add assertions to method ObjectExtensionsTest.Do(!!0, Action`1<!!0>)
        }

        /// <summary>Test stub for Transform(!!0, Func`2&lt;!!0,!!1&gt;)</summary>
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public TReturn Transform<TSource,TReturn>(TSource me, Func<TSource, TReturn> transformation)
        {
            TReturn result
               = ObjectExtensions.Transform<TSource, TReturn>(me, transformation);
            return result;
            // TODO: add assertions to method ObjectExtensionsTest.Transform(!!0, Func`2<!!0,!!1>)
        }
    }
}
