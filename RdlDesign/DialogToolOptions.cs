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
using System.Reflection;
using System.Xml;

namespace fyiReporting.RdlDesign
{
	/// <summary>
	/// Summary description for DialogAbout.
	/// </summary>
	public class DialogToolOptions : System.Windows.Forms.Form
	{
		RdlDesigner _RdlDesigner;
		bool bDesktop=false;
		bool bToolbar=false;

		// Desktop server configuration
		XmlDocument _DesktopDocument;
		XmlNode _DesktopPort;
		XmlNode _DesktopDirectory;
		XmlNode _DesktopLocal;

		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tpGeneral;
		private System.Windows.Forms.TabPage tpToolbar;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbRecentFilesMax;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbHelpUrl;
		private System.Windows.Forms.ListBox lbOperation;
		private System.Windows.Forms.ListBox lbToolbar;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button bCopyItem;
		private System.Windows.Forms.Button bUp;
		private System.Windows.Forms.Button bDown;
		private System.Windows.Forms.Button bReset;
		private System.Windows.Forms.Button bRemove;
		private System.Windows.Forms.Button bApply;
		private System.Windows.Forms.TabPage tpDesktop;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbPort;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox tbDirectory;
		private System.Windows.Forms.CheckBox ckLocal;
        private System.Windows.Forms.Button bBrowse;
        private CheckBox cbEditLines;
        private CheckBox cbOutline;
        private CheckBox cbTabInterface;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DialogToolOptions(RdlDesigner rdl)
		{
			_RdlDesigner = rdl;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			Init();
			return;
		}

		private void Init()
		{
			this.tbRecentFilesMax.Text = _RdlDesigner.RecentFilesMax.ToString();
			this.tbHelpUrl.Text = _RdlDesigner.HelpUrl;

			// init the toolbar

			// list of items in current toolbar
			foreach (string ti in _RdlDesigner.Toolbar)
			{
				this.lbToolbar.Items.Add(ti);
			}

            this.cbEditLines.Checked = _RdlDesigner.ShowEditLines;
            this.cbOutline.Checked = _RdlDesigner.ShowReportItemOutline;
            this.cbTabInterface.Checked = _RdlDesigner.ShowTabbedInterface;

			InitOperations();

			InitDesktop();

			bDesktop = bToolbar = false;			// start with no changes
		}

		private void InitDesktop()
		{
			string optFileName = AppDomain.CurrentDomain.BaseDirectory + "config.xml";
			
			try
			{
				XmlDocument xDoc = _DesktopDocument = new XmlDocument();
				xDoc.PreserveWhitespace = true;
				xDoc.Load(optFileName);
				XmlNode xNode;
				xNode = xDoc.SelectSingleNode("//config");

				// Loop thru all the child nodes
				foreach(XmlNode xNodeLoop in xNode.ChildNodes)
				{
					if (xNodeLoop.NodeType != XmlNodeType.Element)
						continue;
					switch (xNodeLoop.Name.ToLower())
					{
						case "port":
							this.tbPort.Text = xNodeLoop.InnerText;
							_DesktopPort = xNodeLoop;
							break;
						case "localhostonly":
							string tf = xNodeLoop.InnerText.ToLower();
							this.ckLocal.Checked = !(tf == "false");
							_DesktopLocal = xNodeLoop;
							break;
						case "serverroot":
							this.tbDirectory.Text = xNodeLoop.InnerText;
							_DesktopDirectory = xNodeLoop;
							break;
						case "cachedirectory":
							// wd = xNodeLoop.InnerText;
							break;
						case "tracelevel":
							break;
						case "maxreadcache":
							break;
						case "maxreadcacheentrysize":
							break;
						case "mimetypes":
							break;
						default:
							break;
					}
				}
			}
			catch (Exception ex)
			{		// Didn't sucessfully get the startup state: use defaults
				MessageBox.Show(string.Format("Error processing Desktop Configuration; using defaults.\n{0}", ex.Message), "Options");
				this.tbPort.Text = "8080";
				this.ckLocal.Checked = true;
				this.tbDirectory.Text = "Examples";
			}

		}

