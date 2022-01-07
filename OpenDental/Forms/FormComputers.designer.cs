using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormComputers {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormComputers));
			this.listComputer = new OpenDental.UI.ListBoxOD();
			this.butClose = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.butSetSimpleGraphics = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textCurComp = new System.Windows.Forms.TextBox();
			this.labelCurComp = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.textServComment = new System.Windows.Forms.TextBox();
			this.textVersion = new System.Windows.Forms.TextBox();
			this.textService = new System.Windows.Forms.TextBox();
			this.textName = new System.Windows.Forms.TextBox();
			this.labelServComment = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.labelService = new System.Windows.Forms.Label();
			this.labelName = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// listComputer
			// 
			this.listComputer.Items.AddRange(new object[] {
            ""});
			this.listComputer.Location = new System.Drawing.Point(17, 265);
			this.listComputer.Name = "listComputer";
			this.listComputer.Size = new System.Drawing.Size(282, 277);
			this.listComputer.TabIndex = 2;
			this.listComputer.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listComputer_MouseDoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(448, 613);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 62);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(409, 44);
			this.label1.TabIndex = 43;
			this.label1.Text = "Computers are added to this list every time you use Open Dental.  You can safely " +
    "delete unused computer names from this list to speed up messaging.";
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(5, 408);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 26);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 100);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(294, 15);
			this.label2.TabIndex = 45;
			this.label2.Text = "ComputerName";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butSetSimpleGraphics
			// 
			this.butSetSimpleGraphics.Location = new System.Drawing.Point(40, 182);
			this.butSetSimpleGraphics.Name = "butSetSimpleGraphics";
			this.butSetSimpleGraphics.Size = new System.Drawing.Size(115, 24);
			this.butSetSimpleGraphics.TabIndex = 3;
			this.butSetSimpleGraphics.Text = "Use Simple Graphics";
			this.butSetSimpleGraphics.Click += new System.EventHandler(this.butSetSimpleGraphics_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(18, 25);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(161, 151);
			this.label3.TabIndex = 82;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butSetSimpleGraphics);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(310, 264);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(195, 219);
			this.groupBox1.TabIndex = 83;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Fix a Workstation";
			// 
			// textCurComp
			// 
			this.textCurComp.Enabled = false;
			this.textCurComp.Location = new System.Drawing.Point(119, 20);
			this.textCurComp.Name = "textCurComp";
			this.textCurComp.ReadOnly = true;
			this.textCurComp.Size = new System.Drawing.Size(282, 20);
			this.textCurComp.TabIndex = 1;
			// 
			// labelCurComp
			// 
			this.labelCurComp.Location = new System.Drawing.Point(6, 22);
			this.labelCurComp.Name = "labelCurComp";
			this.labelCurComp.Size = new System.Drawing.Size(110, 15);
			this.labelCurComp.TabIndex = 86;
			this.labelCurComp.Text = "Current Computer";
			this.labelCurComp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textCurComp);
			this.groupBox2.Controls.Add(this.butDelete);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.labelCurComp);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(12, 145);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(511, 447);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Workstation";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.textServComment);
			this.groupBox3.Controls.Add(this.textVersion);
			this.groupBox3.Controls.Add(this.textService);
			this.groupBox3.Controls.Add(this.textName);
			this.groupBox3.Controls.Add(this.labelServComment);
			this.groupBox3.Controls.Add(this.labelVersion);
			this.groupBox3.Controls.Add(this.labelService);
			this.groupBox3.Controls.Add(this.labelName);
			this.groupBox3.Location = new System.Drawing.Point(12, 12);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(511, 127);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Database Server";
			// 
			// textServComment
			// 
			this.textServComment.Enabled = false;
			this.textServComment.Location = new System.Drawing.Point(119, 92);
			this.textServComment.Name = "textServComment";
			this.textServComment.ReadOnly = true;
			this.textServComment.Size = new System.Drawing.Size(282, 20);
			this.textServComment.TabIndex = 4;
			// 
			// textVersion
			// 
			this.textVersion.Enabled = false;
			this.textVersion.Location = new System.Drawing.Point(119, 68);
			this.textVersion.Name = "textVersion";
			this.textVersion.ReadOnly = true;
			this.textVersion.Size = new System.Drawing.Size(282, 20);
			this.textVersion.TabIndex = 3;
			// 
			// textService
			// 
			this.textService.Enabled = false;
			this.textService.Location = new System.Drawing.Point(119, 44);
			this.textService.Name = "textService";
			this.textService.ReadOnly = true;
			this.textService.Size = new System.Drawing.Size(282, 20);
			this.textService.TabIndex = 2;
			// 
			// textName
			// 
			this.textName.Enabled = false;
			this.textName.Location = new System.Drawing.Point(119, 20);
			this.textName.Name = "textName";
			this.textName.ReadOnly = true;
			this.textName.Size = new System.Drawing.Size(282, 20);
			this.textName.TabIndex = 1;
			// 
			// labelServComment
			// 
			this.labelServComment.Location = new System.Drawing.Point(6, 93);
			this.labelServComment.Name = "labelServComment";
			this.labelServComment.Size = new System.Drawing.Size(110, 17);
			this.labelServComment.TabIndex = 90;
			this.labelServComment.Text = "Service Comment";
			this.labelServComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelVersion
			// 
			this.labelVersion.Location = new System.Drawing.Point(6, 69);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(110, 17);
			this.labelVersion.TabIndex = 89;
			this.labelVersion.Text = "Service Version";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelService
			// 
			this.labelService.Location = new System.Drawing.Point(6, 45);
			this.labelService.Name = "labelService";
			this.labelService.Size = new System.Drawing.Size(110, 17);
			this.labelService.TabIndex = 88;
			this.labelService.Text = "Service Name";
			this.labelService.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(6, 21);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(110, 17);
			this.labelName.TabIndex = 87;
			this.labelName.Text = "Server Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormComputers
			// 
			this.AcceptButton = this.butClose;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(535, 651);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.listComputer);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox3);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormComputers";
			this.ShowInTaskbar = false;
			this.Text = "Computers";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormComputers_Closing);
			this.Load += new System.EventHandler(this.FormComputers_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBoxOD listComputer;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label label2;
		private UI.Button butSetSimpleGraphics;
		private Label label3;
		private GroupBox groupBox1;// Required designer variable.
		private Label labelCurComp;
		private TextBox textCurComp;
		private GroupBox groupBox2;
		private GroupBox groupBox3;
		private TextBox textServComment;
		private TextBox textVersion;
		private TextBox textService;
		private TextBox textName;
		private Label labelServComment;
		private Label labelVersion;
		private Label labelService;
		private Label labelName;
	}
}
