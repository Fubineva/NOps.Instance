using System;
using System.Xml.Serialization;

namespace Fubineva.NOps.Instance
{
	public abstract class InstanceConfig : Config
	{
		public static T LoadSiteConfig<T>(string siteName) where T: InstanceConfig
		{
			var entry = InstanceRegistry.Current.GetBySiteName(siteName);
			
			if (entry == null) throw new Exception($"Can't find an instance for site {siteName}.");

			var config = Load<T>(entry.Config);
			config.Instance = entry;

			return config;
		}

		[XmlIgnore]
		public InstanceEntry Instance { get; private set; }
	}
}
