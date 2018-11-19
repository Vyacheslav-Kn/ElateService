using ElateService.DAL.Entities;

namespace ElateService.BLL.ModelsDTO
{
    public class RecallDTO
    {
        public int RecallId { get; set; }
        public string CustomerCommentForExecutor { get; set; }
        public int? CustomerMarkForExecutor { get; set; }
        public string ExecutorCommentForCustomer { get; set; }
        public int? ExecutorMarkForCustomer { get; set; }
    }
}
