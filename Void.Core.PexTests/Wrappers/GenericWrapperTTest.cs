// <copyright file="GenericWrapperTTest.cs" company="Microsoft">Copyright � Microsoft 2009</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Void.Wrappers;

namespace Void.Wrappers
{
    /// <summary>This class contains parameterized unit tests for GenericWrapper`1</summary>
    [PexClass(typeof(GenericWrapper<>))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class GenericWrapperTTest
    {
        /// <summary>Test stub for .ctor(!0)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public GenericWrapper<T> Constructor<T>(T wrapped)
        {
            GenericWrapper<T> target = new GenericWrapper<T>(wrapped);
            return target;
            // TODO: add assertions to method GenericWrapperTTest.Constructor(!!0)
        }

        /// <summary>Test stub for Wrapped</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public void WrappedGet<T>([PexAssumeUnderTest]GenericWrapper<T> target)
        {
            T result = target.Wrapped;
            // TODO: add assertions to method GenericWrapperTTest.WrappedGet(GenericWrapper`1<!!0>)
        }
    }
}