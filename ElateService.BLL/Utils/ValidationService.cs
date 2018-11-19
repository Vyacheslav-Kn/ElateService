using ElateService.BLL.Models;
using System;
using System.Text.RegularExpressions;

namespace ElateService.BLL.Services
{
    public static class ValidationService
    {
        public static bool ValidateRegistration(ClientDTO clientRegistrationDTO)
        {
            bool isModelValid = false;

            Regex forNamesRegex = new Regex(@"^[a-zа-я]{2,20}$", RegexOptions.IgnoreCase);

            Regex forMobilePhoneRegex = new Regex(@"^\+375\(\d{2}\)\s\d{3}\-\d{2}\-\d{2}$");

            Regex forEmailRegex = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
             @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);

            Regex forPasswordRegex = new Regex(@"[\-\=\%\@\*\/]+", RegexOptions.IgnoreCase);

            try
            {
                if (!forNamesRegex.IsMatch(clientRegistrationDTO.FirstName.Trim()) || !forNamesRegex.IsMatch(clientRegistrationDTO.Surname.Trim())
                || !forNamesRegex.IsMatch(clientRegistrationDTO.Patronymic.Trim()))
                {
                    return isModelValid;
                }

                if (!forMobilePhoneRegex.IsMatch(clientRegistrationDTO.MobilePhone.Trim()))
                {
                    return isModelValid;
                }

                if (!forEmailRegex.IsMatch(clientRegistrationDTO.Email.Trim()))
                {
                    return isModelValid;
                }

                if (forPasswordRegex.IsMatch(clientRegistrationDTO.Password.Trim()))
                {
                    return isModelValid;
                }
            }
            catch (ArgumentNullException e)
            {
                return isModelValid;
            }

            isModelValid = true;

            return isModelValid;
        }


        public static bool ValidateLogin(ClientDTO clientLoginDTO)
        {
            bool isModelValid = false;

            Regex forEmailRegex = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
             @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);

            Regex forPasswordRegex = new Regex(@"[\-\=\%\@\*\/]+", RegexOptions.IgnoreCase);

            try
            {
                if (!forEmailRegex.IsMatch(clientLoginDTO.Email.Trim()))
                {
                    return isModelValid;
                }

                if (forPasswordRegex.IsMatch(clientLoginDTO.Password.Trim()))
                {
                    return isModelValid;
                }
            }
            catch (ArgumentNullException e)
            {
                return isModelValid;
            }

            isModelValid = true;

            return isModelValid;
        }


        public static bool ValidateEmail(string email)
        {
            bool isModelValid = false;

            Regex forEmailRegex = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
             @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);

            try
            {
                if (!forEmailRegex.IsMatch(email.Trim()))
                {
                    return isModelValid;
                }
            }
            catch (ArgumentNullException e)
            {
                return isModelValid;
            }

            isModelValid = true;

            return isModelValid;
        }


        public static bool ValidateConfirmationCode(string confirmationCode)
        {
            bool isModelValid = false;

            Regex forConfirmationCodeRegex = new Regex(@"^[A-Z0-9]+$", RegexOptions.None);

            try
            {
                if (!forConfirmationCodeRegex.IsMatch(confirmationCode))
                {
                    return isModelValid;
                }
            }            
            catch (ArgumentNullException e)
            {
                return isModelValid;
            }

            isModelValid = true;
            return isModelValid;
        }
    }
}
