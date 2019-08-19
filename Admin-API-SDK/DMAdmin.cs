using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using DMWeb_REST_Admin.Models;

namespace DMWeb_REST_Admin
{
    public class DMAdmin
    {
        public static HttpClient client = new HttpClient();
        public static string _baseUrl = "";
        public static string _sessionKey = "";

        public DMAuthentication Authentication = new DMAuthentication();
        public DMAccount Account = new DMAccount();
        public DMSMTPGateway SMTPGateway = new DMSMTPGateway();

        public DMAdmin()
        {
            _baseUrl = "https://ssl.datamotion.com/Remote";
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        public DMAdmin(string url)
        {
            _baseUrl = url;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
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

                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/GetSessionKey", encryptedPayload);

                    client.DefaultRequestHeaders.Remove("X-Email");
                    client.DefaultRequestHeaders.Remove("X-Iv");
                    client.DefaultRequestHeaders.Remove("X-Company-Automation-ID");
                    client.DefaultRequestHeaders.Remove("X-Hash");
                    client.DefaultRequestHeaders.Remove("X-Session-Key");

                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    _sessionKey = JsonConvert.DeserializeObject<string>(responseString);

                    client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                    return _sessionKey;
                }
                catch (HttpRequestException ex)
                {
                    client.DefaultRequestHeaders.Remove("X-Email");
                    client.DefaultRequestHeaders.Remove("X-Iv");
                    client.DefaultRequestHeaders.Remove("X-Company-Automation-ID");
                    client.DefaultRequestHeaders.Remove("X-Hash");

                    throw ex;
                }
            }

            public async Task<string> GetEncryptionKey(Authentication.IdentityObject2 model)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/GetEncryptionKey", model);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    string encryptionKey = JsonConvert.DeserializeObject<string>(responseString);

                    return encryptionKey;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            public async Task<string> NewEncryptionKey(Authentication.CreateEncryptionKeyRequest model)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/NewEncryptionKey", model);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    string encryptionKey = JsonConvert.DeserializeObject<string>(responseString);

                    return encryptionKey;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            public async Task<string> RevokeSessionKey()
            {
                string baseUrl = _baseUrl;
                baseUrl = baseUrl.Replace("/Remote", "");

                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(baseUrl + "/SecureMessagingAPI/Account/Logout", "");
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    _sessionKey = "";
                    client.DefaultRequestHeaders.Remove("X-Session-Key");

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
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/List", model);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    Account.ListUserAccountsResponse accounts = JsonConvert.DeserializeObject<Account.ListUserAccountsResponse>(responseString);

                    return accounts;
                }
                catch(HttpRequestException ex)
                {
                    throw ex;
                }
            }

            public async Task<Account.CreateUserResponse> CreateUserAccount(Account.CreateUserRequest model)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/Create", model);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    Account.CreateUserResponse user = JsonConvert.DeserializeObject<Account.CreateUserResponse>(responseString);

                    return user;
                }
                catch(HttpRequestException ex)
                {
                    throw ex;
                }
            }

            public async Task<Account.ViewUserResponse> ViewUserAccount(Account.ViewUserRequest model)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/Read", model);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    Account.ViewUserResponse user = JsonConvert.DeserializeObject<Account.ViewUserResponse>(responseString);

                    return user;
                }
                catch(HttpRequestException ex)
                {
                    throw ex;
                }
            }

            public async Task<Account.UpdateUserResponse> UpdateUserAccount(Account.UpdateUserRequest model)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/Update", model);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    Account.UpdateUserResponse user = JsonConvert.DeserializeObject<Account.UpdateUserResponse>(responseString);

                    return user;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            public async Task DeleteUser(Account.DeleteUserRequest model)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/Delete", model);
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            public async Task<List<Account.GetUserTypesResponse>> GetUserTypes()
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Account/GetUserTypes");
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    List<Account.GetUserTypesResponse> userTypes = JsonConvert.DeserializeObject<List<Account.GetUserTypesResponse>>(responseString);

                    return userTypes;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }
        }

        public class DMSMTPGateway
        {
            public async Task<SMTP_Gateway.SMTPCredentialsResponse> GetCompanySMTPCredentials()
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/SMTP/GetCredentials");
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    SMTP_Gateway.SMTPCredentialsResponse credentials = JsonConvert.DeserializeObject<SMTP_Gateway.SMTPCredentialsResponse>(responseString);

                    return credentials;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            public async Task<SMTP_Gateway.ResetPasswordResponse> ResetPassword()
            {
                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync(_baseUrl + "/SMTP/ResetPassword", -1);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    SMTP_Gateway.ResetPasswordResponse resetPasswordResponse = JsonConvert.DeserializeObject<SMTP_Gateway.ResetPasswordResponse>(responseString);

                    return resetPasswordResponse;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            public async Task<SMTP_Gateway.UpdateIPWhitelistResponse> UpdateIPWhitelist(List<SMTP_Gateway.EndpointsObject> model)
            {
                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync(_baseUrl + "/SMTP/PutSmtpEndpoints", model);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    SMTP_Gateway.UpdateIPWhitelistResponse whitelistResponse = JsonConvert.DeserializeObject<SMTP_Gateway.UpdateIPWhitelistResponse>(responseString);

                    return whitelistResponse;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }
        }
    }
}
