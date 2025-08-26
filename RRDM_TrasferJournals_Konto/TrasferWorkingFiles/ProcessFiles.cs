using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRDM_TransferJournals
{
    public class TransferJournals
    {
        public enum FileTransferType
        {
            DBL,
            NCR,
            NCR_VISION,
            OTHER,
            WCR
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }

        public FileTransferType TrasferType { get; set; }

        public int NoOfFiles { get; set; }
        public string ErrorMessage { get; set; }
        public bool ErrorIndicator { get; set; }



        private static int _iNoOfFiles;
        private static int _iBackwardNoOfDays;
        private static int _iForwardNoOfDays;
        private bool _ErrorIndicator;
        private static string _sErrorMessage;
        public static string _sSourceDirectory;
        public static string _sTargetDirectory;
        public static DateTime _dtFrDate;
        public static DateTime _dtToDate;

        public void filesProcessing()
        {
            _sSourceDirectory = this.SourceDirectory;
            _sTargetDirectory = this.TargetDirectory;
            _dtFrDate = this.FromDate;
            _dtToDate = this.ToDate;
            _iForwardNoOfDays = Int32.Parse((string)ConfigurationManager.AppSettings["ForwardNoOfDays"]);
            _iBackwardNoOfDays = Int32.Parse((string)ConfigurationManager.AppSettings["BackwardNoOfDays"]);
            _ErrorIndicator = false;
            _sErrorMessage = string.Empty;


            switch (this.TrasferType)
            {
                case FileTransferType.DBL:
                    ProcessDbl();
                    break;
                case FileTransferType.NCR:
                    ProcessNcr();
                    break;
                case FileTransferType.NCR_VISION:
                    ProcessNcrVision();
                    break;
                case FileTransferType.OTHER:
                    ProcessOth();
                    break;
                case FileTransferType.WCR:
                    ProcessWcr();
                    break;
                default:
                    break;
            }

            this.ErrorMessage = _sErrorMessage;
            this.NoOfFiles = _iNoOfFiles;
            this.ErrorIndicator = _ErrorIndicator;

        }
        private void ProcessDbl()
        {
            _iNoOfFiles = 0;
            modDBL.iBackwardNoOfDays = _iBackwardNoOfDays;
            modDBL.iForwardNoOfDays = _iForwardNoOfDays;
            modDBL.iNoOfFiles = 0;
            modDBL.dtFromDate = _dtFrDate;
            modDBL.dtToDate = _dtToDate;

            if (string.IsNullOrEmpty(_sSourceDirectory))
                modDBL.sSourceDirectory = (string)ConfigurationManager.AppSettings["DBL_Path"];
            else
                modDBL.sSourceDirectory = _sSourceDirectory;

            if (string.IsNullOrEmpty(_sTargetDirectory))
                modDBL.sTargetDirectory = (string)ConfigurationManager.AppSettings["DBL_TargetDir"];
            else
                modDBL.sTargetDirectory = _sTargetDirectory;

            try
            {
                modDBL.ProcessDblDirectory();
                _iNoOfFiles = modDBL.iNoOfFiles;
                _ErrorIndicator = modDBL.bErrorIndicator;
                _sErrorMessage = modDBL.sErrorMessage;
                if ((_ErrorIndicator == false) && (modDBL.sFilesNotProcessed.Trim().Length > 0))
                {
                    _ErrorIndicator = true;
                    _sErrorMessage = modDBL.sFilesNotProcessed.Trim();
                }
            }
            catch (Exception ex)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("ProcessDbl -An exception has occured while processing DBL Files. The Operation is aborted.");
                sb.AppendLine("Exception Details:");
                sb.AppendLine(ex.Message);
                _sErrorMessage = sb.ToString();
                _ErrorIndicator = true;
                return;
            }
            finally
            {
            }
        }


        public void ProcessNcr()
        {
            _iNoOfFiles = 0;
            modNCR.iBackwardNoOfDays = _iBackwardNoOfDays;
            modNCR.iForwardNoOfDays = _iForwardNoOfDays;
            modNCR.iNoOfFiles = 0;
            modNCR.dtFromDate = _dtFrDate;
            modNCR.dtToDate = _dtToDate;
            if (string.IsNullOrEmpty(_sSourceDirectory))
                modNCR.sSourceDirectory = (string)ConfigurationManager.AppSettings["NCR_Path"];
            else
                modNCR.sSourceDirectory = _sSourceDirectory;

            if (string.IsNullOrEmpty(_sTargetDirectory))
                modNCR.sTargetDirectory = (string)ConfigurationManager.AppSettings["NCR_TargetDir"];
            else
                modNCR.sTargetDirectory = _sTargetDirectory;
            try
            {

                modNCR.ProcessNcrDirectory();
                _iNoOfFiles = modNCR.iNoOfFiles;
                _iNoOfFiles = modNCR.iNoOfFiles;
                _ErrorIndicator = modNCR.bErrorIndicator;
                _sErrorMessage = modNCR.sErrorMessage;
                if ((_ErrorIndicator == false) && (modNCR.sFilesNotProcessed.Trim().Length > 0))
                {
                    _ErrorIndicator = true;
                    _sErrorMessage = modNCR.sFilesNotProcessed.Trim();
                }
            }
            catch (Exception ex)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("ProcessNcr - An exception has occured while processing NCR Files. The Operation is aborted.");
                sb.AppendLine("Exception Details:");
                sb.AppendLine(ex.Message);
                _sErrorMessage = sb.ToString();
                _ErrorIndicator = true;
                return;
            }
            finally
            {
            }
        }

        public void ProcessNcrVision()
        {
            _iNoOfFiles = 0;
            modNcrVision.iBackwardNoOfDays = _iBackwardNoOfDays;
            modNcrVision.iForwardNoOfDays = _iForwardNoOfDays;
            modNcrVision.iNoOfFiles = 0;
            modNcrVision.dtFromDate = _dtFrDate;
            modNcrVision.dtToDate = _dtToDate;
            if (string.IsNullOrEmpty(_sSourceDirectory))
                modNcrVision.sSourceDirectory = (string)ConfigurationManager.AppSettings["NCR_Vision_Path"];
            else
                modNcrVision.sSourceDirectory = _sSourceDirectory;

            if (string.IsNullOrEmpty(_sTargetDirectory))
                modNcrVision.sTargetDirectory = (string)ConfigurationManager.AppSettings["NCR_Vision_TargetDir"];
            else
                modNcrVision.sTargetDirectory = _sTargetDirectory;
            try
            {

                modNcrVision.ProcessNcrVisionDirectory();
                _iNoOfFiles = modNcrVision.iNoOfFiles;
                _iNoOfFiles = modNcrVision.iNoOfFiles;
                _ErrorIndicator = modNcrVision.bErrorIndicator;
                _sErrorMessage = modNcrVision.sErrorMessage;
                if ((_ErrorIndicator == false) && (modNCR.sFilesNotProcessed.Trim().Length > 0))
                {
                    _ErrorIndicator = true;
                    _sErrorMessage = modNCR.sFilesNotProcessed.Trim();
                }
            }
            catch (Exception ex)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("ProcessNcrVision - An exception has occured while processing NCR Vision Files. The Operation is aborted.");
                sb.AppendLine("Exception Details:");
                sb.AppendLine(ex.Message);
                _sErrorMessage = sb.ToString();
                _ErrorIndicator = true;
                return;
            }
            finally
            {
            }
        }

        public void ProcessOth()
        {
            _iNoOfFiles = 0;
            modOTH.iBackwardNoOfDays = _iBackwardNoOfDays;
            modOTH.iForwardNoOfDays = _iForwardNoOfDays;
            modOTH.iNoOfFiles = 0;
            modOTH.dtFromDate = _dtFrDate;
            modOTH.dtToDate = _dtToDate;
            if (string.IsNullOrEmpty(_sSourceDirectory))
                modOTH.sSourceDirectory = (string)ConfigurationManager.AppSettings["OTH_Path"];
            else
                modOTH.sSourceDirectory = _sSourceDirectory;

            if (string.IsNullOrEmpty(_sTargetDirectory))
                modOTH.sTargetDirectory = (string)ConfigurationManager.AppSettings["OTH_TargetDir"];
            else
                modOTH.sTargetDirectory = _sTargetDirectory;
            try
            {

                modOTH.ProcessOthDirectory();
                _iNoOfFiles = modOTH.iNoOfFiles;
                _iNoOfFiles = modOTH.iNoOfFiles;
                _ErrorIndicator = modOTH.bErrorIndicator;
                _sErrorMessage = modOTH.sErrorMessage;
                if ((_ErrorIndicator == false) && (modOTH.sFilesNotProcessed.Trim().Length > 0))
                {
                    _ErrorIndicator = true;
                    _sErrorMessage = modOTH.sFilesNotProcessed.Trim();
                }
            }
            catch (Exception ex)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("ProcessOth - An exception has occured while processing OTH Files. The Operation is aborted.");
                sb.AppendLine("Exception Details:");
                sb.AppendLine(ex.Message);
                _sErrorMessage = sb.ToString();
                _ErrorIndicator = true;
                return;
            }
            finally
            {
            }

        }

        public void ProcessWcr()
        {
            _iNoOfFiles = 0;
            modWCR.iBackwardNoOfDays = _iBackwardNoOfDays;
            modWCR.iForwardNoOfDays = _iForwardNoOfDays;
            modWCR.iNoOfFiles = 0;
            modWCR.dtFromDate = _dtFrDate;
            modWCR.dtToDate = _dtToDate;
            if (string.IsNullOrEmpty(_sSourceDirectory))
                modWCR.sSourceDirectory = (string)ConfigurationManager.AppSettings["WCR_Path"];
            else
                modWCR.sSourceDirectory = _sSourceDirectory;

            if (string.IsNullOrEmpty(_sTargetDirectory))
                modWCR.sTargetDirectory = (string)ConfigurationManager.AppSettings["WCR_TargetDir"];
            else
                modWCR.sTargetDirectory = _sTargetDirectory;
            try
            {

                modWCR.ProcessWcrDirectory();
                _iNoOfFiles = modWCR.iNoOfFiles;
                _iNoOfFiles = modWCR.iNoOfFiles;
                _ErrorIndicator = modWCR.bErrorIndicator;
                _sErrorMessage = modWCR.sErrorMessage;
                if ((_ErrorIndicator == false) && (modWCR.sFilesNotProcessed.Trim().Length > 0))
                {
                    _ErrorIndicator = true;
                    _sErrorMessage = modWCR.sFilesNotProcessed.Trim();
                }
            }
            catch (Exception ex)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("ProcessOth - An exception has occured while processing OTH Files. The Operation is aborted.");
                sb.AppendLine("Exception Details:");
                sb.AppendLine(ex.Message);
                _sErrorMessage = sb.ToString();
                _ErrorIndicator = true;
                return;
            }
            finally
            {
            }

        }

    }
}
