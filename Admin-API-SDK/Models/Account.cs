using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin_API_SDK.Models
{
    public class Account
    {
        public class ListUserAccountsRequest
        {
            public int UserTypeId { get; set; }
            public int PageNumber { get; set; }
            public string CompanyId { get; set; }
            public string OrderBy { get; set; }
            public string Filter { get; set; }
        }

        public class AccountsObject
        {
            public int UniqueId { get; set; }
            public int Aid { get; set; }
            public int CompanyId { get; set; }
            public DateTime Created { get; set; }
            public Nullable<DateTime> LastNotice { get; set; }
            public string DiskSpaceUsed { get; set; }
            public int EmailId { get; set; }
            public string Email { get; set; }
            public string EmployeeId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Nullable<DateTime> LastLogin { get; set; }
            public int MessagesReceived { get; set; }
            public int MessagesSent { get; set; }
            public string UserId { get; set; }
            public int UserTypeId { get; set; }
            public Dictionary<string, List<string>> Errors { get; set; }
        }
        public class ListUserAccountsResponse
        {
            public AccountsObject[] Accounts { get; set; }
            public int Count { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
            public int TotalUsers { get; set; }
            public int PageSize { get; set; }
        }

        public class CreateUserRequest
        {
            public string Email { get; set; }
            public string UserId { get; set; }
            public string SingleSignOnId { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public int CompanyId { get; set; }
            public string EmployeeId { get; set; }
            public string Miscellaneous { get; set; }
            public bool Disabled { get; set; }
            public int UserTypeId { get; set; }
            public bool ReceiveOffers { get; set; }
        }

        public class CreateUserResponse
        {
            public int UniqueId { get; set; }
            public string Email { get; set; }
            public string UserId { get; set; }
            public string SingleSignOnId { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public int CompanyId { get; set; }
            public string EmployeeId { get; set; }
            public string Miscellaneous { get; set; }
            public bool Disabled { get; set; }
            public int UserTypeId { get; set; }
            public bool ButtonUser { get; set; }
            public bool ReceiveOffers { get; set; }
            public Dictionary<string,List<string>> Errors { get; set; }

        }
        public class ViewUserRequest
        {
            public int UID { get; set; }
            public string Email { get; set; }
            public string UserId { get; set; }
            public string SingleSignOnId { get; set; }
        }

        public class ViewUserResponse
        {
            public int UniqueId { get; set; }
            public string Email { get; set; }
            public string UserId { get; set; }
            public string SingleSignOnId { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public int CompanyId { get; set; }
            public string EmployeeId { get; set; }
            public string Miscellaneous { get; set; }
            public bool Disabled { get; set; }
            public int UserTypeId { get; set; }
            public bool ButtonUser { get; set; }
            public bool ReceiveOffers { get; set; }
            public Dictionary<string, List<string>> Errors { get; set; }
        }

        public class UpdateUserRequest
        {
            public int UniqueId { get; set; }
            public string Email { get; set; }
            public string UserId { get; set; }
            public string SingleSignOnId { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public int CompanyId { get; set; }
            public string EmployeeId { get; set; }
            public string Miscellaneous { get; set; }
            public bool Disabled { get; set; }
            public int UserTypeId { get; set; }
            public bool ButtonUser { get; set; }
            public bool ReceiveOffers { get; set; }
        }

        public class UpdateUserResponse
        {
            public int UniqueId { get; set; }
            public string Email { get; set; }
            public string UserId { get; set; }
            public string SingleSignOnId { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public int CompanyId { get; set; }
            public string EmployeeId { get; set; }
            public string Miscellaneous { get; set; }
            public bool Disabled { get; set; }
            public int UserTypeId { get; set; }
            public bool ButtonUser { get; set; }
            public bool ReceiveOffers { get; set; }
            public Dictionary<string, List<string>> Errors { get; set; }
        }

        public class DeleteUserRequest
        {
            public int UID { get; set; }
            public string Email { get; set; }
            public string UserId { get; set; }
            public string SingleSignOnId { get; set; }
        }

        public class GetUserTypesResponse
        {
            public int TypeId { get; set; }
            public string Description { get; set; }
            public int MinimumPasswordLength { get; set; }
            public int RequiredPasswordCategories { get; set; }
            public string[] Categories { get; set; }
        }
    }
}
