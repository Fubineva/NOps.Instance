using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Fubineva.NOps.Instance
{
    public abstract class InstanceConfig
    {
        public static T LoadSiteConfig<T>(string siteName) where T: InstanceConfig
        {
            var filePathName = InstanceRegistry.GetSiteConfigFilePathName(siteName);
            var config = Load<T>(filePathName);

            return config;
        }

        public static T Load<T>(string filePathName) where T : InstanceConfig
        {
            T config;
            using (var fileStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var configReader = XmlReader.Create(fileStream))
            {
                var deserializer = new XmlSerializer(typeof(T));
                config = (T)deserializer.Deserialize(configReader);

                configReader.Close();
                fileStream.Close();
            }

            config.FilePathName = filePathName;

            return config;
        }
        
        [XmlIgnore]
        public string FilePathName { get; set; }

        public void Save(string filePathName)
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
            };

            using (var fileStream = new FileStream(filePathName, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = XmlWriter.Create(fileStream, xmlWriterSettings))
            {
                var serializer = new XmlSerializer(GetType());
                serializer.Serialize(writer, this);
            }
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
