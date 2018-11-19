using AutoMapper;
using ElateService.Authorization;
using ElateService.BLL.Infrastructure;
using ElateService.BLL.Interfaces;
using ElateService.BLL.Models;
using ElateService.Common;
using ElateService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using ElateService.BLL.ModelsDTO;
using System.IO;
using ElateService.Localization;
using ElateService.Safe_execution;

namespace ElateService.Controllers
{
    [Culture, RoleAuthorize(new object[] { Role.Customer }), Exception]
    public class CustomerController : Controller
    {
        private ICustomerService _customerService;
        private IUserActivityService _userActivityService;
        private readonly IMapper _mapper;
        private IAuthenticationManager _authenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public CustomerController(ICustomerService customerService, IUserActivityService userActivityService, IMapper mapper)
        {
            _customerService = customerService;
            _userActivityService = userActivityService;
            _mapper = mapper;
        }


        [AllowAnonymous]
        public ActionResult Registration()
        {
            if (TempData["ValidationErrorMessageLogin"] != null)
            {
                ViewBag.ValidationErrorMessageLogin = TempData["ValidationErrorMessageLogin"];      
            }

            return View(new ComplexRegistrationLoginViewModel());
        }

        
        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Registration(RegistrationViewModel registrationViewModel)
        {
            ClientDTO clientRegistrationDTO = _mapper.Map<ClientDTO>(registrationViewModel);
            string language = Session["Language"].ToString();

            try
            {
               await _customerService.Register(clientRegistrationDTO, language);
            }
            catch (ValidationException e)
            {
                ViewBag.ValidationErrorMessage = e.Message;
                ModelState.AddModelError(" "," ");

                return PartialView("RegistrationPartial", registrationViewModel);
            }

            return PartialView("ConfirmationCodePartial");
        }


        [AllowAnonymous]
        public async Task<ActionResult> VerifyConfirmationCode(int id, string confirmationCode)
        {
            string customerName = null;

            try
            {
               customerName = await _customerService.ConfirmRegistration(id, confirmationCode);
            }
            catch (ValidationException e)
            {
                // there is no customer with such id, confirmation code and email confirmed
                return new HttpStatusCodeResult(422);
            }

            Session["Name"] = customerName;
            Session["Id"] = id;

            ClaimsIdentity claim = new ClaimsIdentity("ApplicationCookie");
            claim.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                "OWIN Provider", ClaimValueTypes.String));
            claim.AddClaim(new Claim(ClaimTypes.Role, Role.Customer.ToString(), ClaimValueTypes.String));

