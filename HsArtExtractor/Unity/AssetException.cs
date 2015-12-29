using System;
using System.IO;

namespace HsArtExtractor.Unity
{
    public class AssetException : IOException
    {
        public AssetException()
        {
        }

        public AssetException(String msg) : base(msg)
        {
        }
    }
}