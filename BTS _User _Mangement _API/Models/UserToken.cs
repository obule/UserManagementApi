using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace BTS__User__Mangement__API.Models
{
    public class UserToken

    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }

        public int ExpiryDateInMinutes { get; set; }

        public string IpAddress { get; set; }

        public string UserAgent { get; set; }

        public string Url { get; set; }

        public string IsTokenValid { get; set; }

        public string Email { get; set; }



        public static int InsertToken(int UserId,string Token,int? ExpiryDateInMinutes,string IpAddress,string UserAgent,string Url)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC InsertToken @0,@1,@2,@3,@4,@5,@6", UserId, Token, ExpiryDateInMinutes, IpAddress, UserAgent, Url,"Yes");
                var response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static UserToken GetTokenRow(string IpAddress,string UserAgent,string Token)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetTokenRow @0,@1,@2", IpAddress, UserAgent, Token);
                UserToken response = DataContext.FirstOrDefault<UserToken>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int UpdateToken(string Token,string IpAddress,string UserAgent,int ExpiryDate,string NewToken)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC UpdateToken @0,@1,@2,@3,@4", Token, IpAddress, UserAgent,ExpiryDate,NewToken);
                var response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static int DeactivateToken(string Token)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC DeactivateToken @0,@1", Token, "No");
                var response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static string GetToken(int UserId,string Ip,string UserAgent,string Token)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetToken @0,@1,@2,@3", UserId,Ip,UserAgent,Token);
                string response = DataContext.ExecuteScalar<string>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static int DeleteToken(string Token,string IpAddress,string UserAgent)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC DeleteToken @0,@1,@2", Token, IpAddress, UserAgent);
                int response = DataContext.Execute(sql);
                return response;

            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static UserToken GetTokenById(int UserId,string IsTokenValid,string Ip,string UserAgent)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetTokenById @0,@1,@2,@3", UserId, IsTokenValid,Ip,UserAgent);
                UserToken response = DataContext.FirstOrDefault<UserToken>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public static int UnSetTokenById(int UserId, string IsTokenValid, string Ip, string UserAgent)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC UnSetTokenById @0,@1,@2,@3", UserId, IsTokenValid, Ip, UserAgent);
                int response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }

        }

        public static int DeleteOnlyToken(string Token)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC DeleteOnlyToken @0", Token);
                int response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static UserToken GetTokenDetailsByToken(string Token)
        {
            try
            {
                var DataContext = new Database();
                var sql = Sql.Builder.Append(";EXEC GetTokenDetailsByToken @0", Token);
                var response = DataContext.FirstOrDefault<UserToken>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }





    }



}