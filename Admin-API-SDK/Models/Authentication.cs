using System;

namespace DMWeb_REST_Admin.Models
{
    public class Authentication
    {
        public class IdentityObject
        {
            public int UID { get; set; }
            public string Email { get; set; }
            public string UserId { get; set; }
            public string SingleSignOnId { get; set; }
        }

        public class IdentityObject2
        {
            public string Email { get; set; }
        }
        public class GetSessionKeyRequest
        {
            public string IpAddress { get; set; }
            public DateTime TimeStamp { get; set; }
            public IdentityObject Identity { get; set;  }
        }

        public class GetEncryptionKeyRequest
        {
            public int UID { get; set; }
            public string Email { get; set; }
            public string UserId { get; set; }
            public string SingleSignOnId { get; set; }
        }

        public class CreateEncryptionKeyRequest
        {
            public int UID { get; set; }
            public string Email { get; set; }
            public string UserId { get; set; }
            public string SingleSignOnId { get; set; }
        }
    }
}
