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
using System.Text;
using System.Xml;

namespace fyiReporting.RdlDesign
{
	/// <summary>
	/// Summary description for ReportCtl.
	/// </summary>
	internal class SQLCtl : System.Windows.Forms.Form
	{
		DesignXmlDraw _Draw;
		string _DataSource;
		DataTable _QueryParameters;
		private System.Windows.Forms.TextBox tbSQL;
		private System.Windows.Forms.TreeView tvTablesColumns;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button bMove;
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancel;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal SQLCtl(DesignXmlDraw dxDraw, string datasource, string sql, DataTable queryParameters)
		{
			_Draw = dxDraw;
			_DataSource = datasource;
			_QueryParameters = queryParameters;
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Initialize form using the style node values
			InitValues(sql);			
		}

		private void InitValues(string sql)
		{
			this.tbSQL.Text = sql;

			// Fill out the tables, columns and parameters

			// suppress redraw until tree view is complete
			tvTablesColumns.BeginUpdate();
			
			// Get the schema information
			List<SqlSchemaInfo> si = DesignerUtility.GetSchemaInfo(_Draw, _DataSource);
			if (si != null && si.Count > 0)
			{
				TreeNode ndRoot = new TreeNode("Tables");
				tvTablesColumns.Nodes.Add(ndRoot);
				if (si == null)		// Nothing to initialize
					return;
				bool bView = false;
				foreach (SqlSchemaInfo ssi in si)
				{
					if (!bView && ssi.Type == "VIEW")
					{	// Switch over to views
						ndRoot = new TreeNode("Views");
						tvTablesColumns.Nodes.Add(ndRoot);
						bView=true;
					}

					// Add the node to the tree
					TreeNode aRoot = new TreeNode(ssi.Name);
					ndRoot.Nodes.Add(aRoot);
					aRoot.Nodes.Add("");
				}
			}
			// Now do parameters
			TreeNode qpRoot = null;
			foreach (DataRow dr in _QueryParameters.Rows)
			{
				if (dr[0] == DBNull.Value || dr[1] == null)
					continue;
				string pName = (string) dr[0];
				if (pName.Length == 0)
					continue;
				if (qpRoot == null)
				{
					qpRoot = new TreeNode("Query Parameters");
					tvTablesColumns.Nodes.Add(qpRoot);
				}
				if (pName[0] == '@')
					pName = "@" + pName;
				// Add the node to the tree
				TreeNode aRoot = new TreeNode(pName);
				qpRoot.Nodes.Add(aRoot);
			}

			tvTablesColumns.EndUpdate();
		}

