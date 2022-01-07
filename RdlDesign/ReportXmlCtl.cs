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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using System.IO;


namespace fyiReporting.RdlDesign
{
	/// <summary>
	/// Summary description for StyleCtl.
	/// </summary>
	internal class ReportXmlCtl : System.Windows.Forms.UserControl, IProperty
	{
		private DesignXmlDraw _Draw;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbDataTransform;
		private System.Windows.Forms.TextBox tbDataSchema;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbDataElementName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cbElementStyle;
		private System.Windows.Forms.Button bOpenXsl;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal ReportXmlCtl(DesignXmlDraw dxDraw)
		{
			_Draw = dxDraw;
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Initialize form using the style node values
			InitValues();			
		}

		private void InitValues()
		{
			XmlNode rNode = _Draw.GetReportNode();
			tbDataTransform.Text = _Draw.GetElementValue(rNode, "DataTransform", "");
			tbDataSchema.Text = _Draw.GetElementValue(rNode, "DataSchema", "");
			tbDataElementName.Text = _Draw.GetElementValue(rNode, "DataElementName", "Report");
			cbElementStyle.Text = _Draw.GetElementValue(rNode, "DataElementStyle", "AttributeNormal");
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
			this.tbDataTransform = new System.Windows.Forms.TextBox();
			this.tbDataSchema = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbDataElementName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.cbElementStyle = new System.Windows.Forms.ComboBox();
			this.bOpenXsl = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "XSL Data Transform";
			// 
			// tbDataTransform
			// 
			this.tbDataTransform.Location = new System.Drawing.Point(136, 32);
			this.tbDataTransform.Name = "tbDataTransform";
			this.tbDataTransform.Size = new System.Drawing.Size(248, 20);
			this.tbDataTransform.TabIndex = 1;
			this.tbDataTransform.Text = "textBox1";
			// 
			// tbDataSchema
			// 
			this.tbDataSchema.Location = new System.Drawing.Point(136, 72);
			this.tbDataSchema.Name = "tbDataSchema";
			this.tbDataSchema.Size = new System.Drawing.Size(248, 20);
			this.tbDataSchema.TabIndex = 3;
			this.tbDataSchema.Text = "textBox1";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "Data Schema";
			// 
			// tbDataElementName
			// 
			this.tbDataElementName.Location = new System.Drawing.Point(136, 112);
			this.tbDataElementName.Name = "tbDataElementName";
			this.tbDataElementName.Size = new System.Drawing.Size(248, 20);
			this.tbDataElementName.TabIndex = 5;
			this.tbDataElementName.Text = "textBox1";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 112);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 23);
			this.label3.TabIndex = 4;
			this.label3.Text = "Top Element Name";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 152);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 23);
			this.label4.TabIndex = 6;
			this.label4.Text = "Element Style";
			// 
			// cbElementStyle
			// 
			this.cbElementStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbElementStyle.Items.AddRange(new object[] {
																"AttributeNormal",
																"ElementNormal"});
			this.cbElementStyle.Location = new System.Drawing.Point(136, 152);
			this.cbElementStyle.Name = "cbElementStyle";
			this.cbElementStyle.Size = new System.Drawing.Size(144, 21);
			this.cbElementStyle.TabIndex = 7;
			// 
			// bOpenXsl
			// 
			this.bOpenXsl.Location = new System.Drawing.Point(400, 32);
			this.bOpenXsl.Name = "bOpenXsl";
			this.bOpenXsl.Size = new System.Drawing.Size(24, 23);
			this.bOpenXsl.TabIndex = 8;
			this.bOpenXsl.Text = "...";
			this.bOpenXsl.Click += new System.EventHandler(this.bOpenXsl_Click);
			// 
			// ReportXmlCtl
			// 
			this.Controls.Add(this.bOpenXsl);
			this.Controls.Add(this.cbElementStyle);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbDataElementName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tbDataSchema);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbDataTransform);
			this.Controls.Add(this.label1);
			this.Name = "ReportXmlCtl";
			this.Size = new System.Drawing.Size(472, 288);
			this.ResumeLayout(false);

		}
		#endregion

		public bool IsValid()
		{
			return true;
		}

		public void Apply()
		{
			XmlNode rNode = _Draw.GetReportNode();

			if (tbDataTransform.Text.Length > 0)
				_Draw.SetElement(rNode, "DataTransform", tbDataTransform.Text);
			else
				_Draw.RemoveElement(rNode, "DataTransform");
			
			if (tbDataSchema.Text.Length > 0)
				_Draw.SetElement(rNode, "DataSchema", tbDataSchema.Text);
			else
				_Draw.RemoveElement(rNode, "DataSchema");

			if (tbDataElementName.Text.Length > 0)
				_Draw.SetElement(rNode, "DataElementName", tbDataElementName.Text);
			else
				_Draw.RemoveElement(rNode, "DataElementName");

			_Draw.SetElement(rNode, "DataElementStyle", cbElementStyle.Text);
		}

		private void bOpenXsl_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "XSL files (*.xsl)|*.xsl" +
				"All files (*.*)|*.*|";
			ofd.FilterIndex = 1;
			ofd.FileName = "*.xsl";

			ofd.Title = "Specify DataTransform File Name";
//			ofd.DefaultExt = "xsl";
//			ofd.AddExtension = true;
			
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				string file = Path.GetFileName(ofd.FileName);

				tbDataTransform.Text = file;
			}

		}
	}
}
