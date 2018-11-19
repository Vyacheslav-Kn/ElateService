using System;

namespace ElateService.DAL.Entities
{
    public class ImageIndent
    {
        public int Id { get; set; }
        public string ImgSrc { get; set; }
        public Indent Indent {get;set;}


        public bool Equals(ImageIndent other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }
            
            return Id.Equals(other.Id) && ImgSrc.Equals(other.ImgSrc);
        }
        

        public override int GetHashCode()
        {
            int hashImgSrc = ImgSrc == null ? 0 : ImgSrc.GetHashCode();
            
            int hashId = Id.GetHashCode();
            
            return hashImgSrc ^ hashId;
        }
    }
}
