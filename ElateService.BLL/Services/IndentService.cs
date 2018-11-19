using AutoMapper;
using ElateService.BLL.Infrastructure;
using ElateService.BLL.Interfaces;
using ElateService.BLL.ModelsDTO;
using ElateService.BLL.PaginationDTO;
using ElateService.Common;
using ElateService.DAL.Entities;
using ElateService.DAL.Interfaces;
using ElateService.DAL.PaginationEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElateService.BLL.Services
{
    public class IndentService: IIndentService
    {
        private IUnitOfWork _database;
        private readonly IMapper _mapper;

        public IndentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _database = unitOfWork;
            _mapper = mapper;
        }


        public async Task<int?> Create(IndentDTO indentDTO)
        {
            Indent indent = _mapper.Map<Indent>(indentDTO);

            int? indentId = await _database.Indents.Create(indent);

            if(indentId == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            return indentId;
        }


        public IndentDTO Get(int id)
        {
            Indent indent = _database.Indents.GetById(id);

            if(indent == null)
            {
                throw new ValidationException("Заказ не найден!","");
            }

            IndentDTO indentDTO = _mapper.Map<IndentDTO>(indent);

            return indentDTO;
        }


        public async Task<IndentDTOPage> GetIndentsPerPage(int page, int pageSize, List<Category> categories)
        {
            List<int> categoriesValues = null;

            if(categories != null)
            {
                categoriesValues = new List<int>();

                foreach (Category category in categories)
                {
                    categoriesValues.Add((int)category);
                }
            }            

            IndentPage  indentsSinglePageModel = await _database.Indents.GetIndentsPerPage(page, pageSize, categoriesValues);

            IndentDTOPage indentsDTOSinglePageModel = _mapper.Map<IndentDTOPage>(indentsSinglePageModel);
            indentsDTOSinglePageModel.Page = ++page;
            indentsDTOSinglePageModel.PageSize = pageSize;

            return indentsDTOSinglePageModel;
        }


        public async Task<IEnumerable<IndentDTO>> Search(string searchString)
        {
            IEnumerable<Indent> indents = await _database.Indents.SearchByTitle(searchString);

            IEnumerable<IndentDTO> indentsDTO = _mapper.Map<IEnumerable<Indent>, IEnumerable<IndentDTO>>(indents);

            return indentsDTO;
        }

    }
}
