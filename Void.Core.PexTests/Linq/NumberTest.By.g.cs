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
using Microsoft.Pex.Framework;
using NUnit.Framework;
using Microsoft.Pex.Framework.Generated;

namespace Void.Linq
{
    public partial class NumberTest
    {
[Test]
[PexGeneratedBy(typeof(NumberTest))]
public void By01()
{
    Number.IterationSpecification iterationSpecification;
    iterationSpecification = this.By(0, 0);
    PexAssert.AreEqual<int>(0, iterationSpecification.StartValue);
    PexAssert.AreEqual<int>(0, iterationSpecification.StepSize);
}
    }
}
