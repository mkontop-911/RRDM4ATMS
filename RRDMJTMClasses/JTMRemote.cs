using System;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace RRDMJTMClasses
{

    /// <summary> class JTMRemote 
    /// Provides access to resources on network.
    /// </summary>
    public class JTMRemote : IDisposable 
    {
        #region Members
        private string _jtmRemoteUNC;
        private string _jtmRemoteMachineName;
        public string JtmRemoteMachineName
        {
            get { return this._jtmRemoteMachineName; }
            set
            {
                this._jtmRemoteMachineName = value;
                this._jtmRemoteUNC = @"\\" + this._jtmRemoteMachineName;
            }
        }
        public string UserName { get; set; }
        public string Password { get; set; }
        #endregion

        #region Consts
        private const int RESOURCE_CONNECTED = 0x00000001;
        private const int RESOURCE_GLOBALNET = 0x00000002;
        private const int RESOURCE_REMEMBERED = 0x00000003;
        private const int RESOURCETYPE_ANY = 0x00000000;
        private const int RESOURCETYPE_DISK = 0x00000001;
        private const int RESOURCETYPE_PRINT = 0x00000002;
        private const int RESOURCEDISPLAYTYPE_GENERIC = 0x00000000;
        private const int RESOURCEDISPLAYTYPE_DOMAIN = 0x00000001;
        private const int RESOURCEDISPLAYTYPE_SERVER = 0x00000002;
        private const int RESOURCEDISPLAYTYPE_SHARE = 0x00000003;
        private const int RESOURCEDISPLAYTYPE_FILE = 0x00000004;
        private const int RESOURCEDISPLAYTYPE_GROUP = 0x00000005;
        private const int RESOURCEUSAGE_CONNECTABLE = 0x00000001;
        private const int RESOURCEUSAGE_CONTAINER = 0x00000002;
        private const int CONNECT_INTERACTIVE = 0x00000008;
        private const int CONNECT_PROMPT = 0x00000010;
        private const int CONNECT_REDIRECT = 0x00000080;
        private const int CONNECT_UPDATE_PROFILE = 0x00000001;
        private const int CONNECT_COMMANDLINE = 0x00000800;
        private const int CONNECT_CMD_SAVECRED = 0x00001000;
        private const int CONNECT_LOCALDRIVE = 0x00000100;
        #endregion

        #region Error Codes
        private const int NO_ERROR = 0;
        private const int ERROR_ACCESS_DENIED = 5;
        private const int ERROR_NETWORK_PATH_NOT_FOUND = 53;
        private const int ERROR_ALREADY_ASSIGNED = 85;
        private const int ERROR_BAD_DEVICE = 1200;
        private const int ERROR_BAD_NET_NAME = 67;
        private const int ERROR_BAD_PROVIDER = 1204;
        private const int ERROR_CANCELLED = 1223;
        private const int ERROR_EXTENDED_ERROR = 1208;
        private const int ERROR_INVALID_ADDRESS = 487;
        private const int ERROR_INVALID_PARAMETER = 87;
        private const int ERROR_INVALID_PASSWORD = 1216;
        private const int ERROR_MORE_DATA = 234;
        private const int ERROR_NO_MORE_ITEMS = 259;
        private const int ERROR_NO_NET_OR_BAD_PATH = 1203;
        private const int ERROR_NO_NETWORK = 1222;
        private const int ERROR_BAD_PROFILE = 1206;
        private const int ERROR_CANNOT_OPEN_PROFILE = 1205;
        private const int ERROR_DEVICE_IN_USE = 2404;
        private const int ERROR_NOT_CONNECTED = 2250;
        private const int ERROR_OPEN_FILES = 2401;
        #endregion 

        #region Error Classess
        private struct ErrorStruct
        {
            public int num;
            public string message;
            public ErrorStruct(int num, string message)
            {
                this.num = num;
                this.message = message;
            }
        }

        private static ErrorStruct[] ERROR_LIST = new ErrorStruct[] 
        {
            new ErrorStruct(ERROR_ACCESS_DENIED, "Error: Access Denied"), 
            new ErrorStruct(ERROR_NETWORK_PATH_NOT_FOUND, "Error: Network Path Not Found"),
            new ErrorStruct(ERROR_ALREADY_ASSIGNED, "Error: Already Assigned"), 
            new ErrorStruct(ERROR_BAD_DEVICE, "Error: Bad Device"), 
            new ErrorStruct(ERROR_BAD_NET_NAME, "Error: Bad Net Name"), 
            new ErrorStruct(ERROR_BAD_PROVIDER, "Error: Bad Provider"), 
            new ErrorStruct(ERROR_CANCELLED, "Error: Cancelled"), 
            new ErrorStruct(ERROR_EXTENDED_ERROR, "Error: Extended Error"), 
            new ErrorStruct(ERROR_INVALID_ADDRESS, "Error: Invalid Address"), 
            new ErrorStruct(ERROR_INVALID_PARAMETER, "Error: Invalid Parameter"), 
            new ErrorStruct(ERROR_INVALID_PASSWORD, "Error: Invalid Password"), 
            new ErrorStruct(ERROR_MORE_DATA, "Error: More Data"), 
            new ErrorStruct(ERROR_NO_MORE_ITEMS, "Error: No More Items"), 
            new ErrorStruct(ERROR_NO_NET_OR_BAD_PATH, "Error: No Net Or Bad Path"), 
            new ErrorStruct(ERROR_NO_NETWORK, "Error: No Network"), 
            new ErrorStruct(ERROR_BAD_PROFILE, "Error: Bad Profile"), 
            new ErrorStruct(ERROR_CANNOT_OPEN_PROFILE, "Error: Cannot Open Profile"), 
            new ErrorStruct(ERROR_DEVICE_IN_USE, "Error: Device In Use"), 
            new ErrorStruct(ERROR_EXTENDED_ERROR, "Error: Extended Error"), 
            new ErrorStruct(ERROR_NOT_CONNECTED, "Error: Not Connected"), 
            new ErrorStruct(ERROR_OPEN_FILES, "Error: Open Files"), 
        };

        private static string getErrorMessageFromErrorCode(int errNum)
        {
            foreach (ErrorStruct er in ERROR_LIST)
            {
                if (er.num == errNum) return er.message;
            }
            return "Error: Unknown, " + errNum;
        }
        #endregion

        #region DllImorts (PInvokes signatures)

        [DllImport("Mpr.dll")]
        private static extern int WNetUseConnection(
            IntPtr hwndOwner,
            NETRESOURCE lpNetResource,
            string lpPassword,
            string lpUserID,
            int dwFlags,
            string lpAccessName,
            string lpBufferSize,
            string lpResult
            );

        [DllImport("Mpr.dll")]
        private static extern int WNetCancelConnection2(
            string lpName,
            int dwFlags,
            bool fForce
            );

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public int dwScope = 0;
            public int dwType = 0;
            public int dwDisplayType = 0;
            public int dwUsage = 0;
            public string lpLocalName = "";
            public string lpRemoteName = "";
            public string lpComment = "";
            public string lpProvider = "";
        }
        #endregion

        /// <summary> JTMRemote EstablishConnection 
        /// Instantiates a new JTMRemote for the given machine mame, username and password
        /// </summary>
        /// <param name="remoteMachineName">
        /// e.g:ATMXP-01
        /// </param>
        /// <param name="userName">
        /// (format: Domain | Computername\Username)
        /// </param>
        /// <param name="password"></param>
        public static JTMRemote EstablishConnection(string remoteMachineName, string userName, string password)
        {
            // instantiates the private class
            return new JTMRemote(remoteMachineName, userName, password);
        }

        /// <summary> private JTMRemote constructor
        /// </summary>
        /// <param name="remoteMachineName">
        /// e.g:ATMXP-01
        /// </param>
        /// <param name="userName">
        /// (format: Domain | Computername\Username)
        /// </param>
        /// <param name="password"></param>
        private JTMRemote(string remoteMachineName, string userName, string password)
        {
            JtmRemoteMachineName = remoteMachineName;
            UserName = userName;
            Password = password;
            this.ConnectToRemoteMachine(this._jtmRemoteUNC, this.UserName, this.Password);
        }

        /// <summary> ConnectToRemoteMachine
        /// 
        /// </summary>
        /// <remarks>
        /// Calls WNetUseConnection. On error throws a Win32Exception
        /// </remarks>
        /// <param name="remoteName">
        /// format: \\machinename</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void ConnectToRemoteMachine(string remoteName, string username, string password)
        {
            int result;
            NETRESOURCE nr = new NETRESOURCE { dwType = RESOURCETYPE_DISK, lpRemoteName = remoteName };
            result = WNetUseConnection(IntPtr.Zero, nr, password, username, 0, null, null, null);
            if (result != NO_ERROR)
            {
                throw new Win32Exception(result);
            }
        }

        private void DisconnectFromRemote(string remoteUnc)
        {
            int result = WNetCancelConnection2(remoteUnc, CONNECT_UPDATE_PROFILE, false);
            if (result != NO_ERROR)
            {
                throw new Win32Exception(result);
            }
        }

        public void Dispose()
        {
            this.DisconnectFromRemote(this._jtmRemoteUNC);
        }
    }
    
}
