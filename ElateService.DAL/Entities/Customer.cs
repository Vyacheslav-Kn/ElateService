using ElateService.Common;
using System;
using System.Collections.Generic;

namespace ElateService.DAL.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string ConfirmationCode { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string MobilePhone { get; set; }
        public string Information { get; set; }
        public string ImgSrc { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public int? Mark { get; set; }
        public Role RoleId { get; set; } 

        public ISet<Indent> Indents { get; set; }


        public bool Equals(Customer other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            return  Email.Equals(other.Email);
        }


        public override int GetHashCode()
        {
            int hashCustomerEmail = Email == null ? 0 : Email.GetHashCode();
                        
            return hashCustomerEmail ;
        }
    }
}
