using System;
using System.Reflection;
using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    public partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            //this.Text = String.Format("About {0}", AssemblyTitle);
            //this.labelProductName.Text = AssemblyProduct;
            //this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            //this.labelCopyright.Text = AssemblyCopyright;
            //this.labelCompanyName.Text = AssemblyCompany;
            //this.textBoxDescription.Text = AssemblyDescription;
            logoPictureBox.BackgroundImage = appResImg.logo2;

            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = "RRDM For Banks";
            this.labelVersion.Text = "Version Id : Feb 2017";
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = "RRDM Solutions Ltd";
            // GSB VERSION
            this.textBoxDescription.Text = Environment.NewLine 
                                    + "This Version Incorporates Functionality for . " + Environment.NewLine + Environment.NewLine
                                    + "a) Separation of Reconciliation from Matching . " + Environment.NewLine + Environment.NewLine
                                    + "b) Event Schedules and Loading Services " + Environment.NewLine + Environment.NewLine
                                    + "c) Diputes Pre-Investigation Improvements " + Environment.NewLine + Environment.NewLine
                                    + "d) Enquiries Improvements - Version 4 "
                                     ;
            // ITMX VERSION
            //this.textBoxDescription.Text = Environment.NewLine
            //                     + "This Version Incorporates Functionality for . " + Environment.NewLine + Environment.NewLine
            //                     + "a) Separation of Reconciliation from Matching . " + Environment.NewLine + Environment.NewLine
            //                     + "b) Matching of ITMX to Separate Banks. " + Environment.NewLine + Environment.NewLine
            //                     + "c) Reconciliation of FT of BankA to BankB. " + Environment.NewLine + Environment.NewLine
            //                     + "d) View of Information " + Environment.NewLine + Environment.NewLine
            //                     + "e) Disputes Internal and With Banks " + Environment.NewLine + Environment.NewLine
            //                     + "f) Colaboration for disputes settlement " + Environment.NewLine + Environment.NewLine
            //                     + "g) Settlement Totals of FT for all entities and accounting at Central Bank " + Environment.NewLine + Environment.NewLine
            //                     + "h) Settlement of fees " + Environment.NewLine + Environment.NewLine
            //                     + "j) Fees Version Definition and Layers  "
            //                      ;

        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
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

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void AboutBox1_Load(object sender, EventArgs e)
        {

        }
    }
}
