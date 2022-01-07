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
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Text; 
using System.Reflection;
using fyiReporting.RDL;

namespace fyiReporting.RdlDesign
{
	/// <summary>
	/// DialogListOfStrings: puts up a dialog that lets a user enter a list of strings
	/// </summary>
	public class DialogExprEditor : System.Windows.Forms.Form
	{
		Type[] BASE_TYPES = new Type[] {System.String.Empty.GetType(),
												  System.Double.MinValue.GetType(),
												  System.Single.MinValue.GetType(),
												  System.Decimal.MinValue.GetType(),
												  System.DateTime.MinValue.GetType(),
												  System.Char.MinValue.GetType(),
												  new bool().GetType(),
												  System.Int32.MinValue.GetType(),
												  System.Int16.MinValue.GetType(),
												  System.Int64.MinValue.GetType(),
												  System.Byte.MinValue.GetType(),
												  System.UInt16.MinValue.GetType(),
												  System.UInt32.MinValue.GetType(),
												  System.UInt64.MinValue.GetType()};
		private DesignXmlDraw _Draw;		// design draw 
		private bool _Color;				// true if color list should be displayed

		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.TextBox tbExpr;
		private System.Windows.Forms.Button bCopy;
		private System.Windows.Forms.TreeView tvOp;
		private System.Windows.Forms.Label lExpr;
		private System.Windows.Forms.Label lOp;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal DialogExprEditor(DesignXmlDraw dxDraw, string expr, XmlNode node) : 
			this(dxDraw, expr, node, false)
		{
		}

		internal DialogExprEditor(DesignXmlDraw dxDraw, string expr, XmlNode node, bool bColor)
		{
			_Draw = dxDraw;
			_Color = bColor;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			tbExpr.Text = expr;

			// Fill out the fields list 
			string[] fields = null;
			// Find the dataregion that contains the item (if any)
			for (XmlNode pNode = node; pNode != null; pNode = pNode.ParentNode)
			{
				if (pNode.Name == "List" ||
					pNode.Name == "Table" ||
					pNode.Name == "Matrix" ||
					pNode.Name == "Chart")
				{
					string dsname = _Draw.GetDataSetNameValue(pNode);
					if (dsname != null)	// found it
					{
						fields = _Draw.GetFields(dsname, true);
					}
				}
			}
			BuildTree(fields);

			return;
		}

		void BuildTree(string[] flds)
		{
			// suppress redraw until tree view is complete
			tvOp.BeginUpdate();

			// Handle the globals
			TreeNode ndRoot = new TreeNode("Globals");
			tvOp.Nodes.Add(ndRoot);
			foreach (string item in StaticLists.GlobalList)
			{
				// Add the node to the tree
				TreeNode aRoot = new TreeNode(item.StartsWith("=")? item.Substring(1): item);
				ndRoot.Nodes.Add(aRoot);
			}

			// Fields - only when a dataset is specified
			if (flds != null && flds.Length > 0)
			{
				ndRoot = new TreeNode("Fields");
				tvOp.Nodes.Add(ndRoot);

				foreach (string f in flds)
				{	
					TreeNode aRoot = new TreeNode(f.StartsWith("=")? f.Substring(1): f);
					ndRoot.Nodes.Add(aRoot);
				}
			}

			// Report parameters
			InitReportParameters();

			// Handle the functions
			ndRoot = new TreeNode("Functions");
			tvOp.Nodes.Add(ndRoot);
			InitFunctions(ndRoot);

			// Aggregate functions
			ndRoot = new TreeNode("Aggregate Functions");
			tvOp.Nodes.Add(ndRoot);
			foreach (string item in StaticLists.AggrFunctionList)
			{
				// Add the node to the tree
				TreeNode aRoot = new TreeNode(item);
				ndRoot.Nodes.Add(aRoot);
			}

			// Operators
			ndRoot = new TreeNode("Operators");
			tvOp.Nodes.Add(ndRoot);
			foreach (string item in StaticLists.OperatorList)
			{
				// Add the node to the tree
				TreeNode aRoot = new TreeNode(item);
				ndRoot.Nodes.Add(aRoot);
			}

			// Colors (if requested)
			if (_Color)
			{
				ndRoot = new TreeNode("Colors");
				tvOp.Nodes.Add(ndRoot);
				foreach (string item in StaticLists.ColorList)
				{
					// Add the node to the tree
					TreeNode aRoot = new TreeNode(item);
					ndRoot.Nodes.Add(aRoot);
				}
			}


			tvOp.EndUpdate();

		}

