namespace System.Configuration.Abstractions.TypeConverters
{
    public class PrimitiveConverter : IConvertType
    {
        public Type TargetType { get; private set; }

        public PrimitiveConverter(Type targetType)
        {
            TargetType = targetType;
        }

        public object Convert(string configurationValue)
        {
            return System.Convert.ChangeType(configurationValue, TargetType);
        }
    }
}