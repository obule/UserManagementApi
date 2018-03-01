using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace BTS__User__Mangement__API.Models
{
    public class API_Security
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ApplicationId { get; set; }
        public string ApiKey { get; set; }


        public static API_Security CheckAdminCredentials(string AppId, string ApiKey)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC CheckAdminLogin @0,@1", AppId, ApiKey);
                API_Security response = DataContext.FirstOrDefault<API_Security>(sql);
                return response;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

    }
}