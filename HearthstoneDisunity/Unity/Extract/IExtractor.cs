using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity.Extract
{
    internal interface IExtractor
    {
        void Extract(ObjectData data, string dir);
    }
}