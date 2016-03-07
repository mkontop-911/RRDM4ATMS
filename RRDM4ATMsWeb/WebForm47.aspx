<%@ Page Title="" Language="C#" MasterPageFile="~/RRDMMasters/RRDM4ATMs.Master" AutoEventWireup="true" CodeBehind="WebForm47.aspx.cs" Inherits="RRDM4ATMsWeb.WebForm47" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 1024px;
            height: 899px;
            position: absolute;
            top: 14px;
            left: 11px;
            z-index: 1;
        }
        .auto-style2 {
            width: 480px;
            height: 427px;
            position: absolute;
            top: 51px;
            left: 11px;
            z-index: 1;
        }
        .auto-style3 {
            position: absolute;
            top: 16px;
            left: 17px;
            z-index: 1;
            font-size: x-large;
        }
        .auto-style4 {
            width: 444px;
            height: 427px;
            position: absolute;
            top: 56px;
            left: 513px;
            z-index: 1;
        }
        .auto-style5 {
            position: absolute;
            top: 20px;
            left: 523px;
            z-index: 1;
            height: 28px;
            right: 181px;
            font-size: x-large;
        }
        .auto-style6 {
            width: 269px;
            height: 256px;
            position: absolute;
            top: 579px;
            left: 19px;
            z-index: 1;
        }
        .auto-style7 {
            position: absolute;
            top: 522px;
            left: 17px;
            z-index: 1;
            font-size: x-large;
        }
        .auto-style8 {
            position: absolute;
            top: 550px;
            left: 17px;
            z-index: 1;
            font-size: large;
        }
        .auto-style9 {
            width: 262px;
            height: 247px;
            position: absolute;
            top: 583px;
            left: 323px;
            z-index: 1;
        }
        .auto-style10 {
            position: absolute;
            top: 551px;
            left: 323px;
            z-index: 1;
            width: 85px;
            font-size: large;
        }
        .auto-style11 {
            position: absolute;
            top: 608px;
            left: 629px;
            z-index: 1;
        }
        .auto-style12 {
            position: absolute;
            top: 788px;
            left: 24px;
            z-index: 1;
            }
        .auto-style13 {
            position: absolute;
            top: 791px;
            left: 335px;
            z-index: 1;
        }
        .auto-style14 {
            position: absolute;
            top: 639px;
            left: 626px;
            z-index: 1;
            width: 305px;
            height: 47px;
        }
        .auto-style15 {
            width: 363px;
            height: 172px;
            position: absolute;
            top: 15px;
            left: 17px;
            z-index: 1;
        }
        .auto-style16 {
            width: 391px;
            height: 86px;
            position: absolute;
            top: 24px;
            left: 18px;
            z-index: 1;
        }
        .auto-style17 {
            position: absolute;
            top: 796px;
            left: 676px;
            z-index: 1;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
            <asp:Panel ID="Panel1" runat="server" CssClass="auto-style1" BackColor="#F0F0F0" ForeColor="#3399FF">
            <asp:Panel ID="Panel2" runat="server" CssClass="auto-style2" BackColor="White" ForeColor="Black" ScrollBars="Vertical">
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="auto-style15" DataKeyNames="AtmNo" DataSourceID="SqlDataSource1" OnDataBound="GridView1_DataBound" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                    <Columns>
                        <asp:CommandField ShowSelectButton="True" />
                        <asp:BoundField DataField="AtmNo" HeaderText="AtmNo" ReadOnly="True" SortExpression="AtmNo" />
                        <asp:BoundField DataField="CurrentSesNo" HeaderText="CurrentSesNo" SortExpression="CurrentSesNo" />
                        <asp:BoundField DataField="AtmName" HeaderText="AtmName" SortExpression="AtmName" />
                        <asp:BoundField DataField="AuthUser" HeaderText="AuthUser" SortExpression="AuthUser" />
                        <asp:BoundField DataField="RespBranch" HeaderText="RespBranch" SortExpression="RespBranch" />
                        <asp:BoundField DataField="NextReplDt" HeaderText="NextReplDt" SortExpression="NextReplDt" />
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ATMsConnectionString %>" SelectCommand="SELECT [AtmNo], [CurrentSesNo], [AtmName], [AuthUser], [RespBranch], [NextReplDt] FROM [AtmsMain]"></asp:SqlDataSource>
            </asp:Panel>
            <strong>
            <asp:Label ID="Label1" runat="server" CssClass="auto-style3" Text="MY ATMS"></asp:Label>
            </strong>
            <asp:Panel ID="Panel3" runat="server" BackColor="White" CssClass="auto-style4" ScrollBars="Vertical">
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ATMsConnectionString %>" SelectCommand="SELECT * FROM [AtmsMain] WHERE ([AtmNo] = @AtmNo)">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="GridView1" Name="AtmNo" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CssClass="auto-style16" DataKeyNames="AtmNo" DataSourceID="SqlDataSource2">
                    <Fields>
                        <asp:BoundField DataField="AtmNo" HeaderText="AtmNo" ReadOnly="True" SortExpression="AtmNo" />
                        <asp:BoundField DataField="CurrentSesNo" HeaderText="CurrentSesNo" SortExpression="CurrentSesNo" />
                        <asp:BoundField DataField="AtmName" HeaderText="AtmName" SortExpression="AtmName" />
                        <asp:BoundField DataField="BankId" HeaderText="BankId" SortExpression="BankId" />
                        <asp:BoundField DataField="RespBranch" HeaderText="RespBranch" SortExpression="RespBranch" />
                        <asp:BoundField DataField="BranchName" HeaderText="BranchName" SortExpression="BranchName" />
                        <asp:BoundField DataField="LastReplDt" HeaderText="LastReplDt" SortExpression="LastReplDt" />
                        <asp:BoundField DataField="NextReplDt" HeaderText="NextReplDt" SortExpression="NextReplDt" />
                        <asp:CheckBoxField DataField="ReconcDiff" HeaderText="ReconcDiff" SortExpression="ReconcDiff" />
                        <asp:CheckBoxField DataField="MoreMaxCash" HeaderText="MoreMaxCash" SortExpression="MoreMaxCash" />
                        <asp:CheckBoxField DataField="LessMinCash" HeaderText="LessMinCash" SortExpression="LessMinCash" />
                        <asp:BoundField DataField="NeedType" HeaderText="NeedType" SortExpression="NeedType" />
                        <asp:BoundField DataField="CurrCassettes" HeaderText="CurrCassettes" SortExpression="CurrCassettes" />
                        <asp:BoundField DataField="CurrentDeposits" HeaderText="CurrentDeposits" SortExpression="CurrentDeposits" />
                        <asp:BoundField DataField="EstReplDt" HeaderText="EstReplDt" SortExpression="EstReplDt" />
                        <asp:BoundField DataField="CitId" HeaderText="CitId" SortExpression="CitId" />
                        <asp:BoundField DataField="LastUpdated" HeaderText="LastUpdated" SortExpression="LastUpdated" />
                        <asp:BoundField DataField="AuthUser" HeaderText="AuthUser" SortExpression="AuthUser" />
                        <asp:BoundField DataField="ActionNo" HeaderText="ActionNo" SortExpression="ActionNo" />
                        <asp:BoundField DataField="LastDispensedHistor" HeaderText="LastDispensedHistor" SortExpression="LastDispensedHistor" />
                        <asp:BoundField DataField="LastInNeedReview" HeaderText="LastInNeedReview" SortExpression="LastInNeedReview" />
                        <asp:BoundField DataField="SessionsInDiff" HeaderText="SessionsInDiff" SortExpression="SessionsInDiff" />
                        <asp:BoundField DataField="ErrOutstanding" HeaderText="ErrOutstanding" SortExpression="ErrOutstanding" />
                        <asp:BoundField DataField="ReplCycleNo" HeaderText="ReplCycleNo" SortExpression="ReplCycleNo" />
                        <asp:BoundField DataField="ReconcCycleNo" HeaderText="ReconcCycleNo" SortExpression="ReconcCycleNo" />
                        <asp:BoundField DataField="ReconcDt" HeaderText="ReconcDt" SortExpression="ReconcDt" />
                        <asp:BoundField DataField="CurrNm1" HeaderText="CurrNm1" SortExpression="CurrNm1" />
                        <asp:BoundField DataField="DiffCurr1" HeaderText="DiffCurr1" SortExpression="DiffCurr1" />
                        <asp:BoundField DataField="ProcessMode" HeaderText="ProcessMode" SortExpression="ProcessMode" />
                        <asp:BoundField DataField="AtmsReconcGroup" HeaderText="AtmsReconcGroup" SortExpression="AtmsReconcGroup" />
                        <asp:BoundField DataField="Operator" HeaderText="Operator" SortExpression="Operator" />
                    </Fields>
                </asp:DetailsView>
            </asp:Panel>
            <strong>
            <asp:Label ID="Label2" runat="server" CssClass="auto-style5" Text="DETAILS PER ATM"></asp:Label>
            </strong>
            <asp:Panel ID="Panel4" runat="server" BackColor="White" CssClass="auto-style6">
                <asp:Calendar ID="Calendar1" runat="server" OnSelectionChanged="Calendar1_SelectionChanged" BackColor="#FFFFCC" BorderColor="#FFCC66" BorderWidth="1px" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="#663399" Height="200px" ShowGridLines="True" Width="220px">
                    <DayHeaderStyle BackColor="#FFCC66" Font-Bold="True" Height="1px" />
                    <NextPrevStyle Font-Size="9pt" ForeColor="#FFFFCC" />
                    <OtherMonthDayStyle ForeColor="#CC9966" />
                    <SelectedDayStyle BackColor="#CCCCFF" Font-Bold="True" />
                    <SelectorStyle BackColor="#FFCC66" />
                    <TitleStyle BackColor="#990000" Font-Bold="True" Font-Size="9pt" ForeColor="#FFFFCC" />
                    <TodayDayStyle BackColor="#FFCC66" ForeColor="White" />
                </asp:Calendar>
            </asp:Panel>
            <strong>
            <asp:Label ID="Label3" runat="server" CssClass="auto-style7" Text="REPLENISHMENT CYCLES PER PERIOD"></asp:Label>
            <asp:Label ID="Label4" runat="server" CssClass="auto-style8" Text="FROM DATE"></asp:Label>
            </strong>
            <asp:Panel ID="Panel5" runat="server" BackColor="White" CssClass="auto-style9">
                <asp:Calendar ID="Calendar2" runat="server" Height="200px" OnSelectionChanged="Calendar2_SelectionChanged" BackColor="#FFFFCC" BorderColor="#FFCC66" BorderWidth="1px" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="#663399" ShowGridLines="True" Width="220px">
                    <DayHeaderStyle BackColor="#FFCC66" Font-Bold="True" Height="1px" />
                    <NextPrevStyle Font-Size="9pt" ForeColor="#FFFFCC" />
                    <OtherMonthDayStyle ForeColor="#CC9966" />
                    <SelectedDayStyle BackColor="#CCCCFF" Font-Bold="True" />
                    <SelectorStyle BackColor="#FFCC66" />
                    <TitleStyle BackColor="#990000" Font-Bold="True" Font-Size="9pt" ForeColor="#FFFFCC" />
                    <TodayDayStyle BackColor="#FFCC66" ForeColor="White" />
                </asp:Calendar>
            </asp:Panel>
            <strong>
            <asp:Label ID="Label5" runat="server" CssClass="auto-style10" Text="TO DATE"></asp:Label>
            </strong>
            <asp:Button ID="ButtonShow" runat="server" CssClass="auto-style11" Text="Show" OnClick="ButtonShow_Click" />
            <asp:TextBox ID="TxtFromDt" runat="server" BackColor="#CCCCFF" CssClass="auto-style12" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="TxtToDt" runat="server" BackColor="#CCCCFF" CssClass="auto-style13"></asp:TextBox>
            <asp:TextBox ID="TxtMessage" runat="server" CssClass="auto-style14" ReadOnly="True" Visible="False" ForeColor="#FF3300"></asp:TextBox>
            <asp:Button ID="ButtonHome" runat="server" CssClass="auto-style17" OnClick="ButtonHome_Click" Text="Home" />
        </asp:Panel>
</asp:Content>
