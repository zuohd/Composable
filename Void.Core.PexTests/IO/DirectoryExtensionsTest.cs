// <copyright file="DirectoryExtensionsTest.cs" company="Microsoft">Copyright © Microsoft 2009</copyright>
using System;
using System.IO;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Void.IO;

namespace Void.IO
{
    /// <summary>This class contains parameterized unit tests for DirectoryExtensions</summary>
    [PexClass(typeof(DirectoryExtensions))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class DirectoryExtensionsTest
    {
        /// <summary>Test stub for AsDirectory(String)</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentException))]
        public DirectoryInfo AsDirectory(string path)
        {
            DirectoryInfo result = DirectoryExtensions.AsDirectory(path);
            return result;
            // TODO: add assertions to method DirectoryExtensionsTest.AsDirectory(String)
        }

        /// <summary>Test stub for DeleteRecursive(DirectoryInfo)</summary>
        [PexMethod]
        public void DeleteRecursive(DirectoryInfo me)
        {
            DirectoryExtensions.DeleteRecursive(me);
            // TODO: add assertions to method DirectoryExtensionsTest.DeleteRecursive(DirectoryInfo)
        }

        /// <summary>Test stub for Size(DirectoryInfo)</summary>
        [PexMethod]
        public long Size(DirectoryInfo me)
        {
            long result = DirectoryExtensions.Size(me);
            return result;
            // TODO: add assertions to method DirectoryExtensionsTest.Size(DirectoryInfo)
        }
    }
}
