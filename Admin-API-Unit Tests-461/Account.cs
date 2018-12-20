using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using DMWeb_REST_Admin;
using DMWeb_REST_Admin.Models;

namespace DMWeb_REST_Admin_Unit_Tests
{
    public class AccountContext
    {
        public static DMAdmin admin = new DMAdmin("https://ssl.datamotion.com/Remote");
        public static int uid = 0;
        public static string email = "";
    }

    [TestFixture]
    public class AccountTests
    {
        static string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string rootPath = Directory.GetParent(assemblyFolder).Parent.Parent.FullName;
        private static string _unitTestPath = Path.Combine(rootPath, "Admin-API-Unit Tests-461");
        private static string _credentialsPath = Path.Combine(_unitTestPath, "Test Credentials");
        private string _credentialsDataPath = Path.Combine(_credentialsPath, "Credentials.txt");

        [Test]
        [Order(7)]
        [Category("Get Session Key")]
        [Category("No Session Key")]
        [Category("Positive")]
        public void GetSessionKeyTrueEncryptionKey()
        {
            string encryptionKey = "";
            string email = "";
            string automationID = "";

            string[] lines = File.ReadAllLines(_credentialsDataPath);

            string str1 = lines[0];
            string[] linesplit1 = str1.Split(':');
            encryptionKey = linesplit1[1];

            string str2 = lines[1];
            string[] linesplit2 = str2.Split(':');
            email = linesplit2[1];

            string str3 = lines[2];
            string[] linesplit3 = str3.Split(':');
            automationID = linesplit3[1];

            try
            {
                string sessionKey = AccountContext.admin.Authentication.GetSessionKey(encryptionKey, email, automationID).GetAwaiter().GetResult();
                Assert.AreNotEqual(sessionKey, null);
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test]
        [Order(1)]
        [Category("List User Accounts")]
        [Category("No Session Key")]
        [Category("Negative")]
        public void ListUserAccountsNoSessionKey()
        {
            Account.ListUserAccountsRequest request = new Account.ListUserAccountsRequest
            {
                PageNumber = 1,
                OrderBy = "UniqueId",
                Filter = ""
            };

            Account.ListUserAccountsResponse response = new Account.ListUserAccountsResponse();

            try
            {
                response = AccountContext.admin.Account.ListUserAccounts(request).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("List User Accounts")]
        [Category("Positive")]
        public void ListUserAccountsPositiveTest()
        {
            Account.ListUserAccountsRequest request = new Account.ListUserAccountsRequest
            {
                CompanyId = 15260.ToString(),
                PageNumber = 1,
                OrderBy = "UniqueId",
                Filter = ""
            };

            Account.ListUserAccountsResponse response = new Account.ListUserAccountsResponse();

            response = AccountContext.admin.Account.ListUserAccounts(request).GetAwaiter().GetResult();
        }

        [Test]
        [Category("List User Accounts")]
        [Category("Negative")]
        public void ListUserAccountsNegativeTest()
        {
            Account.ListUserAccountsRequest request = new Account.ListUserAccountsRequest
            {
                PageNumber = 1,
                OrderBy = "",
                Filter = ""
            };

            Account.ListUserAccountsResponse response = new Account.ListUserAccountsResponse();

            try
            {
                response = AccountContext.admin.Account.ListUserAccounts(request).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test]
        [Order(2)]
        [Category("Create a User")]
        [Category("No Session Key")]
        [Category("Negative")]
        public void CreateUserNoSessionKey()
        {
            Account.CreateUserRequest request = new Account.CreateUserRequest
            {
                Email = "unittestnosessionkey@dmfaketest.com",
                UserTypeId = 26
            };

            Account.CreateUserResponse response = new Account.CreateUserResponse();

            try
            {
                AccountContext.admin.Account.CreateUserAccount(request).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Order(8)]
        [Category("Create a User")]
        [Category("Positive")]
        public void CreateUserPositiveTest()
        {
            Account.CreateUserRequest request = new Account.CreateUserRequest
            {
                Email = "CreatedTestUser@dmfaketest.com",
                UserTypeId = 14164
            };

            Account.CreateUserResponse response = new Account.CreateUserResponse();

            response = AccountContext.admin.Account.CreateUserAccount(request).GetAwaiter().GetResult();

            AccountContext.uid = response.UniqueId;
            AccountContext.email = response.Email;

            Thread.Sleep(10000);
        }

        [Test]
        [Category("Create a User")]
        [Category("Negative")]
        public void CreateUserNegativeTest()
        {
            Account.CreateUserRequest request = new Account.CreateUserRequest
            {
                Email = "unittestfailed@dmfaketest.com",
                UserTypeId = 12345
            };

            Account.CreateUserResponse response = new Account.CreateUserResponse();

            try
            {
                AccountContext.admin.Account.CreateUserAccount(request).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Order(3)]
        [Category("View a User Account")]
        [Category("No Session Key")]
        [Category("Negative")]
        public void ViewUserAccountNoSessionKey()
        {
            Account.ViewUserRequest request = new Account.ViewUserRequest
            {
                UID = 0,
                Email = ""
            };

            Account.ViewUserResponse response = new Account.ViewUserResponse();

            try
            {
                AccountContext.admin.Account.ViewUserAccount(request).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Order(9)]
        [Category("View a User Account")]
        [Category("Positive")]
        public void ViewUserAccountPositiveTest()
        {
            Account.ViewUserRequest request = new Account.ViewUserRequest
            {
                UID = AccountContext.uid,
                Email = AccountContext.email
            };

            Account.ViewUserResponse response = new Account.ViewUserResponse();

            AccountContext.admin.Account.ViewUserAccount(request).GetAwaiter().GetResult();
        }

        [Test]
        [Category("View a User Account")]
        [Category("Negative")]
        public void ViewUserAccountNegativeTest()
        {
            Account.ViewUserRequest request = new Account.ViewUserRequest
            {
                UID = 12345,
                Email = AccountContext.email
            };

            Account.ViewUserResponse response = new Account.ViewUserResponse();

            try
            {
                AccountContext.admin.Account.ViewUserAccount(request).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("500"));
            }
        }

        [Test]
        [Order(4)]
        [Category("Update a User Account")]
        [Category("No Session Key")]
        [Category("Negative")]
        public void UpdateUserNoSessionKey()
        {
            Account.UpdateUserRequest request = new Account.UpdateUserRequest
            {
                UniqueId = 0,
                Email = "",
                FirstName = "Test",
                LastName = "User"
            };

            Account.UpdateUserResponse response = new Account.UpdateUserResponse();

            try
            {
                response = AccountContext.admin.Account.UpdateUserAccount(request).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }
        [Test]
        [Order(10)]
        [Category("Update a User Account")]
        [Category("Positive")]
        public void UpdateUserPositiveTest()
        {
            Account.UpdateUserRequest request = new Account.UpdateUserRequest
            {
                UniqueId = AccountContext.uid,
                Email = AccountContext.email,
                FirstName = "Test",
                LastName = "User"
            };

            Account.UpdateUserResponse response = new Account.UpdateUserResponse();

            response = AccountContext.admin.Account.UpdateUserAccount(request).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Update a User Account")]
        [Category("Negative")]
        public void UpdateUserNegativeTest()
        {
            Account.UpdateUserRequest request = new Account.UpdateUserRequest
            {
                UniqueId = 12345,
                Email = AccountContext.email,
                FirstName = "Test",
                LastName = "User"
            };

            Account.UpdateUserResponse response = new Account.UpdateUserResponse();

            try
            {
                response = AccountContext.admin.Account.UpdateUserAccount(request).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Order(5)]
        [Category("Delete a User")]
        [Category("No Session Key")]
        [Category("Negative")]
        public void DeleteUserNoSessionKey()
        {
            Account.DeleteUserRequest request = new Account.DeleteUserRequest
            {
                UID = 0,
                Email = ""
            };

            try
            {
                AccountContext.admin.Account.DeleteUser(request);
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Order(11)]
        [Category("Delete a User")]
        [Category("Positive")]
        public void DeleteUserPositiveTest()
        {
            Account.DeleteUserRequest request = new Account.DeleteUserRequest
            {
                UID = AccountContext.uid,
                Email = AccountContext.email
            };

            AccountContext.admin.Account.DeleteUser(request);
        }

        [Test]
        [Category("Delete a User")]
        [Category("Negative")]
        public void DeleteUserNegativeTest()
        {
            Account.DeleteUserRequest request = new Account.DeleteUserRequest
            {
                UID = 12345,
                Email = AccountContext.email
            };

            try
            {
                AccountContext.admin.Account.DeleteUser(request);
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("500"));
            }
        }

        [Test]
        [Order(6)]
        [Category("Get User Types")]
        [Category("No Session Key")]
        [Category("Negative")]
        public void GetUserTypesNoSessionKey()
        {
            List<Account.GetUserTypesResponse> response = new List<Account.GetUserTypesResponse>();

            try
            {
                response = AccountContext.admin.Account.GetUserTypes().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Get User Types")]
        [Category("Positive")]
        public void GetUserTypesPositiveTest()
        {
            List<Account.GetUserTypesResponse> response = new List<Account.GetUserTypesResponse>();

            response = AccountContext.admin.Account.GetUserTypes().GetAwaiter().GetResult();
        }

        [Test]
        [Category("Get User Types")]
        [Category("Negative")]
        public void GetUserTypesNegativeTest()
        {
            List<Account.GetUserTypesResponse> response = new List<Account.GetUserTypesResponse>();

            try
            {
                response = AccountContext.admin.Account.GetUserTypes().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }
    }
}
