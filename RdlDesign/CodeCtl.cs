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
using System.IO;
using System.Threading;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;

namespace fyiReporting.RdlDesign
{
	/// <summary>
	/// Summary description for CodeCtl.
	/// </summary>
	internal class CodeCtl : System.Windows.Forms.UserControl, IProperty
	{
		static internal long Counter;			// counter used for unique expression count
		private DesignXmlDraw _Draw;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button bCheckSyntax;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox tbCode;
		private System.Windows.Forms.ListBox lbErrors;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Label label2;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal CodeCtl(DesignXmlDraw dxDraw)
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
			XmlNode cNode = _Draw.GetNamedChildNode(rNode, "Code");
			tbCode.Text = "";
			if (cNode == null)
				return;

			StringReader tr = new StringReader(cNode.InnerText);
			List<string> ar = new List<string>();
			while (tr.Peek() >= 0)
			{
				string line = tr.ReadLine();
				ar.Add(line);
			}
			tr.Close();

        //    tbCode.Lines = ar.ToArray("".GetType()) as string[];
            tbCode.Lines = ar.ToArray();
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
			this.bCheckSyntax = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tbCode = new System.Windows.Forms.TextBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.lbErrors = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(98, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(144, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Visual Basic Function Code";
			// 
			// bCheckSyntax
			// 
			this.bCheckSyntax.Location = new System.Drawing.Point(365, 3);
			this.bCheckSyntax.Name = "bCheckSyntax";
			this.bCheckSyntax.Size = new System.Drawing.Size(82, 23);
			this.bCheckSyntax.TabIndex = 2;
			this.bCheckSyntax.Text = "Check Syntax";
			this.bCheckSyntax.Click += new System.EventHandler(this.bCheckSyntax_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tbCode);
			this.panel1.Controls.Add(this.splitter1);
			this.panel1.Controls.Add(this.lbErrors);
			this.panel1.Location = new System.Drawing.Point(13, 31);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(439, 252);
			this.panel1.TabIndex = 4;
			// 
			// tbCode
			// 
			this.tbCode.AcceptsReturn = true;
			this.tbCode.AcceptsTab = true;
			this.tbCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbCode.HideSelection = false;
			this.tbCode.Location = new System.Drawing.Point(87, 0);
			this.tbCode.Multiline = true;
			this.tbCode.Name = "tbCode";
			this.tbCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbCode.Size = new System.Drawing.Size(352, 252);
			this.tbCode.TabIndex = 2;
			this.tbCode.Text = "";
			this.tbCode.WordWrap = false;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(82, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(5, 252);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// lbErrors
			// 
			this.lbErrors.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbErrors.HorizontalScrollbar = true;
			this.lbErrors.IntegralHeight = false;
			this.lbErrors.Location = new System.Drawing.Point(0, 0);
			this.lbErrors.Name = "lbErrors";
			this.lbErrors.Size = new System.Drawing.Size(82, 252);
			this.lbErrors.TabIndex = 0;
			this.lbErrors.SelectedIndexChanged += new System.EventHandler(this.lbErrors_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(15, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(69, 15);
			this.label2.TabIndex = 5;
			this.label2.Text = "Msgs";
			// 
			// CodeCtl
			// 
			this.Controls.Add(this.label2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.bCheckSyntax);
			this.Controls.Add(this.label1);
			this.Name = "CodeCtl";
			this.Size = new System.Drawing.Size(472, 288);
			this.panel1.ResumeLayout(false);
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
			if (tbCode.Text.Trim().Length > 0)
				_Draw.SetElement(rNode, "Code", tbCode.Text);
			else
				_Draw.RemoveElement(rNode, "Code");
		}

		private void bCheckSyntax_Click(object sender, System.EventArgs e)
		{
			CheckAssembly();	
		}
		
		private void CheckAssembly()
		{
			lbErrors.Items.Clear();					// clear out existing items

			// Generate the proxy source code
			List<string> lines = new List<string>();		// hold lines in array in case of error

			VBCodeProvider vbcp =  new VBCodeProvider();
			StringBuilder sb = new StringBuilder();
			//  Generate code with the following general form

			//Imports System
			//Namespace fyiReportingvbgen
			//Public Class MyClassn	   // where n is a uniquely generated integer
			//Sub New()
			//End Sub
			//  ' this is the code in the <Code> tag
			//End Class
			//End Namespace
			string unique = Interlocked.Increment(ref CodeCtl.Counter).ToString();
			lines.Add("Imports System");
			lines.Add("Namespace fyiReporting.vbgen");
			string className = "MyClass" + unique;
			lines.Add("Public Class " + className);
			lines.Add("Sub New()");
			lines.Add("End Sub");
			// Read and write code as lines
			StringReader tr = new StringReader(this.tbCode.Text);
			while (tr.Peek() >= 0)
			{
				string line = tr.ReadLine();
				lines.Add(line);
			}
			tr.Close();
			lines.Add("End Class");
			lines.Add("End Namespace");
			foreach (string l in lines)
				sb.AppendFormat(l + Environment.NewLine);

			string vbcode = sb.ToString();

			// Create Assembly
			CompilerParameters cp = new CompilerParameters();
			cp.ReferencedAssemblies.Add("System.dll");
			cp.GenerateExecutable = false;
			cp.GenerateInMemory = true;
			cp.IncludeDebugInformation = false; 
			
			CompilerResults cr = vbcp.CompileAssemblyFromSource(cp, vbcode);
			if(cr.Errors.Count > 0)
			{
				StringBuilder err = new StringBuilder(string.Format("Code has {0} error(s).", cr.Errors.Count));
				foreach (CompilerError ce in cr.Errors)
				{
					lbErrors.Items.Add(string.Format("Ln {0}- {1}", ce.Line - 5, ce.ErrorText));
				}
			}
			else
				MessageBox.Show("No errors", "Code Verification");

			return ;
		}

		private void lbErrors_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lbErrors.Items.Count == 0)
				return;
						 
			string line = lbErrors.Items[lbErrors.SelectedIndex] as string;
			if (!line.StartsWith("Ln"))
				return;

			int l = line.IndexOf('-');
			if (l < 0)
				return;
			line = line.Substring(3, l-3);
			try
			{
				int i = Convert.ToInt32(line);
				Goto(i);
			}
			catch {}		// we don't care about the error
			return;
		}

		public void Goto(int nLine)
		{	
			int offset = 0; 
			nLine = Math.Min(nLine, tbCode.Lines.Length);		// don't go off the end

			for ( int i = 0; i < nLine - 1 && i < tbCode.Lines.Length; ++i ) 
				offset += (this.tbCode.Lines[i].Length + 2); 

			Control savectl = this.ActiveControl;
			tbCode.Focus(); 
			tbCode.Select( offset, this.tbCode.Lines[nLine > 0? nLine-1: 0].Length);
			this.ActiveControl = savectl;
		}


	}
}
