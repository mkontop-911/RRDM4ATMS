<%@ Page Language="C#" AutoEventWireup="true" CodeFile="summarySheet.aspx.cs" Inherits="summarySheet" %>

<%@ Register TagPrefix="uc" TagName="uchead" Src="pieces/head.ascx" %>

<!doctype html>
<html>
    <head id="Head1" runat="server">
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
                            <li><a href="inputCountedFiqures.aspx"><asp:Label ID="Label8" runat="server" Text="Input Counted Fiqures"></asp:Label></a></li>
                            <li><a href="inputDepositsFiqures.aspx"><asp:Label ID="Label9" runat="server" Text="Input Deposits Figures"></asp:Label></a></li>
                            <li><a href="inMoney.aspx"><asp:Label ID="Label10" runat="server" Text="In Money"></asp:Label></a></li>
                            <li class="active"><asp:Label ID="Label11" runat="server" Text="Summary Sheet"></asp:Label></li>
                        </ol>
                    </div>
                </div>
            </div>
        </div>

        <!-- Content -->
        <div class="container summery-sheet" style="margin-top:20px">
            <div class="row  summery-sheet-box1">
                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label12" runat="server" Text="Cassette Notes"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive no-border">
                                <table class="table no-border">
                                    <thead>
                                        <tr>
                                            <th class="col-sm-6"></th>
                                            <th class="col-sm-2"><asp:Label ID="Label19" runat="server" Text="My Count"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label20" runat="server" Text="Per ATM"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label21" runat="server" Text="Difference"></asp:Label></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:Label ID="Label36" runat="server" Text="Currency EUR"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox12" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox13" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox14" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
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
                            <h3 class="panel-title"><asp:Label ID="Label13" runat="server" Text="Deposits and Cheques - EUR"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive no-border">
                                <table class="table no-border">
                                    <thead>
                                        <tr>
                                            <th class="col-sm-6"></th>
                                            <th class="col-sm-2"><asp:Label ID="Label18" runat="server" Text="My Count"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label22" runat="server" Text="Per ATM"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label23" runat="server" Text="Difference"></asp:Label></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:Label ID="Label24" runat="server" Text="Total Deposites"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox1" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox2" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox3" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label25" runat="server" Text="Cheques Amount"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox4" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox5" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox6" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row  summery-sheet-box2">
                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label14" runat="server" Text="Captured Cards and Physical Check"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive no-border">
                                <table class="table no-border">
                                    <thead>
                                        <tr>
                                            <th class="col-sm-6"></th>
                                            <th class="col-sm-2"><asp:Label ID="Label26" runat="server" Text="My Count"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label27" runat="server" Text="Per ATM"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label28" runat="server" Text="Difference"></asp:Label></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:Label ID="Label29" runat="server" Text="Captured Cards"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox7" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox8" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox9" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label30" runat="server" Text="ATM Physical Inspection Check Problem?"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox26" CssClass="form-control" runat="server"></asp:TextBox></td>
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
                            <h3 class="panel-title"><asp:Label ID="Label15" runat="server" Text="Replenishment Amounts"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive no-border">
                                <table class="table no-border">
                                    <thead>
                                        <tr>
                                            <th class="col-sm-4"></th>
                                            <th class="col-sm-2"><asp:Label ID="Label31" runat="server" Text="In Amount"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label32" runat="server" Text="Days"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label33" runat="server" Text="Avarage"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label35" runat="server" Text="System Said"></asp:Label></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:Label ID="Label34" runat="server" Text="Currency EUR"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox11" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox15" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox16" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox23" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td><asp:TextBox ID="TextBox17" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox18" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox19" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox24" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td><asp:TextBox ID="TextBox20" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox21" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox22" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox25" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="row">
                                <div class="col-sm-4">
                                    <label><asp:Label ID="Label42" runat="server" Text="Money in Alert"></asp:Label></label>
                                </div>
                                <div class="col-sm-4">
                                    <div class="container-black">
                                        <div class="circle bg-success active"></div>
                                        <div class="circle bg-warning"></div>
                                        <div class="circle bg-danger"></div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-4">
                                    <label><asp:Label ID="Label43" runat="server" Text="Next Repl Date:"></asp:Label></label>
                                </div>
                                <div class="col-sm-4">
                                    <asp:TextBox ID="TextBox29" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row  summery-sheet-box3">
                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label16" runat="server" Text="Insurance Status"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive no-border">
                                <table class="table no-border">
                                    <thead>
                                        <tr>
                                            <th class="col-sm-6"></th>
                                            <th class="col-sm-2"><asp:Label ID="Label37" runat="server" Text="My Count"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label38" runat="server" Text="Per ATM"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label39" runat="server" Text="Difference"></asp:Label></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:Label ID="Label40" runat="server" Text="Insurance"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox10" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox27" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox28" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <label><asp:Label ID="Label41" runat="server" Text="Insurance Alert"></asp:Label></label>
                                </div>
                                <div class="col-sm-4">
                                    <div class="container-black">
                                        <div class="circle bg-success active"></div>
                                        <div class="circle bg-warning"></div>
                                        <div class="circle bg-danger"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label17" runat="server" Text="Overall Results"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-sm-8">
                                    <asp:TextBox id="TextArea1" CssClass="form-control" TextMode="multiline" Columns="50" Rows="5" runat="server" ReadOnly="true" />
                                </div>
                                <div class="col-sm-4 text-center">
                                    <div><label><asp:Label ID="Label44" runat="server" Text="Insurance Alert"></asp:Label></label></div>
                                    <div class="container-black" style="margin-bottom:20px">
                                        <div class="circle bg-success active"></div>
                                        <div class="circle bg-warning"></div>
                                        <div class="circle bg-danger"></div>
                                    </div>
                                    <div class="notes">
                                        <i class="icon-doc-text"></i>
                                        <span class="num badge badge-info">
                                            <asp:Label ID="Label45" runat="server" Text="0"></asp:Label>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row" style="margin-bottom:20px">
                <div class="col-md-12">
                    <asp:Button ID="Button1" runat="server" CssClass="btn btn-white pull-right" Text="Authorise" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label47" runat="server" Text="Authoriser Section"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-5">
                                    <p>
                                        <label><asp:Label ID="Label49" runat="server" Text="Requestor:"></asp:Label></label>
                                        <asp:Label ID="Label50" runat="server" Text="Nicos Ioannou"></asp:Label>
                                    </p>
                                    <p>
                                        <label><asp:Label ID="Label51" runat="server" Text="Authoriser:"></asp:Label></label>
                                        <asp:Label ID="Label52" runat="server" Text="Panicos Michael"></asp:Label>
                                    </p>
                                    <p>
                                        <label><asp:Label ID="Label53" runat="server" Text="Date of Request:"></asp:Label></label>
                                        <asp:Label ID="Label54" runat="server" Text="8/9/2005 3:37:00pm"></asp:Label>
                                    </p>
                                    <p>
                                        <label><asp:Label ID="Label55" runat="server" Text="Current Status:"></asp:Label></label>
                                        <asp:Label ID="Label56" runat="server" Text="Authorise accepted. Ready to finish"></asp:Label>
                                    </p>
                                </div>
                                <div class="col-md-5">
                                    <label><asp:Label ID="Label48" runat="server" Text="Authoriser Comment"></asp:Label></label>
                                    <asp:TextBox id="TextBox30" CssClass="form-control" TextMode="multiline" Columns="50" Rows="5" runat="server" ReadOnly="true" />
                                </div>
                                <div class="col-md-2">
                                    <asp:Button ID="Button4" CssClass="btn btn-white pull-right" runat="server" Text="History" />
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
                        <h6><asp:Label ID="Label46" runat="server" Text="Review Suggestions And use Overwrite if Needed"></asp:Label></h6>
                    </div>
                    <div class="col-sm-6">
                        <div class="pull-right">
                            <asp:Button ID="Button2" CssClass="btn btn-info" runat="server" Text="Back"></asp:Button>
                            <asp:Button ID="Button3" CssClass="btn btn-info" runat="server" Text="Finish"></asp:Button>
                        </div>
                    </div>
                </div>
            </div>
        </footer>

        <script type="text/javascript">

            $(document).ready(function () {
                $('.summery-sheet-box1').each(function () {
                    var highestBox = 0;
                    $('.panel-body', this).each(function () {
                        if ($(this).height() > highestBox)
                            highestBox = $(this).height();
                    });
                    $('.panel-body', this).height(highestBox);
                });
                $('.summery-sheet-box2').each(function () {
                    var highestBox = 0;
                    $('.panel-body', this).each(function () {
                        if ($(this).height() > highestBox)
                            highestBox = $(this).height();
                    });
                    $('.panel-body', this).height(highestBox);
                });
                $('.summery-sheet-box3').each(function () {
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