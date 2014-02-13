using System.IO;
using System.Threading;
using Microsoft.SharePoint;
using SPEllex.Exceptions;

namespace SPEllex.SharePoint
{
    public static class FileExtension
    {
        public static Stream OpenBinaryStreamLocked(this SPFile file)
        {
            if (file.LockType != SPFile.SPLockType.Exclusive) return file.OpenBinaryStream();
            for (int i = 0; i < 30; i++)
            {
                if (file.LockType != SPFile.SPLockType.Exclusive)
                {
                    break;
                }
                Thread.Sleep(0x3e8);
                file = file.Web.GetFile(file.Url);
            }
            if (file.LockType != SPFile.SPLockType.Exclusive) return file.OpenBinaryStream();
            throw new FileLockedException(file);
        }
    }
}