using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Collections.Generic;
using Moq;
using ElateService.BLL.Interfaces;
using AutoMapper;
using ElateService.Models;
using System.Threading.Tasks;
using ElateService.BLL.Models;
using ElateService.AutoMapper;
using ElateService.BLL.Infrastructure;
using System.Web;
using ElateService.Common;
using ElateService.BLL.ModelsDTO;
using System.Web.Caching;

namespace ElateService.Controllers.Tests
{
    [TestClass()]
    public class CustomerControllerTests
    {
        [TestMethod()]
        public void RegistrationTest()
        {
            //Arrange
            var iCustomerService = new Mock<ICustomerService>();
            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();
            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object, 
                iUserActivityService.Object, mapper.Object);          
            
            //Act
            ViewResult registrationView = customerControllerTest.Registration() as ViewResult;

            //Asert
            Assert.IsNotNull(registrationView.Model);
            Assert.AreEqual(null, registrationView.ViewData["ValidationErrorMessageLogin"]);
        }


        [TestMethod()]
        public async Task RegistrationPostTest() 
        {
            //Arrange
            string fakeLanguage = "test language";
            var iCustomerService = new Mock<ICustomerService>();
            iCustomerService.Setup(x => x.Register(It.IsAny<ClientDTO>(), fakeLanguage)).ThrowsAsync(new ValidationException("", ""));

            var iUserActivityService = new Mock<IUserActivityService>();

            ClientDTO fakeClientDTO = new ClientDTO();
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<ClientDTO>(It.IsAny<RegistrationViewModel>())).Returns(fakeClientDTO);

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Language"]).Returns(fakeLanguage);