		/// <summary>
		/// Populate tree view with the report parameters (if any)
		/// </summary>
		void InitReportParameters()
		{
			string[] ps = _Draw.GetReportParameters(true);
			
			if (ps == null || ps.Length == 0)
				return;

			TreeNode ndRoot = new TreeNode("Parameters");
			tvOp.Nodes.Add(ndRoot);

			foreach (string p in ps)
			{
				TreeNode aRoot = new TreeNode(p.StartsWith("=")?p.Substring(1): p);
				ndRoot.Nodes.Add(aRoot);
			}

			return;
		}

		void InitFunctions(TreeNode ndRoot)
		{
            List<string> ar = new List<string>();
			
			ar.AddRange(StaticLists.FunctionList);

			// Build list of methods in the  VBFunctions class
			fyiReporting.RDL.FontStyleEnum fsi = FontStyleEnum.Italic;	// just want a class from RdlEngine.dll assembly
			Assembly a = Assembly.GetAssembly(fsi.GetType());
			if (a == null)
				return;
			Type ft = a.GetType("fyiReporting.RDL.VBFunctions");	 
			BuildMethods(ar, ft, "");

			// build list of financial methods in Financial class
			ft = a.GetType("fyiReporting.RDL.Financial");
			BuildMethods(ar, ft, "Financial.");

			a = Assembly.GetAssembly("".GetType());
			ft = a.GetType("System.Math");
			BuildMethods(ar, ft, "Math.");

			ft = a.GetType("System.Convert");
			BuildMethods(ar, ft, "Convert.");

			ft = a.GetType("System.String");
			BuildMethods(ar, ft, "String.");

			ar.Sort();
			string previous="";
			foreach (string item in ar)
			{
				if (item != previous)	// don't add duplicates
				{
					// Add the node to the tree
					TreeNode aRoot = new TreeNode(item);
					ndRoot.Nodes.Add(aRoot);
				}
				previous = item;
			}

		}

        void BuildMethods(List<string> ar, Type ft, string prefix)
		{
			if (ft == null)
				return;
			MethodInfo[] mis = ft.GetMethods(BindingFlags.Static | BindingFlags.Public);
			foreach (MethodInfo mi in mis)
			{
				// Add the node to the tree
				string name = BuildMethodName(mi);
				if (name != null)
					ar.Add(prefix + name);
			}
		}

		string BuildMethodName(MethodInfo mi)
		{
			StringBuilder sb = new StringBuilder(mi.Name);
			sb.Append("(");
			ParameterInfo[] pis = mi.GetParameters();
			bool bFirst=true;
			foreach (ParameterInfo pi in pis)
			{
				if (!IsBaseType(pi.ParameterType))
					return null;
				if (bFirst)
					bFirst = false;
				else
					sb.Append(", ");
				sb.Append(pi.Name);
			}
			sb.Append(")");
			return sb.ToString();
		}

		// Determines if underlying type is a primitive
		bool IsBaseType(Type t)
		{
			foreach (Type bt in BASE_TYPES)
			{
				if (bt == t)
					return true;
			}

			return false;
		}

