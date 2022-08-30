using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Xml;
using fyiReporting.RDL;


namespace fyiReporting.RdlDesign
{
	/// <summary>
	/// Summary description for DialogDataSourceRef.
	/// </summary>
	internal class DialogNewChart : System.Windows.Forms.Form
	{
		private DesignXmlDraw _Draw;
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbDataSets;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox lbFields;
		private System.Windows.Forms.ListBox lbChartCategories;
		private System.Windows.Forms.Button bCategoryUp;
		private System.Windows.Forms.Button bCategoryDown;
		private System.Windows.Forms.Button bCategory;
		private System.Windows.Forms.Button bSeries;
		private System.Windows.Forms.ListBox lbChartSeries;
		private System.Windows.Forms.Button bCategoryDelete;
		private System.Windows.Forms.Button bSeriesDelete;
		private System.Windows.Forms.Button bSeriesDown;
		private System.Windows.Forms.Button bSeriesUp;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cbChartData;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox cbSubType;
		private System.Windows.Forms.ComboBox cbChartType;
		private System.Windows.Forms.Label label7;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal DialogNewChart(DesignXmlDraw dxDraw, XmlNode container)
		{
			_Draw = dxDraw;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			InitValues(container);
		}

		private void InitValues(XmlNode container)
		{
			this.bOK.Enabled = false;		
			//
			// Obtain the existing DataSets info
			//
			object[] datasets = _Draw.DataSetNames;
			if (datasets == null)
				return;		// not much to do if no DataSets

			if (_Draw.IsDataRegion(container))
			{
				string s = _Draw.GetDataSetNameValue(container);
				if (s == null)
					return;
				this.cbDataSets.Items.Add(s);
				this.cbDataSets.Enabled = false;
			}
			else
				this.cbDataSets.Items.AddRange(datasets);
			cbDataSets.SelectedIndex = 0;

			this.cbChartType.SelectedIndex = 2;
		}

