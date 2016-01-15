using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace NOps.Instance.Tests
{
    [TestFixture]
    public class InstanceRegistryTests
    {
        // ToDo: Test this some other way...
        [Test]
        public void NoRegistryFile_should_throw()
        {
            // arrange
            
            // act
            try
            {
                var result = InstanceRegistry.Current;
            }
            catch (FileNotFoundException ex)
            {
                var expectedPathString = Path.GetFullPath(Path.Combine(GetAppDir(), "..\\..\\.."));
                
                Assert.That(ex.FileName.Contains(expectedPathString), "Expected on the exception: " + ex.FileName);
                return;
            }

            // assert
            Assert.Fail("Expected an exception.");
        }

        [Test]
        public void Persist()
        {
            // arrange
            var cfgFilePathName = Path.Combine(GetAppDir(), "InstanceRegistryPersistanceTest.cfg");
            var instanceRegistry = new InstanceRegistry
                {
                    {
                        "test", "some sitename", "some config path", "some version"
                    },
                    new InstanceEntry("test2", "some sitename", "some config path", "some version")
                };

            // act
            instanceRegistry.Save(cfgFilePathName);
            var result = InstanceRegistry.Load(cfgFilePathName);

            // assert
            Assert.AreEqual("test", result[0].Name);
            Assert.AreEqual("test2", result[1].Name);
        }

        private static string GetAppDir()
        {
            var dir = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            return Path.GetDirectoryName(dir);
        }
    }
}
