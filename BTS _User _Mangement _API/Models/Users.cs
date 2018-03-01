using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace BTS__User__Mangement__API.Models
{
    public class Users 
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Pasword { get; set; }
        public string Firstname { get; set; }
        public string Othernames { get; set; }
        public string EmailConfirmationCode { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string DateCreated { get; set; }  //Repair
        public string ApplicationCode { get; set; }
        
        public bool IsEmailConfirmed { get; set; }

        public int AdminId { get; set; }

        public string Email { get; set; }

        public int RoleId { get; set; }

        public string  Token { get; set; }

        public string Status { get; set; }




        public Users InsertUsers()
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC UsersRegistration @0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10", Username, Pasword, Firstname, Othernames, EmailConfirmationCode, Surname, PhoneNumber, DateCreated, ApplicationCode,IsEmailConfirmed, Email);
                Users response = DataContext.FirstOrDefault<Users>(sql);
                return response;

            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public IEnumerable<Users> RetriveUsersDetails(string ApplicationCode)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC GetUsers @0", ApplicationCode);
                IEnumerable<Users> response = DataContext.Query<Users>(sql);

                return response;

            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static string GetAppIdWithVerificationCode(string EmailConfirmationCode)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC GetAppIdWithVerificationCode @0", EmailConfirmationCode);
                string response = DataContext.ExecuteScalar<string>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int CountUsers(string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC CountUsers @0", AppId);
                int response = DataContext.ExecuteScalar<int>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static int CountUsersOnline(string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC CountUsersOnline @0", AppId);
                int response = DataContext.ExecuteScalar<int>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static IEnumerable<Users> SearchEmail(string Email,string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC SearchEmail @0,@1", Email, AppId);
                var response = DataContext.Query<Users>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int CountRole(string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC CountRole @0", AppId);
                int response = DataContext.ExecuteScalar<int>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static Users GetUserById(int id, string ApplicationCode)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC GetUserById @0,@1", id, ApplicationCode);
                Users response = DataContext.FirstOrDefault<Users>(sql);
                return response;

            }
            catch (Exception ex)
            {

                return null;
            }

        }

        //Update Status
        public static int UpdateStatus(string status,string AppId,string Email)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC UpdateStatus @0,@1,@2", status, AppId, Email);
                var response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        //Delete User
        public static int DeleteUser(int id)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC DeletUser @0", id);
                int response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception)
            {

                return -14;
            }
        }

        //Update User
        public int UpdateUser(int id)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC UpdateUser @0,@1,@2,@3,@4,@5", id,Username,Firstname,Othernames,Surname,PhoneNumber);
                int response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception)
            {

                return -14;
            }

        }

        public static Users VerifyLoginDetails(string Email,string ApplicationCode)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC VerifyLoginDetails @0,@1", Email,ApplicationCode);
                Users response = DataContext.FirstOrDefault<Users>(sql);
                return response;
            }
            catch (Exception ex)
            {

                return null;
            }
        }



        public static Users VerifyLoginDetailsWithEmail(string Email)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC VerifyLoginDetailsWithEmail @0", Email);
                Users response = DataContext.FirstOrDefault<Users>(sql);
                return response;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static Users VerifyAdminLoginDetailsWithEmail(string Email)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC VerifyAdminLoginDetailsWithEmail @0", Email);
                Users response = DataContext.FirstOrDefault<Users>(sql);
                return response;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static int InsertRole(int Id,string ApplicationCode, int RoleId)
        {
            try
            { 
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC InsertRole @0,@1,@2", Id, ApplicationCode, RoleId);
                int response = DataContext.Execute(sql);
                return response;

            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static string VerifyConfirmationCode(string EmailConfirmationCode,bool IsEmailConfirmed)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC VerfyConfirmationCode @0,@1", EmailConfirmationCode, IsEmailConfirmed);
                string response = DataContext.ExecuteScalar<string>(sql);
                return response;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static int UpdateUserStatus(string EmailConfirmationCode, bool IsEmailConfirmed)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC UpdateUserStatus @0,@1", EmailConfirmationCode, IsEmailConfirmed);
                int response = DataContext.Execute(sql);
                return response;

            }
            catch (Exception ex)
            {
                return -14;
            }

        }

        public static int UpdatePassword(int id,string AppId,string password)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC UpdatePassword @0,@1,@2", id, AppId,password);
                int response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static string CheckIfEmailAlreadyExist(string Email,string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC CheckIfEmailAlreadyExist @0,@1", Email,AppId);
                string response = DataContext.ExecuteScalar<string>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static Users SendFullResponse(string Email,string AppId,string IpAddress,string UserAgent,string token)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC SendFullResponse @0,@1,@2,@3,@4", Email, AppId, IpAddress, UserAgent,token);
                Users response = DataContext.FirstOrDefault<Users>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}