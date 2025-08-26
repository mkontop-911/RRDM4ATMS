<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="WebForm80b.aspx.cs" Inherits="WebForm80b" %>

<%@ Register TagPrefix="uc" TagName="uchead" Src="pieces/head.ascx" %>

<!doctype html>
<html>
<head id="Head1" runat="server">
    <%--  <!-- Head -->
    <uc:uchead ID="uchead" runat="server" />
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
    </style>--%>
       <!-- Head -->
    <uc:uchead ID="uchead" runat="server" />
    <title></title>
    <link rel="stylesheet" href="css/dataTables.bootstrap.css" type="text/css">
    <script type="text/javascript" src="js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="js/dataTables.bootstrap.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">

        <!-- Header -->
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
                                    <label>
                                        <asp:Label ID="Label6" runat="server" Text="Date:"></asp:Label></label>
                                    <asp:Label ID="Label7" runat="server" Text="8/9/2015"></asp:Label>
                                </div>
                            </div>
                            <div class="form-inline">
                                <div class="form-group">
                                    <label>
                                        <asp:Label ID="Label4" runat="server" Text="Operator:"></asp:Label></label>
                                    <asp:Label ID="Label5" runat="server" Text="CRBAGRAA"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-9 col-sm-7">
                        <ol class="breadcrumb pull-left">
                            <li class="active">
                                <asp:Label ID="Label11" runat="server" Text="Disputes Pre-Investigation"></asp:Label></li>
                        </ol>
                        <nav class="top-menu pull-right">
                            <ul>
                                <li>
                                    <asp:HyperLink ID="HyperLink1" NavigateUrl="webFrom1.aspx" runat="server"><i class="icon-menu"></i></asp:HyperLink>
                                    <asp:Label ID="Label36" runat="server" Text="Main Menu"></asp:Label>
                                </li>
                            </ul>
                        </nav>
                    </div>
                </div>
            </div>
        </div>

       <%-- <div class="header">
            <div class="container">
                <div class="row">
                    <div class="col-md-3 col-sm-5">
                        <div class="header-logo">
                            <asp:Image ID="Image1" ImageUrl="img/rrdsolutions-logo-s.png" runat="server" />
                        </div>
                        <div class="pull-left header-info">
                            <div class="form-inline">
                                <div class="form-group">
                                    <label>
                                        <asp:Label ID="Label6" runat="server" Text="Date:"></asp:Label></label>
                                    <asp:Label ID="Label7" runat="server" Text="8/9/2015"></asp:Label>
                                </div>
                            </div>
                            <div class="form-inline">
                                <div class="form-group">
                                    <label>
                                        <asp:Label ID="Label4" runat="server" Text="Operator:"></asp:Label></label>
                                    <asp:Label ID="Label5" runat="server" Text="CRBAGRAA"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-9 col-sm-7">
                        <ol class="breadcrumb">
                            <li class="active">
                                <asp:Label ID="Label11" runat="server" Text="Historical Transactions"></asp:Label></li>
                        </ol>
                        <nav class="top-menu pull-right">
                            <ul>
                                <li>
                                    <asp:HyperLink ID="HyperLink1" NavigateUrl="webFrom1.aspx" runat="server"><i class="icon-menu"></i></asp:HyperLink>
                                    <asp:Label ID="Label36" runat="server" Text="Main Menu"></asp:Label>
                                </li>
                            </ul>
                        </nav>
                    </div>
                </div>
            </div>
        </div>--%>

        <!-- Content -->
        <div class="container my-atms" style="margin-top: 20px">
            <div class="row my-atms-box1">
                <div class="col-md-5">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title">
                                <asp:Label ID="Label1" runat="server" Text="SELECTED TRANSACTIONS"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="col-md-6">
                                        <div class="input-group">
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="table-responsive">
                               <asp:GridView ID="GridView1" CssClass="table table-bordered table-hover table-condensed"
                                    runat="server" AutoGenerateColumns="false" OnSelectedIndexChanged="OnSelectedIndexChanged" DataKeyNames="RecordId" OnDataBound="GridView1_DataBound">
                                    <Columns>
                                        <asp:CommandField ShowSelectButton="True" />
                                        <asp:BoundField DataField="RecordId" HeaderText="RecordId" />
                                        <asp:BoundField DataField="Status" HeaderText="Status" />
                                        <asp:BoundField DataField="Done" HeaderText="Done" />

                                        <asp:BoundField DataField="Terminal" HeaderText="Terminal" />
                                        <asp:BoundField DataField="Descr" HeaderText="Descr" />
                                        <asp:BoundField DataField="Err" HeaderText="Err" />

                                        <asp:BoundField DataField="Mask" HeaderText="Mask" />
                                        <asp:BoundField DataField="Ccy" HeaderText="Ccy" />
                                        <asp:BoundField DataField="Amount" HeaderText="Amount" />

                                        <asp:BoundField DataField="Date" HeaderText="Date" />
                                        <asp:BoundField DataField="Trace" HeaderText="Trace" />
                                        <asp:BoundField DataField="ActionType" HeaderText="ActionType" />

                                    </Columns>
                               
                                </asp:GridView>
                            </div>
                              <div>
    
      
    </div>
                        </div>
                    </div>
                </div>
                <div class="auto-style1">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title">
                                <asp:Label ID="Label2" runat="server" Text="INFORMATION FOR TRANSACTION"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-5">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label17" runat="server" Text="RMCateg:"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxRMCateg" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label18" runat="server" Text="RMCycle:"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxRMCycle" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label19" runat="server" Text="Terminal:"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxTerminal" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label20" runat="server" Text="Card No:"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxCardNo" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label8" runat="server" Text="Acc No:"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxAccNo" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label9" runat="server" Text="Curr:"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxCurr" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label10" runat="server" Text="Amnt:"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxAmnt" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label12" runat="server" Text="Date Tm"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxDateTm" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group" style="margin-bottom: 20px">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label13" runat="server" Text="Trace No:"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxTraceNo" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label14" runat="server" Text="Unique ID:"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxUniqueId" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group" style="margin-bottom: 37px">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label15" runat="server" Text="Tran is Dispute"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxDispute" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-7">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label3" runat="server" Text="MATCHING AND ACTIONS"></asp:Label></label>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label16" runat="server" Text="Matching Mask"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxMatchingMask" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group" style="margin-bottom: 37px">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="Label21" runat="server" Text="Masked Files"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxMaskedFiles" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="LabelTransCreated" runat="server" Text="Transaction Created"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxTransCreated" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="LabelPosted" runat="server" Text="Posted"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxPosted" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="LabelForcedMatched" runat="server" Text="FORCED MATCHED"></asp:Label></label>
                                        </div>
                                        <div class="form-group" style="margin-bottom: 100px">
                                            <label class="col-md-6 control-label">
                                                <asp:Label ID="LabelReason" runat="server" Text="Reason"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBoxReason" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                       
                                        </div>
                                        <div class="form-group" style="margin-bottom: 100px">
                                             <div class="col-md-6">
                                                <asp:Button ID="ButtonJournalLines" CssClass="btn btn-white" runat="server" Text="Show Journal Lines" OnClick="ButtonJournalLines_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                          <%--  <<%--div class="row">
                                <div class="col-md-12">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label class="col-md-5 control-label">
                                                <asp:Label ID="Label27" runat="server" Text="Status"></asp:Label></label>
                                            <div class="col-md-7">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>--%>--%>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row my-atms-box1">
            </div>
        </div>

        <div id="test" style="display: none">
            <div class="login-body">
                <h5>
                    <label>
                        <asp:Label ID="lblmsg" runat="server" Text=""></asp:Label></label></h5>
            </div>
        </div>

        <!-- Footer -->
        <footer class="footer">
            <div class="container">
                <div class="row">
                    <div class="col-sm-6">
                        <h6>
                            <asp:Label ID="MsgGuidance" runat="server" Text="Current Atms Status. ......."></asp:Label></h6>
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
                    "paging": true,
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

