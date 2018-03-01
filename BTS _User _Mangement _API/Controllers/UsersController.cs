using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BTS__User__Mangement__API.Models;
using System.Threading;
using System.Web.Helpers;
using System.Web;
using System.Net.Http.Headers;
using RazorEngine;
using System.IO;

namespace BTS__User__Mangement__API.Controllers
{

    [BasicAuthentication]
    public class UsersController : ApiController
    {
        
        public static string AppId = Thread.CurrentPrincipal.Identity.Name;

        [HttpGet]
        public HttpResponseMessage GetUsers()   //Get Users
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            Users user = new Users();
            var response = user.RetriveUsersDetails(AppId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Does Not Exist");
            }
        }


        [HttpGet]
        [Route("api/users/CountUsers")]
        public HttpResponseMessage CountUsers()   //Count Users
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = Users.CountUsers(AppId);
            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Count Fail");
            }
        }

        [HttpGet]
        [Route("api/users/CountUsersOnline")]
        public HttpResponseMessage CountUsersOnline()   //Count Users Online
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = Users.CountUsersOnline(AppId);
            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Count Fail");
            }
        }

        [HttpGet]
        [Route("api/users/CountRole")]
        public HttpResponseMessage CountRole()   //Count Roles
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = Users.CountRole(AppId);
            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Count Fail");
            }
        }

        [HttpGet]
        public HttpResponseMessage GetUsersByEmail(int id)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = Users.GetUserById(id, AppId);

            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cant retrive User");
            }

        }

        [HttpPut]
        [Route("api/users/Updatepassword")]
        public HttpResponseMessage UpdatePassword(string Email)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            Users response = Users.VerifyLoginDetails(Email, AppId);  //Verify user credential
            
            if (response !=null)
            {
                //Application ApplicationCriteria = Application.GetApplication(AppId);
                
                AdminAppDetails ApplicationCriteria = AdminAppDetails.GetApplication(AppId);

                string RandomNumber = KeyGenerator.GenerateUniqueKey(10);

                    string hashPassword = Crypto.HashPassword(RandomNumber);
                    Users.UpdatePassword(response.Id, AppId, hashPassword);
                    EmailComposer.profileInfo info = new EmailComposer.profileInfo();
                    EmailComposer.ArrayOfProfileInfo infos = new EmailComposer.ArrayOfProfileInfo();
                    info.info = "Promise";
                    infos.Add(info);
                    EmailComposer.actionList action = new EmailComposer.actionList();
                    EmailComposer.ArrayOfActionList actions = new EmailComposer.ArrayOfActionList();
                    action.label = "Login";
                    action.Url = ApplicationCriteria.CallBackUrl; 
                    actions.Add(action);
                    EmailComposer.ArrayOfString details = new EmailComposer.ArrayOfString();
                    details.Add("Make Sure You change this password immediately");
                    string status = EmailCommunication.SenEmail("a1school", "bts12345.", ApplicationCriteria.LogoUrl, "PASSWORD RECOVERY", "Password Recovery", "Please Ensure to change this password immediately. Your new Password is " + RandomNumber, false, "", infos, actions, "", details, Email);
                    if (status == "0")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadGateway, "Cant Send Message Now");
                    }
                


            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error Getting User");
            }


        }

        [HttpPost]
        public HttpResponseMessage RegisterUsers([FromBody]Users user) //Register Users
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            string doesEmailExist = Users.CheckIfEmailAlreadyExist(user.Email,AppId);
            if (doesEmailExist == null)
            {
                
                AdminAppDetails ApplicationCriteria = AdminAppDetails.GetApplication(AppId);
                if (ApplicationCriteria != null)
                {
                    string NeeedEmailConfirmation = ApplicationCriteria.NeedEmailConfirmation;
                    if (NeeedEmailConfirmation == "No")
                    {
                        user.Pasword = Crypto.HashPassword(user.Pasword);
                        user.ApplicationCode = AppId;
                        Users response = user.InsertUsers();//SP Register Users

                        EmailComposer.profileInfo info = new EmailComposer.profileInfo();
                        EmailComposer.ArrayOfProfileInfo infos = new EmailComposer.ArrayOfProfileInfo();
                        info.info = "";
                        infos.Add(info);
                        EmailComposer.actionList action = new EmailComposer.actionList();
                        EmailComposer.ArrayOfActionList actions = new EmailComposer.ArrayOfActionList();
                        action.label = "Login";
                        action.Url = ApplicationCriteria.CallBackUrl;
                        actions.Add(action);
                        EmailComposer.ArrayOfString details = new EmailComposer.ArrayOfString();
                        //details.Add("From "+ApplicationCriteria.CompanyName);
                        string body = "Hello <strong>"+user.Surname+" "+user.Othernames+"</strong>, an account was created for you on "+ApplicationCriteria.CompanyName+". Click the button below to login and access your account.";
                        string status = EmailCommunication.SenEmail("a1pay", "876#1@OO2", ApplicationCriteria.LogoUrl, ApplicationCriteria.CompanyName, "Your new account ", body, false, "", infos, actions, "", details, user.Email);
                        return Request.CreateResponse(HttpStatusCode.OK,response);
                      
                    }
                    else
                    {
                        Random random = new Random();
                        string RandomNumber = random.Next(1, 10000000).ToString();
                        string EmailCode = Crypto.HashPassword(user.Email + RandomNumber);
                        user.EmailConfirmationCode = EmailCode;
                        string encoded_email_code = HttpUtility.UrlEncode(EmailCode);
                        user.Pasword = Crypto.HashPassword(user.Pasword);
                        user.ApplicationCode = AppId;
                        Users responseWithConfirmation = user.InsertUsers();//SP Register Users
                        EmailComposer.profileInfo info = new EmailComposer.profileInfo();
                        EmailComposer.ArrayOfProfileInfo infos = new EmailComposer.ArrayOfProfileInfo();
                        info.info = "Promise";
                        infos.Add(info);
                        EmailComposer.actionList action = new EmailComposer.actionList();
                        EmailComposer.ArrayOfActionList actions = new EmailComposer.ArrayOfActionList();
                        action.label = "Verify Email";
                        action.Url = "http://184.107.228.154/everylogin/api/BTS_Admin/verify?Verification_Code=" + encoded_email_code;
                        actions.Add(action);
                        EmailComposer.ArrayOfString details = new EmailComposer.ArrayOfString();
                        //details.Add("Make Sure You never leak this code");
                        //Broken
                        //string status = EmailCommunication.SenEmail("a1school", "bts12345.", ApplicationCriteria.LogoUrl, "EMAIL VERIFICATION", "Verify your Email", ApplicationCriteria.Email_Message, false, "", infos, actions, "", details, user.Email);
                        string body = "Hello <strong>" + user.Surname + " " + user.Othernames + "</strong>, an account was created for you on " + ApplicationCriteria.CompanyName + ". Click the button below to verify your account.";
                        string status = EmailCommunication.SenEmail("a1pay", "876#1@OO2", ApplicationCriteria.LogoUrl, ApplicationCriteria.CompanyName + " EMAIL VERIFICATION", "Verify your Email", body, false, "", infos, actions, "", details, user.Email);

                        if (responseWithConfirmation != null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK,responseWithConfirmation);
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please we cant register the student at the moment");
                        }
                    }

                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Check Application Code");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Email Already Exist");
            }


        }
        
        [HttpDelete]
        public HttpResponseMessage DeleteUser(int id)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            Users CheckIfEmailExist = Users.GetUserById(id, AppId); //Check if user exist

            if ( CheckIfEmailExist != null)
            {
                int response = Users.DeleteUser(id); //Delete User.
                if (response != -14)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "User Delete");

                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not delete User");
                }
                
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User was not found");
            }
        }

        [HttpDelete]
        [Route("api/users/DeleteUrlById")]
        public HttpResponseMessage DeleteUrlById(int Id)
        {
            Url url = new Url();
            int response = url.DeleteUrlById(Id);
            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Url Deleted");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not Delete Url");
            }
          
        }
  
        [HttpPut]
        public HttpResponseMessage UpdateUser(int id, Users user)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            Users CheckIfEmailExist = Users.GetUserById(id, AppId); //Check if user exist
            
            if ( CheckIfEmailExist != null)
            {
                int response =user.UpdateUser(id); //Update User
                if (response != -14)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "User Updated");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not Update User");
                }

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User was not found");
            }

        }

        [HttpPut]
        [Route("api/users/UpdateUrl")]
        public HttpResponseMessage UpdateUrl(int id, Url url)
        {
                int response = url.UpdateUrl(id); //Update Url
                if (response != -14)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "User Updated");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not Update User");
                }

            
         

        }

        [HttpPost]
        [Route("api/users/CheckUserLogin")]
        public HttpResponseMessage CheckIfUserIsLoggedIn(string Email,string Password)
        {
            try
            {
                AppId = Thread.CurrentPrincipal.Identity.Name;
                AdminAppDetails ApplicationCriteria = AdminAppDetails.GetApplication(AppId);
                Users response = Users.VerifyLoginDetails(Email, AppId);  //Verify user credential
                string url = HttpContext.Current.Request.Url.AbsoluteUri;
                string UpdatedNewToken = "";
                HttpRequestMessage currentRequest = this.Request;
                HttpHeaderValueCollection<ProductInfoHeaderValue> userAgentHeader = currentRequest.Headers.UserAgent;
                string Ip = NewToken.GetIPAddress();
                if (response != null)
                {
                    if (ApplicationCriteria.NeedEmailConfirmation == "Yes")
                    {
                        bool checkPassword = Crypto.VerifyHashedPassword(response.Pasword, Password);
                        if (checkPassword == true && response.IsEmailConfirmed == true)
                        {
                            //Check if application criteria demands multiple logins
                            if (ApplicationCriteria.AllowMultipleLogin == "No")
                            {
                                //Check whether the user already have a valid token.
                                UserToken userTokenDetails = UserToken.GetTokenById(response.Id, "Yes", Ip, userAgentHeader.ToString());
                                if (userTokenDetails != null)
                                {
                                    
                                    bool IsTokenActive = IsTokenValidForLogin(userTokenDetails.Token, userTokenDetails.IpAddress, userTokenDetails.UserAgent,out UpdatedNewToken);
                                    if (IsTokenActive == true)
                                    {
                                        //Update status
                                        int status = Users.UpdateStatus("Online", AppId, Email);
                                        UserToken newDetails = UserToken.GetTokenDetailsByToken(UpdatedNewToken);
                                        //Return the token,Ip and UserAgent.
                                        return Request.CreateResponse(HttpStatusCode.OK, newDetails);

                                    }
                                    else
                                    {
                                        //Unset Any Old Token From That UserAgent
                                        UserToken.UnSetTokenById(response.Id, "Yes", Ip, userAgentHeader.ToString());
                                        //Token is not active then generate fresh token.
                                        long time = NewToken.ConvertDateTimeToTicks(DateTime.Now.AddMinutes((double)ApplicationCriteria.LengthOfSession));
                                        string token = NewToken.GenerateToken(Email, response.Pasword, Ip, userAgentHeader.ToString(), time);
                                        int result = UserToken.InsertToken(response.Id, token, ApplicationCriteria.LengthOfSession, Ip, userAgentHeader.ToString(), url);
                                        int status = Users.UpdateStatus("Online", AppId, Email);   //Update status
                                        Users reply = Users.SendFullResponse(Email, AppId, Ip, userAgentHeader.ToString(),token);
                                        return Request.CreateResponse(HttpStatusCode.OK, reply); //Return users detail and token.
                                    }
                                }
                                else
                                {
                                    //Unset Any Old Token From That UserAgent
                                    UserToken.UnSetTokenById(response.Id, "Yes", Ip, userAgentHeader.ToString());
                                    long time = NewToken.ConvertDateTimeToTicks(DateTime.Now.AddMinutes((double)ApplicationCriteria.LengthOfSession));
                                    string token = NewToken.GenerateToken(Email, response.Pasword, Ip, userAgentHeader.ToString(), time);
                                    int result = UserToken.InsertToken(response.Id, token, ApplicationCriteria.LengthOfSession, Ip, userAgentHeader.ToString(), url);
                                    Users reply = Users.SendFullResponse(Email, AppId, Ip, userAgentHeader.ToString(),token);
                                    int status = Users.UpdateStatus("Online", AppId, Email);  //Update status
                                    return Request.CreateResponse(HttpStatusCode.OK, reply); //Return users detail and token.
                                }

                            }
                            else
                            {
                                //Unset Any Old Token From That UserAgent
                                UserToken.UnSetTokenById(response.Id, "Yes", Ip, userAgentHeader.ToString());
                                long time = NewToken.ConvertDateTimeToTicks(DateTime.Now.AddMinutes((double)ApplicationCriteria.LengthOfSession));
                                string token = NewToken.GenerateToken(Email, response.Pasword, Ip, userAgentHeader.ToString(), time);
                                int result = UserToken.InsertToken(response.Id, token, ApplicationCriteria.LengthOfSession, Ip, userAgentHeader.ToString(), url);
                                if (result != -14)
                                {
                                    Users reply = Users.SendFullResponse(Email, AppId, Ip, userAgentHeader.ToString(),token);
                                    int status = Users.UpdateStatus("Online", AppId, Email); //Update status
                                    return Request.CreateResponse(HttpStatusCode.OK, reply); //Return users detail and token.       

                                }
                                else
                                {
                                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not insert now");
                                }
                            }
                            //return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Login Details Mis-Matched");
                        }

                    }
                    else
                    {
                        bool checkPassword = Crypto.VerifyHashedPassword(response.Pasword, Password);
                        if (checkPassword == true)
                        {
                            //Check if application criteria demands multiple logins
                            if (ApplicationCriteria.AllowMultipleLogin == "No")
                            {
                                //Check whether the user already have a valid token.
                                UserToken userTokenDetails = UserToken.GetTokenById(response.Id, "Yes", Ip, userAgentHeader.ToString());
                                if (userTokenDetails != null)
                                {
                                    bool IsTokenActive = IsTokenValidForLogin(userTokenDetails.Token, userTokenDetails.IpAddress, userTokenDetails.UserAgent,out UpdatedNewToken);
                                    if (IsTokenActive == true)
                                    {
                                        //Update status
                                        int status = Users.UpdateStatus("Online", AppId, Email);
                                        UserToken newDetails = UserToken.GetTokenDetailsByToken(UpdatedNewToken);
                                        //Return the token,Ip and UserAgent.
                                        return Request.CreateResponse(HttpStatusCode.OK, newDetails);

                                    }
                                    else
                                    {
                                        //Unset Any Old Token From That UserAgent
                                        UserToken.UnSetTokenById(response.Id, "Yes", Ip, userAgentHeader.ToString());
                                        //Token is not active then generate fresh token
                                        long time = NewToken.ConvertDateTimeToTicks(DateTime.Now.AddMinutes((double)ApplicationCriteria.LengthOfSession));
                                        string token = NewToken.GenerateToken(Email, response.Pasword, Ip, userAgentHeader.ToString(), time);
                                        int result = UserToken.InsertToken(response.Id, token, ApplicationCriteria.LengthOfSession, Ip, userAgentHeader.ToString(), url);
                                        int status = Users.UpdateStatus("Online", AppId, Email);   //Update status
                                        Users reply = Users.SendFullResponse(Email, AppId, Ip, userAgentHeader.ToString(),token);
                                        return Request.CreateResponse(HttpStatusCode.OK, reply); //Return users detail and token.
                                    }
                                }
                                else
                                {
                                    //Unset Any Old Token From That UserAgent
                                    UserToken.UnSetTokenById(response.Id, "Yes", Ip, userAgentHeader.ToString());
                                    long time = NewToken.ConvertDateTimeToTicks(DateTime.Now.AddMinutes((double)ApplicationCriteria.LengthOfSession));
                                    string token = NewToken.GenerateToken(Email, response.Pasword, Ip, userAgentHeader.ToString(), time);
                                    int result = UserToken.InsertToken(response.Id, token, ApplicationCriteria.LengthOfSession, Ip, userAgentHeader.ToString(), url);
                                    Users reply = Users.SendFullResponse(Email, AppId, Ip, userAgentHeader.ToString(),token);
                                    int status = Users.UpdateStatus("Online", AppId, Email);  //Update status
                                    return Request.CreateResponse(HttpStatusCode.OK, reply); //Return users detail and token.
                                }

                            }
                            else
                            {
                                //Unset Any Old Token From That UserAgent
                                UserToken.UnSetTokenById(response.Id, "Yes", Ip, userAgentHeader.ToString());
                                long time = NewToken.ConvertDateTimeToTicks(DateTime.Now.AddMinutes((double)ApplicationCriteria.LengthOfSession));
                                string token = NewToken.GenerateToken(Email, response.Pasword, Ip, userAgentHeader.ToString(), time);
                                int result = UserToken.InsertToken(response.Id, token, ApplicationCriteria.LengthOfSession, Ip, userAgentHeader.ToString(), url);
                                if (result != -14)
                                {
                                    Users reply = Users.SendFullResponse(Email, AppId, Ip, userAgentHeader.ToString(),token);
                                    int status = Users.UpdateStatus("Online", AppId, Email); //Update status
                                    return Request.CreateResponse(HttpStatusCode.OK, reply); //Return users detail and token.       

                                }
                                else
                                {
                                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not insert now");
                                }
                            }
                            //return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Login Details Mis-Matched");
                        }
                    }

                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Email Does Not Exist");
                }

            }
            catch (Exception _ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, _ex.StackTrace);
            }
            
        }

        [HttpPost]
        [Route("api/users/SetNewPassword")]
        public HttpResponseMessage SetNewPassword(int EmpId,string OldPassword,string NewPassword)
        {
            try
            {
                AppId = Thread.CurrentPrincipal.Identity.Name;
                Users response = Users.GetUserById(EmpId, AppId);
                if (response != null)
                {
                    //Check if user password is correct
                    bool IsPasswordCorrect = Crypto.VerifyHashedPassword(response.Pasword, OldPassword);
                    if (IsPasswordCorrect == true)
                    {
                        //Hash New Password.
                        string HashNewPassword = Crypto.HashPassword(NewPassword);
                        //Update the hashed new password.
                        Users.UpdatePassword(EmpId, AppId, HashNewPassword);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        //Old Password is not correct
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Password Not Found");
                    }
                }
                else
                {
                    //User does not exist.
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User Not Found");
                }

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
            }

        }


        [HttpGet]
        [Route("api/users/GetAppCriteria")]
        public HttpResponseMessage GetAppCriteria()
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            AdminAppDetails ApplicationCriteria = AdminAppDetails.GetApplication(AppId);

            if (ApplicationCriteria != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApplicationCriteria);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Details Not Found");
            }

        }

        [HttpGet]
        [Route("api/users/GetUrlsByUserId")]
        public HttpResponseMessage GetUrlsByUserId(int UserId)
        {
            try
            {
                Url url = new Url();
                AppId = Thread.CurrentPrincipal.Identity.Name;
                var response = url.GetUrlsByUserId(AppId, UserId);
                if (response != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Details Not Found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Server Error");
            }
        }


        [HttpGet]
        [Route("api/users/GetUserRolesByUserId")]
        public HttpResponseMessage GetUserRolesByUserId(int UserId)
        {
            try
            {
                AppId = Thread.CurrentPrincipal.Identity.Name;
                var response = Roles.GetUserRolesByUserId(AppId, UserId);
                if (response != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Details Not Found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Server Error");
            }
        }

        [HttpGet]
        [Route("api/users/SearchEmail")]
        public HttpResponseMessage SearchEmail(string Email)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = Users.SearchEmail(Email, AppId);

            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Users Not Found");
            }

        }

        [HttpGet]
        [Route("api/users/GetUserRoleDetails")]
        public HttpResponseMessage GetUserRoleDetails()
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = UserRoles.GetUserRoleDetails(AppId);

            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Details Not Found");
            }

        }

        [HttpPost]
        [Route("api/users/UpdateNewRole")]
        public HttpResponseMessage UpdateNewRole(UserRoles userRoles)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = UserRoles.UpdateNewRole(userRoles.Id, userRoles.RoleId, AppId);

            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error Updating UserRole");
            }

        }


        [HttpDelete]
        [Route("api/users/DeleteNewRole")]
        public HttpResponseMessage DeleteNewRole(int Id)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = UserRoles.DeleteNewRole(Id, AppId);

            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error Deleting UserRole");
            }

        }


        [HttpGet]
        [Route("api/users/TokenValidity")]
        public HttpResponseMessage CheckTokenValidity(string Email,string Token)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            Users response = Users.VerifyLoginDetails(Email, AppId);  //Verify user credential
            HttpRequestMessage currentRequest = this.Request;
            HttpHeaderValueCollection<ProductInfoHeaderValue> userAgentHeader = currentRequest.Headers.UserAgent;
            string Ip = NewToken.GetIPAddress();
            string newToken = "";
            bool IsTokenValid = UsersController.IsTokenValid(Token, Ip, userAgentHeader.ToString(), out newToken);
            if (IsTokenValid == true)
            {
                int UserId = response.Id;
                //Update status
                int status = Users.UpdateStatus("Online", AppId, Email);
                //string token = UserToken.GetToken(UserId,Ip,userAgentHeader.ToString(),newToken);
             
                return Request.CreateResponse(HttpStatusCode.OK, newToken);
            }
            else
            {
                //Update status
                int status = Users.UpdateStatus("Offline", AppId, Email);
                //Deactivate Token
                UserToken.DeactivateToken(Token);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Token Expired");
            }
        }

        [HttpPost]
        [Route("api/users/AddUrl")]
        public HttpResponseMessage AddUrl([FromBody]Url url)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            int response = url.SaveUrl(url.UrlString, AppId, url.MenuName,url.ParentId,url.IconClass);
            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK);

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cant save now");
            }


        }

        [HttpGet]
        [Route("api/users/GetAllParent")]
        public HttpResponseMessage GetAllParent()
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            Url url = new Url();
            var response = url.GetAllParent(AppId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Parent Not Found");
            }
        }

        [HttpGet]
        [Route("api/users/GetAllParentMenu")]
        public HttpResponseMessage GetAllParentMenu()
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            Url url = new Url();
            var response = url.GetAllParentMenu(AppId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Parent Not Found");
            }
        }

        [HttpGet]
        [Route("api/users/GetUrls")]
        public HttpResponseMessage GetUrls()
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            Url url = new Url();
            var response = url.GetUrls(AppId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Urls Not Found");
            }

        }

        [HttpGet]
        [Route("api/users/GetRoleUrls")]
        public HttpResponseMessage GetRoleUrls(int Role_Id)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            Url url = new Url();
            var response = url.GetRoleUrls(Role_Id);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Urls Not Found");
            }

        }

        [HttpGet]
        [Route("api/users/GetRoleName")]
        public HttpResponseMessage GetRoleName(int Id)
        {
            var response = Roles.GetRoleName(Id);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Role Not Found");
            }

        }

        [HttpGet]
        [Route("api/users/GetUrlChildren")]
        public HttpResponseMessage GetUrlChildren( int Id)
        {
            Url url = new Url();
            var response = url.GetUrlChildren(Id);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Urls Not Found");
            }

        }

        [HttpPost]
        [Route("api/users/Addroles")]
        public HttpResponseMessage InsertRoles(UserRoles userRole)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = userRole.InsertRole(userRole.RoleName, AppId,userRole.UserId);
            if (response != -14)
            {
                UserRoles result = userRole.GetRole(userRole.RoleName, AppId);
                if (result != null)
                {
                    RoleUrl roleUrl = new RoleUrl();
                    for (int i = 0; i < userRole.UrlId.Length; i++)
                    {
                        roleUrl.InsertRoleUrl(userRole.UrlId[i], result.Id);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cant Insert Role");
            }

        }


        [HttpPost]
        [Route("api/users/EditRole")]
        public HttpResponseMessage UpdateRole(UserRoles userRole)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            //Update RoleName
            var response = UserRoles.UpdateRole(userRole.RoleName, userRole.Id);
            if (response != -14)
            {
                //Delete Previous Roles Urls
                var deleteRoleUrl = RoleUrl.DeleteRoleUrl(userRole.Id);
                if (deleteRoleUrl != -14)
                {
                    RoleUrl roleUrl = new RoleUrl();
                    for (int i = 0; i < userRole.UrlId.Length; i++)
                    {
                        //Insert Role.
                        roleUrl.InsertRoleUrl(userRole.UrlId[i], userRole.Id);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, " Cant Delete Now");
                }
          
                
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cant Update Role");
            }

        }



        [HttpGet]
        [Route("api/users/GetAllRoles")]
        public HttpResponseMessage GetAllRoles()
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = Roles.GetAllRoles(AppId);
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
        [Route("api/users/SelectUrlById")]
        public HttpResponseMessage SelectUrlById(int Id)
        {
            Url url = new Url();
            var response = url.SelectUrlById(Id);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not Found");
            }
        }


        [HttpDelete]
        [Route("api/users/DeleteRole")]
        public HttpResponseMessage DeleteRole(int Id)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = Roles.DeleteRole(Id,AppId);
            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not Found");
            }
        }


        [HttpPost]
        [Route("api/users/Updaterole")]
        public HttpResponseMessage UpdateUserRole(Users user)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = Users.InsertRole(user.Id, AppId, user.RoleId);
            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error Updating Role");   
            }

        }

        [HttpPost]
        [Route("api/users/Updateapp")]
        public HttpResponseMessage UpdateAppSettings(AdminAppDetails application)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            int response = application.UpdateAppSettings(AppId);
            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cant Update App");
            }

        }

        [HttpPost]
        [Route("api/users/SaveTableToken")]
        public HttpResponseMessage SaveTableToken(string Token)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            int response = FormToken.InsertFormToken(Token, AppId);
            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cant Update Table Token");
            }
        }

        [HttpGet]
        [Route("api/users/GetUrlsById")]
        public HttpResponseMessage GetUrlsById(int Id)
        {
            Url url = new Url();
            AppId = Thread.CurrentPrincipal.Identity.Name;
            var response = url.GetUrlsById(Id, AppId);
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
        [Route("api/users/GetLoginPage")]
        public HttpResponseMessage GetLoginPage()
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            AdminAppDetails model = AdminAppDetails.GetApplication(AppId);
            var path = @"~\Views\Login\Index.cshtml";
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            string viewpath = HttpContext.Current.Server.MapPath(path);
            var template = File.ReadAllText(viewpath);
            string parsedView = Razor.Parse(template, model);
            response.Content = new StringContent(parsedView);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;

        }

        [HttpGet]
        [Route("api/users/GetRegisterPage")]
        public HttpResponseMessage GetRegisterPage()
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            AdminAppDetails model = AdminAppDetails.GetApplication(AppId);
            var path = @"~\Views\Login\Register.cshtml";
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            string viewpath = HttpContext.Current.Server.MapPath(path);
            var template = File.ReadAllText(viewpath);
            string parsedView = Razor.Parse(template, model);
            response.Content = new StringContent(parsedView);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;

        }

        [HttpDelete]
        [Route("api/users/DeleteToken")]
        public HttpResponseMessage DeleteToken(string Token)
        {
            var response = UserToken.DeleteOnlyToken(Token);
            if (response != -14)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cant Delete Token");
            }
        }



        public static bool IsTokenValid(string token, string ip, string userAgent,out string UpdatedNewToken)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            bool result = false;
            string computedToken = "";
            try
            {
                // Base64 decode the string, obtaining the token:username:timeStamp.
                string key = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
                // Split the parts.
                string[] parts = key.Split(new char[] { ':' });
                if (parts.Length == 3)
                {
                    // Get the hash message, username, and timestamp.
                    string hash = parts[0];
                    string username = parts[1];
                    long ticks = long.Parse(parts[2]);
                    DateTime timeStamp = new DateTime(ticks);
                    var response = UserToken.GetTokenRow(ip, userAgent, token);
                    // Ensure the timestamp is valid.
                    if (response != null)
                    {
                        double res = Math.Abs((DateTime.Now - timeStamp).TotalMinutes);
                        
                        bool expired = res > (double)response.ExpiryDateInMinutes;
                        double check = res - (double)response.ExpiryDateInMinutes;
                        if (!expired)
                        {
                            AdminAppDetails ApplicationCriteria = AdminAppDetails.GetApplication(AppId);
                            double newTime = res + (double)ApplicationCriteria.LengthOfSession;
                            int convertedTime = (int)newTime;
                            // Lookup the user's account from the db.
                            Users GetUserDetails = Users.VerifyLoginDetails(username, AppId);

                            if (username == GetUserDetails.Email)
                            {
                                string password = GetUserDetails.Pasword;
                                // Hash the message with the key to generate a token.
                                
                                // Compare the computed token with the one supplied and ensure they match.
                                result = (token == response.Token);
                                //If Token is still valid renew the token
                                if (result == true)
                                {
                                    DateTime dt = DateTime.Now.AddMinutes(convertedTime);
                                    long newTokenTime = NewToken.ConvertDateTimeToTicks(dt);
                                    computedToken = NewToken.GenerateToken(username, password, ip, userAgent, newTokenTime);
                                    int reply =UserToken.UpdateToken(token, ip, userAgent, convertedTime,computedToken);
                                    UpdatedNewToken = computedToken;
                                    return result;
                                  
                                }
                                
                              
                            }
                        }
                        else
                        {
                            UpdatedNewToken = "";
                            return false;
                        }

                    }
                    else
                    {
                        UpdatedNewToken = "";
                        return false;
                    }
                }
            }
            catch
            {
            }
            UpdatedNewToken = computedToken;
            return result;
        }


        public static bool IsTokenValidForLogin(string token, string ip, string userAgent,out string UpdatedNewToken)
        {
            AppId = Thread.CurrentPrincipal.Identity.Name;
            bool result = false;
            string computedToken = "";
            try
            {
                // Base64 decode the string, obtaining the token:username:timeStamp.
                string key = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
                // Split the parts.
                string[] parts = key.Split(new char[] { ':' });
                if (parts.Length == 3)
                {
                    // Get the hash message, username, and timestamp.
                    string hash = parts[0];
                    string username = parts[1];
                    long ticks = long.Parse(parts[2]);
                    DateTime timeStamp = new DateTime(ticks);
                    var response = UserToken.GetTokenRow(ip, userAgent, token);
                    // Ensure the timestamp is valid.
                    if (response != null)
                    {
                        double res = Math.Abs((DateTime.Now - timeStamp).TotalMinutes);

                        bool expired = res > (double)response.ExpiryDateInMinutes;
                        double check = res - (double)response.ExpiryDateInMinutes;
                        if (!expired)
                        {
                            AdminAppDetails ApplicationCriteria = AdminAppDetails.GetApplication(AppId);
                            double newTime = res + (double)ApplicationCriteria.LengthOfSession;
                            int convertedTime = (int)newTime;
                            // Lookup the user's account from the db.
                            Users GetUserDetails = Users.VerifyLoginDetails(username, AppId);

                            if (username == GetUserDetails.Email)
                            {
                                string password = GetUserDetails.Pasword;
                                // Hash the message with the key to generate a token.
                                computedToken = NewToken.GenerateToken(username, password, ip, userAgent, ticks);
                                // Compare the computed token with the one supplied and ensure they match.
                                UserToken.UpdateToken(token, ip, userAgent, convertedTime,computedToken);
                                result = (token == response.Token);
                              
                            }
                        }
                        else
                        {
                            UpdatedNewToken = "";
                            return false;

                        }

                    }
                    else
                    {
                        UpdatedNewToken = "";
                        return false;
                    }
                }
            }
            catch
            {
            }
            UpdatedNewToken = computedToken;
            return result;
        }



    }
}
