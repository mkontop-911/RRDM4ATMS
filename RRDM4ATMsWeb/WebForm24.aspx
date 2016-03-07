<%@ Page Title="" Language="C#" MasterPageFile="~/RRDMMasters/RRDM4ATMs.Master" AutoEventWireup="true" CodeBehind="WebForm24.aspx.cs" Inherits="RRDM4ATMsWeb.WebForm24" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
    .auto-style1 {
        width: 1014px;
        height: 605px;
        position: absolute;
        top: 3px;
        left: 3px;
        z-index: 1;
    }
    .auto-style2 {
        width: 963px;
        height: 403px;
        position: absolute;
        top: 99px;
        left: 17px;
        z-index: 1;
    }
    .auto-style3 {
        width: 187px;
        height: 133px;
        position: absolute;
        top: 53px;
        left: 33px;
        z-index: 1;
    }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <p>
    </p>
<asp:Panel ID="Panel1" runat="server" BackColor="#F0F0F0" CssClass="auto-style1">
    <asp:Panel ID="Panel2" runat="server" CssClass="auto-style2" BackColor="White" ScrollBars="Both">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="auto-style3" DataKeyNames="ErrNo" DataSourceID="SqlDataSource1">
            <Columns>
                <asp:BoundField DataField="ErrNo" HeaderText="ErrNo" InsertVisible="False" ReadOnly="True" SortExpression="ErrNo" />
                <asp:BoundField DataField="ErrId" HeaderText="ErrId" SortExpression="ErrId" />
                <asp:BoundField DataField="ErrType" HeaderText="ErrType" SortExpression="ErrType" />
                <asp:BoundField DataField="ErrDesc" HeaderText="ErrDesc" SortExpression="ErrDesc" />
                <asp:BoundField DataField="DateInserted" HeaderText="DateInserted" SortExpression="DateInserted" />
                <asp:BoundField DataField="AtmNo" HeaderText="AtmNo" SortExpression="AtmNo" />
                <asp:BoundField DataField="SesNo" HeaderText="SesNo" SortExpression="SesNo" />
                <asp:CheckBoxField DataField="OpenErr" HeaderText="OpenErr" SortExpression="OpenErr" />
                <asp:BoundField DataField="CurDes" HeaderText="CurDes" SortExpression="CurDes" />
                <asp:BoundField DataField="ErrAmount" HeaderText="ErrAmount" SortExpression="ErrAmount" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ATMsConnectionString %>" SelectCommand="SELECT [ErrNo], [ErrId], [ErrType], [ErrDesc], [DateInserted], [AtmNo], [SesNo], [OpenErr], [CurDes], [ErrAmount] FROM [ErrorsTable]"></asp:SqlDataSource>
    </asp:Panel>
</asp:Panel>
</asp:Content>
