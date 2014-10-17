using System.Data;
using FileHosting.Database;
using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using FileHosting.Database.Models;

namespace FileHosting.MVC.Providers
{
    public class MyRoleProvider : RoleProvider
    {
        private IUnitOfWork _context;

        #region Implemented RoleProvider methods

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            _context = DependencyResolver.Current.GetService<IUnitOfWork>();

            if (_context == null)
                throw new ProviderException("FileHosting Role Provider only works when used in combination with an IUnitOfWork. You should add this component to your container.");

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = typeof(MyRoleProvider).Name;

            base.Initialize(name, config);
        }

        public override string[] GetRolesForUser(string email)
        {
            string[] roles = { };

            var user = _context.UserRepository.FirstOrDefault(u => u.Email == email);

            if (user == null) return roles;

            roles = user.Roles.Select(r => r.Name).ToArray();

            return roles.Any() ? roles : new string[] { };
        }

        public override void CreateRole(string roleName)
        {
            var newRole = new Role
            {
                Name = roleName,
                Users = new List<User>()
            };

            _context.RoleRepository.Add(newRole);
            _context.Commit();
        }

        public override bool IsUserInRole(string email, string roleName)
        {
            var outputResult = false;

            var user = _context.UserRepository.FirstOrDefault(u => u.Email == email);

            if (user == null) return false;

            var userRole = user.Roles.FirstOrDefault(r => r.Name == roleName);

            if (userRole != null && userRole.Name == roleName)
            {
                outputResult = true;
            }

            return outputResult;
        }

        public override string[] GetAllRoles()
        {
            string[] roleNames = { };

            var roles = _context.RoleRepository.GetAll().ToArray();

            if (!roles.Any())
                return roleNames;

            roleNames = roles.Select(r => r.Name).ToArray();

            return roleNames.Any() ? roleNames : new string[] { };
        }

        public override void AddUsersToRoles(string[] userEmails, string[] roleNames)
        {
            foreach (var user in userEmails.Select(email => _context.UserRepository.FirstOrDefault(u => u.Email == email)).Where(user => user != null))
            {
                _context.UserRepository.Attach(user);

                var userRoles = new List<Role>(roleNames.Length);

                userRoles.AddRange(roleNames.Select(name => _context.RoleRepository.First(r => r.Name == name)));

                user.Roles = userRoles;

                _context.Commit();
            }
        }

        #endregion

        #region Non-implemented RoleProvider methods

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
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

        #endregion

        #region Non-implemented RoleProvider properties

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion
    }
}