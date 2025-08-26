<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="WebForm67_BDC.aspx.cs" Inherits="WebForm67_BDC" %>

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
                                <asp:Label ID="Label11" runat="server" Text="Journal Lines"></asp:Label></li>
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
                <div class="col-md-10">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title">
                                <asp:Label ID="Label1" runat="server" Text="JOURNAL LINES"></asp:Label></h3>
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
                                    runat="server"
                                    AutoGenerateColumns="False" OnSelectedIndexChanged="OnSelectedIndexChanged" DataKeyNames="AtmNo" OnDataBound="GridView1_DataBound">
                                    <Columns>
                                        <asp:CommandField ShowSelectButton="True" />
                                        <asp:BoundField DataField="AtmNo" HeaderText="AtmNo" />
                                        <asp:BoundField DataField="Journal_id" HeaderText="Journal_id" />
                                        <asp:BoundField DataField="Journal_LN" HeaderText="Journal_LN" />
                                        <asp:BoundField DataField="TxtLine" HeaderText="TxtLine" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div class="atmlinks">

                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="row my-atms-box1">
                <div class="col-md-12">
                    <div class="panel panel-default">
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
                            <asp:Button ID="ButtonFinish" CssClass="btn btn-info" runat="server" Text="Finish" OnClick="ButtonFinish_Click"></asp:Button>
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
                    "searching": false,
                    "autoWidth" : true
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
