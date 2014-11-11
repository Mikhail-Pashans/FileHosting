using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Data;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using FileHosting.Database;
using FileHosting.Database.Models;
using FileHosting.MVC.Extensions;

namespace FileHosting.MVC.Providers
{
    public class MyMembershipProvider : MembershipProvider
    {
        private IUnitOfWork _context;

        #region Implemented MembershipProvider methods

        public override void Initialize(string name, NameValueCollection config)
        {
            _context = DependencyResolver.Current.GetService<IUnitOfWork>();

            if (_context == null)
                throw new ProviderException(
                    "FileHosting Membership Provider only works when used in combination with an IUnitOfWork. You should add this component to your container.");

            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrWhiteSpace(name))
                name = typeof (MyMembershipProvider).Name;

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "FileHosting Membership Provider");
            }

            base.Initialize(name, config);
        }

        public override bool ValidateUser(string email, string password)
        {
            var isValid = false;
            email = email.ToLowerInvariant();

            User user = _context.UserRepository.FirstOrDefault(u => u.Email == email);

            if (user != null && Crypto.VerifyHashedPassword(user.Password, password))
            {
                isValid = true;
            }

            return isValid;
        }

        public override MembershipUser CreateUser(string userName, string password, string email,
            string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey,
            out MembershipCreateStatus status)
        {
            userName = userName.Trim();
            email = email.ToLowerInvariant().Trim();
            status = MembershipCreateStatus.Success;

            var args = new ValidatePasswordEventArgs(email, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (!email.IsValidEmail())
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            MembershipUser membershipUser = GetUser(userName, email);

            if (membershipUser != null)
            {
                if (membershipUser.UserName == userName)
                    status = MembershipCreateStatus.DuplicateUserName;

                if (membershipUser.Email == email)
                    status = MembershipCreateStatus.DuplicateEmail;

                return null;
            }

            Role role = _context.RoleRepository.FirstOrDefault(r => r.Name == "RegisteredUser");

            _context.UserRepository.Add(new User
            {
                Name = userName,
                Email = email,
                Password = Crypto.HashPassword(password),
                CreationDate = DateTime.UtcNow,
                DownloadAmountLimit = 0,
                DownloadSpeedLimit = 0,
                Comments = new List<Comment>(),
                Downloads = new List<Download>(),
                Files = new List<File>(),
                AllowedFiles = new List<File>(),
                SubscribedFiles = new List<File>(),
                News = new List<News>(),
                Roles = role == null ? new List<Role>() : new List<Role> {role},
            });

            try
            {
                _context.Commit();
            }
            catch (DataException)
            {
                status = MembershipCreateStatus.ProviderError;
                return null;
            }

            membershipUser = GetUser(userName, email);

            return membershipUser;
        }

        private MembershipUser GetUser(string userName, string email)
        {
            User[] users = _context.UserRepository.Find(u => u.Name == userName || u.Email == email).ToArray();

            if (!users.Any()) return null;

            User user = users.First();

            var memberUser = new MembershipUser("MyMembershipProvider", user.Name, Guid.NewGuid(), user.Email, null,
                null, true, false, user.CreationDate, DateTime.UtcNow, DateTime.UtcNow, user.CreationDate,
                DateTime.MinValue);

            return memberUser;
        }

        public User GetUserByEmail(string email)
        {
            User[] users = _context.UserRepository.Find(u => u.Email == email).ToArray();

            if (!users.Any()) return null;

            User user = users.First();

            return user;
        }

        public bool ChangeUserPassword(string userEmail, string newUserPassword)
        {
            User user = _context.UserRepository.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
                return false;

            var args = new ValidatePasswordEventArgs(userEmail, newUserPassword, false);

            OnValidatingPassword(args);

            if (args.Cancel)
                return false;

            _context.UserRepository.Attach(user);

            user.Password = Crypto.HashPassword(newUserPassword);

            try
            {
                _context.Commit();
            }
            catch (DataException)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region  Non-implemented MembershipProvider methods

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
            string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
            out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
            out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Non-implemented MembershipProvider properties

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}