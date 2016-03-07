<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RRDM4ATMsWeb.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <title></title>
    <style type="text/css">
        .auto-style1 {
            margin-bottom: 151px;
        }
        .auto-style2 {
            position: absolute;
            top: 128px;
            left: 87px;
            z-index: 1;
        }
        .auto-style3 {
            position: absolute;
            top: 192px;
            left: 90px;
            z-index: 1;
        }
        .auto-style4 {
            position: absolute;
            top: 244px;
            left: 93px;
            z-index: 1;
        }
        .auto-style5 {
            position: absolute;
            top: 237px;
            left: 189px;
            z-index: 1;
            width: 140px;
            height: 21px;
            right: 231px;
            bottom: 155px;
        }
        .auto-style6 {
            position: absolute;
            top: 192px;
            left: 193px;
            z-index: 1;
            width: 142px;
            height: 28px;
        }
        .auto-style7 {
            position: absolute;
            top: 349px;
            left: 461px;
            z-index: 1;
            width: 87px;
            height: 37px;
        }
        .auto-style8 {
            position: absolute;
            top: 62px;
            left: 89px;
            z-index: 1;
            font-size: xx-large;
        }
        .auto-style9 {
            position: absolute;
            top: 371px;
            left: 38px;
            z-index: 1;
            width: 416px;
            height: 26px;
            font-size: medium;
            right: 118px;
        }
        .auto-style11 {
            position: absolute;
            top: 248px;
            left: 350px;
            z-index: 1;
        }
        .auto-style12 {
            position: absolute;
            top: 281px;
            left: 347px;
            z-index: 1;
        }
        .auto-style13 {
            z-index: 1;
            position: relative;
            width: 604px;
            height: 485px;
            left: 13px;
            top: 45px;
            margin-bottom: 151px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div>
    
        <asp:Panel ID="Panel1" runat="server" BackColor="White" CssClass="auto-style13" ValidateRequestMode="Enabled">
            <strong>
            <asp:Label ID="Label1" runat="server" CssClass="auto-style2" Text="Please Login To The System"></asp:Label>
            </strong>
            <asp:Label ID="Label2" runat="server" CssClass="auto-style3" Text="User Name"></asp:Label>
            <asp:Label ID="Label3" runat="server" CssClass="auto-style4" Text="Password"></asp:Label>
            <asp:TextBox ID="TextBoxPassword" runat="server" CssClass="auto-style5" BackColor="White" ForeColor="#996633"></asp:TextBox>
            <asp:DropDownList ID="DropDownListUsers" runat="server" CssClass="auto-style6">
                <asp:ListItem Value="1005">1005</asp:ListItem>
                <asp:ListItem Value="487116">487116</asp:ListItem>
                <asp:ListItem>Blank</asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="Button1" runat="server" CssClass="auto-style7" OnClick="Button1_Click" Text="Login" ValidationGroup="AllValidations" style="z-index: 2" />
            <strong>
            <asp:Label ID="Label4" runat="server" CssClass="auto-style8" Text="Login " style="position: relative"></asp:Label>
            <asp:Label ID="MessageBox" runat="server" BackColor="White" CssClass="auto-style9" ForeColor="Black" Text="MessageBox"></asp:Label>
            </strong>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBoxPassword" CssClass="auto-style11" ErrorMessage="Please Enter Password" ForeColor="Red" ValidationGroup="AllValidations"></asp:RequiredFieldValidator>
            +<asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="TextBoxPassword" CssClass="auto-style12" Display="Dynamic" ErrorMessage="CustomValidator" ForeColor="#FF0066" OnServerValidate="CustomValidator1_ServerValidate" ValidationGroup="AllValidations "></asp:CustomValidator>
        </asp:Panel>
    
    </div>
</asp:Content>
