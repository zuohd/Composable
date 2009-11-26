// <copyright file="WrapperExtensionsTest.cs" company="Microsoft">Copyright © Microsoft 2009</copyright>
using System;
using System.Collections.Generic;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Void.Wrappers;

namespace Void.Wrappers
{
    /// <summary>This class contains parameterized unit tests for WrapperExtensions</summary>
    [PexClass(typeof(WrapperExtensions))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class WrapperExtensionsTest
    {
        /// <summary>Test stub for Unwrap(IEnumerable`1&lt;IWrapper`1&lt;!!0&gt;&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public IEnumerable<T> Unwrap<T>(IEnumerable<IWrapper<T>> wrapper)
        {
            IEnumerable<T> result = WrapperExtensions.Unwrap<T>(wrapper);
            return result;
            // TODO: add assertions to method WrapperExtensionsTest.Unwrap(IEnumerable`1<IWrapper`1<!!0>>)
        }
    }
}
