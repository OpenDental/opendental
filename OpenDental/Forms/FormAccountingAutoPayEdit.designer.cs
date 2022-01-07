using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAccountingAutoPayEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				components?.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAccountingAutoPayEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.comboPayType = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.listAccounts = new UI.ListBoxOD();
			this.label6 = new System.Windows.Forms.Label();
			this.butRemove = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(515, 273);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(515, 232);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// comboPayType
			// 
			this.comboPayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPayType.FormattingEnabled = true;
			this.comboPayType.Location = new System.Drawing.Point(252, 22);
			this.comboPayType.Name = "comboPayType";
			this.comboPayType.Size = new System.Drawing.Size(230, 21);
			this.comboPayType.TabIndex = 43;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(5, 22);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(245, 19);
			this.label7.TabIndex = 42;
			this.label7.Text = "When this type of payment is entered:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listAccounts
			// 
			this.listAccounts.Location = new System.Drawing.Point(252, 49);
			this.listAccounts.Name = "listAccounts";
			this.listAccounts.Size = new System.Drawing.Size(230, 95);
			this.listAccounts.TabIndex = 41;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(85, 49);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(165, 69);
			this.label6.TabIndex = 40;
			this.label6.Text = "User will get to pick from this list of accounts to deposit into.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butRemove
			// 
			this.butRemove.Location = new System.Drawing.Point(407, 153);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(75, 26);
			this.butRemove.TabIndex = 45;
			this.butRemove.Text = "Remove";
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// butAdd
			// 
			this.butAdd.Location = new System.Drawing.Point(324, 153);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 26);
			this.butAdd.TabIndex = 44;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(28, 273);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 26);
			this.butDelete.TabIndex = 46;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormAccountingAutoPayEdit
			// 
			this.ClientSize = new System.Drawing.Size(642, 324);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.comboPayType);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.listAccounts);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAccountingAutoPayEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Auto Pay Entry";
			this.Load += new System.EventHandler(this.FormAccountingAutoPayEdit_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private ComboBox comboPayType;
		private Label label7;
		private UI.ListBoxOD listAccounts;
		private Label label6;
		private OpenDental.UI.Button butRemove;
		private OpenDental.UI.Button butAdd;
	}
}
