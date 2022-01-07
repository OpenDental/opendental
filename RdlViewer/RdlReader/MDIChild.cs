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
using System.Data;
using System.Drawing.Printing;
using fyiReporting.RDL;
using fyiReporting.RdlViewer;

namespace fyiReporting.RdlReader
{
	/// <summary>
	/// RdlReader is a application for displaying reports based on RDL.
	/// </summary>
	public class MDIChild : Form
	{
		private fyiReporting.RdlViewer.RdlViewer rdlViewer1;

		public MDIChild(int width, int height)
		{
			this.rdlViewer1 = new fyiReporting.RdlViewer.RdlViewer();
			this.SuspendLayout();
			// 
			// rdlViewer1
			// 
			this.rdlViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rdlViewer1.Location = new System.Drawing.Point(0, 0);
			this.rdlViewer1.Name = "rdlViewer1";
			this.rdlViewer1.Size = new System.Drawing.Size(width, height);
			this.rdlViewer1.TabIndex = 0;
			// 
			// RdlReader
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(width, height);
			this.Controls.Add(this.rdlViewer1);
			this.Name = "";
			this.Text = "";
			this.ResumeLayout(false);
		}

		/// <summary>
		/// The RDL file that should be displayed.
		/// </summary>
		public string SourceFile
		{
			get {return this.rdlViewer1.SourceFile;}
			set 
			{
				this.rdlViewer1.SourceFile = value;
				this.rdlViewer1.Refresh();		// force the repaint
			}
		}

		public RdlViewer.RdlViewer Viewer
		{
			get {return this.rdlViewer1;}
		}
	}
}
