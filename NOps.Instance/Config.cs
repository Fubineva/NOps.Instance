using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Fubineva.NOps.Instance
{
    public abstract class Config
    {
        private static readonly IConfigLoader s_cfgLoader = new ConfigLoader();

        public static T Load<T>(string filePathName) where T : Config
        {
            var config = s_cfgLoader.Load<T>(filePathName);
            config.FilePathName = filePathName;
            return config;
        }
        
        [Obsolete("Use the ConfigLoader class.")]
        public static T Load<T>(Stream stream) where T : Config
        {
            return s_cfgLoader.Load<T>(stream);
        }
        
        [IgnoreDataMember]
        [XmlIgnore] // ToDo: remove this along side the LegacyXml feature
        public string FilePathName { get; private set; }

        public void Save(string filePathName)
        {
            FilePathName = filePathName;

            s_cfgLoader.Save(this, filePathName);
        }
        
        public void Save()
        {
            if (string.IsNullOrEmpty(FilePathName))
            {
                throw new ApplicationException("This configuration has no FilePathName set, please set it or specify one explicitly.");
            }

            Save(FilePathName);
        }
        
    }
}