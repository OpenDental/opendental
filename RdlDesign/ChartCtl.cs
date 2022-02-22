/* ====================================================================
    Copyright (C) 2004-2006  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    The RDL project is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    For additional information, email info@fyireporting.com or visit
    the website www.fyiReporting.com.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;

namespace fyiReporting.RdlDesign
{
	/// <summary>
	/// Summary description for ChartCtl.
	/// </summary>
	internal class ChartCtl : System.Windows.Forms.UserControl, IProperty
	{
        private List<XmlNode> _ReportItems;
		private DesignXmlDraw _Draw;
		bool fChartType, fSubtype, fPalette, fRenderElement, fPercentWidth;
		bool fNoRows, fDataSet, fPageBreakStart, fPageBreakEnd;
		bool fChartData;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cbChartType;
		private System.Windows.Forms.ComboBox cbSubType;
		private System.Windows.Forms.ComboBox cbPalette;
		private System.Windows.Forms.ComboBox cbRenderElement;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown tbPercentWidth;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbNoRows;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox cbDataSet;
		private System.Windows.Forms.CheckBox chkPageBreakStart;
		private System.Windows.Forms.CheckBox chkPageBreakEnd;
		private System.Windows.Forms.ComboBox cbChartData;
		private System.Windows.Forms.Label label8;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        internal ChartCtl(DesignXmlDraw dxDraw, List<XmlNode> ris)
		{
			_ReportItems = ris;
			_Draw = dxDraw;
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Initialize form using the style node values
			InitValues();			
		}

		private void InitValues()
		{
			XmlNode node = _ReportItems[0];

			this.cbChartType.Text = _Draw.GetElementValue(node, "Type", "Column");
			this.cbSubType.Text = _Draw.GetElementValue(node, "Subtype", "Plain");
			this.cbPalette.Text = _Draw.GetElementValue(node, "Palette", "Default");
			this.cbRenderElement.Text = _Draw.GetElementValue(node, "ChartElementOutput", "Output");
			this.tbPercentWidth.Text = _Draw.GetElementValue(node, "PointWidth", "0");
			this.tbNoRows.Text = _Draw.GetElementValue(node, "NoRows", "");
			// Handle the dataset for this dataregion
			object[] dsNames = _Draw.DataSetNames;
			string defName="";
			if (dsNames != null && dsNames.Length > 0)
			{
				this.cbDataSet.Items.AddRange(_Draw.DataSetNames);
				defName = (string) dsNames[0];
			}
			cbDataSet.Text = _Draw.GetDataSetNameValue(node);
			if (_Draw.GetReportItemDataRegionContainer(node) != null)
				cbDataSet.Enabled = false;
			// page breaks
			this.chkPageBreakStart.Checked = _Draw.GetElementValue(node, "PageBreakAtStart", "false").ToLower() == "true"? true: false;
			this.chkPageBreakEnd.Checked = _Draw.GetElementValue(node, "PageBreakAtEnd", "false").ToLower() == "true"? true: false;

			// Chart data-- this is a simplification of what is possible (TODO) 
			string cdata="";
//        <ChartData>
//          <ChartSeries>
//            <DataPoints>
//              <DataPoint>
//                <DataValues>
//                  <DataValue>
//                    <Value>=Sum(Fields!Sales.Value)</Value>
//                  </DataValue>
//                </DataValues>
//                <DataLabel>
//                  <Style>
//                    <Format>c</Format>
//                  </Style>
//                </DataLabel>
//                <Marker />
//              </DataPoint>
//            </DataPoints>
//          </ChartSeries>
//        </ChartData>

			XmlNode cnode = DesignXmlDraw.FindNextInHierarchy(node, 
				"ChartData", "ChartSeries", "DataPoints", "DataPoint", "DataValues", "DataValue", "Value");
			if (cnode != null)
				cdata = cnode.InnerText;
			this.cbChartData.Text = cdata;

			fChartType = fSubtype = fPalette = fRenderElement = fPercentWidth =
				fNoRows = fDataSet = fPageBreakStart = fPageBreakEnd = fChartData = false;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.cbChartType = new System.Windows.Forms.ComboBox();
			this.cbSubType = new System.Windows.Forms.ComboBox();
			this.cbPalette = new System.Windows.Forms.ComboBox();
			this.cbRenderElement = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tbPercentWidth = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.tbNoRows = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.cbDataSet = new System.Windows.Forms.ComboBox();
			this.chkPageBreakStart = new System.Windows.Forms.CheckBox();
			this.chkPageBreakEnd = new System.Windows.Forms.CheckBox();
			this.cbChartData = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.tbPercentWidth)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Chart Type";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Palette";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 16);
			this.label3.TabIndex = 2;
			this.label3.Text = "Render XML Element";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 120);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(360, 16);
			this.label4.TabIndex = 3;
			this.label4.Text = "Percent width for Bars/Columns (>100% will cause overlap of columns)";
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
			this.cbChartType.Location = new System.Drawing.Point(136, 16);
			this.cbChartType.Name = "cbChartType";
			this.cbChartType.Size = new System.Drawing.Size(121, 21);
			this.cbChartType.TabIndex = 4;
			this.cbChartType.SelectedIndexChanged += new System.EventHandler(this.cbChartType_SelectedIndexChanged);
			// 
			// cbSubType
			// 
			this.cbSubType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSubType.Location = new System.Drawing.Point(336, 16);
			this.cbSubType.Name = "cbSubType";
			this.cbSubType.Size = new System.Drawing.Size(80, 21);
			this.cbSubType.TabIndex = 5;
			this.cbSubType.SelectedIndexChanged += new System.EventHandler(this.cbSubType_SelectedIndexChanged);
			// 
			// cbPalette
			// 
			this.cbPalette.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPalette.Items.AddRange(new object[] {
														   "Default",
														   "EarthTones",
														   "Excel",
														   "GrayScale",
														   "Light",
														   "Pastel",
														   "SemiTransparent"});
			this.cbPalette.Location = new System.Drawing.Point(136, 48);
			this.cbPalette.Name = "cbPalette";
			this.cbPalette.Size = new System.Drawing.Size(121, 21);
			this.cbPalette.TabIndex = 6;
			this.cbPalette.SelectedIndexChanged += new System.EventHandler(this.cbPalette_SelectedIndexChanged);
			// 
			// cbRenderElement
			// 
			this.cbRenderElement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRenderElement.Items.AddRange(new object[] {
																 "Output",
																 "NoOutput"});
			this.cbRenderElement.Location = new System.Drawing.Point(136, 86);
			this.cbRenderElement.Name = "cbRenderElement";
			this.cbRenderElement.Size = new System.Drawing.Size(121, 21);
			this.cbRenderElement.TabIndex = 7;
			this.cbRenderElement.SelectedIndexChanged += new System.EventHandler(this.cbRenderElement_SelectedIndexChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(272, 15);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 23);
			this.label5.TabIndex = 8;
			this.label5.Text = "Sub-type";
			// 
			// tbPercentWidth
			// 
			this.tbPercentWidth.Location = new System.Drawing.Point(368, 118);
			this.tbPercentWidth.Name = "tbPercentWidth";
			this.tbPercentWidth.Size = new System.Drawing.Size(48, 20);
			this.tbPercentWidth.TabIndex = 9;
			this.tbPercentWidth.ValueChanged += new System.EventHandler(this.tbPercentWidth_ValueChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 152);
			this.label6.Name = "label6";
			this.label6.TabIndex = 10;
			this.label6.Text = "No Rows Message";
			// 
			// tbNoRows
			// 
			this.tbNoRows.Location = new System.Drawing.Point(144, 152);
			this.tbNoRows.Name = "tbNoRows";
			this.tbNoRows.Size = new System.Drawing.Size(272, 20);
			this.tbNoRows.TabIndex = 11;
			this.tbNoRows.Text = "";
			this.tbNoRows.TextChanged += new System.EventHandler(this.tbNoRows_TextChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(16, 184);
			this.label7.Name = "label7";
			this.label7.TabIndex = 12;
			this.label7.Text = "Data Set Name";
			// 
			// cbDataSet
			// 
			this.cbDataSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbDataSet.Location = new System.Drawing.Point(144, 184);
			this.cbDataSet.Name = "cbDataSet";
			this.cbDataSet.Size = new System.Drawing.Size(121, 21);
			this.cbDataSet.TabIndex = 13;
			this.cbDataSet.SelectedIndexChanged += new System.EventHandler(this.cbDataSet_SelectedIndexChanged);
			// 
			// chkPageBreakStart
			// 
			this.chkPageBreakStart.Location = new System.Drawing.Point(16, 256);
			this.chkPageBreakStart.Name = "chkPageBreakStart";
			this.chkPageBreakStart.Size = new System.Drawing.Size(136, 24);
			this.chkPageBreakStart.TabIndex = 15;
			this.chkPageBreakStart.Text = "Page Break at Start";
			this.chkPageBreakStart.CheckedChanged += new System.EventHandler(this.chkPageBreakStart_CheckedChanged);
			// 
			// chkPageBreakEnd
			// 
			this.chkPageBreakEnd.Location = new System.Drawing.Point(280, 256);
			this.chkPageBreakEnd.Name = "chkPageBreakEnd";
			this.chkPageBreakEnd.Size = new System.Drawing.Size(136, 24);
			this.chkPageBreakEnd.TabIndex = 16;
			this.chkPageBreakEnd.Text = "Page Break at End";
			this.chkPageBreakEnd.CheckedChanged += new System.EventHandler(this.chkPageBreakEnd_CheckedChanged);
			// 
			// cbChartData
			// 
			this.cbChartData.Location = new System.Drawing.Point(144, 216);
			this.cbChartData.Name = "cbChartData";
			this.cbChartData.Size = new System.Drawing.Size(272, 21);
			this.cbChartData.TabIndex = 14;
			this.cbChartData.TextChanged += new System.EventHandler(this.cbChartData_Changed);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(16, 216);
			this.label8.Name = "label8";
			this.label8.TabIndex = 16;
			this.label8.Text = "Chart Data";
			// 
			// ChartCtl
			// 
			this.Controls.Add(this.cbChartData);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.chkPageBreakEnd);
			this.Controls.Add(this.chkPageBreakStart);
			this.Controls.Add(this.cbDataSet);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.tbNoRows);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.tbPercentWidth);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cbRenderElement);
			this.Controls.Add(this.cbPalette);
			this.Controls.Add(this.cbSubType);
			this.Controls.Add(this.cbChartType);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "ChartCtl";
			this.Size = new System.Drawing.Size(440, 288);
			((System.ComponentModel.ISupportInitialize)(this.tbPercentWidth)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		public bool IsValid()
		{
			return true;
		}

		public void Apply()
		{
			// take information in control and apply to all the style nodes
			//  Only change information that has been marked as modified;
			//   this way when group is selected it is possible to change just
			//   the items you want and keep the rest the same.
				
			foreach (XmlNode riNode in this._ReportItems)
				ApplyChanges(riNode);

			// No more changes
			fChartType = fSubtype = fPalette = fRenderElement = fPercentWidth =
				fNoRows = fDataSet = fPageBreakStart = fPageBreakEnd = fChartData = false;
		}

		public void ApplyChanges(XmlNode node)
		{
			if (fChartType)
			{
				_Draw.SetElement(node, "Type", this.cbChartType.Text);
			}
			if (fSubtype)
			{
				_Draw.SetElement(node, "Subtype", this.cbSubType.Text);
			}
			if (fPalette)
			{
				_Draw.SetElement(node, "Palette", this.cbPalette.Text);
			}
			if (fRenderElement)
			{
				_Draw.SetElement(node, "ChartElementOutput", this.cbRenderElement.Text);
			}
			if (fPercentWidth)
			{
				_Draw.SetElement(node, "PointWidth", this.tbPercentWidth.Text);
			}
			if (fNoRows)
			{
				_Draw.SetElement(node, "NoRows", this.tbNoRows.Text);
			}
			if (fDataSet)
			{
				_Draw.SetElement(node, "DataSetName", this.cbDataSet.Text);
			}
			if (fPageBreakStart)
			{
				_Draw.SetElement(node, "PageBreakAtStart", this.chkPageBreakStart.Checked? "true": "false");
			}
			if (fPageBreakEnd)
			{
				_Draw.SetElement(node, "PageBreakAtEnd", this.chkPageBreakEnd.Checked? "true": "false");
			}
			if (fChartData)
			{
				//        <ChartData>
				//          <ChartSeries>
				//            <DataPoints>
				//              <DataPoint>
				//                <DataValues>
				//                  <DataValue>
				//                    <Value>=Sum(Fields!Sales.Value)</Value>
				//                  </DataValue>
				//                </DataValues>
				//                <DataLabel>
				//                  <Style>
				//                    <Format>c</Format>
				//                  </Style>
				//                </DataLabel>
				//                <Marker />
				//              </DataPoint>
				//            </DataPoints>
				//          </ChartSeries>
				//        </ChartData>
				XmlNode chartdata = _Draw.SetElement(node, "ChartData", null);
				XmlNode chartseries = _Draw.SetElement(chartdata, "ChartSeries", null);
				XmlNode datapoints = _Draw.SetElement(chartseries, "DataPoints", null);
				XmlNode datapoint = _Draw.SetElement(datapoints, "DataPoint", null);
				XmlNode datavalues = _Draw.SetElement(datapoint, "DataValues", null);
				XmlNode datavalue = _Draw.SetElement(datavalues, "DataValue", null);
				_Draw.SetElement(datavalue, "Value", this.cbChartData.Text);
			}
		}

		private void cbChartType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			fChartType = true;
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

		private void cbSubType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			fSubtype = true;
		}

		private void cbPalette_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			fPalette = true;
		}

		private void cbRenderElement_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			fRenderElement = true;
		}

		private void tbPercentWidth_ValueChanged(object sender, System.EventArgs e)
		{
			fPercentWidth = true;
		}

		private void tbNoRows_TextChanged(object sender, System.EventArgs e)
		{
			fNoRows = true;
		}

		private void cbDataSet_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			fDataSet = true;
		}

		private void chkPageBreakStart_CheckedChanged(object sender, System.EventArgs e)
		{
			fPageBreakStart = true;
		}

		private void chkPageBreakEnd_CheckedChanged(object sender, System.EventArgs e)
		{
			fPageBreakEnd = true;
		}

		private void cbChartData_Changed(object sender, System.EventArgs e)
		{
			fChartData = true;		
		}
	}
}
