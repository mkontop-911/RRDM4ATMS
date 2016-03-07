using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Security.Principal;
using System.IO.IsolatedStorage;
using System.IO;
//using log4net.Appender;

namespace RRDM4ATMsWin
{
     public enum LoggerMessageTypes
    {
        DEBUG_ = 1,
        ERROR_ = 2,
        FATAL_= 3,
        INFO_ = 4,
        WARNING_=5
    }
 

public class Log
{
	//static log4net.ILog mLog;
	//static Log()
	//{
  //      mLog = log4net.LogManager.GetLogger("GAS");
//	}
	/// <summary>
	/// WriteToLog
	/// </summary>
	/// <param name="pMessage"></param>
	/// <param name="pException"></param>
	/// <param name="pMessageType"></param>
	/// <remarks></remarks>
	public static void WriteToLog(string pMessage, Exception pException, LoggerMessageTypes pMessageType)
	{
		switch (pMessageType) {
			case LoggerMessageTypes.DEBUG_:
			//	if (mLog.IsDebugEnabled)
			//		mLog.Debug(pMessage, pException);
				break;
			case LoggerMessageTypes.ERROR_:
			//	if (mLog.IsErrorEnabled)
			//		mLog.Error(pMessage, pException);
				break;
			case LoggerMessageTypes.FATAL_:
			//	if (mLog.IsFatalEnabled)
			//		mLog.Fatal(pMessage, pException);
				break;
			case LoggerMessageTypes.INFO_:
			//	if (mLog.IsInfoEnabled)
				//	mLog.Info(pMessage, pException);
				break;
			case LoggerMessageTypes.WARNING_:
			//	if (mLog.IsWarnEnabled)
			//		mLog.Warn(pMessage, pException);
				break;
			default:
			//	if (mLog.IsErrorEnabled)
			//		mLog.Error(pMessage, pException);
				break;
		}
	}
	/// <summary>
	/// WriteToLog
	/// </summary>
	/// <param name="pMessage"></param>
	/// <param name="pException"></param>
	/// <remarks></remarks>
	public static void WriteToLog(string pMessage, Exception pException)
	{
	//	if (mLog.IsErrorEnabled)
	//		mLog.Error(pMessage, pException);
	}
	/// <summary>
	/// 'WriteErrorMessageToLog
	/// </summary>
	/// <param name="pMessage"></param>
	/// <param name="pFormName"></param>
	/// <param name="pMethodName"></param>
	/// <remarks></remarks>
	public static void WriteErrorMessageToLog(string pMessage, string pFormName = "", string pMethodName = "")
	{
		System.Text.StringBuilder lErrorStr = new System.Text.StringBuilder();

		if (!string.IsNullOrEmpty(pFormName)) {
			lErrorStr.Append("FORM: ");
			lErrorStr.Append(pFormName);
			lErrorStr.Append(Constants.vbNewLine);
		}

		if (!string.IsNullOrEmpty(pMethodName)) {
			lErrorStr.Append("METHOD: ");
			lErrorStr.Append(pMethodName);
			lErrorStr.Append(Constants.vbNewLine);
		}

		if (!string.IsNullOrEmpty(pMessage)) {
			lErrorStr.Append("MESSAGE: ");
			lErrorStr.Append(pMessage);
			lErrorStr.Append(Constants.vbNewLine);
		}


		lErrorStr.Append("USER: ");
		lErrorStr.Append(WindowsIdentity.GetCurrent().Name);
		lErrorStr.Append(Constants.vbNewLine);

		lErrorStr.Append("MACHINE NAME: ");
		lErrorStr.Append(Environment.MachineName);
		lErrorStr.Append(Constants.vbNewLine);

    // if (mLog.IsErrorEnabled)
      //   mLog.Error(lErrorStr.ToString());
	}
	/// <summary>
	/// 'ProcessException
	/// </summary>
	/// <param name="pException"></param>
	/// <param name="pFormName"></param>
	/// <param name="pMethodName"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public static string ProcessException(Exception pException, string pFormName = "", string pMethodName = "")
	{
		System.Text.StringBuilder lErrorStr = new System.Text.StringBuilder();

		if (!(pException is System.Threading.ThreadAbortException))
        {
            lErrorStr.Append(Constants.vbNewLine);

            if ((HttpContext.Current != null))
            {
                lErrorStr.Append("User Environment: OS - ");
                lErrorStr.Append(HttpContext.Current.Request.Browser.Platform);
                lErrorStr.Append("; Browser - ");
                lErrorStr.Append(HttpContext.Current.Request.Browser.Type);
                lErrorStr.Append(" (");
                lErrorStr.Append(HttpContext.Current.Request.Browser.Version);
                lErrorStr.Append(")");
                lErrorStr.Append(Constants.vbNewLine);

                lErrorStr.Append("URL: ");
                lErrorStr.Append(HttpContext.Current.Request.Url.AbsoluteUri);
                lErrorStr.Append(Constants.vbNewLine);
            }

            lErrorStr.Append("ERROR_MESSAGE: ");
            lErrorStr.Append(pException.Message);
            lErrorStr.Append(Constants.vbNewLine);

            if (!string.IsNullOrEmpty(pFormName))
            {
                lErrorStr.Append("FORM: ");
                lErrorStr.Append(pFormName);
                lErrorStr.Append(Constants.vbNewLine);
            }

            if (!string.IsNullOrEmpty(pMethodName))
            {
                lErrorStr.Append("METHOD: ");
                lErrorStr.Append(pMethodName);
                lErrorStr.Append(Constants.vbNewLine);
            }

            lErrorStr.Append("SOURCE: ");
            lErrorStr.Append(pException.Source);
            lErrorStr.Append(Constants.vbNewLine);

            lErrorStr.Append("TARGET_TYPE: ");
            lErrorStr.Append(pException.TargetSite.ReflectedType.Name);
            lErrorStr.Append(Constants.vbNewLine);

            lErrorStr.Append("TARGET_METHOD: ");
            lErrorStr.Append(pException.TargetSite.Name.ToString());
            lErrorStr.Append(Constants.vbNewLine);

            lErrorStr.Append("STACKTRACE: ");
            lErrorStr.Append(pException.StackTrace);
            lErrorStr.Append(Constants.vbNewLine);

            WriteErrorMessageToLog(lErrorStr.ToString());
		}

		return lErrorStr.ToString();
	}
}


}
