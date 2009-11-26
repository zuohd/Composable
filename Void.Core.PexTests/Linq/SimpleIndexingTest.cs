// <copyright file="SimpleIndexingTest.cs" company="Microsoft">Copyright © Microsoft 2009</copyright>
using System;
using System.Collections.Generic;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Void.Linq;

namespace Void.Linq
{
    /// <summary>This class contains parameterized unit tests for SimpleIndexing</summary>
    [PexClass(typeof(SimpleIndexing))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class SimpleIndexingTest
    {
        /// <summary>Test stub for Eighth(IEnumerable`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod, PexAllowedException(typeof(ArgumentOutOfRangeException))]
        public T Eighth<T>(IEnumerable<T> me)
        {
            T result = SimpleIndexing.Eighth<T>(me);
            return result;
            // TODO: add assertions to method SimpleIndexingTest.Eighth(IEnumerable`1<!!0>)
        }

        /// <summary>Test stub for Fifth(IEnumerable`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod, PexAllowedException(typeof(ArgumentOutOfRangeException))]
        public T Fifth<T>(IEnumerable<T> me)
        {
            T result = SimpleIndexing.Fifth<T>(me);
            return result;
            // TODO: add assertions to method SimpleIndexingTest.Fifth(IEnumerable`1<!!0>)
        }

        /// <summary>Test stub for Fourth(IEnumerable`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod, PexAllowedException(typeof(ArgumentOutOfRangeException))]
        public T Fourth<T>(IEnumerable<T> me)
        {
            T result = SimpleIndexing.Fourth<T>(me);
            return result;
            // TODO: add assertions to method SimpleIndexingTest.Fourth(IEnumerable`1<!!0>)
        }

        /// <summary>Test stub for Ninth(IEnumerable`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod, PexAllowedException(typeof(ArgumentOutOfRangeException))]
        public T Ninth<T>(IEnumerable<T> me)
        {
            T result = SimpleIndexing.Ninth<T>(me);
            return result;
            // TODO: add assertions to method SimpleIndexingTest.Ninth(IEnumerable`1<!!0>)
        }

        /// <summary>Test stub for Second(IEnumerable`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod, PexAllowedException(typeof(ArgumentOutOfRangeException))]
        public T Second<T>(IEnumerable<T> me)
        {
            T result = SimpleIndexing.Second<T>(me);
            return result;
            // TODO: add assertions to method SimpleIndexingTest.Second(IEnumerable`1<!!0>)
        }

        /// <summary>Test stub for Seventh(IEnumerable`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod, PexAllowedException(typeof(ArgumentOutOfRangeException))]
        public T Seventh<T>(IEnumerable<T> me)
        {
            T result = SimpleIndexing.Seventh<T>(me);
            return result;
            // TODO: add assertions to method SimpleIndexingTest.Seventh(IEnumerable`1<!!0>)
        }

        /// <summary>Test stub for Sixth(IEnumerable`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod, PexAllowedException(typeof(ArgumentOutOfRangeException))]
        public T Sixth<T>(IEnumerable<T> me)
        {
            T result = SimpleIndexing.Sixth<T>(me);
            return result;
            // TODO: add assertions to method SimpleIndexingTest.Sixth(IEnumerable`1<!!0>)
        }

        /// <summary>Test stub for Third(IEnumerable`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod, PexAllowedException(typeof(ArgumentOutOfRangeException))]
        public T Third<T>(IEnumerable<T> me)
        {
            T result = SimpleIndexing.Third<T>(me);
            return result;
            // TODO: add assertions to method SimpleIndexingTest.Third(IEnumerable`1<!!0>)
        }
    }
}
