namespace System.Configuration.Abstractions.TypeConverters
{
    public class PrimitiveConverter : IConvertType
    {
        public Type TargetType { get; }

        public PrimitiveConverter(Type targetType)
        {
            TargetType = targetType;
        }

        public object Convert(string configurationValue)
        {
            return TargetType.IsEnum
                ? Enum.Parse(TargetType, configurationValue, true)
                : System.Convert.ChangeType(configurationValue, TargetType);
        }
    }
}