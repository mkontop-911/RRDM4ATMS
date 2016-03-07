<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="RRDM4ATMsGIS.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        #TextArea1 {
            z-index: 1;
            left: 150px;
            top: 328px;
            position: absolute;
            width: 468px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <div style="height: 586px; left:15px">

        <asp:Label ID="Label1" runat="server" Text="Enter the Group Number and press GO" style="top: 245px; position: absolute; left: 149px; bottom: 394px;"></asp:Label>
        <asp:Button ID="btAddr" runat="server" style="z-index: 1; left: 586px; top: 404px; position: absolute; height: 31px; width: 53px; " OnClick="btAddr_Click" Text="GO!" />
        <asp:TextBox ID="tbGroupNo" runat="server" style="z-index: 1; left: 440px; top: 238px; position: absolute; height: 21px; width: 84px"></asp:TextBox>
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Main.aspx" Style="top: 189px; position: absolute; left: 148px; right: 623px;">ATM Locations Demo (ALL ATMs)</asp:HyperLink>

        <asp:Button ID="btGroup" runat="server" style="z-index: 1; left: 584px; top: 236px; position: absolute; height: 31px; width: 53px; " OnClick="btGroup_Click" Text="GO!" />

        <asp:Label ID="Label2" runat="server" Text="To Update an ATM's coordinates enter the ATM SeqNo and press GO   :" style="top: 399px; position: absolute; left: 148px; height: 42px; width: 265px;"></asp:Label>
        <asp:TextBox ID="tbATMID" runat="server" style="z-index: 1; left: 438px; top: 406px; position: absolute; height: 21px; width: 84px"></asp:TextBox>
        <hr style="z-index: 1; left: -32px; top: 317px; position: absolute; height: 2px; width: 1024px" />
        <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Names="Adobe Garamond Pro" Font-Size="XX-Large" style="z-index: 1; left: 151px; top: 138px; position: absolute; width: 487px; height: 35px" Text="View ATM Locations on Map"></asp:Label>
        <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Names="Adobe Garamond Pro" Font-Size="XX-Large" style="z-index: 1; left: 151px; top: 355px; position: absolute; width: 487px; height: 35px" Text="Update Coordinates on Map"></asp:Label>
    </div>

</asp:Content>
