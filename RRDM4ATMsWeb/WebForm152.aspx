<%@ Page Title="Replenishment WFlow" Language="C#" MasterPageFile="~/RRDMMasters/RRDM4ATMs.Master" AutoEventWireup="true" CodeBehind="WebForm152.aspx.cs" Inherits="RRDM4ATMsWeb.WebForm152" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 1024px;
            height: 619px;
            position: absolute;
            top: 0px;
            left: 0px;
            z-index: 1;
        }
        .auto-style2 {
            width: 376px;
            height: 268px;
            position: absolute;
            top: 105px;
            left: 13px;
            z-index: 1;
        }
        .auto-style3 {
            position: absolute;
            top: 76px;
            left: 18px;
            z-index: 1;
        }
        .auto-style4 {
            position: absolute;
            top: 80px;
            left: 419px;
            z-index: 1;
        }
        .auto-style5 {
            width: 546px;
            height: 259px;
            position: absolute;
            top: 111px;
            left: 420px;
            z-index: 1;
        }
        .auto-style6 {
            width: 492px;
            height: 139px;
            position: absolute;
            top: 428px;
            left: 21px;
            z-index: 1;
        }
        .auto-style7 {
            position: absolute;
            top: 394px;
            left: 13px;
            z-index: 1;
            right: 487px;
        }
        .auto-style8 {
            position: absolute;
            top: 430px;
            left: 685px;
            z-index: 1;
            width: 75px;
            height: 30px;
        }
        .auto-style9 {
            position: absolute;
            top: 470px;
            left: 691px;
            z-index: 1;
        }
        .auto-style11 {
            position: absolute;
            top: 500px;
            left: 689px;
            z-index: 1;
            height: 90px;
            width: 259px;
        }
        .auto-style13 {
            width: 341px;
            height: 187px;
            position: absolute;
            top: 37px;
            left: 7px;
            z-index: 1;
        }
        .auto-style14 {
            width: 441px;
            height: 175px;
            position: absolute;
            top: 37px;
            left: 7px;
            z-index: 1;
        }
        .auto-style15 {
            position: absolute;
            top: 35px;
            left: 334px;
            z-index: 1;
        }
        </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:Panel ID="Panel1" runat="server" BackColor="#F0F0F0" CssClass="auto-style1">
        <asp:Panel ID="Panel2" runat="server" CssClass="auto-style2" BackColor="White" ScrollBars="Both">
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="auto-style13" DataKeyNames="AtmNo" DataSourceID="SqlDataSource1" OnDataBound="GridView1_DataBound" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="AtmNo" HeaderText="AtmNo" ReadOnly="True" SortExpression="AtmNo" />
                    <asp:BoundField DataField="CurrentSesNo" HeaderText="CurrentSesNo" SortExpression="CurrentSesNo" />
                    <asp:BoundField DataField="AtmName" HeaderText="AtmName" SortExpression="AtmName" />
                    <asp:BoundField DataField="LastReplDt" HeaderText="LastReplDt" SortExpression="LastReplDt" />
                    <asp:BoundField DataField="NextReplDt" HeaderText="NextReplDt" SortExpression="NextReplDt" />
                    <asp:BoundField DataField="BankId" HeaderText="BankId" SortExpression="BankId" />
                    <asp:BoundField DataField="AuthUser" HeaderText="AuthUser" SortExpression="AuthUser" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ATMsConnectionString %>" SelectCommand="SELECT [AtmNo], [CurrentSesNo], [AtmName], [LastReplDt], [NextReplDt], [BankId], [AuthUser] FROM [AtmsMain]"></asp:SqlDataSource>
        </asp:Panel>
        <asp:Label ID="Label1" runat="server" CssClass="auto-style3" Font-Bold="True" Font-Size="Large" ForeColor="#0066FF" Text="MY ATMS"></asp:Label>
        <asp:Label ID="Label2" runat="server" CssClass="auto-style4" Font-Bold="True" Font-Size="Large" ForeColor="#0066FF" Text="REPLENISHMENT CYCLES"></asp:Label>
        <asp:Panel ID="Panel3" runat="server" BackColor="White" CssClass="auto-style5" ScrollBars="Both">
            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" CssClass="auto-style14" DataKeyNames="SesNo" DataSourceID="SqlDataSource2" OnDataBound="GridView2_DataBound" OnSelectedIndexChanged="GridView2_SelectedIndexChanged">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="SesNo" HeaderText="SesNo" InsertVisible="False" ReadOnly="True" SortExpression="SesNo" />
                    <asp:BoundField DataField="AtmNo" HeaderText="AtmNo" SortExpression="AtmNo" />
                    <asp:BoundField DataField="SesDtTimeStart" HeaderText="SesDtTimeStart" SortExpression="SesDtTimeStart" />
                    <asp:BoundField DataField="SesDtTimeEnd" HeaderText="SesDtTimeEnd" SortExpression="SesDtTimeEnd" />
                    <asp:BoundField DataField="FirstTraceNo" HeaderText="FirstTraceNo" SortExpression="FirstTraceNo" />
                    <asp:BoundField DataField="LastTraceNo" HeaderText="LastTraceNo" SortExpression="LastTraceNo" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ATMsConnectionString %>" SelectCommand="SELECT [SesNo], [AtmNo], [SesDtTimeStart], [SesDtTimeEnd], [FirstTraceNo], [LastTraceNo] FROM [SessionsStatusTraces] WHERE ([AtmNo] = @AtmNo)">
                <SelectParameters>
                    <asp:ControlParameter ControlID="GridView1" Name="AtmNo" PropertyName="SelectedValue" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
        </asp:Panel>
        <asp:Panel ID="Panel4" runat="server" BackColor="White" CssClass="auto-style6">
            <asp:Label ID="Label5" runat="server" Text="Status"></asp:Label>
            <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            <asp:Label ID="Label6" runat="server" Text="Condition"></asp:Label>
            <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
        </asp:Panel>
        <asp:Label ID="Label3" runat="server" CssClass="auto-style7" Font-Bold="True" Font-Size="Large" ForeColor="#0066FF" Text="STATUS OF CHOSEN"></asp:Label>
        <asp:Button ID="Button1" runat="server" BackColor="White" CssClass="auto-style8" Font-Bold="True" ForeColor="#0066FF" Text="Proceed" OnClick="Button1_Click" />
        <asp:Label ID="Label4" runat="server" CssClass="auto-style9" Font-Bold="True" Font-Size="Large" ForeColor="#FF0066" Text="MESSAGE"></asp:Label>
        <asp:TextBox ID="txtMessage" runat="server" CssClass="auto-style11"></asp:TextBox>
        <asp:Label ID="labelDescription" runat="server" CssClass="auto-style15" Font-Bold="True" Font-Size="X-Large" ForeColor="#000099" Text="DESCRIPTION "></asp:Label>
    </asp:Panel>
</asp:Content>
