// <copyright file="ZippingPairTT2Test.cs" company="Microsoft">Copyright © Microsoft 2009</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Void.Linq;

namespace Void.Linq
{
    /// <summary>This class contains parameterized unit tests for Pair`2</summary>
    [PexClass(typeof(Zipping.Pair<, >))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class ZippingPairTT2Test
    {
        /// <summary>Test stub for .ctor(!0, !1)</summary>
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public Zipping.Pair<T, T2> Constructor<T,T2>(T first, T2 second)
        {
            Zipping.Pair<T, T2> target = new Zipping.Pair<T, T2>(first, second);
            return target;
            // TODO: add assertions to method ZippingPairTT2Test.Constructor(!!0, !!1)
        }

        /// <summary>Test stub for First</summary>
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void FirstGetSet<T,T2>([PexAssumeUnderTest]Zipping.Pair<T, T2> target, T value)
        {
            target.First = value;
            T result = target.First;
            // TODO: add assertions to method ZippingPairTT2Test.FirstGetSet(Pair`2<!!0,!!1>, !!0)
        }

        /// <summary>Test stub for Second</summary>
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void SecondGetSet<T,T2>([PexAssumeUnderTest]Zipping.Pair<T, T2> target, T2 value)
        {
            target.Second = value;
            T2 result = target.Second;
            // TODO: add assertions to method ZippingPairTT2Test.SecondGetSet(Pair`2<!!0,!!1>, !!1)
        }

        /// <summary>Test stub for ToString()</summary>
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public string ToString<T,T2>([PexAssumeUnderTest]Zipping.Pair<T, T2> target)
        {
            string result = target.ToString();
            return result;
            // TODO: add assertions to method ZippingPairTT2Test.ToString(Pair`2<!!0,!!1>)
        }
    }
}
