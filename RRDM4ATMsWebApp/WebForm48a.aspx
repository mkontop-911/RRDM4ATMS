<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="WebForm48a.aspx.cs" Inherits="WebForm48a" %>

<%@ Register TagPrefix="uc" TagName="uchead" Src="pieces/head.ascx" %>

<!doctype html>
<html>
    <head id="Head1" runat="server">
        <!-- Head -->
        <uc:uchead id="uchead" runat="server" />
        <title></title>
       <link rel="stylesheet" href="css/dataTables.bootstrap.css" type="text/css">
        <script type="text/javascript" src="js/jquery.dataTables.min.js"></script>
        <script type="text/javascript" src="js/dataTables.bootstrap.min.js"></script>
        <style type="text/css">
            .auto-style1 {
                position: relative;
                min-height: 1px;
                float: left;
                width: 58.33333333%;
                left: 0px;
                top: -17px;
                padding-left: 15px;
                padding-right: 15px;
            }
        </style>
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
                            <div class="form-inline">
                                <div class="form-group">
                                    <label><asp:Label ID="Label4" runat="server" Text="Operator:"></asp:Label></label>
                                    <asp:Label ID="Label5" runat="server" Text="CRBAGRAA"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-9 col-sm-7">
                        <ol class="breadcrumb">
                            <li class="active"><asp:Label ID="Label11" runat="server" Text="ATM Replenishment Cycles"></asp:Label></li>
                        </ol>
                    </div>
                </div>
            </div>
        </div>

        <!-- Content -->
        <div class="container my-atms" style="margin-top:20px">
            <div class="row my-atms-box1">
                <div class="col-md-5">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label1" runat="server" Text="Basic Info for the Repl Cycles"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label class="col-md-6 control-label"><asp:Label ID="Label30" runat="server" Text="Choose antry from table"></asp:Label></label>
                                    <div class="col-md-6">
                                        <div class="input-group">
                                            <asp:TextBox ID="TextBox1" cssClass="form-control" runat="server"></asp:TextBox>
                                            <span class="input-group-btn">
                                                <asp:Button ID="Button1" CssClass="btn btn-info" runat="server" Text="Go" />
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="table-responsive">
                                <asp:GridView ID="GridView1" CssClass="table table-bordered table-hover table-condensed"
                                runat="server" AutoGenerateColumns="false" OnSelectedIndexChanged="OnSelectedIndexChanged" DataKeyNames="AtmNo" OnDataBound="GridView1_DataBound">
                                <Columns>
                                <asp:CommandField ShowSelectButton="True" />
                                <asp:BoundField DataField="ReplCycle" HeaderText="ReplCycle"  />
                                <asp:BoundField DataField="CycleStart" HeaderText="CycleStart" />
                                <asp:BoundField DataField="CycleEnd" HeaderText="CycleEnd"  />
                                <asp:BoundField DataField="FirstTraceNo" HeaderText="FirstTraceNo" />
                                <asp:BoundField DataField="LastTraceNo" HeaderText="LastTraceNo"  />
                                <asp:BoundField DataField="AtmNo" HeaderText="AtmNo"  />
           
                                </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="auto-style1">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label2" runat="server" Text="Info for Replenishment Cycle"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-5">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label17" runat="server" Text="Bank"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxBank" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label18" runat="server" Text="Branch"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxBranch" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label19" runat="server" Text="Owner User"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxOwnerUser" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label20" runat="server" Text="Name"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxName" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label8" runat="server" Text="Email"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxEmail" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label9" runat="server" Text="Mobile"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxMobile" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label10" runat="server" Text="Repl. Cycle No"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxReplCycleNo" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label12" runat="server" Text="Last Repl. Dt"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxLastReplDt" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group" style="margin-bottom:20px">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label13" runat="server" Text="Next Repl. Dt"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxNextReplDt" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label14" runat="server" Text="Cassettes Amnt"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxCassettesAmnt" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label15" runat="server" Text="Deposited Amnt"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxDepositedAmnt" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-7">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label16" runat="server" Text="Last Reconciliation Date"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBoxLastReconcDt" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group" style="margin-bottom:37px">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label21" runat="server" Text="Reconciliation Diff"></asp:Label></label>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="TextBoxReconcDiff" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-6 control-label"><asp:Label ID="Label22" runat="server" Text="Currency"></asp:Label></label>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="TextBoxCurrency" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label23" runat="server" Text="Amount in Diff"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBoxAmountInDiff" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label24" runat="server" Text="Sessions in Diff"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBoxSessionsInDiff" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group" style="margin-bottom:100px">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label25" runat="server" Text="Outstanding Error"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBoxOutstandingErr" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:Button ID="ButtonErrors" CssClass="btn btn-white" runat="server" Text="Errors" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label26" runat="server" Text="In process of action"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBoxInProcessForAction" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label  class="col-md-5 control-label"><asp:Label ID="Label27" runat="server" Text="Status"></asp:Label></label>
                                            <div class="col-md-7">
                                                <asp:TextBox id="TextAreaStatus" CssClass="form-control" TextMode="multiline" Columns="50" Rows="3" runat="server" ReadOnly="true" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row my-atms-box1">
            </div>
        </div>

         <div id="test" style="display:none">
            <div class="login-body">
                <h5><label><asp:Label ID="lblmsg" runat="server" Text=""></asp:Label></label></h5>
            </div>
        </div>

        <!-- Footer -->
        <footer class="footer">
            <div class="container">
                <div class="row">
                    <div class="col-sm-6">
                        <h6><asp:Label ID="MsgGuidance" runat="server" Text="Current Atms Status. ......."></asp:Label></h6>
                    </div>
                    <div class="col-sm-6">
                        <div class="pull-right">
                            <asp:Button ID="Button3" CssClass="btn btn-info" runat="server" Text="Finish"></asp:Button>
                        </div>
                    </div>
                </div>
            </div>
        </footer>

        <script type="text/javascript">
            $(document).ready(function () {
                $('.table').DataTable({
                    "paging": false,
                    "info": false,
                    "searching": false
                });
            });
            //Datepicker
            $(".datepicker").each(function (e) {
                $(this).datepicker({
                    format: "dd/mm/yyyy",
                    autoclose: true
                }).on('changeDate', function (ev) {
                    $(this).datepicker('hide');
                });
                if ($(this).hasClass('datepicker')) {
                    $(this).removeClass('datepicker');
                }
                $(this).datepicker();
            });
            $(document).ready(function () {
                $('.my-atms-box1').each(function () {
                    var highestBox = 0;
                    $('.panel-body', this).each(function () {
                        if ($(this).height() > highestBox)
                            highestBox = $(this).height();
                    });
                    $('.panel-body', this).height(highestBox);
                });
            });

            $('#RadioButton1').click(function () {

                $('#dvShow').show();
            });

            $('#RadioButton2').click(function () {
                $('#dvShow').hide();
            });

            $('#RadioButton3').click(function () {
                $('#dvShow').hide();
            });

            function showMessage(message) {
                $('#<%=lblmsg.ClientID%>').html(message);

                $("#test").dialog({
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
            }

            //Tooltip
            $(document).ready(function () {
                $("#tooltip1").popover({
                    placement: 'top',
                    html: true,
                    title: 'This is title',
                    trigger: 'hover',
                    content: "Click here to find examples of bootstrap tooltip"
                });
            });

        </script>

           
        </form>
    </body>
</html>

