using Aveva.Core.PMLNet;
using mE3DClasses;
using mE3DClasses.BLL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aveva.Core.mE3DClasses.Common
{
    [PMLNetCallable()]
    public class DBOperator
    {
        [PMLNetCallable()]
        public string configPath { get; set; }
        [PMLNetCallable()]
        public string DBName { get; set; }
        [PMLNetCallable()]
        public DBOperator()
        {

        }
       /* [PMLNetCallable()]
        public DBOperator(string path,string dbname)
        {
            this.configPath = path;
            this.DBName = dbname;
        }*/
        [PMLNetCallable()]
        public void Initial(string path, string dbname)
        {
            this.configPath = path;
            this.DBName = dbname;
        }
        [PMLNetCallable()]
        public void Assign(DBOperator that)
        {
            //No state
        }
        [PMLNetCallable()]
        public int addSingleItem(Hashtable items, string TableName)
        {
            dbBussiness bussiness = new dbBussiness(configPath, DBName);    
            //dbBussiness.initialDB(configPath, DBName);
            return bussiness.addSingleItem(items, TableName);            
        }
        [PMLNetCallable()]
        public bool isExistItem(string Condition, string TableName)
        {
            dbBussiness bussiness = new dbBussiness(configPath, DBName);
            //dbBussiness.initialDB(configPath, DBName);
            return bussiness.isExistItem(Condition, TableName);
        }
        [PMLNetCallable()]
        public string insertorupdateItem(Hashtable keyitems, Hashtable items, string TableName)
        {
            dbBussiness bussiness = new dbBussiness(configPath, DBName);
            //dbBussiness.initialDB(configPath, DBName);                      
            return bussiness.insertorupdateSingleItem(keyitems, items, TableName);
             
            
        }
        [PMLNetCallable()]
        public int updateItem(Hashtable keyitems, Hashtable items, string TableName)
        {
            dbBussiness bussiness = new dbBussiness(configPath, DBName);
            //dbBussiness.initialDB(configPath, DBName);
            return bussiness.updateSingleItem(keyitems, items, TableName);
        }
        [PMLNetCallable()]
        public int deleteItem(Hashtable keyitems, string TableName)
        {
            dbBussiness bussiness = new dbBussiness(configPath, DBName);
            //dbBussiness.initialDB(configPath, DBName);
            return bussiness.deleteSingleItem(keyitems, TableName);
        }

        
        /* [PMLNetCallable()]
         public void addSingleItem(PMLNetAny obj,string TableName)
         {
             try
             {
                 object dbClass = ConstructObject(obj);

                 object result = null;
                 //获取pml对象中的每个属性的值，赋予db对象
                 obj.invokeMethod("attributes", null, 0, ref result);
                 Hashtable attributes = (Hashtable)result;

                 foreach (string value in attributes.Values)
                 {
                     string attribute = value;
                     try
                     {
                         obj.getStructureMember(attribute, ref result);
                         dbClass.GetType().GetProperty(attribute).SetValue(dbClass, result, null);
                     }
                     catch (Exception ex)
                     {
                         continue;
                     }

                     MessageBox.Show(attribute + "," + dbClass.GetType().GetProperty(attribute).GetValue(dbClass).ToString());
                 }
                 string dbTable = dbClass.GetType() + "Bussiness";
                 var dbTableBussiness = Type.GetType(dbTable).Assembly.CreateInstance(dbTable);
                 dbTableBussiness.GetType().GetMethod("addSingleItem").Invoke(dbTableBussiness, new object[1] { dbClass });
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message);
             }

         }
         //通过pml中传入的对象，构建数据库对象
         public object ConstructObject(PMLNetAny obj)
         {
             string ObjectTypeName = obj.type();
             dynamic mapdbClass = null;
             if (Type.GetType(ObjectTypeName).IsClass)
             {
                 mapdbClass = Type.GetType(ObjectTypeName).Assembly.CreateInstance(ObjectTypeName);               
             }
             Convert.ChangeType(mapdbClass, Type.GetType(ObjectTypeName));
             return mapdbClass;
         }*/
    }
}
