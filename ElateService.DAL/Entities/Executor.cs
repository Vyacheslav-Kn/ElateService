using ElateService.Common;
using System;
using System.Collections.Generic;

namespace ElateService.DAL.Entities
{
    public class Executor
    {
        public int ExecutorId { get; set; }
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

        public ISet<CategoryExecutor> Categories { get; set; }
        public ISet<Responce> Responces { get; set; }
        public ISet<Indent> Indents { get; set; }


        public bool Equals(Executor other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            return ExecutorId.Equals(other.ExecutorId) && Email.Equals(other.Email);
        }


        public override int GetHashCode()
        {
            int hashExecutorEmail = Email == null ? 0 : Email.GetHashCode();
            
            int hashExecutorId = ExecutorId.GetHashCode();
            
            return hashExecutorEmail ^ hashExecutorId;
        }
    }


    public class ExecutorComparer : EqualityComparer<Executor>
    {
        public override bool Equals(Executor firstObject, Executor secondObject)
        {
            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(secondObject, firstObject)) return true;

            //Check whether the Responces' properties are equal. 
            return secondObject.ExecutorId.Equals(firstObject.ExecutorId);
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public override int GetHashCode(Executor item)
        {
            //Get hash code for the Id field. 
            int hashExecutorId = item.ExecutorId.GetHashCode();

            //Calculate the hash code for the Responce. 
            return hashExecutorId;
        }
    }
}
