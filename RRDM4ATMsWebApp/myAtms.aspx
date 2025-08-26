<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="myAtms.aspx.cs" Inherits="myAtms" %>

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
                top: 16px;
        
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
                        <ol class="breadcrumb pull-left">
                            <li class="active"><asp:Label ID="Label11" runat="server" Text="My ATM/s"></asp:Label></li>
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

        <!-- Content -->
        <div class="container my-atms" style="margin-top:20px">
            <div class="row my-atms-box1">
                <div class="col-md-5">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label1" runat="server" Text="Basic Info of my ATMs"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label class="col-md-6 control-label"><asp:Label ID="Label30" runat="server" Text="Choose antry from table Or Atm No"></asp:Label></label>
                                    <div class="col-md-6">
                                        <div class="input-group">
                                            <asp:TextBox ID="TextBox1" cssClass="form-control" runat="server"></asp:TextBox>
                                            <span class="input-group-btn">
                                                <asp:Button ID="ButtonGo" CssClass="btn btn-info" runat="server" Text="Go" OnClick="ButtonGo_Click" />
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="table-responsive">
                                <asp:GridView ID="GridView1" CssClass="table table-bordered table-hover table-condensed"
                                runat="server" 
                                     AutoGenerateColumns="False" OnSelectedIndexChanged="OnSelectedIndexChanged" DataKeyNames="AtmNo" OnDataBound="GridView1_DataBound" >
                                <Columns>
                                <asp:CommandField ShowSelectButton="True" />
                                <asp:BoundField DataField="AtmNo" HeaderText="AtmNo"  />
                                <asp:BoundField DataField="ReplCycle" HeaderText="ReplCycle" />
                                           <asp:BoundField DataField="AtmName" HeaderText="AtmName"  />
                                <asp:BoundField DataField="RespBranch" HeaderText="RespBranch" />
                                           <asp:BoundField DataField="AuthUser" HeaderText="AuthUser"  />
           
                                </Columns>
                                </asp:GridView>
                            </div>
                            <div class="atmlinks">
            <asp:HyperLink ID="btnFirstPage" NavigateUrl="javascript:void(0)" onclick="btnFirstPage_Click" runat="server">First Page</asp:HyperLink>
            <asp:HyperLink ID="btnPreviousPage" NavigateUrl="javascript:void(0)" onclick="btnPreviousPage_Click" runat="server">Previous Page</asp:HyperLink>
            <asp:HyperLink ID="btnNextPage" NavigateUrl="javascript:void(0)" onclick="btnNextPage_Click" runat="server">Next Page</asp:HyperLink>
            <asp:HyperLink ID="btnLastPage" NavigateUrl="javascript:void(0)" onclick="btnLastPage_Click" runat="server">Last Page</asp:HyperLink>
           
        </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Button ID="Button2" CssClass="btn btn-white pull-left" runat="server" Text="Refresh" />
                                    <asp:Button ID="Button4" CssClass="btn btn-white pull-right" runat="server" Text="ATM Location" OnClick="Button4_Click" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="col-md-7">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="LabelHeadingRight" runat="server" Text="Current Info For ATM: AB102"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label17" runat="server" Text="Bank"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxBank" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label18" runat="server" Text="Branch"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxBranch" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label19" runat="server" Text="Owner User"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxOwnerUser" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label20" runat="server" Text="Name"></asp:Label></label>
                                            <div class="col-md-8">
                                                <asp:TextBox ID="TextBoxName" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label8" runat="server" Text="Email"></asp:Label></label>
                                            <div class="col-md-8">
                                                <asp:TextBox ID="TextBoxEmail" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label9" runat="server" Text="Mobile"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxMobile" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label10" runat="server" Text="Repl. Cycle No"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxReplCycleNo" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label12" runat="server" Text="Last Repl. Dt"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxLastReplDt" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group" style="margin-bottom:20px">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label13" runat="server" Text="Next Repl. Dt"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxNextReplDt" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label14" runat="server" Text="Cassettes Amnt"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxCassettesAmnt" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label15" runat="server" Text="Deposited Amnt"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxDepositedAmnt" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label16" runat="server" Text="Last Reconciliation Date"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBoxLastReconcDt" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group" style="margin-bottom:37px">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label21" runat="server" Text="Reconciliation Diff"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBoxReconcDiff" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label22" runat="server" Text="Currency"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBoxCurrency" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label23" runat="server" Text="Amount in Diff"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBoxAmountInDiff" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label24" runat="server" Text="Sessions in Diff"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBoxSessionsInDiff" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group" style="margin-bottom:100px">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label25" runat="server" Text="Outstanding Error"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBoxOutstandingErr" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:Button ID="ButtonErrors" CssClass="btn btn-white" runat="server" Text="Errors" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label26" runat="server" Text="In process of action"></asp:Label></label>
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
                <div class="col-md-12">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label3" runat="server" Text="Select From Below"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label><asp:Label ID="Label28" runat="server" Text="Start From:"></asp:Label></label>
                                        <asp:TextBox ID="TextBoxFrom" CssClass="form-control datepicker date" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label><asp:Label ID="Label29" runat="server" Text="End:"></asp:Label></label>
                                        <asp:TextBox ID="TextBoxTo" CssClass="form-control datepicker date" runat="server" OnTextChanged="TextBoxTo_TextChanged"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="radio">
                                        <label>
                                            <asp:RadioButton ID="RadioButton1" runat="server" GroupName="myatm" />
                                            <asp:Label ID="Label31" runat="server" Text="Repl Cycles"></asp:Label>
                                        </label>
                                    </div>
                                    <div class="radio">
                                        <label>
                                            <asp:RadioButton ID="RadioButton2" runat="server" GroupName="myatm" />
                                            <asp:Label ID="Label32" runat="server" Text="Actions Life Cycle"></asp:Label>
                                        </label>
                                    </div>
                                    <div class="radio">
                                        <label>
                                            <asp:RadioButton ID="RadioButton3" runat="server" GroupName="myatm" />
                                            <asp:Label ID="Label33" runat="server" Text="Accounts Trans"></asp:Label>
                                        </label>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="dvShow" style="display: none" class="well well-sm">
                                        <div class="form-horizontal form-smpad">
                                            <div class="form-group">
                                                <label  class="col-md-6 control-label"><asp:Label ID="Label34" runat="server" Text="Acc Name"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList CssClass="form-control" ID="DropDownList3" runat="server">
                                                        <asp:ListItem>ATM American Express</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label  class="col-md-6 control-label"><asp:Label ID="Label35" runat="server" Text="Currency"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList CssClass="form-control" ID="DropDownList1" runat="server">
                                                        <asp:ListItem>EUR</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <asp:Button ID="Button6" runat="server" CssClass="btn btn-white pull-right" Text="Show" OnClick="Button6_Click" />
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


                //Datepicker
                $(".datepicker").each(function (e) {

                    $(this).datepicker({
                        dateFormat: 'dd/mm/yy',
                        autoclose: true
                    }).on('changeDate', function (ev) {
                        $(this).datepicker('hide');
                    });
                    if ($(this).hasClass('datepicker')) {
                        $(this).removeClass('datepicker');
                    }
                    $(this).datepicker();
                });



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


            // hide show
            $('#RadioButton1').click (function(){
                
                $('#dvShow').hide();
            });
            
            $('#RadioButton2').click (function(){
                $('#dvShow').hide();
            });

            $('#RadioButton3').click (function(){
                $('#dvShow').show();
            });
            // end


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
