using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProcButtonEdit {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcButtonEdit));
			this.listAutoCodes = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.listADA = new OpenDental.UI.ListBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.comboCategory = new System.Windows.Forms.ComboBox();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.label6 = new System.Windows.Forms.Label();
			this.butImport = new OpenDental.UI.Button();
			this.butClear = new OpenDental.UI.Button();
			this.listView = new System.Windows.Forms.ListView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.label7 = new System.Windows.Forms.Label();
			this.checkMultiVisit = new System.Windows.Forms.CheckBox();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// listAutoCodes
			// 
			this.listAutoCodes.Location = new System.Drawing.Point(258, 128);
			this.listAutoCodes.Name = "listAutoCodes";
			this.listAutoCodes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listAutoCodes.Size = new System.Drawing.Size(158, 355);
			this.listAutoCodes.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(41, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(124, 13);
			this.label1.TabIndex = 25;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(165, 2);
			this.textDescript.Name = "textDescript";
			this.textDescript.Size = new System.Drawing.Size(316, 20);
			this.textDescript.TabIndex = 24;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(704, 494);
			this.butCancel.Name = "butCancel";
			this.butCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 27;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(614, 494);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 26;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(36, 111);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(188, 14);
			this.label2.TabIndex = 29;
			this.label2.Text = "Add Procedure Codes";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(256, 103);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(191, 22);
			this.label3.TabIndex = 31;
			this.label3.Text = "Highlight Auto Codes";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listADA
			// 
			this.listADA.Location = new System.Drawing.Point(36, 128);
			this.listADA.Name = "listADA";
			this.listADA.Size = new System.Drawing.Size(160, 355);
			this.listADA.TabIndex = 32;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(41, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(358, 23);
			this.label4.TabIndex = 35;
			this.label4.Text = "Add any number of procedure codes and Auto Codes";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(35, 493);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 26);
			this.butAdd.TabIndex = 36;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(122, 493);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 26);
			this.butDelete.TabIndex = 37;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(41, 33);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(124, 13);
			this.label5.TabIndex = 38;
			this.label5.Text = "Category";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCategory
			// 
			this.comboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCategory.FormattingEnabled = true;
			this.comboCategory.Location = new System.Drawing.Point(165, 30);
			this.comboCategory.Name = "comboCategory";
			this.comboCategory.Size = new System.Drawing.Size(215, 21);
			this.comboCategory.TabIndex = 39;
			// 
			// pictureBox
			// 
			this.pictureBox.BackColor = System.Drawing.SystemColors.Window;
			this.pictureBox.Location = new System.Drawing.Point(554, 31);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(20, 20);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox.TabIndex = 40;
			this.pictureBox.TabStop = false;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(430, 35);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(124, 13);
			this.label6.TabIndex = 41;
			this.label6.Text = "Image (20x20)";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butImport
			// 
			this.butImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butImport.Location = new System.Drawing.Point(661, 28);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(75, 26);
			this.butImport.TabIndex = 42;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// butClear
			// 
			this.butClear.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClear.Location = new System.Drawing.Point(580, 28);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(75, 26);
			this.butClear.TabIndex = 43;
			this.butClear.Text = "Clear";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// listView
			// 
			this.listView.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.listView.HideSelection = false;
			this.listView.Location = new System.Drawing.Point(483, 76);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(292, 141);
			this.listView.SmallImageList = this.imageList;
			this.listView.TabIndex = 44;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.SmallIcon;
			this.listView.ItemActivate += new System.EventHandler(this.listView_ItemActivate);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "procButtonAmalgam.gif");
			this.imageList.Images.SetKeyName(1, "procButtonComp.gif");
			this.imageList.Images.SetKeyName(2, "procButtonCrown.gif");
			this.imageList.Images.SetKeyName(3, "procButtonExtract.gif");
			this.imageList.Images.SetKeyName(4, "procButtonPA.gif");
			this.imageList.Images.SetKeyName(5, "procButtonRCT.gif");
			this.imageList.Images.SetKeyName(6, "procButtonRCTbuPFM.gif");
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(480, 54);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(180, 19);
			this.label7.TabIndex = 45;
			this.label7.Text = "Or pick an image from this list";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkMultiVisit
			// 
			this.checkMultiVisit.Location = new System.Drawing.Point(12, 59);
			this.checkMultiVisit.Name = "checkMultiVisit";
			this.checkMultiVisit.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkMultiVisit.Size = new System.Drawing.Size(167, 20);
			this.checkMultiVisit.TabIndex = 46;
			this.checkMultiVisit.Text = "Group for multiple visits";
			this.checkMultiVisit.UseVisualStyleBackColor = true;
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.Location = new System.Drawing.Point(202, 305);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(33, 26);
			this.butDown.TabIndex = 49;
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.Location = new System.Drawing.Point(202, 273);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(33, 26);
			this.butUp.TabIndex = 50;
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// FormProcButtonEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(796, 536);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.checkMultiVisit);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.listView);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.butImport);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.comboCategory);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listADA);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.listAutoCodes);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProcButtonEdit";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Edit Procedure Button";
			this.Load += new System.EventHandler(this.FormChartProcedureEntryEdit_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.ListBoxOD listAutoCodes;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescript;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.ListBoxOD listADA;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label label4;
		private Label label5;
		private ComboBox comboCategory;
		private PictureBox pictureBox;
		private Label label6;
		private OpenDental.UI.Button butImport;
		private OpenDental.UI.Button butClear;
		private ListView listView;
		private Label label7;
		private ImageList imageList;
		private CheckBox checkMultiVisit;
		private UI.Button butDown;
		private UI.Button butUp;
	}
}
