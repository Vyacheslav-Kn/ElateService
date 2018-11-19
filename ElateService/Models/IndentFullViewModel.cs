namespace ElateService.Models
{
    public class IndentFullViewModel
    {
        public IndentViewModel indentViewModel { get; set; }
        public HelpModelForClientOpportunities clientOpportunities { get; set; }

        public IndentFullViewModel()
        {
            indentViewModel = new IndentViewModel();
            clientOpportunities = new HelpModelForClientOpportunities();
        }
    }
}