		internal string ChartXml
		{
			get 
			{
				StringBuilder chart = new StringBuilder("<Chart><Height>2in</Height><Width>4in</Width>");
				chart.AppendFormat("<DataSetName>{0}</DataSetName>", this.cbDataSets.Text);
				chart.Append("<NoRows>Query returned no rows!</NoRows><Style>"+
					"<BorderStyle><Default>Solid</Default></BorderStyle>"+
					"<BackgroundColor>White</BackgroundColor>"+
					"<BackgroundGradientType>LeftRight</BackgroundGradientType>"+
					"<BackgroundGradientEndColor>Azure</BackgroundGradientEndColor>"+
					"</Style>");
				chart.AppendFormat("<Type>{0}</Type><Subtype>{1}</Subtype>",
					this.cbChartType.Text, this.cbSubType.Text);
				// do the categories
				string tcat="";
				if (this.lbChartCategories.Items.Count > 0)
				{
					chart.Append("<CategoryGroupings>");
					foreach (string cname in this.lbChartCategories.Items)
					{
						if (tcat == "")
							tcat = cname;
						chart.Append("<CategoryGrouping>");
						chart.Append("<DynamicCategories>");
						chart.AppendFormat("<Grouping><GroupExpressions>"+
							"<GroupExpression>=Fields!{0}.Value</GroupExpression>"+
							"</GroupExpressions></Grouping>", cname);

						chart.Append("</DynamicCategories>");
						chart.Append("</CategoryGrouping>");
					}
					chart.Append("</CategoryGroupings>");
					// Do the category axis
					chart.AppendFormat("<CategoryAxis><Axis><Visible>True</Visible>"+
						"<MajorTickMarks>Inside</MajorTickMarks>"+
						"<MajorGridLines><ShowGridLines>True</ShowGridLines>"+
						"<Style><BorderStyle><Default>Solid</Default></BorderStyle>"+
						"</Style></MajorGridLines>" +
						"<MinorGridLines><ShowGridLines>True</ShowGridLines>"+
						"<Style><BorderStyle><Default>Solid</Default></BorderStyle>"+
						"</Style></MinorGridLines>"+
			            "<Title><Caption>{0}</Caption>"+
						"</Title></Axis></CategoryAxis>",tcat);

				}
				// do the series
				string	tser="";
				if (this.lbChartSeries.Items.Count > 0)
				{
					chart.Append("<SeriesGroupings>");
					foreach (string sname in this.lbChartSeries.Items)
					{
						if (tser == "")
							tser = sname;
						chart.Append("<SeriesGrouping>");
						chart.Append("<DynamicSeries>");
						chart.AppendFormat("<Grouping><GroupExpressions>"+
							"<GroupExpression>=Fields!{0}.Value</GroupExpression>"+
							"</GroupExpressions></Grouping>", sname);
						chart.AppendFormat("<Label>=Fields!{0}.Value</Label>", sname);
						chart.Append("</DynamicSeries>");
						chart.Append("</SeriesGrouping>");
					}
					chart.Append("</SeriesGroupings>");
				}
				// Chart Data
				if (this.cbChartData.Text.Length > 0)
				{
					chart.AppendFormat("<ChartData><ChartSeries><DataPoints><DataPoint>"+
						"<DataValues><DataValue><Value>{0}</Value></DataValue></DataValues>"+
						"</DataPoint></DataPoints></ChartSeries></ChartData>", 
						this.cbChartData.Text);
					// Do the value axis
					string vtitle;
					int start = this.cbChartData.Text.LastIndexOf("!");
					if (start > 0)
					{
						int end = this.cbChartData.Text.LastIndexOf(".Value");
						if (end < 0 || end <= start+1)
							vtitle = this.cbChartData.Text.Substring(start+1);
						else
							vtitle = this.cbChartData.Text.Substring(start+1, end-start-1);
					}
					else 
						vtitle = "Values";
					chart.AppendFormat("<ValueAxis><Axis><Visible>True</Visible>"+
						"<MajorTickMarks>Inside</MajorTickMarks>"+
						"<MajorGridLines><ShowGridLines>True</ShowGridLines>"+
						"<Style><BorderStyle><Default>Solid</Default></BorderStyle>"+
						"<FontSize>8pt</FontSize>"+
						"</Style></MajorGridLines>" +
						"<MinorGridLines><ShowGridLines>True</ShowGridLines>"+
						"<Style><BorderStyle><Default>Solid</Default></BorderStyle>"+
						"</Style></MinorGridLines>"+
						"<Title><Caption>{0}</Caption>"+
						"<Style><WritingMode>tb-rl</WritingMode></Style>"+
						"</Title></Axis></ValueAxis>",vtitle);
				}
				// Legend
				chart.Append("<Legend><Style><BorderStyle><Default>Solid</Default>"+
					"</BorderStyle><PaddingLeft>5pt</PaddingLeft>"+
					"<FontSize>8pt</FontSize></Style><Visible>True</Visible>"+
					"<Position>RightCenter</Position></Legend>");

				// Title
				chart.AppendFormat("<Title><Style><FontWeight>Bold</FontWeight>"+
					"<FontSize>14pt</FontSize><TextAlign>Center</TextAlign>"+
					"</Style><Caption>{0} {1} Chart</Caption></Title>", tcat, tser);

				// end of Chart defintion
				chart.Append("</Chart>");

				return chart.ToString();
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.bOK = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.cbDataSets = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lbFields = new System.Windows.Forms.ListBox();
			this.lbChartCategories = new System.Windows.Forms.ListBox();
			this.bCategoryUp = new System.Windows.Forms.Button();
			this.bCategoryDown = new System.Windows.Forms.Button();
			this.bCategory = new System.Windows.Forms.Button();
			this.bSeries = new System.Windows.Forms.Button();
			this.lbChartSeries = new System.Windows.Forms.ListBox();
			this.bCategoryDelete = new System.Windows.Forms.Button();
			this.bSeriesDelete = new System.Windows.Forms.Button();
			this.bSeriesDown = new System.Windows.Forms.Button();
			this.bSeriesUp = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.cbChartData = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.cbSubType = new System.Windows.Forms.ComboBox();
			this.cbChartType = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(272, 392);
			this.bOK.Name = "bOK";
			this.bOK.TabIndex = 13;
			this.bOK.Text = "OK";
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// bCancel
			// 
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(368, 392);
			this.bCancel.Name = "bCancel";
			this.bCancel.TabIndex = 14;
			this.bCancel.Text = "Cancel";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 23);
			this.label1.TabIndex = 11;
			this.label1.Text = "DataSet";
			// 
			// cbDataSets
			// 
			this.cbDataSets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbDataSets.Location = new System.Drawing.Point(80, 16);
			this.cbDataSets.Name = "cbDataSets";
			this.cbDataSets.Size = new System.Drawing.Size(360, 21);
			this.cbDataSets.TabIndex = 0;
			this.cbDataSets.SelectedIndexChanged += new System.EventHandler(this.cbDataSets_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 80);
			this.label2.Name = "label2";
			this.label2.TabIndex = 13;
			this.label2.Text = "DataSet Fields";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(224, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(184, 23);
			this.label3.TabIndex = 14;
			this.label3.Text = "Chart Categories (X) Groupings";
			// 
			// lbFields
			// 
			this.lbFields.Location = new System.Drawing.Point(16, 104);
			this.lbFields.Name = "lbFields";
			this.lbFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbFields.Size = new System.Drawing.Size(152, 225);
			this.lbFields.TabIndex = 1;
			// 
			// lbChartCategories
			// 
			this.lbChartCategories.Location = new System.Drawing.Point(232, 104);
			this.lbChartCategories.Name = "lbChartCategories";
			this.lbChartCategories.Size = new System.Drawing.Size(152, 94);
			this.lbChartCategories.TabIndex = 3;
			// 
			// bCategoryUp
			// 
			this.bCategoryUp.Location = new System.Drawing.Point(392, 104);
			this.bCategoryUp.Name = "bCategoryUp";
			this.bCategoryUp.Size = new System.Drawing.Size(48, 24);
			this.bCategoryUp.TabIndex = 4;
			this.bCategoryUp.Text = "Up";
			this.bCategoryUp.Click += new System.EventHandler(this.bCategoryUp_Click);
			// 
			// bCategoryDown
			// 
			this.bCategoryDown.Location = new System.Drawing.Point(392, 136);
			this.bCategoryDown.Name = "bCategoryDown";
			this.bCategoryDown.Size = new System.Drawing.Size(48, 24);
			this.bCategoryDown.TabIndex = 5;
			this.bCategoryDown.Text = "Down";
			this.bCategoryDown.Click += new System.EventHandler(this.bCategoryDown_Click);
			// 
			// bCategory
			// 
			this.bCategory.Location = new System.Drawing.Point(184, 112);
			this.bCategory.Name = "bCategory";
			this.bCategory.Size = new System.Drawing.Size(32, 24);
			this.bCategory.TabIndex = 2;
			this.bCategory.Text = ">";
			this.bCategory.Click += new System.EventHandler(this.bCategory_Click);
			// 
			// bSeries
			// 
			this.bSeries.Location = new System.Drawing.Point(184, 240);
			this.bSeries.Name = "bSeries";
			this.bSeries.Size = new System.Drawing.Size(32, 24);
			this.bSeries.TabIndex = 7;
			this.bSeries.Text = ">";
			this.bSeries.Click += new System.EventHandler(this.bSeries_Click);
			// 
			// lbChartSeries
			// 
			this.lbChartSeries.Location = new System.Drawing.Point(232, 232);
			this.lbChartSeries.Name = "lbChartSeries";
			this.lbChartSeries.Size = new System.Drawing.Size(152, 94);
			this.lbChartSeries.TabIndex = 8;
			// 
			// bCategoryDelete
			// 
			this.bCategoryDelete.Location = new System.Drawing.Point(392, 168);
			this.bCategoryDelete.Name = "bCategoryDelete";
			this.bCategoryDelete.Size = new System.Drawing.Size(48, 24);
			this.bCategoryDelete.TabIndex = 6;
			this.bCategoryDelete.Text = "Delete";
			this.bCategoryDelete.Click += new System.EventHandler(this.bCategoryDelete_Click);
			// 
			// bSeriesDelete
			// 
			this.bSeriesDelete.Location = new System.Drawing.Point(392, 296);
			this.bSeriesDelete.Name = "bSeriesDelete";
			this.bSeriesDelete.Size = new System.Drawing.Size(48, 24);
			this.bSeriesDelete.TabIndex = 11;
			this.bSeriesDelete.Text = "Delete";
			this.bSeriesDelete.Click += new System.EventHandler(this.bSeriesDelete_Click);
			// 
			// bSeriesDown
			// 
			this.bSeriesDown.Location = new System.Drawing.Point(392, 264);
			this.bSeriesDown.Name = "bSeriesDown";
			this.bSeriesDown.Size = new System.Drawing.Size(48, 24);
			this.bSeriesDown.TabIndex = 10;
			this.bSeriesDown.Text = "Down";
			this.bSeriesDown.Click += new System.EventHandler(this.bSeriesDown_Click);
			// 
			// bSeriesUp
			// 
			this.bSeriesUp.Location = new System.Drawing.Point(392, 232);
			this.bSeriesUp.Name = "bSeriesUp";
			this.bSeriesUp.Size = new System.Drawing.Size(48, 24);
			this.bSeriesUp.TabIndex = 9;
			this.bSeriesUp.Text = "Up";
			this.bSeriesUp.Click += new System.EventHandler(this.bSeriesUp_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(224, 208);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(184, 23);
			this.label4.TabIndex = 31;
			this.label4.Text = "Chart Series";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 344);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(120, 23);
			this.label5.TabIndex = 32;
			this.label5.Text = "Chart Cell Expression";
			// 
			// cbChartData
			// 
			this.cbChartData.Location = new System.Drawing.Point(16, 360);
			this.cbChartData.Name = "cbChartData";
			this.cbChartData.Size = new System.Drawing.Size(368, 21);
			this.cbChartData.TabIndex = 12;
			this.cbChartData.TextChanged += new System.EventHandler(this.cbChartData_TextChanged);
			this.cbChartData.Enter += new System.EventHandler(this.cbChartData_Enter);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(232, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(64, 23);
			this.label6.TabIndex = 36;
			this.label6.Text = "Sub-type";
			// 
			// cbSubType
			// 
			this.cbSubType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSubType.Location = new System.Drawing.Point(312, 48);
			this.cbSubType.Name = "cbSubType";
			this.cbSubType.Size = new System.Drawing.Size(80, 21);
			this.cbSubType.TabIndex = 35;
			// 
			// cbChartType
			// 
			this.cbChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbChartType.Items.AddRange(new object[] {
															 "Area",
															 "Bar",
															 "Column",
															 "Doughnut",
															 "Line",
															 "Pie"});
			this.cbChartType.Location = new System.Drawing.Point(96, 48);
			this.cbChartType.Name = "cbChartType";
			this.cbChartType.Size = new System.Drawing.Size(121, 21);
			this.cbChartType.TabIndex = 34;
			this.cbChartType.SelectedIndexChanged += new System.EventHandler(this.cbChartType_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(16, 48);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(72, 16);
			this.label7.TabIndex = 33;
			this.label7.Text = "Chart Type";
			// 
			// DialogNewChart
			// 
			this.AcceptButton = this.bOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(456, 424);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.cbSubType);
			this.Controls.Add(this.cbChartType);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.cbChartData);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.bSeriesDelete);
			this.Controls.Add(this.bSeriesDown);
			this.Controls.Add(this.bSeriesUp);
			this.Controls.Add(this.bCategoryDelete);
			this.Controls.Add(this.lbChartSeries);
			this.Controls.Add(this.bSeries);
			this.Controls.Add(this.bCategory);
			this.Controls.Add(this.bCategoryDown);
			this.Controls.Add(this.bCategoryUp);
			this.Controls.Add(this.lbChartCategories);
			this.Controls.Add(this.lbFields);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cbDataSets);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DialogNewChart";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Chart";
			this.ResumeLayout(false);

		}
		#endregion

