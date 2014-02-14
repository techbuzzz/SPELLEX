using System;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace SPEllex.SharePoint
{
    public static class ListCollectionExtension
    {
        public static bool Contains(this SPListCollection lists, Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("id");
            }
            var typedEnumerator = lists.GetTypedEnumerator<SPList>();
            while (typedEnumerator.MoveNext())
            {
                if (typedEnumerator.Current.ID == id)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Contains(this SPListCollection lists, string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException("title");
            }
            var typedEnumerator = lists.GetTypedEnumerator<SPList>();
            while (typedEnumerator.MoveNext())
            {
                if (typedEnumerator.Current.Title == title)
                {
                    return true;
                }
            }
            return false;
        }
    }
}