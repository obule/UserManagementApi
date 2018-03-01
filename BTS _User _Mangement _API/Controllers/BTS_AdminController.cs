using System.Net;
using System.Net.Http;
using System.Web.Http;
using BTS__User__Mangement__API.Models;
using System.Web.Routing;
using System.Web.Helpers;
using System.Web;
using System.IO;
using RazorEngine;
using System.Net.Http.Headers;
using System;

namespace BTS__User__Mangement__API.Controllers
{

    public class BTS_AdminController : ApiController
    {
       
        [HttpPost]
        
        public HttpResponseMessage AdminRegistration(Admin admin )
        {
            string applicationId = (KeyGenerator.GenerateUniqueKey(10)).TrimStart();
            string apiKey = (KeyGenerator.GenerateUniqueKey(12)).TrimStart();
            admin.ApplicationId = applicationId;
            admin.ApiKey = apiKey;
            string password = Crypto.HashPassword(admin.Pasword);
            admin.Pasword = password;
            if (admin.EmailCheck() == null)
            {
                Random random = new Random();
                string RandomNumber = random.Next(1, 10000000).ToString();
                string EmailCode = Crypto.HashPassword(admin.Email + RandomNumber);
                admin.EmailConfirmationCode = EmailCode;
                string encoded_email_code = HttpUtility.UrlEncode(EmailCode);
                EmailComposer.profileInfo info = new EmailComposer.profileInfo();
                EmailComposer.ArrayOfProfileInfo infos = new EmailComposer.ArrayOfProfileInfo();
                info.info = "Promise";
                infos.Add(info);
                EmailComposer.actionList action = new EmailComposer.actionList();
                EmailComposer.ArrayOfActionList actions = new EmailComposer.ArrayOfActionList();
                action.label = "Verify Email";
                action.Url = "http://184.107.228.154/everylogin/api/BTS_Admin/Adminverify?Verification_Code=" + encoded_email_code;
                actions.Add(action);
                EmailComposer.ArrayOfString details = new EmailComposer.ArrayOfString();
                details.Add("Make Sure You never leak this code");
                //Broken
                string status = EmailCommunication.SenEmail("a1school", "bts12345.", "", "EMAIL VERIFICATION", "Verify your Email", "Please You need to confirm your email to proceed with your application", false, "", infos, actions, "", details, admin.Email);


                int response = admin.RegisterAdmin();//Insert Into Users Table
                if (response != -14)
                {
                    int AdminId = Models.AdminAppDetails.GetAdminId(admin.Email); //Get Admin Id

                    int AdminAppDetails = Admin.InsertAdminAppDetail(applicationId, apiKey, admin.CompanyName, AdminId);//Insert App Details
                                                                                                                        // int Applicationresponse = Application.InsertApplication(admin.CompanyName, applicationId,AdminId); //Insert Into applications table //Modified...
                    int InsertAdminApplication = Admin_Application.InsertAdminApplication(AdminId, applicationId); //Insert into Admin_Application table
                    if (InsertAdminApplication != -14)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, admin);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error Updating Application");
                    }

                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cant create Admin now");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Email Already Exist");
            }

        }


        [HttpPost]
        [Route("api/BTS_Admin/Createnew")]
        public HttpResponseMessage CreateNewApplication(Admin admin)
        {
            string applicationId = (KeyGenerator.GenerateUniqueKey(10)).TrimStart();
            string apiKey = (KeyGenerator.GenerateUniqueKey(12)).TrimStart();
            int AdminAppDetails = Admin.InsertAdminAppDetail(applicationId, apiKey, admin.CompanyName, admin.Id);//Insert App Details
           // int Applicationresponse = Application.InsertApplication(admin.CompanyName, applicationId, admin.Id); //Insert Into applications table Modified  
            int InsertAdminApplication = Admin_Application.InsertAdminApplication(admin.Id, applicationId); //Insert into Admin_Application table
            var AppDetails = Admin.GetNewAppDetails(applicationId);
            if (AppDetails != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, AppDetails);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Problem Encountered");
            }

        }

        [HttpPost]
        [Route("api/BTS_Admin/Login")]
        public IHttpActionResult AdminLogin(Admin admin)
        {
            Admin response = admin.EmailCheck();
            var adminAppDetails = Users.VerifyAdminLoginDetailsWithEmail(admin.Email);
            bool doesPasswordMatch = Crypto.VerifyHashedPassword(response.Pasword, admin.Pasword);
            if (doesPasswordMatch == true && adminAppDetails.IsEmailConfirmed == true )
            {
                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("api/BTS_Admin/verify")]
        public HttpResponseMessage VerifyCode([FromUri]string Verification_Code)
        {
            //Check if comfirmationCode is valid and user have not confirmed
            var response = Users.VerifyConfirmationCode(Verification_Code, false);
            string ApplicationCode = Users.GetAppIdWithVerificationCode(Verification_Code); //Get the applicationCode using the verificationCode
            if (response != null)
            {
                int UpdateStatus = Users.UpdateUserStatus(Verification_Code, true);
                if (UpdateStatus != -14)
                {
                    
                    AdminAppDetails model = AdminAppDetails.GetApplication(ApplicationCode);
                    var path = @"~\Views\Login\Verify.cshtml";
                    var reply = new HttpResponseMessage(HttpStatusCode.OK);
                    string viewpath = HttpContext.Current.Server.MapPath(path);
                    var template = File.ReadAllText(viewpath);
                    string parsedView = Razor.Parse(template, model);
                    reply.Content = new StringContent(parsedView);
                    reply.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                    return reply;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error Updating");
                }
            }
            else
            {
                AdminAppDetails model = AdminAppDetails.GetApplication(ApplicationCode);
                var path = @"~\Views\Login\Verify_Fail.cshtml";
                var reply = new HttpResponseMessage(HttpStatusCode.OK);
                string viewpath = HttpContext.Current.Server.MapPath(path);
                var template = File.ReadAllText(viewpath);
                string parsedView = Razor.Parse(template, model);
                reply.Content = new StringContent(parsedView);
                reply.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return reply;
            }

        }


        [HttpGet]
        [Route("api/BTS_Admin/Adminverify")]
        public HttpResponseMessage AdminVerifyCode([FromUri]string Verification_Code)
        {
            //Check if comfirmationCode is valid and user have not confirmed
            var response = Users.VerifyConfirmationCode(Verification_Code, false);
            string ApplicationCode = Users.GetAppIdWithVerificationCode(Verification_Code); //Get the applicationCode using the verificationCode
            if (response != null)
            {
                int UpdateStatus = Users.UpdateUserStatus(Verification_Code, true);
                if (UpdateStatus != -14)
                {

                    AdminAppDetails model = AdminAppDetails.GetApplication(ApplicationCode);
                    var path = @"~\Views\Login\Adminverify.cshtml";
                    var reply = new HttpResponseMessage(HttpStatusCode.OK);
                    string viewpath = HttpContext.Current.Server.MapPath(path);
                    var template = File.ReadAllText(viewpath);
                    string parsedView = Razor.Parse(template, model);
                    reply.Content = new StringContent(parsedView);
                    reply.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                    return reply;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error Updating");
                }
            }
            else
            {
                AdminAppDetails model = AdminAppDetails.GetApplication(ApplicationCode);
                var path = @"~\Views\Login\AdminVerify_Fail.cshtml";
                var reply = new HttpResponseMessage(HttpStatusCode.OK);
                string viewpath = HttpContext.Current.Server.MapPath(path);
                var template = File.ReadAllText(viewpath);
                string parsedView = Razor.Parse(template, model);
                reply.Content = new StringContent(parsedView);
                reply.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return reply;
            }

        }


        [HttpGet]
        [Route("api/BTS_Admin/GetAllCompanyName")]
        public HttpResponseMessage GetAllCompanyName(string ApplicationId)
        {
            Admin resultForAdminId = Admin.GetAdminIdForExec(ApplicationId);
            var response = Admin.GetAllCompanyName(resultForAdminId.UserId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not Found");
            }
        }

        [HttpGet]
        [Route("api/BTS_Admin/ParticularApp")]
        public HttpResponseMessage GetParticularAppDetails(string Code)
        {
            Admin resultForAdminId = Admin.GetAdminIdForExec(Code);
            
            if (resultForAdminId != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, resultForAdminId);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not Found");
            }
        }
        
        [HttpPost]
        [Route("api/BTS_Admin/VerifyUserLogin")]
        public HttpResponseMessage VerifyUserLogin(string Email, string Password)
        {
            var response = Users.VerifyLoginDetailsWithEmail(Email);
            if (response != null)
            {
                bool checkPassword = Crypto.VerifyHashedPassword(response.Pasword, Password);
                if (checkPassword == true)
                {
                    AdminAppDetails ApplicationCriteria = AdminAppDetails.GetApplication(response.ApplicationCode);
                    return Request.CreateResponse(HttpStatusCode.OK, ApplicationCriteria);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Login Details Mismatched!");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Email Not Found");
            }

        }

       

    }
}
