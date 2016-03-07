<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm152c.aspx.cs" Inherits="RRDM4ATMsWeb.WebForm152c" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">

        .auto-style1 {
            width: 1024px;
            height: 731px;
            position: absolute;
            top: 0px;
            left: 0px;
            z-index: 1;
        }
        .auto-style2 {
            width: 500px;
            height: 211px;
            position: absolute;
            top: 101px;
            left: 14px;
            z-index: 1;
        }
        .auto-style16 {
            position: absolute;
            top: 29px;
            left: 25px;
            z-index: 1;
            width: 89px;
        }
        .auto-style18 {
            position: absolute;
            top: 29px;
            left: 240px;
            z-index: 1;
        }
        .auto-style19 {
            position: absolute;
            top: 29px;
            left: 374px;
            z-index: 1;
        }
        .auto-style26 {
            position: absolute;
            top: 128px;
            left: 123px;
            z-index: 1;
            width: 80px;
        }
        .auto-style20 {
            position: absolute;
            top: 60px;
            left: 24px;
            z-index: 1;
            height: 23px;
            width: 93px;
        }
        .auto-style25 {
            position: absolute;
            top: 96px;
            left: 123px;
            z-index: 1;
            width: 80px;
        }
        .auto-style21 {
            position: absolute;
            top: 94px;
            left: 24px;
            z-index: 1;
            width: 84px;
        }
        .auto-style22 {
            position: absolute;
            top: 127px;
            left: 24px;
            z-index: 1;
            width: 71px;
        }
        .auto-style23 {
            position: absolute;
            top: 159px;
            left: 24px;
            z-index: 1;
            width: 79px;
        }
        .auto-style24 {
            position: absolute;
            top: 61px;
            left: 123px;
            z-index: 1;
            width: 80px;
        }
        .auto-style36 {
            position: absolute;
            top: 161px;
            left: 123px;
            z-index: 1;
            width: 80px;
        }
        .auto-style28 {
            position: absolute;
            top: 61px;
            left: 238px;
            z-index: 1;
            width: 80px;
        }
        .auto-style29 {
            position: absolute;
            top: 96px;
            left: 238px;
            z-index: 1;
            width: 80px;
            right: 203px;
        }
        .auto-style30 {
            position: absolute;
            top: 128px;
            left: 238px;
            z-index: 1;
            width: 80px;
        }
        .auto-style31 {
            position: absolute;
            top: 161px;
            left: 238px;
            z-index: 1;
            width: 80px;
        }
        .auto-style32 {
            position: absolute;
            top: 61px;
            left: 368px;
            z-index: 1;
            width: 80px;
        }
        .auto-style33 {
            position: absolute;
            top: 96px;
            left: 368px;
            z-index: 1;
            width: 80px;
        }
        .auto-style34 {
            position: absolute;
            top: 128px;
            left: 368px;
            z-index: 1;
            width: 80px;
        }
        .auto-style43 {
            position: absolute;
            top: 161px;
            left: 368px;
            z-index: 1;
            width: 80px;
            right: 53px;
        }
        .auto-style55 {
            position: absolute;
            top: 31px;
            left: 131px;
            z-index: 1;
        }
        .auto-style3 {
            width: 1001px;
            height: 123px;
            position: absolute;
            top: 577px;
            left: 14px;
            z-index: 1;
        }
        .auto-style44 {
            position: absolute;
            top: 23px;
            left: 10px;
            z-index: 1;
            width: 289px;
            height: 80px;
        }
        .auto-style45 {
            position: absolute;
            top: 25px;
            left: 450px;
            z-index: 1;
            width: 141px;
            height: 34px;
            right: 410px;
        }
        .auto-style47 {
            position: absolute;
            top: 23px;
            left: 613px;
            z-index: 1;
            width: 104px;
            height: 34px;
            right: 269px;
        }
        .auto-style48 {
            position: absolute;
            top: 64px;
            left: 451px;
            z-index: 1;
            width: 141px;
            height: 27px;
            right: 409px;
        }
        .auto-style49 {
            position: absolute;
            top: 87px;
            left: 731px;
            z-index: 1;
        }
        .auto-style50 {
            position: absolute;
            top: 82px;
            left: 310px;
            z-index: 1;
            right: 518px;
        }
        .auto-style37 {
            width: 500px;
            height: 211px;
            position: absolute;
            top: 351px;
            left: 14px;
            z-index: 1;
        }
        .auto-style27 {
            position: absolute;
            top: 160px;
            left: 123px;
            z-index: 1;
            width: 80px;
        }
        .auto-style35 {
            position: absolute;
            top: 161px;
            left: 368px;
            z-index: 1;
            width: 80px;
        }
        .auto-style38 {
            position: absolute;
            top: 28px;
            left: 128px;
            z-index: 1;
        }
        .auto-style39 {
            width: 475px;
            height: 211px;
            position: absolute;
            top: 353px;
            left: 543px;
            z-index: 1;
        }
        .auto-style46 {
            position: absolute;
            top: 185px;
            left: 31px;
            z-index: 1;
            width: 126px;
        }
        .auto-style40 {
            position: absolute;
            top: 28px;
            left: 127px;
            z-index: 1;
        }
        .auto-style41 {
            width: 475px;
            height: 211px;
            position: absolute;
            top: 101px;
            left: 543px;
            z-index: 1;
        }
        .auto-style42 {
            position: absolute;
            top: 31px;
            left: 129px;
            z-index: 1;
            width: 85px;
        }
        .auto-style51 {
            position: absolute;
            top: 72px;
            left: 14px;
            z-index: 1;
            right: 910px;
        }
        .auto-style52 {
            position: absolute;
            top: 72px;
            left: 546px;
            z-index: 1;
        }
        .auto-style53 {
            position: absolute;
            top: 325px;
            left: 20px;
            z-index: 1;
        }
        .auto-style54 {
            position: absolute;
            top: 325px;
            left: 548px;
            z-index: 1;
            right: 267px;
        }
        .auto-style57 {
            position: absolute;
            top: 662px;
            left: 817px;
            z-index: 1;
            width: 183px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    <asp:Panel ID="Panel1" runat="server" BackColor="#F0F0F0" CssClass="auto-style1">
        <asp:Panel ID="Panel2" runat="server" CssClass="auto-style2" BackColor="White">
            <asp:Label ID="Label1" runat="server" CssClass="auto-style16" Font-Bold="True" Text="Notes "></asp:Label>
            <asp:Label ID="Label3" runat="server" CssClass="auto-style18" Text="Per ATM"></asp:Label>
            <asp:Label ID="Label4" runat="server" CssClass="auto-style19" Text="Difference"></asp:Label>
            <asp:TextBox ID="txtBox3" runat="server" CssClass="auto-style26" height="22px"></asp:TextBox>
            <asp:Label ID="lbel13" runat="server" CssClass="auto-style20" Text="Label"></asp:Label>
            <asp:TextBox ID="txtBox2" runat="server" CssClass="auto-style25" height="22px"></asp:TextBox>
            <asp:Label ID="lbel14" runat="server" CssClass="auto-style21" height="23px" Text="Label"></asp:Label>
            <asp:Label ID="lbel16" runat="server" CssClass="auto-style22" height="23px" Text="Label"></asp:Label>
            <asp:Label ID="lbel17" runat="server" CssClass="auto-style23" height="23px" Text="Label"></asp:Label>
            <asp:TextBox ID="txtBox1" runat="server" CssClass="auto-style24"></asp:TextBox>
            <asp:TextBox ID="txtBox4" runat="server" CssClass="auto-style36" height="22px"></asp:TextBox>
            <asp:TextBox ID="txtBox11" runat="server" CssClass="auto-style28" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox13" runat="server" CssClass="auto-style29" height="22px" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox15" runat="server" CssClass="auto-style30" height="22px" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox17" runat="server" CssClass="auto-style31" height="22px" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox12" runat="server" CssClass="auto-style32" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox14" runat="server" CssClass="auto-style33" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox16" runat="server" CssClass="auto-style34" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox18" runat="server" CssClass="auto-style43" ReadOnly="True"></asp:TextBox>
            <asp:Label ID="Label38" runat="server" CssClass="auto-style55" Text="My Count "></asp:Label>
        </asp:Panel>
        <asp:Panel ID="Panel3" runat="server" BackColor="White" CssClass="auto-style3" ForeColor="#0066FF">
            <asp:TextBox ID="txtMessage" runat="server" CssClass="auto-style44" TextMode="MultiLine"></asp:TextBox>
            <asp:Button ID="ButtonUseATMsFigures" runat="server" BackColor="White" BorderStyle="Solid" CssClass="auto-style45" Font-Bold="True" ForeColor="#0066FF" OnClick="ButtonUseATMsFigures_Click" Text="Use ATM Figures" />
            <asp:Button ID="ButtonUpdate" runat="server" BackColor="White" BorderStyle="Solid" CssClass="auto-style47" Font-Bold="True" ForeColor="#0066FF" OnClick="ButtonUpdate_Click" Text="Update" />
            <asp:Button ID="ButtonErrors" runat="server" BackColor="White" BorderStyle="Solid" CssClass="auto-style48" Font-Bold="True" ForeColor="#0066FF" Text="Errors" OnClick="ButtonErrors_Click" />
            <asp:Button ID="ButtonBack" runat="server" CssClass="auto-style49" Font-Bold="True" ForeColor="#0066FF" OnClick="ButtonBack_Click" Text="&lt;Back" />
            <asp:CheckBox ID="CheckBoxApprove" runat="server" CssClass="auto-style50" Text="Approve" OnCheckedChanged="CheckBoxApprove_CheckedChanged" />
        </asp:Panel>
        <asp:Panel ID="Panel4" runat="server" BackColor="White" CssClass="auto-style37">
            <asp:Label ID="Label9" runat="server" CssClass="auto-style16" Font-Bold="True" Text="Notes "></asp:Label>
            <asp:Label ID="Label10" runat="server" CssClass="auto-style18" Text="Per ATM"></asp:Label>
            <asp:Label ID="Label11" runat="server" CssClass="auto-style19" Text="Difference"></asp:Label>
            <asp:TextBox ID="txtBox7" runat="server" CssClass="auto-style26" height="22px"></asp:TextBox>
            <asp:Label ID="lbel11" runat="server" CssClass="auto-style20" Text="Label"></asp:Label>
            <asp:TextBox ID="txtBox6" runat="server" CssClass="auto-style25" height="22px"></asp:TextBox>
            <asp:Label ID="lbel9" runat="server" CssClass="auto-style21" height="23px" Text="Label"></asp:Label>
            <asp:Label ID="lbel8" runat="server" CssClass="auto-style22" height="23px" Text="Label"></asp:Label>
            <asp:Label ID="lbel7" runat="server" CssClass="auto-style23" height="23px" Text="Label"></asp:Label>
            <asp:TextBox ID="txtBox5" runat="server" CssClass="auto-style24"></asp:TextBox>
            <asp:TextBox ID="TextBox17" runat="server" CssClass="auto-style36" height="22px"></asp:TextBox>
            <asp:TextBox ID="txtBox8" runat="server" CssClass="auto-style27" height="22px"></asp:TextBox>
            <asp:TextBox ID="txtBox22" runat="server" CssClass="auto-style28" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox24" runat="server" CssClass="auto-style29" height="22px" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox26" runat="server" CssClass="auto-style30" height="22px" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox19" runat="server" CssClass="auto-style31" height="22px" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox23" runat="server" CssClass="auto-style32" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox25" runat="server" CssClass="auto-style33" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox27" runat="server" CssClass="auto-style34" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox20" runat="server" CssClass="auto-style35" ReadOnly="True"></asp:TextBox>
            <asp:Label ID="Label24" runat="server" CssClass="auto-style38" Text="My Count"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="Panel5" runat="server" BackColor="White" CssClass="auto-style39">
            <asp:Label ID="Label17" runat="server" CssClass="auto-style16" Font-Bold="True" Text="Notes "></asp:Label>
            <asp:Label ID="Label18" runat="server" CssClass="auto-style18" Text="Per ATM"></asp:Label>
            <asp:Label ID="Label19" runat="server" CssClass="auto-style19" Text="Difference"></asp:Label>
            <asp:TextBox ID="txtBox31" runat="server" CssClass="auto-style26" height="22px"></asp:TextBox>
            <asp:Label ID="lbel29" runat="server" CssClass="auto-style20" Text="Label"></asp:Label>
            <asp:TextBox ID="txtBox30" runat="server" CssClass="auto-style25" height="22px"></asp:TextBox>
            <asp:Label ID="lbel19" runat="server" CssClass="auto-style21" height="23px" Text="Label"></asp:Label>
            <asp:Label ID="lbel3" runat="server" CssClass="auto-style46" Text="Label"></asp:Label>
            <asp:Label ID="lbel25" runat="server" CssClass="auto-style22" height="23px" Text="Label"></asp:Label>
            <asp:Label ID="lbel28" runat="server" CssClass="auto-style23" height="23px" Text="Label"></asp:Label>
            <asp:TextBox ID="txtBox28" runat="server" CssClass="auto-style24"></asp:TextBox>
            <asp:TextBox ID="TextBox30" runat="server" CssClass="auto-style36" height="22px"></asp:TextBox>
            <asp:TextBox ID="txtBox29" runat="server" CssClass="auto-style27" height="22px"></asp:TextBox>
            <asp:TextBox ID="txtBox32" runat="server" CssClass="auto-style28" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox35" runat="server" CssClass="auto-style29" height="22px" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox37" runat="server" CssClass="auto-style30" height="22px" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox39" runat="server" CssClass="auto-style31" height="22px" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox33" runat="server" CssClass="auto-style32" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox36" runat="server" CssClass="auto-style33" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox38" runat="server" CssClass="auto-style34" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox40" runat="server" CssClass="auto-style35" ReadOnly="True"></asp:TextBox>
            <asp:Label ID="Label25" runat="server" CssClass="auto-style40" Text="My Count"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="Panel6" runat="server" BackColor="White" CssClass="auto-style41">
            <asp:Label ID="Label27" runat="server" CssClass="auto-style18" Text="Per ATM"></asp:Label>
            <asp:Label ID="Label28" runat="server" CssClass="auto-style19" Text="Difference"></asp:Label>
            <asp:Label ID="Label29" runat="server" CssClass="auto-style20" Text="Label"></asp:Label>
            <asp:TextBox ID="txtBox34" runat="server" CssClass="auto-style24"></asp:TextBox>
            <asp:TextBox ID="txtBox9" runat="server" CssClass="auto-style28" ReadOnly="True"></asp:TextBox>
            <asp:TextBox ID="txtBox10" runat="server" CssClass="auto-style32" ReadOnly="True"></asp:TextBox>
            <asp:Label ID="Label33" runat="server" CssClass="auto-style42" Text="My Count"></asp:Label>
        </asp:Panel>
        <asp:Label ID="Label34" runat="server" CssClass="auto-style51" Font-Bold="True" Font-Size="Large" ForeColor="#0066FF" Text="CASSETTES"></asp:Label>
        <asp:Label ID="Label35" runat="server" CssClass="auto-style52" Font-Bold="True" Font-Size="Large" ForeColor="#0066FF" Text="CAPTURED CARDS "></asp:Label>
        <asp:Label ID="Label36" runat="server" CssClass="auto-style53" Font-Bold="True" Font-Size="Large" ForeColor="#0066FF" Text="REJECT TRAY "></asp:Label>
        <asp:Label ID="Label37" runat="server" CssClass="auto-style54" Font-Bold="True" Font-Size="Large" ForeColor="#0066FF" Text="MONEY VALUE"></asp:Label>
        <asp:Button ID="Button1" runat="server" BackColor="White" BorderColor="Blue" CssClass="auto-style57" Font-Bold="True" ForeColor="#0066FF" Text="Finish and Print " />
    </asp:Panel>
    
    </div>
    </form>
</body>
</html>
