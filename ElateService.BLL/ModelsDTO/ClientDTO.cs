using ElateService.Common;

namespace ElateService.BLL.Models
{
    public class ClientDTO
    {
        public int ClientId { get; set; }
        public Role RoleId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string Password { get; set; }
    }
}
