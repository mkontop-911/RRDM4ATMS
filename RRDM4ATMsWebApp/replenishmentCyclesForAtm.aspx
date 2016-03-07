<%@ Page Language="C#" AutoEventWireup="true" CodeFile="replenishmentCyclesForAtm.aspx.cs" Inherits="replenishmentCyclesForAtm" %>

<%@ Register TagPrefix="uc" TagName="uchead" Src="pieces/head.ascx" %>

<!doctype html>
<html>
    <head runat="server">
        <!-- Head -->
        <uc:uchead id="uchead" runat="server" />
        <title></title>
        <link rel="stylesheet" href="css/dataTables.bootstrap.css" type="text/css">
        <script type="text/javascript" src="js/jquery.dataTables.min.js"></script>
        <script type="text/javascript" src="js/dataTables.bootstrap.min.js"></script>
    </head>
    <body>
        <form id="form1" runat="server">
        
        <!-- Header -->
        <div class="header">
            <div class="container">
                <div class="row">
                    <div class="col-md-3 col-sm-5">
                        <div class="header-logo">
                            <asp:Image ID="Image1" ImageUrl="img/rrdsolutions-logo-s.png" runat="server" />
                        </div>
                        <div class="pull-left header-info">
                            <div class="form-inline">
                                <div class="form-group">
                                    <label><asp:Label ID="Label6" runat="server" Text="Date:"></asp:Label></label>
                                    <asp:Label ID="Label7" runat="server" Text="8/9/2015"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-9 col-sm-7">
                        <ol class="breadcrumb">
                            <li class="active"><asp:Label ID="Label11" runat="server" Text="Replenishment Cycles for AtmNo: AB102"></asp:Label></li>
                        </ol>
                    </div>
                </div>
            </div>
        </div>

        <!-- Content -->
        <div class="container" style="margin-top:20px">
            <div class="row">
                <div class="col-md-12">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label1" runat="server" Text="Repl. Cycles"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive">
                                <asp:GridView ID="GridView1" CssClass="table table-bordered table-hover table-condensed" runat="server" AutoGenerateColumns="False"
                                    OnSelectedIndexChanged="OnSelectedIndexChanged" DataKeyNames="AtmNo" OnDataBound="GridView1_DataBound" >
                                    <Columns>
                                        <asp:CommandField ShowSelectButton="True" />
                                        <asp:BoundField DataField="ReplCycle" HeaderText="ReplCycle" />
                                        <asp:BoundField DataField="CycleStart" HeaderText="CycleStart" />
                                        <asp:BoundField DataField="CycleEnd" HeaderText="CycleEnd" />
                                        <asp:BoundField DataField="FirstTraceNo" HeaderText="FirstTraceNo" />
                                        <asp:BoundField DataField="LastTraceNo" HeaderText="LastTraceNo" />
                                    </Columns>
                                </asp:GridView>
                                <br />
                            <ul class="pagination">
                                <li></li>
                                <li><asp:Button ID="btnFirstPage" runat="server" Text="First Page" onclick="btnFirstPage_Click" /></li>
                                <li><asp:Button ID="btnPreviousPage" runat="server" Text="Previous Page" onclick="btnPreviousPage_Click" /></li>
                                <li><asp:Button ID="btnNextPage" runat="server" Text="Next Page" onclick="btnNextPage_Click" /></li>
                                <li><asp:Button ID="btnLastPage" runat="server" Text="Last Page" onclick="btnLastPage_Click" /></li>
                            </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label2" runat="server" Text="Repl. Cycles Information Fro: 3144"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row" style="margin-bottom:20px">
                                <div class="col-md-4">
                                    <asp:TextBox ID="TextBox4" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label17" runat="server" Text="Atm No"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox2" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label5" runat="server" Text="Repl. Cycle"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox5" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:Button ID="Button5" CssClass="btn btn-white" runat="server" Text="eJournal" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label8" runat="server" Text="First Trace No"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox6" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label9" runat="server" Text="Last Trace No"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox7" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label10" runat="server" Text="Number of Tranc"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox8" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label12" runat="server" Text="Dispernsed Amt"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox9" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label13" runat="server" Text="Remaining Amt"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox10" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label3" runat="server" Text="Repl Dt Tm Start"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBox1" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label14" runat="server" Text="Repl Dt Tm End"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBox11" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label15" runat="server" Text="Reconc Dt Tm Start"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBox12" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label16" runat="server" Text="Reconc Dt Tm End"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBox13" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label4" runat="server" Text="Currency"></asp:Label></label>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="TextBox3" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label18" runat="server" Text="Amount In Diff"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox14" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label19" runat="server" Text="Sessions In Diff"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox15" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label20" runat="server" Text="Number of Err"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox16" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label21" runat="server" Text="Outstanding Err"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox17" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:Button ID="Button1" CssClass="btn btn-white" runat="server" Text="Errors" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label  class="col-md-3 control-label"><asp:Label ID="Label22" runat="server" Text="User for Replenishment"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBox18" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                            <div class="col-md-4">
                                                <asp:Button ID="Button2" CssClass="btn btn-white pull-right" runat="server" Text="Repl Cycle PlayBack" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-3 control-label"><asp:Label ID="Label23" runat="server" Text="User for Reconciliation"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBox19" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                            <div class="col-md-4">
                                                <asp:Button ID="Button3" CssClass="btn btn-white pull-right" runat="server" Text="Reconc. PlayBack" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

            <div id="fmessage" style="display:none">
            <div class="login-body">
                <h5><label><asp:Label ID="lblmsg" runat="server" Text=""></asp:Label></label></h5>
            </div>
        </div>

            <!-- Footer -->
        <footer class="footer">
            <div class="container">
                <div class="row">
                    <div class="col-sm-6">
                        <h6><asp:Label ID="Label46" runat="server" Text="No quidance information available"></asp:Label></h6>
                    </div>
                    <div class="col-sm-6">
                        <div class="pull-right">
                            <asp:Button ID="Button4" CssClass="btn btn-info" runat="server" Text="Finish"></asp:Button>
                        </div>
                    </div>
                </div>
            </div>
        </footer>

        </form>

        <script type="text/javascript">
            $(document).ready(function () {
                $('.table').DataTable({
                    "paging": false,
                    "info": false,
                    "searching": false
                });
            });

            function showMessage(message) {
                $('#<%=lblmsg.ClientID%>').html(message);

                $("#fmessage").dialog({
                    title: "System Message Board",
                    buttons: {
                        "Ok": function () {
                            $(this).dialog("close");

                        }
                    },
                    create: function () {
                        $(this).closest(".ui-dialog")
                            .find(".ui-dialog-buttonset button")
                            .addClass("btn btn-info");
                    },
                    modal: true
                });
        </script>

    </body>
</html>
