namespace DDNHRIS.Reports.Profiling
{
    partial class applicationList
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.textBox69 = new Telerik.Reporting.TextBox();
            this.textBox68 = new Telerik.Reporting.TextBox();
            this.textBox67 = new Telerik.Reporting.TextBox();
            this.textBox66 = new Telerik.Reporting.TextBox();
            this.textBox58 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.crosstab1 = new Telerik.Reporting.Crosstab();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.sqlDataApplicant = new Telerik.Reporting.SqlDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.sqlDataPublication = new Telerik.Reporting.SqlDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(18.918975830078125D), Telerik.Reporting.Drawing.Unit.Cm(0.891666829586029D));
            this.textBox3.Style.Font.Name = "Arial Black";
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.Value = "Applicant List";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Cm(2.4999997615814209D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox69,
            this.textBox68,
            this.textBox67,
            this.textBox66,
            this.textBox58,
            this.textBox13});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // textBox69
            // 
            this.textBox69.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.1718491315841675D), Telerik.Reporting.Drawing.Unit.Inch(0.15748034417629242D));
            this.textBox69.Name = "textBox69";
            this.textBox69.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.9921259880065918D), Telerik.Reporting.Drawing.Unit.Inch(0.23622040450572968D));
            this.textBox69.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox69.Style.Font.Bold = true;
            this.textBox69.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox69.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox69.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox69.Value = "= Fields.departmentName";
            // 
            // textBox68
            // 
            this.textBox68.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.1718491315841675D), Telerik.Reporting.Drawing.Unit.Inch(0.39377954602241516D));
            this.textBox68.Name = "textBox68";
            this.textBox68.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.9921259880065918D), Telerik.Reporting.Drawing.Unit.Inch(0.23622040450572968D));
            this.textBox68.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox68.Style.Font.Bold = true;
            this.textBox68.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox68.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox68.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox68.Value = "= Format(\"{0:0000}\", Fields.itemNo)";
            // 
            // textBox67
            // 
            this.textBox67.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.1718491315841675D), Telerik.Reporting.Drawing.Unit.Inch(0.63007885217666626D));
            this.textBox67.Name = "textBox67";
            this.textBox67.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.9921259880065918D), Telerik.Reporting.Drawing.Unit.Inch(0.23622040450572968D));
            this.textBox67.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox67.Style.Font.Bold = true;
            this.textBox67.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox67.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox67.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox67.Value = "= Fields.positionTitle";
            // 
            // textBox66
            // 
            this.textBox66.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.63007873296737671D));
            this.textBox66.Name = "textBox66";
            this.textBox66.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1717307567596436D), Telerik.Reporting.Drawing.Unit.Inch(0.23622040450572968D));
            this.textBox66.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox66.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox66.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox66.Value = "POSITION TITLE:";
            // 
            // textBox58
            // 
            this.textBox58.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.39377948641777039D));
            this.textBox58.Name = "textBox58";
            this.textBox58.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1717307567596436D), Telerik.Reporting.Drawing.Unit.Inch(0.23622040450572968D));
            this.textBox58.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox58.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox58.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox58.Value = "ITEM #:";
            // 
            // textBox13
            // 
            this.textBox13.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.15748034417629242D));
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1717307567596436D), Telerik.Reporting.Drawing.Unit.Inch(0.23622040450572968D));
            this.textBox13.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox13.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox13.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox13.Value = "OFFICE:";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Cm(5.0000004768371582D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.crosstab1});
            this.detail.Name = "detail";
            // 
            // crosstab1
            // 
            this.crosstab1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Cm(18.918975830078125D)));
            this.crosstab1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Cm(1.1033334732055664D)));
            this.crosstab1.Body.SetCellContent(0, 0, this.table1);
            tableGroup5.Groupings.Add(new Telerik.Reporting.Grouping("=\'ColumnGroup\'"));
            tableGroup5.Name = "columnGroup";
            tableGroup5.ReportItem = this.textBox3;
            this.crosstab1.ColumnGroups.Add(tableGroup5);
            this.crosstab1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1,
            this.textBox3});
            this.crosstab1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.00010012308484874666D), Telerik.Reporting.Drawing.Unit.Cm(0.39999997615814209D));
            this.crosstab1.Name = "crosstab1";
            tableGroup6.Groupings.Add(new Telerik.Reporting.Grouping("=\'RowGroup\'"));
            tableGroup6.Name = "rowGroup";
            this.crosstab1.RowGroups.Add(tableGroup6);
            this.crosstab1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(18.918975830078125D), Telerik.Reporting.Drawing.Unit.Cm(1.9950003623962402D));
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Cm(0.88338720798492432D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Cm(7.7293882369995117D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Cm(10.306201934814453D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Cm(1.1033334732055664D)));
            this.table1.Body.SetCellContent(0, 1, this.textBox6);
            this.table1.Body.SetCellContent(0, 2, this.textBox5);
            this.table1.Body.SetCellContent(0, 0, this.textBox2);
            tableGroup1.Name = "group";
            tableGroup2.Name = "tableGroup";
            tableGroup3.Name = "tableGroup1";
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.DataSource = this.sqlDataApplicant;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox6,
            this.textBox5,
            this.textBox2});
            this.table1.Name = "table1";
            tableGroup4.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup4.Name = "detailTableGroup";
            this.table1.RowGroups.Add(tableGroup4);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(18.918975830078125D), Telerik.Reporting.Drawing.Unit.Cm(1.1033334732055664D));
            this.table1.Style.Font.Name = "Arial Black";
            this.table1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.table1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.table1.StyleName = "";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(7.7293872833251953D), Telerik.Reporting.Drawing.Unit.Cm(1.1033334732055664D));
            this.textBox6.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox6.Style.Font.Name = "Arial Black";
            this.textBox6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox6.Value = "= DDNHRIS.Reports.Profiling.applicationList.getName(Fields.applicantCode)";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(10.306201934814453D), Telerik.Reporting.Drawing.Unit.Cm(1.1033334732055664D));
            this.textBox5.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox5.Style.Font.Name = "Arial";
            this.textBox5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox5.StyleName = "";
            this.textBox5.Value = "= IIf(Fields.appTypeCode =1,\" - Insider\",\" - Outsider\")";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(0.88338702917099D), Telerik.Reporting.Drawing.Unit.Cm(1.1033334732055664D));
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox2.StyleName = "";
            this.textBox2.Value = "= RowNumber()+\".\"";
            // 
            // sqlDataApplicant
            // 
            this.sqlDataApplicant.ConnectionString = "HRISDB";
            this.sqlDataApplicant.Name = "sqlDataApplicant";
            this.sqlDataApplicant.Parameters.AddRange(new Telerik.Reporting.SqlDataSourceParameter[] {
            new Telerik.Reporting.SqlDataSourceParameter("@publicationItemCode", System.Data.DbType.String, "=Parameters.publicationItemCode.Value")});
            this.sqlDataApplicant.SelectCommand = "select * from tRSPapplication where publicationItemCode = @publicationItemCode OR" +
    "DER BY appTypeCode DESC ";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Cm(3D);
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // sqlDataPublication
            // 
            this.sqlDataPublication.ConnectionString = "HRISDB";
            this.sqlDataPublication.Name = "sqlDataPublication";
            this.sqlDataPublication.Parameters.AddRange(new Telerik.Reporting.SqlDataSourceParameter[] {
            new Telerik.Reporting.SqlDataSourceParameter("@publicationItemCode", System.Data.DbType.String, "=Parameters.publicationItemCode.Value")});
            this.sqlDataPublication.SelectCommand = "select * from vRSPPublicationItem where publicationItemCode =@publicationItemCode" +
    " ";
            // 
            // applicationList
            // 
            this.DataSource = this.sqlDataPublication;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "applicationList";
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Mm(12.699999809265137D), Telerik.Reporting.Drawing.Unit.Mm(12.699999809265137D), Telerik.Reporting.Drawing.Unit.Mm(12.699999809265137D), Telerik.Reporting.Drawing.Unit.Mm(12.699999809265137D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Legal;
            reportParameter1.Name = "publicationItemCode";
            reportParameter1.Text = "publicationItemCode";
            reportParameter1.Value = "PUB2110271432411642619LL7";
            this.ReportParameters.Add(reportParameter1);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Cm(19.049900054931641D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.Crosstab crosstab1;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.SqlDataSource sqlDataApplicant;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.SqlDataSource sqlDataPublication;
        private Telerik.Reporting.TextBox textBox69;
        private Telerik.Reporting.TextBox textBox68;
        private Telerik.Reporting.TextBox textBox67;
        private Telerik.Reporting.TextBox textBox66;
        private Telerik.Reporting.TextBox textBox58;
        private Telerik.Reporting.TextBox textBox13;
    }
}