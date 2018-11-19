using AutoMapper;
using ElateService.Authorization;
using ElateService.BLL.Interfaces;
using ElateService.BLL.ModelsDTO;
using ElateService.Common;
using ElateService.Localization;
using ElateService.Models;
using ElateService.Safe_execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ElateService.Controllers
{
    [Culture, Exception]
    public class UserActivityController : Controller
    {
        private IUserActivityService _userActivityService;
        private readonly IMapper _mapper;

        public UserActivityController(IUserActivityService userActivityService, IMapper mapper)
        {
            _userActivityService = userActivityService;
            _mapper = mapper;
        }


        [RoleAuthorize(new object[] { Role.Customer })]
        [HttpPost]
        public async Task<ActionResult> SaveCustomerRecallForExecutor(UserActivityViewModel activityModel)
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Customer");
            }

            if (activityModel.Mark == null)
            {
                TempData["ErrorMessage"] = "Вы не оценили работу исполнителя!";

                return RedirectToAction("ShowIndent", "Indent", new { id = activityModel.IndentId });
            }

            int customerId = Convert.ToInt32(clientId);
            string customerName = Session["Name"].ToString();

            RecallDTO recallDTO = new RecallDTO()
            {
                RecallId = activityModel.IndentId,
                CustomerCommentForExecutor = activityModel.Comment,
                CustomerMarkForExecutor = activityModel.Mark
            };

            await _userActivityService.SaveCustomerRecallForExecutor(recallDTO);

            string cacheKeyForIndent = "show-indent-" + activityModel.IndentId.ToString();
            IndentDTO cachedIndent = HttpContext.Cache[cacheKeyForIndent] as IndentDTO;
            if (cachedIndent != null)
            {
                HttpContext.Cache.Remove(cacheKeyForIndent);
            }
            string cacheKeyForExecutor = "show-executor-" + activityModel.UserOpponentId.ToString();
            ExecutorDTO cachedExecutor = HttpContext.Cache[cacheKeyForExecutor] as ExecutorDTO;
            if (cachedExecutor != null)
            {
                HttpContext.Cache.Remove(cacheKeyForExecutor);
            }

            NotificationDTO notificationDTO = GenerateNotification(activityModel, Role.Executor, "Recall");
            notificationDTO.FromId = customerId;
            notificationDTO.FromName = customerName;

            await _userActivityService.SaveNotification(notificationDTO);            

            return RedirectToAction("ShowIndent","Indent", new { id = activityModel.IndentId });
        }


        [RoleAuthorize(new object[] { Role.Executor })]
        [HttpPost]
        public async Task<ActionResult> SaveExecutorRecallForCustomer(UserActivityViewModel activityModel)
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Executor");
            }

            if (activityModel.Mark == null)
            {
                TempData["ErrorMessage"] = "Вы не оценили действия заказчика!";

                return RedirectToAction("ShowIndent", "Indent", new { id = activityModel.IndentId });
            }

            int executorId = Convert.ToInt32(clientId);
            string executorName = Session["Name"].ToString();

            RecallDTO recallDTO = new RecallDTO()
            {
                RecallId = activityModel.IndentId,
                ExecutorCommentForCustomer = activityModel.Comment,
                ExecutorMarkForCustomer = activityModel.Mark
            };

            await _userActivityService.SaveExecutorRecallForCustomer(recallDTO);

            string cacheKeyForIndent = "show-indent-" + activityModel.IndentId.ToString();
            IndentDTO cachedIndent = HttpContext.Cache[cacheKeyForIndent] as IndentDTO;
            if (cachedIndent != null)
            {
                HttpContext.Cache.Remove(cacheKeyForIndent);
            }
            string cacheKeyForCustomer = "show-customer-" + activityModel.UserOpponentId.ToString();
            CustomerDTO cachedCustomer = HttpContext.Cache[cacheKeyForCustomer] as CustomerDTO;
            if (cachedCustomer != null)
            {
                HttpContext.Cache.Remove(cacheKeyForCustomer);
            }

            NotificationDTO notificationDTO = GenerateNotification(activityModel, Role.Customer, "Recall");
            notificationDTO.FromId = executorId;
            notificationDTO.FromName = executorName;

            await _userActivityService.SaveNotification(notificationDTO);

            return RedirectToAction("ShowIndent", "Indent", new { id = activityModel.IndentId });
        }


        [RoleAuthorize(new object[] { Role.Executor })]
        [HttpPost]
        public async Task<ActionResult> SaveExecutorResponce(UserActivityViewModel activityModel, string indentPrice)
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Executor");
            }

            int executorId = Convert.ToInt32(clientId);
            string executorName = Session["Name"].ToString();

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
                catch (Exception e)
                {
                    TempData["ErrorMessage"] = "Были введены некорректные данные, попробуйте снова!";

                    return RedirectToAction("ShowIndent", "Indent", new { id = activityModel.IndentId });
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Были введены некорректные данные, попробуйте снова!";

                return RedirectToAction("ShowIndent", "Indent", new { id = activityModel.IndentId });
            }

            ResponceDTO responceDTO = new ResponceDTO()
            {
                Price = priceOfIndent,
                IndentId = activityModel.IndentId,
                Executor = new ExecutorDTO() {
                    ExecutorId = executorId
                },
                ResponceText = activityModel.Comment
            };

            await _userActivityService.SaveResponce(responceDTO);

            string cacheKey = "show-indent-" + activityModel.IndentId.ToString();
            IndentDTO cachedIndent = HttpContext.Cache[cacheKey] as IndentDTO;
            if (cachedIndent != null)
            {
                HttpContext.Cache.Remove(cacheKey);
            }

            NotificationDTO notificationDTO = GenerateNotification(activityModel, Role.Customer, "Responce");
            notificationDTO.FromId = executorId;
            notificationDTO.FromName = executorName;

            await _userActivityService.SaveNotification(notificationDTO);

            return RedirectToAction("ShowIndent", "Indent", new { id = activityModel.IndentId });
        }

        
        [RoleAuthorize(new object[] { Role.Customer })]
        [HttpPost]
        public async Task<ActionResult> CustomerSetExecutorForIndent(UserActivityViewModel activityModel)
        {
            var clientId = Session["Id"];

            if (clientId == null)
            {
                return RedirectToAction("Registration", "Customer");
            }

            int customerId = Convert.ToInt32(clientId);
            string customerName = Session["Name"].ToString();

            IndentDTO indentDTO = new IndentDTO()
            {
                IndentId = activityModel.IndentId
            };
            await _userActivityService.SaveExecutorForIndent(indentDTO, activityModel.UserOpponentId);

            string cacheKey = "show-indent-" + activityModel.IndentId.ToString();
            IndentDTO cachedIndent = HttpContext.Cache[cacheKey] as IndentDTO;
            if (cachedIndent != null)
            {
                HttpContext.Cache.Remove(cacheKey);
            }

            NotificationDTO notificationDTO = GenerateNotification(activityModel, Role.Executor, "Сandidature confirmed");
            notificationDTO.FromId = customerId;
            notificationDTO.FromName = customerName;

            await _userActivityService.SaveNotification(notificationDTO);

            return RedirectToAction("ShowIndent", "Indent", new { id = activityModel.IndentId });
        }


        [HttpPost]
        public async Task<ActionResult> OfferIndentToExecutor(int fromId, string fromName, UserActivityViewModel activityModel)
        {
            NotificationDTO notificationDTO = GenerateNotification(activityModel, Role.Executor, "Invite to complete order");
            notificationDTO.FromId = fromId;
            notificationDTO.FromName = fromName;

            await _userActivityService.SaveNotification(notificationDTO);

            return RedirectToAction("ShowExecutor", "Executor", new { id = activityModel.UserOpponentId });
        }


        ///<summary>
        ///Generate notification according to: action, notification receiver.
        ///</summary>
        [NonAction]
        private NotificationDTO GenerateNotification(UserActivityViewModel userActivityModel, Role roleOfPersonWhoWillGetNotification,
            string notificationType)
        {
            NotificationDTO notificationDTO = _mapper.Map<NotificationDTO>(userActivityModel);
            notificationDTO.RoleId = roleOfPersonWhoWillGetNotification;
            notificationDTO.WasRead = false;

            switch (notificationType)
            {
                case "Recall": notificationDTO.Context = "оставил(а) вам отзыв после работы над заказом"; break;

                case "Responce": notificationDTO.Context = "предлагает свою кандидатуру для выполнения заказа"; break;

                case "Сandidature confirmed": notificationDTO.Context = "выбрал(а) вас для выполнения заказа"; break;

                case "Invite to complete order": notificationDTO.Context = "предлагает вам выполнить заказ"; break;

                default: notificationDTO.Context = "оставил(а) вам отзыв после работы над заказом"; break;
            }          

            return notificationDTO;
        }

    }
}