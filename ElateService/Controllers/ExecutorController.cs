using AutoMapper;
using ElateService.Authorization;
using ElateService.BLL.Infrastructure;
using ElateService.BLL.Interfaces;
using ElateService.BLL.Models;
using ElateService.BLL.ModelsDTO;
using ElateService.BLL.PaginationDTO;
using ElateService.Common;
using ElateService.Localization;
using ElateService.Models;
using ElateService.Safe_execution;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ElateService.Controllers
{
    [Culture, RoleAuthorize(new object[] { Role.Executor }), Exception]
    public class ExecutorController : Controller
    {
        private IExecutorService _executorService;
        private IUserActivityService _userActivityService;
        private readonly IMapper _mapper;
        private IAuthenticationManager _authenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ExecutorController(IExecutorService executorService, IUserActivityService userActivityService, IMapper mapper)
        {
            _executorService = executorService;
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
                await _executorService.Register(clientRegistrationDTO, language);
            }
            catch (ValidationException e)
            {
                ViewBag.ValidationErrorMessage = e.Message;
                ModelState.AddModelError(" ", " ");

                return PartialView("RegistrationPartial", registrationViewModel);
            }

            return PartialView("ConfirmationCodePartial");
        }


        [AllowAnonymous]
        public async Task<ActionResult> VerifyConfirmationCode(int id, string confirmationCode)
        {
            string executorName = null;

            try
            {
                executorName = await _executorService.ConfirmRegistration(id, confirmationCode);
            }
            catch (ValidationException e)
            {
                // there is no executor with such id, confirmation code and email confirmed
                return new HttpStatusCodeResult(422);
            }

            Session["Name"] = executorName;
            Session["Id"] = id;

            ClaimsIdentity claim = new ClaimsIdentity("ApplicationCookie");
            claim.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                "OWIN Provider", ClaimValueTypes.String));
            claim.AddClaim(new Claim(ClaimTypes.Role, Role.Executor.ToString(), ClaimValueTypes.String));

            _authenticationManager.SignOut();
            _authenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, claim);

            return RedirectToAction("Index", "Home");
        }


        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            ClientDTO clientLoginDTO = _mapper.Map<ClientDTO>(loginViewModel);

            ClientDTO minLoginInfo = new ClientDTO();

            try
            {
                minLoginInfo = await _executorService.Login(clientLoginDTO);
            }
            catch (ValidationException e)
            {
                TempData["ValidationErrorMessageLogin"] = e.Message;

                return RedirectToAction("Registration", "Executor");
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

            return RedirectToAction("PrivateOffice", "Executor"); 
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
                await _executorService.GenerateNewConfirmationCode(email, language);
            }
            catch (ValidationException e)
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
                await _executorService.VerifyNewConfirmationCode(id, confirmationCode);
            }
            catch (ValidationException e)
            {
                // there is no executor with such id, confirmation code and email confirmed
                return new HttpStatusCodeResult(422);
            }

            Session["ClientId"] = id;

            return View("EnterNewPassword");
        }


        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetNewPassword(string password, string passwordCopy)
        {
            var clientId = Session["ClientId"];

            if (clientId == null || !password.Equals(passwordCopy))
            {
                ViewBag.ValidationErrorForPasswordRefresh = "Были введены  некорректные данные, попробуйте снова!";

                return View("EnterNewPassword");
            }

            string clientName = null;

            try
            {
                clientName = await _executorService.SetNewPassword(Convert.ToInt32(clientId), password);
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
            claim.AddClaim(new Claim(ClaimTypes.Role, Role.Executor.ToString(), ClaimValueTypes.String));

            _authenticationManager.SignOut();
            _authenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, claim);

            return RedirectToAction("Index", "Home");
        }


        ///<summary>
        ///Returns view of executor profile which contains: recalls, indents with responces, personal data.
        ///Uses _PrivateOfficeExecutorLayout
        ///</summary>
        public ActionResult PrivateOffice()
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Executor");
            }

            int executorId = Convert.ToInt32(clientId);
            ExecutorDTO executorDTO = null;

            string cacheKey = "show-executor-" + executorId.ToString();
            executorDTO = HttpContext.Cache[cacheKey] as ExecutorDTO;

            if (executorDTO == null)
            {
                try
                {
                    executorDTO = _executorService.GetExecutorProfile(executorId);
                }
                catch (ValidationException e)
                {
                    return RedirectToAction("Registration", "Executor");
                }

                HttpContext.Cache.Insert(cacheKey, executorDTO, null, DateTime.Now.AddSeconds(120), TimeSpan.Zero);
            }            

            UserProfileViewModel userProfileView = _mapper.Map<UserProfileViewModel>(executorDTO);

            return View(userProfileView);
        }


        ///<summary>
        ///Returns red circle pointer near menu item, which shows if executor has notifications.
        ///</summary>
        public ActionResult CheckForNotifications()
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Executor");
            }

            int executorId = Convert.ToInt32(clientId);
            NotificationsViewModel notificationsViewModel = new NotificationsViewModel();

            NotificationDTO notificationDTO = new NotificationDTO();
            notificationDTO = _userActivityService.CheckForNewNotifications(Role.Executor, executorId);

            if (notificationDTO != null)
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
                return RedirectToAction("Registration", "Executor");
            }

            int executorId = Convert.ToInt32(clientId);

            IEnumerable<NotificationDTO> notificationsDTO = await _userActivityService.GetNotifications(Role.Executor, executorId);

            NotificationsViewModel notificationsViewModel = new NotificationsViewModel();
            
            notificationsViewModel.Notifications = notificationsDTO;                 

            return View(notificationsViewModel);
        }


        public async Task<ActionResult> GetIndentsWithResponce()
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Executor");
            }

            int executorId = Convert.ToInt32(clientId);
            IEnumerable<IndentDTO> indentsDTO = await _userActivityService.GetIndentsWithExecutorResponce(executorId);

            IEnumerable<IndentViewModel> indentsView = _mapper.Map<IEnumerable<IndentDTO>, IEnumerable<IndentViewModel>>(indentsDTO);           

            return View(indentsView);
        }


        ///<summary>
        ///Allows executor to modificate profile: change 2 categories (once), download photo, change description.
        ///</summary>
        public ActionResult ModificateProfile()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];              
            }

            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Executor");
            }

            int executorId = Convert.ToInt32(clientId);

            ExecutorDTO executorDTO = _executorService.GetExecutorPropertiesForEdition(executorId);       

   ExecutorPropertiesForEditionViewModel propertiesViewModel = _mapper.Map<ExecutorPropertiesForEditionViewModel>(executorDTO);            
            
            return View(propertiesViewModel);
        }


        [HttpPost]
        public async Task<ActionResult> ModificateProfile(string information, string categoryFirst, string categorySecond)
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Executor");
            }

            int executorId = Convert.ToInt32(clientId);

            HashSet<Category> categories = null;

            if (categoryFirst != null && categorySecond != null)
            {
                if (categoryFirst.Equals(categorySecond))
                {
                    TempData["ErrorMessage"] = "Категории должны различаться!";

                    return RedirectToAction("ModificateProfile", "Executor");
                }
                else
                {
                    if (Session["Language"] != null)
                    {
                        string lan = Session["Language"].ToString();
                        IEnumerable<Category> categoriesInEnumEquivalent = new List<Category>();

                        if (lan == "en")
                        {
                            categoriesInEnumEquivalent = CategoryExtension.
                                TranslateFromEnglishToEnumEquivalents(new string[] { categoryFirst, categorySecond });                            
                        }
                        else
                        {
                            categoriesInEnumEquivalent = CategoryExtension.
                                TranslateFromRussianToEnumEquivalents(new string[] { categoryFirst, categorySecond });
                        }

                        categories = new HashSet<Category>(categoriesInEnumEquivalent);
                    }
                }                             
            }

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
                        imgSrc = Path.Combine(Server.MapPath("~/Content/Executor"), executorId.ToString() + ".jpeg");
                        if (System.IO.File.Exists(imgSrc))
                        {
                            System.IO.File.Delete(imgSrc);
                        }
                        file.SaveAs(imgSrc);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Загружать фотографии разрешается только в jpeg формате!";

                        return RedirectToAction("ModificateProfile", "Executor");
                    }
                }                    
            }

            ExecutorDTO propertiesAfterEdition = new ExecutorDTO()
            {
                ExecutorId = executorId,
                ImgSrc = imgSrc,
                Information = information,
                Categories = categories
            };

            await _executorService.SaveExecutorPropertiesAfterEdition(propertiesAfterEdition);

            string cacheKeyForExecutor = "show-executor-" + executorId.ToString();
            ExecutorDTO cachedExecutor = HttpContext.Cache[cacheKeyForExecutor] as ExecutorDTO;
            if (cachedExecutor != null)
            {
                HttpContext.Cache.Remove(cacheKeyForExecutor);
            }

            return RedirectToAction("PrivateOffice", "Executor");
        }


        [AllowAnonymous]
        public async Task<ActionResult> ShowExecutorsPerPage(int? page, string[] categories)
        {
            int pageSize = 4;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            
            List<Category> categoriesInEnumEquivalent = null;
            if (categories != null)
            {
                List<string> categoriesCopy = new List<string>(categories);

                if (Session["Language"] != null)
                {
                    string lan = Session["Language"].ToString();

                    if (lan == "en")
                    {
                        categoriesInEnumEquivalent = CategoryExtension.
                            TranslateFromEnglishToEnumEquivalents(categoriesCopy.ToArray());                       
                    }
                    else
                    {
                        categoriesInEnumEquivalent = CategoryExtension.
                            TranslateFromRussianToEnumEquivalents(categoriesCopy.ToArray());
                    }
                }
            }            

            ExecutorDTOPage executorsDTOSinglePageModel = await _executorService.GetExecutorsPerPage(currentPageIndex, pageSize,
                categoriesInEnumEquivalent);

            ExecutorPageViewModel executorPageViewModel = new ExecutorPageViewModel();
            executorPageViewModel = _mapper.Map<ExecutorPageViewModel>(executorsDTOSinglePageModel);
            executorPageViewModel.AvailableCategories = categories;

            return View(executorPageViewModel);
        }


        ///<summary>
        ///Returns executors which name, surname or patronymic contains searchString.
        ///</summary>
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> Search(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return RedirectToAction("ShowExecutorsPerPage", "Executor");
            }

            IEnumerable<ExecutorDTO> executorsDTO = await _executorService.Search(searchString.Trim());

            IEnumerable<ExecutorViewModel> executorsView = _mapper.Map<IEnumerable<ExecutorDTO>, IEnumerable<ExecutorViewModel>>(executorsDTO);            

            return PartialView(executorsView);
        }


        ///<summary>
        ///Returns view of executor profile which contains: recalls, indents with responces, personal data.
        ///Uses _Layout
        ///</summary>
        [AllowAnonymous]
        public ActionResult ShowExecutor(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ShowExecutorsPerPage", "Executor");
            }

            int executorId = Convert.ToInt32(id);
            ExecutorDTO executorDTO = null;

            string cacheKey = "show-executor-" + executorId.ToString();
            executorDTO = HttpContext.Cache[cacheKey] as ExecutorDTO;

            if (executorDTO == null)
            {
                try
                {
                    executorDTO = _executorService.GetExecutorProfile(executorId);
                }
                catch (ValidationException e)
                {
                    return new HttpStatusCodeResult(404);
                }

                HttpContext.Cache.Insert(cacheKey, executorDTO, null, DateTime.Now.AddSeconds(120), TimeSpan.Zero);
            }            

            if (TempData["OfferIndentToExecutorErrorMessage"] != null)
            {
                ViewBag.OfferIndentToExecutorErrorMessage = TempData["OfferIndentToExecutorErrorMessage"];
            }

            UserProfileViewModel userProfileView = _mapper.Map<UserProfileViewModel>(executorDTO);

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

            if (clientId != null && (clientRole != null && clientRole == Role.Customer))
            {
                DefineIfCustomerCanSeePersonalExecutorData(ref userProfileView, clientId);
            }

            return View(userProfileView);
        }


        ///<summary>
        ///Defines client opportunities: can client see executors mobilephone and email or not.
        ///Option available only for customers.
        ///</summary>
        [NonAction]
        private void DefineIfCustomerCanSeePersonalExecutorData(ref UserProfileViewModel executorProfile, int? customerId)
        {
            executorProfile.ShowPersonalData = false;

            if (executorProfile.Indents.Count() > 0)
            {
                foreach (IndentDTO indent in executorProfile.Indents)
                {
                    if (indent.Customer.CustomerId == customerId)
                    {
                        executorProfile.ShowPersonalData = true;
                    }
                }
            }
        }

    }
}