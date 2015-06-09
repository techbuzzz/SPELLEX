using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace SPEllex.SharePoint.Utils
{
    /// <summary>
    /// This class contains utils for work with Permissons.
    /// </summary>
    public static class Rights
    {
        #region Public methods

        public static void AssignRights(SPWeb web, SPListItem item, object user, string roleType)
        {
            if (user != null)
            {
                bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
                web.AllowUnsafeUpdates = true;
                SPRoleDefinition roleDefinition = GetRoleDefinition(web, roleType);
                if (user is string)
                {
                    var value2 = new SPFieldUserValue(web, (string) user);
                    AssignRights(web, value2, roleDefinition, item);
                }
                else if (user is SPFieldUserValueCollection)
                {
                    AssignRights(web, (SPFieldUserValueCollection) user, roleDefinition, item);
                }
                else if (user is SPFieldUserValue)
                {
                    AssignRights(web, (SPFieldUserValue) user, roleDefinition, item);
                }
                else if (user is SPPrincipal)
                {
                    AssignRights(web, (SPPrincipal) user, roleDefinition, item);
                }
                else if (user is List<SPPrincipal>)
                {
                    AssignRights(web, (List<SPPrincipal>) user, roleDefinition, item);
                }
                web.AllowUnsafeUpdates = allowUnsafeUpdates;
            }
        }


        public static void ClearRights(SPListItem item)
        {
            SPWeb web = item.Web;
            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            if (!item.HasUniqueRoleAssignments)
            {
                item.BreakRoleInheritance(false);
                if (web.AssociatedOwnerGroup != null)
                {
                    var roleAssignment = new SPRoleAssignment(web.AssociatedOwnerGroup);
                    SPRoleDefinition byType = web.RoleDefinitions.GetByType(SPRoleType.Administrator);
                    roleAssignment.RoleDefinitionBindings.Add(byType);
                    item.RoleAssignments.Add(roleAssignment);
                }
            }
            else
            {
                int num = 0x3fffffff;
                int num2 = (web.AssociatedOwnerGroup != null) ? web.AssociatedOwnerGroup.ID : -1;
                for (int i = item.RoleAssignments.Count - 1; i >= 0; i--)
                {
                    int iD = item.RoleAssignments[i].Member.ID;
                    if ((iD != num) && (iD != num2))
                    {
                        item.RoleAssignments.Remove(i);
                    }
                }
            }
            web.AllowUnsafeUpdates = allowUnsafeUpdates;
        }

        public static void ClearRights(SPWeb web, SPListItem item, bool deleteGuest)
        {
            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            item.BreakRoleInheritance(true);
            web.AllowUnsafeUpdates = true;
            SPRoleAssignmentCollection roleAssignments = item.RoleAssignments;
            bool flag2 = false;
            bool flag3 = false;
            int iD = web.Site.SystemAccount.ID;
            int num2 = (web.AssociatedOwnerGroup != null) ? web.AssociatedOwnerGroup.ID : -1;
            for (int i = roleAssignments.Count - 1; i > -1; i--)
            {
                SPRoleAssignment assignment = roleAssignments[i];
                int num4 = assignment.Member.ID;
                if (num4 == iD)
                {
                    flag3 = true;
                }
                else if (num4 == num2)
                {
                    flag2 = true;
                }
                else if (deleteGuest)
                {
                    roleAssignments.Remove(i);
                }
                else
                {
                    assignment.RoleDefinitionBindings.RemoveAll();
                    assignment.Update();
                }
            }
            web.AllowUnsafeUpdates = true;
            SPRoleDefinition byType = web.RoleDefinitions.GetByType(SPRoleType.Administrator);
            if (!flag3)
            {
                var roleAssignment = new SPRoleAssignment(web.Site.SystemAccount);
                roleAssignment.RoleDefinitionBindings.Add(byType);
                roleAssignments.Add(roleAssignment);
            }
            if (!(flag2 || (web.AssociatedOwnerGroup == null)))
            {
                var assignment3 = new SPRoleAssignment(web.AssociatedOwnerGroup);
                assignment3.RoleDefinitionBindings.Add(byType);
                roleAssignments.Add(assignment3);
            }
            web.AllowUnsafeUpdates = allowUnsafeUpdates;
        }

        public static SPRoleDefinition CreateRoleDefinition(SPWeb rootWeb, string name, string description)
        {
            SPRoleDefinition roleDefinition = GetRoleDefinition(rootWeb, name);
            if (roleDefinition == null)
            {
                try
                {
                    roleDefinition = CreateRoleDefinitionInternal(rootWeb, name, description);
                    foreach (SPWeb web in rootWeb.Site.AllWebs)
                    {
                        if (GetRoleDefinition(web, name) == null)
                        {
                            CreateRoleDefinitionInternal(web, name, description);
                        }
                    }
                }
                catch (Exception exception)
                {
                    //throw new ApplicationException("Action_CreateRole_Error", new object[] { name }), exception);
                }
            }
            return roleDefinition;
        }


        public static bool DoesPrincipalHasPermissions(SPListItem item, SPPrincipal principal,
            SPBasePermissions permissions)
        {
            if (principal is SPUser)
            {
                return item.DoesUserHavePermissions((SPUser) principal, permissions);
            }
            SPRoleAssignment assignmentByPrincipal = null;
            try
            {
                assignmentByPrincipal = item.RoleAssignments.GetAssignmentByPrincipal(principal);
            }
            catch
            {
                return false;
            }
            foreach (SPRoleDefinition definition in assignmentByPrincipal.RoleDefinitionBindings)
            {
                if ((definition.BasePermissions & permissions) == permissions)
                {
                    return true;
                }
            }
            return false;
        }

        public static SPRoleDefinition GetRoleDefinition(SPWeb web, string name)
        {
            SPRoleDefinitionCollection roleDefinitions = web.RoleDefinitions;
            IEnumerator enumerator = roleDefinitions.GetEnumerator();

            SPRoleDefinition current;
            while (enumerator.MoveNext())
            {
                current = (SPRoleDefinition) enumerator.Current;
                if (current.Name == name)
                {
                    return current;
                }
            }
            return null;
        }

        public static void CopyRights(SPListItem fromItem, SPListItem toItem)
        {
            SPRoleAssignmentCollection fromRights = fromItem.RoleAssignments;
            ClearRights(toItem);
            foreach (SPRoleAssignment right in fromRights)
            {
                toItem.RoleAssignments.Add(right);
            }
        }


        public static void RepeatClearRights(SPWeb web, SPListItem item)
        {
            RepeatClearRights(web, item, true);
        }

        public static void RepeatClearRights(SPWeb web, SPListItem item, bool deleteGuest)
        {
            // This item is obfuscated and can not be translated.
            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            bool flag2 = false;
            int num = 0;
            while (flag2)
            {
                Label_000E:
                if (!flag2)
                {
                    try
                    {
                        ClearRights(web, item, deleteGuest);
                    }
                    catch (Exception)
                    {
                    }
                }
                web.AllowUnsafeUpdates = allowUnsafeUpdates;
                return;

                try
                {
                    num++;
                    ClearRights(web, item, deleteGuest);
                    flag2 = true;
                }
                catch (Exception)
                {
                }
                goto Label_000E;
            }
        }

        public static void SetReadOnly(SPListItem item)
        {
            SPWeb web = item.Web;
            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            if (!item.HasUniqueRoleAssignments)
            {
                item.BreakRoleInheritance(true);
            }
            web.AllowUnsafeUpdates = true;
            SPRoleAssignmentCollection roleAssignments = item.RoleAssignments;
            int iD = web.Site.SystemAccount.ID;
            int num2 = (web.AssociatedOwnerGroup != null) ? web.AssociatedOwnerGroup.ID : -1;
            for (int i = roleAssignments.Count - 1; i > -1; i--)
            {
                SPRoleAssignment assignment = roleAssignments[i];
                int num4 = assignment.Member.ID;
                if ((num4 != iD) && (num4 != num2))
                {
                    SPRoleDefinitionBindingCollection roleDefinitionBindings = assignment.RoleDefinitionBindings;
                    if ((roleDefinitionBindings.Count != 1) || (roleDefinitionBindings[0].Type != SPRoleType.Reader))
                    {
                        roleDefinitionBindings.RemoveAll();
                        SPRoleDefinition byType = web.RoleDefinitions.GetByType(SPRoleType.Reader);
                        roleDefinitionBindings.Add(byType);
                        assignment.Update();
                    }
                }
            }
            web.AllowUnsafeUpdates = allowUnsafeUpdates;
        }

        public static void SetReadOnly(SPListItem item, int userId)
        {
            SPWeb web = item.Web;
            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            if (!item.HasUniqueRoleAssignments)
            {
                item.BreakRoleInheritance(true);
            }
            web.AllowUnsafeUpdates = true;
            SPRoleAssignmentCollection roleAssignments = item.RoleAssignments;
            for (int i = roleAssignments.Count - 1; i > -1; i--)
            {
                SPRoleAssignment assignment = roleAssignments[i];
                if (assignment.Member.ID == userId)
                {
                    SPRoleDefinitionBindingCollection roleDefinitionBindings = assignment.RoleDefinitionBindings;
                    if ((roleDefinitionBindings.Count != 1) || (roleDefinitionBindings[0].Type != SPRoleType.Reader))
                    {
                        roleDefinitionBindings.RemoveAll();
                        SPRoleDefinition byType = web.RoleDefinitions.GetByType(SPRoleType.Reader);
                        roleDefinitionBindings.Add(byType);
                        assignment.Update();
                    }
                    break;
                }
            }
            web.AllowUnsafeUpdates = allowUnsafeUpdates;
        }

        public static void AssignRights(SPWeb web, SPFolder folder, object user, string role)
        {
            if (user != null)
            {
                bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
                web.AllowUnsafeUpdates = true;
                SPRoleDefinition roleDefinition = GetRoleDefinition(web, role);
                if (user is string)
                {
                    var value2 = new SPFieldUserValue(web, (string) user);
                    AssignRights(web, value2, roleDefinition, folder);
                }
                else if (user is SPFieldUserValueCollection)
                {
                    AssignRights(web, (SPFieldUserValueCollection) user, roleDefinition, folder);
                }
                else if (user is SPFieldUserValue)
                {
                    AssignRights(web, (SPFieldUserValue) user, roleDefinition, folder);
                }
                else if (user is SPPrincipal)
                {
                    AssignRights(web, (SPPrincipal) user, roleDefinition, folder);
                }
                else if (user is List<SPPrincipal>)
                {
                    AssignRights(web, (List<SPPrincipal>) user, roleDefinition, folder);
                }
                web.AllowUnsafeUpdates = allowUnsafeUpdates;
            }
        }

        public static void AssignTravelingRights(SPWeb web, Dictionary<SPUser, SPRoleDefinition> users, SPListItem item)
        {
            ClearRights(item);
            foreach (var user in users)
            {
                AssignRights(web, user.Key, user.Value, item);
            }
        }

        #endregion
        #region Private methods

        private static void AssignRights(SPWeb web, SPFieldUserValue user, SPRoleDefinition roleDefinition,
            SPListItem item)
        {
            SPPrincipal byId;
            if (user.User != null)
            {
                byId = user.User;
            }
            else
            {
                byId = web.SiteGroups.GetByID(user.LookupId);
            }
            AssignRights(web, byId, roleDefinition, item);
        }

        private static void AssignRights(SPWeb web, SPFieldUserValueCollection users, SPRoleDefinition roleDefinition,
            SPListItem item)
        {
            foreach (SPFieldUserValue value2 in users)
            {
                AssignRights(web, value2, roleDefinition, item);
            }
        }

        private static void AssignRights(SPWeb web, SPPrincipal principal, SPRoleDefinition roleDefinition,
            SPListItem item)
        {
            if (!DoesPrincipalHasPermissions(item, principal, roleDefinition.BasePermissions))
            {
                if (!item.HasUniqueRoleAssignments)
                {
                    item.BreakRoleInheritance(true);
                }
                var roleAssignment = new SPRoleAssignment(principal);
                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                item.RoleAssignments.Add(roleAssignment);
            }
        }

        private static void AssignRights(SPWeb web, List<SPPrincipal> principals, SPRoleDefinition roleDefinition,
            SPListItem item)
        {
            foreach (SPPrincipal principal in principals)
            {
                AssignRights(web, principal, roleDefinition, item);
            }
        }

        private static SPRoleDefinition CreateRoleDefinitionInternal(SPWeb rootWeb, string name, string description)
        {
            var role = new SPRoleDefinition
            {
                Name = name,
                Description = description,
                BasePermissions =
                    SPBasePermissions.BrowseDirectories | SPBasePermissions.AddDelPrivateWebParts |
                    SPBasePermissions.BrowseUserInfo | SPBasePermissions.CreateSSCSite |
                    SPBasePermissions.UpdatePersonalWebParts | SPBasePermissions.EditMyUserInfo |
                    SPBasePermissions.CreateAlerts | SPBasePermissions.UseRemoteAPIs |
                    SPBasePermissions.UseClientIntegration | SPBasePermissions.ViewVersions |
                    SPBasePermissions.OpenItems | SPBasePermissions.AddListItems | SPBasePermissions.ViewListItems |
                    SPBasePermissions.ViewPages | SPBasePermissions.Open | SPBasePermissions.ManagePersonalViews |
                    SPBasePermissions.ViewFormPages
            };
            rootWeb.RoleDefinitions.Add(role);
            rootWeb.Update();
            return role;
        }

        private static void AssignRights(SPWeb web, List<SPPrincipal> principals, SPRoleDefinition roleDefinition,
            SPFolder folder)
        {
            foreach (SPPrincipal principal in principals)
            {
                AssignRights(web, principal, roleDefinition, folder);
            }
        }

        private static void AssignRights(SPWeb web, SPPrincipal principal, SPRoleDefinition roleDefinition,
            SPFolder folder)
        {
            if (!DoesPrincipalHasPermissions(folder, principal, roleDefinition.BasePermissions))
            {
                if (!folder.Item.HasUniqueRoleAssignments)
                {
                    folder.Item.BreakRoleInheritance(true);
                }
                var roleAssignment = new SPRoleAssignment(principal);
                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                folder.Item.RoleAssignments.Add(roleAssignment);
            }
        }

        private static bool DoesPrincipalHasPermissions(SPFolder folder, SPPrincipal principal,
            SPBasePermissions permissions)
        {
            var user = principal as SPUser;
            if (user != null)
            {
                return folder.Item.DoesUserHavePermissions(user, permissions);
            }
            SPRoleAssignment assignmentByPrincipal = null;
            try
            {
                assignmentByPrincipal = folder.Item.RoleAssignments.GetAssignmentByPrincipal(principal);
            }
            catch
            {
                return false;
            }
            foreach (SPRoleDefinition definition in assignmentByPrincipal.RoleDefinitionBindings)
            {
                if ((definition.BasePermissions & permissions) == permissions)
                {
                    return true;
                }
            }
            return false;
        }

        private static void AssignRights(SPWeb web, SPFieldUserValueCollection users, SPRoleDefinition roleDefinition,
            SPFolder folder)
        {
            foreach (SPFieldUserValue value2 in users)
            {
                AssignRights(web, value2, roleDefinition, folder);
            }
        }

        private static void AssignRights(SPWeb web, SPFieldUserValue user, SPRoleDefinition roleDefinition,
            SPFolder folder)
        {
            SPPrincipal byId;
            if (user.User != null)
            {
                byId = user.User;
            }
            else
            {
                byId = web.SiteGroups.GetByID(user.LookupId);
            }
            AssignRights(web, byId, roleDefinition, folder);
        }

        #endregion
    }
}