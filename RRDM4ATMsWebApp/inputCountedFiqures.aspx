<%@ Page Language="C#" AutoEventWireup="true" CodeFile="inputCountedFiqures.aspx.cs" Inherits="inputCountedFiqures" %>

<%@ Register TagPrefix="uc" TagName="uchead" Src="pieces/head.ascx" %>

<!doctype html>
<html>
    <head runat="server">
        <!-- Head -->
        <uc:uchead id="uchead" runat="server" />
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
                            <li><a href="physicalInspection.aspx"><asp:Label ID="Label1" runat="server" Text="Physical Inspection"></asp:Label></a></li>
                            <li class="active"><asp:Label ID="Label8" runat="server" Text="Input Counted Fiqures"></asp:Label></li>
                            <li><a href="inputDepositsFiqures.aspx"><asp:Label ID="Label9" runat="server" Text="Input Deposits Figures"></asp:Label></a></li>
                            <li><a href="inMoney.aspx"><asp:Label ID="Label10" runat="server" Text="In Money"></asp:Label></a></li>
                            <li><a href="summarySheet.aspx"><asp:Label ID="Label11" runat="server" Text="Summary Sheet"></asp:Label></a></li>
                        </ol>
                    </div>
                </div>
            </div>
        </div>

        <!-- Content -->
        <div class="container counted-fiqures" style="margin-top:20px">
            <div class="row counted-fiqures-box">

                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label12" runat="server" Text="Cassettes"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive no-border">
                                <table class="table no-border">
                                    <thead>
                                        <tr>
                                            <th class="col-sm-6"><asp:Label Font-Size="14px" ID="Label16" runat="server" Text="Notes"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label17" runat="server" Text="My Count"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label18" runat="server" Text="Per ATM"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label19" runat="server" Text="Difference"></asp:Label></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:Label ID="Label21" runat="server" Text="Type1 - 100 EUR"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox1" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox5" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox6" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label22" runat="server" Text="Type1 - 50 EUR"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox2" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox7" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox8" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label23" runat="server" Text="Type1 - 20 EUR"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox3" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox9" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox10" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label24" runat="server" Text="Type1 - 10 EUR"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox4" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox11" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox12" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label13" runat="server" Text="Captures Cards"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive no-border">
                                <table class="table no-border">
                                    <thead>
                                        <tr>
                                            <th class="col-sm-6"></th>
                                            <th class="col-sm-2"><asp:Label ID="Label33" runat="server" Text="My Count"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label34" runat="server" Text="Per ATM"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label35" runat="server" Text="Difference"></asp:Label></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:Label ID="Label36" runat="server" Text="Captured Cards"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox25" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox26" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox27" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <h6 class="help-block"><asp:Label ID="Label40" runat="server" Text="Difference Same as Errors Value"></asp:Label></h6>
                        </div>
                    </div>
                </div>

                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label14" runat="server" Text="Reject Tray"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive no-border">
                                <table class="table no-border">
                                    <thead>
                                        <tr>
                                            <th class="col-sm-6"><asp:Label Font-Size="14px" ID="Label20" runat="server" Text="Notes"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label25" runat="server" Text="My Count"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label26" runat="server" Text="Per ATM"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label27" runat="server" Text="Difference"></asp:Label></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:Label ID="Label28" runat="server" Text="Type1 - 100 EUR"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox13" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox14" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox15" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label29" runat="server" Text="Type1 - 50 EUR"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox16" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox17" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox18" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label30" runat="server" Text="Type1 - 20 EUR"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox19" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox20" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox21" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label31" runat="server" Text="Type1 - 10 EUR"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox22" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox23" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox24" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label15" runat="server" Text="Money Value"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive no-border">
                                <table class="table no-border">
                                    <thead>
                                        <tr>
                                            <th class="col-sm-6"></th>
                                            <th class="col-sm-2"><asp:Label ID="Label32" runat="server" Text="My Count"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label37" runat="server" Text="Per ATM"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label38" runat="server" Text="Difference"></asp:Label></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:Label ID="Label39" runat="server" Text="Captured Cards"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox28" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox29" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox30" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-12">
                    <div class="pull-right form-group">
                        <asp:Button ID="Button2" CssClass="btn btn-white" runat="server" Text="Use ATM Figures"></asp:Button>
                        <asp:Button ID="Button4" CssClass="btn btn-white" runat="server" Text="Updates"></asp:Button>
                    </div>
                </div>
                <div class="col-md-12">
                    <div class="pull-right form-group">
                        <asp:Button ID="Button6" CssClass="btn btn-white" runat="server" Text="Show Errors"></asp:Button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Footer -->
        <footer class="footer">
            <div class="container">
            <div class="row">
                <div class="col-sm-6">
                    <h6><asp:Label ID="Label44" runat="server" Text="Updateing completed - ................."></asp:Label></h6>
                </div>
                <div class="col-sm-6">
                    <div class="pull-right">
                        <asp:Button ID="ButtonBack" CssClass="btn btn-info" runat="server" Text="Back" OnClick="ButtonBack_Click"></asp:Button>
                        <asp:Button ID="ButtonNext" CssClass="btn btn-info" runat="server" Text="Next"></asp:Button>
                    </div>
                </div>
            </div>
            </div>
        </footer>

    </form>

    <script type="text/javascript">
        $(document).ready(function () {
            $('.counted-fiqures-box').each(function () {
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
