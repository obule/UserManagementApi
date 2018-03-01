using System;
using PetaPoco;

namespace BTS__User__Mangement__API.Models
{
    public class FormToken

    {

        public int Id { get; set; }

        public string ApplicationId { get; set; }

        public string TableToken { get; set; }

        public static int InsertFormToken(string TableToken,string ApplicationId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC InsertFormToken @0,@1", TableToken,ApplicationId);
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