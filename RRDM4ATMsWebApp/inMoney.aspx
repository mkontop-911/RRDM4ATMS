<%@ Page Language="C#" AutoEventWireup="true" CodeFile="inMoney.aspx.cs" Inherits="inMoney" %>

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
        <script type="text/javascript" src="js/raphael-min.js"></script>
        <script type="text/javascript" src="js/morris.min.js"></script>
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
                            <li class="active"><asp:Label ID="Label10" runat="server" Text="In Money"></asp:Label></li>
                            <li><a href="summarySheet.aspx"><asp:Label ID="Label11" runat="server" Text="Summary Sheet"></asp:Label></a></li>
                        </ol>
                    </div>
                </div>
            </div>
        </div>

        <!-- Content -->
        <div class="container in-mouney" style="margin-top:20px">
            <div class="row in-mouney-box1">
                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label12" runat="server" Text="Replenishment - EUR"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <label class="col-md-6 control-label"><asp:Label ID="Label17" runat="server" Text="Current Balance"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBox1" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <h5><u><asp:Label ID="Label14" runat="server" Text="Choose method of Repl"></asp:Label></u></h5>
                                    <div class="radio">
                                        <label>
                                            <asp:RadioButton ID="RadioButton1" runat="server" GroupName="methodRepl" />
                                            <asp:Label ID="Label15" runat="server" Text="Repl to be Next Working Date"></asp:Label>
                                        </label>
                                    </div>
                                    <div class="form-inline" style="margin-bottom:10px">
                                        <div class="radio">
                                            <label>
                                                <asp:RadioButton ID="RadioButton2" runat="server" GroupName="methodRepl" />
                                                <asp:Label ID="Label16" runat="server" Text="At Defined Date"></asp:Label>
                                            </label>
                                        </div>
                                        <div class="form-group">
                                            <asp:DropDownList CssClass="form-control" ID="DropDownList1" runat="server">
                                                <asp:ListItem>Friday.....</asp:ListItem>
                                                <asp:ListItem>Saturday....</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="form-inline">
                                        <div class="radio">
                                            <label>
                                                <asp:RadioButton ID="RadioButton3" runat="server" GroupName="methodRepl" />
                                                <asp:Label ID="Label18" runat="server" Text="Repl with this amount"></asp:Label>
                                            </label>
                                        </div>
                                        <div class="form-group">
                                            <asp:TextBox ID="TextBox2" CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="pull-right form-group">
                                        <asp:Button ID="Button4" CssClass="btn btn-white"  runat="server" Text="show"></asp:Button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label13" runat="server" Text="Last 30 Days"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div id="bar-example" style="height: 250px;"></div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="pull-right form-group">
                                        <asp:Button ID="Button2" CssClass="btn btn-white" runat="server" Text="Zoom"></asp:Button>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="row in-mouney-box2">
                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label19" runat="server" Text="General Replenishments Info"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label class="col-md-6 control-label"><asp:Label ID="Label22" runat="server" Text="Replenishment Type"></asp:Label></label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="TextBox3" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-md-6 control-label"><asp:Label ID="Label23" runat="server" Text="Next Replenishment Day"></asp:Label></label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="TextBox4" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-md-6 control-label"><asp:Label ID="Label24" runat="server" Text="Next Replenishment Date"></asp:Label></label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="TextBox5" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-md-6 control-label"><asp:Label ID="Label25" runat="server" Text="Numbr od days including today"></asp:Label></label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="TextBox6" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-md-6 control-label"><asp:Label ID="Label26" runat="server" Text="To be Added %"></asp:Label></label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="TextBox7" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-md-6 control-label"><asp:Label ID="Label27" runat="server" Text="Insured Limit"></asp:Label></label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="TextBox8" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label><asp:Label ID="Label28" runat="server" Text="Holiday Descr"></asp:Label></label>
                                <asp:TextBox id="TextArea1" CssClass="form-control" TextMode="multiline" Columns="50" Rows="3" runat="server" ReadOnly="true" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><asp:Label ID="Label20" runat="server" Text="Estimated Amount Per Day - EUR"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive">
                                <asp:GridView ID="GridView1" CssClass="table table-bordered table-hover table-condensed" runat="server"></asp:GridView>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <label class="col-md-6 control-label"><asp:Label ID="Label29" runat="server" Text="Total"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBox9" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <label class="col-md-6 control-label"><asp:Label ID="Label30" runat="server" Text="With Addes %"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBox10" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-md-6 control-label"><asp:Label ID="Label31" runat="server" Text="Override"></asp:Label></label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="TextBox11" CssClass="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Button ID="Button5" CssClass="btn btn-white pull-right" runat="server" Text="Apply" />
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
                            <h3 class="panel-title"><asp:Label ID="Label21" runat="server" Text="To Fill-in-Notes"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive no-border">
                                <table class="table no-border">
                                    <thead>
                                        <tr>
                                            <th class="col-sm-4"></th>
                                            <th class="col-sm-2"><asp:Label ID="Label32" runat="server" Text="Cassette 100 EUR"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label33" runat="server" Text="Cassette 50 EUR"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label34" runat="server" Text="Cassette 20 EUR"></asp:Label></th>
                                            <th class="col-sm-2"><asp:Label ID="Label35" runat="server" Text="Cassette 10 EUR"></asp:Label></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:Label ID="Label36" runat="server" Text="Current Notes"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox12" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox13" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox14" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox40" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label37" runat="server" Text="Add (Suggested)"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox15" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox16" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox17" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox41" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label38" runat="server" Text="Replace Add With"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox18" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox19" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox20" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox42" CssClass="form-control" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label39" runat="server" Text="Total Notes"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox21" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox22" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox23" CssClass="form-control" runat="server"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox43" CssClass="form-control" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label41" runat="server" Text="Total Money"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox31" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox32" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox33" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox44" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="Label40" runat="server" Text="% of Total"></asp:Label></td>
                                            <td><asp:TextBox ID="TextBox45" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox46" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox47" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                            <td><asp:TextBox ID="TextBox48" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" class="text-right"><label><asp:Label ID="Label42" runat="server" Text="GL IN"></asp:Label></label></td>
                                            <td><asp:TextBox ID="TextBox24" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Button ID="Button6" CssClass="btn btn-white pull-right" runat="server" Text="Update" />
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
                        <h6><asp:Label ID="Label44" runat="server" Text="Review Suggestions And use Overwrite if Needed"></asp:Label></h6>
                    </div>
                    <div class="col-sm-6">
                        <div class="pull-right">
                            <asp:Button ID="Button1" CssClass="btn btn-info" runat="server" Text="Back"></asp:Button>
                            <asp:Button ID="Button3" CssClass="btn btn-info" runat="server" Text="Next"></asp:Button>
                        </div>
                    </div>
                </div>
            </div>
        </footer>

        <script type="text/javascript">

            //Morris charts snippet - js
            Morris.Bar({
                element: 'bar-example',
                data: [
                    { y: '2/2/2014', a: 15000, b: 24000 },
                    { y: '5/2/2014', a: 19000, b: 15500 },
                    { y: '8/2/2014', a: 12000, b: 13200 },
                    { y: '11/2/2014', a: 12700, b: 12400 },
                    { y: '14/2/2014', a: 13900, b: 13400 },
                    { y: '17/2/2014', a: 11200, b: 14100 },
                    { y: '20/2/2014', a: 11900, b: 11700 },
                    { y: '23/2/2014', a: 13800, b: 12900 },
                    { y: '28/2/2014', a: 13700, b: 13800 }
                    ],
                xkey: 'y',
                ykeys: ['a', 'b'],
                labels: ['Cash', 'Dispensed']
            });
            
            $(document).ready(function () {
                $('.in-mouney-box1').each(function () {
                    var highestBox = 0;
                    $('.panel-body', this).each(function () {
                        if ($(this).height() > highestBox)
                            highestBox = $(this).height();
                    });
                    $('.panel-body', this).height(highestBox);
                });
                $('.in-mouney-box2').each(function () {
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
