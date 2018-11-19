namespace ElateService.Models
{
    ///<summary>
    ///Contains properties which allow user to: write recall/responce, choose executor for indent.
    ///</summary>
    public class HelpModelForClientOpportunities
    {
        public bool customerIsAllowToSelectExecutor { get; set; }
        public bool customerIsAllowToWriteRecall { get; set; }
        public bool executorIsAllowToSendResponce { get; set; }
        public bool executorIsAllowToWriteRecall { get; set; }

        public HelpModelForClientOpportunities()
        {
            customerIsAllowToSelectExecutor = false;
            customerIsAllowToWriteRecall = false;
            executorIsAllowToSendResponce = false;
            executorIsAllowToWriteRecall = false;
        }
    }
}