<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="RRDM4ATMsGIS._Main" %>

<%@ MasterType VirtualPath="~/SiteMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<%--    <script type="text/javascript">
        window.onload = function () {
            var div1 = document.getElementById("DivGrid");
            var div1_position = document.getElementById("DivGrid_Pos");
            var position1 = parseInt('<%=Request.Form["DivGrid_Pos"] %>');

            var div2 = document.getElementById("DivDistricts");
            var div2_position = document.getElementById("DivDistricts_Pos");
            var position2 = parseInt('<%=Request.Form["DivDistricts_Pos"] %>');

                if (isNaN(position1)) {
                    position1 = 0;
                }
                div1.scrollTop = position1;
                div1.onscroll = function () {
                    div1_position.value = div1.scrollTop;
                };

                if (isNaN(position2)) {
                    position2 = 0;
                }
                div2.scrollTop = position2;
                div2.onscroll = function () {
                    div2_position.value = div2.scrollTop;
                };
        };
    </script>--%>

    <style type="text/css">
        .auto-style1 {
            text-align: right;
            height: 15px;
            margin-right: 3px;
            width: 62px;
        }
    </style>

</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:ObjectDataSource ID="odsDistricts" runat="server" SelectMethod="GetListofDistricts" TypeName="RRDM4ATMs.AtmDataAccess"></asp:ObjectDataSource>

    <%-- District selection --%>
    <div id="Div1" class="RRDM0" runat="server" style="z-index: 1; left: 0px; top: 8px; position: absolute; height: 39px; width: 326px; float: left;">
        <asp:CheckBox ID="cbAllRecords" runat="server" OnCheckedChanged="cbAllRecords_StatusChanged" AutoPostBack="True" />

        <asp:Label ID="Label1" runat="server" Style="z-index: 1; left: 30px; top: 1px; position: absolute; height: 35px; width: 247px; float: right">
            Check to display all records <br/> or select items from the list below... 
        </asp:Label>
    </div>

    <input type="hidden" id="DivDistricts_Pos" name="DivDistricts_Pos" />
    <div id="DivDistricts" class="RRDM0" style="position: absolute; left: 4px; top: 49px; padding: 2px; margin: 2px; border-style: solid; border-width: 1px; border-color: #2377C0; height: 100px; overflow: scroll; width: 316px;">
        <asp:CheckBoxList ID="cblDistricts" runat="server" Height="100%" Width="100%" DataSourceID="odsDistricts" DataTextField="District" DataValueField="District" OnSelectedIndexChanged="cblDistricts_SelectedIndexChanged" AutoPostBack="true">
        </asp:CheckBoxList>
    </div>

    <%-- GridView Area --%>
    <input type="hidden" id="DivGrid_Pos" name="DivGrid_Pos" />
    <div id="DivGrid" class="RRDM0" style="border: 1px solid #2377C0; position: absolute; left: 4px; top: 158px; padding: 2px; margin: 2px; height: 271px; width: 316px; overflow: scroll; bottom: 219px;">
        <asp:GridView ID="gvATMs" runat="server" AutoGenerateColumns="False" Height="304px" CellPadding="4" Width="312px"
            DataKeyNames="ATMId"
            OnDataBound="gvATMs_DataBound"
            OnSelectedIndexChanged="gvATMs_SelectedIndexChanged"
            AutoGenerateSelectButton="True"
            EmptyDataText="   No Records to display!"
            BorderStyle="None" Font-Names="Arial" Font-Size="Small"
            BackColor="White" BorderColor="#3366CC" BorderWidth="1px">
            <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
            <HeaderStyle BackColor="#003399" Font-Bold="True" ForeColor="#CCCCFF" />
            <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" />
            <RowStyle BackColor="White" ForeColor="#003399" />
            <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
            <SortedAscendingCellStyle BackColor="#EDF6F6" />
            <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
            <SortedDescendingCellStyle BackColor="#D6DFDF" />
            <SortedDescendingHeaderStyle BackColor="#002876" />
            <Columns>
                <asp:BoundField DataField="ATMId" HeaderText="ID" ReadOnly="True">
                    <ItemStyle VerticalAlign="Middle" Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="ATMNumber" HeaderText="ATM No">
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="ATMColorId" HeaderText="Color Id" ConvertEmptyStringToNull="False">
                    <ItemStyle VerticalAlign="Middle" Wrap="False" Width="180px" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
    </div>

    <%-- Details Panel --%>
    <asp:Panel ID="pnlDetails" CssClass="RRDM0" runat="server" Style="border: 1px solid #2377C0; position: absolute; left: 4px; top: 440px; padding: 2px; margin: 2px; height: 208px; width: 316px;">
        <table class="dtlTable">
            <tr class="dtlRow">
                <td class="auto-style1">ATM No</td>
                <td>
                    <asp:TextBox CssClass="TextBoxCell" ID="tbAtmNo" runat="server" Height="15px" Width="50px"></asp:TextBox>
                </td>
                <td class="FldLabel">Code</td>
                <td>
                    <asp:TextBox ID="tbColorId" CssClass="TextBoxCell" runat="server" Height="15px" Width="100px"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">Street</td>
                <td colspan="3">
                    <asp:TextBox ID="tbStreet" runat="server" CssClass="TextBoxCellTall2" TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">Locality</td>
                <td colspan="3" style="height: 20px">
                    <asp:TextBox ID="tbTown" CssClass="TextBoxCell" runat="server" Height="15px"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">Postal Code</td>
                <td colspan="3" style="height: 20px">
                    <asp:TextBox ID="tbPostalCode" CssClass="TextBoxCell" runat="server" Height="15px"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">District</td>
                <td colspan="3">
                    <asp:TextBox ID="tbDistrict" CssClass="TextBoxCell" runat="server" Height="15px"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">Country</td>
                <td colspan="3">
                    <asp:TextBox ID="tbCountry" CssClass="TextBoxCell" runat="server" Height="15px"></asp:TextBox>
                </td>
            </tr>
            <tr class="dtlRow">
                <td class="auto-style1">Descr.</td>
                <td colspan="3">
                    <asp:TextBox ID="tbGroupDesc" CssClass="TextBoxCell" runat="server" Height="15px"></asp:TextBox>
                </td>
            </tr>
        </table>

    </asp:Panel>


    <%-- Google Map Area --%>
    <asp:Panel ID="pnlMap" runat="server" Style="z-index: 1; position: absolute; left: 330px; top: 1px; float: right; height: 653px; width: 1266px"
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
            OnLoad="GoogleMap1_Load" OnPreRender="GoogleMap1_PreRender" 
            Key="AIzaSyCECJJGPaqP4T8vkHiZT2FOL2iRdJzZSGc">
            <Center Latitude="0" Longitude="0"></Center>
        </artem:GoogleMap>
        <artem:GoogleMarkers ID="GoogleMarkers1" TargetControlID="GoogleMap1" runat="server" Enabled="true">
            <MarkerOptions Draggable="false" ></MarkerOptions>
        </artem:GoogleMarkers>

    </asp:Panel>

</asp:Content>


