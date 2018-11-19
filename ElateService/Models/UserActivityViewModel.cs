namespace ElateService.Models
{
    ///<summary>
    ///Defines properties which contain information we need when user performs: write recall/responce, choose executor for indent.
    ///</summary>
    public class UserActivityViewModel
    {
        public int IndentId { get; set; }
        public string IndentTitle { get; set; }
        public int? Mark { get; set; }
        public double? Price { get; set; }
        public string Comment { get; set; }
        public int UserOpponentId { get; set; }
    }
}