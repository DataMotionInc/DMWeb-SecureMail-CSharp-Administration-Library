using System.Collections.Generic;

namespace DMWeb_REST_Admin.Models
{
    public class SMTP_Gateway
    {
        public class EndpointsObject
        {
            public string IpAddress { get; set; }
            public string Subnet { get; set; }
            public bool Incoming { get; set; }
            public bool Outgoing { get; set; }
            public int Port { get; set; }
            public string Domain { get; set; }
            public Dictionary<string, List<string>> Errors { get; set; }
        }
        public class SMTPCredentialsResponse
        {
            public string UserId { get; set; }
            public string Password { get; set; }
            public EndpointsObject[] Endpoints { get; set; }
        }

        public class ResetPasswordResponse
        {
            public string UserId { get; set; }
            public string Password { get; set; }
            public EndpointsObject[] Endpoints { get; set; }
        }

        public class UpdateIPWhitelistRequest
        {
            public EndpointsObject[] Endpoint { get; set; }
        }

        public class UpdateIPWhitelistResponse
        {
            public string UserId { get; set; }
            public string Password { get; set; }
            public EndpointsObject[] Endpoints { get; set; }
        }
    }
}
