<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RRDM4ATMsWeb._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper" >
            <hgroup class="title">
                <h1>&nbsp;</h1>
                <h2>Welcome to RRDM 4 ATMs application </h2>
            </hgroup>
            <p>
                To learn more about the system visit www,rrdmsolutions.com.
                Also refer to user and system manuals.
            </p>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h3>The System covers:</h3>
    <ol class="round">
        <li class="one">
            <h5>Registration of ATMs and Users</h5>
            ATMs are registered using simple to be used functionality.&nbsp;
        </li>
        <li class="two">
            <h5>ATMs replenishment </h5>
            Systematic approach is be used based on a work flow of five steps
        </li>
        <li class="three">
            <h5>ATMs reconciliation </h5>
            In a workflow approach decisions and actions are taken to avoid disputes
        </li>
         <li class="four">
            <h5>ATMs Dispute Management </h5>
             All past information including errors and actions are readily available to resolve the disputes in no time&nbsp;
        </li>
        <li class="five">
            <h5>ATMs reporting and MIS </h5>
            Operational and MIS reporting is available based on daily historical information
        </li>
    </ol>
    <h5>&nbsp;</h5>
&nbsp;
</asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="HeadContent">
</asp:Content>

