using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;
namespace BTS__User__Mangement__API.Models
{
    public class RoleUrl
    {
        public int Id { get; set; }
        public int Url_Id { get; set; }
        public int Role_Id { get; set; }

        public int InsertRoleUrl(int Url_Id, int Role_Id)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC InsertRole_Url @0,@1", Url_Id, Role_Id);
                var response = DataContext.Execute(sql);
                return response;
            }
            catch (Exception ex)
            {
                return -14;
            }
        }

        public static int DeleteRoleUrl(int Role_Id)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC DeleteRoleUrl @0",Role_Id);
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