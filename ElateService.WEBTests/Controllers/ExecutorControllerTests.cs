using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ElateService.BLL.Interfaces;
using ElateService.Controllers;
using System.Web.Mvc;
using AutoMapper;
using System.Threading.Tasks;
using ElateService.BLL.Models;
using ElateService.BLL.Infrastructure;
using ElateService.Models;
using System.Web;
using ElateService.Common;
using ElateService.BLL.ModelsDTO;
using ElateService.BLL.PaginationDTO;

namespace ElateService.WEBTests.Controllers
{
    [TestClass]
    public class ExecutorControllerTests
    {
        [TestMethod()]
        public void RegistrationTest()
        {
            //Arrange
            var iExecutorService = new Mock<IExecutorService>();
            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();
            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);

            //Act
            ViewResult registrationView = executorControllerTest.Registration() as ViewResult;

            //Asert
            Assert.IsNotNull(registrationView.Model);
            Assert.AreEqual(null, registrationView.ViewData["ValidationErrorMessageLogin"]);
        }


        [TestMethod()]
        public async Task RegistrationPostTest()
        {
            //Arrange
            string fakeLanguage = "test language";
            var iExecutorService = new Mock<IExecutorService>();
            iExecutorService.Setup(x => x.Register(It.IsAny<ClientDTO>(), fakeLanguage)).ThrowsAsync(new ValidationException("", ""));

            var iUserActivityService = new Mock<IUserActivityService>();

            ClientDTO fakeClientDTO = new ClientDTO();
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<ClientDTO>(It.IsAny<RegistrationViewModel>())).Returns(fakeClientDTO);

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Language"]).Returns(fakeLanguage);

