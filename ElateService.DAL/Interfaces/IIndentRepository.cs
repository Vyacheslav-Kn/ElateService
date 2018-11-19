using ElateService.DAL.Entities;
using ElateService.DAL.PaginationEntity;
using System.Collections.Generic;
using System.Threading.Tasks;
 
namespace ElateService.DAL.Interfaces
{
    public interface IIndentRepository
    {
        Task<int?> Create(Indent indent);
        Indent GetById(int id);
        Task SetExecutorId(Indent indent);
        Task<IEnumerable<Indent>> GetIndentsByCustomerId(int id);
        Task<IEnumerable<Indent>> GetIndentsWithExecutorResponce(int id);

        ///<summary>
        ///Returns indents according to some page, page size and categories.
        ///</summary>
        Task<IndentPage> GetIndentsPerPage(int page, int pageSize, List<int> categories);

        ///<summary>
        ///Returns indents which contain searchString parameter in their title.
        ///</summary>
        Task<IEnumerable<Indent>> SearchByTitle(string title);

        ///<summary>
        ///Returns customers indents which don't have any executor and have category in range of future executor(executorId).
        ///</summary>
        Task<IEnumerable<Indent>> GetFreeCustomerIndentsForExecutor(int customerId, int executorId);
    }
}
