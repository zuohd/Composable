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
    public partial class IterationTest
    {
[Test]
[PexGeneratedBy(typeof(IterationTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void ForEach0101()
{
    try
    {
      if (!PexContract.HasRequiredRuntimeContracts(typeof(Iteration), (PexRuntimeContractsFlags)4223))
        PexAssert.Inconclusive("assembly Void.Core is not instrumented with runtime contracts");
      this.ForEach01<int>((IEnumerable<int>)null, (Action<int>)null);
      throw new AssertFailedException();
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[Test]
[PexGeneratedBy(typeof(IterationTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void ForEach0102()
{
    try
    {
      if (!PexContract.HasRequiredRuntimeContracts(typeof(Iteration), (PexRuntimeContractsFlags)4223))
        PexAssert.Inconclusive("assembly Void.Core is not instrumented with runtime contracts");
      int[] ints = new int[0];
      this.ForEach01<int>((IEnumerable<int>)ints, (Action<int>)null);
      throw new AssertFailedException();
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[Test]
[PexGeneratedBy(typeof(IterationTest))]
public void ForEach0103()
{
    int[] ints = new int[0];
    this.ForEach01<int>((IEnumerable<int>)ints, PexChoose.CreateDelegate<Action<int>>());
}
[Test]
[PexGeneratedBy(typeof(IterationTest))]
public void ForEach0104()
{
    int[] ints = new int[1];
    this.ForEach01<int>((IEnumerable<int>)ints, PexChoose.CreateDelegate<Action<int>>());
}
[Test]
[PexGeneratedBy(typeof(IterationTest))]
public void ForEach0105()
{
    int[] ints = new int[2];
    this.ForEach01<int>((IEnumerable<int>)ints, PexChoose.CreateDelegate<Action<int>>());
}
[Test]
[PexGeneratedBy(typeof(IterationTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void ForEach03()
{
    try
    {
      if (!PexContract.HasRequiredRuntimeContracts(typeof(Iteration), (PexRuntimeContractsFlags)4223))
        PexAssert.Inconclusive("assembly Void.Core is not instrumented with runtime contracts");
      this.ForEach<int, int>((IEnumerable<int>)null, (Func<int, int>)null);
      throw new AssertFailedException();
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[Test]
[PexGeneratedBy(typeof(IterationTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void ForEach04()
{
    try
    {
      if (!PexContract.HasRequiredRuntimeContracts(typeof(Iteration), (PexRuntimeContractsFlags)4223))
        PexAssert.Inconclusive("assembly Void.Core is not instrumented with runtime contracts");
      int[] ints = new int[0];
      this.ForEach<int, int>((IEnumerable<int>)ints, (Func<int, int>)null);
      throw new AssertFailedException();
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[Test]
[PexGeneratedBy(typeof(IterationTest))]
public void ForEach05()
{
    int[] ints = new int[0];
    this.ForEach<int, int>((IEnumerable<int>)ints, PexChoose.CreateDelegate<Func<int, int>>());
}
[Test]
[PexGeneratedBy(typeof(IterationTest))]
public void ForEach06()
{
    int[] ints = new int[1];
    this.ForEach<int, int>((IEnumerable<int>)ints, PexChoose.CreateDelegate<Func<int, int>>());
}
[Test]
[PexGeneratedBy(typeof(IterationTest))]
public void ForEach07()
{
    IPexChoiceRecorder choices = PexChoose.NewTest();
    int[] ints = new int[2];
    this.ForEach<int, int>((IEnumerable<int>)ints, PexChoose.CreateDelegate<Func<int, int>>());
}
    }
}
