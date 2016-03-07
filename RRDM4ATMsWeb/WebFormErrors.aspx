<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebFormErrors.aspx.cs" Inherits="WebApplication52_Alex.WebFormErrors" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            font-size: xx-large;
            color: #FF3300;
        }
        .auto-style2 {
            width: 857px;
            height: 537px;
            position: absolute;
            top: 118px;
            left: 10px;
            z-index: 1;
        }
        .auto-style3 {
            position: absolute;
            top: 262px;
            left: 6px;
            z-index: 1;
            font-size: medium;
        }
        .auto-style4 {
            position: absolute;
            top: 472px;
            left: 678px;
            z-index: 1;
            width: 114px;
            height: 39px;
            color: #FFFFFF;
            background-color: #0000FF;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <strong>
        <asp:Label ID="Label1" runat="server" CssClass="auto-style1" Text="ERROR MESSAGE FROM RRDM FOR ATMS"></asp:Label>
        </strong>
    
    </div>
        <asp:Panel ID="Panel1" runat="server" CssClass="auto-style2" Width="857px">
            <asp:Label ID="ErrMessageBody" runat="server" Text="Message Body"></asp:Label>
            <strong>
            <asp:Label ID="Label2" runat="server" CssClass="auto-style3" Text="Instructions : Please Send email to rrdmsupport@rrdmsolutions.com"></asp:Label>
            <asp:Button ID="Button1" runat="server" CssClass="auto-style4" OnClick="Button1_Click" Text="Go to Main Menu" />
            </strong>
        </asp:Panel>
    </form>
</body>
</html>
