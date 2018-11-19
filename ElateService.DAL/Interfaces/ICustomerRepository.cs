using ElateService.DAL.Entities;
using System.Threading.Tasks;

namespace ElateService.DAL.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByEmail(string email);
        Task<Customer> GetById(int id);
        Task<int?> Create(Customer customer);
        Task Update(Customer customer);

        ///<summary>
        ///Sets EmailConfirmed property to true.
        ///</summary>
        Task<Customer> ConfirmRegistration(int id, string confirmationCode);

        Task<Customer> VerifyNewConfirmationCode(int id, string confirmationCode);
        Task<Customer> UpdateConfirmationCode(string email, string newConfirmationCode);
        Task<Customer> UpdatePassword(int id, string salt, string passwordHash);
        Task SaveCustomerPropertiesAfterEdition(string information, string imgSrc, int id);

        ///<summary>
        ///Returns customer with its: recalls, indents.
        ///</summary>
        Customer GetProfile(int id);
    }
}
