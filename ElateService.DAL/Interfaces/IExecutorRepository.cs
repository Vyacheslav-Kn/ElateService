using ElateService.DAL.Entities;
using ElateService.DAL.PaginationEntity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElateService.DAL.Interfaces
{
    public interface IExecutorRepository
    {
        Task<Executor> GetByEmail(string email);
        Executor GetById(int id);
        Task<int?> Create(Executor executor);
        Task Update(Executor executor);

        ///<summary>
        ///Sets EmailConfirmed property to true.
        ///</summary>
        Task<Executor> ConfirmRegistration(int id, string confirmationCode);

        Task<Executor> VerifyNewConfirmationCode(int id, string confirmationCode);
        Task<Executor> UpdateConfirmationCode(string email, string newConfirmationCode);
        Task<Executor> UpdatePassword(int id, string salt, string passwordHash);
        Task SaveExecutorPropertiesAfterEdition(string information, string imgSrc, List<int?> categories, int id);

        ///<summary>
        ///Returns executor with its: categories, recalls, responces.
        ///</summary>
        Executor GetProfile(int id);
        
        ///<summary>
        ///Returns executors according to some page, page size and categories.
        ///</summary>
        Task<ExecutorPage> GetExecutorsPerPage(int page, int pageSize, List<int> categories);

        ///<summary>
        ///Returns executors which name, surname or patronymic contains searchString.
        ///</summary>
        Task<IEnumerable<Executor>> SearchInNames(string searchString);
    }
}