using ElateService.DAL.Entities;
using System.Collections.Generic;
 
namespace ElateService.DAL.PaginationEntity
{
    ///<summary>
    ///Class contains information useful for single page and page output.
    ///</summary>
    public class ExecutorPage
    {
        public IEnumerable<Executor> ExecutorsOnPage { get; set; }
        public int NumberOfAllExecutorsWithSomeCategory { get; set; }
    }
}
