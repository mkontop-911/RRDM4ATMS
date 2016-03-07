namespace RRDMJTMService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RRDMJTMSvcProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.RRDMJTMSvcInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // RRDMJTMSvcProcessInstaller
            // 
            this.RRDMJTMSvcProcessInstaller.Password = null;
            this.RRDMJTMSvcProcessInstaller.Username = null;
            // 
            // RRDMJTMSvcInstaller
            // 
            this.RRDMJTMSvcInstaller.DisplayName = "RRDM Journal Transfer Manager Service";
            this.RRDMJTMSvcInstaller.ServiceName = "RRDMJTMSvc";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.RRDMJTMSvcProcessInstaller,
            this.RRDMJTMSvcInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller RRDMJTMSvcProcessInstaller;
        private System.ServiceProcess.ServiceInstaller RRDMJTMSvcInstaller;
    }
}