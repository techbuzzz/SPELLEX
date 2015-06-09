using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SPEllex.SharePoint.Utils
{
    public static class Security
    {
        private static SPUserToken GetSiteAdminUserToken(Guid siteId, SPUrlZone urlZone)
        {
            SPUserToken siteAdminUserToken = null;
            RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(siteId, urlZone))
                {
                    using (SPSite site2 = new SPSite(siteId, urlZone, site.SystemAccount.UserToken))
                    {
                        IEnumerator enumerator = site2.RootWeb.SiteUsers.GetEnumerator();

                        SPUser current;
                        while (enumerator.MoveNext())
                        {
                            current = (SPUser)enumerator.Current;
                            if (current.IsSiteAdmin)
                            {
                                goto Label_0061;
                            }
                        }
                        return;
                    Label_0061:
                        siteAdminUserToken = current.UserToken;

                    }
                }
            });
            return siteAdminUserToken;
        }

        private static SPUserToken GetSystemAccountUserToken(Guid siteId, SPUrlZone urlZone)
        {
            SPUserToken systemAccountUserToken = null;
            RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(siteId, urlZone))
                {
                    systemAccountUserToken = site.SystemAccount.UserToken;
                }
            });
            return systemAccountUserToken;
        }

        public static void RunWithAccountsChainPrivileges(SPList list, CodeToRunElevatedWithList codeToRunElevatedWithList)
        {
            try
            {
                RunWithSystemAccountPrivileges(list, codeToRunElevatedWithList);
            }
            catch
            {
                try
                {
                    RunWithSiteAdminPrivileges(list, codeToRunElevatedWithList);
                }
                catch (Exception exception)
                {
                    throw new CanNotPrivilegesElevationException(exception);
                }
            }
        }

        public static void RunWithAccountsChainPrivileges(SPListItem listItem, CodeToRunElevatedWithItem codeToRunElevatedWithItem)
        {
            try
            {
                RunWithSystemAccountPrivileges(listItem, codeToRunElevatedWithItem);
            }
            catch
            {
                try
                {
                    RunWithSiteAdminPrivileges(listItem, codeToRunElevatedWithItem);
                }
                catch (Exception exception)
                {
                    throw new CanNotPrivilegesElevationException(exception);
                }
            }
        }

        public static void RunWithAccountsChainPrivileges(SPSite site, CodeToRunElevatedWithSite codeToRunElevatedWithSite)
        {
            try
            {
                RunWithSystemAccountPrivileges(site, codeToRunElevatedWithSite);
            }
            catch
            {
                try
                {
                    RunWithSiteAdminPrivileges(site, codeToRunElevatedWithSite);
                }
                catch (Exception exception)
                {
                    throw new CanNotPrivilegesElevationException(exception);
                }
            }
        }

        public static void RunWithAccountsChainPrivileges(SPWeb web, CodeToRunElevatedWithWeb codeToRunElevatedWithWeb)
        {
            try
            {
                RunWithSystemAccountPrivileges(web, codeToRunElevatedWithWeb);
            }
            catch
            {
                try
                {
                    RunWithSiteAdminPrivileges(web, codeToRunElevatedWithWeb);
                }
                catch (Exception exception)
                {
                    throw new CanNotPrivilegesElevationException(exception);
                }
            }
        }

        public static void RunWithAccountsChainPrivileges(Guid siteId, SPUrlZone urlZone, CodeToRunElevatedWithSite codeToRunElevatedWithSite)
        {
            try
            {
                RunWithSystemAccountPrivileges(siteId, urlZone, codeToRunElevatedWithSite);
            }
            catch
            {
                try
                {
                    RunWithSiteAdminPrivileges(siteId, urlZone, codeToRunElevatedWithSite);
                }
                catch (Exception exception)
                {
                    throw new CanNotPrivilegesElevationException(exception);
                }
            }
        }

        public static void RunWithAccountsChainPrivileges(Guid siteId, Guid webId, SPUrlZone urlZone, CodeToRunElevatedWithWeb codeToRunElevatedWithWeb)
        {
            try
            {
                RunWithSystemAccountPrivileges(siteId, webId, urlZone, codeToRunElevatedWithWeb);
            }
            catch
            {
                try
                {
                    RunWithSiteAdminPrivileges(siteId, webId, urlZone, codeToRunElevatedWithWeb);
                }
                catch (Exception exception)
                {
                    throw new CanNotPrivilegesElevationException(exception);
                }
            }
        }

        public static void RunWithAccountsChainPrivileges(Guid siteId, Guid webId, Guid listId, SPUrlZone urlZone, CodeToRunElevatedWithList codeToRunElevatedWithList)
        {
            try
            {
                RunWithSystemAccountPrivileges(siteId, webId, listId, urlZone, codeToRunElevatedWithList);
            }
            catch
            {
                try
                {
                    RunWithSiteAdminPrivileges(siteId, webId, listId, urlZone, codeToRunElevatedWithList);
                }
                catch (Exception exception)
                {
                    throw new CanNotPrivilegesElevationException(exception);
                }
            }
        }

        public static void RunWithAccountsChainPrivileges(Guid siteId, Guid webId, Guid listId, Guid itemId, SPUrlZone urlZone, CodeToRunElevatedWithItem codeToRunElevatedWithItem)
        {
            try
            {
                RunWithSystemAccountPrivileges(siteId, webId, listId, itemId, urlZone, codeToRunElevatedWithItem);
            }
            catch
            {
                try
                {
                    RunWithSiteAdminPrivileges(siteId, webId, listId, itemId, urlZone, codeToRunElevatedWithItem);
                }
                catch (Exception exception)
                {
                    throw new CanNotPrivilegesElevationException(exception);
                }
            }
        }

        public static void RunWithElevatedPrivileges(SPSecurity.CodeToRunElevated codeToRunElevated)
        {
            RunWithoutAccessDenied(() => SPSecurity.RunWithElevatedPrivileges(codeToRunElevated));
        }

        private static void RunWithoutAccessDenied(CodeToRunWithoutAccessDenied codeToRunWithoutAccessDenied)
        {
            bool catchAccessDeniedException = SPSecurity.CatchAccessDeniedException;
            SPSecurity.CatchAccessDeniedException = false;
            codeToRunWithoutAccessDenied();
            SPSecurity.CatchAccessDeniedException = catchAccessDeniedException;
        }

        public static void RunWithProcessIdentity(SPSecurity.CodeToRunElevated secureCode)
        {
            using (WindowsIdentity.Impersonate(IntPtr.Zero))
            {
                secureCode();
            }
        }

        public static void RunWithSiteAdminPrivileges(SPList list, CodeToRunElevatedWithList codeToRunElevatedWithList)
        {
            RunWithSiteAdminPrivileges(list.ParentWeb.Site.ID, list.ParentWeb.ID, list.ID, list.ParentWeb.Site.Zone, codeToRunElevatedWithList);
        }

        public static void RunWithSiteAdminPrivileges(SPListItem listItem, CodeToRunElevatedWithItem codeToRunElevatedWithItem)
        {
            RunWithSiteAdminPrivileges(listItem.ParentList.ParentWeb.Site.ID, listItem.ParentList.ParentWeb.ID, listItem.ParentList.ID, listItem.UniqueId, listItem.ParentList.ParentWeb.Site.Zone, codeToRunElevatedWithItem);
        }

        public static void RunWithSiteAdminPrivileges(SPSite site, CodeToRunElevatedWithSite codeToRunElevatedWithSite)
        {
            RunWithSiteAdminPrivileges(site.ID, site.Zone, codeToRunElevatedWithSite);
        }

        public static void RunWithSiteAdminPrivileges(SPWeb web, CodeToRunElevatedWithWeb codeToRunElevatedWithWeb)
        {
            RunWithSiteAdminPrivileges(web.Site.ID, web.ID, web.Site.Zone, codeToRunElevatedWithWeb);
        }

        public static void RunWithSiteAdminPrivileges(Guid siteId, SPUrlZone urlZone, CodeToRunElevatedWithSite codeToRunElevatedWithSite)
        {
            RunWithoutAccessDenied(
                () =>
                    RunWithUserTokenPrivilege(siteId, urlZone, GetSiteAdminUserToken(siteId, urlZone),
                        codeToRunElevatedWithSite));
        }

        public static void RunWithSiteAdminPrivileges(Guid siteId, Guid webId, SPUrlZone urlZone, CodeToRunElevatedWithWeb codeToRunElevatedWithWeb)
        {
            RunWithoutAccessDenied(
                () =>
                    RunWithUserTokenPrivilege(siteId, webId, urlZone, GetSiteAdminUserToken(siteId, urlZone),
                        codeToRunElevatedWithWeb));
        }

        public static void RunWithSiteAdminPrivileges(Guid siteId, Guid webId, Guid listId, SPUrlZone urlZone, CodeToRunElevatedWithList codeToRunElevatedWithList)
        {
            RunWithoutAccessDenied(
                () =>
                    RunWithUserTokenPrivilege(siteId, webId, listId, urlZone, GetSiteAdminUserToken(siteId, urlZone),
                        codeToRunElevatedWithList));
        }

        public static void RunWithSiteAdminPrivileges(Guid siteId, Guid webId, Guid listId, Guid itemId, SPUrlZone urlZone, CodeToRunElevatedWithItem codeToRunElevatedWithItem)
        {
            RunWithoutAccessDenied(
                () =>
                    RunWithUserTokenPrivilege(siteId, webId, listId, itemId, urlZone,
                        GetSiteAdminUserToken(siteId, urlZone), codeToRunElevatedWithItem));
        }

        public static void RunWithSystemAccountPrivileges(SPList list, CodeToRunElevatedWithList codeToRunElevatedWithList)
        {
            RunWithSystemAccountPrivileges(list.ParentWeb.Site.ID, list.ParentWeb.ID, list.ID, list.ParentWeb.Site.Zone, codeToRunElevatedWithList);
        }

        public static void RunWithSystemAccountPrivileges(SPListItem listItem, CodeToRunElevatedWithItem codeToRunElevatedWithItem)
        {
            RunWithSystemAccountPrivileges(listItem.ParentList.ParentWeb.Site.ID, listItem.ParentList.ParentWeb.ID, listItem.ParentList.ID, listItem.UniqueId, listItem.ParentList.ParentWeb.Site.Zone, codeToRunElevatedWithItem);
        }

        public static void RunWithSystemAccountPrivileges(SPSite site, CodeToRunElevatedWithSite codeToRunElevatedWithSite)
        {
            RunWithSystemAccountPrivileges(site.ID, site.Zone, codeToRunElevatedWithSite);
        }

        public static void RunWithSystemAccountPrivileges(SPWeb web, CodeToRunElevatedWithWeb codeToRunElevatedWithWeb)
        {
            RunWithSystemAccountPrivileges(web.Site.ID, web.ID, web.Site.Zone, codeToRunElevatedWithWeb);
        }

        public static void RunWithSystemAccountPrivileges(Guid siteId, SPUrlZone urlZone, CodeToRunElevatedWithSite codeToRunElevatedWithSite)
        {
            RunWithoutAccessDenied(
                () =>
                    RunWithUserTokenPrivilege(siteId, urlZone, GetSystemAccountUserToken(siteId, urlZone),
                        codeToRunElevatedWithSite));
        }

        public static void RunWithSystemAccountPrivileges(Guid siteId, Guid webId, SPUrlZone urlZone, CodeToRunElevatedWithWeb codeToRunElevatedWithWeb)
        {
            RunWithoutAccessDenied(
                () =>
                    RunWithUserTokenPrivilege(siteId, webId, urlZone, GetSystemAccountUserToken(siteId, urlZone),
                        codeToRunElevatedWithWeb));
        }

        public static void RunWithSystemAccountPrivileges(Guid siteId, Guid webId, Guid listId, SPUrlZone urlZone, CodeToRunElevatedWithList codeToRunElevatedWithList)
        {
            RunWithoutAccessDenied(
                () =>
                    RunWithUserTokenPrivilege(siteId, webId, listId, urlZone, GetSystemAccountUserToken(siteId, urlZone),
                        codeToRunElevatedWithList));
        }

        public static void RunWithSystemAccountPrivileges(Guid siteId, Guid webId, Guid listId, Guid itemId, SPUrlZone urlZone, CodeToRunElevatedWithItem codeToRunElevatedWithItem)
        {
            RunWithoutAccessDenied(
                () =>
                    RunWithUserTokenPrivilege(siteId, webId, listId, itemId, urlZone,
                        GetSystemAccountUserToken(siteId, urlZone), codeToRunElevatedWithItem));
        }

        private static void RunWithUserTokenPrivilege(Guid siteId, SPUrlZone urlZone, SPUserToken userToken, CodeToRunElevatedWithSite codeToRunElevatedWithSite)
        {
            RunWithoutAccessDenied(() =>
            {
                using (SPSite site = new SPSite(siteId, urlZone, userToken))
                {
                    codeToRunElevatedWithSite(site);
                }
            });
        }

        private static void RunWithUserTokenPrivilege(Guid siteId, Guid webId, SPUrlZone urlZone, SPUserToken userToken, CodeToRunElevatedWithWeb codeToRunElevatedWithWeb)
        {
            RunWithoutAccessDenied(() =>
            {
                using (SPSite site = new SPSite(siteId, urlZone, userToken))
                {
                    using (SPWeb web = site.OpenWeb(webId))
                    {
                        codeToRunElevatedWithWeb(web);
                    }
                }
            });
        }

        private static void RunWithUserTokenPrivilege(Guid siteId, Guid webId, Guid listId, SPUrlZone urlZone, SPUserToken userToken, CodeToRunElevatedWithList codeToRunElevatedWithList)
        {
            RunWithoutAccessDenied(() =>
            {
                using (SPSite site = new SPSite(siteId, urlZone, userToken))
                {
                    using (SPWeb web = site.OpenWeb(webId))
                    {
                        SPList list = web.Lists[listId];
                        codeToRunElevatedWithList(list);
                    }
                }
            });
        }

        private static void RunWithUserTokenPrivilege(Guid siteId, Guid webId, Guid listId, Guid itemId, SPUrlZone urlZone, SPUserToken userToken, CodeToRunElevatedWithItem codeToRunElevatedWithItem)
        {
            RunWithoutAccessDenied(() =>
            {
                using (SPSite site = new SPSite(siteId, urlZone, userToken))
                {
                    using (SPWeb web = site.OpenWeb(webId))
                    {
                        SPList list = web.Lists[listId];
                        SPListItem item = list.Items[itemId];
                        codeToRunElevatedWithItem(item);
                    }
                }
            });
        }

        public delegate void CodeToRunElevatedWithItem(SPListItem item);

        public delegate void CodeToRunElevatedWithList(SPList list);

        public delegate void CodeToRunElevatedWithSite(SPSite site);

        public delegate void CodeToRunElevatedWithWeb(SPWeb web);

        public delegate void CodeToRunWithoutAccessDenied();

        [Serializable]
        public class CanNotPrivilegesElevationException : Exception
        {
            public CanNotPrivilegesElevationException(Exception innerException)
                : base("Not elevated", innerException)
            {
            }
        }

    }
}
