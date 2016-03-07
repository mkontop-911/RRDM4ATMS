<%@ Page Title="" Language="C#" MasterPageFile="~/RRDMMasters/RRDM4ATMs.Master" AutoEventWireup="true" CodeBehind="WebForm48b.aspx.cs" Inherits="RRDM4ATMsWeb.WebForm48b" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 1024px;
            height: 768px;
            position: absolute;
            top: 6px;
            left: 10px;
            z-index: 1;
        }
        .auto-style2 {
            position: absolute;
            top: 634px;
            left: 12px;
            z-index: 1;
            width: 479px;
            height: 34px;
        }
        .auto-style3 {
            position: absolute;
            top: 704px;
            left: 12px;
            z-index: 1;
            width: 136px;
            height: 34px;
        }
        .auto-style4 {
            width: 509px;
            height: 552px;
            position: absolute;
            top: 42px;
            left: 7px;
            z-index: 1;
        }
        .auto-style6 {
            width: 459px;
            height: 549px;
            position: absolute;
            top: 46px;
            left: 533px;
            z-index: 1;
        }
        .auto-style8 {
            width: 695px;
            height: 133px;
            position: absolute;
            top: 45px;
            left: 3px;
            z-index: 1;
        }
        .auto-style9 {
            width: 436px;
            height: 67px;
            position: absolute;
            top: 47px;
            left: 14px;
            z-index: 1;
            margin-right: 56px;
        }
        .auto-style12 {
            position: absolute;
            top: 7px;
            left: 554px;
            z-index: 1;
            width: 452px;
            height: 24px;
        }
        .auto-style13 {
            position: absolute;
            top: 9px;
            left: -9px;
            z-index: 1;
            width: 497px;
            height: 24px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:Panel ID="Panel1" runat="server" CssClass="auto-style1" BackColor="#F0F0F0" ForeColor="#0066FF" ScrollBars="Both">
            <asp:TextBox ID="MessageBox" runat="server" CssClass="auto-style2" BackColor="Silver" Font-Bold="True" Font-Size="Medium" ForeColor="White">MessageBox</asp:TextBox>
            <asp:Button ID="Button1" runat="server" CssClass="auto-style3" OnClick="Button1_Click" Text="Back" BackColor="#0066FF" Font-Bold="True" Font-Size="Medium" ForeColor="White" />
            <asp:Panel ID="Panel2" runat="server" CssClass="auto-style4" ScrollBars="Both" BackColor="White">
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ATMsConnectionString %>" SelectCommand="SELECT [SesNo], [SesDtTimeStart], [SesDtTimeEnd], [BankId], [FirstTraceNo], [LastTraceNo], [SessionsInDiff], [AtmNo] FROM [SessionsStatusTraces]"></asp:SqlDataSource>
                <br />
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="auto-style8" DataKeyNames="SesNo" DataSourceID="SqlDataSource1">
                    <Columns>
                        <asp:CommandField ShowSelectButton="True" />
                        <asp:BoundField DataField="SesNo" HeaderText="SesNo" InsertVisible="False" ReadOnly="True" SortExpression="SesNo" />
                        <asp:BoundField DataField="SesDtTimeStart" HeaderText="SesDtTimeStart" SortExpression="SesDtTimeStart" />
                        <asp:BoundField DataField="SesDtTimeEnd" HeaderText="SesDtTimeEnd" SortExpression="SesDtTimeEnd" />
                        <asp:BoundField DataField="BankId" HeaderText="BankId" SortExpression="BankId" />
                        <asp:BoundField DataField="FirstTraceNo" HeaderText="FirstTraceNo" SortExpression="FirstTraceNo" />
                        <asp:BoundField DataField="LastTraceNo" HeaderText="LastTraceNo" SortExpression="LastTraceNo" />
                        <asp:BoundField DataField="SessionsInDiff" HeaderText="SessionsInDiff" SortExpression="SessionsInDiff" />
                        <asp:BoundField DataField="AtmNo" HeaderText="AtmNo" SortExpression="AtmNo" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>
            <asp:Panel ID="Panel3" runat="server" CssClass="auto-style6" ScrollBars="Vertical" BackColor="White">
                <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CssClass="auto-style9" DataKeyNames="SesNo" DataSourceID="SqlDataSource2" Height="50px">
                    <Fields>
                        <asp:BoundField DataField="SesNo" HeaderText="SesNo" InsertVisible="False" ReadOnly="True" SortExpression="SesNo" />
                        <asp:BoundField DataField="AtmNo" HeaderText="AtmNo" SortExpression="AtmNo" />
                        <asp:BoundField DataField="PreSes" HeaderText="PreSes" SortExpression="PreSes" />
                        <asp:BoundField DataField="NextSes" HeaderText="NextSes" SortExpression="NextSes" />
                        <asp:BoundField DataField="AtmName" HeaderText="AtmName" SortExpression="AtmName" />
                        <asp:BoundField DataField="BankId" HeaderText="BankId" SortExpression="BankId" />
                        <asp:BoundField DataField="RespBranch" HeaderText="RespBranch" SortExpression="RespBranch" />
                        <asp:BoundField DataField="SesDtTimeStart" HeaderText="SesDtTimeStart" SortExpression="SesDtTimeStart" />
                        <asp:BoundField DataField="SesDtTimeEnd" HeaderText="SesDtTimeEnd" SortExpression="SesDtTimeEnd" />
                        <asp:BoundField DataField="FirstTraceNo" HeaderText="FirstTraceNo" SortExpression="FirstTraceNo" />
                        <asp:BoundField DataField="LastTraceNo" HeaderText="LastTraceNo" SortExpression="LastTraceNo" />
                        <asp:BoundField DataField="OfflineMinutes" HeaderText="OfflineMinutes" SortExpression="OfflineMinutes" />
                        <asp:BoundField DataField="NoOfTranCash" HeaderText="NoOfTranCash" SortExpression="NoOfTranCash" />
                        <asp:BoundField DataField="NoOfTranDepCash" HeaderText="NoOfTranDepCash" SortExpression="NoOfTranDepCash" />
                        <asp:BoundField DataField="NoOfTranDepCheq" HeaderText="NoOfTranDepCheq" SortExpression="NoOfTranDepCheq" />
                        <asp:BoundField DataField="NoOfCheques" HeaderText="NoOfCheques" SortExpression="NoOfCheques" />
                        <asp:BoundField DataField="SignIdRepl" HeaderText="SignIdRepl" SortExpression="SignIdRepl" />
                        <asp:CheckBoxField DataField="StartRepl" HeaderText="StartRepl" SortExpression="StartRepl" />
                        <asp:CheckBoxField DataField="FinishRepl" HeaderText="FinishRepl" SortExpression="FinishRepl" />
                        <asp:BoundField DataField="ReplStartDtTm" HeaderText="ReplStartDtTm" SortExpression="ReplStartDtTm" />
                        <asp:BoundField DataField="ReplFinDtTm" HeaderText="ReplFinDtTm" SortExpression="ReplFinDtTm" />
                        <asp:CheckBoxField DataField="DiffRepl" HeaderText="DiffRepl" SortExpression="DiffRepl" />
                        <asp:CheckBoxField DataField="ErrsRepl" HeaderText="ErrsRepl" SortExpression="ErrsRepl" />
                        <asp:BoundField DataField="NextRepDtTm" HeaderText="NextRepDtTm" SortExpression="NextRepDtTm" />
                        <asp:BoundField DataField="SignIdReconc" HeaderText="SignIdReconc" SortExpression="SignIdReconc" />
                        <asp:CheckBoxField DataField="StartReconc" HeaderText="StartReconc" SortExpression="StartReconc" />
                        <asp:CheckBoxField DataField="FinishReconc" HeaderText="FinishReconc" SortExpression="FinishReconc" />
                        <asp:BoundField DataField="RecStartDtTm" HeaderText="RecStartDtTm" SortExpression="RecStartDtTm" />
                        <asp:BoundField DataField="RecFinDtTm" HeaderText="RecFinDtTm" SortExpression="RecFinDtTm" />
                        <asp:CheckBoxField DataField="DiffReconcStart" HeaderText="DiffReconcStart" SortExpression="DiffReconcStart" />
                        <asp:CheckBoxField DataField="DiffReconcEnd" HeaderText="DiffReconcEnd" SortExpression="DiffReconcEnd" />
                        <asp:BoundField DataField="NumOfErrors" HeaderText="NumOfErrors" SortExpression="NumOfErrors" />
                        <asp:BoundField DataField="ErrOutstanding" HeaderText="ErrOutstanding" SortExpression="ErrOutstanding" />
                        <asp:BoundField DataField="SessionsInDiff" HeaderText="SessionsInDiff" SortExpression="SessionsInDiff" />
                        <asp:BoundField DataField="ProcessMode" HeaderText="ProcessMode" SortExpression="ProcessMode" />
                        <asp:BoundField DataField="ReplGenComment" HeaderText="ReplGenComment" SortExpression="ReplGenComment" />
                        <asp:BoundField DataField="Operator" HeaderText="Operator" SortExpression="Operator" />
                    </Fields>
                </asp:DetailsView>
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ATMsConnectionString %>" SelectCommand="SELECT [SesNo], [AtmNo], [PreSes], [NextSes], [AtmName], [BankId], [RespBranch], [SesDtTimeStart], [SesDtTimeEnd], [FirstTraceNo], [LastTraceNo], [OfflineMinutes], [NoOfTranCash], [NoOfTranDepCash], [NoOfTranDepCheq], [NoOfCheques], [SignIdRepl], [StartRepl], [FinishRepl], [ReplStartDtTm], [ReplFinDtTm], [DiffRepl], [ErrsRepl], [NextRepDtTm], [SignIdReconc], [StartReconc], [FinishReconc], [RecStartDtTm], [RecFinDtTm], [DiffReconcStart], [DiffReconcEnd], [NumOfErrors], [ErrOutstanding], [SessionsInDiff], [ProcessMode], [ReplGenComment], [Operator] FROM [SessionsStatusTraces] WHERE ([SesNo] = @SesNo)">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="GridView1" Name="SesNo" PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </asp:Panel>
            <asp:TextBox ID="TextBox1" runat="server" BackColor="#0066FF" CssClass="auto-style12" Font-Bold="True" Font-Size="X-Large" ForeColor="White">DETAILS</asp:TextBox>
            <asp:TextBox ID="TextBox2" runat="server" CssClass="auto-style13" BackColor="#0066FF" BorderColor="#0066FF" Font-Bold="True" Font-Size="X-Large" ForeColor="White">REPLENISHMENT CYCLES</asp:TextBox>
        </asp:Panel>  
</asp:Content>
