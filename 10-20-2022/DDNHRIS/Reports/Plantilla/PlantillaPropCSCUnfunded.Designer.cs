namespace DDNHRIS.Reports.Plantilla
{
    partial class PlantillaPropCSCUnfunded
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.Drawing.FormattingRule formattingRule1 = new Telerik.Reporting.Drawing.FormattingRule();
            Telerik.Reporting.Drawing.FormattingRule formattingRule2 = new Telerik.Reporting.Drawing.FormattingRule();
            Telerik.Reporting.Drawing.FormattingRule formattingRule3 = new Telerik.Reporting.Drawing.FormattingRule();
            Telerik.Reporting.Drawing.FormattingRule formattingRule4 = new Telerik.Reporting.Drawing.FormattingRule();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlantillaPropCSCUnfunded));
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group2 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group3 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group4 = new Telerik.Reporting.Group();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.groupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.textBox56 = new Telerik.Reporting.TextBox();
            this.groupFooterSection1 = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection1 = new Telerik.Reporting.GroupHeaderSection();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.groupFooterSection2 = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection2 = new Telerik.Reporting.GroupHeaderSection();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.groupFooterSection3 = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection3 = new Telerik.Reporting.GroupHeaderSection();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox22 = new Telerik.Reporting.TextBox();
            this.textBox23 = new Telerik.Reporting.TextBox();
            this.textBox24 = new Telerik.Reporting.TextBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.textBox26 = new Telerik.Reporting.TextBox();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.textBox30 = new Telerik.Reporting.TextBox();
            this.textBox31 = new Telerik.Reporting.TextBox();
            this.textBox32 = new Telerik.Reporting.TextBox();
            this.textBox34 = new Telerik.Reporting.TextBox();
            this.textBox36 = new Telerik.Reporting.TextBox();
            this.textBox37 = new Telerik.Reporting.TextBox();
            this.textBox72 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox50 = new Telerik.Reporting.TextBox();
            this.textBox51 = new Telerik.Reporting.TextBox();
            this.textBox29 = new Telerik.Reporting.TextBox();
            this.textBox52 = new Telerik.Reporting.TextBox();
            this.textBox53 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox41 = new Telerik.Reporting.TextBox();
            this.textBox44 = new Telerik.Reporting.TextBox();
            this.textBox55 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox38 = new Telerik.Reporting.TextBox();
            this.textBox39 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox60 = new Telerik.Reporting.TextBox();
            this.textBox46 = new Telerik.Reporting.TextBox();
            this.textBox47 = new Telerik.Reporting.TextBox();
            this.sqlDataSource1 = new Telerik.Reporting.SqlDataSource();
            this.reportFooterSection1 = new Telerik.Reporting.ReportFooterSection();
            this.textBox61 = new Telerik.Reporting.TextBox();
            this.textBox64 = new Telerik.Reporting.TextBox();
            this.textBox65 = new Telerik.Reporting.TextBox();
            this.textBox69 = new Telerik.Reporting.TextBox();
            this.textBox70 = new Telerik.Reporting.TextBox();
            this.textBox71 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox42 = new Telerik.Reporting.TextBox();
            this.textBox43 = new Telerik.Reporting.TextBox();
            this.textBox49 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // groupFooterSection
            // 
            this.groupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.29272755980491638D);
            this.groupFooterSection.Name = "groupFooterSection";
            this.groupFooterSection.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.groupFooterSection.Style.Visible = true;
            // 
            // groupHeaderSection
            // 
            formattingRule1.Filters.Add(new Telerik.Reporting.Filter("=Fields.clusterName", Telerik.Reporting.FilterOperator.NotEqual, "Null"));
            formattingRule1.Style.Font.Bold = true;
            formattingRule1.Style.Font.Name = "Arial";
            formattingRule1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            formattingRule1.Style.Visible = true;
            this.groupHeaderSection.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule1});
            this.groupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.18848641216754913D);
            this.groupHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox56});
            this.groupHeaderSection.KeepTogether = false;
            this.groupHeaderSection.Name = "groupHeaderSection";
            this.groupHeaderSection.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.groupHeaderSection.Style.Font.Bold = true;
            this.groupHeaderSection.Style.Visible = false;
            // 
            // textBox56
            // 
            this.textBox56.CanShrink = true;
            this.textBox56.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.00010012308484874666D), Telerik.Reporting.Drawing.Unit.Cm(0.019439497962594032D));
            this.textBox56.Name = "textBox56";
            this.textBox56.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(29.717899322509766D), Telerik.Reporting.Drawing.Unit.Cm(0.45719996094703674D));
            this.textBox56.Style.Font.Bold = true;
            this.textBox56.Style.Font.Name = "Arial";
            this.textBox56.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox56.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox56.Value = "=Fields.clusterName";
            // 
            // groupFooterSection1
            // 
            this.groupFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D);
            this.groupFooterSection1.Name = "groupFooterSection1";
            this.groupFooterSection1.Style.Visible = false;
            // 
            // groupHeaderSection1
            // 
            formattingRule2.Filters.Add(new Telerik.Reporting.Filter("=Fields.divisionName", Telerik.Reporting.FilterOperator.NotEqual, "Null"));
            formattingRule2.Style.Font.Bold = true;
            formattingRule2.Style.Font.Name = "Arial";
            formattingRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            formattingRule2.Style.Visible = true;
            this.groupHeaderSection1.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule2});
            this.groupHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.19888898730278015D);
            this.groupHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox4});
            this.groupHeaderSection1.KeepTogether = false;
            this.groupHeaderSection1.Name = "groupHeaderSection1";
            this.groupHeaderSection1.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.groupHeaderSection1.Style.Visible = false;
            // 
            // textBox4
            // 
            this.textBox4.CanShrink = true;
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.012222184799611569D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.700000762939453D), Telerik.Reporting.Drawing.Unit.Inch(0.17852698266506195D));
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.Style.Font.Name = "Arial";
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox4.Value = "=Fields.divisionName.toUpper()";
            // 
            // groupFooterSection2
            // 
            this.groupFooterSection2.Height = Telerik.Reporting.Drawing.Unit.Inch(0.14787673950195313D);
            this.groupFooterSection2.Name = "groupFooterSection2";
            this.groupFooterSection2.Style.Visible = false;
            // 
            // groupHeaderSection2
            // 
            formattingRule3.Filters.Add(new Telerik.Reporting.Filter("=Fields.sectionName", Telerik.Reporting.FilterOperator.NotEqual, "Null"));
            formattingRule3.Style.Font.Bold = true;
            formattingRule3.Style.Font.Name = "Arial";
            formattingRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            formattingRule3.Style.Visible = true;
            this.groupHeaderSection2.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule3});
            this.groupHeaderSection2.Height = Telerik.Reporting.Drawing.Unit.Inch(0.17537730932235718D);
            this.groupHeaderSection2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox3});
            this.groupHeaderSection2.KeepTogether = false;
            this.groupHeaderSection2.Name = "groupHeaderSection2";
            this.groupHeaderSection2.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.groupHeaderSection2.Style.Visible = false;
            // 
            // textBox3
            // 
            this.textBox3.CanShrink = true;
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.700000762939453D), Telerik.Reporting.Drawing.Unit.Inch(0.15999984741210938D));
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.Font.Name = "Arial";
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.Value = "=Fields.sectionName";
            // 
            // groupFooterSection3
            // 
            this.groupFooterSection3.Height = Telerik.Reporting.Drawing.Unit.Inch(0.099960006773471832D);
            this.groupFooterSection3.Name = "groupFooterSection3";
            this.groupFooterSection3.Style.Visible = false;
            // 
            // groupHeaderSection3
            // 
            formattingRule4.Filters.Add(new Telerik.Reporting.Filter("=Fields.unitName", Telerik.Reporting.FilterOperator.NotEqual, "Null"));
            formattingRule4.Style.Font.Bold = true;
            formattingRule4.Style.Font.Name = "Arial";
            formattingRule4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            formattingRule4.Style.Visible = true;
            this.groupHeaderSection3.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule4});
            this.groupHeaderSection3.Height = Telerik.Reporting.Drawing.Unit.Inch(0.17128923535346985D);
            this.groupHeaderSection3.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox5});
            this.groupHeaderSection3.KeepTogether = false;
            this.groupHeaderSection3.Name = "groupHeaderSection3";
            this.groupHeaderSection3.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.groupHeaderSection3.Style.Font.Bold = true;
            this.groupHeaderSection3.Style.Visible = false;
            // 
            // textBox5
            // 
            this.textBox5.CanShrink = true;
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.013055536895990372D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.700000762939453D), Telerik.Reporting.Drawing.Unit.Inch(0.15795600414276123D));
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.Style.Font.Name = "Arial";
            this.textBox5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox5.Value = "=Fields.unitName";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(2.4669444561004639D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox6,
            this.textBox7,
            this.textBox8,
            this.textBox22,
            this.textBox23,
            this.textBox24,
            this.textBox25,
            this.textBox26,
            this.textBox27,
            this.textBox30,
            this.textBox31,
            this.textBox32,
            this.textBox34,
            this.textBox36,
            this.textBox37,
            this.textBox72,
            this.textBox10,
            this.textBox11,
            this.textBox12,
            this.textBox13,
            this.textBox14,
            this.textBox15,
            this.textBox50,
            this.textBox51,
            this.textBox29,
            this.textBox52,
            this.textBox53,
            this.textBox9,
            this.textBox16});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.096784748136997223D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(12.003215789794922D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox6.Value = "Republic of the Philippines";
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.096784748136997223D), Telerik.Reporting.Drawing.Unit.Inch(0.60000008344650269D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(12.003215789794922D), Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D));
            this.textBox7.Style.Font.Bold = false;
            this.textBox7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox7.Value = "for the Fiscal Year  2022";
            // 
            // textBox8
            // 
            this.textBox8.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.096784748136997223D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(12.003215789794922D), Telerik.Reporting.Drawing.Unit.Inch(0.21041662991046906D));
            this.textBox8.Style.Font.Bold = true;
            this.textBox8.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox8.Value = "PROVINCIAL GOVERNMENT OF DAVAO DEL NORTE";
            // 
            // textBox22
            // 
            this.textBox22.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.026388930156826973D), Telerik.Reporting.Drawing.Unit.Inch(1.3000786304473877D));
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.3235323429107666D), Telerik.Reporting.Drawing.Unit.Inch(1.1289893388748169D));
            this.textBox22.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox22.Style.BorderStyle.Left = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox22.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox22.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox22.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox22.Style.Font.Bold = true;
            this.textBox22.Style.Font.Name = "Arial Narrow";
            this.textBox22.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox22.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox22.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox22.Value = " ITEM NO";
            // 
            // textBox23
            // 
            this.textBox23.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.35000002384185791D), Telerik.Reporting.Drawing.Unit.Inch(1.3000786304473877D));
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6499999761581421D), Telerik.Reporting.Drawing.Unit.Inch(1.1289891004562378D));
            this.textBox23.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox23.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox23.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox23.Style.Font.Bold = true;
            this.textBox23.Style.Font.Name = "Arial Narrow";
            this.textBox23.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox23.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox23.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox23.Value = "POSITION TITLE";
            // 
            // textBox24
            // 
            this.textBox24.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.0000789165496826D), Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D));
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.23749987781047821D), Telerik.Reporting.Drawing.Unit.Inch(1.1290684938430786D));
            this.textBox24.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox24.Style.BorderStyle.Left = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox24.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox24.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox24.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox24.Style.Font.Bold = true;
            this.textBox24.Style.Font.Name = "Arial Narrow";
            this.textBox24.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox24.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox24.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox24.Value = "SG";
            // 
            // textBox25
            // 
            this.textBox25.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.2376575469970703D), Telerik.Reporting.Drawing.Unit.Inch(1.2963128089904785D));
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.68697905540466309D), Telerik.Reporting.Drawing.Unit.Inch(1.1327556371688843D));
            this.textBox25.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox25.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox25.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox25.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Name = "Arial Narrow";
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox25.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox25.Value = "AUTHORIZED SALARY (Monthly)";
            // 
            // textBox26
            // 
            this.textBox26.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.9247157573699951D), Telerik.Reporting.Drawing.Unit.Inch(1.3045027256011963D));
            this.textBox26.Name = "textBox26";
            this.textBox26.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.625884473323822D), Telerik.Reporting.Drawing.Unit.Inch(1.1266975402832031D));
            this.textBox26.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox26.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox26.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox26.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox26.Style.Font.Bold = true;
            this.textBox26.Style.Font.Name = "Arial Narrow";
            this.textBox26.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox26.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox26.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox26.Value = "ACTUAL SALARY (Monthly)";
            // 
            // textBox27
            // 
            this.textBox27.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.5506792068481445D), Telerik.Reporting.Drawing.Unit.Inch(1.2963128089904785D));
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.2139044851064682D), Telerik.Reporting.Drawing.Unit.Inch(1.1326758861541748D));
            this.textBox27.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox27.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox27.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox27.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox27.Style.Font.Bold = true;
            this.textBox27.Style.Font.Name = "Arial Narrow";
            this.textBox27.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox27.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(5D);
            this.textBox27.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(5D);
            this.textBox27.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox27.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox27.Value = "STEP";
            // 
            // textBox30
            // 
            this.textBox30.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.7272415161132812D), Telerik.Reporting.Drawing.Unit.Inch(1.3000801801681519D));
            this.textBox30.Name = "textBox30";
            this.textBox30.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.93734186887741089D), Telerik.Reporting.Drawing.Unit.Inch(1.1303799152374268D));
            this.textBox30.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox30.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox30.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox30.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox30.Style.Font.Bold = true;
            this.textBox30.Style.Font.Name = "Arial Narrow";
            this.textBox30.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox30.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox30.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox30.Value = "LAST NAME";
            // 
            // textBox31
            // 
            this.textBox31.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.6645832061767578D), Telerik.Reporting.Drawing.Unit.Inch(1.3000800609588623D));
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0974719524383545D), Telerik.Reporting.Drawing.Unit.Inch(1.1289874315261841D));
            this.textBox31.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox31.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox31.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox31.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox31.Style.Font.Bold = true;
            this.textBox31.Style.Font.Name = "Arial Narrow";
            this.textBox31.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox31.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox31.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox31.Value = "FIRST NAME";
            // 
            // textBox32
            // 
            this.textBox32.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.7504863739013672D), Telerik.Reporting.Drawing.Unit.Inch(1.3000800609588623D));
            this.textBox32.Name = "textBox32";
            this.textBox32.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.8763883113861084D), Telerik.Reporting.Drawing.Unit.Inch(1.1289874315261841D));
            this.textBox32.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox32.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox32.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox32.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox32.Style.Font.Bold = true;
            this.textBox32.Style.Font.Name = "Arial Narrow";
            this.textBox32.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox32.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox32.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox32.Value = "MIDDLE NAME";
            // 
            // textBox34
            // 
            this.textBox34.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.3328561782836914D), Telerik.Reporting.Drawing.Unit.Inch(1.3000788688659668D));
            this.textBox34.Name = "textBox34";
            this.textBox34.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.58576774597167969D), Telerik.Reporting.Drawing.Unit.Inch(1.1289886236190796D));
            this.textBox34.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox34.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox34.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox34.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox34.Style.Font.Bold = true;
            this.textBox34.Style.Font.Name = "Arial Narrow";
            this.textBox34.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox34.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox34.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox34.Value = "BIRTHDATE";
            // 
            // textBox36
            // 
            this.textBox36.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(9.5600776672363281D), Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D));
            this.textBox36.Name = "textBox36";
            this.textBox36.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.63984429836273193D), Telerik.Reporting.Drawing.Unit.Inch(1.12898850440979D));
            this.textBox36.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox36.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox36.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox36.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox36.Style.Font.Bold = true;
            this.textBox36.Style.Font.Name = "Arial Narrow";
            this.textBox36.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox36.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.textBox36.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.textBox36.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox36.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox36.Value = "DATE OF ORIGINAL APPOINTMENT";
            // 
            // textBox37
            // 
            this.textBox37.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(10.199999809265137D), Telerik.Reporting.Drawing.Unit.Inch(1.3000788688659668D));
            this.textBox37.Name = "textBox37";
            this.textBox37.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.59992176294326782D), Telerik.Reporting.Drawing.Unit.Inch(1.12898850440979D));
            this.textBox37.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox37.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox37.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox37.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox37.Style.Font.Bold = true;
            this.textBox37.Style.Font.Name = "Arial Narrow";
            this.textBox37.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox37.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.textBox37.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.textBox37.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox37.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox37.Value = "DATE OF LAST  PROMOTION";
            // 
            // textBox72
            // 
            this.textBox72.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.096784748136997223D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.textBox72.Name = "textBox72";
            this.textBox72.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(12.006431579589844D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox72.Style.Font.Bold = true;
            this.textBox72.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox72.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox72.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox72.Value = "POSITION CREATED BUT NOT FUNDED";
            // 
            // textBox10
            // 
            this.textBox10.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.026388909667730331D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.7240200042724609D), Telerik.Reporting.Drawing.Unit.Inch(0.299999862909317D));
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox10.Style.Font.Bold = true;
            this.textBox10.Style.Font.Name = "Arial Narrow";
            this.textBox10.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox10.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox10.Value = "=Fields.departmentName";
            // 
            // textBox11
            // 
            this.textBox11.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.7504863739013672D), Telerik.Reporting.Drawing.Unit.Inch(1.0000001192092896D));
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.4370131492614746D), Telerik.Reporting.Drawing.Unit.Inch(0.299999862909317D));
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.BorderStyle.Left = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox11.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.Font.Name = "Arial Narrow";
            this.textBox11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox11.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox11.Value = "PROVINCIAL GOVERNMENT OF DAVAO DEL NORTE";
            // 
            // textBox12
            // 
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.7703127861022949D), Telerik.Reporting.Drawing.Unit.Inch(1.3000801801681519D));
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.42968735098838806D), Telerik.Reporting.Drawing.Unit.Inch(0.29984113574028015D));
            this.textBox12.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox12.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox12.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox12.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox12.Style.Font.Bold = true;
            this.textBox12.Style.Font.Name = "Arial Narrow";
            this.textBox12.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox12.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox12.Value = "AREA";
            // 
            // textBox13
            // 
            this.textBox13.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.7703127861022949D), Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D));
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.21927048265933991D), Telerik.Reporting.Drawing.Unit.Inch(0.82906848192214966D));
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox13.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox13.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox13.Style.Font.Bold = true;
            this.textBox13.Style.Font.Name = "Arial Narrow";
            this.textBox13.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox13.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(5D);
            this.textBox13.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(5D);
            this.textBox13.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox13.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox13.Value = "CODE";
            // 
            // textBox14
            // 
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9896621704101562D), Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.21033795177936554D), Telerik.Reporting.Drawing.Unit.Inch(0.8290674090385437D));
            this.textBox14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox14.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox14.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox14.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox14.Style.Font.Bold = true;
            this.textBox14.Style.Font.Name = "Arial Narrow";
            this.textBox14.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox14.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(5D);
            this.textBox14.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(5D);
            this.textBox14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox14.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox14.Value = "TYPE";
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.2000789642333984D), Telerik.Reporting.Drawing.Unit.Inch(1.3000788688659668D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.26450431346893311D), Telerik.Reporting.Drawing.Unit.Inch(1.1289886236190796D));
            this.textBox15.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox15.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox15.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox15.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox15.Style.Font.Bold = true;
            this.textBox15.Style.Font.Name = "Arial Narrow";
            this.textBox15.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox15.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(6D);
            this.textBox15.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(6D);
            this.textBox15.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox15.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox15.Value = "LEVEL";
            // 
            // textBox50
            // 
            this.textBox50.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.4646625518798828D), Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D));
            this.textBox50.Name = "textBox50";
            this.textBox50.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.2625003457069397D), Telerik.Reporting.Drawing.Unit.Inch(1.1289886236190796D));
            this.textBox50.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox50.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox50.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox50.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox50.Style.Font.Bold = true;
            this.textBox50.Style.Font.Name = "Arial Narrow";
            this.textBox50.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(6D);
            this.textBox50.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox50.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox50.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox50.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox50.Value = "P/P/A ATTRIBUTION";
            // 
            // textBox51
            // 
            this.textBox51.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.626953125D), Telerik.Reporting.Drawing.Unit.Inch(1.3045028448104858D));
            this.textBox51.Name = "textBox51";
            this.textBox51.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.26249948143959045D), Telerik.Reporting.Drawing.Unit.Inch(1.1244862079620361D));
            this.textBox51.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox51.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox51.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox51.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox51.Style.Font.Bold = true;
            this.textBox51.Style.Font.Name = "Arial Narrow";
            this.textBox51.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox51.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox51.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox51.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox51.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox51.Value = "SUFFIX";
            // 
            // textBox29
            // 
            this.textBox29.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.8895306587219238D), Telerik.Reporting.Drawing.Unit.Inch(1.3045027256011963D));
            this.textBox29.Name = "textBox29";
            this.textBox29.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.44324767589569092D), Telerik.Reporting.Drawing.Unit.Inch(1.1244857311248779D));
            this.textBox29.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox29.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox29.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox29.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox29.Style.Font.Bold = true;
            this.textBox29.Style.Font.Name = "Arial Narrow";
            this.textBox29.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox29.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox29.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox29.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox29.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox29.Value = "SEX";
            // 
            // textBox52
            // 
            this.textBox52.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.91869831085205D), Telerik.Reporting.Drawing.Unit.Inch(1.3022117614746094D));
            this.textBox52.Name = "textBox52";
            this.textBox52.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.64130043983459473D), Telerik.Reporting.Drawing.Unit.Inch(1.1289886236190796D));
            this.textBox52.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox52.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox52.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox52.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox52.Style.Font.Bold = true;
            this.textBox52.Style.Font.Name = "Arial Narrow";
            this.textBox52.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox52.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox52.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox52.Value = "TIN";
            // 
            // textBox53
            // 
            this.textBox53.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(10.799999237060547D), Telerik.Reporting.Drawing.Unit.Inch(1.3022117614746094D));
            this.textBox53.Name = "textBox53";
            this.textBox53.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.36000019311904907D), Telerik.Reporting.Drawing.Unit.Inch(1.12898850440979D));
            this.textBox53.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox53.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox53.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox53.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox53.Style.Font.Bold = true;
            this.textBox53.Style.Font.Name = "Arial Narrow";
            this.textBox53.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox53.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.textBox53.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.textBox53.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox53.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox53.Value = "APPOINTMENT STATUS";
            // 
            // textBox9
            // 
            this.textBox9.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(11.160079002380371D), Telerik.Reporting.Drawing.Unit.Inch(1.2963128089904785D));
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.4763132631778717D), Telerik.Reporting.Drawing.Unit.Inch(1.1348874568939209D));
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox9.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox9.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.Style.Font.Name = "Arial Narrow";
            this.textBox9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox9.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.textBox9.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.textBox9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox9.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox9.Value = "CIVIL SERVICE ELIGIBILITY";
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(11.636470794677734D), Telerik.Reporting.Drawing.Unit.Inch(1.2963128089904785D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.55103009939193726D), Telerik.Reporting.Drawing.Unit.Inch(1.1348874568939209D));
            this.textBox16.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox16.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox16.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox16.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Name = "Arial Narrow";
            this.textBox16.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox16.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.textBox16.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.textBox16.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox16.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox16.Value = "VACANT/ NOT VACANT";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.29901334643363953D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2,
            this.textBox41,
            this.textBox44,
            this.textBox55,
            this.textBox1,
            this.textBox17,
            this.textBox38,
            this.textBox39});
            this.detail.Name = "detail";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D), Telerik.Reporting.Drawing.Unit.Inch(0.02496115118265152D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.27405211329460144D));
            this.textBox2.Style.Font.Name = "Arial Narrow";
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox2.Value = "=Iif(Fields.subPositionTitle IS Null, Fields.positionTitle, Fields.positionTitle " +
    "+ \" (\" + Fields.subPositionTitle + \")\")";
            // 
            // textBox41
            // 
            this.textBox41.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.0000789165496826D), Telerik.Reporting.Drawing.Unit.Inch(0.02496115118265152D));
            this.textBox41.Name = "textBox41";
            this.textBox41.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.24992115795612335D), Telerik.Reporting.Drawing.Unit.Inch(0.27405208349227905D));
            this.textBox41.Style.Font.Name = "Arial Narrow";
            this.textBox41.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox41.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox41.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox41.Value = "=Fields.salaryGrade";
            // 
            // textBox44
            // 
            this.textBox44.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.5506792068481445D), Telerik.Reporting.Drawing.Unit.Inch(0.024961259216070175D));
            this.textBox44.Name = "textBox44";
            this.textBox44.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.21390466392040253D), Telerik.Reporting.Drawing.Unit.Inch(0.27405208349227905D));
            this.textBox44.Style.Font.Name = "Arial Narrow";
            this.textBox44.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.textBox44.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox44.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox44.Value = "=Fields.step";
            // 
            // textBox55
            // 
            this.textBox55.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.764662504196167D), Telerik.Reporting.Drawing.Unit.Inch(0.024961259216070175D));
            this.textBox55.Name = "textBox55";
            this.textBox55.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.22492103278636932D), Telerik.Reporting.Drawing.Unit.Inch(0.27405208349227905D));
            this.textBox55.Style.Font.Name = "Arial Narrow";
            this.textBox55.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.textBox55.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox55.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox55.Value = "11";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9896621704101562D), Telerik.Reporting.Drawing.Unit.Inch(0.024961259216070175D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.21033808588981628D), Telerik.Reporting.Drawing.Unit.Inch(0.27405211329460144D));
            this.textBox1.Style.Font.Name = "Arial Narrow";
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.Value = "P";
            // 
            // textBox17
            // 
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.2000789642333984D), Telerik.Reporting.Drawing.Unit.Inch(0.024961156770586967D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.26450446248054504D), Telerik.Reporting.Drawing.Unit.Inch(0.27405211329460144D));
            this.textBox17.Style.Font.Name = "Arial Narrow";
            this.textBox17.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.textBox17.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox17.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox17.Value = "= Fields.keyCSCLevel";
            // 
            // textBox38
            // 
            this.textBox38.Format = "{0:N2}";
            this.textBox38.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.2999997138977051D), Telerik.Reporting.Drawing.Unit.Inch(0.024961259216070175D));
            this.textBox38.Name = "textBox38";
            this.textBox38.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.59963393211364746D), Telerik.Reporting.Drawing.Unit.Inch(0.25405225157737732D));
            this.textBox38.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox38.Style.Font.Name = "Arial Narrow";
            this.textBox38.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.textBox38.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox38.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox38.Value = " ";
            // 
            // textBox39
            // 
            this.textBox39.Format = "{0:N2}";
            this.textBox39.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.9481539726257324D), Telerik.Reporting.Drawing.Unit.Inch(0.024961259216070175D));
            this.textBox39.Name = "textBox39";
            this.textBox39.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.55184608697891235D), Telerik.Reporting.Drawing.Unit.Inch(0.25405228137969971D));
            this.textBox39.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox39.Style.Font.Name = "Arial Narrow";
            this.textBox39.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.textBox39.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox39.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox39.Value = " ";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.24999958276748657D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox60,
            this.textBox46,
            this.textBox47});
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.pageFooterSection1.Style.Visible = true;
            // 
            // textBox60
            // 
            this.textBox60.Format = "{0:000}";
            this.textBox60.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(11.467707633972168D), Telerik.Reporting.Drawing.Unit.Inch(4.9670538793122887E-09D));
            this.textBox60.Name = "textBox60";
            this.textBox60.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.23229344189167023D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox60.Style.Font.Bold = true;
            this.textBox60.Style.Font.Name = "Arial Narrow";
            this.textBox60.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox60.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.textBox60.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.textBox60.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox60.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox60.Value = "= ((Fields.pageNo) + (PageNumber))-1";
            // 
            // textBox46
            // 
            this.textBox46.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(11.049999237060547D), Telerik.Reporting.Drawing.Unit.Inch(0.0020285695791244507D));
            this.textBox46.Name = "textBox46";
            this.textBox46.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.41762885451316833D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox46.Style.Font.Bold = false;
            this.textBox46.Style.Font.Name = "Arial Narrow";
            this.textBox46.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox46.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox46.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox46.Value = "Page ";
            // 
            // textBox47
            // 
            this.textBox47.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(11.699999809265137D), Telerik.Reporting.Drawing.Unit.Inch(0.0020285695791244507D));
            this.textBox47.Name = "textBox47";
            this.textBox47.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.34999847412109375D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox47.Style.Font.Bold = false;
            this.textBox47.Style.Font.Name = "Arial Narrow";
            this.textBox47.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox47.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox47.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox47.Value = "of 192";
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionString = "HRISDB";
            this.sqlDataSource1.Name = "sqlDataSource1";
            this.sqlDataSource1.Parameters.AddRange(new Telerik.Reporting.SqlDataSourceParameter[] {
            new Telerik.Reporting.SqlDataSourceParameter("@paramCode", System.Data.DbType.String, "=Parameters.paramCode.Value")});
            this.sqlDataSource1.SelectCommand = resources.GetString("sqlDataSource1.SelectCommand");
            // 
            // reportFooterSection1
            // 
            this.reportFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.7500003576278687D);
            this.reportFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox61,
            this.textBox64,
            this.textBox65,
            this.textBox69,
            this.textBox70,
            this.textBox71,
            this.textBox18,
            this.textBox19,
            this.textBox42,
            this.textBox43,
            this.textBox49});
            this.reportFooterSection1.Name = "reportFooterSection1";
            this.reportFooterSection1.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            // 
            // textBox61
            // 
            this.textBox61.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.41041699051856995D));
            this.textBox61.Name = "textBox61";
            this.textBox61.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.4597225189208984D), Telerik.Reporting.Drawing.Unit.Inch(0.50301659107208252D));
            this.textBox61.Style.Font.Name = "Arial Narrow";
            this.textBox61.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox61.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox61.Value = resources.GetString("textBox61.Value");
            // 
            // textBox64
            // 
            this.textBox64.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.49999997019767761D), Telerik.Reporting.Drawing.Unit.Inch(1.2200003862380981D));
            this.textBox64.Name = "textBox64";
            this.textBox64.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.8041667938232422D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox64.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox64.Style.Font.Bold = true;
            this.textBox64.Style.Font.Underline = true;
            this.textBox64.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox64.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox64.Value = "EDWIN A. PALERO, MPA, MHRM";
            // 
            // textBox65
            // 
            this.textBox65.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(1.4200791120529175D));
            this.textBox65.Name = "textBox65";
            this.textBox65.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.8041670322418213D), Telerik.Reporting.Drawing.Unit.Inch(0.15677134692668915D));
            this.textBox65.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox65.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox65.Value = "P. G. Department Head";
            // 
            // textBox69
            // 
            this.textBox69.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.8263893127441406D), Telerik.Reporting.Drawing.Unit.Inch(0.41041693091392517D));
            this.textBox69.Name = "textBox69";
            this.textBox69.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox69.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox69.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox69.Value = "APPROVED BY:";
            // 
            // textBox70
            // 
            this.textBox70.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.8263893127441406D), Telerik.Reporting.Drawing.Unit.Inch(1.2200003862380981D));
            this.textBox70.Name = "textBox70";
            this.textBox70.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.8041667938232422D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox70.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox70.Style.Font.Bold = true;
            this.textBox70.Style.Font.Underline = true;
            this.textBox70.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox70.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox70.Value = "EDWIN I. JUBAHIB";
            // 
            // textBox71
            // 
            this.textBox71.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.8263893127441406D), Telerik.Reporting.Drawing.Unit.Inch(1.4075002670288086D));
            this.textBox71.Name = "textBox71";
            this.textBox71.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.8041670322418213D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox71.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox71.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox71.Value = "Governor";
            // 
            // textBox18
            // 
            this.textBox18.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.3042459487915039D), Telerik.Reporting.Drawing.Unit.Inch(1.4075002670288086D));
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89575451612472534D), Telerik.Reporting.Drawing.Unit.Inch(0.22924655675888062D));
            this.textBox18.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox18.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox18.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox18.Value = "Date";
            // 
            // textBox19
            // 
            this.textBox19.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(10.641742706298828D), Telerik.Reporting.Drawing.Unit.Inch(1.3021591901779175D));
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89575451612472534D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox19.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox19.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox19.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox19.Value = "Date";
            // 
            // textBox42
            // 
            this.textBox42.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.45000001788139343D), Telerik.Reporting.Drawing.Unit.Inch(0.050000086426734924D));
            this.textBox42.Name = "textBox42";
            this.textBox42.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox42.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox42.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox42.Value = "Total Number of Items:";
            // 
            // textBox43
            // 
            this.textBox43.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.9500001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.050000086426734924D));
            this.textBox43.Name = "textBox43";
            this.textBox43.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5499999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox43.Style.Font.Bold = true;
            this.textBox43.Style.Font.Underline = true;
            this.textBox43.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox43.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox43.Value = "=Count(Fields.itemNo)";
            // 
            // textBox49
            // 
            this.textBox49.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.50729173421859741D), Telerik.Reporting.Drawing.Unit.Inch(1.5700184106826782D));
            this.textBox49.Name = "textBox49";
            this.textBox49.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.8041670322418213D), Telerik.Reporting.Drawing.Unit.Inch(0.14531262218952179D));
            this.textBox49.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox49.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox49.Value = "PHRMO";
            // 
            // PlantillaPropCSCUnfunded
            // 
            this.DataSource = this.sqlDataSource1;
            group1.GroupFooter = this.groupFooterSection;
            group1.GroupHeader = this.groupHeaderSection;
            group1.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.clusterCode"));
            group1.Name = "Cluster";
            group2.GroupFooter = this.groupFooterSection1;
            group2.GroupHeader = this.groupHeaderSection1;
            group2.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.divisionCode"));
            group2.Name = "Division";
            group3.GroupFooter = this.groupFooterSection2;
            group3.GroupHeader = this.groupHeaderSection2;
            group3.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.sectionCode"));
            group3.Name = "Section";
            group4.GroupFooter = this.groupFooterSection3;
            group4.GroupHeader = this.groupHeaderSection3;
            group4.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.unitCode"));
            group4.Name = "Unit";
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1,
            group2,
            group3,
            group4});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.groupHeaderSection,
            this.groupFooterSection,
            this.groupHeaderSection1,
            this.groupFooterSection1,
            this.groupHeaderSection2,
            this.groupFooterSection2,
            this.groupHeaderSection3,
            this.groupFooterSection3,
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportFooterSection1});
            this.Name = "Plantilla";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.10000000149011612D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Legal;
            reportParameter1.Name = "paramCode";
            reportParameter1.Text = "paramCode";
            reportParameter1.Value = "CSC20221011";
            this.ReportParameters.Add(reportParameter1);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(12.199999809265137D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.SqlDataSource sqlDataSource1;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection;
        private Telerik.Reporting.GroupFooterSection groupFooterSection;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection1;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.GroupFooterSection groupFooterSection1;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.GroupFooterSection groupFooterSection2;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection3;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.GroupFooterSection groupFooterSection3;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox22;
        private Telerik.Reporting.TextBox textBox23;
        private Telerik.Reporting.TextBox textBox24;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.TextBox textBox26;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.TextBox textBox30;
        private Telerik.Reporting.TextBox textBox31;
        private Telerik.Reporting.TextBox textBox32;
        private Telerik.Reporting.TextBox textBox34;
        private Telerik.Reporting.TextBox textBox36;
        private Telerik.Reporting.TextBox textBox37;
        private Telerik.Reporting.TextBox textBox41;
        private Telerik.Reporting.TextBox textBox44;
        private Telerik.Reporting.TextBox textBox55;
        private Telerik.Reporting.TextBox textBox56;
        private Telerik.Reporting.ReportFooterSection reportFooterSection1;
        private Telerik.Reporting.TextBox textBox61;
        private Telerik.Reporting.TextBox textBox64;
        private Telerik.Reporting.TextBox textBox65;
        private Telerik.Reporting.TextBox textBox69;
        private Telerik.Reporting.TextBox textBox70;
        private Telerik.Reporting.TextBox textBox71;
        private Telerik.Reporting.TextBox textBox72;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox38;
        private Telerik.Reporting.TextBox textBox39;
        private Telerik.Reporting.TextBox textBox42;
        private Telerik.Reporting.TextBox textBox43;
        private Telerik.Reporting.TextBox textBox60;
        private Telerik.Reporting.TextBox textBox46;
        private Telerik.Reporting.TextBox textBox47;
        private Telerik.Reporting.TextBox textBox49;
        private Telerik.Reporting.TextBox textBox50;
        private Telerik.Reporting.TextBox textBox51;
        private Telerik.Reporting.TextBox textBox29;
        private Telerik.Reporting.TextBox textBox52;
        private Telerik.Reporting.TextBox textBox53;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox16;
    }
}