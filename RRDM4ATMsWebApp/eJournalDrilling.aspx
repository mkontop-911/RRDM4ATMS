<%@ Page Language="C#" AutoEventWireup="true" CodeFile="eJournalDrilling.aspx.cs" Inherits="eJournalDrilling" %>

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
                            <li class="active"><asp:Label ID="Label11" runat="server" Text="E_Journa Drilling"></asp:Label></li>
                        </ol>
                    </div>
                </div>
            </div>
        </div>

        <!-- Content -->
        <div class="container eJournal" style="margin-top:20px">
            <div class="row">
                <div class="col-md-12">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label1" runat="server" Text="Your ATMs"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <label class="col-md-5 control-label"><asp:Label ID="Label30" runat="server" Text="Choose entry from table Or Atm No"></asp:Label></label>
                                            <div class="col-md-4">
                                                <div class="input-group">
                                                    <asp:TextBox ID="TextBox1" cssClass="form-control" runat="server"></asp:TextBox>
                                                    <span class="input-group-btn">
                                                        <asp:Button ID="Button1" CssClass="btn btn-info" runat="server" Text="Find" />
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:Button ID="Button2" CssClass="btn btn-white pull-right" runat="server" Text="Refresh" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="table-responsive">
                                        <asp:GridView ID="GridView1" CssClass="table table-bordered table-hover table-condensed" runat="server"></asp:GridView>
                                    </div>
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label  class="col-md-8 control-label"><asp:Label ID="Label17" runat="server" Text="Chosen Atm No"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox2" CssClass="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <h4 style="padding-bottom: 29px;"><label class="col-md-12 control-label"><asp:Label ID="Label4" runat="server" Text="Repl. Cycles/s Fro Atm: AB104"></asp:Label></label></h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="GridView2" CssClass="table table-bordered table-hover table-condensed" runat="server"></asp:GridView>
                                    </div>
                                    <div class="form-horizontal form-smpad">
                                        <div class="form-group">
                                            <label  class="col-md-4 control-label"><asp:Label ID="Label5" runat="server" Text="For Repl. Cycle"></asp:Label></label>
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TextBox4" CssClass="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row eJournal-box1">
                <div class="col-md-4">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label2" runat="server" Text="Select Period And Mode"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <h4><asp:Label ID="Label8" runat="server" Text="Period"></asp:Label></h4>
                            <div class="form-group">
                                <label><asp:Label ID="Label28" runat="server" Text="Start From:"></asp:Label></label>
                                <asp:DropDownList CssClass="form-control" ID="DropDownList1" runat="server">
                                    <asp:ListItem>---Dates---</asp:ListItem>
                                    <asp:ListItem>---Dates---</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label><asp:Label ID="Label29" runat="server" Text="End:"></asp:Label></label>
                                <asp:DropDownList CssClass="form-control" ID="DropDownList2" runat="server">
                                    <asp:ListItem>---Dates---</asp:ListItem>
                                    <asp:ListItem>---Dates---</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="row">
                                <div class="col-md-offset-6 col-md-6">
                                    <h4><asp:Label ID="Label9" runat="server" Text="Mode"></asp:Label></h4>
                                    <div class="radio">
                                        <label>
                                            <asp:RadioButton ID="RadioButton1" runat="server" GroupName="eJournal" />
                                            Single ATM
                                        </label>
                                    </div>
                                    <div class="radio">
                                        <label>
                                            <asp:RadioButton ID="RadioButton2" runat="server" GroupName="eJournal" />
                                            All My Atms
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-8">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label3" runat="server" Text="Select One of Below"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <h3 class="panel-title"><asp:Label ID="Label10" runat="server" Text="Journal Lines For Period:"></asp:Label></h3>
                                    <div class="radio">
                                        <label>
                                            <asp:RadioButton ID="RadioButton3" runat="server" GroupName="jPeriod" />
                                            <asp:Label ID="Label13" runat="server" Text="Trans"></asp:Label>
                                        </label>
                                    </div>
                                    <div class="radio">
                                        <label>
                                            <asp:RadioButton ID="RadioButton4" runat="server" GroupName="jPeriod" />
                                            <asp:Label ID="Label14" runat="server" Text="Repl Cycles Operation"></asp:Label>
                                        </label>
                                    </div>
                                    <div class="radio">
                                        <label>
                                            <asp:RadioButton ID="RadioButton5" runat="server" GroupName="jPeriod" />
                                            <asp:Label ID="Label15" runat="server" Text="Error"></asp:Label>
                                        </label>
                                    </div>
                                    <div class="radio">
                                        <label>
                                            <asp:RadioButton ID="RadioButton6" runat="server" GroupName="jPeriod" />
                                            <asp:Label ID="Label16" runat="server" Text="Capture Cards"></asp:Label>
                                        </label>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <h3 class="panel-title"><asp:Label ID="Label12" runat="server" Text="Journal Lines For Specific:"></asp:Label></h3>
                                    <div class="radio">
                                        <label>
                                            <asp:RadioButton ID="RadioButton7" runat="server" GroupName="jSpecific" />
                                            <asp:Label ID="Label18" runat="server" Text="Capture Cards"></asp:Label>
                                        </label>
                                    </div>
                                    <label class="radio-inline">
                                        <asp:RadioButton ID="RadioButton8" runat="server" GroupName="jSpecific" />
                                        <asp:Label ID="Label19" runat="server" Text="Card No"></asp:Label>
                                    </label>
                                    <label class="radio-inline">
                                        <asp:RadioButton ID="RadioButton9" runat="server" GroupName="jSpecific" />
                                        <asp:Label ID="Label20" runat="server" Text="Account No"></asp:Label>
                                    </label>
                                    <label class="radio-inline">
                                        <asp:RadioButton ID="RadioButton10" runat="server" GroupName="jSpecific" />
                                        <asp:Label ID="Label21" runat="server" Text="Trace No"></asp:Label>
                                    </label>
                                    <div style="margin-top: 7px;">
                                        <asp:TextBox ID="TextBox3" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <h6 class="help-block"><asp:Label ID="Label22" runat="server" Text="NOTE: There is printing for Trace Number and Repl Cycle"></asp:Label></h6>
                                    <asp:Button ID="Button4" runat="server" CssClass="btn btn-white pull-right" Text="Show" />
                                </div>
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
                        <h6><asp:Label ID="Label46" runat="server" Text="Make your selection for Drilling"></asp:Label></h6>
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
            $(document).ready(function () {
                $('.eJournal-box1').each(function () {
                    var highestBox = 0;
                    $('.panel-body', this).each(function () {
                        if ($(this).height() > highestBox)
                            highestBox = $(this).height();
                    });
                    $('.panel-body', this).height(highestBox);
                });
            });
        </script>

        </form>
    </body>
</html>
