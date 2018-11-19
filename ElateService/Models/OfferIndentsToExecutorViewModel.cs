using ElateService.BLL.ModelsDTO;
using System.Collections.Generic;

namespace ElateService.Models
{
    public class OfferIndentsToExecutorViewModel
    {
        public IEnumerable<IndentDTO> Indents { get; set; }
        public int ToId { get; set; }
        public int FromId { get; set; }
        public string FromName { get; set; }
    }
}