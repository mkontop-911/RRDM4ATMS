<%@ Page Language="C#" AutoEventWireup="true" CodeFile="homepage.aspx.cs" Inherits="homepage" %>

<%@ Register TagPrefix="uc" TagName="uchead" Src="pieces/head.ascx" %>

<!doctype html>
<html>
    <head id="Head1" runat="server">
        <!-- Head -->
        <uc:uchead id="uchead" runat="server" />
        <script src="js/jssor.slider.mini.js"></script>
        <script type="text/javascript">
            jQuery(document).ready(function ($) {
                var options = {
                    $AutoPlay: true,
                    $DragOrientation: 3,                                //[Optional] Orientation to drag slide, 0 no drag, 1 horizental, 2 vertical, 3 either, default value is 1 (Note that the $DragOrientation should be the same as $PlayOrientation when $DisplayPieces is greater than 1, or parking position is not 0)
                    $ArrowNavigatorOptions: {                       //[Optional] Options to specify and enable arrow navigator or not
                        $Class: $JssorArrowNavigator$,              //[Requried] Class to create arrow navigator instance
                        $ChanceToShow: 1,                               //[Required] 0 Never, 1 Mouse Over, 2 Always
                        $AutoCenter: 2,                                 //[Optional] Auto center arrows in parent container, 0 No, 1 Horizontal, 2 Vertical, 3 Both, default value is 0
                        $Steps: 1                                       //[Optional] Steps to go for each navigation request, default value is 1
                    }
                };

                var jssor_slider1 = new $JssorSlider$("slider1_container", options);

                //responsive code begin
                //you can remove responsive code if you don't want the slider scales
                //while window resizes
                function ScaleSlider() {
                    var parentWidth = $('#slider1_container').parent().width();
                    if (parentWidth) {
                        jssor_slider1.$ScaleWidth(parentWidth);
                    }
                    else
                        window.setTimeout(ScaleSlider, 30);
                }
                //Scale slider after document ready
                ScaleSlider();

                //Scale slider while window load/resize/orientationchange.
                $(window).bind("load", ScaleSlider);
                $(window).bind("resize", ScaleSlider);
                $(window).bind("orientationchange", ScaleSlider);
                //responsive code end
            });
        </script>
        <title></title>
    </head>
    <body>
        <form id="form1" runat="server">

            <header class="container-fluid top-header">
                <div class="container">
                    <div class="row">
                        <div class="col-md-4">
                            <div class="homepage-logo">
                                <asp:Image ID="Image1" ImageUrl="img/rrdsolutions-logo.png" runat="server" />
                            </div>
                        </div>
                        <div class="col-md-8">
                            <nav class="homepage-menu pull-right">
                                <ul>
                                    <li>
                                        <asp:Button ID="Button1" CssClass="btn btn-info" runat="server" Text="Login" OnClick="Button1_Click"></asp:Button>
                                    </li>
                                </ul>
                            </nav>
                        </div>
                    </div>
                </div>
            </header>

            <div class="slider">
                <div id="slider1_container" style="position: relative; width: 1900px; height: 520px; overflow: hidden;">
                    <!-- Slides Container -->
                    <div u="slides" style="cursor: move; position: absolute; overflow: hidden; left: 0px; top: 0px; width: 1900px; height: 520px;">
                        <div><asp:Image u="image" ID="Image3" ImageUrl="img/image1.jpg" runat="server" /></div>
                        <div><asp:Image u="image" ID="Image4" ImageUrl="img/image2.jpg" runat="server" /></div>
                        <div><asp:Image u="image" ID="Image5" ImageUrl="img/image3.jpg" runat="server" /></div>
                        <div><asp:Image u="image" ID="Image6" ImageUrl="img/image4.jpg" runat="server" /></div>
                        <div><asp:Image u="image" ID="Image7" ImageUrl="img/image5.jpg" runat="server" /></div>
                    </div>
                    <!-- Arrow Left -->
                    <span u="arrowleft" class="jssora01l" style="top: 123px; left: 8px;"></span>
                    <!-- Arrow Right -->
                    <span u="arrowright" class="jssora01r" style="top: 123px; right: 8px;"></span>
                </div>
            </div>
            
            <section class="container-fluid secion">
                <div class="container">
                    <div class="row text-center">
                        <div class="col-md-3">
                            <div class="front-icon">
                                <i class="icon-cubes"></i>
                            </div>
                            <h4 class="text-uppercase"><asp:Label ID="Label2" runat="server" Text="Replenishment & Cash"></asp:Label></h4>
                            <div class="service-title-sep"></div>
                            <p><asp:Label ID="Label6" runat="server" Text="Manages replenishment fast and easily with in-built workflow management."></asp:Label></p>
                            <p><asp:Label ID="Label7" runat="server" Text="Paper journal transactions and alerts are available online."></asp:Label></p>
                            <p><asp:Label ID="Label8" runat="server" Text="Replenishment by Branch personnel or by an external company."></asp:Label></p>
                            <p><asp:Label ID="Label9" runat="server" Text="Forecast cash requirement is available based on statistical analysis of historical data with the possibility of applying ad hoc and systematic adjustments for specific future requirements."></asp:Label></p>
                            <p><asp:Label ID="Label10" runat="server" Text="Up to four currencies per ATM."></asp:Label></p>
                        </div>
                        <div class="col-md-3">
                             <div class="front-icon">
                                <i class="icon-chart-pie"></i>
                            </div>
                            <h4 class="text-uppercase"><asp:Label ID="Label3" runat="server" Text="Reconciliation"></asp:Label></h4>
                            <div class="service-title-sep"></div>
                            <p><asp:Label ID="Label11" runat="server" Text="Delivers a single screen to provide the user with an overview of the reconciliation state."></asp:Label></p>
                            <p><asp:Label ID="Label12" runat="server" Text="Reconciliation for both ATM and back-end systems."></asp:Label></p>
                            <p><asp:Label ID="Label13" runat="server" Text="Reconciliation can be done for one or many ATMs simultaneously."></asp:Label></p>
                            <p><asp:Label ID="Label14" runat="server" Text="Reconciliation can be centralized or decentralized."></asp:Label></p>
                            <p><asp:Label ID="Label15" runat="server" Text="Solutions to reconciliation differences are proposed by the system."></asp:Label></p>
                            <p><asp:Label ID="Label16" runat="server" Text="Transactions created automatically."></asp:Label></p>
                        </div>
                        <div class="col-md-3">
                             <div class="front-icon">
                                <i class="icon-hammer"></i>
                            </div>
                            <h4 class="text-uppercase"><asp:Label ID="Label4" runat="server" Text="Disputes"></asp:Label></h4>
                            <div class="service-title-sep"></div>
                            <p><asp:Label ID="Label17" runat="server" Text="Disputes are reduced due to proactive and exact actions taken by personnel during Replenishment and Reconciliation."></asp:Label></p>
                            <p><asp:Label ID="Label18" runat="server" Text="All past information is readily available centrally or at branch level including errors and customer transaction video clips."></asp:Label></p>
                            <p><asp:Label ID="Label19" runat="server" Text="Automatically creates cases for incident management based on the available information, following a cardholder claim."></asp:Label></p>
                        </div>
                        <div class="col-md-3">
                             <div class="front-icon">
                                <i class="icon-users"></i>
                            </div>
                            <h4 class="text-uppercase"><asp:Label ID="Label5" runat="server" Text="Management"></asp:Label></h4>
                            <div class="service-title-sep"></div>
                            <p><asp:Label ID="Label20" runat="server" Text="Centrally controlled replenishment , reconciliation and disputes."></asp:Label></p>
                            <p><asp:Label ID="Label21" runat="server" Text="An MIS tool that includes ATM operational statistics, cash utilization, profitability."></asp:Label></p>
                            <p><asp:Label ID="Label22" runat="server" Text="Personnel or Cash In Transit companies performance monitoring."></asp:Label></p>
                            <p><asp:Label ID="Label23" runat="server" Text="Extensive reporting, including audit trail on open incidents, actions taken."></asp:Label></p>
                            <p><asp:Label ID="Label24" runat="server" Text="Reporting of deviation from quality benchmarks set by management."></asp:Label></p>
                        </div>
                    </div>
                </div>
            </section>

            <section class="container-fluid content">
                <div class="container">
                    <div class="row">
                        <div class="col-md-6 rrdm_modules_img">
                            <asp:Image ID="Image2" CssClass="img center-block img-responsive" ImageUrl="img/rrdm-modules.png" runat="server"></asp:Image>
                        </div>
                        <div class="col-md-6">
                            <h3><asp:Label ID="Label25" runat="server" Text="Replenishment Reconciliation and Dispute Management For ATMs"></asp:Label></h3>
                            <p><asp:Label ID="Label26" runat="server" Text="RRDM is an innovative and truly integrated solution designed to redress the many issues faced by banking professionals required to work with disparate systems for their ATM operation and management."></asp:Label></p>
                            <p><asp:Label ID="Label27" runat="server" Text="The system implements a systematic approach to considerably improve hitherto manual, as well as ad hoc processes of managing the workflows of:"></asp:Label></p>
                            <ul>
                                <li><asp:Label ID="Label29" runat="server" Text="ATM replenishment"></asp:Label></li>
                                <li><asp:Label ID="Label30" runat="server" Text="ATM reconciliation"></asp:Label></li>
                                <li><asp:Label ID="Label31" runat="server" Text="Dispute Management"></asp:Label></li>
                                <li><asp:Label ID="Label32" runat="server" Text="Dispute Management based on current and past information"></asp:Label></li>
                                <li><asp:Label ID="Label33" runat="server" Text="Capture Cards Management"></asp:Label></li>
                                <li><asp:Label ID="Label34" runat="server" Text="Cash in ATM management"></asp:Label></li>
                                <li><asp:Label ID="Label35" runat="server" Text="Cash In Transit (CIT) cash statement"></asp:Label></li>
                                <li><asp:Label ID="Label36" runat="server" Text="MIS including ATM profitability and personnel performance"></asp:Label></li>
                            </ul>
                            <p><asp:Label ID="Label28" runat="server" Text="The comprehensive, user-friendly and intuitive interface guides users through the necessary steps required to reconcile both the ATM and host general ledger files. A single screen provides the operator with an overview of the reconciliation, automatically identifies differences and proposes actions to be taken to resolve them."></asp:Label></p>
                        </div>
                    </div>
                </div>
            </section>

            <footer class="container-fluid footer">
                <div class="container">
                    <div class="row">
                        <div class="col-md-6"></div>
                        <div class="col-md-6 copyright text-right">
                            <asp:Label ID="Label1" runat="server" Text="© RRDM Solutions. All Rights Reserved"></asp:Label>
                        </div>
                    </div>
                </div>
            </footer>

        </form>

    </body>
</html>
