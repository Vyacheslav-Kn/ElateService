using System.Collections.Generic;

namespace ElateService.BLL.ModelsDTO
{
    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string Information { get; set; }
        public string ImgSrc { get; set; }
        public int? Mark { get; set; }

        public ISet<IndentDTO> Indents { get; set; }
    }
}
