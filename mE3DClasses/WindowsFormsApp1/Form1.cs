using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aveva.Core.mE3DClasses.Common;
using mE3DClasses.DAL;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //dbModel.InitialDB(@"D:\AVEVA\E3D\E3D2.1\Everything3D2.10\mProjects\PMLLIB\BuildingForce\Configuration", "BuildingObject");
            DBOperator xx = new DBOperator();
            xx.Initial(@"D:\AVEVA\E3D\E3D2.10\Everything3D2.10\mProjects\PMLLIB\BuildingForce\Configuration\app.config", "ForceCalculation");
            Hashtable test = new Hashtable();
            Hashtable test2 = new Hashtable();
            Hashtable temp = new Hashtable();
            temp.Add(1, "buildingref");
            temp.Add(2, "xxyy");
            test.Add(1,temp);
            temp = new Hashtable();
            temp.Add(1, "length");
            temp.Add(2, "30");
            test2.Add(1, temp);
            xx.insertorupdateItem(test,test2,"BuildingObject");
            //XMLOperator xMLOperator = new XMLOperator();
            //xMLOperator.WritexmlFile("d:\\ol.xml", "xx yy zz", "aa", "all-rigth");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            XMLOperator xMLOperator = new XMLOperator();
            Hashtable ht = new Hashtable();
            ht.Add(0, "path");    
            ht.Add(1, "hierachy");
            ht.Add(2, "dtext");
            ht.Add(3, "rtext");
            Hashtable ht2 = xMLOperator.ReadxmlFile(@"D:\AVEVA\E3D\E3D2.1\Everything3D2.10\PMLLIB\HBCompany\HangerDesign\Configuration\DesignConfig.xml", "configs JointForm JointType reffile", ht);
        }
    }
}
