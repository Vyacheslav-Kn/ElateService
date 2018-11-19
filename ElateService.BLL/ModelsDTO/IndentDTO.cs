using ElateService.Common;
using System;
using System.Collections.Generic;

namespace ElateService.BLL.ModelsDTO
{
    public class IndentDTO
    {
        public int IndentId { get; set; }
        public string Title { get; set; }
        public string IndentDescription { get; set; }
        public string City { get; set; }
        public DateTime IndentDate { get; set; }
        public double? Price { get; set; }
        public string ImgSrc { get; set; }
        public Category CategoryId { get; set; }
        public ExecutorDTO Executor { get; set; }
        public CustomerDTO Customer { get; set; }
        public RecallDTO Recall { get; set; }

        public ISet<ResponceDTO> Responces { get; set; }
    }
}
