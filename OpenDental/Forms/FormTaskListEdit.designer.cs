using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTaskListEdit {
		private System.ComponentModel.IContainer components = null;

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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskListEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelDescription = new System.Windows.Forms.Label();
			this.labelGlobalFilter = new System.Windows.Forms.Label();
			this.comboGlobalFilter = new OpenDental.UI.ComboBoxOD();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.labelDate = new System.Windows.Forms.Label();
			this.labelDate2 = new System.Windows.Forms.Label();
			this.labelDateType = new System.Windows.Forms.Label();
			this.listDateType = new OpenDental.UI.ListBoxOD();
			this.checkFromNum = new System.Windows.Forms.CheckBox();
			this.textDateTL = new OpenDental.ValidDate();
			this.listObjectType = new OpenDental.UI.ListBoxOD();
			this.labelObjectType = new System.Windows.Forms.Label();
			this.textTaskListNum = new System.Windows.Forms.TextBox();
			this.labelTaskListNum = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(395, 223);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(395, 182);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(8, 18);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(116, 19);
			this.labelDescription.TabIndex = 2;
			this.labelDescription.Text = "Description";
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelGlobalFilter
			// 
			this.labelGlobalFilter.Location = new System.Drawing.Point(11, 105);
			this.labelGlobalFilter.Name = "labelGlobalFilter";
			this.labelGlobalFilter.Size = new System.Drawing.Size(116, 19);
			this.labelGlobalFilter.TabIndex = 138;
			this.labelGlobalFilter.Text = "Global Filter Override";
			this.labelGlobalFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboGlobalFilter
			// 
			this.comboGlobalFilter.Location = new System.Drawing.Point(127, 105);
			this.comboGlobalFilter.Name = "comboGlobalFilter";
			this.comboGlobalFilter.Size = new System.Drawing.Size(120, 21);
			this.comboGlobalFilter.TabIndex = 137;
			this.comboGlobalFilter.SelectionChangeCommitted += new System.EventHandler(this.comboGlobalFilter_SelectionChangeCommitted);
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(127, 18);
			this.textDescript.Name = "textDescript";
			this.textDescript.Size = new System.Drawing.Size(293, 20);
			this.textDescript.TabIndex = 0;
			// 
			// labelDate
			// 
			this.labelDate.Location = new System.Drawing.Point(11, 139);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(116, 19);
			this.labelDate.TabIndex = 4;
			this.labelDate.Text = "Date";
			this.labelDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDate2
			// 
			this.labelDate2.Location = new System.Drawing.Point(218, 135);
			this.labelDate2.Name = "labelDate2";
			this.labelDate2.Size = new System.Drawing.Size(185, 32);
			this.labelDate2.TabIndex = 6;
			this.labelDate2.Text = "Leave blank unless you want this list to show on a dated list";
			// 
			// labelDateType
			// 
			this.labelDateType.Location = new System.Drawing.Point(11, 170);
			this.labelDateType.Name = "labelDateType";
			this.labelDateType.Size = new System.Drawing.Size(116, 19);
			this.labelDateType.TabIndex = 7;
			this.labelDateType.Text = "Date Type";
			this.labelDateType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listDateType
			// 
			this.listDateType.Location = new System.Drawing.Point(127, 170);
			this.listDateType.Name = "listDateType";
			this.listDateType.Size = new System.Drawing.Size(120, 56);
			this.listDateType.TabIndex = 2;
			// 
			// checkFromNum
			// 
			this.checkFromNum.CheckAlign = System.Drawing.ContentAlignment.TopRight;
			this.checkFromNum.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkFromNum.Location = new System.Drawing.Point(7, 232);
			this.checkFromNum.Name = "checkFromNum";
			this.checkFromNum.Size = new System.Drawing.Size(133, 21);
			this.checkFromNum.TabIndex = 3;
			this.checkFromNum.Text = "Is From Repeating";
			this.checkFromNum.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateTL
			// 
			this.textDateTL.Location = new System.Drawing.Point(127, 138);
			this.textDateTL.Name = "textDateTL";
			this.textDateTL.Size = new System.Drawing.Size(87, 20);
			this.textDateTL.TabIndex = 1;
			// 
			// listObjectType
			// 
			this.listObjectType.Location = new System.Drawing.Point(127, 50);
			this.listObjectType.Name = "listObjectType";
			this.listObjectType.Size = new System.Drawing.Size(120, 43);
			this.listObjectType.TabIndex = 15;
			// 
			// labelObjectType
			// 
			this.labelObjectType.Location = new System.Drawing.Point(10, 50);
			this.labelObjectType.Name = "labelObjectType";
			this.labelObjectType.Size = new System.Drawing.Size(116, 19);
			this.labelObjectType.TabIndex = 16;
			this.labelObjectType.Text = "Object Type";
			this.labelObjectType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTaskListNum
			// 
			this.textTaskListNum.Location = new System.Drawing.Point(366, 94);
			this.textTaskListNum.Name = "textTaskListNum";
			this.textTaskListNum.ReadOnly = true;
			this.textTaskListNum.Size = new System.Drawing.Size(54, 20);
			this.textTaskListNum.TabIndex = 136;
			this.textTaskListNum.Visible = false;
			// 
			// labelTaskListNum
			// 
			this.labelTaskListNum.Location = new System.Drawing.Point(276, 95);
			this.labelTaskListNum.Name = "labelTaskListNum";
			this.labelTaskListNum.Size = new System.Drawing.Size(88, 16);
			this.labelTaskListNum.TabIndex = 135;
			this.labelTaskListNum.Text = "TaskListNum";
			this.labelTaskListNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelTaskListNum.Visible = false;
			// 
			// errorProvider1
			// 
			this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.errorProvider1.ContainerControl = this;
			// 
			// FormTaskListEdit
			// 
			this.ClientSize = new System.Drawing.Size(503, 274);
			this.Controls.Add(this.labelGlobalFilter);
			this.Controls.Add(this.comboGlobalFilter);
			this.Controls.Add(this.textTaskListNum);
			this.Controls.Add(this.labelTaskListNum);
			this.Controls.Add(this.listObjectType);
			this.Controls.Add(this.labelObjectType);
			this.Controls.Add(this.textDateTL);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.checkFromNum);
			this.Controls.Add(this.listDateType);
			this.Controls.Add(this.labelDateType);
			this.Controls.Add(this.labelDate2);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.labelDescription);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTaskListEdit";
			this.ShowInTaskbar = false;
			this.Text = "Task List";
			this.Load += new System.EventHandler(this.FormTaskListEdit_Load);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.Label labelDate;
		private System.Windows.Forms.Label labelDate2;
		private System.Windows.Forms.Label labelDateType;
		private OpenDental.UI.ListBoxOD listDateType;
		private System.Windows.Forms.TextBox textDescript;
		private OpenDental.ValidDate textDateTL;
		private System.Windows.Forms.CheckBox checkFromNum;
		private OpenDental.UI.ListBoxOD listObjectType;
		private System.Windows.Forms.Label labelObjectType;
		private TextBox textTaskListNum;
		private Label labelTaskListNum;
		private UI.ComboBoxOD comboGlobalFilter;
		private Label labelGlobalFilter;
		private ErrorProvider errorProvider1;
	}
}
