using ElateService.BLL.ModelsDTO;
using ElateService.BLL.PaginationDTO;
using ElateService.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElateService.BLL.Interfaces
{
    public interface IIndentService
    {
        Task<int?> Create(IndentDTO indent);
        IndentDTO Get(int id);

        ///<summary>
        ///Returns indents according to some page and categories.
        ///</summary>
        Task<IndentDTOPage> GetIndentsPerPage(int page, int pageSize, List<Category> categories);

        ///<summary>
        ///Returns indents which title contains searchString.
        ///</summary>
        Task<IEnumerable<IndentDTO>> Search(string searchString);
    }
}
