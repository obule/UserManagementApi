using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace BTS__User__Mangement__API.Models
{
    public class Admin //Refactor this Code
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Pasword { get; set; }
        public string ApiKey { get; set; }
        public string CompanyName { get; set; }
        public string ApplicationId { get; set; }
        public int UserId { get; set; }
        public string EmailConfirmationCode { get; set; }


        public  int RegisterAdmin()
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC AdminRegistration @0,@1,@2", Email, Pasword,EmailConfirmationCode);
                int response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {

                return -14;
            }
        }

        public static int InsertAdminAppDetail(string ApplicationId,string ApiKey,string CompanyName,int AdminId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC InsertAdminAppDetail @0,@1,@2,@3", ApplicationId, ApiKey, CompanyName, AdminId);
                int response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }


        public Admin EmailCheck()
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC AdminLoginCheck @0", Email);
                Admin response = DataContext.FirstOrDefault<Admin>(sql);
                return response;

            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static Admin GetNewAppDetails(string ApplicationId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetNewAppDetails @0", ApplicationId);
                Admin response = DataContext.FirstOrDefault<Admin>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IEnumerable<Admin> GetAllCompanyName(int AdminId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetAllCompanyName @0", AdminId);
                IEnumerable<Admin> response = DataContext.Query<Admin>(sql);
                return response;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Admin GetAdminIdForExec(string ApplicationId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetAdminIdForExec @0", ApplicationId);
                Admin response = DataContext.FirstOrDefault<Admin>(sql);
                return response;

            }
            catch (Exception ex)
            {
                return null;
            }
        }





    }
}