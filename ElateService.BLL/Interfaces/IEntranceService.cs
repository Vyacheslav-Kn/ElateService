using ElateService.BLL.Models;
using System.Threading.Tasks;

namespace ElateService.BLL.Interfaces
{
    public interface IEntranceService
    {
        ///<summary>
        ///Saves clients entity and sends confirmation code to email.
        ///</summary>
        Task Register(ClientDTO clientRegistrationDTO, string language);

        Task<string> ConfirmRegistration(int id, string confirmationCode);

        ///<summary>
        ///Generates new confirmation code and sends verification link to email.
        ///</summary>
        Task GenerateNewConfirmationCode(string email, string language);

        Task<string> SetNewPassword(int id, string newPassword);
        Task VerifyNewConfirmationCode(int id, string confirmationCode);
        Task<ClientDTO> Login(ClientDTO clientLoginDTO);
    }
}
