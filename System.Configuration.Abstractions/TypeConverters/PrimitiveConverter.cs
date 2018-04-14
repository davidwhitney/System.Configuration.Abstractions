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
            if (TargetType.IsEnum)
            {
                return Enum.Parse(TargetType, configurationValue, true);
            }

            return System.Convert.ChangeType(configurationValue, TargetType);
        }
    }
}