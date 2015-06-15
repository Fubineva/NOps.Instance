using System;
using System.Xml.Serialization;

namespace Fubineva.NOps.Instance
{
    [XmlRoot("InstanceEntry")]
    public class InstanceEntry
    {
        private InstanceEntry()
        {
            ;
        }

        public InstanceEntry(string name, string siteName, string configFilePathName, string version)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrWhiteSpace(siteName))
            {
                throw new ArgumentNullException("siteName");
            }
            if (string.IsNullOrWhiteSpace(configFilePathName))
            {
                throw new ArgumentNullException("configFilePathName");
            }
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException("version");
            }

            Name = name;
            SiteName = siteName;
            Config = configFilePathName;
            Version = version;
        }

        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string SiteName;

        [XmlAttribute]
        public string Config;

        [XmlAttribute]
        public string Version;
        
    }
}
