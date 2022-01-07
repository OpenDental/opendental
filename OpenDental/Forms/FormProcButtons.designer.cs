using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProcButtons {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcButtons));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listCategories = new OpenDental.UI.ListBoxOD();
			this.listViewButtons = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.imageListProcButtons = new System.Windows.Forms.ImageList(this.components);
			this.labelEdit = new System.Windows.Forms.Label();
			this.panelQuickButtons = new OpenDental.UI.ODButtonPanel();
			this.butEdit = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(324, 33);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(237, 22);
			this.label1.TabIndex = 36;
			this.label1.Text = "Buttons for the selected category";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(36, 33);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(237, 22);
			this.label2.TabIndex = 38;
			this.label2.Text = "Button Categories";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listCategories
			// 
			this.listCategories.Location = new System.Drawing.Point(38, 59);
			this.listCategories.Name = "listCategories";
			this.listCategories.Size = new System.Drawing.Size(234, 316);
			this.listCategories.TabIndex = 37;
			this.listCategories.Click += new System.EventHandler(this.listCategories_Click);
			// 
			// listViewButtons
			// 
			this.listViewButtons.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.listViewButtons.AutoArrange = false;
			this.listViewButtons.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewButtons.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewButtons.HideSelection = false;
			this.listViewButtons.Location = new System.Drawing.Point(326, 59);
			this.listViewButtons.MultiSelect = false;
			this.listViewButtons.Name = "listViewButtons";
			this.listViewButtons.Size = new System.Drawing.Size(234, 316);
			this.listViewButtons.SmallImageList = this.imageListProcButtons;
			this.listViewButtons.TabIndex = 189;
			this.listViewButtons.UseCompatibleStateImageBehavior = false;
			this.listViewButtons.View = System.Windows.Forms.View.Details;
			this.listViewButtons.DoubleClick += new System.EventHandler(this.listViewButtons_DoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Width = 155;
			// 
			// imageListProcButtons
			// 
			this.imageListProcButtons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListProcButtons.ImageStream")));
			this.imageListProcButtons.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListProcButtons.Images.SetKeyName(0, "deposit.gif");
			// 
			// labelEdit
			// 
			this.labelEdit.Location = new System.Drawing.Point(326, 259);
			this.labelEdit.Name = "labelEdit";
			this.labelEdit.Size = new System.Drawing.Size(235, 72);
			this.labelEdit.TabIndex = 204;
			this.labelEdit.Text = "The Quick Buttons category allows custom placement of buttons and labels.  Double" +
    " click anywhere on panel above to add or edit an item.";
			// 
			// panelQuickButtons
			// 
			this.panelQuickButtons.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.panelQuickButtons.Location = new System.Drawing.Point(326, 59);
			this.panelQuickButtons.Name = "panelQuickButtons";
			this.panelQuickButtons.Size = new System.Drawing.Size(199, 192);
			this.panelQuickButtons.TabIndex = 203;
			this.panelQuickButtons.RowDoubleClick += new OpenDental.UI.ODButtonPanelEventHandler(this.panelQuickButtons_RowDoubleClick);
			// 
			// butEdit
			// 
			this.butEdit.Icon = OpenDental.UI.EnumIcons.Add;
			this.butEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEdit.Location = new System.Drawing.Point(38, 395);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(109, 26);
			this.butEdit.TabIndex = 39;
			this.butEdit.Text = "Edit Categories";
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(478, 433);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(82, 26);
			this.butDown.TabIndex = 34;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(478, 395);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(82, 26);
			this.butUp.TabIndex = 35;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(326, 395);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(82, 26);
			this.butAdd.TabIndex = 32;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(326, 433);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(82, 26);
			this.butDelete.TabIndex = 33;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(648, 433);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 8;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormProcButtons
			// 
			this.ClientSize = new System.Drawing.Size(746, 483);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.labelEdit);
			this.Controls.Add(this.panelQuickButtons);
			this.Controls.Add(this.listViewButtons);
			this.Controls.Add(this.butEdit);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listCategories);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butDelete);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProcButtons";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Setup Procedure Buttons";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormProcButtons_Closing);
			this.Load += new System.EventHandler(this.FormChartProcedureEntry_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butDown;
		private OpenDental.UI.Button butUp;
		private Label label1;
		private Label label2;
		private OpenDental.UI.ListBoxOD listCategories;
		private OpenDental.UI.Button butEdit;
		private ListView listViewButtons;
		private ColumnHeader columnHeader1;
		private ImageList imageListProcButtons;
		private UI.ODButtonPanel panelQuickButtons;
		private Label labelEdit;
	}
}
