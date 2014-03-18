namespace System.Configuration.Abstractions
{
    public interface IConnectionStrings
    {
        ConnectionStringSettings this[string name] { get; }
        ConnectionStringSettings this[int index] { get; }
        int IndexOf(ConnectionStringSettings settings);
        void Add(ConnectionStringSettings settings);
        void Remove(ConnectionStringSettings settings);
        void RemoveAt(int index);
        void Remove(string name);
        void Clear();
    }
}