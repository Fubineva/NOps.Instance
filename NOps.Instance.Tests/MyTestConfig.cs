namespace NOps.Instance.Tests
{
    public class MyTestConfig : InstanceConfig
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