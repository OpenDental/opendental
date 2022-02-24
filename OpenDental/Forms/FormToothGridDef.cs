using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public class FormToothGridDef:FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.GridOD gridMain;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.Button butDown;
		private OpenDental.UI.Button butUp;
		private OpenDental.UI.ListBoxOD listAvailable;
		private Label labelAvailable;
		private OpenDental.UI.Button butRight;
		private OpenDental.UI.Button butLeft;
		//private bool changed;
		private OpenDental.UI.Button butOK;
		private Label labelCategory;
		private Label labelCustomField;
		private TextBox textCustomField;
		//public DisplayFieldCategory category;
		///<summary>This is the list of all "hardcoded" display fields. Users may add additional fields that will be used to populate ListShowing, but will not be added to AvailList.</summary>
		private List<string> AvailList;
		

		///<summary></summary>
		public FormToothGridDef()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormToothGridDef));
			this.listAvailable = new OpenDental.UI.ListBoxOD();
			this.labelAvailable = new System.Windows.Forms.Label();
			this.labelCategory = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.labelCustomField = new System.Windows.Forms.Label();
			this.textCustomField = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butRight = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listAvailable
			// 
			this.listAvailable.IntegralHeight = false;
			this.listAvailable.Location = new System.Drawing.Point(373, 89);
			this.listAvailable.Name = "listAvailable";
			this.listAvailable.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listAvailable.Size = new System.Drawing.Size(158, 187);
			this.listAvailable.TabIndex = 15;
			this.listAvailable.Click += new System.EventHandler(this.listAvailable_Click);
			// 
			// labelAvailable
			// 
			this.labelAvailable.Location = new System.Drawing.Point(370, 69);
			this.labelAvailable.Name = "labelAvailable";
			this.labelAvailable.Size = new System.Drawing.Size(213, 17);
			this.labelAvailable.TabIndex = 16;
			this.labelAvailable.Text = "Available Fields";
			this.labelAvailable.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelCategory
			// 
			this.labelCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCategory.Location = new System.Drawing.Point(12, 9);
			this.labelCategory.Name = "labelCategory";
			this.labelCategory.Size = new System.Drawing.Size(213, 25);
			this.labelCategory.TabIndex = 57;
			this.labelCategory.Text = "Category Description";
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(12, 76);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(292, 425);
			this.gridMain.TabIndex = 3;
			this.gridMain.Title = "Fields Showing";
			this.gridMain.TranslationName = "FormDisplayFields";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// labelCustomField
			// 
			this.labelCustomField.Location = new System.Drawing.Point(370, 279);
			this.labelCustomField.Name = "labelCustomField";
			this.labelCustomField.Size = new System.Drawing.Size(213, 17);
			this.labelCustomField.TabIndex = 58;
			this.labelCustomField.Text = "New Field";
			this.labelCustomField.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textCustomField
			// 
			this.textCustomField.Location = new System.Drawing.Point(372, 299);
			this.textCustomField.Name = "textCustomField";
			this.textCustomField.Size = new System.Drawing.Size(158, 20);
			this.textCustomField.TabIndex = 59;
			this.textCustomField.Click += new System.EventHandler(this.textCustomField_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(566, 474);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 56;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(320, 292);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(35, 24);
			this.butRight.TabIndex = 55;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(320, 252);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(35, 24);
			this.butLeft.TabIndex = 54;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(109, 507);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(82, 24);
			this.butDown.TabIndex = 14;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(12, 507);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(82, 24);
			this.butUp.TabIndex = 13;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(566, 504);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormToothGridDef
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(664, 556);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textCustomField);
			this.Controls.Add(this.labelCustomField);
			this.Controls.Add(this.labelCategory);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.labelAvailable);
			this.Controls.Add(this.listAvailable);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormToothGridDef";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Setup Toothgrid Def Fields";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDisplayFields_FormClosing);
			this.Load += new System.EventHandler(this.FormDisplayFields_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormDisplayFields_Load(object sender,EventArgs e) {
			FillAvailList();//fills local variable
			FillListAvailable();//fills list in UI
			//category = DisplayFieldCategory.ToothGrid;
			FillGrids();
			//ListShowing = ToothGridDefs
			//labelCategory.Text=category.ToString();
			//textCustomField.Visible=false;
			//labelCustomField.Visible=false;
			//listAvailable.Height=412;
			//DisplayFields.RefreshCache();
			//ListShowing=DisplayFields.GetForCategory(category);
			//if(category==DisplayFieldCategory.OrthoChart) {
			//  textCustomField.Visible=true;
			//  labelCustomField.Visible=true;
			//  listAvailable.Height=227;//227px for short, 412px for tall
			//  labelAvailable.Text=Lan.g(this,"Previously Used Fields");
			//}
			//FillGrids();
		}

		private void FillAvailList() {
			AvailList = new List<string>();
			AvailList.Add("Tooth");
			AvailList.Add("IsPrimary");
			AvailList.Add("IsMissing");
		}

		private void FillListAvailable() {
			listAvailable.Items.AddList(AvailList,x => x.ToString());
		}

		private void FillGrids(){
			//AvailList=DisplayFields.GetAllAvailableList(category);//This one needs to be called repeatedly.
			//gridMain.BeginUpdate();
			//gridMain.Columns.Clear();
			//ODGridColumn col=new ODGridColumn(Lan.g("FormDisplayFields","FieldName"),110);
			//gridMain.Columns.Add(col);
			//col=new ODGridColumn(Lan.g("FormDisplayFields","New Descript"),110);
			//gridMain.Columns.Add(col);
			//col=new ODGridColumn(Lan.g("FormDisplayFields","Width"),60);
			//gridMain.Columns.Add(col);
			//gridMain.Rows.Clear();
			//ODGridRow row;
			//for(int i=0;i<ListShowing.Count;i++){
			//  row=new ODGridRow();
			//  row.Cells.Add(ListShowing[i].InternalName);
			//  row.Cells.Add(ListShowing[i].Description);
			//  row.Cells.Add(ListShowing[i].ColumnWidth.ToString());
			//  gridMain.Rows.Add(row);
			//}
			//gridMain.EndUpdate();
			////Remove things from AvailList that are in the ListShowing.
			//for(int i=0;i<ListShowing.Count;i++){
			//  for(int j=0;j<AvailList.Count;j++) {
			//    //Only removing one item from AvailList per iteration of i, so RemoveAt() is safe without going backwards.
			//    if(category==DisplayFieldCategory.OrthoChart) {
			//      //OrthoChart category does not use InternalNames.
			//      if(ListShowing[i].Description==AvailList[j].Description) {
			//        AvailList.RemoveAt(j);
			//        break;
			//      }
			//    }
			//    else {
			//      if(ListShowing[i].InternalName==AvailList[j].InternalName) {
			//        AvailList.RemoveAt(j);
			//        break;
			//      }
			//    }
			//  }
			//}
			//listAvailable.Items.Clear();
			//if(category==DisplayFieldCategory.OrthoChart) {
			//  listAvailable.Items.AddList(AvailList,x => x.Description);
			//}
			//else {
			//  listAvailable.Items.AddList(AvailList,x => x.ToString());
			//}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//using FormDisplayFieldEdit formD=new FormDisplayFieldEdit();
			//formD.FieldCur=ListShowing[e.Row];
			//DisplayField tempField=ListShowing[e.Row].Copy();
			//formD.ShowDialog();
			//if(formD.DialogResult!=DialogResult.OK) {
			//  ListShowing[e.Row]=tempField.Copy();
			//  return;
			//}
			//if(category==DisplayFieldCategory.OrthoChart) {
			//  if(ListShowing[e.Row].Description=="") {
			//    ListShowing[e.Row]=tempField.Copy();
			//    MsgBox.Show(this,"Description cannot be blank.");
			//    return;
			//  }
			//  for(int i=0;i<ListShowing.Count;i++) {//Check against ListShowing only
			//    if(i==e.Row) {
			//      continue;
			//    }
			//    if(ListShowing[e.Row].Description==ListShowing[i].Description) {
			//      ListShowing[e.Row]=tempField;
			//      MsgBox.Show(this,"That field name already exists.");
			//      return;
			//    }
			//  }
			//  for(int i=0;i<AvailList.Count;i++) {//check against AvailList only
			//    if(ListShowing[e.Row].Description==AvailList[i].Description) {
			//      ListShowing[e.Row]=tempField;
			//      MsgBox.Show(this,"That field name already exists.");
			//      return;
			//    }
			//  }
			//}
			//FillGrids();
			//changed=true;
		}

		private void butDefault_Click(object sender,EventArgs e) {
			//ListShowing=DisplayFields.GetDefaultList(category);//empty for ortho
			//FillGrids();
			//changed=true;
		}

		private void butLeft_Click(object sender,EventArgs e) {
			
				//if(listAvailable.SelectedIndices.Count==0 && textCustomField.Text=="") {
				//  MsgBox.Show(this,"Please select an item in the list on the right or create a new field first.");
				//  return;
				//}
				//if(textCustomField.Text!="") {//Add new ortho chart field
				//  foreach(ToothGridDef defShowing in ListShowing) {
				//    if(textCustomField.Text==defShowing.NameInternal || textCustomField.Text==defShowing.NameShowing) {
				//      MsgBox.Show(this,"That field is already displaying.");
				//      return;
				//    }
				//  }
				//  for(int i=0;i<AvailList.Count;i++) {
				//    if(textCustomField.Text==AvailList[i].Description) {
				//      ListShowing.Add(AvailList[i]);
				//      textCustomField.Text="";
				//      changed=true;
				//      FillGrids();
				//      return;
				//    }
				//  }
				//  DisplayField df=new DisplayField("",100,DisplayFieldCategory.OrthoChart);
				//  df.Description=textCustomField.Text;
				//  ListShowing.Add(df);
				//  textCustomField.Text="";
				//}
				//else {//add existing ortho chart field(s)
				//  DisplayField field;
				//  for(int i=0;i<listAvailable.SelectedIndices.Count;i++) {
				//    field=AvailList[listAvailable.SelectedIndices[i]];
				//    field.ColumnWidth=100;
				//    ListShowing.Add(field);
				//  }
				}


			//if(category==DisplayFieldCategory.OrthoChart) {//Ortho Chart
			//  if(listAvailable.SelectedIndices.Count==0 && textCustomField.Text=="") {
			//    MsgBox.Show(this,"Please select an item in the list on the right or create a new field first.");
			//    return;
			//  }
			//  if(textCustomField.Text!="") {//Add new ortho chart field
			//    for(int i=0;i<ListShowing.Count;i++) {
			//      if(textCustomField.Text==ListShowing[i].Description) {
			//        MsgBox.Show(this,"That field is already displaying.");
			//        return;
			//      }
			//    }
			//    for(int i=0;i<AvailList.Count;i++) {
			//      if(textCustomField.Text==AvailList[i].Description) {
			//        ListShowing.Add(AvailList[i]);
			//        textCustomField.Text="";
			//        changed=true;
			//        FillGrids();
			//        return;
			//      }
			//    }
			//    DisplayField df=new DisplayField("",100,DisplayFieldCategory.OrthoChart);
			//    df.Description=textCustomField.Text;
			//    ListShowing.Add(df);
			//    textCustomField.Text="";
			//  }
			//  else {//add existing ortho chart field(s)
			//    DisplayField field;
			//    for(int i=0;i<listAvailable.SelectedIndices.Count;i++) {
			//      field=AvailList[listAvailable.SelectedIndices[i]];
			//      field.ColumnWidth=100;
			//      ListShowing.Add(field);
			//    }
			//  }
			//}
			//else {//All other display field types
			//  if(listAvailable.SelectedIndices.Count==0) {
			//    MsgBox.Show(this,"Please select an item in the list on the right first.");
			//    return;
			//  }
			//  DisplayField field;
			//  for(int i=0;i<listAvailable.SelectedIndices.Count;i++) {
			//    field=(DisplayField)listAvailable.Items.GetObjectAt(listAvailable.SelectedIndices[i]);
			//    ListShowing.Add(field);
			//  }
			//}
			//changed=true;
			//FillGrids();
		//}

		private void butRight_Click(object sender,EventArgs e) {
			//if(gridMain.SelectedIndices.Length==0) {
			//  MsgBox.Show(this,"Please select an item in the grid on the left first.");
			//  return;
			//}
			//for(int i=gridMain.SelectedIndices.Length-1;i>=0;i--){//go backwards
			//  ListShowing.RemoveAt(gridMain.SelectedIndices[i]);
			//}
			//FillGrids();
			//changed=true;
		}

		private void butUp_Click(object sender,EventArgs e) {
			//if(gridMain.SelectedIndices.Length==0) {
			//  MsgBox.Show(this,"Please select an item in the grid first.");
			//  return;
			//}
			//int[] selected=new int[gridMain.SelectedIndices.Length];
			//for(int i=0;i<gridMain.SelectedIndices.Length;i++){
			//  selected[i]=gridMain.SelectedIndices[i];
			//}
			//if(selected[0]==0){
			//  return;
			//}
			//for(int i=0;i<selected.Length;i++){
			//  ListShowing.Reverse(selected[i]-1,2);
			//}
			//FillGrids();
			//for(int i=0;i<selected.Length;i++){
			//  gridMain.SetSelected(selected[i]-1,true);
			//}
			//changed=true;
		}

		private void butDown_Click(object sender,EventArgs e) {
			//if(gridMain.SelectedIndices.Length==0) {
			//  MsgBox.Show(this,"Please select an item in the grid first.");
			//  return;
			//}
			//int[] selected=new int[gridMain.SelectedIndices.Length];
			//for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
			//  selected[i]=gridMain.SelectedIndices[i];
			//}
			//if(selected[selected.Length-1]==ListShowing.Count-1) {
			//  return;
			//}
			//for(int i=selected.Length-1;i>=0;i--) {//go backwards
			//  ListShowing.Reverse(selected[i],2);
			//}
			//FillGrids();
			//for(int i=0;i<selected.Length;i++) {
			//  gridMain.SetSelected(selected[i]+1,true);
			//}
			//changed=true;
		}

		private void listAvailable_Click(object sender,EventArgs e) {
			//textCustomField.Text="";
		}

		private void textCustomField_Click(object sender,EventArgs e) {
			//listAvailable.SelectedIndex=-1;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//if(!changed) {
			//  DialogResult=DialogResult.OK;
			//  return;
			//}
			//if(category==DisplayFieldCategory.OrthoChart) {
			//  DisplayFields.SaveListForOrthoChart(ListShowing);
			//}
			//else {
			//  DisplayFields.SaveListForCategory(ListShowing,category);
			//}
			//DataValid.SetInvalid(InvalidType.DisplayFields);
			//DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormDisplayFields_FormClosing(object sender,FormClosingEventArgs e) {

		}

		
		

		

		

		

		

		


	}
}





















