// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>
using System;
using Microsoft.Pex.Framework.Generated;
using Microsoft.Pex.Framework;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.Pex.Engine.Exceptions;

namespace Void.Linq
{
    public partial class SimpleIndexingTest
    {
[Test]
[PexGeneratedBy(typeof(SimpleIndexingTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void Seventh01()
{
    try
    {
      if (!PexContract.HasRequiredRuntimeContracts(typeof(SimpleIndexing), (PexRuntimeContractsFlags)4223))
        PexAssert.Inconclusive("assembly Void.Core is not instrumented with runtime contracts");
      int i;
      i = this.Seventh<int>((IEnumerable<int>)null);
      throw new AssertFailedException();
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[Test]
[PexGeneratedBy(typeof(SimpleIndexingTest))]
[ExpectedException(typeof(ArgumentOutOfRangeException))]
public void Seventh02()
{
    int i;
    int[] ints = new int[0];
    i = this.Seventh<int>((IEnumerable<int>)ints);
}
[Test]
[PexGeneratedBy(typeof(SimpleIndexingTest))]
public void Seventh03()
{
    int i;
    int[] ints = new int[7];
    i = this.Seventh<int>((IEnumerable<int>)ints);
    PexAssert.AreEqual<int>(0, i);
}
    }
}
