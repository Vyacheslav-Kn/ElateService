using ElateService.Common;
using System;
using System.Collections.Generic;

namespace ElateService.Models
{
    public class ExecutorPageViewModel
    {
        public IEnumerable<ExecutorViewModel> ExecutorsOnPage { get; set; }
        public int NumberOfAllExecutorsWithSomeCategory { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string[] Categories { get; set; }
        public string[] AvailableCategories { get; set; }

        public ExecutorPageViewModel()
        {
            string[] indentCategories = Enum.GetNames(typeof(Category));
            Categories = CategoryExtension.TranslateFromEnumToRussianEquivalents(indentCategories);
        }
    }
}