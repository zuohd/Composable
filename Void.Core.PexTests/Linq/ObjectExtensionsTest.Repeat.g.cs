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
using System.Collections.Generic;
using Microsoft.Pex.Framework;
using NUnit.Framework;
using Microsoft.Pex.Framework.Generated;

namespace Void.Linq
{
    public partial class ObjectExtensionsTest
    {
[Test]
[PexGeneratedBy(typeof(ObjectExtensionsTest))]
public void Repeat01()
{
    IEnumerable<int> iEnumerable;
    iEnumerable = this.Repeat<int>(0, 0);
    PexAssert.IsNotNull((object)iEnumerable);
}
    }
}
