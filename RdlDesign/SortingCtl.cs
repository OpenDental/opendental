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
using System.Text;

namespace fyiReporting.RdlDesign
{
	/// <summary>
	/// Sorting specification
	/// </summary>
	internal class SortingCtl : System.Windows.Forms.UserControl, IProperty
	{
		private DesignXmlDraw _Draw;
		private XmlNode _SortingParent;
		private DataTable _DataTable;
		private DataGridTextBoxColumn dgtbExpr;
		private DataGridBoolColumn dgtbDir;

		private System.Windows.Forms.Button bDelete;
		private System.Windows.Forms.DataGridTableStyle dgTableStyle;
		private System.Windows.Forms.Button bUp;
		private System.Windows.Forms.Button bDown;
		private System.Windows.Forms.DataGrid dgSorting;
		private System.Windows.Forms.Button bValueExpr;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal SortingCtl(DesignXmlDraw dxDraw, XmlNode sortingParent)
		{
			_Draw = dxDraw;
			_SortingParent = sortingParent;
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Initialize form using the style node values
			InitValues();			
		}

		private void InitValues()
		{
			// Initialize the DataGrid columns
			dgtbExpr = new DataGridTextBoxColumn();
		
			dgtbDir = new DataGridBoolColumn();

			this.dgTableStyle.GridColumnStyles.AddRange(new DataGridColumnStyle[] {
															this.dgtbExpr,
															this.dgtbDir});
			// 
			// dgtbExpr
			// 
			
			dgtbExpr.HeaderText = "Sort Expression";
			dgtbExpr.MappingName = "SortExpression";
			dgtbExpr.Width = 75;
			// Get the parent's dataset name
//			string dataSetName = _Draw.GetDataSetNameValue(_SortingParent);
//
//			string[] fields = _Draw.GetFields(dataSetName, true);
//			if (fields != null)
//				dgtbExpr.CB.Items.AddRange(fields);
			// 
			// dgtbDir
			// 
			dgtbDir.HeaderText = "Sort Ascending";
			dgtbDir.MappingName = "Direction";
			dgtbDir.Width = 70;
			dgtbDir.AllowNull = false;

			// Initialize the DataTable
			_DataTable = new DataTable();
			_DataTable.Columns.Add(new DataColumn("SortExpression", typeof(string)));
			_DataTable.Columns.Add(new DataColumn("Direction", typeof(bool)));

			object[] rowValues = new object[2];
			XmlNode sorts = _Draw.GetNamedChildNode(_SortingParent, "Sorting");

			if (sorts != null)
			foreach (XmlNode sNode in sorts.ChildNodes)
			{
				if (sNode.NodeType != XmlNodeType.Element || 
						sNode.Name != "SortBy")
					continue;
				rowValues[0] = _Draw.GetElementValue(sNode, "SortExpression", "");
				if (_Draw.GetElementValue(sNode, "Direction", "Ascending") == "Ascending")
					rowValues[1] = true;
				else
					rowValues[1] = false;

				_DataTable.Rows.Add(rowValues);
			}
			this.dgSorting.DataSource = _DataTable;
			DataGridTableStyle ts = dgSorting.TableStyles[0];
			ts.PreferredRowHeight = 14;
			ts.GridColumnStyles[0].Width = 240;
			ts.GridColumnStyles[1].Width = 90;
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
			this.dgSorting = new System.Windows.Forms.DataGrid();
			this.dgTableStyle = new System.Windows.Forms.DataGridTableStyle();
			this.bDelete = new System.Windows.Forms.Button();
			this.bUp = new System.Windows.Forms.Button();
			this.bDown = new System.Windows.Forms.Button();
			this.bValueExpr = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgSorting)).BeginInit();
			this.SuspendLayout();
			// 
			// dgSorting
			// 
			this.dgSorting.CaptionVisible = false;
			this.dgSorting.DataMember = "";
			this.dgSorting.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgSorting.Location = new System.Drawing.Point(8, 8);
			this.dgSorting.Name = "dgSorting";
			this.dgSorting.Size = new System.Drawing.Size(376, 264);
			this.dgSorting.TabIndex = 0;
			this.dgSorting.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
																								  this.dgTableStyle});
			// 
			// dgTableStyle
			// 
			this.dgTableStyle.AllowSorting = false;
			this.dgTableStyle.DataGrid = this.dgSorting;
			this.dgTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgTableStyle.MappingName = "";
			// 
			// bDelete
			// 
			this.bDelete.Location = new System.Drawing.Point(392, 40);
			this.bDelete.Name = "bDelete";
			this.bDelete.Size = new System.Drawing.Size(48, 23);
			this.bDelete.TabIndex = 2;
			this.bDelete.Text = "Delete";
			this.bDelete.Click += new System.EventHandler(this.bDelete_Click);
			// 
			// bUp
			// 
			this.bUp.Location = new System.Drawing.Point(392, 72);
			this.bUp.Name = "bUp";
			this.bUp.Size = new System.Drawing.Size(48, 23);
			this.bUp.TabIndex = 3;
			this.bUp.Text = "Up";
			this.bUp.Click += new System.EventHandler(this.bUp_Click);
			// 
			// bDown
			// 
			this.bDown.Location = new System.Drawing.Point(392, 104);
			this.bDown.Name = "bDown";
			this.bDown.Size = new System.Drawing.Size(48, 23);
			this.bDown.TabIndex = 4;
			this.bDown.Text = "Down";
			this.bDown.Click += new System.EventHandler(this.bDown_Click);
			// 
			// bValueExpr
			// 
			this.bValueExpr.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.bValueExpr.Location = new System.Drawing.Point(392, 16);
			this.bValueExpr.Name = "bValueExpr";
			this.bValueExpr.Size = new System.Drawing.Size(22, 16);
			this.bValueExpr.TabIndex = 1;
			this.bValueExpr.Tag = "value";
			this.bValueExpr.Text = "fx";
			this.bValueExpr.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.bValueExpr.Click += new System.EventHandler(this.bValueExpr_Click);
			// 
			// SortingCtl
			// 
			this.Controls.Add(this.bValueExpr);
			this.Controls.Add(this.bDown);
			this.Controls.Add(this.bUp);
			this.Controls.Add(this.bDelete);
			this.Controls.Add(this.dgSorting);
			this.Name = "SortingCtl";
			this.Size = new System.Drawing.Size(488, 304);
			((System.ComponentModel.ISupportInitialize)(this.dgSorting)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		public bool IsValid()
		{
			return true;
		}

		public void Apply()
		{
			// Remove the old filters
			XmlNode sorts = null;
			_Draw.RemoveElement(_SortingParent, "Sorting");
			// Loop thru and add all the filters
			foreach (DataRow dr in _DataTable.Rows)
			{
				if (dr[0] == DBNull.Value)
					continue;
				string expr = (string) dr[0];
				bool dir = dr[1] == DBNull.Value? true: (bool) dr[1];
				
				if (expr.Length <= 0)
					continue;

				if (sorts == null)
					sorts = _Draw.CreateElement(_SortingParent, "Sorting", null);

				XmlNode sNode = _Draw.CreateElement(sorts, "SortBy", null);
				_Draw.CreateElement(sNode, "SortExpression", expr);
				_Draw.CreateElement(sNode, "Direction", dir?"Ascending":"Descending");
			}
		}

		private void bDelete_Click(object sender, System.EventArgs e)
		{
			this._DataTable.Rows.RemoveAt(this.dgSorting.CurrentRowIndex);
		}

		private void bUp_Click(object sender, System.EventArgs e)
		{
			int cr = dgSorting.CurrentRowIndex;
			if (cr <= 0)		// already at the top
				return;
			
			SwapRow(_DataTable.Rows[cr-1], _DataTable.Rows[cr]);
			dgSorting.CurrentRowIndex = cr-1;
		}

		private void bDown_Click(object sender, System.EventArgs e)
		{
			int cr = dgSorting.CurrentRowIndex;
			if (cr < 0)			// invalid index
				return;
			if (cr + 1 >= _DataTable.Rows.Count)
				return;			// already at end
			
			SwapRow(_DataTable.Rows[cr+1], _DataTable.Rows[cr]);
			dgSorting.CurrentRowIndex = cr+1;
		}

		private void SwapRow(DataRow tdr, DataRow fdr)
		{
			// column 1
			object save = tdr[0];
			tdr[0] = fdr[0];
			fdr[0] = save;
			// column 2
			save = tdr[1];
			tdr[1] = fdr[1];
			fdr[1] = save;
			// column 3
			save = tdr[2];
			tdr[2] = fdr[2];
			fdr[2] = save;
			return;
		}

		private void bValueExpr_Click(object sender, System.EventArgs e)
		{
			int cr = dgSorting.CurrentRowIndex;
			if (cr < 0)
			{	// No rows yet; create one
				string[] rowValues = new string[2];
				rowValues[0] = null;
				rowValues[1] = null;

				_DataTable.Rows.Add(rowValues);
				cr = 0;
			}
			int cc = 0;
			DataRow dr = _DataTable.Rows[cr];
			string cv = dr[cc] as string;

			DialogExprEditor ee = new DialogExprEditor(_Draw, cv, _SortingParent, false);
			DialogResult dlgr = ee.ShowDialog();
			if (dlgr == DialogResult.OK)
				dr[cc] = ee.Expression;

		}
	}
}
