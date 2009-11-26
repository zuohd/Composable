// <copyright file="SeqTest.cs" company="Microsoft">Copyright © Microsoft 2009</copyright>
using System;
using System.Collections.Generic;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Void.Linq;

namespace Void.Linq
{
    /// <summary>This class contains parameterized unit tests for Seq</summary>
    [PexClass(typeof(Seq))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class SeqTest
    {
        /// <summary>Test stub for Create(!!0[])</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public IEnumerable<T> Create<T>(T[] values)
        {
            IEnumerable<T> result = Seq.Create<T>(values);
            return result;
            // TODO: add assertions to method SeqTest.Create(!!0[])
        }
    }
}
