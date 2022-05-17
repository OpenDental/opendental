using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAutoCodeEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoCodeEdit));
			this.checkHidden = new System.Windows.Forms.CheckBox();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.checkLessIntrusive = new System.Windows.Forms.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// checkHidden
			// 
			this.checkHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHidden.Location = new System.Drawing.Point(390, 14);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.Size = new System.Drawing.Size(124, 18);
			this.checkHidden.TabIndex = 1;
			this.checkHidden.Text = "Hidden";
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(682, 528);
			this.butCancel.Name = "butCancel";
			this.butCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 20;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(682, 494);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 19;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(148, 16);
			this.textDescript.Name = "textDescript";
			this.textDescript.Size = new System.Drawing.Size(210, 20);
			this.textDescript.TabIndex = 22;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(20, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(122, 16);
			this.label1.TabIndex = 23;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(402, 496);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(246, 56);
			this.label2.TabIndex = 26;
			this.label2.Text = "Clicking Cancel does not undo changes already made to items.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(36, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(436, 18);
			this.label3.TabIndex = 27;
			this.label3.Text = "You may have duplicate codes  in the following list.";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(158, 478);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(88, 26);
			this.butDelete.TabIndex = 29;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(36, 478);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(84, 26);
			this.butAdd.TabIndex = 28;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// checkLessIntrusive
			// 
			this.checkLessIntrusive.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkLessIntrusive.Location = new System.Drawing.Point(390, 34);
			this.checkLessIntrusive.Name = "checkLessIntrusive";
			this.checkLessIntrusive.Size = new System.Drawing.Size(354, 30);
			this.checkLessIntrusive.TabIndex = 30;
			this.checkLessIntrusive.Text = "Do not check codes in the procedure edit window, but only use this auto code for " +
    "procedure buttons.";
			this.checkLessIntrusive.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(36, 94);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(719, 356);
			this.gridMain.TabIndex = 31;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// FormAutoCodeEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(794, 582);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.checkLessIntrusive);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.checkHidden);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAutoCodeEdit";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "FormAutoCodeEdit";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAutoCodeEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormAutoCodeEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.CheckBox checkHidden;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.TextBox textDescript;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butAdd;
		private System.Windows.Forms.CheckBox checkLessIntrusive;
		private UI.GridOD gridMain;
	}
}
