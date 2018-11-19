using ElateService.BLL.ModelsDTO;
using ElateService.BLL.PaginationDTO;
using ElateService.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElateService.BLL.Interfaces
{
    public interface IExecutorService: IEntranceService
    {
        ExecutorDTO GetExecutorPropertiesForEdition(int id); 
        Task SaveExecutorPropertiesAfterEdition(ExecutorDTO properties);

        ///<summary>
        ///Returns executor with its: categories, recalls, responces.
        ///</summary>
        ExecutorDTO GetExecutorProfile(int id);

        ///<summary>
        ///Returns executors according to some page and categories.
        ///</summary>
        Task<ExecutorDTOPage> GetExecutorsPerPage(int page, int pageSize, List<Category> categories);

        ///<summary>
        ///Returns executors which name, surname or patronymic contains searchString.
        ///</summary>
        Task<IEnumerable<ExecutorDTO>> Search(string searchString);
    }
}
