using System;
using Microsoft.SharePoint;

namespace SPEllex.SharePoint
{
    public static class WebExtension
    {
        public static WebList GetSubwebs(this SPWeb web)
        {
            var list = new WebList();
            SPWebCollection subwebsForCurrentUser = web.GetSubwebsForCurrentUser();
            list.AddRange(subwebsForCurrentUser);
            return list;
        }

        public static string ReadProperty(this SPWeb web, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            string str = string.Empty;
            if (web.Properties.ContainsKey(key))
            {
                str = web.Properties[key];
            }
            return str;
        }

        public static void RemoveProperty(this SPWeb web, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            if ((SPContext.Current != null) && (SPContext.Current.Web != null))
            {
                SPContext.Current.Web.AllowUnsafeUpdates = true;
                web.AllowUnsafeUpdates = true;
            }
            if (web.Properties.ContainsKey(key))
            {
                web.Properties[key] = null;
                web.Properties.Remove(key);
                web.Update();
            }
        }

        public static void UpdateProperty(this SPWeb web, string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            if ((SPContext.Current != null) && (SPContext.Current.Web != null))
            {
                SPContext.Current.Web.AllowUnsafeUpdates = true;
                web.AllowUnsafeUpdates = true;
            }
            web.Properties[key] = value;
            web.Properties.Update();
        }
    }
}