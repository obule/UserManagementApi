using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace BTS__User__Mangement__API.Models
{
    public class Roles
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string AppId { get; set; }

        public static IEnumerable<Roles> GetAllRoles(string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetAllRoles @0", AppId);
                IEnumerable<Roles> response = DataContext.Query<Roles>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static int DeleteRole(int Id,string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC DeleteRole @0,@1", Id,AppId);
                int response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static string GetRoleName(int Id)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetRoleName @0", Id);
                string response = DataContext.ExecuteScalar<string>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IEnumerable<Roles> GetUserRolesByUserId(string AppId,int UserId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetUserRolesByUserId @0,@1", AppId, UserId);
                var response = DataContext.Query<Roles>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



    }
}