using HsArtExtractor.Util;

namespace HsArtExtractor.Unity.Extract
{
    internal interface IExtractor
    {
        void Extract(ObjectData data, string dir);
    }
}