using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Fubineva.NOps.Instance
{
	public abstract class InstanceConfig : Config
	{
		// ToDo: make this baseclass optional

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

		[XmlIgnore] // ToDo: remove this along side the LegacyXml feature
		[IgnoreDataMember]
		public InstanceEntry Instance { get; private set; }
	}
}
