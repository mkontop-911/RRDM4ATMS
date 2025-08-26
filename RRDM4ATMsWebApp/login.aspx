<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

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
                <div class="row" style="margin-bottom:10px">
                    <div class="col-md-6">
                        <div class="header-logo">
                            <asp:Image ID="Image2" ImageUrl="img/rrdsolutions-logo-s.png" runat="server" />
                        </div>
                        <div class="pull-left menu-ttl">
                            <h2><asp:Label ID="Label12" runat="server" Text="RRD Solutions Login Page"></asp:Label></h2>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <nav class="top-menu pull-right">
                        </nav>
                    </div>
                </div>
            </div>
        </div>

        <!-- Footer -->
        <div class="container">
            <div class="row">
                <div class="col-md-offset-3 col-md-6 col-sm-offset-3 col-sm-6">
                    <div class="panel panel-pages panel-primary login">
                        <div class="panel-heading login-header">
                            <div class="header-logo">
                                <asp:Image ID="Image1" ImageUrl="img/rrdsolutions-logo-s.png" runat="server" />
                            </div>
                            <h3 class="login-ttl"><asp:Label ID="Label1" runat="server" Text="Login"></asp:Label></h3>
                            <div class="languages">
                                <asp:DropDownList CssClass="form-control" ID="DropDownList1" runat="server">
                                    <asp:ListItem>English</asp:ListItem>
                                    <asp:ListItem>Greek</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="panel-body">
                            <div class="login-body">
                                <h5><label><asp:Label ID="Label4" runat="server" Text="Please Login to the system"></asp:Label></label></h5>
                                <div class="form-group">
                                    <label><asp:Label ID="Label2" runat="server" Text="USER ID"></asp:Label></label>
                                    <asp:DropDownList CssClass="form-control" ID="DropDownListUsers" runat="server">
                                        <asp:ListItem Value="PILOT_001"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label><asp:Label ID="Label3" runat="server" Text="PASSWORD"></asp:Label></label>
                                    <asp:TextBox ID="TextBoxPassWord" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="login-footer">
                                <asp:Button ID="Button1" CssClass="btn btn-info pull-left" runat="server" Text="Change"></asp:Button>
                                <asp:Button ID="ButtonLogin" CssClass="btn btn-info pull-right" runat="server"
                                    Text="Login" onclick="ButtonLogin_Click"></asp:Button>
                                <asp:Label ID="Label11" runat="server" Text="Message Box "></asp:Label>
                                <asp:TextBox ID="MessageBox" runat="server" Width="265px"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                  
                </div>
            </div>
        </div>

        <!-- Footer -->
        <footer class="footer"></footer>

        <!-- Modal -->
        <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header bg-primary">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="myModalLabel">Modal title</h4>
                    </div>
                    <div class="modal-body">
                        <div class="login-body">
                            <h5><label><asp:Label ID="Label5" runat="server" Text="Please Login to the system"></asp:Label></label></h5>

                            <div class="form-group">
                                <label><asp:Label ID="Label6" runat="server" Text="USER ID"></asp:Label></label>
                                <asp:DropDownList CssClass="form-control" ID="DropDownList3" runat="server">
                                    <asp:ListItem>0001</asp:ListItem>
                                    <asp:ListItem>0002</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label><asp:Label ID="Label7" runat="server" Text="PASSWORD"></asp:Label></label>
                                <asp:TextBox ID="TextBox2" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer bg-primary">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                        <button type="button" class="btn btn-primary">Save changes</button>
                    </div>
                </div>
            </div>
        </div>


        <div id="test" style="display:none">
            <div class="login-body">
                <h5><label><asp:Label ID="Label8" runat="server" Text="Please Login to the system"></asp:Label></label></h5>
                <div class="form-group">
                    <label><asp:Label ID="Label9" runat="server" Text="USER ID"></asp:Label></label>
                    <asp:DropDownList CssClass="form-control" ID="DropDownList4" runat="server">
                        <asp:ListItem>0001</asp:ListItem>
                        <asp:ListItem>0002</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label><asp:Label ID="Label10" runat="server" Text="PASSWORD"></asp:Label></label>
                    <asp:TextBox ID="TextBox3" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
            </div>
        </div>


        <script type="text/javascript">
            function showMessage(message) {
                $('#<%=Label8.ClientID%>').html(message);

                $("#test").dialog({
                    title: "System Message Board",
                    buttons: {
                        "Ok": function () {
                            $(this).dialog("close");
    
                        }
                    },
                create:function () {
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
                    trigger:'hover',
                    content: "Click here to find examples of bootstrap tooltip"
                });
            });
            
        </script>

    </form>
</body>
</html>
