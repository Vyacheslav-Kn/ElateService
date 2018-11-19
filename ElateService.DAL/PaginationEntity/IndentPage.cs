using ElateService.DAL.Entities;
using System.Collections.Generic;

namespace ElateService.DAL.PaginationEntity
{
    ///<summary>
    ///Class contains information useful for single page and page output.
    ///</summary>
    public class IndentPage
    {
        public IEnumerable<Indent> IndentsOnPage { get; set; }
        public int NumberOfAllIndentsWithSomeCategory { get; set; }
    }
}
