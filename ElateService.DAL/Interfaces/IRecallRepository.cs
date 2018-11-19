using ElateService.DAL.Entities;
using System.Threading.Tasks;

namespace ElateService.DAL.Interfaces
{
    public interface IRecallRepository
    {
        ///<summary>
        ///Saves customer recall for executor.
        ///</summary>
        Task AddCustomerRecallPropertiesForExecutor(Recall recall);

        ///<summary>
        ///Saves executor recall for customer.
        ///</summary>
        Task AddExecutorRecallPropertiesForCustomer(Recall recall); 
    }
}
