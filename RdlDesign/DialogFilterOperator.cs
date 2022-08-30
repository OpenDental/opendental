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
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Text;
using System.Reflection;
using fyiReporting.RDL;

namespace fyiReporting.RdlDesign
{
	/// <summary>
	/// DialogFilterOperator: puts up a dialog that lets a user pick a Filter Operator
	/// </summary>
	public class DialogFilterOperator : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.Label lOp;
		private System.Windows.Forms.ComboBox cbOperator;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal DialogFilterOperator(string op)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			if (op != null && op.Length > 0)
				cbOperator.Text = op;

			return;
		}

		public string Operator
		{
			get	{return this.cbOperator.Text; }
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
			this.lOp = new System.Windows.Forms.Label();
			this.cbOperator = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// bOK
			// 
			this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bOK.Location = new System.Drawing.Point(88, 168);
			this.bOK.Name = "bOK";
			this.bOK.TabIndex = 3;
			this.bOK.Text = "OK";
			// 
			// bCancel
			// 
			this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bCancel.CausesValidation = false;
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(176, 168);
			this.bCancel.Name = "bCancel";
			this.bCancel.TabIndex = 4;
			this.bCancel.Text = "Cancel";
			// 
			// lOp
			// 
			this.lOp.Location = new System.Drawing.Point(8, 8);
			this.lOp.Name = "lOp";
			this.lOp.Size = new System.Drawing.Size(112, 16);
			this.lOp.TabIndex = 13;
			this.lOp.Text = "Select Filter Operator";
			// 
			// cbOperator
			// 
			this.cbOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this.cbOperator.Items.AddRange(new object[] {
															"Equal",
															"Like",
															"NotEqual",
															"GreaterThan",
															"GreaterThanOrEqual",
															"LessThan",
															"LessThanOrEqual",
															"TopN",
															"BottomN",
															"TopPercent",
															"BottomPercent",
															"In",
															"Between"});
			this.cbOperator.Location = new System.Drawing.Point(120, 8);
			this.cbOperator.Name = "cbOperator";
			this.cbOperator.Size = new System.Drawing.Size(128, 150);
			this.cbOperator.TabIndex = 14;
			this.cbOperator.Text = "Equal";
			this.cbOperator.Validating += new System.ComponentModel.CancelEventHandler(this.DialogFilterOperator_Validating);
			// 
			// DialogFilterOperator
			// 
			this.AcceptButton = this.bOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(256, 198);
			this.Controls.Add(this.cbOperator);
			this.Controls.Add(this.lOp);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DialogFilterOperator";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Pick Filter Operator";
			this.Validating += new System.ComponentModel.CancelEventHandler(this.DialogFilterOperator_Validating);
			this.ResumeLayout(false);

		}
		#endregion

		private void DialogFilterOperator_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (string op in cbOperator.Items)
			{
				if (op == cbOperator.Text)
					return;
			}
			MessageBox.Show(string.Format("Operator '{0}' must be in the operator list", cbOperator.Text), "Pick Filter Operator");
			e.Cancel = true;
		}

	}

}