            _authenticationManager.SignOut();
            _authenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, claim);

            return RedirectToAction("Index","Home");
        }


        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            ClientDTO clientLoginDTO = _mapper.Map<ClientDTO>(loginViewModel);

            ClientDTO minLoginInfo = new ClientDTO();

            try
            {
                minLoginInfo = await _customerService.Login(clientLoginDTO);
            }
            catch (ValidationException e)
            {
                TempData["ValidationErrorMessageLogin"] = e.Message;

                return RedirectToAction("Registration", "Customer");
            }

            Session["Name"] = minLoginInfo.FirstName;
            Session["Id"] = minLoginInfo.ClientId;

            ClaimsIdentity claim = new ClaimsIdentity("ApplicationCookie");
            claim.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                "OWIN Provider", ClaimValueTypes.String));
            claim.AddClaim(new Claim(ClaimTypes.Role, minLoginInfo.RoleId.ToString(), ClaimValueTypes.String));

            _authenticationManager.SignOut();
            _authenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, claim);

            return RedirectToAction("PrivateOffice", "Customer");
        }


        [AllowAnonymous]
        public ActionResult ForgetPassword()
        {
            return View();
        }


        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgetPassword(string email)
        {
            string language = Session["Language"].ToString();

            try
            {
                await _customerService.GenerateNewConfirmationCode(email, language);
            }
            catch(ValidationException e)
            {
                ViewBag.ValidationErrorForgetPasswordEmail = e.Message;

                return PartialView("ForgetPasswordPartial");
            }

            return PartialView("NewPasswordMessage");
        }


        [AllowAnonymous]
        public async Task<ActionResult> EnterNewPassword(int id, string confirmationCode)
        {
            try
            {
               await _customerService.VerifyNewConfirmationCode(id, confirmationCode);
            }
            catch (ValidationException e)
            {
                // there is no customer with such id, confirmation code and email confirmed
                return new HttpStatusCodeResult(422);
            }

            Session["ClientId"] = id;

            return View("EnterNewPassword");
        }


        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetNewPassword(string password, string passwordCopy)
        {
            var clientId = Session["ClientId"];

            if(clientId == null || !password.Equals(passwordCopy))
            {
                ViewBag.ValidationErrorForPasswordRefresh = "Были введены  некорректные данные, попробуйте снова!";

                return View("EnterNewPassword");
            }

            string clientName = null;

            try
            {
                clientName = await _customerService.SetNewPassword(Convert.ToInt32(clientId), password);
            }
            catch (ValidationException e)
            {
                ViewBag.ValidationErrorForPasswordRefresh = e.Message;

                return View("EnterNewPassword");
            }

            Session["Name"] = clientName;
            Session["Id"] = Convert.ToInt32(clientId);
            Session["ClientId"] = null;

            ClaimsIdentity claim = new ClaimsIdentity("ApplicationCookie");
            claim.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                "OWIN Provider", ClaimValueTypes.String));
            claim.AddClaim(new Claim(ClaimTypes.Role, Role.Customer.ToString(), ClaimValueTypes.String));

            _authenticationManager.SignOut();
            _authenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, claim);

            return RedirectToAction("Index", "Home");
        }


        ///<summary>
        ///Returns view of customer profile which contains: recalls, indents, personal data.
        ///Uses _PrivateOfficeCustomerLayout
        ///</summary>
        public ActionResult PrivateOffice()
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration","Customer");
            }

            int customerId = Convert.ToInt32(clientId);
            CustomerDTO customerDTO = null;

            string cacheKey = "show-customer-" + customerId.ToString();
            customerDTO = HttpContext.Cache[cacheKey] as CustomerDTO;

            if (customerDTO == null)
            {
                try
                {
                    customerDTO = _customerService.GetCustomerProfile(customerId);
                }
                catch (ValidationException e)
                {
                    return RedirectToAction("Registration", "Customer");
                }

                HttpContext.Cache.Insert(cacheKey, customerDTO, null, DateTime.Now.AddSeconds(120), TimeSpan.Zero);
            }            

            UserProfileViewModel userProfileView = _mapper.Map<UserProfileViewModel>(customerDTO);

            return View(userProfileView);
        }


        ///<summary>
        ///Returns red circle pointer near menu item, which shows if customer has notifications.
        ///</summary>
        public ActionResult CheckForNotifications()
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Customer");
            }

            int customerId = Convert.ToInt32(clientId);
            NotificationsViewModel notificationsViewModel = new NotificationsViewModel();

            NotificationDTO notificationDTO = new NotificationDTO();
            notificationDTO = _userActivityService.CheckForNewNotifications(Role.Customer, customerId);

            if(notificationDTO != null)
            {
                notificationsViewModel.isUserHasNewNotifications = true;
            }

            return PartialView(notificationsViewModel);
        }


        public async Task<ActionResult> GetNotifications()
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Customer");
            }

            int customerId = Convert.ToInt32(clientId);            

            IEnumerable<NotificationDTO> notificationsDTO = await _userActivityService.GetNotifications(Role.Customer,customerId);

            NotificationsViewModel notificationsViewModel = new NotificationsViewModel();

            notificationsViewModel.Notifications = notificationsDTO;

            return View(notificationsViewModel);
        }


        public async Task<ActionResult> GetCustomerIndents()
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Customer");
            }

            int customerId = Convert.ToInt32(clientId);
            IEnumerable<IndentDTO> indentsDTO = await _userActivityService.GetCustomerIndents(customerId);

            IEnumerable<IndentViewModel> indentsView = _mapper.Map<IEnumerable<IndentDTO>, IEnumerable<IndentViewModel>>(indentsDTO);            

            return View(indentsView);
        }


        ///<summary>
        ///Allows customer to modificate profile: download photo, change description.
        ///</summary>
        public async Task<ActionResult> ModificateProfile()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Customer");
            }

            int customerId = Convert.ToInt32(clientId);

            CustomerDTO customerPropertiesForEdition = await _customerService.GetCustomerPropertiesForEdition(customerId);

   CustomerPropertiesForEditionViewModel propertiesViewModel = _mapper.Map<CustomerPropertiesForEditionViewModel>(customerPropertiesForEdition);

            return View(propertiesViewModel);
        }


        [HttpPost]
        public async Task<ActionResult> ModificateProfile(string information)
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Customer");
            }

            int customerId = Convert.ToInt32(clientId);

            string imgSrc = null;
            string fileMimeType = null;

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                fileMimeType = file.ContentType;

                if (!fileMimeType.Equals("application/octet-stream"))
                {
                    if (fileMimeType.Equals("image/jpeg"))
                    {
                        imgSrc = Path.Combine(Server.MapPath("~/Content/Customer"), customerId.ToString() + ".jpeg");
                        if (System.IO.File.Exists(imgSrc))
                        {
                            System.IO.File.Delete(imgSrc);
                        }
                        file.SaveAs(imgSrc);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Загружать фотографии разрешается только в jpeg формате!";

                        return RedirectToAction("ModificateProfile", "Customer");
                    }
                }
            }

            await _customerService.SaveCustomerPropertiesAfterEdition(information, imgSrc, customerId);

            string cacheKeyForCustomer = "show-customer-" + customerId.ToString();
            CustomerDTO cachedCustomer = HttpContext.Cache[cacheKeyForCustomer] as CustomerDTO;
            if (cachedCustomer != null)
            {
                HttpContext.Cache.Remove(cacheKeyForCustomer);
            }

            return RedirectToAction("PrivateOffice","Customer");
        }


        public ActionResult CreateIndent()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Customer");
            }

            IndentViewModel indentViewModel = new IndentViewModel();

            return View(indentViewModel);
        }


        ///<summary>
        ///Returns view of customer profile which contains: recalls, indents, personal data.
        //////Uses _Layout
        ///</summary>
        [AllowAnonymous]
        public ActionResult ShowCustomer(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int customerId = Convert.ToInt32(id);
            CustomerDTO customerDTO = null;

            string cacheKey = "show-customer-" + customerId.ToString();
            customerDTO = HttpContext.Cache[cacheKey] as CustomerDTO;

            if (customerDTO == null)
            {
                try
                {
                    customerDTO = _customerService.GetCustomerProfile(customerId);
                }
                catch (ValidationException e)
                {
                    return new HttpStatusCodeResult(404);
                }

                HttpContext.Cache.Insert(cacheKey, customerDTO, null, DateTime.Now.AddSeconds(120), TimeSpan.Zero);
            }            

            UserProfileViewModel userProfileView = _mapper.Map<UserProfileViewModel>(customerDTO);

            var claimIdentity = HttpContext.User.Identity as ClaimsIdentity;
            string role = "";
            Role? clientRole = null;
            int? clientId = null;

            if (claimIdentity != null)
            {
                var roleClaim = claimIdentity.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
                if (roleClaim != null)
                {
                    role = roleClaim.Value.Trim();
                    clientRole = (Role)Enum.Parse(typeof(Role), role);
                }
            }

            var varClientId = Session["Id"];

            if (varClientId != null)
            {
                clientId = Convert.ToInt32(varClientId);
            }

            if (clientId != null && (clientRole != null && clientRole == Role.Executor))
            {
                DefineIfExecutorCanSeePersonalCustomerData(ref userProfileView, clientId);
            }

            return View(userProfileView);
        }


        ///<summary>
        ///Returns View with indents which don't have any executor and have category in range of future executor(executorId).
        ///</summary>
        [AllowAnonymous]
        public async Task<ActionResult> OfferIndentToExecutor(int executorId)
        {
            var claimIdentity = HttpContext.User.Identity as ClaimsIdentity;
            string role = "";
            Role? clientRole = null;
            int? clientId = null;

            if (claimIdentity != null)
            {
                var roleClaim = claimIdentity.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
                if (roleClaim != null)
                {
                    role = roleClaim.Value.Trim();
                    clientRole = (Role)Enum.Parse(typeof(Role), role);
                }
            }

            var varClientId = Session["Id"];

            if (varClientId != null)
            {
                clientId = Convert.ToInt32(varClientId);
            }

            if (clientId == null || clientRole == null || clientRole != Role.Customer)
            {
                TempData["OfferIndentToExecutorErrorMessage"] = "Вы должны войти в свой аккаунт заказчика, чтобы предложить заказ!";

                return RedirectToAction("ShowExecutor", "Executor", new { id = executorId });
            }

            IEnumerable<IndentDTO> freeIndents = await _userActivityService.
                GetFreeCustomerIndentsForExecutor(Convert.ToInt32(clientId), executorId);
            
            OfferIndentsToExecutorViewModel viewModel = new OfferIndentsToExecutorViewModel()
            {
                Indents = freeIndents,
                FromId = Convert.ToInt32(clientId),
                FromName = Session["Name"].ToString(),
                ToId = executorId
            };

            return View(viewModel);
        }

        ///<summary>
        ///Defines client opportunities: can client see customers mobilephone and email or not.
        ///Option available only for executors.
        ///</summary>
        [NonAction]
        private void DefineIfExecutorCanSeePersonalCustomerData(ref UserProfileViewModel customerProfile, int? executorId)
        {
            customerProfile.ShowPersonalData = false;

            if(customerProfile.Indents.Count() > 0)
            {
                foreach (IndentDTO indent in customerProfile.Indents)
                {
                    if(indent.Executor != null)
                    {
                        if(indent.Executor.ExecutorId == executorId)
                        {
                            customerProfile.ShowPersonalData = true;
                        }
                    }
                }
            }
        }

    }
}