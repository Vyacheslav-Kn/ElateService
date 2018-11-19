using ElateService.BLL.ModelsDTO;
using ElateService.Common;
using System;
using System.Collections.Generic;

namespace ElateService.Models
{
    public class IndentPageViewModel
    {
        public IEnumerable<IndentViewModel> IndentsOnPage { get; set; }
        public int NumberOfAllIndentsWithSomeCategory { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string[] AvailableCategories { get; set; }
    }
}