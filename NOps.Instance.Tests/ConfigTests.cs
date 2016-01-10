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
    public class ConfigTests
    {
        private string _testFilePathName;

        [SetUp]
        public void Setup()
        {
            _testFilePathName = Path.Combine(TestHelpers.GetAppDir(), "testfile.cfg");
            if (File.Exists(_testFilePathName))
            {
                File.Delete(_testFilePathName);
            }
        }

        [Test]
        public void Config_persistence()
        {
            // arrange
            var cfg = new MyTestConfig()
            {
                MyNumbericValue = 333,
                MyStringValue = "abcde",
                NestedConfig = new MyNestedTestConfig()
                {
                    NestedValue = "nested abcde"
                }
            };

            // act
            cfg.Save(_testFilePathName);

            // assert
            Assert.IsTrue(File.Exists(_testFilePathName));

            var loadedCfg = Config.Load<MyTestConfig>(_testFilePathName);
            Assert.AreEqual(cfg.MyStringValue, loadedCfg.MyStringValue);
            Assert.AreEqual(cfg.MyNumbericValue, loadedCfg.MyNumbericValue);
            Assert.AreEqual(cfg.NestedConfig.NestedValue, loadedCfg.NestedConfig.NestedValue);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void Save_on_new_config_without_filename_should_throw()
        {
            // arrange
            var cfg = new MyTestConfig();

            // act
            cfg.Save();

            // assert
        }

        [Test]
        public void After_load_the_config_knows_its_path()
        {
            // arrange
            var cfg = new MyTestConfig();
            cfg.Save(_testFilePathName);

            // act
            var loadedCfg = Config.Load<MyTestConfig>(_testFilePathName);

            // assert
            Assert.AreEqual(_testFilePathName, loadedCfg.FilePathName);
        }

        [Test]
        public void After_load_can_save_with_implicit_filepathname()
        {
            // arrange
            var cfg = new MyTestConfig();
            cfg.Save(_testFilePathName);
            var loadedCfg = Config.Load<MyTestConfig>(_testFilePathName);
            
            // act
            loadedCfg.Save();

            // assert
            Assert.Pass();
        }
    }

    
}
