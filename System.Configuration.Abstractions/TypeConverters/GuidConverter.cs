namespace System.Configuration.Abstractions.TypeConverters
{
    public class GuidConverter : IConvertType
    {
        public Type TargetType => typeof(Guid);

        public object Convert(string configurationValue)
        {
            return Guid.Parse(configurationValue);
        }
    }
}