		public string Expression
		{
			get	{return tbExpr.Text; }
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
			this.tbExpr = new System.Windows.Forms.TextBox();
			this.bCancel = new System.Windows.Forms.Button();
			this.tvOp = new System.Windows.Forms.TreeView();
			this.lExpr = new System.Windows.Forms.Label();
			this.lOp = new System.Windows.Forms.Label();
			this.bCopy = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// bOK
			// 
			this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bOK.Location = new System.Drawing.Point(264, 280);
			this.bOK.Name = "bOK";
			this.bOK.TabIndex = 3;
			this.bOK.Text = "OK";
			// 
			// tbExpr
			// 
			this.tbExpr.AcceptsReturn = true;
			this.tbExpr.AcceptsTab = true;
			this.tbExpr.Location = new System.Drawing.Point(208, 32);
			this.tbExpr.Multiline = true;
			this.tbExpr.Name = "tbExpr";
			this.tbExpr.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbExpr.Size = new System.Drawing.Size(224, 232);
			this.tbExpr.TabIndex = 2;
			this.tbExpr.Text = "";
			this.tbExpr.WordWrap = false;
			// 
			// bCancel
			// 
			this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(352, 280);
			this.bCancel.Name = "bCancel";
			this.bCancel.TabIndex = 4;
			this.bCancel.Text = "Cancel";
			// 
			// tvOp
			// 
			this.tvOp.ImageIndex = -1;
			this.tvOp.Location = new System.Drawing.Point(8, 32);
			this.tvOp.Name = "tvOp";
			this.tvOp.SelectedImageIndex = -1;
			this.tvOp.Size = new System.Drawing.Size(152, 224);
			this.tvOp.TabIndex = 0;
			// 
			// lExpr
			// 
			this.lExpr.Location = new System.Drawing.Point(216, 8);
			this.lExpr.Name = "lExpr";
			this.lExpr.Size = new System.Drawing.Size(208, 16);
			this.lExpr.TabIndex = 12;
			this.lExpr.Text = "Expressions start with \'=\'";
			// 
			// lOp
			// 
			this.lOp.Location = new System.Drawing.Point(8, 8);
			this.lOp.Name = "lOp";
			this.lOp.Size = new System.Drawing.Size(136, 16);
			this.lOp.TabIndex = 13;
			this.lOp.Text = "Select and hit \'>>\'";
			// 
			// bCopy
			// 
			this.bCopy.Location = new System.Drawing.Point(168, 48);
			this.bCopy.Name = "bCopy";
			this.bCopy.Size = new System.Drawing.Size(32, 23);
			this.bCopy.TabIndex = 1;
			this.bCopy.Text = ">>";
			this.bCopy.Click += new System.EventHandler(this.bCopy_Click);
			// 
			// DialogExprEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(440, 310);
			this.Controls.Add(this.bCopy);
			this.Controls.Add(this.lOp);
			this.Controls.Add(this.lExpr);
			this.Controls.Add(this.tvOp);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.tbExpr);
			this.Controls.Add(this.bOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DialogExprEditor";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Expression";
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.DialogExprEditor_Layout);
			this.ResumeLayout(false);

		}
		#endregion

		private void bCopy_Click(object sender, System.EventArgs e)
		{
			if (tvOp.SelectedNode == null ||
				tvOp.SelectedNode.Parent == null)
				return;		// this is the top level nodes (Fields, Parameters, ...)

			TreeNode node = tvOp.SelectedNode;
			string t = node.Text;
			tbExpr.SelectedText = t;
		}

		private void DialogExprEditor_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{
			int bottom = this.Height - bOK.Top + 5;

			tvOp.Height = tbExpr.Height = this.Height - tbExpr.Top - bottom;

			tbExpr.Left = this.Width / 2;
			lExpr.Left = tbExpr.Left;
			tbExpr.Width = this.Width / 2 - 16;

			tvOp.Width = this.Width / 2 - tvOp.Left - bCopy.Width - 8;
			bCopy.Left = tvOp.Left + tvOp.Width + 4;
		}

	}

}
