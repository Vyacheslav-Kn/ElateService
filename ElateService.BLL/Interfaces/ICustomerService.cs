using ElateService.BLL.ModelsDTO;
using System.Threading.Tasks;

namespace ElateService.BLL.Interfaces
{
    public interface ICustomerService: IEntranceService
    {
        Task<CustomerDTO> GetCustomerPropertiesForEdition(int id);
        Task SaveCustomerPropertiesAfterEdition(string information, string imgSrc, int id);

        ///<summary>
        ///Returns customer with its: recalls, indents.
        ///</summary>
        CustomerDTO GetCustomerProfile(int id);
    }
}
