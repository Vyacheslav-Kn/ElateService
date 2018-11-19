using ElateService.BLL.ModelsDTO;
using System.Collections.Generic;

namespace ElateService.BLL.PaginationDTO
{
    ///<summary>
    ///Class contains information useful for single page and page output.
    ///</summary>
    public class ExecutorDTOPage
    {
        public IEnumerable<ExecutorDTO> ExecutorsOnPage { get; set; }
        public int NumberOfAllExecutorsWithSomeCategory { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
