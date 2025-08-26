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

        <asp:Label ID="Label1" runat="server" Text="Enter the Group Number and the User Id and press GO" style="top: 225px; position: absolute; left: 155px; bottom: 414px;"></asp:Label>
        <asp:Label ID="Label2" runat="server" Text="To Update an ATM's coordinates enter the ATM SeqNo and press GO   :" style="top: 488px; position: absolute; left: 145px; height: 42px; width: 265px;"></asp:Label>
        <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Names="Adobe Garamond Pro" Font-Size="XX-Large" style="z-index: 1; left: 151px; top: 35px; position: absolute; width: 487px; height: 35px" Text="View ATM Locations on Map"></asp:Label>
        <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Names="Adobe Garamond Pro" Font-Size="X-Large" style="z-index: 1; left: 149px; top: 174px; position: absolute; width: 487px; height: 35px" Text="2.  View ATMs in a Group"></asp:Label>
        <asp:Label ID="Label5" runat="server" Text="User Id" style="top: 309px; position: absolute; left: 150px; bottom: 330px;"></asp:Label>
        <asp:Label ID="Label6" runat="server" Text="Group No" style="top: 260px; position: absolute; left: 154px; bottom: 379px;"></asp:Label>
        <asp:Label ID="Label7" runat="server" Font-Bold="True" Font-Names="Adobe Garamond Pro" Font-Size="X-Large" style="z-index: 1; left: 146px; top: 437px; position: absolute; width: 487px; height: 35px" Text="3.  Update Coordinates on Map"></asp:Label>
        <asp:Label ID="Label8" runat="server" Text="Group Description" style="top: 359px; position: absolute; left: 153px; bottom: 280px;"></asp:Label>

        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Main.aspx" Style="top: 104px; position: absolute; left: 148px; right: 448px;" Font-Bold="True" Font-Names="Adobe Garamond Pro" Font-Size="X-Large">1.  ATM Locations Demo (ALL ATMs)</asp:HyperLink>

        <asp:TextBox ID="tbGroupNo" runat="server" style="z-index: 1; left: 278px; top: 254px; position: absolute; height: 31px; width: 84px"></asp:TextBox>
        <asp:TextBox ID="tbUserId" runat="server" style="z-index: 1; left: 277px; top: 306px; position: absolute; height: 27px; width: 193px; margin-bottom: 0px;"></asp:TextBox>
        <asp:TextBox ID="tbGroupDesc" runat="server" style="z-index: 1; left: 276px; top: 353px; position: absolute; height: 27px; width: 286px; margin-bottom: 0px; margin-top: 0px;"></asp:TextBox>
        <asp:Button ID="btGroup" runat="server" style="z-index: 1; left: 604px; top: 354px; position: absolute; height: 31px; width: 53px; " OnClick="btGroup_Click" Text="GO!" />

        <asp:TextBox ID="tbATMID" runat="server" style="z-index: 1; left: 450px; top: 496px; position: absolute; height: 21px; width: 84px"></asp:TextBox>
        <asp:Button ID="btAddr" runat="server" style="z-index: 1; left: 598px; top: 495px; position: absolute; height: 31px; width: 53px; " OnClick="btAddr_Click" Text="GO!" />

        <hr style="z-index: 1; left: -45px; top: 403px; position: absolute; height: 2px; width: 1024px" />

    </div>

</asp:Content>


