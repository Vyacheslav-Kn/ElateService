using ElateService.Common;
using System;
using System.Collections.Generic;

namespace ElateService.DAL.Entities
{
    public class CategoryExecutor
    {
        public int Id { get; set; }
        public Category CategoryId { get; set; }
        public Executor Executor { get; set; }


        public bool Equals(CategoryExecutor other)
        {
            if (Object.ReferenceEquals(other, null)) {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }
            
            return CategoryId.Equals(other.CategoryId);
        }


        public override int GetHashCode()
        {            
            int hashCategoryId = CategoryId.GetHashCode();
 
            return hashCategoryId;
        }
    }


    public class CategoryComparer : EqualityComparer<CategoryExecutor>
    {
        public override bool Equals(CategoryExecutor firstObject, CategoryExecutor secondObject)
        {
            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(secondObject, firstObject)) return true;

            //Check whether the Responces' properties are equal. 
            return secondObject.CategoryId.Equals(firstObject.CategoryId);
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public override int GetHashCode(CategoryExecutor item)
        {
            //Get hash code for the Id field. 
            int hashCategoryId = item.CategoryId.GetHashCode();

            //Calculate the hash code for the Responce. 
            return hashCategoryId;
        }
    }
}
