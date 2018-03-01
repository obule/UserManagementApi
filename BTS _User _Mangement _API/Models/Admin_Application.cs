using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;


namespace BTS__User__Mangement__API.Models
{
    public class Admin_Application
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string ApplicationId { get; set; }


        public static int InsertAdminApplication(int AdminId, string ApplicationId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC InsertAdminApplication @0,@1", AdminId, ApplicationId);
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