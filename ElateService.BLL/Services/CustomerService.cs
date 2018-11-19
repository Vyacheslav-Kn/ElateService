using AutoMapper;
using ElateService.BLL.AutoMapper;
using ElateService.BLL.Infrastructure;
using ElateService.BLL.Interfaces;
using ElateService.BLL.Models;
using ElateService.BLL.ModelsDTO;
using ElateService.Common;
using ElateService.DAL.Entities;
using ElateService.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElateService.BLL.Services
{
    public class CustomerService: ICustomerService
    {
        private IUnitOfWork _database;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
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

            Customer customer = new Customer();
            string salt = null;

            customer = _mapper.Map<Customer>(clientRegistrationDTO);

            customer.PasswordHash = CryptoService.CreatePasswordHash(clientRegistrationDTO.Password, out salt);
            customer.Salt = salt;
            customer.ConfirmationCode = CryptoService.GenerateConfirmationCode(); 
            customer.EmailConfirmed = false;
            customer.RoleId = Role.Customer;                     

            int? idOfInsertedCustomer = await _database.Customers.Create(customer);

            if (idOfInsertedCustomer == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            try
            {
                await EmailService.SendConfirmationCode(customer.Email, customer.RoleId, customer.ConfirmationCode,
                    idOfInsertedCustomer, language);
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

            Customer customer = await _database.Customers.ConfirmRegistration(id, confirmationCode);

            if (customer == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            return customer.FirstName;
        }


        public async Task GenerateNewConfirmationCode(string email, string language)
        {
            bool isEmailValid = ValidationService.ValidateEmail(email);

            if (!isEmailValid)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            string newConfirmationCode = CryptoService.GenerateConfirmationCode();

            Customer customer = await _database.Customers.UpdateConfirmationCode(email, newConfirmationCode);

            if (customer == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            try
            {
                await EmailService.SendNewConfirmationCode(customer.Email, customer.RoleId, customer.ConfirmationCode,
                    customer.CustomerId, language);
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

            Customer customer = await _database.Customers.UpdatePassword(id, salt, passwordHash);

            if (customer == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            return customer.FirstName;
        }


        public async Task VerifyNewConfirmationCode(int id, string confirmationCode)
        {
            bool isConfirmationCodeValid = ValidationService.ValidateConfirmationCode(confirmationCode);

            if (!isConfirmationCodeValid)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            Customer customer = await _database.Customers.VerifyNewConfirmationCode(id, confirmationCode);

            if (customer == null)
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

            Customer customer = await _database.Customers.GetByEmail(clientLoginDTO.Email); 

            if (customer == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            bool isInputPasswordCorrect = CryptoService.VerifyPassword(customer.PasswordHash, customer.Salt, clientLoginDTO.Password);

            if (!isInputPasswordCorrect)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            ClientDTO minLoginInfo = _mapper.Map<ClientDTO>(customer);

            return minLoginInfo;
        }


        public async Task<CustomerDTO> GetCustomerPropertiesForEdition(int id)
        {
            Customer customer = await _database.Customers.GetById(id);

            CustomerDTO customerPropertiesForEdition = _mapper.Map<CustomerDTO>(customer);

            return customerPropertiesForEdition;
        }


        public async Task SaveCustomerPropertiesAfterEdition(string information, string imgSrc, int id)
        {
            await _database.Customers.SaveCustomerPropertiesAfterEdition(information, imgSrc, id);
        }


        public CustomerDTO GetCustomerProfile(int id)
        {
            Customer customer =  _database.Customers.GetProfile(id);

            if (customer == null)
            {
                throw new ValidationException("Были введены некорректные данные, попробуйте снова!", "");
            }

            CustomerDTO customerDTO = _mapper.Map<CustomerDTO>(customer);

            return customerDTO;
        }


    }
}
