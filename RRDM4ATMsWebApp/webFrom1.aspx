<%@ Page Language="C#" AutoEventWireup="true" CodeFile="webFrom1.aspx.cs" Inherits="webFrom1" %>

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
                                <asp:Image ID="Image1" ImageUrl="img/rrdsolutions-logo-s.png" runat="server" />
                            </div>
                            <div class="pull-left menu-ttl">
                                <h2><asp:Label ID="Label1" runat="server" Text="Main Menu"></asp:Label></h2>
                                <asp:Label ID="Label2" runat="server" Text="User No:"></asp:Label>
                                <asp:Label ID="Label3" runat="server" Text="1005"></asp:Label>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <nav class="top-menu pull-right">
                                <ul>
                                    <li>
                                        <asp:HyperLink ID="HyperLink1" NavigateUrl="#" runat="server"><i class="icon-mail"></i></asp:HyperLink>
                                        <asp:Label ID="Label4" runat="server" Text="Messages"></asp:Label>
                                    </li>
                                    <li>
                                        <asp:HyperLink ID="HyperLink2" NavigateUrl="#" runat="server"><i class="icon-users"></i></asp:HyperLink>
                                        <asp:Label ID="Label5" runat="server" Text="Communicate with controller"></asp:Label>
                                    </li>
                                </ul>
                            </nav>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:HyperLink ID="HyperLink3" runat="server" CssClass="text-left">Logout</asp:HyperLink>
                            <div class="checkbox pull-right">
                                <label>
                                    <asp:CheckBox ID="CheckBox1" runat="server" /> Check me out
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Content -->
            <div class="container main-menu" style="margin-top:20px">
                <div class="row">
                    <div class="col-md-2 col-sm-3 home-padd">
                        <div class="row">
                            <h5 class="ttl col-md-12"><asp:Label ID="Label15" runat="server" Text="Configuration"></asp:Label></h5>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-green" ID="HyperLink4" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label6" runat="server" Text="Define Banks 1111"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-blue" ID="HyperLink5" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label7" runat="server" Text="ATMs Maintenace"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-green" ID="HyperLink13" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label21" runat="server" Text="Define Banks 2222"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-blue" ID="HyperLink14" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label22" runat="server" Text="ATMs Maintenace"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-green" ID="HyperLink15" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label23" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-green" ID="HyperLink16" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label24" runat="server" Text="ATMs Maintenace"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-green" ID="HyperLink17" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label25" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-green" ID="HyperLink18" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label26" runat="server" Text="ATMs Maintenace"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-green" ID="HyperLink19" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label27" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-green" ID="HyperLink20" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label28" runat="server" Text="ATMs Maintenace"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-light-green" ID="HyperLink21" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label29" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-grey" ID="HyperLink22" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label30" runat="server" Text="ATMs Maintenace"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-light-green" ID="HyperLink33" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label41" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                        </div>
                    </div>
                    <div class="col-md-2 col-sm-3 home-padd">
                        <div class="row">
                            <h5 class="ttl col-md-12"><asp:Label ID="Label16" runat="server" Text="Operations"></asp:Label></h5>
                            <asp:LinkButton CssClass="col-xs-6 bg-purple" ID="lnkToATM" runat="server" OnClick="lnkToATM_Click">
                                <span class="square">
                                    <asp:Label ID="Label8" runat="server" Text="My ATMs"></asp:Label>
                                </span>
                            </asp:LinkButton>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-purple" ID="HyperLink7" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label9" runat="server" Text="Replenishment"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-purple" ID="HyperLink23" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label31" runat="server" Text="Capture Cards"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-purple" ID="HyperLink24" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label32" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-purple" ID="HyperLink25" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label33" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-red" ID="HyperLink26" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label34" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-purple" ID="HyperLink27" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label35" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-purple" ID="HyperLink28" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label36" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-red" ID="HyperLink29" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label37" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-purple" ID="HyperLink30" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label38" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-red" ID="HyperLink31" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label39" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-red" ID="HyperLink32" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label40" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-12 bg-olive" ID="HyperLink34" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label42" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                        </div>
                    </div>
                    <div class="col-md-1 col-sm-3 home-padd">
                        <div class="row">
                            <h5 class="ttl col-md-12"><asp:Label ID="Label17" runat="server" Text="Monitoring"></asp:Label></h5>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 col-sm-6 col-md-12 bg-red" ID="HyperLink8" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label10" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 col-sm-6 col-md-12 bg-red" ID="HyperLink35" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label43" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 col-sm-6 col-md-12 bg-red" ID="HyperLink36" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label44" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 col-sm-6 col-md-12 bg-red" ID="HyperLink37" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label45" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 col-sm-6 col-md-12 bg-red" ID="HyperLink38" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label46" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                        </div>
                    </div>
                    <div class="col-md-1 col-sm-3 home-padd">
                        <div class="row">
                            <h5 class="ttl col-md-12"><asp:Label ID="Label18" runat="server" Text="Disputes"></asp:Label></h5>
                             <asp:LinkButton CssClass="col-xs-6 col-sm-6 col-md-12 bg-purple" ID="lnkToDisp" runat="server" OnClick="lnkToDISP_Click">
                                <span class="square">
                                    <asp:Label ID="Label66" runat="server" Text="Disputes Pre Investigation"></asp:Label>
                                </span>
                            </asp:LinkButton>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 col-sm-6 col-md-12 bg-purple" ID="HyperLink40" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label48" runat="server" Text="Dispute Registration"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 col-sm-6 col-md-12 bg-pink" ID="HyperLink41" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label49" runat="server" Text="Dispute Management"></asp:Label>
                                </span>
                            </asp:HyperLink>
                        </div>
                    </div>
                    <div class="clearfix visible-sm"></div>
                    <div class="col-md-2 col-sm-3 home-padd">
                        <div class="row">
                            <h5 class="ttl col-md-12"><asp:Label ID="Label19" runat="server" Text="MIS"></asp:Label>
                           
                            </h5>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-dark-red" ID="HyperLink10" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label12" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-dark-red" ID="HyperLink11" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label13" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-dark-red" ID="HyperLink39" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label47" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-dark-red" ID="HyperLink42" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label50" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-dark-red" ID="HyperLink43" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label51" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-grey" ID="HyperLink44" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label52" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-grey" ID="HyperLink45" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label53" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-grey" ID="HyperLink46" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label54" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-grey" ID="HyperLink47" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label55" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-grey" ID="HyperLink48" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label56" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-dark-red" ID="HyperLink49" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label57" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 bg-grey" ID="HyperLink50" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label58" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-12 bg-grey" ID="HyperLink55" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label65" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                        </div>
                    </div>
                    <div class="col-md-1 col-sm-3 home-padd">
                        <div class="row">
                            <h5 class="ttl col-md-12"><asp:Label ID="Label20" runat="server" Text="Quality"></asp:Label></h5>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 col-sm-6 col-md-12 bg-olive" ID="HyperLink12" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label14" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 col-sm-6 col-md-12 bg-olive" ID="HyperLink51" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label59" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 col-sm-6 col-md-12 bg-olive" ID="HyperLink52" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label60" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-6 col-sm-6 col-md-12 bg-olive" ID="HyperLink53" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label61" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                        </div>
                    </div>
                    <div class="col-md-2 col-sm-3 home-padd">
                        <div class="row">
                            <h5 class="ttl col-md-12"><asp:Label ID="Label63" runat="server" Text="Electronic Authorisation"></asp:Label></h5>
                            <asp:HyperLink NavigateUrl="#" CssClass="col-xs-12 bg-light-pink" ID="HyperLink54" runat="server">
                                <span class="square">
                                    <asp:Label ID="Label62" runat="server" Text="Define Banks"></asp:Label>
                                </span>
                            </asp:HyperLink>
                        </div>
                    </div>
                </div>
            </div>

            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <div class="checkbox pull-right">
                            <label>
                                <asp:CheckBox ID="CheckBox2" runat="server" /> Assist
                            </label>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Mobile Menu -->
           <ul id="mobilemenu">
                <li>Configuration
                    <ul>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink56" runat="server">Define Banks</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink57" runat="server">ATMs Maintenance</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink58" runat="server">Holidays and Special dates</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink59" runat="server">Users and CIT Providers</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink60" runat="server">Accounts Managements</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink61" runat="server">ATM groups</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink62" runat="server">Matched Dates exceptopms for an ATM</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink63" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink64" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink65" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink66" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink67" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink68" runat="server">...</asp:HyperLink></li>
                    </ul>
                </li>
                <li>Operations
                    <ul>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink69" runat="server">My ATMs Operation</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink70" runat="server">ATM Replenishment</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink71" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink72" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink73" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink74" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink75" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink76" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink77" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink78" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink79" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink80" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink81" runat="server">...</asp:HyperLink></li>
                    </ul>
                </li>
                <li>Monitoring
                    <ul>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink82" runat="server">Todays's Operation Status</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink83" runat="server">Daily Reporting</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink84" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink85" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink86" runat="server">...</asp:HyperLink></li>
                    </ul>
                </li>
                <li>Disputes
                    <ul>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink87" runat="server">Dispute Pre - Investigation</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink88" runat="server">Dispute Registration</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink89" runat="server">...</asp:HyperLink></li>
                    </ul>
                </li>
                <li>MIS
                    <ul>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink90" runat="server">MIS for ATMs Repl and </asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink91" runat="server">CIT Providers Performance</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink92" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink93" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink94" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink95" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink96" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink97" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink98" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink99" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink100" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink101" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink102" runat="server">...</asp:HyperLink></li>
                    </ul>
                </li>
                <li>Quality
                    <ul>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink103" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink104" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink105" runat="server">...</asp:HyperLink></li>
                        <li><asp:HyperLink NavigateUrl="#" ID="HyperLink106" runat="server">...</asp:HyperLink></li>
                    </ul>
                </li>
                <li><asp:HyperLink NavigateUrl="#" ID="HyperLink107" runat="server">Pending Authorisations</asp:HyperLink></li>
            </ul>

            <!-- Footer -->
            <footer class="footer"></footer>

        </form>
    </body>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#mobilemenu').slicknav();
        });
    </script>
</html>
