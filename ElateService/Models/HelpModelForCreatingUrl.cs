namespace ElateService.Models
{
    ///<summary>
    ///Contains properties which allow to build link to logined user's private office.
    ///</summary>
    public class HelpModelForCreatingUrl
    {
        public int ClientId { get; set; }
        public string ClientFirstName { get; set; }
        public string ClientRole { get; set; }
    }
}