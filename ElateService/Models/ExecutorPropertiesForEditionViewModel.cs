using ElateService.Common;
using System.Collections.Generic;

namespace ElateService.Models
{
    public class ExecutorPropertiesForEditionViewModel
    {
        public string Information { get; set; }
        public string ImgSrc { get; set; }

        public List<Category> Categories { get; set; }
    }
}