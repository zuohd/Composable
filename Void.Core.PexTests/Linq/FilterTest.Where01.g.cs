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
using System.Linq;
using NUnit.Framework;
using Microsoft.Pex.Engine.Exceptions;
using System.Collections.Generic;

namespace Void.Linq
{
    public partial class FilterTest
    {
[Test]
[PexGeneratedBy(typeof(FilterTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void Where0101()
{
    try
    {
      if (!PexContract.HasRequiredRuntimeContracts(typeof(Filter), (PexRuntimeContractsFlags)4223))
        PexAssert.Inconclusive("assembly Void.Core is not instrumented with runtime contracts");
      IQueryable<int> iQueryable;
      iQueryable = this.Where01<int>((IQueryable<int>)null, (IFilter<int>)null);
      throw new AssertFailedException();
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[Test]
[PexGeneratedBy(typeof(FilterTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void Where02()
{
    try
    {
      if (!PexContract.HasRequiredRuntimeContracts(typeof(Filter), (PexRuntimeContractsFlags)4223))
        PexAssert.Inconclusive("assembly Void.Core is not instrumented with runtime contracts");
      IEnumerable<int> iEnumerable;
      iEnumerable = this.Where<int>((IEnumerable<int>)null, (IFilter<int>)null);
      throw new AssertFailedException();
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[Test]
[PexGeneratedBy(typeof(FilterTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void Where03()
{
    try
    {
      if (!PexContract.HasRequiredRuntimeContracts(typeof(Filter), (PexRuntimeContractsFlags)4223))
        PexAssert.Inconclusive("assembly Void.Core is not instrumented with runtime contracts");
      IEnumerable<int> iEnumerable;
      int[] ints = new int[0];
      iEnumerable = this.Where<int>((IEnumerable<int>)ints, (IFilter<int>)null);
      throw new AssertFailedException();
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
    }
}
