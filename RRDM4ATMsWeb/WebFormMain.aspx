<%@ Page Title="Main Menu" Language="C#" MasterPageFile="~/RRDMMasters/RRDM4ATMs.Master" AutoEventWireup="true" CodeBehind="WebFormMain.aspx.cs" Inherits="RRDM4ATMsWeb.WebFormMain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <title></title>
    <style type="text/css">
        .auto-style10 {
            position: absolute;
            top: 13px;
            left: 772px;
            z-index: 1;
            width: 142px;
            height: 39px;
        }
        .auto-style11 {
            position: absolute;
            top: 49px;
            left: 427px;
            z-index: 1;
            height: 61px;
            width: 120px;
        }
        .auto-style12 {
            position: absolute;
            top: 133px;
            left: 84px;
            z-index: 1;
            width: 120px;
            height: 61px;
            right: 820px;
        }
        .auto-style13 {
            position: absolute;
            top: 49px;
            left: 84px;
            z-index: 1;
            width: 120px;
            height: 61px;
        }
        .auto-style14 {
            position: absolute;
            top: 132px;
            left: 427px;
            z-index: 1;
            width: 120px;
            height: 61px;
        }
        .auto-style15 {
            position: absolute;
            top: 215px;
            left: 428px;
            z-index: 1;
            width: 120px;
            height: 61px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
       <div>
    
        <asp:Button ID="Button2" runat="server" CssClass="auto-style10" OnClick="Button2_Click" Text="Sign out" />
    
           <asp:Panel ID="Panel1" runat="server" BackColor="#F0F0F0" Height="677px">
               <asp:Button ID="Button3" runat="server" Text="ATMs Maintenance" CssClass="auto-style11" />
               <asp:Button ID="Button5" runat="server" CssClass="auto-style13" OnClick="Button5_Click" Text="Replenishment" />
               <asp:Button ID="Button6" runat="server" CssClass="auto-style14" Text="Captured Cards" />
               <asp:Button ID="Button7" runat="server" CssClass="auto-style15" Text="E-Journal Drilling " />
               <asp:Button ID="Button4" runat="server" CssClass="auto-style12" OnClick="Button4_Click" Text="My ATMs operation" />
           </asp:Panel>
    
    </div>
        </asp:Content>
