using System.Collections;
using Microsoft.SharePoint;

namespace SPEllex.SharePoint
{
    public static class ListExtension
    {
        public static SPForm GetForm(this SPList list, PAGETYPE pageType)
        {
            SPForm form = null;
            var enumerator = list.Forms.GetEnumerator();
            SPForm current;
            while (enumerator.MoveNext())
            {
                current = (SPForm) enumerator.Current;
                if (current.Type == pageType)
                {
                    goto Success;
                }
            }
            return null;
            Success:
            form = current;

            return form;
        }

        public static string GetFullUrl(this SPList list)
        {
            return string.Format("{0}/{1}", list.ParentWeb.Url, list.RootFolder.Url);
        }

        public static string GetRelativeUrl(this SPList list)
        {
            string serverRelativeUrl = list.ParentWeb.ServerRelativeUrl;
            if (serverRelativeUrl == "/")
            {
                serverRelativeUrl = string.Empty;
            }
            return string.Format("{0}/{1}", serverRelativeUrl, list.RootFolder.Url);
        }
    }
}