namespace System.Configuration.Abstractions
{
    public interface IConvertType
    {
        Type TargetType { get; }
        object Convert(string configurationValue);
    }
}