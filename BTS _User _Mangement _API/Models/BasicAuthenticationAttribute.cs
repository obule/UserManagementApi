using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BTS__User__Mangement__API.Models
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null) //Check if there is authorization header
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                string authenticationToken = actionContext.Request.Headers.Authorization.Parameter; //Retrive the Authorization header parameter in base64.
                string decodedauthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken)); //Convert(Decode) Authorization Header From Base 64 string
                string[] AdminCredentialArray = decodedauthenticationToken.Split(':');
                string AppId = AdminCredentialArray[0];
                string ApiKey = AdminCredentialArray[1];

                if (API_Security.CheckAdminCredentials(AppId,ApiKey) != null)
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(AppId), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }

        }
    }
}