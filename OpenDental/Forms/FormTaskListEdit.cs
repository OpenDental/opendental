using System;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormTaskListEdit : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.Label labelDate;
		private System.Windows.Forms.Label labelDate2;
		private System.Windows.Forms.Label labelDateType;
		private OpenDental.UI.ListBoxOD listDateType;
		private System.Windows.Forms.TextBox textDescript;
		private OpenDental.ValidDate textDateTL;
		private TaskList Cur;
		private System.Windows.Forms.CheckBox checkFromNum;
		private OpenDental.UI.ListBoxOD listObjectType;
		private System.Windows.Forms.Label labelObjectType;
		private TextBox textTaskListNum;
		private Label labelTaskListNum;
		private UI.ComboBoxOD comboGlobalFilter;
		private Label labelGlobalFilter;
		private ErrorProvider errorProvider1;

		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FormTaskListEdit(TaskList cur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Cur=cur;
			Lan.F(this);
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

		private void FormTaskListEdit_Load(object sender, System.EventArgs e) {
			if(ODBuild.IsDebug()) {
				labelTaskListNum.Visible=true;
				textTaskListNum.Visible=true;
				textTaskListNum.Text=Cur.TaskListNum.ToString();
			}
			bool isTasksUseRepeating=PrefC.GetBool(PrefName.TasksUseRepeating);
			if(!isTasksUseRepeating){//Repeating Task List (Legacy) disabled.
				labelDate.Visible=false;
				labelDate2.Visible=false;
				labelDateType.Visible=false;
				textDateTL.Visible=false;
				listDateType.Visible=false;
				checkFromNum.Visible=false;
			}
			textDescript.Text=Cur.Descript;
			if(Cur.DateTL.Year>1880){
				textDateTL.Text=Cur.DateTL.ToShortDateString();
			}
			listDateType.Items.AddEnums<TaskDateType>();
			listDateType.SetSelectedEnum(Cur.DateType);
			if(Cur.FromNum==0){
				checkFromNum.Checked=false;
				checkFromNum.Enabled=false;
			}
			else{
				checkFromNum.Checked=true;
			}
			if(Cur.IsRepeating){
				textDateTL.Enabled=false;
				listObjectType.Enabled=false;
				if(Cur.Parent!=0){//not a main parent
					listDateType.Enabled=false;
				}
			}
			listObjectType.Items.AddEnums<TaskObjectType>();
			listObjectType.SetSelectedEnum(Cur.ObjectType);
			FillComboGlobalFilter();
		}

		private void FillComboGlobalFilter() {
			if((GlobalTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)==GlobalTaskFilterType.Disabled) {
				comboGlobalFilter.Visible=false;
				labelGlobalFilter.Visible=false;
				return;
			}
			comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.Default.GetDescription()),GlobalTaskFilterType.Default);
			comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.None.GetDescription()),GlobalTaskFilterType.None);
			if(PrefC.HasClinicsEnabled) {
				comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.Clinic.GetDescription()),GlobalTaskFilterType.Clinic);
				if(Defs.GetDefsForCategory(DefCat.Regions).Count>0) {
					comboGlobalFilter.Items.Add(Lan.g(this,GlobalTaskFilterType.Region.GetDescription()),GlobalTaskFilterType.Region);
				}
			}
			comboGlobalFilter.SetSelectedEnum(Cur.GlobalTaskFilterType);
			if(comboGlobalFilter.SelectedIndex==-1) {
				errorProvider1.SetError(comboGlobalFilter,$"Previous selection \"{Cur.GlobalTaskFilterType.GetDescription()}\" is no longer available.  "
					+"Saving will overwrite previous setting.");
				comboGlobalFilter.SelectedIndex=0;
			}
		}

		private void comboGlobalFilter_SelectionChangeCommitted(object sender,EventArgs e) {
			errorProvider1.SetError(comboGlobalFilter,string.Empty);//Clear the error, if applicable.
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateTL.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			Cur.Descript=textDescript.Text;
			Cur.DateTL=PIn.Date(textDateTL.Text);
			Cur.DateType=listDateType.GetSelected<TaskDateType>();
			if(!checkFromNum.Checked){//user unchecked the box
				Cur.FromNum=0;
			}
			Cur.ObjectType=listObjectType.GetSelected<TaskObjectType>();
			Cur.GlobalTaskFilterType=comboGlobalFilter.GetSelected<GlobalTaskFilterType>();
			try{
				if(IsNew) {
					TaskLists.Insert(Cur);
					SecurityLogs.MakeLogEntry(Permissions.TaskListCreate,0,Cur.Descript+" "+Lan.g(this,"added"));
				}
				else {
					TaskLists.Update(Cur);
				}
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















