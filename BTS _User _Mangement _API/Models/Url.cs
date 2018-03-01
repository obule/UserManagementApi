using System;
using System.Collections.Generic;
using PetaPoco;

namespace BTS__User__Mangement__API.Models
{
    public class Url
    {
        public int Id { get; set; }
        public string UrlString { get; set; }
        public string AppId { get; set; }
        public string MenuName { get; set; }
        public string IconClass { get; set; }
        public int ParentId { get; set; }
        List<Url> ParentChild { get; set; }

        public  int SaveUrl(string urlstring,string AppId, string Urlname,int ParentId,string IconClass)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC InsertUrl @0,@1,@2,@3,@4", urlstring, AppId, Urlname,ParentId,IconClass);  
                var response = DataContext.Execute(sql);
                return response;

            }
            catch (Exception ex)
            {

                return -14;
            }

        }

        public int UpdateUrl(int Id)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC UpdateUrl @0,@1,@2,@3", Id, UrlString, MenuName, IconClass);
                var response = DataContext.Execute(sql);
                return response;

            }
            catch (Exception ex)
            {

                return -14;
            }

        }

        public int DeleteUrlById(int Id)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC DeleteUrlById @0", Id);
                var response = DataContext.Execute(sql);
                return response;

            }
            catch (Exception ex)
            {

                return -14;
            }

        }

        public IEnumerable<Url> GetUrls(string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC GetUrls @0", AppId);
                var response = DataContext.Query<Url>(sql);
                return response;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public  IEnumerable<Url> GetRoleUrls(int Role_Id)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetRoleUrls @0", Role_Id);
                var response = DataContext.Query<Url>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }


        public IEnumerable<Url> GetUrlChildren(int Id)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append("; EXEC GetUrlChildren @0", Id);
                var response = DataContext.Query<Url>(sql);
                return response;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public  IEnumerable<Url> GetUrlsById(int Id, string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetUrlsById @0,@1", Id, AppId);
                var response = DataContext.Query<Url>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public  IEnumerable<Url> GetAllParent(string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetAllParent @0",AppId);
                IEnumerable<Url> response = DataContext.Query<Url>(sql);

                List<Url> Child = new List<Url>();
                foreach (var item in response)
                {
                    Child.Add(item);
                    var reply = GetUrlChildren(item.Id);
                    foreach (var item2 in reply)
                    {
                            Child.Add(item2);
                       
                    }
                }
                return Child;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public IEnumerable<Url> GetAllParentMenu(string AppId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetAllParent @0", AppId);
                IEnumerable<Url> response = DataContext.Query<Url>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Url SelectUrlById(int Id)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC SelectUrlById @0", Id);
                Url response = DataContext.FirstOrDefault<Url>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public  IEnumerable<Url> GetUrlsByUserId(string AppId,int UserId)
        {
            try
            {
                var DataContext = new PetaPoco.Database();
                var sql = Sql.Builder.Append(";EXEC GetUrlsByUserId @0,@1", AppId, UserId);
                var response = DataContext.Query<Url>(sql);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}