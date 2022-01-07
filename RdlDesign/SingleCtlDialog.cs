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

namespace fyiReporting.RdlDesign
{
	internal enum SingleCtlTypeEnum
	{
		NoneForNow			// don't have a single ctl dialog right now
	}

	/// <summary>
	/// Summary description for PropertyDialog.
	/// </summary>
	internal class SingleCtlDialog : System.Windows.Forms.Form
	{
		private DesignXmlDraw _Draw;		// design draw 
		private List<XmlNode> _Nodes;			// selected nodes
		private SingleCtlTypeEnum _Type;	
		IProperty _Ctl;
		private bool _Changed=false;		
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bApply;
		private System.Windows.Forms.Panel panelMain;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal SingleCtlDialog(DesignXmlDraw dxDraw, List<XmlNode> sNodes, SingleCtlTypeEnum type)
		{
			this._Draw = dxDraw;
			this._Nodes = sNodes;
			this._Type = type;
			_Ctl = null;			// when we have a case this should be initialized
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//   Add the controls for the selected ReportItems
		}

		internal bool Changed
		{
			get {return _Changed; }
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.bApply = new System.Windows.Forms.Button();
			this.bOK = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.panelMain = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.CausesValidation = false;
			this.panel1.Controls.Add(this.bApply);
			this.panel1.Controls.Add(this.bOK);
			this.panel1.Controls.Add(this.bCancel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 310);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(458, 56);
			this.panel1.TabIndex = 1;
			// 
			// bApply
			// 
			this.bApply.Location = new System.Drawing.Point(376, 16);
			this.bApply.Name = "bApply";
			this.bApply.TabIndex = 2;
			this.bApply.Text = "Apply";
			this.bApply.Click += new System.EventHandler(this.bApply_Click);
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(200, 17);
			this.bOK.Name = "bOK";
			this.bOK.TabIndex = 0;
			this.bOK.Text = "OK";
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// bCancel
			// 
			this.bCancel.CausesValidation = false;
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(288, 16);
			this.bCancel.Name = "bCancel";
			this.bCancel.TabIndex = 1;
			this.bCancel.Text = "Cancel";
			// 
			// panelMain
			// 
			this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMain.Location = new System.Drawing.Point(0, 0);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(458, 310);
			this.panelMain.TabIndex = 2;
			// 
			// PropertyDialog
			// 
			this.AcceptButton = this.bOK;
            this.AutoScaleMode = AutoScaleMode.None;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(458, 366);
			this.Controls.Add(this.panelMain);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PropertyDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Properties";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.PropertyDialog_Closing);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void bApply_Click(object sender, System.EventArgs e)
		{
			this._Changed = true;
			_Ctl.Apply();
			this._Draw.Invalidate();		// Force screen to redraw
		}

		private void bOK_Click(object sender, System.EventArgs e)
		{
			bApply_Click(sender, e);	// Apply does all the work
			this.DialogResult = DialogResult.OK;
		}

		private void PropertyDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		}
	}
}
