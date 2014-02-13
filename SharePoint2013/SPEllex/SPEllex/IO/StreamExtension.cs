using System;
using System.IO;

namespace SPEllex.IO
{
    public static class StreamExtension
    {
        public static void CopyTo(this Stream src, Stream dest)
        {
            int num2;
            int num = src.CanSeek ? Math.Min((int) (src.Length - src.Position), 0x1000) : 0x1000;
            var buffer = new byte[num];
            do
            {
                num2 = src.Read(buffer, 0, buffer.Length);
                dest.Write(buffer, 0, num2);
            } while (num2 != 0);
        }
    }
}