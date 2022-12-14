<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HRIS.aspx.cs" Inherits="DDNHRIS.Reports.HRIS" %>

<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=7.2.13.1016, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HRIS Report</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <telerik:ReportViewer ID="HRISViewer" runat="server" ViewMode="PrintPreview" Width="100%" Height="8000px"></telerik:ReportViewer>
    </div>
    </form>

    <script type="text/javascript">
            // Overriding the PDF method of report printing
            ReportViewer.prototype.PrintReport = function () {
                switch (this.defaultPrintFormat) {
                    case "Default":
                        this.DefaultPrint();
                        break;
                    case "PDF":
                        this.PrintAs("PDF");
                        previewFrame = document.getElementById(this.previewFrameID);
                        previewFrame.onload = function () {
                            this.contentDocument.execCommand("print", true, null);
                        }
                        break;
                }
            };
        </script>
</body>
</html>
