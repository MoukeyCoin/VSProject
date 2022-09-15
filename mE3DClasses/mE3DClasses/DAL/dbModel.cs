using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mE3DClasses.DAL
{
    public class dbModel
    {
        public DbContext dbContext;
        public dbModel(string Connstr)
        {
            dbContext = new DbContext(Connstr);
        }

        public int executeSql(string sql, params SqlParameter[] args)
        {
            int quantity = 0;
            using (var db = dbContext)
            {
                //string connstr = db.Database.Connection.ConnectionString;
                           
                
                try
                {
                    //db.Database.Connection.Open();
                    quantity = db.Database.ExecuteSqlCommand(sql, args);
                    db.SaveChanges();
                    //db.Database.Connection.Close();
                }
                catch (Exception ex)
                {
                }
            }
           
           
            return quantity;
        }
        public List<object> SqlQuery(string sql, params SqlParameter[] args)
        {
            List<object> result = new List<object>() ;
            using (var db = dbContext)
            {
                //string connstr = db.Database.Connection.ConnectionString;


                try
                {
                    //db.Database.Connection.Open();
                    result = db.Database.SqlQuery<object>(sql,args).ToList();
                    //db.Database.Connection.Close();
                }
                catch (Exception ex)
                {
                }
            }
            return result;
        }
    }
}
