// <copyright file="LinqExtensionsTest.cs" company="Microsoft">Copyright © Microsoft 2009</copyright>
using System;
using System.Collections.Generic;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Void.Linq;

namespace Void.Linq
{
    /// <summary>This class contains parameterized unit tests for LinqExtensions</summary>
    [PexClass(typeof(LinqExtensions))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class LinqExtensionsTest
    {
        /// <summary>Test stub for Append(IEnumerable`1&lt;!!0&gt;, !!0[])</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public IEnumerable<T> Append<T>(IEnumerable<T> source, T[] instances)
        {
            IEnumerable<T> result = LinqExtensions.Append<T>(source, instances);
            return result;
            // TODO: add assertions to method LinqExtensionsTest.Append(IEnumerable`1<!!0>, !!0[])
        }

        /// <summary>Test stub for ChopIntoSizesOf(IEnumerable`1&lt;!!0&gt;, Int32)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public IEnumerable<IEnumerable<T>> ChopIntoSizesOf<T>(IEnumerable<T> me, int size)
        {
            IEnumerable<IEnumerable<T>> result = LinqExtensions.ChopIntoSizesOf<T>(me, size)
              ;
            return result;
            // TODO: add assertions to method LinqExtensionsTest.ChopIntoSizesOf(IEnumerable`1<!!0>, Int32)
        }

        /// <summary>Test stub for Flatten(IEnumerable`1&lt;!!0&gt;)</summary>
        [PexMethod]
        public IEnumerable<TChild> Flatten<T,TChild>(IEnumerable<T> me)
            where T : IEnumerable<TChild>
        {
            IEnumerable<TChild> result = LinqExtensions.Flatten<T, TChild>(me);
            return result;
            // TODO: add assertions to method LinqExtensionsTest.Flatten(IEnumerable`1<!!0>)
        }

        /// <summary>Test stub for Let(IEnumerable`1&lt;!!0&gt;, Func`2&lt;IEnumerable`1&lt;!!0&gt;,IEnumerable`1&lt;!!1&gt;&gt;)</summary>
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public IEnumerable<TResult> Let<TSource,TResult>(
            IEnumerable<TSource> me,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> selector
        )
        {
            IEnumerable<TResult> result = LinqExtensions.Let<TSource, TResult>(me, selector)
              ;
            return result;
            // TODO: add assertions to method LinqExtensionsTest.Let(IEnumerable`1<!!0>, Func`2<IEnumerable`1<!!0>,IEnumerable`1<!!1>>)
        }

        /// <summary>Test stub for None(IEnumerable`1&lt;!!0&gt;, Func`2&lt;!!0,Boolean&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public bool None<T>(IEnumerable<T> me, Func<T, bool> predicate)
        {
            bool result = LinqExtensions.None<T>(me, predicate);
            return result;
            // TODO: add assertions to method LinqExtensionsTest.None(IEnumerable`1<!!0>, Func`2<!!0,Boolean>)
        }
    }
}
