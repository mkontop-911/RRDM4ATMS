using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TWF_ProcessFiles
{
    public class ProcessFiles
    {
        private readonly IConfiguration _configuration;

        public ProcessFiles(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public enum FileTransferType
        {
            DBL,
            NCR,
            NCR_VISION,
            OTHER,
            WCR
        }
        public DateTime ReferenceDate { get; set; }
        public DateTime MaxReturnDate { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }

        public FileTransferType TrasferType { get; set; }

        public int NoOfFiles { get; set; }
        public string ErrorMessage { get; set; }
        public bool ErrorIndicator { get; set; }

        private static int _iNoOfFiles;
        private static int _iBackwardNoOfDays;
        private static int _iForwardNoOfDays;
        private static bool _ErrorIndicator;
        private static string _sErrorMessage;
        private static string _sFilesNotProcessed;

        public static string _sSourceDirectory;
        public static string _sTargetDirectory;
        public static DateTime _dtReferenceDate;
        public static DateTime _dtMaxReturnDate;

        public void filesProcessing()
        {
            _sSourceDirectory = this.SourceDirectory;
            _sTargetDirectory = this.TargetDirectory;
            _dtReferenceDate = this.ReferenceDate;
            _dtMaxReturnDate = this.MaxReturnDate;
            
            _iForwardNoOfDays = int.Parse(_configuration["ForwardNoOfDays"]);
            _iBackwardNoOfDays = int.Parse(_configuration["BackwardNoOfDays"]);

            _ErrorIndicator = false;
            _sErrorMessage = string.Empty;
            _sFilesNotProcessed = string.Empty;


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
            this.ErrorIndicator = _ErrorIndicator;
            this.NoOfFiles = _iNoOfFiles;
            this.MaxReturnDate = _dtMaxReturnDate;

        }
        private void ProcessDbl()
        {
            _iNoOfFiles = 0;
            ModDBL.BackwardNoOfDays = _iBackwardNoOfDays;
            ModDBL.ForwardNoOfDays = _iForwardNoOfDays;
            ModDBL.NoOfFiles = 0;
            ModDBL.ReferenceDate = _dtReferenceDate;
            ModDBL.MaxReturnDate = _dtMaxReturnDate;

            if (string.IsNullOrEmpty(_sSourceDirectory))
                ModDBL.SourceDirectory = _configuration["DBL_Path"];
            else
                ModDBL.SourceDirectory = _sSourceDirectory;

            if (string.IsNullOrEmpty(_sTargetDirectory))
                ModDBL.TargetDirectory = _configuration["DBL_TargetDir"];
            else
                ModDBL.TargetDirectory = _sTargetDirectory;

            try
            {
                ModDBL.ProcessDblDirectory(_configuration);

                _iNoOfFiles = ModDBL.NoOfFiles;
                _ErrorIndicator = ModDBL.ErrorIndicator;
                _sErrorMessage = ModDBL.ErrorMessage;
                _dtMaxReturnDate = ModDBL.MaxReturnDate;

                if ((_ErrorIndicator == false) && (ModDBL.FilesNotProcessed != null))
                {
                    if (ModDBL.FilesNotProcessed.Trim().Length > 0)
                    {
                        _ErrorIndicator = true;
                        _sErrorMessage = ModDBL.FilesNotProcessed.Trim();
                    }
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
            ModNCR.BackwardNoOfDays = _iBackwardNoOfDays;
            ModNCR.ForwardNoOfDays = _iForwardNoOfDays;
            ModNCR.NoOfFiles = 0;
            ModNCR.ReferenceDate = _dtReferenceDate;
            ModNCR.MaxReturnDate = _dtMaxReturnDate;
            if (string.IsNullOrEmpty(_sSourceDirectory))
                ModNCR.SourceDirectory = _configuration["NCR_Path"];
            else
                ModNCR.SourceDirectory = _sSourceDirectory;

            if (string.IsNullOrEmpty(_sTargetDirectory))
                ModNCR.TargetDirectory = _configuration["NCR_TargetDir"];
            else
                ModNCR.TargetDirectory = _sTargetDirectory;
            try
            {

                ModNCR.ProcessNcrDirectory(_configuration);

                _iNoOfFiles = ModNCR.NoOfFiles;
                _ErrorIndicator = ModNCR.ErrorIndicator;
                _sErrorMessage = ModNCR.ErrorMessage;
                _dtMaxReturnDate = ModNCR.MaxReturnDate;

                if ((_ErrorIndicator == false) && (ModNCR.FilesNotProcessed != null))
                {
                    if (ModNCR.FilesNotProcessed.Trim().Length > 0)
                    {
                        _ErrorIndicator = true;
                        _sErrorMessage = ModNCR.FilesNotProcessed.Trim();
                    }
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
            ModNcrVision.BackwardNoOfDays = _iBackwardNoOfDays;
            ModNcrVision.ForwardNoOfDays = _iForwardNoOfDays;
            ModNcrVision.NoOfFiles = 0;
            ModNcrVision.ReferenceDate = _dtReferenceDate;
            ModNcrVision.MaxReturnDate = _dtMaxReturnDate;
            if (string.IsNullOrEmpty(_sSourceDirectory))
                ModNcrVision.SourceDirectory = _configuration["NCR_Vision_Path"];
            else
                ModNcrVision.SourceDirectory = _sSourceDirectory;

            if (string.IsNullOrEmpty(_sTargetDirectory))
                ModNcrVision.TargetDirectory = _configuration["NCR_Vision_TargetDir"];
            else
                ModNcrVision.TargetDirectory = _sTargetDirectory;
            try
            {

                ModNcrVision.ProcessNcrVisionDirectory(_configuration);
                _iNoOfFiles = ModNcrVision.NoOfFiles;
                _ErrorIndicator = ModNcrVision.ErrorIndicator;
                _sErrorMessage = ModNcrVision.ErrorMessage;
                _dtMaxReturnDate = ModNcrVision.MaxReturnDate;


                if ((_ErrorIndicator == false) && (ModNcrVision.FilesNotProcessed != null))
                {
                    if (ModNcrVision.FilesNotProcessed.Trim().Length > 0)
                    {
                        _ErrorIndicator = true;
                        _sErrorMessage = ModNcrVision.FilesNotProcessed.Trim();
                    }
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
            ModOTH.BackwardNoOfDays = _iBackwardNoOfDays;
            ModOTH.ForwardNoOfDays = _iForwardNoOfDays;
            ModOTH.NoOfFiles = 0;
            ModOTH.ReferenceDate = _dtReferenceDate;
            ModOTH.MaxReturnDate = _dtMaxReturnDate;

            if (string.IsNullOrEmpty(_sSourceDirectory))
                ModOTH.SourceDirectory = _configuration["OTH_Path"];
            else
                ModOTH.SourceDirectory = _sSourceDirectory;

            if (string.IsNullOrEmpty(_sTargetDirectory))
                ModOTH.TargetDirectory = _configuration["OTH_TargetDir"];
            else
                ModOTH.TargetDirectory = _sTargetDirectory;
            try
            {

                ModOTH.ProcessOthDirectory(_configuration);

                _iNoOfFiles = ModOTH.NoOfFiles;
                _ErrorIndicator = ModOTH.ErrorIndicator;
                _sErrorMessage = ModOTH.ErrorMessage;
                _dtMaxReturnDate = ModOTH.MaxReturnDate;

                if ((_ErrorIndicator == false) && (ModOTH.FilesNotProcessed != null))
                {
                    if (ModOTH.FilesNotProcessed.Trim().Length > 0)
                    {
                        _ErrorIndicator = true;
                        _sErrorMessage = ModOTH.FilesNotProcessed.Trim();
                    }
                }
                _dtMaxReturnDate = ModOTH.MaxReturnDate;
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
            ModWCR.BackwardNoOfDays = _iBackwardNoOfDays;
            ModWCR.ForwardNoOfDays = _iForwardNoOfDays;
            ModWCR.NoOfFiles = 0;
            ModWCR.ReferenceDate = _dtReferenceDate;
            ModWCR.MaxReturnDate = _dtMaxReturnDate;
            if (string.IsNullOrEmpty(_sSourceDirectory))
                ModWCR.SourceDirectory = _configuration["WCR_Path"];
            else
                ModWCR.SourceDirectory = _sSourceDirectory;

            if (string.IsNullOrEmpty(_sTargetDirectory))
                ModWCR.TargetDirectory = _configuration["WCR_TargetDir"];
            else
                ModWCR.TargetDirectory = _sTargetDirectory;
            try
            {

                ModWCR.ProcessWcrDirectory(_configuration);

                _iNoOfFiles = ModWCR.NoOfFiles;
                _ErrorIndicator = ModWCR.ErrorIndicator;
                _sErrorMessage = ModWCR.ErrorMessage;
                _dtMaxReturnDate = ModWCR.MaxReturnDate;

                if ((_ErrorIndicator == false) && (ModWCR.FilesNotProcessed != null))
                {
                    if (ModWCR.FilesNotProcessed.Trim().Length > 0)
                    {
                        _ErrorIndicator = true;
                        _sErrorMessage = ModWCR.FilesNotProcessed.Trim();
                    }
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
