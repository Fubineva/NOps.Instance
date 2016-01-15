using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using NUnit.Framework;

namespace NOps.Instance.Tests
{
    [TestFixture]
    public class LegacyXmlConfigAutoConversionTests
    {
        private string _legacyCfg;

        [SetUp]
        public void Setup()
        {
            var appDir = TestHelpers.GetAppDir();
            var legacyConfigSource = Path.Combine(appDir, "LegacyXml.cfg");

            // copy the test file so we can reuse this test without recompiling
            _legacyCfg = Path.Combine(appDir, "LegacyXml-copy.cfg");
            File.Copy(legacyConfigSource, _legacyCfg, true);
        }

        [Test]
        public void TestXmlConfig()
        {
            // arrange

            // act
            var myCfg = Config.Load<MyLegacyConfig>(_legacyCfg);

            // assert
            Assert.AreEqual("Hi!", myCfg.MyProp);

            AssertFileIsJson(_legacyCfg);
        }

        private void AssertFileIsJson(string filePathName)
        {
            using (var stream = new FileStream(filePathName, FileMode.Open, FileAccess.Read))
            using (var textReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(textReader))
            {
                var read = reader.Read();
                Assert.IsTrue(read, "reading from the config failed!");
                Assert.That(reader.TokenType == JsonToken.StartObject, "This file doesn't seem to be JSON.");
            }
        }
    }

    public class MyLegacyConfig : Config
    {
        public string MyProp { get; set; }
    }
}
