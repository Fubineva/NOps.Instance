using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
            var applicationPhysicalPath = ApplicationPhysicalPath();

            var filePathName = SeekFile(applicationPhysicalPath, "InstanceRegistry.cfg");

            if (filePathName == null)
            {
                throw new FileNotFoundException("Can't find InstanceRegistry.cfg on path.", applicationPhysicalPath);
            }

            return filePathName;
        }

        private static string SeekFile(string startPath, string fileName)
        {
            foreach (var dir in Directories(startPath))
            {
                var candidate = Path.Combine(dir, fileName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static IEnumerable<string> Directories(string dir)
        {
            while (dir != null)
            {
                yield return dir;
                var parent = GetParentDirectory(dir);
                // when we get to the root drive it keeps returning the same root drive
                if (dir == parent)
                {
                    yield break;
                }
                dir = parent;
            }
        }

        private static string GetParentDirectory(string dir)
        {
            return Path.GetFullPath(Path.Combine(dir, ".."));
        }

        private static string ApplicationPhysicalPath()
        {
            var path = HostingEnvironment.ApplicationPhysicalPath;

            if (path == null)
            {
                return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            }

            return path;
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
            if (string.IsNullOrEmpty(FilePathName))
            {
                throw new ApplicationException("No FilePathName set, set one of pass a filePathName explicitly.");
            }

            Save(FilePathName);
        }

        public void Save(string filePathName)
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
            };

            using (var fileStream = new FileStream(filePathName, FileMode.Create, FileAccess.Write, FileShare.None))
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

        public bool Exists(string instanceName)
        {
            return this.Any(i => string.Equals(i.Name, instanceName, StringComparison.OrdinalIgnoreCase));
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

        public static string GetConfigFilePathName()
        {
            var instanceConfig = Current.GetBySiteName(HostingEnvironment.SiteName);
            if (instanceConfig == null)
            {
                throw new ApplicationException("Can't find instance configuration for site: " + HostingEnvironment.SiteName);
            }

            var filePathName = instanceConfig.Config;
            return filePathName;
        }
    }
}
