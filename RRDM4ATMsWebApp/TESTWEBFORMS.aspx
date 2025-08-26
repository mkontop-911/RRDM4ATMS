<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TESTWEBFORMS.aspx.cs" Inherits="TESTWEBFORMS" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="height: 647px">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; THIS IS A TEST FORM<br />
            <br />
            <asp:Label ID="Label1" runat="server" Text="FIRST TEXT BOX"></asp:Label>
            <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            <br />
            <br />
            <br />

            <asp:Label ID="Label2" runat="server" Text="SECOND TEXT BOX"></asp:Label>
            <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
            <br />
            <div class="col-md-4">
                <div class="radio">
                    <label>
                        <asp:RadioButton ID="RadioButton1" runat="server" GroupName="mySelection" />
                        <asp:Label ID="Label31" runat="server" Text="Trace No"></asp:Label>
                    </label>
                </div>
                <div class="radio">
                    <label>
                        <asp:RadioButton ID="RadioButton2" runat="server" GroupName="mySelection" />
                        <asp:Label ID="Label32" runat="server" Text="Reference Number"></asp:Label>
                    </label>
                </div>
                <div class="radio">
                    <label>
                        <asp:RadioButton ID="RadioButton3" runat="server" GroupName="mySelection" />
                        <asp:Label ID="Label33" runat="server" Text="ATM NO"></asp:Label>
                    </label>
                </div>
            </div>
            <input type="text" value="9/23/2009" style="width: 100px;" readonly="readonly" name="Date" id="Date" class="hasDatepicker" />
            Enter a date before 1980-01-01:
  <input />
            type="date" name="bday" max="1979-12-31"><br />
            Enter a date after 2000-01-01:
  <input />
            type="date" name="bday" min="2000-01-02"><br />
        </div>
    </form>
</body>
</html>
