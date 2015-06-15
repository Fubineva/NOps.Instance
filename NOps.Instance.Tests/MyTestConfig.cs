using Fubineva.NOps.Instance;

namespace NOps.Instance.Tests
{
    public class MyTestConfig : Config
    {
        public string MyStringValue;

        public long MyNumbericValue;

        public MyNestedTestConfig NestedConfig;
    }

    public class MyNestedTestConfig
    {
        public string NestedValue;
    }
}