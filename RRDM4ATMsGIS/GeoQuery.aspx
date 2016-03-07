<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="GeoQuery.aspx.cs" Inherits="RRDM4ATMsGIS.GeoQuery" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        window.onload = function () {
            var div1 = document.getElementById("DivGrid");
            var div1_position = document.getElementById("DivGrid_Pos");
            var position1 = parseInt('<%=Request.Form["DivGrid_Pos"] %>');


            if (isNaN(position1)) {
                position1 = 0;
            }
            div1.scrollTop = position1;
            div1.onscroll = function () {
                div1_position.value = div1.scrollTop;
            };


        };
    </script>
    <style type="text/css">
        .auto-style1 {
            text-align: right;
            height: 15px;
            margin-right: 3px;
            width: 59px;
        }
        .auto-style2 {
            height: 15px;
        }
    </style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <asp:Panel ID="Panel1" CssClass="RRDM0" runat="server" Style="border: 1px solid #2377C0; position: absolute; left: 3px; top: 10px; padding: 2px; margin: 2px; height: 25px; width: 316px;">
        <asp:Label ID="lbAtmNo" runat="server" Style="vertical-align: middle; z-index: 1; left: 14px; top: 9px; position: absolute; height: 14px; width: 52px;">ATM No</asp:Label>
        <asp:TextBox ID="txAtmNo" runat="server" BackColor="#3477B2" ForeColor="#E4E4BB" Style="z-index: 1; left: 67px; top: 3px; position: absolute; width: 206px; height: 21px;"></asp:TextBox>
        <%-- ToDo: make hidden --%>
        <asp:TextBox ID="txAtmId" runat="server" Width="27px" Style="z-index: 1; left: 283px; top: 2px; position: absolute" Visible="False"></asp:TextBox>
    </asp:Panel>

    <%-- ATM Details Record --%>
    <asp:Panel ID="pnlDetails" CssClass="RRDM0" runat="server" Style="border: 1px solid #2377C0; position: absolute; left: 3px; top: 45px; padding: 2px; margin: 2px; height: 455px; width: 316px;">
        <table class="dtlTable">
            <tr class="dtlRow">
                <td class="auto-style1">Street</td>
                <td>
                    <asp:TextBox ID="tbAddress" CssClass="TextBoxCellTall" runat="server" TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">Locality</td>
                <td>
                    <asp:TextBox ID="tbTown" runat="server" CssClass="TextBoxCell" Height="15px"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">Postal Code</td>
                <td>
                    <asp:TextBox ID="tbPostalCode" runat="server" CssClass="TextBoxCell" Height="15px"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">District</td>
                <td>
                    <asp:TextBox ID="tbDistrict" runat="server" CssClass="TextBoxCell" Height="15px"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">Country</td>
                <td>
                    <asp:TextBox ID="tbCountry" runat="server" CssClass="TextBoxCell" Height="15px"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">&nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">Lat</td>
                <td>
                    <asp:TextBox ID="tbLat" runat="server" CssClass="TextBoxCell" Height="15px"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">Lon</td>
                <td>
                    <asp:TextBox ID="tbLon" runat="server" CssClass="TextBoxCell" Height="15px"></asp:TextBox>
                </td>
            </tr>
        </table>

        <asp:Button ID="btReload" runat="server" BorderColor="#2377C0" BorderStyle="Solid" BorderWidth="1px" OnClick="btReload_Click" Style="z-index: 1; left: 95px; top: 334px; position: absolute; width: 131px; margin: 1px; height: 28px;" Text="Reload the data" ToolTip="Reload this page to start again (changes will be lost)" />
        <asp:Button ID="btShow" runat="server" BorderColor="#2377C0" BorderStyle="Solid" BorderWidth="1px" OnClick="btShow_Click" Style="z-index: 1; left: 59px; top: 257px; position: absolute; width: 210px; margin: 1px; height: 28px;" Text="Show Lat/Lon on Map" ToolTip="Show Lat/Lon on the map" />
        <asp:Button ID="btFind" runat="server" BorderColor="#2377C0" BorderStyle="Solid" BorderWidth="1px" OnClick="btFind_Click" Style="z-index: 1; left: 57px; top: 296px; position: absolute; width: 210px; margin: 1px; height: 28px;" Text="Find Coordinates from Address" ToolTip="Use the address fields to locate the ATM on the map" />
        <asp:Button ID="btSave" runat="server" BorderColor="#2377C0" BorderStyle="Solid" BorderWidth="1px" OnClick="btSave_Click" Style="z-index: 1; left: 96px; top: 377px; position: absolute; width: 131px; margin: 1px; height: 45px;" Text="Save" ToolTip="Update the data in the database" />
        
        <div id="Div2" style="z-index: 1; left: 5px; top: 426px; position: absolute; width: 298px; height: 24px; background-color: #3477B2; vertical-align: middle; font-size: small; font-weight: bold; color: #E4E4BB; padding-top: 3px; padding-left: 8px;">
            <asp:Label ID="lblStatus" runat="server" CssClass="Label" Visible="False">
            </asp:Label>
        </div>

    </asp:Panel>

    <%-- Google Map Area --%>
    <asp:Panel ID="pnlMap" runat="server" Style="position: absolute; left: 330px; top: 1px; float: right; height: 505px; width: 1266px"
        BorderColor="#2377C0" BorderStyle="Solid" BorderWidth="2px">
        <artem:GoogleMap ID="GoogleMap1" runat="server" Height="100%" Width="100%" IsSensor="false"
            EnableMapTypeControl="False"
            EnableOverviewMapControl="False"
            EnableReverseGeocoding="False"
            EnableScaleControl="False"
            EnableScrollWheelZoom="True"
            EnableStreetViewControl="False"
            EnableZoomControl="True"
            EnablePanControl="True"
            MapType="Roadmap"
            Zoom="2"
            OnLoad="GoogleMap1_Load"
            OnPreRender="GoogleMap1_PreRender"
            Key="AIzaSyCECJJGPaqP4T8vkHiZT2FOL2iRdJzZSGc">
            <Center Latitude="0" Longitude="0"></Center>
        </artem:GoogleMap>
        <artem:GoogleMarkers ID="GoogleMarkers1" TargetControlID="GoogleMap1" runat="server" Enabled="true"
            OnDragEnd="MarkerDragEnd">
            <MarkerOptions Draggable="false"></MarkerOptions>
        </artem:GoogleMarkers>

    </asp:Panel>

    <%-- GeoCode Search Results Area --%>
    <input type="hidden" id="DivGrid_Pos" name="DivGrid_Pos" />
    <div id="DivGrid" class="RRDM0" style="border: 1px solid #2377C0; position: absolute; left: 2px; top: 513px; padding: 2px; margin: 2px; height: 116px; width: 1590px; overflow: scroll;">
        <asp:GridView ID="gvATMs" runat="server" AutoGenerateColumns="False" AutoGenerateSelectButton="True" BackColor="White" BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" CellPadding="4" DataKeyNames="Id" Font-Names="Arial" Font-Size="Small" OnDataBound="gvATMs_DataBound" ShowHeaderWhenEmpty="True" Style="left: 2px; top: 4px; position: absolute; width: 1570px" OnSelectedIndexChanged="gvATMs_SelectedIndexChanged">
            <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
            <HeaderStyle BackColor="#003399" Font-Bold="True" ForeColor="#CCCCFF" />
            <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" />
            <RowStyle BackColor="White" ForeColor="#003399" />
            <AlternatingRowStyle />
            <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
            <SortedAscendingCellStyle BackColor="#EDF6F6" />
            <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
            <SortedDescendingCellStyle BackColor="#D6DFDF" />
            <SortedDescendingHeaderStyle BackColor="#002876" />
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="True">
                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center" Width="80" Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="Lat" HeaderText="Latitude">
                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center" Width="150px" Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="Lon" HeaderText="Longitude">
                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center" Width="150px" Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="FormattedAddress" HeaderText="Address">
                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="380" Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="AddressType" HeaderText="Addr.Type">
                    <ItemStyle VerticalAlign="Middle" Width="350px" Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="LocationType" HeaderText="Loc.Type">
                    <ItemStyle VerticalAlign="Middle" Width="350px" Wrap="False" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
    </div>








</asp:Content>
