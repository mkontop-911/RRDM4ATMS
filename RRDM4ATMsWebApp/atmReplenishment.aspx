<%@ Page Language="C#" AutoEventWireup="true" CodeFile="atmReplenishment.aspx.cs" Inherits="atmReplenishment" %>

<%@ Register TagPrefix="uc" TagName="uchead" Src="pieces/head.ascx" %>

<!doctype html>
    <head runat="server">
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
                                <asp:Image ID="Image2" ImageUrl="img/rrdsolutions-logo-s.png" runat="server" />
                            </div>
                            <div class="pull-left header-info">
                                <div class="form-inline">
                                    <div class="form-group">
                                        <label><asp:Label ID="Label8" runat="server" Text="Date:"></asp:Label></label>
                                        <asp:Label ID="Label9" runat="server" Text="8/9/2015"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-9 col-sm-7">
                            <ol class="breadcrumb">
                                <li class="active"><asp:Label ID="Label1" runat="server" Text="ATMs Replenishment"></asp:Label></li>
                            </ol>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Content -->
            <div class="container replenishment" style="margin-top:20px">
                <div class="row">
                    <div class="col-md-5">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><asp:Label ID="Label6" runat="server" Text="Your ATMs"></asp:Label></h3>
                            </div>
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView1" CssClass="table table-bordered table-hover table-condensed" runat="server"></asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-7">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><asp:Label ID="Label7" runat="server" Text="Repl. Cycle/s For ATM"></asp:Label></h3>
                            </div>
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView2" CssClass="table table-bordered table-hover table-condensed" runat="server"></asp:GridView>                                    
                                </div>
                                <div class="row" style="margin-top:20px">
                                    <div class="col-md-4">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <label class="col-sm-8 control-label"><asp:Label ID="Label5" runat="server" Text="Chosen Repo Cycle Mode"></asp:Label></label>
                                                <div class="col-sm-4">
                                                    <asp:TextBox class="form-control" ID="TextBox1" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-8">
                                        <asp:TextBox id="TextArea1" CssClass="form-control" TextMode="multiline" Columns="50" Rows="5" runat="server" ReadOnly="true" />
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
                        <h6><asp:Label ID="Label4" runat="server" Text="Replenishment HAs been delayed. Please take action"></asp:Label></h6>
                    </div>
                    <div class="col-sm-6">
                        <div class="pull-right">
                            <asp:Button ID="Button2" CssClass="btn btn-info" runat="server" Text="Finish"></asp:Button>
                            <asp:Button ID="Button1" CssClass="btn btn-info" runat="server" Text="Next"></asp:Button>
                        </div>
                    </div>
                </div>
                </div>
            </footer>
        
        </form>
    </body>

    <script type="text/javascript">
        $(document).ready(function () {
            $('.table').DataTable({
                "paging": false,
                "info": false,
                "searching": false
            });
        });
        $(document).ready(function () {
            $('.replenishment').each(function () {
                var highestBox = 0;
                $('.panel-body', this).each(function () {
                    if ($(this).height() > highestBox)
                        highestBox = $(this).height();
                });
                $('.panel-body', this).height(highestBox);
            });
        });
    </script>

</html>
