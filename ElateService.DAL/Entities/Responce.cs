using System;
using System.Collections.Generic;

namespace ElateService.DAL.Entities
{
    public class Responce
    {
        public int ResponceId { get; set; }
        public string ResponceText { get; set; }
        public double? Price { get; set; }
        public Indent Indent { get; set; }
        public Executor Executor { get; set; }


        public bool Equals(Responce other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }
 
            return ResponceId.Equals(other.ResponceId) && ResponceText.Equals(other.ResponceText);
        }
        

        public override int GetHashCode()
        {
            int hashResponceText = ResponceText == null ? 0 : ResponceText.GetHashCode();
            
            int hashResponceId = ResponceId.GetHashCode();
            
            return hashResponceText ^ hashResponceId;
        }
    }


    public class ResponceComparer : EqualityComparer<Responce>
    {
        public override bool Equals(Responce firstObject, Responce secondObject)
        {
            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(secondObject, firstObject)) return true;

            //Check whether the Responces' properties are equal. 
            return secondObject.ResponceId.Equals(firstObject.ResponceId) && secondObject.ResponceText.Equals(firstObject.ResponceText);
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public override int GetHashCode(Responce item)
        {
            //Get hash code for the Id field. 
            int hashResponceId = item.ResponceId.GetHashCode();

            //Calculate the hash code for the Responce. 
            return hashResponceId;
        }
    }
}