		internal string SQL
		{
			get {return tbSQL.Text;}
			set {tbSQL.Text = value;}
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
			this.tbSQL = new System.Windows.Forms.TextBox();
			this.tvTablesColumns = new System.Windows.Forms.TreeView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.bOK = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.bMove = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbSQL
			// 
			this.tbSQL.AllowDrop = true;
			this.tbSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbSQL.Location = new System.Drawing.Point(176, 0);
			this.tbSQL.Multiline = true;
			this.tbSQL.Name = "tbSQL";
			this.tbSQL.Size = new System.Drawing.Size(304, 142);
			this.tbSQL.TabIndex = 5;
			this.tbSQL.Text = "";
			// 
			// tvTablesColumns
			// 
			this.tvTablesColumns.Dock = System.Windows.Forms.DockStyle.Left;
			this.tvTablesColumns.FullRowSelect = true;
			this.tvTablesColumns.ImageIndex = -1;
			this.tvTablesColumns.Location = new System.Drawing.Point(0, 0);
			this.tvTablesColumns.Name = "tvTablesColumns";
			this.tvTablesColumns.SelectedImageIndex = -1;
			this.tvTablesColumns.Size = new System.Drawing.Size(136, 142);
			this.tvTablesColumns.TabIndex = 4;
			this.tvTablesColumns.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvTablesColumns_BeforeExpand);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.bOK);
			this.panel1.Controls.Add(this.bCancel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 142);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(480, 40);
			this.panel1.TabIndex = 6;
			// 
			// bOK
			// 
			this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bOK.Location = new System.Drawing.Point(312, 8);
			this.bOK.Name = "bOK";
			this.bOK.TabIndex = 2;
			this.bOK.Text = "OK";
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// bCancel
			// 
			this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bCancel.CausesValidation = false;
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(400, 8);
			this.bCancel.Name = "bCancel";
			this.bCancel.TabIndex = 3;
			this.bCancel.Text = "Cancel";
			// 
			// bMove
			// 
			this.bMove.Location = new System.Drawing.Point(142, 8);
			this.bMove.Name = "bMove";
			this.bMove.Size = new System.Drawing.Size(32, 23);
			this.bMove.TabIndex = 8;
			this.bMove.Text = ">>";
			this.bMove.Click += new System.EventHandler(this.bMove_Click);
			// 
			// SQLCtl
			// 
			this.AcceptButton = this.bOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(480, 182);
			this.ControlBox = false;
			this.Controls.Add(this.bMove);
			this.Controls.Add(this.tvTablesColumns);
			this.Controls.Add(this.tbSQL);
			this.Controls.Add(this.panel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SQLCtl";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "SQL Syntax Helper";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void bOK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}
		private void tvTablesColumns_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			tvTablesColumns_ExpandTable(e.Node);
		}

		private void tvTablesColumns_ExpandTable(TreeNode tNode)
		{
			if (tNode.Parent == null)	// Check for Tables or Views
				return;					

			if (tNode.FirstNode.Text != "")	// Have we already filled it out?
				return;

			// Need to obtain the column information for the requested table/view
			// suppress redraw until tree view is complete
			tvTablesColumns.BeginUpdate();
			
			string sql = "SELECT * FROM " + NormalizeName(tNode.Text);
			List<SqlColumn> tColumns = DesignerUtility.GetSqlColumns(_Draw, _DataSource, sql);
			bool bFirstTime=true;
			foreach (SqlColumn sc in tColumns)
			{
				if (bFirstTime)
				{
					bFirstTime = false;
					tNode.FirstNode.Text = sc.Name;
				}
				else
					tNode.Nodes.Add(sc.Name);
			}

			tvTablesColumns.EndUpdate();
		}

		private string NormalizeName(string name)
		{	// Routine ensures valid sql name
			bool bLetterOrDigit = true;
			for (int i=0; i < name.Length && bLetterOrDigit; i++)
			{
				if (name[i] == '.')
				{}						// allow names to have a "." for owner qualified tables
				else if (!Char.IsLetterOrDigit(name, i))
					bLetterOrDigit = false;
			}
			if (bLetterOrDigit)
				return name;
			else
				return "\"" + name + "\"";
		}

		private void bMove_Click(object sender, System.EventArgs e)
		{
			if (tvTablesColumns.SelectedNode == null ||
				tvTablesColumns.SelectedNode.Parent == null)
				return;		// this is the Tables/Views node

			TreeNode node = tvTablesColumns.SelectedNode;
			string t = node.Text;
			if (tbSQL.Text == "")
			{
				if (node.Parent.Parent == null)
				{	// select table; generate full select for table
					tvTablesColumns_ExpandTable(node);	// make sure we've obtained the columns

					StringBuilder sb = new StringBuilder("SELECT ");
					TreeNode next = node.FirstNode;
					while (true)
					{
						sb.Append(NormalizeName(next.Text));
						next = next.NextNode;
						if (next == null)
							break;
						sb.Append(", ");
					}
					sb.Append(" FROM ");
					sb.Append(NormalizeName(node.Text));
					t = sb.ToString();
				}
				else
				{	// select column; generate select of that column	
					t = "SELECT " + NormalizeName(node.Text) + " FROM " + NormalizeName(node.Parent.Text);
				}
			}

			tbSQL.SelectedText = t;
		}


	}
}