		private void InitOperations()
		{
			// list of operations; 
			lbOperation.Items.Clear();

			List<string> dups = _RdlDesigner.ToolbarAllowDups;
			foreach (string ti in _RdlDesigner.ToolbarOperations)
			{
				// if item is allowed to be duplicated or if
				//   item has not already been used we add to operation list
				if (dups.Contains(ti) || !lbToolbar.Items.Contains(ti))
					this.lbOperation.Items.Add(ti);
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogToolOptions));
            this.bOK = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpGeneral = new System.Windows.Forms.TabPage();
            this.cbTabInterface = new System.Windows.Forms.CheckBox();
            this.cbOutline = new System.Windows.Forms.CheckBox();
            this.cbEditLines = new System.Windows.Forms.CheckBox();
            this.tbHelpUrl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbRecentFilesMax = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tpToolbar = new System.Windows.Forms.TabPage();
            this.bRemove = new System.Windows.Forms.Button();
            this.bReset = new System.Windows.Forms.Button();
            this.bDown = new System.Windows.Forms.Button();
            this.bUp = new System.Windows.Forms.Button();
            this.bCopyItem = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbToolbar = new System.Windows.Forms.ListBox();
            this.lbOperation = new System.Windows.Forms.ListBox();
            this.tpDesktop = new System.Windows.Forms.TabPage();
            this.bBrowse = new System.Windows.Forms.Button();
            this.tbDirectory = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ckLocal = new System.Windows.Forms.CheckBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.bApply = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            this.tpToolbar.SuspendLayout();
            this.tpDesktop.SuspendLayout();
            this.SuspendLayout();
            // 
            // bOK
            // 
            this.bOK.Location = new System.Drawing.Point(210, 275);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 1;
            this.bOK.Text = "OK";
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // bCancel
            // 
            this.bCancel.CausesValidation = false;
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(298, 275);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 2;
            this.bCancel.Text = "Cancel";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpGeneral);
            this.tabControl1.Controls.Add(this.tpToolbar);
            this.tabControl1.Controls.Add(this.tpDesktop);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(466, 269);
            this.tabControl1.TabIndex = 0;
            // 
            // tpGeneral
            // 
            this.tpGeneral.Controls.Add(this.cbTabInterface);
            this.tpGeneral.Controls.Add(this.cbOutline);
            this.tpGeneral.Controls.Add(this.cbEditLines);
            this.tpGeneral.Controls.Add(this.tbHelpUrl);
            this.tpGeneral.Controls.Add(this.label3);
            this.tpGeneral.Controls.Add(this.label2);
            this.tpGeneral.Controls.Add(this.tbRecentFilesMax);
            this.tpGeneral.Controls.Add(this.label1);
            this.tpGeneral.Location = new System.Drawing.Point(4, 22);
            this.tpGeneral.Name = "tpGeneral";
            this.tpGeneral.Size = new System.Drawing.Size(458, 243);
            this.tpGeneral.TabIndex = 0;
            this.tpGeneral.Tag = "general";
            this.tpGeneral.Text = "General";
            // 
            // cbTabInterface
            // 
            this.cbTabInterface.AutoSize = true;
            this.cbTabInterface.Location = new System.Drawing.Point(14, 189);
            this.cbTabInterface.Name = "cbTabInterface";
            this.cbTabInterface.Size = new System.Drawing.Size(133, 17);
            this.cbTabInterface.TabIndex = 7;
            this.cbTabInterface.Text = "Show tabbed interface";
            this.cbTabInterface.UseVisualStyleBackColor = true;
            this.cbTabInterface.CheckedChanged += new System.EventHandler(this.cbTabInterface_CheckedChanged);
            // 
            // cbOutline
            // 
            this.cbOutline.AutoSize = true;
            this.cbOutline.Location = new System.Drawing.Point(14, 146);
            this.cbOutline.Name = "cbOutline";
            this.cbOutline.Size = new System.Drawing.Size(172, 17);
            this.cbOutline.TabIndex = 6;
            this.cbOutline.Text = "Outline report items in Designer";
            this.cbOutline.UseVisualStyleBackColor = true;
            // 
            // cbEditLines
            // 
            this.cbEditLines.AutoSize = true;
            this.cbEditLines.Location = new System.Drawing.Point(14, 105);
            this.cbEditLines.Name = "cbEditLines";
            this.cbEditLines.Size = new System.Drawing.Size(175, 17);
            this.cbEditLines.TabIndex = 5;
            this.cbEditLines.Text = "Show line numbers in RDL Text";
            this.cbEditLines.UseVisualStyleBackColor = true;
            // 
            // tbHelpUrl
            // 
            this.tbHelpUrl.Location = new System.Drawing.Point(27, 60);
            this.tbHelpUrl.Name = "tbHelpUrl";
            this.tbHelpUrl.Size = new System.Drawing.Size(404, 20);
            this.tbHelpUrl.TabIndex = 4;
            this.tbHelpUrl.Text = "tbHelpUrl";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(11, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(404, 23);
            this.label3.TabIndex = 3;
            this.label3.Text = "Help System URL  (Empty string means use default help url.)";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(100, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "items in most recently used lists.";
            // 
            // tbRecentFilesMax
            // 
            this.tbRecentFilesMax.Location = new System.Drawing.Point(58, 11);
            this.tbRecentFilesMax.Name = "tbRecentFilesMax";
            this.tbRecentFilesMax.Size = new System.Drawing.Size(31, 20);
            this.tbRecentFilesMax.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(11, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Display";
            // 
            // tpToolbar
            // 
            this.tpToolbar.Controls.Add(this.bRemove);
            this.tpToolbar.Controls.Add(this.bReset);
            this.tpToolbar.Controls.Add(this.bDown);
            this.tpToolbar.Controls.Add(this.bUp);
            this.tpToolbar.Controls.Add(this.bCopyItem);
            this.tpToolbar.Controls.Add(this.label5);
            this.tpToolbar.Controls.Add(this.label4);
            this.tpToolbar.Controls.Add(this.lbToolbar);
            this.tpToolbar.Controls.Add(this.lbOperation);
            this.tpToolbar.Location = new System.Drawing.Point(4, 22);
            this.tpToolbar.Name = "tpToolbar";
            this.tpToolbar.Size = new System.Drawing.Size(458, 243);
            this.tpToolbar.TabIndex = 1;
            this.tpToolbar.Tag = "toolbar";
            this.tpToolbar.Text = "Toolbar";
            // 
            // bRemove
            // 
            this.bRemove.Location = new System.Drawing.Point(179, 74);
            this.bRemove.Name = "bRemove";
            this.bRemove.Size = new System.Drawing.Size(23, 23);
            this.bRemove.TabIndex = 2;
            this.bRemove.Text = "<";
            this.bRemove.Click += new System.EventHandler(this.bRemove_Click);
            // 
            // bReset
            // 
            this.bReset.Location = new System.Drawing.Point(374, 104);
            this.bReset.Name = "bReset";
            this.bReset.Size = new System.Drawing.Size(53, 23);
            this.bReset.TabIndex = 6;
            this.bReset.Text = "Reset";
            this.bReset.Click += new System.EventHandler(this.bReset_Click);
            // 
            // bDown
            // 
            this.bDown.Location = new System.Drawing.Point(374, 65);
            this.bDown.Name = "bDown";
            this.bDown.Size = new System.Drawing.Size(53, 23);
            this.bDown.TabIndex = 5;
            this.bDown.Text = "Down";
            this.bDown.Click += new System.EventHandler(this.bDown_Click);
            // 
            // bUp
            // 
            this.bUp.Location = new System.Drawing.Point(374, 35);
            this.bUp.Name = "bUp";
            this.bUp.Size = new System.Drawing.Size(53, 23);
            this.bUp.TabIndex = 4;
            this.bUp.Text = "Up";
            this.bUp.Click += new System.EventHandler(this.bUp_Click);
            // 
            // bCopyItem
            // 
            this.bCopyItem.Location = new System.Drawing.Point(179, 40);
            this.bCopyItem.Name = "bCopyItem";
            this.bCopyItem.Size = new System.Drawing.Size(23, 23);
            this.bCopyItem.TabIndex = 1;
            this.bCopyItem.Text = ">";
            this.bCopyItem.Click += new System.EventHandler(this.bCopyItem_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(213, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 23);
            this.label5.TabIndex = 8;
            this.label5.Text = "Toolbar Layout";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(15, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 23);
            this.label4.TabIndex = 7;
            this.label4.Text = "ToolBar Operations";
            // 
            // lbToolbar
            // 
            this.lbToolbar.Location = new System.Drawing.Point(213, 33);
            this.lbToolbar.Name = "lbToolbar";
            this.lbToolbar.Size = new System.Drawing.Size(152, 199);
            this.lbToolbar.TabIndex = 3;
            // 
            // lbOperation
            // 
            this.lbOperation.Location = new System.Drawing.Point(14, 33);
            this.lbOperation.Name = "lbOperation";
            this.lbOperation.Size = new System.Drawing.Size(152, 199);
            this.lbOperation.TabIndex = 0;
            // 
            // tpDesktop
            // 
            this.tpDesktop.Controls.Add(this.bBrowse);
            this.tpDesktop.Controls.Add(this.tbDirectory);
            this.tpDesktop.Controls.Add(this.label9);
            this.tpDesktop.Controls.Add(this.label8);
            this.tpDesktop.Controls.Add(this.label7);
            this.tpDesktop.Controls.Add(this.ckLocal);
            this.tpDesktop.Controls.Add(this.tbPort);
            this.tpDesktop.Controls.Add(this.label6);
            this.tpDesktop.Location = new System.Drawing.Point(4, 22);
            this.tpDesktop.Name = "tpDesktop";
            this.tpDesktop.Size = new System.Drawing.Size(458, 243);
            this.tpDesktop.TabIndex = 2;
            this.tpDesktop.Tag = "desktop";
            this.tpDesktop.Text = "Desktop Server";
            // 
            // bBrowse
            // 
            this.bBrowse.Location = new System.Drawing.Point(411, 102);
            this.bBrowse.Name = "bBrowse";
            this.bBrowse.Size = new System.Drawing.Size(26, 19);
            this.bBrowse.TabIndex = 2;
            this.bBrowse.Text = "...";
            this.bBrowse.Click += new System.EventHandler(this.bBrowse_Click);
            // 
            // tbDirectory
            // 
            this.tbDirectory.Location = new System.Drawing.Point(68, 100);
            this.tbDirectory.Name = "tbDirectory";
            this.tbDirectory.Size = new System.Drawing.Size(334, 20);
            this.tbDirectory.TabIndex = 1;
            this.tbDirectory.Text = "textBox1";
            this.tbDirectory.TextChanged += new System.EventHandler(this.Desktop_Changed);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(13, 101);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 23);
            this.label9.TabIndex = 5;
            this.label9.Text = "Directory:";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(33, 156);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(414, 26);
            this.label8.TabIndex = 4;
            this.label8.Text = "Important: Desktop server is not intended to be used as a production web server. " +
                " Use an ASP enabled server for anything other than local desktop use.";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(10, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(435, 57);
            this.label7.TabIndex = 3;
            this.label7.Text = resources.GetString("label7.Text");
            // 
            // ckLocal
            // 
            this.ckLocal.Location = new System.Drawing.Point(15, 131);
            this.ckLocal.Name = "ckLocal";
            this.ckLocal.Size = new System.Drawing.Size(190, 24);
            this.ckLocal.TabIndex = 3;
            this.ckLocal.Text = "Restrict access to local machine";
            this.ckLocal.CheckedChanged += new System.EventHandler(this.Desktop_Changed);
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(44, 66);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(50, 20);
            this.tbPort.TabIndex = 0;
            this.tbPort.TextChanged += new System.EventHandler(this.Desktop_Changed);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(10, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 23);
            this.label6.TabIndex = 0;
            this.label6.Text = "Port";
            // 
            // bApply
            // 
            this.bApply.Location = new System.Drawing.Point(386, 275);
            this.bApply.Name = "bApply";
            this.bApply.Size = new System.Drawing.Size(75, 23);
            this.bApply.TabIndex = 3;
            this.bApply.Text = "Apply";
            this.bApply.Click += new System.EventHandler(this.bApply_Click);
            // 
            // DialogToolOptions
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(466, 304);
            this.Controls.Add(this.bApply);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogToolOptions";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.tabControl1.ResumeLayout(false);
            this.tpGeneral.ResumeLayout(false);
            this.tpGeneral.PerformLayout();
            this.tpToolbar.ResumeLayout(false);
            this.tpDesktop.ResumeLayout(false);
            this.tpDesktop.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private bool Verify()
		{
			try
			{
				int i = Convert.ToInt32(this.tbRecentFilesMax.Text);
				return (i >= 1 || i <= 50);
			}
			catch
			{
				MessageBox.Show("Recent files maximum must be an integer between 1 and 50", "Options");
				return false;
			}
		}

		private void bOK_Click(object sender, System.EventArgs e)
		{
			if (DoApply())
			{
				DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private bool DoApply()
		{
			lock(this)
			{
				try
				{
					if (!Verify())
						return false;
					HandleRecentFilesMax();
					_RdlDesigner.HelpUrl = this.tbHelpUrl.Text;
                    HandleShows();
					if (bToolbar)
						HandleToolbar();
					if (bDesktop)
						HandleDesktop();
					bToolbar = bDesktop = false;		// no changes now
					return true;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Options");
					return false;
				}
			}
		}

		private void HandleDesktop()
		{
			if (_DesktopDocument == null)
			{
				_DesktopDocument = new XmlDocument();
				XmlProcessingInstruction xPI;
				xPI = _DesktopDocument.CreateProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
				_DesktopDocument.AppendChild(xPI);
			}

			if (_DesktopPort == null)
			{
				_DesktopPort = _DesktopDocument.CreateElement("port");
				_DesktopDocument.AppendChild(_DesktopPort);
			}
			_DesktopPort.InnerText = this.tbPort.Text;

			if (_DesktopDirectory == null)
			{
				_DesktopDirectory = _DesktopDocument.CreateElement("serverroot");
				_DesktopDocument.AppendChild(_DesktopDirectory);
			}
			_DesktopDirectory.InnerText = this.tbDirectory.Text;

			if (_DesktopLocal == null)
			{
				_DesktopLocal = _DesktopDocument.CreateElement("localhostonly");
				_DesktopDocument.AppendChild(_DesktopLocal);
			}
			_DesktopLocal.InnerText = this.ckLocal.Checked? "true": "false";

			string optFileName = AppDomain.CurrentDomain.BaseDirectory + "config.xml";

			_DesktopDocument.Save(optFileName);
			this._RdlDesigner.menuToolsCloseProcess(false);		// close the server
		}

		private void HandleRecentFilesMax()
		{
			// Handle the RecentFilesMax
			int i = Convert.ToInt32(this.tbRecentFilesMax.Text);
			if (i < 1 || i > 50)
				throw new Exception("Recent files maximum must be an integer between 1 and 50");
			if (this._RdlDesigner.RecentFilesMax == i)	// if not different we don't need to do anything
				return;

			this._RdlDesigner.RecentFilesMax = i;

			// Make the list match the maximum size
			bool bChangeMenu=false;
			while (_RdlDesigner.RecentFiles.Count > _RdlDesigner.RecentFilesMax)
			{
				_RdlDesigner.RecentFiles.RemoveAt(0);	// remove the first entry
				bChangeMenu = true;
			}

			if (bChangeMenu)
				_RdlDesigner.RecentFilesMenu();			// reset the menu since the list changed
			return;
		}

		private void HandleToolbar()
		{
            List<string> ar = new List<string>();
			foreach (string item in this.lbToolbar.Items)
				ar.Add(item);
			this._RdlDesigner.Toolbar = ar;
		}

        private void HandleShows()
        {
            _RdlDesigner.ShowEditLines = this.cbEditLines.Checked;
            _RdlDesigner.ShowReportItemOutline = this.cbOutline.Checked;
            _RdlDesigner.ShowTabbedInterface = this.cbTabInterface.Checked;

		    foreach (MDIChild mc in _RdlDesigner.MdiChildren)
			{
                mc.ShowEditLines(this.cbEditLines.Checked);
                mc.ShowReportItemOutline = this.cbOutline.Checked;
			}

        }

		private void bCopyItem_Click(object sender, System.EventArgs e)
		{
			bToolbar=true;
			int i = this.lbOperation.SelectedIndex;
			if (i < 0)
				return;
			string itm = lbOperation.Items[i] as String;
			lbToolbar.Items.Add(itm);
			// Remove from list if not allowed to be duplicated in toolbar
			if (!_RdlDesigner.ToolbarAllowDups.Contains(itm))
				lbOperation.Items.RemoveAt(i);
		}

		private void bRemove_Click(object sender, System.EventArgs e)
		{
			bToolbar = true;
			int i = this.lbToolbar.SelectedIndex;
			if (i < 0)
				return;
			string itm = lbToolbar.Items[i] as String;
			if (itm == "Newline" || itm == "Space") 
				{}
			else
				lbOperation.Items.Add(itm);

			lbToolbar.Items.RemoveAt(i);
		}

		private void bUp_Click(object sender, System.EventArgs e)
		{
			int i = this.lbToolbar.SelectedIndex;
			if (i <= 0)
				return;

			Swap(i-1, i);
		}

		private void bDown_Click(object sender, System.EventArgs e)
		{
			int i = this.lbToolbar.SelectedIndex;
			if (i < 0 || i == lbToolbar.Items.Count-1)
				return;

			Swap(i, i+1);
		}

		/// <summary>
		/// Swap items in the toolbar listbox.  i1 should always be less than i2
		/// </summary>
		/// <param name="i1"></param>
		/// <param name="i2"></param>
		private void Swap(int i1, int i2)
		{
			bToolbar = true;
			bool b1 = (i1 == lbToolbar.SelectedIndex);

			string s1 = lbToolbar.Items[i1] as string;
			string s2 = lbToolbar.Items[i2] as string;
			lbToolbar.SuspendLayout();
			lbToolbar.Items.RemoveAt(i2);
			lbToolbar.Items.RemoveAt(i1);
			lbToolbar.Items.Insert(i1, s2);
			lbToolbar.Items.Insert(i2, s1);
			lbToolbar.SelectedIndex = b1? i2: i1;
			lbToolbar.ResumeLayout(true);
		}

		private void bReset_Click(object sender, System.EventArgs e)
		{
			bToolbar = true;

			this.lbToolbar.Items.Clear();
			List<string> ar =   this._RdlDesigner.ToolbarDefault;
			foreach (string itm in ar)
				this.lbToolbar.Items.Add(itm);

			InitOperations();
		}

		private void bApply_Click(object sender, System.EventArgs e)
		{
			DoApply();		
		}

		private void Desktop_Changed(object sender, System.EventArgs e)
		{
			bDesktop = true;
		}

		private void bBrowse_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			// Set the help text description for the FolderBrowserDialog.
			fbd.Description = 
				"Select the directory that will contain reports.";

			// Do not allow the user to create new files via the FolderBrowserDialog.
			fbd.ShowNewFolderButton = false;
			//			fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
			fbd.SelectedPath = this.tbDirectory.Text.Length == 0?
				"Examples": tbDirectory.Text;

			if (fbd.ShowDialog(this) == DialogResult.Cancel)
				return;

			tbDirectory.Text = fbd.SelectedPath;
			bDesktop = true;		// we modified Desktop settings
			return;
		}

		static internal DesktopConfig DesktopConfiguration
		{
			get 
			{
				string optFileName = AppDomain.CurrentDomain.BaseDirectory + "config.xml";
			
				DesktopConfig dc = new DesktopConfig();
				try
				{
					XmlDocument xDoc = new XmlDocument();
					xDoc.Load(optFileName);
					XmlNode xNode;
					xNode = xDoc.SelectSingleNode("//config");

					// Loop thru all the child nodes
					foreach(XmlNode xNodeLoop in xNode.ChildNodes)
					{
						if (xNodeLoop.NodeType != XmlNodeType.Element)
							continue;
						switch (xNodeLoop.Name.ToLower())
						{
							case "serverroot":
								dc.Directory = xNodeLoop.InnerText;
								break;
							case "port":
								dc.Port = xNodeLoop.InnerText;
								break;
						}	
					}
					return dc;
				}
				catch (Exception ex)
				{	
					throw new Exception(string.Format("Unable to obtain Desktop config information.\n{0}", ex.Message));
				}
			}
		}

        private void cbTabInterface_CheckedChanged(object sender, EventArgs e)
        {
            this.bToolbar = true;   // tabbed interface is part of the toolbar
        }
	}

	internal class DesktopConfig
	{
		internal string Directory;
		internal string Port;
	}
}
