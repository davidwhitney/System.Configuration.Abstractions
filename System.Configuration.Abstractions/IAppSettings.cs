using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace System.Configuration.Abstractions
{
    public interface IAppSettings : IAppSettingsExtended
    {
        void Add(NameValueCollection c);
        void Clear();
        void CopyTo(Array dest, int index);
        bool HasKeys();
        void Add(string name, string value);
        string Get(string name);
        string[] GetValues(string name);
        void Set(string name, string value);
        void Remove(string name);
        string Get(int index);
        string[] GetValues(int index);
        string GetKey(int index);
        string this[string name] { get; set; }
        string this[int index] { get; }
        string[] AllKeys { get; }
        int Count { get; }
        NameObjectCollectionBase.KeysCollection Keys { get; }
        void GetObjectData(SerializationInfo info, StreamingContext context);
        void OnDeserialization(object sender);
        IEnumerator GetEnumerator();
    }
}