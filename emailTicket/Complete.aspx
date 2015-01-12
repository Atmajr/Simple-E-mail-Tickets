<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Complete.aspx.cs" Inherits="emailTicket.Complete" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ticket Complete</title>
    <link rel="stylesheet" type="text/css" href="index.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <% string emailExists = Request.QueryString["emailExists"]; %>
        <div class="label">Thank you for using the Tech Support Online Support System!<br /> <br />
            <% if (emailExists == "True")
               { %>
                   You have been e-mailed a receipt of this ticket. <br />
             <%  } %>
            A support representative will contact you within one business day.
        </div>
        <div class="homelink"><a href="http://your.tech.company.link.here/" target="_top">Back to TechSupport Home Page</a></div>
    </div>
    </form>
</body>
</html>