            RegistrationViewModel registrationViewModel = new RegistrationViewModel();
            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);
            executorControllerTest.ControllerContext = controllerContext.Object;

            //Act
            PartialViewResult registrationView = await executorControllerTest.Registration(registrationViewModel) as PartialViewResult;

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
            var iExecutorService = new Mock<IExecutorService>();
            iExecutorService.Setup(x => x.ConfirmRegistration(testId, testConfirmationCode))
                .ThrowsAsync(new ValidationException("", ""));

            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);

            //Act
            dynamic statusCodeResult = await executorControllerTest.VerifyConfirmationCode(testId, testConfirmationCode);

            //Asert
            Assert.AreEqual(422, statusCodeResult.StatusCode);
        }


        [TestMethod()]
        public async Task LoginPostTest()
        {
            //Arrange
            var iExecutorService = new Mock<IExecutorService>();
            iExecutorService.Setup(x => x.Login(It.IsAny<ClientDTO>())).ThrowsAsync(new ValidationException("", ""));

            var iUserActivityService = new Mock<IUserActivityService>();

            ClientDTO fakeClientDTO = new ClientDTO();
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<ClientDTO>(It.IsAny<LoginViewModel>())).Returns(fakeClientDTO);

            LoginViewModel loginViewModel = new LoginViewModel();
            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);

            //Act
            RedirectToRouteResult redirect = await executorControllerTest.Login(loginViewModel) as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("Registration", redirect.RouteValues["action"]);
            Assert.AreEqual("Executor", redirect.RouteValues["controller"]);
        }


        [TestMethod()]
        public void ForgetPasswordTest()
        {
            //Arrange
            var iExecutorService = new Mock<IExecutorService>();
            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();
            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);

            //Act
            ViewResult view = executorControllerTest.ForgetPassword() as ViewResult;

            //Asert
            Assert.IsNotNull(view);
        }


        [TestMethod()]
        public async Task SetNewPasswordPostTest()
        {
            //Arrange
            int fakeId = 0;
            string fakePassword = "test password";
            var iExecutorService = new Mock<IExecutorService>();
            iExecutorService.Setup(x => x.SetNewPassword(fakeId, fakePassword)).ThrowsAsync(new ValidationException("", ""));

            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["ClientId"]).Returns(fakeId);

            string fakePasswordCopy = fakePassword;
            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);
            executorControllerTest.ControllerContext = controllerContext.Object;

            //Act
            ViewResult registrationView = await executorControllerTest.SetNewPassword(fakePassword, fakePasswordCopy) as ViewResult;

            //Asert
            Assert.AreEqual("EnterNewPassword", registrationView.ViewName);
        }


        [TestMethod()]
        public void PrivateOfficeTest()
        {
            //Arrange
            int? fakeId = null;
            var iExecutorService = new Mock<IExecutorService>();

            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(fakeId);

            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);
            executorControllerTest.ControllerContext = controllerContext.Object;

            //Act
            RedirectToRouteResult redirect = executorControllerTest.PrivateOffice() as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("Registration", redirect.RouteValues["action"]);
            Assert.AreEqual("Executor", redirect.RouteValues["controller"]);
        }


        [TestMethod()]
        public void CheckForNotificationsTest()
        {
            //Arrange
            var iExecutorService = new Mock<IExecutorService>();
            var mapper = new Mock<IMapper>();

            int fakeId = 0;
            var iUserActivityService = new Mock<IUserActivityService>();
            iUserActivityService.Setup(x => x.CheckForNewNotifications(Role.Executor, fakeId)).Returns(new NotificationDTO());

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(fakeId);

            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);
            executorControllerTest.ControllerContext = controllerContext.Object;

            //Act
            dynamic view = executorControllerTest.CheckForNotifications() as PartialViewResult;

            //Asert
            Assert.IsTrue(view.Model.isUserHasNewNotifications);
        }


        [TestMethod()]
        public async Task GetNotificationsTest()
        {
            //Arrange
            var iExecutorService = new Mock<IExecutorService>();
            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(null);

            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);
            executorControllerTest.ControllerContext = controllerContext.Object;

            //Act
            RedirectToRouteResult redirect = await executorControllerTest.GetNotifications() as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("Registration", redirect.RouteValues["action"]);
            Assert.AreEqual("Executor", redirect.RouteValues["controller"]);
        }


        [TestMethod()]
        public async Task GetIndentsWithResponceTest()
        {
            //Arrange 
            var iExecutorService = new Mock<IExecutorService>();
            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(null);

            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);
            executorControllerTest.ControllerContext = controllerContext.Object;

            //Act
            RedirectToRouteResult redirect = await executorControllerTest.GetIndentsWithResponce() as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("Registration", redirect.RouteValues["action"]);
            Assert.AreEqual("Executor", redirect.RouteValues["controller"]);
        }


        [TestMethod()]
        public void ModificateProfileTest()
        {
            //Arrange 
            int fakeId = 0;
            ExecutorDTO fakeExecutorDTO = new ExecutorDTO();
            var iExecutorService = new Mock<IExecutorService>();
            iExecutorService.Setup(x => x.GetExecutorPropertiesForEdition(fakeId)).Returns(fakeExecutorDTO);

            var iUserActivityService = new Mock<IUserActivityService>();

            ExecutorPropertiesForEditionViewModel fakeViewModel = new ExecutorPropertiesForEditionViewModel();
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<ExecutorPropertiesForEditionViewModel>(It.IsAny<ExecutorDTO>())).Returns(fakeViewModel);

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(fakeId);

            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);
            executorControllerTest.ControllerContext = controllerContext.Object;

            //Act
            ViewResult view = executorControllerTest.ModificateProfile() as ViewResult;

            //Asert
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.Model);
        }


        [TestMethod()]
        public async Task ModificateProfilePostTest()
        {
            //Arrange
            var iExecutorService = new Mock<IExecutorService>();
            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);
            controllerContext.Setup(p => p.HttpContext.Session["Id"]).Returns(null);

            string fakeInformation = "test information";
            string fakeCategoryFirst = "test category";
            string fakeCategorySecond = fakeCategoryFirst;
            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);
            executorControllerTest.ControllerContext = controllerContext.Object;

            //Act
            RedirectToRouteResult redirect = await executorControllerTest.ModificateProfile(fakeInformation, fakeCategoryFirst,
                fakeCategorySecond) as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("Registration", redirect.RouteValues["action"]);
            Assert.AreEqual("Executor", redirect.RouteValues["controller"]);
        }


        [TestMethod()]
        public void ShowExecutorTest()
        {
            //Arrange
            int? fakeId = null;
            var iExecutorService = new Mock<IExecutorService>();

            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);

            //Act
            RedirectToRouteResult redirect = executorControllerTest.ShowExecutor(fakeId) as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("ShowExecutorsPerPage", redirect.RouteValues["action"]);
            Assert.AreEqual("Executor", redirect.RouteValues["controller"]);
        }


        [TestMethod()]
        public async Task ShowExecutorsPerPageTest()
        {
            //Arrange
            int fakePageNumber = 0;
            string[] fakeCategories = null;
            ExecutorDTOPage fakePage = new ExecutorDTOPage();
            var iExecutorService = new Mock<IExecutorService>();
            iExecutorService.Setup(x => x.GetExecutorsPerPage(-1, 1, null)).ReturnsAsync(fakePage);

            var iUserActivityService = new Mock<IUserActivityService>();

            ExecutorPageViewModel fakeViewModel = new ExecutorPageViewModel();
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<ExecutorPageViewModel>(It.IsAny<ExecutorDTOPage>())).Returns(fakeViewModel);

            var controllerContext = new Mock<ControllerContext>();
            var controllerSession = new Mock<HttpSessionStateBase>();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(controllerSession.Object);

            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);
            executorControllerTest.ControllerContext = controllerContext.Object;

            //Act
            ViewResult view = await executorControllerTest.ShowExecutorsPerPage(fakePageNumber, fakeCategories) as ViewResult;

            //Asert
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.Model);
        }


        [TestMethod()]
        public async Task SearchTest()
        {
            //Arrange
            string fakeSearchString = " ";
            var iExecutorService = new Mock<IExecutorService>();

            var iUserActivityService = new Mock<IUserActivityService>();
            var mapper = new Mock<IMapper>();

            ExecutorController executorControllerTest = new ExecutorController(iExecutorService.Object,
                iUserActivityService.Object, mapper.Object);

            //Act
            RedirectToRouteResult redirect = await executorControllerTest.Search(fakeSearchString) as RedirectToRouteResult;

            //Asert
            Assert.AreEqual("ShowExecutorsPerPage", redirect.RouteValues["action"]);
            Assert.AreEqual("Executor", redirect.RouteValues["controller"]);
        }

    }
}
