using System;
using System.IO;

namespace HearthstoneDisunity.Unity
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