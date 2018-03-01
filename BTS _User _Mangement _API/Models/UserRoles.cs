using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace BTS__User__Mangement__API.Models
{
    public class UserRoles
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string AppId { get; set; }
        public int UserId { get; set; }
        public string  Surname { get; set; }
        public int RoleId { get; set; }
        public int[] UrlId { get; set; }


        public int InsertRole(string RoleName,string AppId,int UserId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC InsertRoles @0,@1,@2", RoleName, AppId,UserId);
                var response = DataContext.Execute(sql);
                return response;

            }
            catch (Exception ex)
            {

                return -14;
            }
        }

        public UserRoles GetRole(string RoleName, string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetRole @0,@1", RoleName, AppId);
                var response = DataContext.FirstOrDefault<UserRoles>(sql);
                return response;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int UpdateRole(string RoleName, int Id)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC UpdateRole @0,@1", RoleName, Id);
                int response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static IEnumerable<UserRoles> GetUserRoleDetails(string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetUserRoleDetails @0", AppId);
                IEnumerable<UserRoles> response = DataContext.Query<UserRoles>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int UpdateNewRole(int Id,int RoleId,string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC UpdateNewRole @0,@1,@2", Id, RoleId, AppId);
                var response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }


        public static int DeleteNewRole(int Id, string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC DeleteNewRole @0,@1", Id, AppId);
                var response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }


    }
}