		private void bOK_Click(object sender, System.EventArgs e)
		{
			// apply the result
			DialogResult = DialogResult.OK;
		}

		private void cbDataSets_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.lbChartCategories.Items.Clear();
			this.lbChartSeries.Items.Clear();
			bOK.Enabled = false;
			this.lbFields.Items.Clear();
			string [] fields = _Draw.GetFields(cbDataSets.Text, false);
			if (fields != null)
				lbFields.Items.AddRange(fields);
		}

		private void bCategory_Click(object sender, System.EventArgs e)
		{
			ICollection sic = lbFields.SelectedIndices;
			int count=sic.Count;
			foreach (int i in sic)
			{
				string fname = (string) lbFields.Items[i];
				if (this.lbChartCategories.Items.IndexOf(fname) < 0)
					lbChartCategories.Items.Add(fname);
			}
			OkEnable();
		}

		private void bSeries_Click(object sender, System.EventArgs e)
		{
			ICollection sic = lbFields.SelectedIndices;
			int count=sic.Count;
			foreach (int i in sic)
			{
				string fname = (string) lbFields.Items[i];
				if (this.lbChartSeries.Items.IndexOf(fname) < 0)
					lbChartSeries.Items.Add(fname);
			}
			OkEnable();
		}

		private void bCategoryUp_Click(object sender, System.EventArgs e)
		{
			int index = lbChartCategories.SelectedIndex;
			if (index <= 0)
				return;

			string prename = (string) lbChartCategories.Items[index-1];
			lbChartCategories.Items.RemoveAt(index-1);
			lbChartCategories.Items.Insert(index, prename);
		}

		private void bCategoryDown_Click(object sender, System.EventArgs e)
		{
			int index = lbChartCategories.SelectedIndex;
			if (index < 0 || index + 1 == lbChartCategories.Items.Count)
				return;

			string postname = (string) lbChartCategories.Items[index+1];
			lbChartCategories.Items.RemoveAt(index+1);
			lbChartCategories.Items.Insert(index, postname);
		}

		private void bCategoryDelete_Click(object sender, System.EventArgs e)
		{
			int index = lbChartCategories.SelectedIndex;
			if (index < 0)
				return;

			lbChartCategories.Items.RemoveAt(index);
			OkEnable();
		}

		private void bSeriesUp_Click(object sender, System.EventArgs e)
		{
			int index = lbChartSeries.SelectedIndex;
			if (index <= 0)
				return;

			string prename = (string) lbChartSeries.Items[index-1];
			lbChartSeries.Items.RemoveAt(index-1);
			lbChartSeries.Items.Insert(index, prename);
		}

		private void bSeriesDown_Click(object sender, System.EventArgs e)
		{
			int index = lbChartSeries.SelectedIndex;
			if (index < 0 || index + 1 == lbChartSeries.Items.Count)
				return;

			string postname = (string) lbChartSeries.Items[index+1];
			lbChartSeries.Items.RemoveAt(index+1);
			lbChartSeries.Items.Insert(index, postname);
		}

		private void bSeriesDelete_Click(object sender, System.EventArgs e)
		{
			int index = lbChartSeries.SelectedIndex;
			if (index < 0)
				return;

			lbChartSeries.Items.RemoveAt(index);
			OkEnable();
		}

		private void OkEnable()
		{
			// We need values in datasets and Categories or Series for OK to work correctly
			bOK.Enabled = (this.lbChartCategories.Items.Count > 0 ||
						  this.lbChartSeries.Items.Count > 0) && 
						this.cbDataSets.Text != null &&
						this.cbDataSets.Text.Length > 0;
		}

		private void cbChartData_Enter(object sender, System.EventArgs e)
		{
			cbChartData.Items.Clear();
			foreach (string field in this.lbFields.Items)
			{
				if (this.lbChartCategories.Items.IndexOf(field) >= 0 ||
					this.lbChartSeries.Items.IndexOf(field) >= 0)
					continue;
				// Field selected in columns and rows
				this.cbChartData.Items.Add(string.Format("=Sum(Fields!{0}.Value)", field));
			}
		}

		private void cbChartData_TextChanged(object sender, System.EventArgs e)
		{
			OkEnable();
		}

		private void cbChartType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// Change the potential sub-types
			string savesub = cbSubType.Text;
			string[] subItems;
			switch (cbChartType.Text)
			{
				case "Column":
					subItems = new string [] {"Plain", "Stacked", "PercentStacked"};
					break;
				case "Bar":
					subItems = new string [] {"Plain", "Stacked", "PercentStacked"};
					break;
				case "Line":
					subItems = new string [] {"Plain", "Smooth"};
					break;
				case "Pie":
					subItems = new string [] {"Plain", "Exploded"};
					break;
				case "Area":
					subItems = new string [] {"Plain", "Stacked"};
					break;
				case "Doughnut":
					subItems = new string [] {"Plain"};
					break;
				case "Scatter":
					subItems = new string [] {"Plain", "Line", "SmoothLine"};
					break;
				case "Stock":
				case "Bubble":
				default:
					subItems = new string [] {"Plain"};
					break;
			}
			cbSubType.Items.Clear();
			cbSubType.Items.AddRange(subItems);
			int i=0;
			foreach (string s in subItems)
			{
				if (s == savesub)
				{
					cbSubType.SelectedIndex = i;
					return;
				}
				i++;
			}
			// Didn't match old style
			cbSubType.SelectedIndex = 0;
		}
	}
}
