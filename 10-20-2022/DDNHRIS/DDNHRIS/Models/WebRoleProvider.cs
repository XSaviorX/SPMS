using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace DDNHRIS.Models
{
    [SessionTimeout]
    public class WebRoleProvider : RoleProvider
    {
        public override string ApplicationName { get { throw new Exception(); } set { throw new Exception(); } }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {

            try
            {
                var tempRole = HttpContext.Current.Session["_RoleList"];
                string[] roles = tempRole as string[];
                if (roles != null)
                {
                    return roles.ToArray();
                }
                string[] noRoles = { "" };
                return noRoles;
            }
            catch (Exception)
            {
                string[] roles = {""};
                return roles;
            }
             
            
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}