using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

using Newtonsoft.Json;

using Formatting = Newtonsoft.Json.Formatting;

namespace Fubineva.NOps.Instance
{
    public abstract class Config
    {
        private static readonly JsonSerializerSettings s_serializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        // ToDo: make this baseclass optional
        public static T Load<T>(string filePathName) where T : Config
        {
            T config;
            using (var fileStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {

                try
                {
                    config = Load<T>(fileStream);
                    config.FilePathName = filePathName;
                    return config;
                }
                catch (JsonReaderException)
                {
                    // if we fail loading the file, attempt loading it as XML then save it back as json.
                }
            }

            using (var fileStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                config = LoadLegacyXml<T>(fileStream);
                config.FilePathName = filePathName;

                fileStream.Close();
                config.Save(filePathName);
            }
            return config;
        }

        public static T Load<T>(Stream stream) where T : Config
        {
            using (var textReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(textReader))
            {
                var deserializer = JsonSerializer.Create(s_serializerSettings);
                return deserializer.Deserialize<T>(reader);
            }
        }

        [Obsolete]
        private static T LoadLegacyXml<T>(Stream stream) where T : Config
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

        [IgnoreDataMember]
        [XmlIgnore] // ToDo: remove this along side the LegacyXml feature
        public string FilePathName { get; protected set; }

        public void Save(string filePathName)
        {
            var serializer = JsonSerializer.Create(s_serializerSettings);
            using (var stream = new FileStream(filePathName, FileMode.Create))
            using(var writer = new StreamWriter(stream))
            {
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