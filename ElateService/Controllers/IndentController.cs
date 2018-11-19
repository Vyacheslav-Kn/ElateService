using AutoMapper;
using ElateService.Authorization;
using ElateService.BLL.Infrastructure;
using ElateService.BLL.Interfaces;
using ElateService.BLL.ModelsDTO;
using ElateService.BLL.PaginationDTO;
using ElateService.Common;
using ElateService.Localization;
using ElateService.Models;
using ElateService.Safe_execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ElateService.Controllers
{
    [Culture, Exception]
    public class IndentController : Controller
    {
        private IIndentService _indentService;
        private IExecutorService _executorService;
        private readonly IMapper _mapper;

        public IndentController(IIndentService indentService, IExecutorService executorService, IMapper mapper)
        {
            _indentService = indentService;
            _executorService = executorService;
            _mapper = mapper;
        }


        [RoleAuthorize(new object[] { Role.Customer })]
        [HttpPost]
        public async Task<ActionResult> SaveIndent(IndentViewModel indentViewModel, string indentCategory, string indentPrice)
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Customer");
            }

            int customerId = Convert.ToInt32(clientId);

            string imgSrc = null;
            string fileMimeType;

            if (indentViewModel.IndentDate.Year < DateTime.Now.Year ||
                (indentViewModel.IndentDate.Year == DateTime.Now.Year && indentViewModel.IndentDate.Month < DateTime.Now.Month) ||
                 (indentViewModel.IndentDate.Year == DateTime.Now.Year && indentViewModel.IndentDate.Month == DateTime.Now.Month &&
                    indentViewModel.IndentDate.Day < DateTime.Now.Day))
            {
                TempData["ErrorMessage"] = "Были введены некорректные данные, попробуйте снова!";

                return RedirectToAction("CreateIndent", "Customer");
            }   

            if (Request.Files.Count > 0)
            {
               HttpPostedFileBase file = Request.Files[0];

               fileMimeType = file.ContentType;

                if (!fileMimeType.Equals("application/octet-stream"))
                {
                    if (fileMimeType.Equals("image/jpeg"))
                    {
                        imgSrc = Path.Combine(Server.MapPath("~/Content/Indent"), "indentId.jpeg");
                        if (System.IO.File.Exists(imgSrc))
                        {
                            System.IO.File.Delete(imgSrc);
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Загружать фотографии разрешается только в jpeg формате!";

                        return RedirectToAction("CreateIndent", "Customer");
                    }
                }                             
            }

            double? priceOfIndent;

            Regex patternForPrice = new Regex(@"^[0-9]*\,?[0-9]+\s?$", RegexOptions.IgnoreCase);
            
                if (indentPrice == null)
                {
                    priceOfIndent = null;
                }
                else if (patternForPrice.IsMatch(indentPrice.Trim()))
                {
                    try
                    {
                        priceOfIndent = Convert.ToDouble(indentPrice.Trim());
                    }
                    catch(Exception e)
                    {
                        TempData["ErrorMessage"] = "Были введены некорректные данные, попробуйте снова!";

                        return RedirectToAction("CreateIndent", "Customer");
                    }                     
                }
                else
                {
                    TempData["ErrorMessage"] = "Были введены некорректные данные, попробуйте снова!";

                    return RedirectToAction("CreateIndent", "Customer");
                }            

            if (indentViewModel.Title == null || indentViewModel.City == null || indentViewModel.IndentDescription == null)
            {
                TempData["ErrorMessage"] = "Были введены некорректные данные, попробуйте снова!";

                return RedirectToAction("CreateIndent", "Customer");
            }
                        
            if (Session["Language"] != null)
            {
                string lan = Session["Language"].ToString();

                if (lan == "en")
                {
                    indentViewModel.Category = CategoryExtension.
                         TranslateFromEnglishToEnumEquivalents(new string[] { indentCategory }).FirstOrDefault();
                }
                else
                {

                    indentViewModel.Category = CategoryExtension.
                        TranslateFromRussianToEnumEquivalents(new string[] { indentCategory }).FirstOrDefault();
                }
            }
            
            indentViewModel.Price = priceOfIndent;
            indentViewModel.ImgSrc = imgSrc;
            indentViewModel.Customer = new CustomerDTO() { CustomerId = customerId };

            IndentDTO indentDTO = _mapper.Map<IndentDTO>(indentViewModel);
            int? indentId;
            try
            {
                indentId = await _indentService.Create(indentDTO);
            }
            catch (ValidationException e)
            {
                TempData["ErrorMessage"] = e.Message;

                return RedirectToAction("CreateIndent", "Customer");
            }

            if(imgSrc != null)
            {
            imgSrc = imgSrc.Replace("indentId", indentId.ToString());
            Request.Files[0].SaveAs(imgSrc);
            }            

            return RedirectToAction("GetCustomerIndents","Customer");
        }


        ///<summary>
        ///Returns view which contains indent model with: indent data, recalls, responces.
        ///Defines client opportunities to: write recall/responce, choose executor for indent.
        ///</summary>
        public ActionResult ShowIndent(int? id)
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            IndentDTO indentDTO = null;
            int indentId = id ?? default(int);

            string cacheKey = "show-indent-" + indentId.ToString();
            indentDTO = HttpContext.Cache[cacheKey] as IndentDTO;

            if (indentDTO == null)
            {
                try
                {
                    indentDTO = _indentService.Get(indentId);
                }
                catch (ValidationException e)
                {
                    return new HttpStatusCodeResult(404);
                }

                HttpContext.Cache.Insert(cacheKey, indentDTO, null, DateTime.Now.AddSeconds(120), TimeSpan.Zero);
            }            

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

            HelpModelForClientOpportunities clientOpportunities = new HelpModelForClientOpportunities();

            if (clientId != null && clientRole != null)
            {
                clientOpportunities = SetClientOpportunities(indentDTO, clientRole, clientId);
            }

            IndentViewModel indentViewModel = _mapper.Map<IndentViewModel>(indentDTO);          

            IndentFullViewModel indentFullViewModel = new IndentFullViewModel() {
                indentViewModel = indentViewModel,
                clientOpportunities = clientOpportunities
            };

            return View(indentFullViewModel);
        }


        public async Task<ActionResult> ShowIndentsPerPage(int? page, string[] categories)
        {
            int pageSize = 4;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            List<Category> categoriesInEnumEquivalent = null;
            if (categories != null)
            {
                List<string> categoriesStringCopy = new List<string>(categories);

                if (Session["Language"] != null)
                {
                    string lan = Session["Language"].ToString();

                    if (lan == "en")
                    {
                        categoriesInEnumEquivalent = CategoryExtension.
                            TranslateFromEnglishToEnumEquivalents(categoriesStringCopy.ToArray());
                    }
                    else
                    {
                        categoriesInEnumEquivalent = CategoryExtension.
                            TranslateFromRussianToEnumEquivalents(categoriesStringCopy.ToArray());
                    }
                }
            }

            IndentDTOPage indentsDTOSinglePageModel = await _indentService.GetIndentsPerPage(currentPageIndex,
                pageSize, categoriesInEnumEquivalent);

            IndentPageViewModel indentPageViewModel = new IndentPageViewModel();
            indentPageViewModel = _mapper.Map<IndentPageViewModel>(indentsDTOSinglePageModel);
            indentPageViewModel.AvailableCategories = categories;       

            return View(indentPageViewModel);
        }


        ///<summary>
        ///Returns view with indents which contain searchString parameter in their title.
        ///</summary>
        [HttpPost]
        public async Task<ActionResult> Search(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return RedirectToAction("ShowIndentsPerPage", "Indent");
            }

            IEnumerable<IndentDTO> indentsDTO = await _indentService.Search(searchString.Trim());

            IEnumerable<IndentViewModel> indentsView = _mapper.Map<IEnumerable<IndentDTO>, IEnumerable<IndentViewModel>>(indentsDTO);                      

            return PartialView(indentsView);
        }


        ///<summary>
        ///Defines client opportunities to: write recall/responce, choose executor for indent.
        ///</summary>
        [NonAction]
        private HelpModelForClientOpportunities SetClientOpportunities(IndentDTO indentDTO, Role? clientRole, int? clientId)
        {
            HelpModelForClientOpportunities clientPossibilities = new HelpModelForClientOpportunities();

            switch (clientRole)
            {
                case Role.Customer:
                    {
                        if(clientId == indentDTO.Customer.CustomerId)
                        {
                            if(indentDTO.Executor == null )
                            {
                                clientPossibilities.customerIsAllowToSelectExecutor = true;
                            }

                            if(indentDTO.Executor != null && 
                                (indentDTO.Recall == null || indentDTO.Recall.CustomerMarkForExecutor == null))
                            {
                                clientPossibilities.customerIsAllowToWriteRecall = true;
                            }
                        }
                        return clientPossibilities;
                    } 

                case Role.Executor:
                    {
                        if(indentDTO.Executor == null)
                        {
                            int executorId = clientId ?? default(int);
                            ExecutorDTO clientExecutorDTO = _executorService.GetExecutorPropertiesForEdition(executorId);

                            if(clientExecutorDTO.Categories.Count() > 0)
                            {
                                foreach(Category executorCategory in clientExecutorDTO.Categories)
                                {
                                    if (executorCategory.Equals(indentDTO.CategoryId))
                                    {
                                        clientPossibilities.executorIsAllowToSendResponce = true;

                                        foreach(var responce in indentDTO.Responces)
                                        {
                                            if(responce.Executor.ExecutorId == clientId)
                                            {
                                                clientPossibilities.executorIsAllowToSendResponce = false; 
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (indentDTO.Executor != null)
                        {
                            if (indentDTO.Executor.ExecutorId == clientId && 
                                (indentDTO.Recall == null || indentDTO.Recall.ExecutorMarkForCustomer == null))
                            {
                                clientPossibilities.executorIsAllowToWriteRecall = true;
                            }
                        }
                        return clientPossibilities;
                    }

                default: return clientPossibilities; 
            }
        }

    }
}