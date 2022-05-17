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
	internal class ChartLegendCtl : System.Windows.Forms.UserControl, IProperty
	{
        private List<XmlNode> _ReportItems;
		private DesignXmlDraw _Draw;
		bool fVisible, fLayout, fPosition, fInsidePlotArea;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbPosition;
		private System.Windows.Forms.ComboBox cbLayout;
		private System.Windows.Forms.CheckBox chkVisible;
		private System.Windows.Forms.CheckBox chkInsidePlotArea;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        internal ChartLegendCtl(DesignXmlDraw dxDraw, List<XmlNode> ris)
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

			this.cbPosition.Text = _Draw.GetElementValue(node, "Position", "RightTop");
			this.cbLayout.Text = _Draw.GetElementValue(node, "Layout", "Column");
			this.chkVisible.Checked = _Draw.GetElementValue(node, "Visible", "false").ToLower() == "true"? true: false;
			this.chkInsidePlotArea.Checked = _Draw.GetElementValue(node, "InsidePlotArea", "false").ToLower() == "true"? true: false;

			fVisible = fLayout = fPosition = fInsidePlotArea = false;
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
			this.cbPosition = new System.Windows.Forms.ComboBox();
			this.cbLayout = new System.Windows.Forms.ComboBox();
			this.chkVisible = new System.Windows.Forms.CheckBox();
			this.chkInsidePlotArea = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Position in Chart";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Layout within Legend";
			// 
			// cbPosition
			// 
			this.cbPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPosition.Items.AddRange(new object[] {
															"TopLeft",
															"TopCenter",
															"TopRight",
															"LeftTop",
															"LeftCenter",
															"LeftBottom",
															"RightTop",
															"RightCenter",
															"RightBottom",
															"BottomRight",
															"BottomCenter",
															"BottomLeft"});
			this.cbPosition.Location = new System.Drawing.Point(144, 16);
			this.cbPosition.Name = "cbPosition";
			this.cbPosition.Size = new System.Drawing.Size(121, 21);
			this.cbPosition.TabIndex = 4;
			this.cbPosition.SelectedIndexChanged += new System.EventHandler(this.cbPosition_SelectedIndexChanged);
			// 
			// cbLayout
			// 
			this.cbLayout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLayout.Items.AddRange(new object[] {
														  "Column",
														  "Row",
														  "Table"});
			this.cbLayout.Location = new System.Drawing.Point(144, 48);
			this.cbLayout.Name = "cbLayout";
			this.cbLayout.Size = new System.Drawing.Size(121, 21);
			this.cbLayout.TabIndex = 6;
			this.cbLayout.SelectedIndexChanged += new System.EventHandler(this.cbLayout_SelectedIndexChanged);
			// 
			// chkVisible
			// 
			this.chkVisible.Location = new System.Drawing.Point(16, 80);
			this.chkVisible.Name = "chkVisible";
			this.chkVisible.Size = new System.Drawing.Size(136, 24);
			this.chkVisible.TabIndex = 14;
			this.chkVisible.Text = "Visible";
			this.chkVisible.CheckedChanged += new System.EventHandler(this.chkVisible_CheckedChanged);
			// 
			// chkInsidePlotArea
			// 
			this.chkInsidePlotArea.Location = new System.Drawing.Point(16, 112);
			this.chkInsidePlotArea.Name = "chkInsidePlotArea";
			this.chkInsidePlotArea.Size = new System.Drawing.Size(136, 24);
			this.chkInsidePlotArea.TabIndex = 15;
			this.chkInsidePlotArea.Text = "Draw Inside Plot Area";
			this.chkInsidePlotArea.CheckedChanged += new System.EventHandler(this.chkInsidePlotArea_CheckedChanged);
			// 
			// ChartLegendCtl
			// 
			this.Controls.Add(this.chkInsidePlotArea);
			this.Controls.Add(this.chkVisible);
			this.Controls.Add(this.cbLayout);
			this.Controls.Add(this.cbPosition);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "ChartLegendCtl";
			this.Size = new System.Drawing.Size(440, 288);
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
			fVisible = fLayout = fPosition = fInsidePlotArea = false;
		}

		public void ApplyChanges(XmlNode node)
		{
			if (fVisible)
			{
				_Draw.SetElement(node, "Visible", this.chkVisible.Checked? "true": "false");
			}
			if (fLayout)
			{
				_Draw.SetElement(node, "Layout", this.cbLayout.Text);
			}
			if (fPosition)
			{
				_Draw.SetElement(node, "Position", this.cbPosition.Text);
			}
			if (fInsidePlotArea)
			{
				_Draw.SetElement(node, "InsidePlotArea", this.chkInsidePlotArea.Checked? "true": "false");
			}
		}

		private void cbPosition_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			fPosition = true;
		}

		private void cbLayout_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			fLayout = true;
		}

		private void chkVisible_CheckedChanged(object sender, System.EventArgs e)
		{
			fVisible = true;
		}

		private void chkInsidePlotArea_CheckedChanged(object sender, System.EventArgs e)
		{
			fInsidePlotArea = true;
		}

	}
}
