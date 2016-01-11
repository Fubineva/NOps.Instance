using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

namespace Fubineva.NOps.Instance
{
	// ToDo: Inherit from HashSet and use IConfig + ConfigLoader (see Migrator registry as example)

	[XmlRoot("InstanceRegistry")]
	public class InstanceRegistry : Config, IList<InstanceEntry>
	{
		private static Lazy<InstanceRegistry> s_current
			= new Lazy<InstanceRegistry>(() => Config.Load<InstanceRegistry>(DetermineFilePathName()), LazyThreadSafetyMode.ExecutionAndPublication);

		private IList<InstanceEntry> _entries = new List<InstanceEntry>();

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

		public static InstanceRegistry Load(string filePathName)
		{
			return Config.Load<InstanceRegistry>(filePathName);
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
			return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
		}
		
		public static InstanceRegistry Current
		{
			get { return s_current.Value; }
			set { s_current = new Lazy<InstanceRegistry>(() => value); }
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

			if (currentInstance != null)
			{
				currentInstance.Config = GetConfigFilePathName(currentInstance);
			}
			return currentInstance;
		}

		public void Add(string instanceName, string siteName, string configFilePathName, string version)
		{
			var instance = new InstanceEntry(instanceName, siteName, configFilePathName, version);

			this.Add(instance);
		}

		// ToDo: can we do this internally in a clean way, without saving the result?
		private static string GetConfigFilePathName(InstanceEntry instanceEntry)
		{
			var filePathName = instanceEntry.Config;

			if (!Path.IsPathRooted(filePathName))
			{
				filePathName = Path.Combine(Path.GetDirectoryName(Current.FilePathName), filePathName);
			}
			
			return filePathName;
		}

		public static void Create<TCfg>(string instanceName, string siteName) where TCfg : InstanceConfig, new()
		{
			var instanceRegistry = Current;

			if (instanceRegistry.Exists(instanceName))
			{
				throw new Exception($"Instance name '{instanceName}' already in use. Can't create it.");
			}

			var configBaseDir = Path.GetDirectoryName(instanceRegistry.FilePathName);

			var instanceEntry = CreateInstance<TCfg>(instanceName, siteName, configBaseDir);

			instanceRegistry.Add(instanceEntry);
			instanceRegistry.Save();
		}

		private static InstanceEntry CreateInstance<TCfg>(string instanceName, string siteName, string configBaseDir) where TCfg : InstanceConfig, new()
		{
			const string instance_cfg_file_name = "Instance.cfg";

			var relativeConfigFilePathName = Path.Combine(instanceName, instance_cfg_file_name);

			var instanceEntry = new InstanceEntry(instanceName, siteName, relativeConfigFilePathName, "0.1");

			var instanceDir = CreateInstanceDirectory(instanceName, configBaseDir);
			var instanceCfgFilePathName = Path.Combine(instanceDir, instance_cfg_file_name);

			var cfg = new TCfg();
			cfg.Save(instanceCfgFilePathName);

			return instanceEntry;
		}

		private static string CreateInstanceDirectory(string instanceName, string configBaseDir)
		{
			var instanceDir = Path.Combine(configBaseDir, instanceName);
			Directory.CreateDirectory(instanceDir);
			return instanceDir;
		}

		public IEnumerator<InstanceEntry> GetEnumerator()
		{
			return _entries.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_entries).GetEnumerator();
		}

		public void Add(InstanceEntry item)
		{
			_entries.Add(item);
		}

		public void Clear()
		{
			_entries.Clear();
		}

		public bool Contains(InstanceEntry item)
		{
			return _entries.Contains(item);
		}

		public void CopyTo(InstanceEntry[] array, int arrayIndex)
		{
			_entries.CopyTo(array, arrayIndex);
		}

		public bool Remove(InstanceEntry item)
		{
			return _entries.Remove(item);
		}

		public int Count => _entries.Count;

		public bool IsReadOnly => _entries.IsReadOnly;

		public int IndexOf(InstanceEntry item)
		{
			return _entries.IndexOf(item);
		}

		public void Insert(int index, InstanceEntry item)
		{
			_entries.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_entries.RemoveAt(index);
		}

		public InstanceEntry this[int index]
		{
			get { return _entries[index]; }
			set { _entries[index] = value; }
		}
	}
}