            RegistrationViewModel registrationViewModel = new RegistrationViewModel();
            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);
            customerControllerTest.ControllerContext = controllerContext.Object;

            //Act
            PartialViewResult registrationView = await customerControllerTest.Registration(registrationViewModel) as PartialViewResult;

            //Asert
            Assert.IsNotNull(registrationView.Model);
            Assert.AreEqual("RegistrationPartial", registrationView.ViewName);
        }


        [TestMethod()]
        public async Task VerifyConfirmationCodeTest()
        {
            //Arrange
            int testId = 0;
            string testConfirmationCode = "test code";
            var iCustomerService = new Mock<ICustomerService>();
            iCustomerService.Setup(x => x.ConfirmRegistration(testId, testConfirmationCode))
                .ThrowsAsync(new ValidationException("", ""));

            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();
            
            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);

            //Act
            dynamic statusCodeResult = await customerControllerTest.VerifyConfirmationCode(testId, testConfirmationCode);            

            //Asert
            Assert.AreEqual(422, statusCodeResult.StatusCode);
        }


        [TestMethod()]
        public async Task LoginPostTest()
        {
            //Arrange
            var iCustomerService = new Mock<ICustomerService>();
            iCustomerService.Setup(x => x.Login(It.IsAny<ClientDTO>())).ThrowsAsync(new ValidationException("", ""));

            var iUserActivityService = new Mock<IUserActivityService>();

            ClientDTO fakeClientDTO = new ClientDTO();
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<ClientDTO>(It.IsAny<LoginViewModel>())).Returns(fakeClientDTO);

            LoginViewModel loginViewModel = new LoginViewModel();
            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);

            //Act
            RedirectToRouteResult redirect = await customerControllerTest.Login(loginViewModel) as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("Registration", redirect.RouteValues["action"]);
            Assert.AreEqual("Customer", redirect.RouteValues["controller"]);
        }


        [TestMethod()]
        public void ForgetPasswordTest()
        {
            //Arrange
            var iCustomerService = new Mock<ICustomerService>();
            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();
            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);

            //Act
            ViewResult view = customerControllerTest.ForgetPassword() as ViewResult;

            //Asert
            Assert.IsNotNull(view);
        }


        [TestMethod()]
        public async Task ForgetPasswordPostTest()
        {
            //Arrange
            string fakeEmail = "test email";
            string fakeLanguage = "test language";
            var iCustomerService = new Mock<ICustomerService>();
            iCustomerService.Setup(x => x.GenerateNewConfirmationCode(fakeEmail, fakeLanguage)).ThrowsAsync(new ValidationException("", ""));

            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Language"]).Returns(fakeLanguage);

            RegistrationViewModel registrationViewModel = new RegistrationViewModel();
            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);
            customerControllerTest.ControllerContext = controllerContext.Object;

            //Act
            PartialViewResult view = await customerControllerTest.ForgetPassword(fakeEmail) as PartialViewResult;

            //Asert
            Assert.AreEqual("ForgetPasswordPartial", view.ViewName);
        }


        [TestMethod()]
        public async Task SetNewPasswordPostTest()
        {
            //Arrange
            int fakeId = 0;
            string fakePassword = "test password";
            var iCustomerService = new Mock<ICustomerService>();
            iCustomerService.Setup(x => x.SetNewPassword(fakeId, fakePassword)).ThrowsAsync(new ValidationException("", ""));

            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["ClientId"]).Returns(fakeId);

            string fakePasswordCopy = fakePassword;
            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);
            customerControllerTest.ControllerContext = controllerContext.Object;

            //Act
            ViewResult registrationView = await customerControllerTest.SetNewPassword(fakePassword, fakePasswordCopy) as ViewResult;

            //Asert
            Assert.AreEqual("EnterNewPassword", registrationView.ViewName);
        }


        [TestMethod()]
        public void PrivateOfficeTest()
        {
            //Arrange
            int? fakeId = null;
            var iCustomerService = new Mock<ICustomerService>();

            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(fakeId);

            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);
            customerControllerTest.ControllerContext = controllerContext.Object;

            //Act
            RedirectToRouteResult redirect = customerControllerTest.PrivateOffice() as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("Registration", redirect.RouteValues["action"]);
            Assert.AreEqual("Customer", redirect.RouteValues["controller"]);
        }


        [TestMethod()]
        public void CheckForNotificationsTest()
        {
            //Arrange
            var iCustomerService = new Mock<ICustomerService>();
            var mapper = new Mock<IMapper>();

            int fakeId = 0;
            var iUserActivityService = new Mock<IUserActivityService>();
            iUserActivityService.Setup(x => x.CheckForNewNotifications(Role.Customer, fakeId)).Returns(new NotificationDTO());

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(fakeId);

            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);
            customerControllerTest.ControllerContext = controllerContext.Object;

            //Act
            dynamic view = customerControllerTest.CheckForNotifications() as PartialViewResult;

            //Asert
            Assert.IsTrue(view.Model.isUserHasNewNotifications);
        }


        [TestMethod()]
        public async Task GetNotificationsTest()
        {
            //Arrange
            var iCustomerService = new Mock<ICustomerService>();
            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(null);

            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);
            customerControllerTest.ControllerContext = controllerContext.Object;

            //Act
            RedirectToRouteResult redirect = await customerControllerTest.GetNotifications() as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("Registration", redirect.RouteValues["action"]);
            Assert.AreEqual("Customer", redirect.RouteValues["controller"]);
        }


        [TestMethod()]
        public async Task GetCustomerIndentsTest()
        {
            //Arrange 
            var iCustomerService = new Mock<ICustomerService>();
            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(null);

            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);
            customerControllerTest.ControllerContext = controllerContext.Object;

            //Act
            RedirectToRouteResult redirect = await customerControllerTest.GetCustomerIndents() as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("Registration", redirect.RouteValues["action"]);
            Assert.AreEqual("Customer", redirect.RouteValues["controller"]);
        }


        [TestMethod()]
        public async Task ModificateProfileTest()
        {
            //Arrange 
            int fakeId = 0;
            CustomerDTO fakeCustomerDTO = new CustomerDTO();
            var iCustomerService = new Mock<ICustomerService>();
            iCustomerService.Setup(x => x.GetCustomerPropertiesForEdition(fakeId)).ReturnsAsync(fakeCustomerDTO);

            var iUserActivityService = new Mock<IUserActivityService>();

            CustomerPropertiesForEditionViewModel fakeViewModel = new CustomerPropertiesForEditionViewModel();
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<CustomerPropertiesForEditionViewModel>(It.IsAny<CustomerDTO>())).Returns(fakeViewModel);

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(fakeId);

            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);
            customerControllerTest.ControllerContext = controllerContext.Object;

            //Act
            ViewResult view = await customerControllerTest.ModificateProfile() as ViewResult;

            //Asert
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.Model);
        }


        [TestMethod()]
        public async Task ModificateProfilePostTest()
        {
            //Arrange
            var iCustomerService = new Mock<ICustomerService>();
            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(null);

            string fakeInformation = "test information";
            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);
            customerControllerTest.ControllerContext = controllerContext.Object;

            //Act
            RedirectToRouteResult redirect = await customerControllerTest.ModificateProfile(fakeInformation) as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("Registration", redirect.RouteValues["action"]);
            Assert.AreEqual("Customer", redirect.RouteValues["controller"]);
        }


        [TestMethod()]
        public void CreateIndentTest()
        {
            //Arrange
            var iCustomerService = new Mock<ICustomerService>();
            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            int fakeId = 0;
            string fakeLanguage = "en";
            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(fakeId);
            controllerContext.Setup(p => p.HttpContext.Session["Language"]).Returns(fakeLanguage);

            var tempData = new Mock<TempDataDictionary>();
            tempData.Object.Add("ErrorMessage", "test value");

            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);
            customerControllerTest.ControllerContext = controllerContext.Object;
            customerControllerTest.TempData = tempData.Object;

            //Act
            ViewResult view = customerControllerTest.CreateIndent() as ViewResult;

            //Asert
            Assert.IsNotNull(view.Model);
            Assert.IsNotNull(view.ViewData["ErrorMessage"]);
        }


        [TestMethod()]
        public void ShowCustomerTest()
        {
            //Arrange
            int? fakeId = null;
            var iCustomerService = new Mock<ICustomerService>();

            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            CustomerController customerControllerTest = new CustomerController(iCustomerService.Object,
                iUserActivityService.Object, mapper.Object);

            //Act
            RedirectToRouteResult redirect = customerControllerTest.ShowCustomer(fakeId) as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("Index", redirect.RouteValues["action"]);
            Assert.AreEqual("Home", redirect.RouteValues["controller"]);
        }

    }
}