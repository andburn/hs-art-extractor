using System;

namespace HearthstoneDisunity.Image
{
    public class TextureException : Exception
    {
        public TextureException()
        {
        }

        public TextureException(String msg) : base(msg)
        {
        }
    }
}