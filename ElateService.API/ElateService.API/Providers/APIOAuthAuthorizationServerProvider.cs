using ElateService.BLL.Infrastructure;
using ElateService.BLL.Interfaces;
using ElateService.BLL.Models;
using ElateService.Common;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ElateService.Api.Providers
{
    public class APIOAuthAuthorizationServerProvider: OAuthAuthorizationServerProvider
    {
        private ICustomerService _customerService;
        private IExecutorService _executorService;

        public APIOAuthAuthorizationServerProvider(ICustomerService customerService, IExecutorService executorService) : base()
        {
            _customerService = customerService;
            _executorService = executorService;
        }


        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
             context.Validated();

             await Task<bool>.FromResult(true);
        }


        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new string[] {"*"});

            var requestBodyData = await context.Request.ReadFormAsync();

            ClientDTO user = new ClientDTO()
            {
                Email = requestBodyData["email"], 
                Password = requestBodyData["password"]
            };

            try
            {
                string clientRole = requestBodyData["role"];

                if (clientRole == "customer")
                {
                    user = await _customerService.Login(user);
                }
                else if (clientRole == "executor")
                {
                    user = await _executorService.Login(user);
                }
                else
                {
                    throw new ValidationException("","");
                }                
            }
            catch (ValidationException e)
            {
                context.SetError("invalid_grant", "Invalid credentials!");
                return;
            }

            ClaimsIdentity identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.ClientId.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.RoleId.ToString().ToLower()));

            context.Validated(identity);
        }
    }
}