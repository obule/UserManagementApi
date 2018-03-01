using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace BTS__User__Mangement__API.Models
{
    public class AdminAppDetails
    {
        public int Id { get; set; }

        public string ApplicationId { get; set; }

        public string ApiKey { get; set; }

        public string CompanyName { get; set; }

        public int UserId { get; set; }

        public bool? IsBlocked { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateModify { get; set; }

        public int? ModifyBy { get; set; }

        public string HaveSmtpServer { get; set; }

        public string SmtpHost { get; set; }

        public string SmtpPassword { get; set; }

        public string SmtpUseSsl { get; set; }

        public string SmtpSenderName { get; set; }

        public int? SmtpPort { get; set; }

        public string NeedEmailConfirmation { get; set; }

        public string Email_Message { get; set; }

        public int? LengthOfSession { get; set; }

        public string AllowMultipleLogin { get; set; }

        public string SmtpUsername { get; set; }

        public string CallBackUrl { get; set; }

        public string LogoUrl { get; set; }




        public static int InsertApplication(string Application_Name, string Application_Code,int AdminId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC InsertApplication @0,@1,@2", Application_Name, Application_Code, AdminId);
                var response = DataContext.Execute(sql);
                return response;

            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static int GetAdminId(string Email)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetAdminId @0", Email);
                int response = DataContext.ExecuteScalar<int>(sql);
                return response;

            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static AdminAppDetails GetApplication(string Application_Code)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetApplication @0", Application_Code);
                AdminAppDetails response = DataContext.FirstOrDefault<AdminAppDetails>(sql);
                return response;

            }
            catch (Exception ex)
            {
                return null;

            }
        }

        public  int UpdateAppSettings(string Application_Code)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC UpdateAppSettings @0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13",  SmtpUseSsl,SmtpSenderName,SmtpPort,AllowMultipleLogin,LengthOfSession,NeedEmailConfirmation,Email_Message,HaveSmtpServer,SmtpHost,SmtpUsername,SmtpPassword,Application_Code,CallBackUrl,LogoUrl);
                int response = DataContext.Execute(sql);
                return response;

            }
            catch (Exception ex)
            {
                return -14;
            }
        }
    }
}