<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CS.aspx.cs" Inherits="_Default"
    EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body
        {
            font-family: Arial;
            font-size: 10pt;
        }
        td
        {
            cursor: pointer;
        }
        .hover_row
        {
            background-color: #FFFFBF;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:GridView ID="GridView1" HeaderStyle-BackColor="#3AC0F2" HeaderStyle-ForeColor="White"
        runat="server" AutoGenerateColumns="false" OnRowDataBound="OnRowDataBound" OnSelectedIndexChanged="OnSelectedIndexChanged">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-Width="150" />
            <asp:BoundField DataField="Description" HeaderText="Description" ItemStyle-Width="150" />
        </Columns>
    </asp:GridView>
    <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("[id*=GridView1] td").hover(function () {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function () {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
    </form>
</body>
</html>
