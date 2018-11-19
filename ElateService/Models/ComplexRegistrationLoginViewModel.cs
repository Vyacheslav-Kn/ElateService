namespace ElateService.Models
{
    public class ComplexRegistrationLoginViewModel
    {
        public LoginViewModel loginViewModel { get; set; }
        public RegistrationViewModel registrationViewModel { get; set; }

        public ComplexRegistrationLoginViewModel()
        {
            loginViewModel = new LoginViewModel();
            registrationViewModel = new RegistrationViewModel();
        }
    }
}