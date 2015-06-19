namespace Fubineva.NOps.Instance
{
    public abstract class InstanceConfig : Config
    {
        public static T LoadSiteConfig<T>(string siteName) where T: InstanceConfig
        {
            var filePathName = InstanceRegistry.GetSiteConfigFilePathName(siteName);
            var config = Load<T>(filePathName);

            return config;
        }

    }
}
