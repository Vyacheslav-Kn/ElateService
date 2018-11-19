using ElateService.Common;
using System;
using System.Collections.Generic;

namespace ElateService.DAL.Entities
{
    public class Indent
    {
        public int IndentId { get; set; }
        public string Title { get; set; }
        public string IndentDescription { get; set; }
        public string City { get; set; }
        public DateTime IndentDate { get; set; }
        public double? Price { get; set; }
        public string ImgSrc { get; set; }
        public Category CategoryId { get; set; }
        public Executor Executor { get; set; }
        public Customer Customer { get; set; }
        public Recall Recall { get; set; }

        public ISet<Responce> Responces { get; set; }


        public bool Equals(Indent other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }
 
            return IndentId.Equals(other.IndentId) && Title.Equals(other.Title);
        }


        public override int GetHashCode()
        {
            int hashIndentTitle = Title == null ? 0 : Title.GetHashCode();
            
            int hashIndentId = IndentId.GetHashCode();
            
            return hashIndentTitle ^ hashIndentId;
        }        
    }

    public class IndentComparer : EqualityComparer<Indent>
    {
        public override bool Equals(Indent firstObject, Indent secondObject)
        {
            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(secondObject, firstObject)) return true;

            //Check whether the Responces' properties are equal. 
            return secondObject.IndentId.Equals(firstObject.IndentId);
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public override int GetHashCode(Indent item)
        {
            //Get hash code for the Id field. 
            int hashIndentId = item.IndentId.GetHashCode();

            //Calculate the hash code for the Responce. 
            return hashIndentId;
        }
    }

}
