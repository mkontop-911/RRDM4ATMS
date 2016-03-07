<%@ Page Title="" Language="C#" MasterPageFile="~/RRDMMasters/RRDM4ATMs.Master" AutoEventWireup="true" CodeBehind="WebForm152a.aspx.cs" Inherits="RRDM4ATMsWeb.WebForm152a" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 1024px;
            height: 686px;
            position: absolute;
            top: 0px;
            left: 0px;
            z-index: 1;
        }
        .auto-style2 {
            position: absolute;
            top: 50px;
            left: 300px;
            z-index: 1;
            width: 352px;
        }
        .auto-style3 {
            width: 519px;
            height: 331px;
            position: absolute;
            top: 155px;
            left: 412px;
            z-index: 1;
        }
        .auto-style4 {
            position: absolute;
            top: 29px;
            left: 21px;
            z-index: 1;
            height: 22px;
            right: 351px;
        }
        .auto-style5 {
            position: absolute;
            top: 30px;
            left: 291px;
            z-index: 1;
        }
        .auto-style6 {
            position: absolute;
            top: 64px;
            left: 22px;
            z-index: 1;
            width: 139px;
        }
        .auto-style7 {
            position: absolute;
            top: 65px;
            left: 291px;
            z-index: 1;
        }
        .auto-style8 {
            position: absolute;
            top: 102px;
            left: 23px;
            z-index: 1;
            width: 122px;
        }
        .auto-style9 {
            position: absolute;
            top: 121px;
            left: 447px;
            z-index: 1;
            width: 359px;
            height: 23px;
        }
        .auto-style10 {
            position: absolute;
            top: 160px;
            left: 22px;
            z-index: 1;
            width: 457px;
            height: 54px;
            bottom: 111px;
        }
        .auto-style11 {
            position: absolute;
            top: 279px;
            left: 372px;
            z-index: 1;
            width: 64px;
            height: 31px;
        }
        .auto-style12 {
            position: absolute;
            top: 140px;
            left: 25px;
            z-index: 1;
        }
        .auto-style13 {
            position: absolute;
            top: 580px;
            left: 138px;
            z-index: 1;
            width: 251px;
            height: 57px;
        }
        .auto-style14 {
            position: absolute;
            top: 553px;
            left: 146px;
            z-index: 1;
        }
        .auto-style15 {
            position: absolute;
            top: 512px;
            left: 788px;
            z-index: 1;
            width: 59px;
        }
        .auto-style16 {
            position: absolute;
            top: 513px;
            left: 708px;
            z-index: 1;
            width: 57px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:Panel ID="Panel1" runat="server" BackColor="#F0F0F0" CssClass="auto-style1">
        <asp:Label ID="labelDescription" runat="server" CssClass="auto-style2" Text="DESCRIPTION" Font-Bold="True" Font-Size="X-Large" ForeColor="#000099"></asp:Label>
        <asp:Panel ID="Panel2" runat="server" BackColor="White" CssClass="auto-style3" ForeColor="Black">
            <asp:CheckBox ID="checkBoxNoChips" runat="server" CssClass="auto-style4" Text="No Microchips" />
            <asp:CheckBox ID="checkBoxNoSuspCards" runat="server" CssClass="auto-style5" Text="No Suspicious captured cards" />
            <asp:CheckBox ID="checkBoxNoCameras" runat="server" CssClass="auto-style6" Text="No Cameras" />
            <asp:CheckBox ID="checkBoxNoGlue" runat="server" CssClass="auto-style7" Text="No Glue remains" />
            <asp:CheckBox ID="checkBoxNoOtherSusp" runat="server" CssClass="auto-style8" Text="No Other" />
            <asp:TextBox ID="TextBox1" runat="server" CssClass="auto-style10" MaxLength="500" TextMode="MultiLine"></asp:TextBox>
            <asp:Button ID="ButtonUpdate" runat="server" BackColor="White" BorderColor="#0066FF" CssClass="auto-style11" Font-Bold="True" ForeColor="#0066FF" OnClick="ButtonUpdate_Click" Text="Update" />
            <asp:Label ID="Label2" runat="server" CssClass="auto-style12" Text="Insert Description if any YES"></asp:Label>
        </asp:Panel>
        <asp:Label ID="Label1" runat="server" CssClass="auto-style9" Font-Bold="True" Font-Size="Large" ForeColor="#0066FF" Text="ATM PHYSICAL INSPECTION"></asp:Label>
        <asp:TextBox ID="txtMessage" runat="server" CssClass="auto-style13" ReadOnly="True" TextMode="MultiLine"></asp:TextBox>
        <asp:Label ID="Label3" runat="server" CssClass="auto-style14" Font-Bold="True" Font-Size="Large" ForeColor="#FF0066" Text="Guidance "></asp:Label>
        <asp:Button ID="ButtonNext" runat="server" BorderColor="Blue" CssClass="auto-style15" Font-Bold="True" ForeColor="Blue" OnClick="ButtonNext_Click" Text="Next" />
        <asp:Button ID="ButtonBack" runat="server" BorderColor="Blue" CssClass="auto-style16" Font-Bold="True" ForeColor="Blue" OnClick="ButtonBack_Click" Text="Back" />
    </asp:Panel>
</asp:Content>
