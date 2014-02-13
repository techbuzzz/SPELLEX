using System;
using Microsoft.SharePoint;

namespace SPEllex.Exceptions
{
    public class FileLockedException : Exception
    {
        public FileLockedException(SPFile file)
            : base(
                string.Format("File is locked by user '{0}'. Url: {1}",
                    (file.LockedByUser != null) ? file.LockedByUser.Name : "n/a", file.Url))
        {
        }

        public FileLockedException(string msg)
            : base(msg)
        {
        }
    }
}