using ElateService.BLL.ModelsDTO;
using System.Collections.Generic;

namespace ElateService.BLL.PaginationDTO
{
    ///<summary>
    ///Class contains information useful for single page and page output.
    ///</summary>
    public class IndentDTOPage
    {
        public IEnumerable<IndentDTO> IndentsOnPage { get; set; }
        public int NumberOfAllIndentsWithSomeCategory { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
