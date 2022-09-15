using mE3DClasses.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aveva.Core.PMLNet;
using System.Xml;

namespace mE3DClasses.BLL
{
    public class dbBussiness
    {
        public string Connstring;
        public dbBussiness(string path,string DBName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            XmlNode xmlnode = xmlDocument.SelectSingleNode("configuration");
            xmlnode = xmlnode.SelectSingleNode("connectionStrings");
            //xmlnode = xmlnode.SelectSingleNode("add");
            XmlNodeList xmlNodeList = xmlnode.ChildNodes;
            foreach (XmlNode node in xmlNodeList)
            {
                string dbname = (node as XmlElement).GetAttribute("name");
                if (dbname == DBName)
                {
                    Connstring = (node as XmlElement).GetAttribute("connectionString");
                    break;
                }
                
            }
        }
 
        public int addSingleItem(Hashtable items, string TableName)
        {
            dbModel model = new dbModel(Connstring);
            //传进来的是个object
            string colName = "(";
            string value = "(";
            Hashtable newitems = ConvertKeytoInt(items);
            Hashtable newitem = new Hashtable();
            foreach (Hashtable item in newitems.Values)
            {
                newitem = ConvertKeytoInt(item);
                colName = colName + newitem[1].ToString() + ",";
                value = value + "'" + newitem[2].ToString() + "',";
            }
            colName = colName.Substring(0, colName.Length - 1) + ")";
            value = value.Substring(0, value.Length - 1) + ")";
            string sql = "INSERT INTO dbo." + TableName + " " + colName + " values "
                         + value;
            var args = new SqlParameter("@test", "");
            return model.executeSql(sql, args);

        }
        public int deleteSingleItem(Hashtable keyitems, string TableName)
        {
            dbModel model = new dbModel(Connstring);
            //传进来的是个object
            string where = "1=1";
            Hashtable newkeyitems = ConvertKeytoInt(keyitems);
            Hashtable newkeyitem = new Hashtable();
            foreach (Hashtable keyitem in newkeyitems.Values)
            {
                newkeyitem = ConvertKeytoInt(keyitem);
                where = where + " and " + newkeyitem[1].ToString() + "='" + newkeyitem[2].ToString() + "'";

            }
            string sql = "delete from dbo." + TableName + " where "
                         + where;
            var args = new SqlParameter("@test", "");
            return model.executeSql(sql, args);

        }
        //
        public int updateSingleItem(Hashtable keyitems, Hashtable items, string TableName)
        {
            dbModel model = new dbModel(Connstring);
            //传进来的是个object
            string set = "";
            string where = "1=1";
            Hashtable newitems = ConvertKeytoInt(items);
            Hashtable newitem = new Hashtable();
            Hashtable newkeyitems = ConvertKeytoInt(keyitems);
            Hashtable newkeyitem = new Hashtable();
            foreach (Hashtable keyitem in newkeyitems.Values)
            {
                newkeyitem = ConvertKeytoInt(keyitem);
                where = where + " and " + newkeyitem[1].ToString() + "='" + newkeyitem[2].ToString() + "'";

            }
            foreach (Hashtable item in items.Values)
            {
                newitem = ConvertKeytoInt(item);
                set = set + newitem[1].ToString() + "='" + newitem[2].ToString() + "',";

            }
            set = set.Substring(0, set.Length - 1);
            string sql = "update dbo." + TableName + " set " + set + " where "
                         + where;

            var args = new SqlParameter("@test", "");
            return model.executeSql(sql, args);

        }
        public List<object> queryItem(string Condition, string TableName)
        {
            //传进来的是个object
            dbModel model = new dbModel(Connstring);
            string sql = "Select * dbo." + TableName + " where "
                         + Condition;
            var args = new SqlParameter("@test", "");
            return model.SqlQuery(sql, args);

        }
        public bool isExistItem(string Condition, string TableName)
        {
            //传进来的是个object
            dbModel model = new dbModel(Connstring);
            string sql = "Select * from dbo." + TableName + " where "
                         + Condition;
            var args = new SqlParameter("@test", "");
            int count = model.SqlQuery(sql, args).Count;
            if (count == 0)
            {
                return false;
            }
            return true;
        }
        //先查询，有就更新，没有就插入新的
        public string insertorupdateSingleItem(Hashtable keyitems, Hashtable items, string TableName)
        {
            //1=1占位
            string conditioin = "1=1 ";
            Hashtable newkeyitems = ConvertKeytoInt(keyitems);
            Hashtable newkeyitem = new Hashtable();
            foreach (Hashtable keyitem in keyitems.Values)
            {
                newkeyitem = ConvertKeytoInt(keyitem);
                conditioin = conditioin + " and " + newkeyitem[1].ToString() + "='"
                             + newkeyitem[2].ToString() + "'";
            }

            if (isExistItem(conditioin, TableName))
            {
                if (updateSingleItem(keyitems, items, TableName) == 0)
                {
                    return "update fail";
                }
                else
                {
                    return "update success";
                }
            }
            else
            {
                Hashtable total = ConvertKeytoInt(keyitems);

                foreach (object key in items.Keys)
                {
                    total.Add(total.Count + 1, items[key]);
                }
                if (addSingleItem(total, TableName) == 0)
                {
                    return "insert fail";
                }
                else
                {
                    return "insert success";
                }
            }
        }
        private Hashtable ConvertKeytoInt(Hashtable hashtable)
        {
            Hashtable result = new Hashtable();
            foreach (object key in hashtable.Keys)
            {
                if (hashtable[key].GetType().Name == "PMLNetAny")
                {                     
                    result.Add(Convert.ToInt32(key), hashtable[key].ToString());
                }
                else
                {
                    result.Add(Convert.ToInt32(key), hashtable[key]);
                }
                
            }
            return result;
        }
    }
}
