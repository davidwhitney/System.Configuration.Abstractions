namespace System.Configuration.Abstractions.TypeConverters
{
    public class UriConverter : IConvertType
    {
        public Type TargetType => typeof (Uri);

        public object Convert(string configurationValue)
        {
            return new Uri(configurationValue);
        }
    }
}