using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormInsCatsSetup {
		private System.ComponentModel.IContainer components=null;

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components!=null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsCatsSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butAddSpan = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butAddCat = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.butDefaultsReset = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butOK.Location = new System.Drawing.Point(613, 619);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(85, 24);
			this.butOK.TabIndex = 6;
			this.butOK.Text = "&Close";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butAddSpan
			// 
			this.butAddSpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddSpan.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddSpan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSpan.Location = new System.Drawing.Point(512, 189);
			this.butAddSpan.Name = "butAddSpan";
			this.butAddSpan.Size = new System.Drawing.Size(86, 24);
			this.butAddSpan.TabIndex = 9;
			this.butAddSpan.Text = "Add Span";
			this.butAddSpan.Click += new System.EventHandler(this.butAddSpan_Click);
			// 
			// butUp
			// 
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(8, 19);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(86, 24);
			this.butUp.TabIndex = 12;
			this.butUp.Text = "Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butAddCat
			// 
			this.butAddCat.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddCat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddCat.Location = new System.Drawing.Point(8, 61);
			this.butAddCat.Name = "butAddCat";
			this.butAddCat.Size = new System.Drawing.Size(86, 24);
			this.butAddCat.TabIndex = 11;
			this.butAddCat.Text = "A&dd";
			this.butAddCat.Click += new System.EventHandler(this.butAddCat_Click);
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(100, 19);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(86, 24);
			this.butDown.TabIndex = 13;
			this.butDown.Text = "Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 68);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(476, 578);
			this.gridMain.TabIndex = 14;
			this.gridMain.Title = "Coverage Spans";
			this.gridMain.TranslationName = "TableCovSpans";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.butUp);
			this.groupBox1.Controls.Add(this.butDown);
			this.groupBox1.Controls.Add(this.butAddCat);
			this.groupBox1.Location = new System.Drawing.Point(504, 68);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(194, 94);
			this.groupBox1.TabIndex = 15;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Categories";
			// 
			// textBox1
			// 
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(12, 12);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(543, 53);
			this.textBox1.TabIndex = 16;
			this.textBox1.Text = resources.GetString("textBox1.Text");
			// 
			// butDefaultsReset
			// 
			this.butDefaultsReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDefaultsReset.Location = new System.Drawing.Point(512, 219);
			this.butDefaultsReset.Name = "butDefaultsReset";
			this.butDefaultsReset.Size = new System.Drawing.Size(86, 24);
			this.butDefaultsReset.TabIndex = 18;
			this.butDefaultsReset.Text = "Set to Defaults";
			this.butDefaultsReset.Click += new System.EventHandler(this.butDefaultsReset_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(509, 246);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(148, 51);
			this.label1.TabIndex = 20;
			this.label1.Text = "This safely fixes the orders and spans.";
			// 
			// FormInsCatsSetup
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butOK;
			this.ClientSize = new System.Drawing.Size(713, 660);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDefaultsReset);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.butAddSpan);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormInsCatsSetup";
			this.ShowInTaskbar = false;
			this.Text = "Setup Insurance Categories";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormInsCatsSetup_Closing);
			this.Load += new System.EventHandler(this.FormInsCatsSetup_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butAddSpan;
		private OpenDental.UI.Button butUp;
		private OpenDental.UI.Button butAddCat;
		private OpenDental.UI.Button butDown;
		private OpenDental.UI.GridOD gridMain;
		private GroupBox groupBox1;
		private TextBox textBox1;
		private OpenDental.UI.Button butDefaultsReset;
		private Label label1;
	}
}
