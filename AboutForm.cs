﻿using System;
using System.Reflection;
using System.Windows.Forms;

namespace Email2Zalo
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            this.labelProduct.Text = AssemblyProduct;
            this.labelVersion.Text = AssemblyVersion;
        }

        public string AssemblyVersion
        {
            get
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("Version {0}.{1}", v.Major, v.Minor);
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
