using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Admin_API_SDK.Models;
using System.Collections.Generic;

namespace Admin_API_SDK
{
    public class DMAdmin
    {
        static string _baseUrl = "";
        static string _sessionKey = "";

        public DMAuthentication Authentication = new DMAuthentication();
        public DMAccount Account = new DMAccount();
        public DMSMTPGateway SMTPGateway = new DMSMTPGateway();

        public DMAdmin()
        {
            _baseUrl = "https://ssl.datamotion.com";
        }

        public DMAdmin(string url)
        {
            _baseUrl = url;
        }
        public class DMAuthentication
        {
            public string Encryption(byte[] json, byte[] key, byte[] IV)
            {

                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.IV = IV;
                aes.Key = key;

                using (ICryptoTransform encrypt = aes.CreateEncryptor())
                {
                    byte[] dest = encrypt.TransformFinalBlock(json, 0, json.Length);

                    return Convert.ToBase64String(dest);
                }
            }

            public async Task<string> GetSessionKey(string encryptionKey, string email, string automationID)
            {
                HttpClient client = new HttpClient();

                //1. Start AES encryption
                Aes aes = Aes.Create(); //Creates an Aes object
                aes.KeySize = 256; //Set the key length to 256

                aes.GenerateIV(); //Generates an initialization vector
                string iv = Convert.ToBase64String(aes.IV); //Converts the IV to a base64 string

                //2. Serialize Payload
                Authentication.GetSessionKeyRequest user = new Authentication.GetSessionKeyRequest
                {
                    Identity = new Authentication.IdentityObject
                    {
                        Email = email
                    },
                    //IpAddress = "",
                    TimeStamp = DateTime.UtcNow
                };

                string jsonString = JsonConvert.SerializeObject(user);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString); //JSON to byte array

                //3. Get Hash of Payload
                byte[] sharedKey = Convert.FromBase64String(encryptionKey); //Encryption key to byte array
                HMACSHA512 hash = new HMACSHA512(sharedKey); //Create a hash object using the encryption key
                hash.ComputeHash(jsonByteArray); //Compute the hash of the unencrypted payload
                string hashHeader = Convert.ToBase64String(hash.Hash); //Convert hash to base64 string for header value

                //4. Encrypt Payload
                string encryptedPayload = Encryption(jsonByteArray, sharedKey, aes.IV);

                client.DefaultRequestHeaders.Add("X-Email", email);
                client.DefaultRequestHeaders.Add("X-Iv", iv);
                client.DefaultRequestHeaders.Add("X-Company-Automation-ID", automationID);
                client.DefaultRequestHeaders.Add("X-Hash", hashHeader);

                HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Remote/Account/GetSessionKey", encryptedPayload);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                _sessionKey = JsonConvert.DeserializeObject<string>(responseString);

                return _sessionKey;
            }

            public async Task<string> GetEncryptionKey(Authentication.IdentityObject2 model)
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Remote/Account/GetEncryptionKey", model);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                string encryptionKey = JsonConvert.DeserializeObject<string>(responseString);

                return encryptionKey;
            }

            public async Task<string> NewEncryptionKey(Authentication.CreateEncryptionKeyRequest model)
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Remote/Account/NewEncryptionKey", model);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                string encryptionKey = JsonConvert.DeserializeObject<string>(responseString);

                return encryptionKey;
            }

            public async Task<string> RevokeSessionKey()
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/SecureMessagingAPI/Account/Logout", "");
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    _sessionKey = "";

                    return responseString;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }
        }

        public class DMAccount
        {
            public async Task<Account.ListUserAccountsResponse> ListUserAccounts(Account.ListUserAccountsRequest model)
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Remote/Account/List", model);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                Account.ListUserAccountsResponse accounts = JsonConvert.DeserializeObject<Account.ListUserAccountsResponse>(responseString);

                return accounts;
            }

            public async Task<Account.CreateUserResponse> CreateUserAccount(Account.CreateUserRequest model)
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Remote/Account/Create", model);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                Account.CreateUserResponse user = JsonConvert.DeserializeObject<Account.CreateUserResponse>(responseString);

                return user;
            }

            public async Task<Account.ViewUserResponse> ViewUserAccount(Account.ViewUserRequest model)
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Remote/Account/Read", model);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                Account.ViewUserResponse user = JsonConvert.DeserializeObject<Account.ViewUserResponse>(responseString);

                return user;
            }

            public async Task<Account.UpdateUserResponse> UpdateUserAccount(Account.UpdateUserRequest model)
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Remote/Account/Update", model);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                Account.UpdateUserResponse user = JsonConvert.DeserializeObject<Account.UpdateUserResponse>(responseString);

                return user;
            }

            public async void DeleteUser(Account.DeleteUserRequest model)
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Remote/Account/Delete", model);
                response.EnsureSuccessStatusCode();
            }

            public async Task<List<Account.GetUserTypesResponse>> GetUserTypes()
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Remote/Account/GetUserTypes");
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                List<Account.GetUserTypesResponse> userTypes = JsonConvert.DeserializeObject<List<Account.GetUserTypesResponse>>(responseString);

                return userTypes;
            }
        }

        public class DMSMTPGateway
        {
            public async Task<SMTP_Gateway.SMTPCredentialsResponse> GetCompanySMTPCredentials()
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Remote/SMTP/GetCredentials");
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                SMTP_Gateway.SMTPCredentialsResponse credentials = JsonConvert.DeserializeObject<SMTP_Gateway.SMTPCredentialsResponse>(responseString);

                return credentials;
            }

            public async Task<SMTP_Gateway.ResetPasswordResponse> ResetPassword()
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                HttpResponseMessage response = await client.PutAsJsonAsync(_baseUrl + "/Remote/SMTP/ResetPassword", -1);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                SMTP_Gateway.ResetPasswordResponse resetPasswordResponse = JsonConvert.DeserializeObject<SMTP_Gateway.ResetPasswordResponse>(responseString);

                return resetPasswordResponse;
            }

            public async Task<SMTP_Gateway.UpdateIPWhitelistResponse> UpdateIPWhitelist(List<SMTP_Gateway.EndpointsObject> model)
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                HttpResponseMessage response = await client.PutAsJsonAsync(_baseUrl + "/Remote/SMTP/PutSmtpEndpoints", model);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                SMTP_Gateway.UpdateIPWhitelistResponse whitelistResponse = JsonConvert.DeserializeObject<SMTP_Gateway.UpdateIPWhitelistResponse>(responseString);

                return whitelistResponse;
            }
        }
    }
}
