using NUnit.Framework;
using System.IO;
using System.Net.Http;
using System.Reflection;
using DMWeb_REST_Admin;
using DMWeb_REST_Admin.Models;

namespace DMWeb_REST_Admin_Unit_Tests
{
    public class SMTPGatewayContext
    {
        public static DMAdmin admin = new DMAdmin("https://ssl.datamotion.com/Remote");
    }

    [TestFixture]
    public class SMTPGatewayTests
    {
        static string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string rootPath = Directory.GetParent(assemblyFolder).Parent.Parent.FullName;
        private static string _unitTestPath = Path.Combine(rootPath, "Admin-API-Unit Tests-461");
        private static string _credentialsPath = Path.Combine(_unitTestPath, "Test Credentials");
        private string _credentialsDataPath = Path.Combine(_credentialsPath, "Credentials.txt");

        [Test]
        [Order(3)]
        [Category("No Session Key")]
        [Category("Get Session Key")]
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

        [Category("Get Company SMTP Credentials")]
        [Category("Positive")]
        public void GetCompanySMTPCredentialsPositiveTest()
        {
            SMTP_Gateway.SMTPCredentialsResponse response = new SMTP_Gateway.SMTPCredentialsResponse();

            response = SMTPGatewayContext.admin.SMTPGateway.GetCompanySMTPCredentials().GetAwaiter().GetResult();
        }

        [Test]
        [Order(1)]
        [Category("Get Company SMTP Credentials")]
        [Category("No Session Key")]
        [Category("Negative")]
        public void GetCompanySMTPCredentialsNegativeTest()
        {
            SMTP_Gateway.SMTPCredentialsResponse response = new SMTP_Gateway.SMTPCredentialsResponse();

            try
            {
                response = SMTPGatewayContext.admin.SMTPGateway.GetCompanySMTPCredentials().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Get Company SMTP Credentials")]
        [Category("Positive")]
        public void ResetPasswordPositiveTest()
        {
            SMTP_Gateway.ResetPasswordResponse response = new SMTP_Gateway.ResetPasswordResponse();

            SMTPGatewayContext.admin.SMTPGateway.ResetPassword().GetAwaiter().GetResult();
        }

        [Test]
        [Order(2)]
        [Category("Get Company SMTP Credentials")]
        [Category("No Session Key")]
        [Category("Negative")]
        public void ResetPasswordNegativeTest()
        {
            SMTP_Gateway.ResetPasswordResponse response = new SMTP_Gateway.ResetPasswordResponse();

            try
            {
                SMTPGatewayContext.admin.SMTPGateway.ResetPassword().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }
    }
}