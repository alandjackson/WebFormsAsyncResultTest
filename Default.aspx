<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebFormsAsyncResultTest._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            <%= Session["result"] != null ? "CheckStatus();" : "" %>
        });
        function CheckStatus() {
            $("#WaitText").text($("#WaitText").text() + ".");
            <%= ClientScript.GetCallbackEventReference(this,"", "ShowResult", null) %>;
        }
        function ShowResult(eventArgument, context) {
            if (eventArgument == "SUCCESS") {
                $("#Wait").hide();
                $("#DoneSuccess").show();
                window.location.assign("/Default.aspx?GetResult=1");
            } else if (eventArgument == "WORKING") {
                setTimeout("CheckStatus();", 1000);
            } else if (eventArgument == "UNKNOWN") {
                $("#WaitText").text("UNKNOWN STATUS");
            } else {
                $("#Wait").hide();
                $("#DoneErrorMsg").html(eventArgument);
                $("#DoneError").show();
            }
        }
    </script>


</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>
            <span>Do something long: </span>
            <asp:Button runat="server" Text="Start" ID="StartButton" OnClick="StartButton_Click" />
        </div>

        <asp:Panel runat="server" ID="WorkerProgressPanel" Visible="false">

            <div id="Wait">
                <span id="WaitText">Working.</span></div>
            <div id="DoneSuccess" style="display:none;">
                <strong>Success!</strong> Process Finished.  
            </div>
            <div id="DoneError" style="display:none;">
                <strong>Error: </strong> 
                <label id="DoneErrorMsg"></label>
            </div>

        </asp:Panel>
    </div>
    </form>
</body>
</html>
