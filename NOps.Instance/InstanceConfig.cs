using System;
using System.Runtime.Serialization;

using NOps.Common;

namespace NOps.Instance
{
	public abstract class InstanceConfig : Config
	{
		public static T LoadSiteConfig<T>(string siteName) where T: InstanceConfig
		{
			var entry = InstanceRegistry.Current.GetBySiteName(siteName);
			
			if (entry == null) throw new Exception($"Can't find an instance for site '{siteName}' in the registry.");

			var config = Load<T>(entry);

			return config;
		}

		private static T Load<T>(InstanceEntry entry) where T : InstanceConfig
		{
			var config = Load<T>(entry.Config);
			config.Instance = entry;
			return config;
		}

		public static T LoadConfig<T>(string instanceName) where T : InstanceConfig
		{
			var entry = InstanceRegistry.Current.Get(instanceName);

			if (entry == null) throw new Exception($"Can't find an the instance '{instanceName}' in the registry.");

			var config = Load<T>(entry);

			return config;
		}
		
		[IgnoreDataMember]
		public InstanceEntry Instance { get; private set; }
	}
}
