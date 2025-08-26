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
            this.RRDMRFMSvcProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.RRDMRFMSvcInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // RRDMRFMSvcProcessInstaller
            // 
            this.RRDMRFMSvcProcessInstaller.Password = null;
            this.RRDMRFMSvcProcessInstaller.Username = null;
            // 
            // RRDMRFMSvcInstaller
            // 
            this.RRDMRFMSvcInstaller.Description = "RRDM Reconciliation File Moniror Service";
            this.RRDMRFMSvcInstaller.DisplayName = "RRDM Reconciliation File Moniror Service";
            this.RRDMRFMSvcInstaller.ServiceName = "RRDMRFMSvc";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.RRDMRFMSvcProcessInstaller,
            this.RRDMRFMSvcInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller RRDMRFMSvcProcessInstaller;
        private System.ServiceProcess.ServiceInstaller RRDMRFMSvcInstaller;
    }
}