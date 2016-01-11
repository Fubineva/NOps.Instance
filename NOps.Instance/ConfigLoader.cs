using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using Newtonsoft.Json;

using Formatting = Newtonsoft.Json.Formatting;

namespace Fubineva.NOps.Instance
{
    public class ConfigLoader : IConfigLoader
    {
        private static readonly JsonSerializerSettings s_serializerSettings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Include,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                };

        public T Load<T>(string filePathName)
        {
            T config;
            using (var fileStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    config = Load<T>(fileStream);

                    var iConfig = config as IConfig;
                    iConfig?.SetFilePathName(filePathName);

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

                fileStream.Close();
                Save(config, filePathName);
            }

            return config;
        }

        public T Load<T>(Stream stream)
        {
            using (var textReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(textReader))
            {
                var deserializer = JsonSerializer.Create(s_serializerSettings);
                var config = deserializer.Deserialize<T>(reader);
                
                return config;
            }
        }

        [Obsolete]
        private static T LoadLegacyXml<T>(Stream stream)
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

        public void Save<T>(T config, string filePathName)
        {
            var iConfig = config as IConfig;
            iConfig?.SetFilePathName(filePathName);

            var serializer = JsonSerializer.Create(s_serializerSettings);
            using (var stream = new FileStream(filePathName, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            {
                serializer.Serialize(writer, config);
            }
        }
    }
}
