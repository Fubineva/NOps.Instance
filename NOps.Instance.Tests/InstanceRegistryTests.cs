using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Fubineva.NOps.Instance;

using NUnit.Framework;

namespace NOps.Instance.Tests
{
    [TestFixture]
    public class InstanceRegistryTests
    {
        [Test]
        public void NoRegistryFile_should_throw()
        {
            // arrange
            
            // act
            try
            {
                var result = InstanceRegistry.Current;
            }
            catch (Exception ex)
            {
                var expectedPathString = Path.GetFullPath(Path.Combine(GetAppDir(), "..\\..\\.."));
                Assert.That(ex.Message.Contains(expectedPathString), "Expected a path in the exception message: " + ex.Message);
                return;
            }

            // assert
            Assert.Fail("Expected an exception.");
        }

        private static string GetAppDir()
        {
            var dir = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            return Path.GetDirectoryName(dir);
        }
    }
}
