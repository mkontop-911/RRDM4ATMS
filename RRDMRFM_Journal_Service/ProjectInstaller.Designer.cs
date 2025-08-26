namespace RRDMRFMJService
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
            this.RRDMRFMJSvcProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.RRDMRFMJSvcInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // RRDMRFMSvcProcessInstaller
            // 
            this.RRDMRFMJSvcProcessInstaller.Password = null;
            this.RRDMRFMJSvcProcessInstaller.Username = null;
            // 
            // RRDMRFMSvcInstaller
            // 
            this.RRDMRFMJSvcInstaller.Description = "RRDM Reconciliation File Moniror for Journals Service";
            this.RRDMRFMJSvcInstaller.DisplayName = "RRDM Reconciliation File Moniror for Journals Service";
            this.RRDMRFMJSvcInstaller.ServiceName = "RRDMRFMJSvc";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.RRDMRFMJSvcProcessInstaller,
            this.RRDMRFMJSvcInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller RRDMRFMJSvcProcessInstaller;
        private System.ServiceProcess.ServiceInstaller RRDMRFMJSvcInstaller;
    }
}