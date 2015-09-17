namespace System.Configuration.Abstractions.TypeConverters
{
    public class UriConverter : IConvertType
    {
        public Type TargetType { get { return typeof (Uri); } }

        public object Convert(string configurationValue)
        {
            return new Uri(configurationValue);
        }
    }
}