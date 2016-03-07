<%@ Page Language="C#" AutoEventWireup="true" CodeFile="physicalInspection.aspx.cs" Inherits="physicalInspection" %>

<%@ Register TagPrefix="uc" TagName="uchead" Src="pieces/head.ascx" %>

<!doctype html>
<html>
    <head id="Head1" runat="server">
        <!-- Head -->
        <uc:uchead id="uchead" runat="server" />
        <link rel="stylesheet" href="css/dataTables.bootstrap.css" type="text/css">
        <script type="text/javascript" src="js/jquery.dataTables.min.js"></script>
        <script type="text/javascript" src="js/dataTables.bootstrap.min.js"></script>
        <title></title>
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
                                        <label><asp:Label ID="Label2" runat="server" Text="ATM No:"></asp:Label></label>
                                        <asp:Label ID="Label3" runat="server" Text="AB102"></asp:Label>
                                    </div>
                                </div>
                                <div class="form-inline">
                                    <div class="form-group">
                                        <label><asp:Label ID="Label4" runat="server" Text="Repl Cycle:"></asp:Label></label>
                                        <asp:Label ID="Label5" runat="server" Text="31444"></asp:Label>
                                    </div>
                                </div>
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
                                <li class="active"><asp:Label ID="Label1" runat="server" Text="Physical Inspection"></asp:Label></li>
                                <li><a href="inputCountedFiqures.aspx"><asp:Label ID="Label8" runat="server" Text="Input Counted Fiqures"></asp:Label></a></li>
                                <li><a href="inputDepositsFiqures.aspx"><asp:Label ID="Label9" runat="server" Text="Input Deposits Fiqures"></asp:Label></a></li>
                                <li><a href="inMoney.aspx"><asp:Label ID="Label10" runat="server" Text="In Money"></asp:Label></a></li>
                                <li><a href="summarySheet.aspx"><asp:Label ID="Label11" runat="server" Text="Summary Sheet"></asp:Label></a></li>
                            </ol>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Content -->
            <div class="container inspectection" style="margin-top:20px">
                <div class="row inspectection-box">

                    <div class="col-md-6">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><asp:Label ID="Label12" runat="server" Text="This Repl Cycle Summary"></asp:Label></h3>
                            </div>
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label17" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox1" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label18" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox2" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label19" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox3" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label20" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox4" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label21" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox5" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label22" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox6" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label23" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox7" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label24" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox8" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><asp:Label ID="Label13" runat="server" Text="Current Filling Level"></asp:Label></h3>
                            </div>
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-md-3">ss</div>
                                    <div class="col-md-9">
                                        <label><asp:Label ID="Label29" runat="server" Text="100 EU"></asp:Label></label>
                                        <div class="progress-container">
                                            <div class="progress progress-striped active">
                                                <div class="progress-bar progress-bar-success progress-100" style="width:0%"></div>
                                            </div>
                                        </div>
                                        <label><asp:Label ID="Label30" runat="server" Text="50 EU"></asp:Label></label>
                                        <div class="progress-container">
                                            <div class="progress progress-striped active">
                                                <div class="progress-bar progress-bar-success progress-50" style="width:0%"></div>
                                            </div>
                                        </div>
                                        <label><asp:Label ID="Label31" runat="server" Text="20 EU"></asp:Label></label>
                                        <div class="progress-container">
                                            <div class="progress progress-striped active">
                                                <div class="progress-bar progress-bar-success progress-20" style="width:0%"></div>
                                            </div>
                                        </div>
                                        <label><asp:Label ID="Label32" runat="server" Text="10 EU"></asp:Label></label>
                                        <div class="progress-container">
                                            <div class="progress progress-striped active">
                                                <div class="progress-bar progress-bar-success progress-10" style="width:0%"></div>
                                            </div>
                                        </div>
                                        <h6 class="help-block"><asp:Label ID="Label35" runat="server" Text="Note: Reject Tray Contains 25 Notes of different donominations"></asp:Label></h6>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><asp:Label ID="Label14" runat="server" Text="Machine Reconciliation"></asp:Label></h3>
                            </div>
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"></label>
                                                <div class="col-md-6 text-center">
                                                    <asp:Label ID="Label34" runat="server" Text="EUR"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label25" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox9" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label26" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox10" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label27" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox11" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label28" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox12" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <label for="TextBox1" class="col-md-6 control-label"><asp:Label ID="Label33" runat="server" Text="Own Customers"></asp:Label></label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="TextBox13" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><asp:Label ID="Label15" runat="server" Text="ATM Physical Inspection - Input"></asp:Label></h3>
                            </div>
                            <div class="panel-body">
                                <div class="row ins-input">
                                    <div class="col-md-6">
                                        <div class="checkbox">
                                            <label>
                                                <input type="checkbox" value="">
                                                <asp:Label ID="Label36" runat="server" Text="No Microchips"></asp:Label>
                                            </label>
                                        </div>
                                        <div class="checkbox">
                                            <label>
                                                <input type="checkbox" value="">
                                                <asp:Label ID="Label38" runat="server" Text="No Cameras"></asp:Label>
                                            </label>
                                        </div>
                                        <div class="checkbox">
                                            <label>
                                                <input type="checkbox" value="">
                                                <asp:Label ID="Label40" runat="server" Text="No Other"></asp:Label>
                                            </label>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="checkbox">
                                            <label>
                                                <input type="checkbox" value="">
                                                <asp:Label ID="Label37" runat="server" Text="No Suspicious Captrured cards"></asp:Label>
                                            </label>
                                        </div>
                                        <div class="checkbox">
                                            <label>
                                                <input type="checkbox" value="">
                                                <asp:Label ID="Label39" runat="server" Text="No Glue Remains"></asp:Label>
                                            </label>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12 text-center">
                                        <h5><label><asp:Label ID="Label41" runat="server" Text="Insert Notes if Any Yes"></asp:Label></label></h5>
                                        <div class="notes">
                                            <i class="icon-doc-text"></i>
                                            <span class="num badge badge-info">
                                                <asp:Label ID="Label43" runat="server" Text="0"></asp:Label>
                                            </span>
                                        </div>
                                        <h6 class="help-block"><asp:Label ID="Label42" runat="server" Text="Note: If there is any issue then you create a Note entry to explain"></asp:Label></h6>
                                        <asp:Button CssClass="btn btn-white pull-right" ID="Button1" runat="server" Text="Update" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><asp:Label ID="Label16" runat="server" Text="Errors"></asp:Label></h3>
                            </div>
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView1" CssClass="table table-bordered table-hover table-condensed" runat="server"></asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <!-- Footer -->
            <footer class="footer">
                <div class="container">
                <div class="row">
                    <div class="col-sm-6">
                        <h6><asp:Label ID="Label44" runat="server" Text="There is ................."></asp:Label></h6>
                    </div>
                    <div class="col-sm-6">
                        <div class="pull-right">
                            <asp:Button ID="Button3" CssClass="btn btn-info" runat="server" Text="Next"></asp:Button>
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
            $(".progress-100").animate({
                width: "70%"
            }, 2500);
            $(".progress-50").animate({
                width: "60%"
            }, 2500);
            $(".progress-20").animate({
                width: "30%"
            }, 2500);
            $(".progress-10").animate({
                width: "5%"
            }, 2500);
            $(document).ready(function () {
                $('.inspectection-box').each(function () {
                    var highestBox = 0;
                    $('.panel-body', this).each(function () {
                        if ($(this).height() > highestBox)
                            highestBox = $(this).height();
                    });
                    $('.panel-body', this).height(highestBox);
                });
            });
        </script>

    </body>
</html>
