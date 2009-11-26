// <copyright file="FilterTest.cs" company="Microsoft">Copyright © Microsoft 2009</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Void.Linq;

namespace Void.Linq
{
    /// <summary>This class contains parameterized unit tests for Filter</summary>
    [PexClass(typeof(Filter))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class FilterTest
    {
        /// <summary>Test stub for Matches(IFilter`1&lt;!!0&gt;, !!0)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public bool Matches<T>(IFilter<T> filter, T item)
        {
            bool result = Filter.Matches<T>(filter, item);
            return result;
            // TODO: add assertions to method FilterTest.Matches(IFilter`1<!!0>, !!0)
        }

        /// <summary>Test stub for Where(IEnumerable`1&lt;!!0&gt;, IFilter`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public IEnumerable<TItemType> Where<TItemType>(IEnumerable<TItemType> source, IFilter<TItemType> filter)
        {
            IEnumerable<TItemType> result = Filter.Where<TItemType>(source, filter);
            return result;
            // TODO: add assertions to method FilterTest.Where(IEnumerable`1<!!0>, IFilter`1<!!0>)
        }

        /// <summary>Test stub for Where(IQueryable`1&lt;!!0&gt;, IFilter`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public IQueryable<TItemType> Where01<TItemType>(IQueryable<TItemType> source, IFilter<TItemType> filter)
        {
            IQueryable<TItemType> result = Filter.Where<TItemType>(source, filter);
            return result;
            // TODO: add assertions to method FilterTest.Where01(IQueryable`1<!!0>, IFilter`1<!!0>)
        }
    }
}
