using System;
using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
//using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccidentalFish.AspNet.Identity.Azure;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Cookies;
using System.Security.Claims;
using Microsoft.WindowsAzure.Storage;
using System.Threading;

namespace AccidentalFish.AspNet.Identity.Azure.Tests
{
    [TestClass]
    public class TableUserStoreTests
    {
        private string connectionString = ConfigurationManager.AppSettings["AzureTestStorage"];
        private UserManager<TableUser> _mgr = null;
        private string userName = "testusername";
        private TableUserStore<TableUser> _userStore = null;

        [TestInitialize]
        public void Initialize()
        {
            //Cleanup(); //wait 40 seconds
            _userStore = new TableUserStore<TableUser>(connectionString);
            _mgr = new UserManager<TableUser>(_userStore);
        }

        [TestMethod]
        public async Task Create_User()
        {
            TableUser user = new TableUser(userName);
            var result = await _mgr.CreateAsync(user);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task Add_User_To_Role()
        {
            TableUser user = new TableUser(userName);
            var result = await _mgr.CreateAsync(user);
            Assert.IsTrue(result.Succeeded);
            TableUserRole role = new TableUserRole(user.Id,"Admin");
            await _userStore.AddToRoleAsync(user, "Admin");
            Assert.IsTrue(_mgr.IsInRole<TableUser>(user.Id,"Admin"));
            var result2 = await _userStore.FindByIdAsync("e81b55be-33d5-4f42-9ecd-713345ea8c60");
            await _userStore.AddToRoleAsync(result2, "Admin");
        }

        [TestMethod]
        public async Task Add_Login()
        {
            TableUser user = new TableUser(userName);
            var result = await _mgr.CreateAsync(user);
            Assert.IsTrue(result.Succeeded);
            //TableUserLogin login = new TableUserLogin(user.Id, "testprovider", "testproviderkey");
            UserLoginInfo info = new UserLoginInfo("testprovider", "testproviderkey");
            result = await _mgr.AddLoginAsync(user.Id, info);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task Create_Identity()
        {
            TableUser user = new TableUser(userName);
            var result = await _mgr.CreateAsync(user);
            Assert.IsTrue(result.Succeeded);
            UserLoginInfo info = new UserLoginInfo("testprovider", "testproviderkey");
            result = await _mgr.AddLoginAsync(user.Id, info);
            var result2 = await _mgr.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);
            Assert.AreEqual(CookieAuthenticationDefaults.AuthenticationType,result2.AuthenticationType);
        }

        [TestMethod]
        public async Task Create_Claim()
        {
            TableUser user = new TableUser(userName);
            var result = await _mgr.CreateAsync(user);
            Assert.IsTrue(result.Succeeded);
            System.Security.Claims.Claim clm = new System.Security.Claims.Claim("type", "value", ClaimValueTypes.String, "testissuer");
            var result2 = await _mgr.AddClaimAsync(user.Id,clm);
            Assert.IsTrue(result2.Succeeded);
        }

        [TestCleanup]
        public void Cleanup()
        {
            //var storageAccount = CloudStorageAccount.Parse(connectionString);
            //var tableClient = storageAccount.CreateCloudTableClient();
            //var _userTable = tableClient.GetTableReference("users");
            //var _loginTable = tableClient.GetTableReference("logins");
            //var _claimsTable = tableClient.GetTableReference("roles");
            //var _rolesTable = tableClient.GetTableReference("claims");
            //_userTable.DeleteIfExists();
            //_loginTable.DeleteIfExists();
            //_claimsTable.DeleteIfExists();
            //_rolesTable.DeleteIfExists();
            //Thread.Sleep(40000);
        }
    }
}
