using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SPEllex.Server;

namespace SPEllex.SharePoint
{
    public static class FarmExtension
    {
        public static bool ExistProperty(this SPFarm farm, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            return farm.Properties.ContainsKey(key);
        }

        public static object GetProperty(this SPFarm farm, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            object obj2 = null;
            if (farm.ExistProperty(key))
            {
                obj2 = farm.Properties[key];
            }
            return obj2;
        }

        public static string GetPropertyString(this SPFarm farm, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            string str = string.Empty;
            if (farm.ExistProperty(key) && (farm.Properties[key] != null))
            {
                str = farm.Properties[key].ToString();
            }
            return str;
        }

        public static SharePointVersions GetVersion(this SPFarm farm)
        {
            switch (farm.BuildVersion.Major)
            {
                case 12:
                    return SharePointVersions.SP2007;
                case 14:
                    return SharePointVersions.SP2010;
                case 15:
                    return SharePointVersions.SP2013;
            }
            return SharePointVersions.Unknown;
        }

        public static List<SPServer> GetWebFrontEndServers(this SPFarm farm)
        {
            return SPFarm.Local.Servers.Where(server => server.IsWebFrontEndServer()).ToList();
        }

        public static void RemoveProperty(this SPFarm farm, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            if ((SPContext.Current != null) && (SPContext.Current.Web != null))
            {
                SPContext.Current.Web.AllowUnsafeUpdates = true;
            }
            if (farm.ExistProperty(key))
            {
                farm.Properties.Remove(key);
                farm.Update();
            }
        }

        public static void SetProperty(this SPFarm farm, string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            if (value == null)
            {
                value = string.Empty;
            }
            if ((SPContext.Current != null) && (SPContext.Current.Web != null))
            {
                SPContext.Current.Web.AllowUnsafeUpdates = true;
            }
            if ((farm.Properties[key] != null) && (farm.Properties[key].ToString() == value.ToString())) return;
            farm.Properties[key] = value;
            farm.Update();
        }
    }
}