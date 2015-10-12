using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Fubineva.NOps.Instance
{
    public abstract class Config
    {
        // ToDo: make this baseclass optional
        public static T Load<T>(string filePathName) where T : Config
        {
            T config;
            using (var fileStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                config = Load<T>(fileStream);
            }
            config.FilePathName = filePathName;
            return config;
        }

        public static T Load<T>(Stream stream) where T : Config
        {
            T config;
            using (var configReader = XmlReader.Create(stream))
            {
                var deserializer = new XmlSerializer(typeof(T));
                config = (T)deserializer.Deserialize(configReader);

                configReader.Close();
                stream.Close();
            }

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