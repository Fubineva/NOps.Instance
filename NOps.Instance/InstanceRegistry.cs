using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;

namespace Fubineva.NOps.Instance
{
    [XmlRoot("InstanceRegistry")]
    public class InstanceRegistry : List<InstanceEntry>
    {
        private static Lazy<InstanceRegistry> s_current
            = new Lazy<InstanceRegistry>(() => Load(DetermineFilePathName()), LazyThreadSafetyMode.ExecutionAndPublication);

        private static string DetermineFilePathName()
        {
            var filePathName = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, @"..\..", "InstanceRegistry.cfg");

            if (File.Exists(filePathName))
            {
                return filePathName;
            }

            //filePathName = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, @"..\..\..", "InstanceRegistry.cfg");

            if (!File.Exists(filePathName))
            {
                throw new FileNotFoundException("Can't find InstanceRegistry.cfg");
            }

            return filePathName;
        }

        [XmlIgnore]
        public string FilePathName { get; set; }

        public static InstanceRegistry Current
        {
            get { return s_current.Value; }
            set { s_current = new Lazy<InstanceRegistry>(() => value); }
        }

        public static InstanceRegistry Load(string filePathName)
        {
            InstanceRegistry instanceRegistry;
            using (var fileStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var configReader = XmlReader.Create(fileStream))
            {
                var deserializer = new XmlSerializer(typeof(InstanceRegistry));
                try
                {
                    instanceRegistry = (InstanceRegistry)deserializer.Deserialize(configReader);
                }
                catch (XmlException ex)
                {

                    throw new Exception(string.Format("The instances configuration file {0} contains invalid Xml.", filePathName), ex);
                }

                configReader.Close();
                fileStream.Close();
            }

            instanceRegistry.FilePathName = filePathName;
            return instanceRegistry;
        }

        public void Save()
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
            };

            using (var fileStream = new FileStream(FilePathName, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = XmlWriter.Create(fileStream, xmlWriterSettings))
            {
                var serializer = new XmlSerializer(typeof(InstanceRegistry));
                serializer.Serialize(writer, this);
                writer.Flush();
                writer.Close();
                fileStream.Close();
            }
        }

        public string InstanceConfigAbsoluteFilePathName(InstanceEntry currentInstance)
        {
            var instanceConfigFilePathName = currentInstance.Config;

            if (!Path.IsPathRooted(instanceConfigFilePathName))
            {
                instanceConfigFilePathName = Path.Combine(Path.GetDirectoryName(FilePathName), instanceConfigFilePathName);
            }
            return instanceConfigFilePathName;
        }

        public InstanceEntry ResolveInstance(string instanceOrSiteName)
        {
            var currentInstance = Get(instanceOrSiteName) ?? GetBySiteName(instanceOrSiteName);
            if (currentInstance == null)
            {
                throw new Exception("Can't resolve instance configuration for application " + instanceOrSiteName);
            }
            return currentInstance;
        }

        public InstanceEntry Get(string instanceName)
        {
            var currentInstance =
                this.SingleOrDefault(i => string.Equals(i.Name, instanceName, StringComparison.OrdinalIgnoreCase));

            return currentInstance;
        }

        public InstanceEntry GetBySiteName(string siteName)
        {
            var currentInstance =
                this.SingleOrDefault(i => string.Equals(i.SiteName, siteName, StringComparison.OrdinalIgnoreCase));

            return currentInstance;
        }

        public void Add(string instanceName, string siteName, string configFilePathName, string version)
        {
            var instance = new InstanceEntry(instanceName, siteName, configFilePathName, version);

            this.Add(instance);
        }

    }
}
