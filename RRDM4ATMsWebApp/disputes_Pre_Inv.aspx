<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="disputes_Pre_Inv.aspx.cs" Inherits="disputes_Pre_Inv" %>

<%@ Register TagPrefix="uc" TagName="uchead" Src="pieces/head.ascx" %>

<!doctype html>
<html>
<head id="Head1" runat="server">
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

        <!-- Content -->
        <div class="container my-atms" style="margin-top: 20px">
            <div class="row my-atms-box1">
                <div class="col-md-5">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title">
                                <asp:Label ID="Label1" runat="server" Text="FROM BRANCH"></asp:Label></h3>
                        </div>
                        <asp:Image ID="Image2" runat="server" Height="211px" ImageUrl="~/img/imageBranch.jpg" Width="313px" />
                        <br />
                        <br />
                        <br />
                        <div class="panel-heading">
                            <h3 class="panel-title">
                                <asp:Label ID="Label37" runat="server" Text="FROM CALL CENTRE"></asp:Label></h3>
                        </div>
                        <asp:Image ID="Image3" runat="server" Height="211px" ImageUrl="~/img/CallCentre1.jpg" Width="313px" />
                        <br />
                        <br />
                        <br />
                        <div class="panel-heading">
                            <h3 class="panel-title">
                                <asp:Label ID="Label38" runat="server" Text="FROM BACK OFFICE"></asp:Label></h3>
                        </div>
                        <asp:Image ID="Image4" runat="server" Height="211px" ImageUrl="~/img/BackOffice.jpg" Width="313px" />


                    </div>
                </div>

                <div class="col-md-7">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title">
                                <asp:Label ID="Label2" runat="server" Text="SEARCH BY ..."></asp:Label></h3>
                        </div>
                        <br />
                        <div class="col-md-4">
                            <div class="radio">
                                <label>
                                    <asp:RadioButton ID="RadioButtonTrace" runat="server" GroupName="mySelection" />
                                    <asp:Label ID="Label31" runat="server" Text="Trace No"></asp:Label>
                                </label>
                            </div>
                            <div class="radio">
                                <label>
                                    <asp:RadioButton ID="RadioButtonRRN" runat="server" GroupName="mySelection" />
                                    <asp:Label ID="Label32" runat="server" Text="Reference Number"></asp:Label>
                                </label>
                            </div>
                            <div class="radio">
                                <label>
                                    <asp:RadioButton ID="RadioButtonATM" runat="server" GroupName="mySelection" />
                                    <asp:Label ID="Label33" runat="server" Text="ATM NO"></asp:Label>
                                </label>
                            </div>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-horizontal form-smpad">

                                        <br />
                                        <br />
                                        <div class="form-group">
                                            <label class="col-md-4 control-label">
                                                <asp:Label ID="Label17" runat="server" Text="Input"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxInput" CssClass="form-control" runat="server" ReadOnly="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-4 control-label">
                                                <asp:Label ID="Label3" runat="server" Text="ATM NO"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxATM" CssClass="form-control" runat="server" ReadOnly="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-4 control-label">
                                                <asp:Label ID="Label18" runat="server" Text="From Date(dd-mm-yy)"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxFromDt" CssClass="form-control datepicker date" runat="server"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-4 control-label">
                                                <asp:Label ID="Label19" runat="server" Text="To Date(dd-mm-yy)"></asp:Label></label>
                                            <div class="col-md-5">
                                                <asp:TextBox ID="TextBoxToDt" CssClass="form-control datepicker date" runat="server"></asp:TextBox>
                                            </div>
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




        <div id="fmessage" style="display: none">
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
            $('#RadioButton1').click(function () {

                $('#dvShow').hide();
            });

            $('#RadioButton2').click(function () {
                $('#dvShow').hide();
            });

            $('#RadioButton3').click(function () {
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
