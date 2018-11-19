using AutoMapper;
using ElateService.BLL.AutoMapper;
using ElateService.BLL.Infrastructure;
using ElateService.BLL.Interfaces;
using ElateService.BLL.Models;
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
    public class ExecutorService : IExecutorService
    {
        private IUnitOfWork _database;
        private readonly IMapper _mapper;

        public ExecutorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _database = unitOfWork;
            _mapper = mapper;
        }

        
        public async Task Register(ClientDTO clientRegistrationDTO, string language)
        {
            bool isModelValid = ValidationService.ValidateRegistration(clientRegistrationDTO);

            if (!isModelValid)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            Executor executor = new Executor();
            string salt = null;

            executor = _mapper.Map<Executor>(clientRegistrationDTO);

            executor.PasswordHash = CryptoService.CreatePasswordHash(clientRegistrationDTO.Password, out salt);
            executor.Salt = salt;
            executor.ConfirmationCode = CryptoService.GenerateConfirmationCode();
            executor.EmailConfirmed = false;
            executor.RoleId = Role.Executor;

            int? idOfInsertedExecutor = await _database.Executors.Create(executor);

            if (idOfInsertedExecutor == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            try
            {
                await EmailService.SendConfirmationCode(executor.Email, executor.RoleId, executor.ConfirmationCode,
                    idOfInsertedExecutor, language);
            }
            catch (Exception e)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }
        }


        public async Task<string> ConfirmRegistration(int id, string confirmationCode)
        {
            bool isConfirmationCodeValid = ValidationService.ValidateConfirmationCode(confirmationCode);

            if (!isConfirmationCodeValid)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            Executor executor = await _database.Executors.ConfirmRegistration(id, confirmationCode);

            if (executor == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            return executor.FirstName;
        }

        
        public async Task GenerateNewConfirmationCode(string email, string language)
        {
            bool isEmailValid = ValidationService.ValidateEmail(email);

            if (!isEmailValid)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            string newConfirmationCode = CryptoService.GenerateConfirmationCode();

            Executor executor = await _database.Executors.UpdateConfirmationCode(email, newConfirmationCode);

            if (executor == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            try
            {
                await EmailService.SendNewConfirmationCode(executor.Email, executor.RoleId, executor.ConfirmationCode,
                    executor.ExecutorId, language);
            }
            catch (Exception e)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }
        }


        public async Task<string> SetNewPassword(int id, string password)
        {
            string salt;
            string passwordHash = CryptoService.CreatePasswordHash(password, out salt);

            Executor executor = await _database.Executors.UpdatePassword(id, salt, passwordHash);

            if (executor == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            return executor.FirstName;
        }


        public async Task VerifyNewConfirmationCode(int id, string confirmationCode)
        {
            bool isConfirmationCodeValid = ValidationService.ValidateConfirmationCode(confirmationCode);

            if (!isConfirmationCodeValid)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            Executor executor = await _database.Executors.VerifyNewConfirmationCode(id, confirmationCode);

            if (executor == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }
        }


        public async Task<ClientDTO> Login(ClientDTO clientLoginDTO)
        {
            bool isModelValid = ValidationService.ValidateLogin(clientLoginDTO);

            if (!isModelValid)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            Executor executor = await _database.Executors.GetByEmail(clientLoginDTO.Email);

            if (executor == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            bool isInputPasswordCorrect = CryptoService.VerifyPassword(executor.PasswordHash, executor.Salt, clientLoginDTO.Password);

            if (!isInputPasswordCorrect)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            ClientDTO minLoginInfo = _mapper.Map<ClientDTO>(executor);

            return minLoginInfo;
        }


        public ExecutorDTO GetExecutorPropertiesForEdition(int id)
        {
            Executor executor = _database.Executors.GetById(id);
            
            ExecutorDTO executorPropertiesForEdition = _mapper.Map<ExecutorDTO>(executor);

            return executorPropertiesForEdition;
        }


        public async Task SaveExecutorPropertiesAfterEdition(ExecutorDTO properties)
        {
            List<int?> categoriesValue = null;

            if(properties.Categories != null)
            {
                categoriesValue = new List<int?>();
                foreach (Category category in properties.Categories)
                {
                    categoriesValue.Add((int)category);
                }
            }

            await _database.Executors.SaveExecutorPropertiesAfterEdition(properties.Information, properties.ImgSrc, 
                categoriesValue, properties.ExecutorId);
        }


        public ExecutorDTO GetExecutorProfile(int id)
        {
            Executor executor = _database.Executors.GetProfile(id);

            if (executor == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            ExecutorDTO executorDTO = _mapper.Map<ExecutorDTO>(executor);

            return executorDTO;
        }


        public async Task<ExecutorDTOPage> GetExecutorsPerPage(int page, int pageSize, List<Category> categories)
        {
            List<int> categoriesValues = null;

            if (categories != null)
            {
                categoriesValues = new List<int>();
                foreach (Category category in categories)
                {
                    categoriesValues.Add((int)category);
                }
            }

            ExecutorPage executorsSinglePageModel = await _database.Executors.GetExecutorsPerPage(page, pageSize, categoriesValues);

            ExecutorDTOPage executorsDTOSinglePageModel = _mapper.Map<ExecutorDTOPage>(executorsSinglePageModel);
            executorsDTOSinglePageModel.Page = ++page;
            executorsDTOSinglePageModel.PageSize = pageSize;

            return executorsDTOSinglePageModel;
        }


        public async Task<IEnumerable<ExecutorDTO>> Search(string searchString)
        {
            IEnumerable<Executor> executors = await _database.Executors.SearchInNames(searchString);

            IEnumerable<ExecutorDTO> executorsDTO = _mapper.Map<IEnumerable<Executor>, IEnumerable<ExecutorDTO>>(executors);

            return executorsDTO;
        }

    }
}
