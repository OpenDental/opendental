using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormReqNeededs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReqNeededs));
			this.label1 = new System.Windows.Forms.Label();
			this.comboClassFrom = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboCourseFrom = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butCopy = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.comboClassTo = new System.Windows.Forms.ComboBox();
			this.comboCourseTo = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.butAdd = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOk = new OpenDental.UI.Button();
			this.butDeleteReq = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(61, 18);
			this.label1.TabIndex = 16;
			this.label1.Text = "Class";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClassFrom
			// 
			this.comboClassFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClassFrom.FormattingEnabled = true;
			this.comboClassFrom.Location = new System.Drawing.Point(73, 28);
			this.comboClassFrom.Name = "comboClassFrom";
			this.comboClassFrom.Size = new System.Drawing.Size(234, 21);
			this.comboClassFrom.TabIndex = 0;
			this.comboClassFrom.SelectionChangeCommitted += new System.EventHandler(this.comboClass_SelectionChangeCommitted);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 18);
			this.label2.TabIndex = 18;
			this.label2.Text = "Course";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCourseFrom
			// 
			this.comboCourseFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCourseFrom.FormattingEnabled = true;
			this.comboCourseFrom.Location = new System.Drawing.Point(73, 55);
			this.comboCourseFrom.Name = "comboCourseFrom";
			this.comboCourseFrom.Size = new System.Drawing.Size(234, 21);
			this.comboCourseFrom.TabIndex = 17;
			this.comboCourseFrom.SelectionChangeCommitted += new System.EventHandler(this.comboCourse_SelectionChangeCommitted);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.butCopy);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.comboClassTo);
			this.groupBox1.Controls.Add(this.comboCourseTo);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(465, 133);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(316, 119);
			this.groupBox1.TabIndex = 19;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Copy Requirements To";
			// 
			// butCopy
			// 
			this.butCopy.Location = new System.Drawing.Point(230, 86);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(81, 23);
			this.butCopy.TabIndex = 24;
			this.butCopy.Text = "Copy";
			this.butCopy.UseVisualStyleBackColor = true;
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 59);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(67, 18);
			this.label3.TabIndex = 23;
			this.label3.Text = "Course";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClassTo
			// 
			this.comboClassTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClassTo.FormattingEnabled = true;
			this.comboClassTo.Location = new System.Drawing.Point(77, 32);
			this.comboClassTo.Name = "comboClassTo";
			this.comboClassTo.Size = new System.Drawing.Size(234, 21);
			this.comboClassTo.TabIndex = 20;
			// 
			// comboCourseTo
			// 
			this.comboCourseTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCourseTo.FormattingEnabled = true;
			this.comboCourseTo.Location = new System.Drawing.Point(77, 59);
			this.comboCourseTo.Name = "comboCourseTo";
			this.comboCourseTo.Size = new System.Drawing.Size(234, 21);
			this.comboCourseTo.TabIndex = 22;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(3, 32);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(71, 18);
			this.label4.TabIndex = 21;
			this.label4.Text = "Class";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.comboClassFrom);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.comboCourseFrom);
			this.groupBox3.Controls.Add(this.butAdd);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Location = new System.Drawing.Point(465, 17);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(316, 114);
			this.groupBox3.TabIndex = 21;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Selected Class/Course";
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(230, 82);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(81, 26);
			this.butAdd.TabIndex = 10;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(16, 17);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(433, 556);
			this.gridMain.TabIndex = 13;
			this.gridMain.Title = "Requirements Needed";
			this.gridMain.TranslationName = "TableRequirementsNeeded";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(700, 547);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(82, 26);
			this.butOk.TabIndex = 22;
			this.butOk.Text = "&OK";
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// butDeleteReq
			// 
			this.butDeleteReq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDeleteReq.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteReq.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteReq.Location = new System.Drawing.Point(16, 579);
			this.butDeleteReq.Name = "butDeleteReq";
			this.butDeleteReq.Size = new System.Drawing.Size(104, 26);
			this.butDeleteReq.TabIndex = 20;
			this.butDeleteReq.Text = "Delete All";
			this.butDeleteReq.Click += new System.EventHandler(this.butDeleteReq_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(699, 579);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(82, 26);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Cancel";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormReqNeededs
			// 
			this.ClientSize = new System.Drawing.Size(793, 617);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butDeleteReq);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReqNeededs";
			this.ShowInTaskbar = false;
			this.Text = "Requirements Needed";
			this.Load += new System.EventHandler(this.FormRequirementsNeeded_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.GridOD gridMain;
		private ComboBox comboClassFrom;
		private Label label1;
		private Label label2;
		private ComboBox comboCourseFrom;
		private GroupBox groupBox1;
		private Label label3;
		private ComboBox comboClassTo;
		private ComboBox comboCourseTo;
		private Label label4;
		private UI.Button butDeleteReq;
		private GroupBox groupBox3;
		private UI.Button butOk;
		private UI.Button butCopy;
	}
}
