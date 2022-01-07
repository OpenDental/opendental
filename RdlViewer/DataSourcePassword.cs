/* ====================================================================
    Copyright (C) 2004-2006  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    This library is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 2.1 of the License, or
    (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA

    For additional information, email info@fyireporting.com or visit
    the website www.fyiReporting.com.
*/
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using fyiReporting.RDL;
using fyiReporting.RdlViewer;

namespace fyiReporting.RdlViewer
{
	/// <summary>
	/// Summary description for ZoomTo.
	/// </summary>
	public class DataSourcePassword : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.TextBox tbPassPhrase;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DataSourcePassword()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataSourcePassword));
			this.label1 = new System.Windows.Forms.Label();
			this.bOK = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.tbPassPhrase = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16,16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(280,23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Please enter the pass phrase for shared data sources";
			// 
			// bOK
			// 
			this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bOK.Location = new System.Drawing.Point(112,80);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(75,23);
			this.bOK.TabIndex = 2;
			this.bOK.Text = "OK";
			// 
			// bCancel
			// 
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(208,80);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(75,23);
			this.bCancel.TabIndex = 3;
			this.bCancel.Text = "Cancel";
			// 
			// tbPassPhrase
			// 
			this.tbPassPhrase.Location = new System.Drawing.Point(16,40);
			this.tbPassPhrase.Name = "tbPassPhrase";
			this.tbPassPhrase.PasswordChar = '*';
			this.tbPassPhrase.Size = new System.Drawing.Size(272,20);
			this.tbPassPhrase.TabIndex = 1;
			// 
			// DataSourcePassword
			// 
			this.AcceptButton = this.bOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(306,112);
			this.Controls.Add(this.tbPassPhrase);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bOK);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataSourcePassword";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Shared Data Sources";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		public string PassPhrase
		{
			get {return tbPassPhrase.Text;}
		}
	}
}
