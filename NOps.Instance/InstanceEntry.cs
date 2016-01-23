using System;

namespace NOps.Instance
{
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
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(siteName))
            {
                throw new ArgumentNullException(nameof(siteName));
            }
            if (string.IsNullOrWhiteSpace(configFilePathName))
            {
                throw new ArgumentNullException(nameof(configFilePathName));
            }
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            Name = name;
            SiteName = siteName;
            Config = configFilePathName;
            Version = version;
        }

        public string QualityTag;

        public string Name;

        public string SiteName;

        public string Config;

        public string Version;
        
    }
}
