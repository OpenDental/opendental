using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormFeesForIns : FormODBase {
		private List<FeeSched> FeeSchedsForType;
		private bool _disableBlueBook;
		private bool _enableBlueBook;
		private DataTable table;
		private int pagesPrinted;
		private bool headingPrinted;
		private int headingPrintH;

		///<summary></summary>
		public FormFeesForIns()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormFeesForIns_Load(object sender,EventArgs e) {
			Array arrayValues=Enum.GetValues(typeof(FeeScheduleType));
			for(int i=0;i<arrayValues.Length;i++) {
				FeeScheduleType feeSchedType=((FeeScheduleType)arrayValues.GetValue(i));
				if(feeSchedType==FeeScheduleType.OutNetwork) {
					listType.Items.Add("Out of Network");
				}
				else {
					listType.Items.Add(arrayValues.GetValue(i).ToString());
				}
			}
			listType.SelectedIndex=0;
			ResetSelections();
			FillGrid();
		}

		private void ResetSelections(){
			comboFeeSchedWithout.Items.Clear();
			comboFeeSchedWith.Items.Clear();
			comboFeeSchedNew.Items.Clear();
			comboFeeSchedWithout.Items.Add(Lan.g(this,"none"));
			comboFeeSchedWith.Items.Add(Lan.g(this,"none"));
			comboFeeSchedNew.Items.Add(Lan.g(this,"none"));
			comboFeeSchedWithout.SelectedIndex=0;
			comboFeeSchedWith.SelectedIndex=0;
			comboFeeSchedNew.SelectedIndex=0;
			FeeSchedsForType=FeeScheds.GetListForType((FeeScheduleType)(listType.SelectedIndex),false);
			for(int i=0;i<FeeSchedsForType.Count;i++){
				comboFeeSchedWithout.Items.Add(FeeSchedsForType[i].Description);
				comboFeeSchedWith.Items.Add(FeeSchedsForType[i].Description);
				comboFeeSchedNew.Items.Add(FeeSchedsForType[i].Description);
			}
		}

		private void listType_Click(object sender,EventArgs e) {
			ResetSelections();
			FillGrid();
		}

		private void FillGrid() {
			long feeSchedWithout=0;
			long feeSchedWith=0;
			if(comboFeeSchedWithout.SelectedIndex!=0) {
				feeSchedWithout=FeeSchedsForType[comboFeeSchedWithout.SelectedIndex-1].FeeSchedNum;
			}
			if(comboFeeSchedWith.SelectedIndex!=0) {
				feeSchedWith=FeeSchedsForType[comboFeeSchedWith.SelectedIndex-1].FeeSchedNum;
			}
			table=InsPlans.GetListFeeCheck(textCarrier.Text,textCarrierNot.Text,feeSchedWithout,feeSchedWith,
				(FeeScheduleType)(listType.SelectedIndex));
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Employer",170);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Carrier",170);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Group#",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Group Name",100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Plan Type",65);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Fee Schedule",90);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string planType;
			for(int i = 0;i<table.Rows.Count;i++) {
				row=new GridRow();
				row.Tag=new InsPlanRow(table.Rows[i]);
				row.Cells.Add(table.Rows[i]["EmpName"].ToString());
				row.Cells.Add(table.Rows[i]["CarrierName"].ToString());
				row.Cells.Add(table.Rows[i]["GroupNum"].ToString());
				row.Cells.Add(table.Rows[i]["GroupName"].ToString());
				planType=table.Rows[i]["PlanType"].ToString();
				if(planType=="p") {
					row.Cells.Add("PPO");
				}
				else if(planType=="f") {
					row.Cells.Add("FlatCopay");
				}
				else if(planType=="c") {
					row.Cells.Add("Capitation");
				}
				else {
					row.Cells.Add("Cat%");
				}
				row.Cells.Add(table.Rows[i]["FeeSchedName"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.ScrollValue=0;
		}

		private bool ChangePlansWarningsOk(long newFeeSchedNum,List<InsPlanRow> listInsPlanRowsToChange) {
			if(gridMain.ListGridRows.Count==0) {
				MsgBox.Show(this,"No rows to fix.");
				return false;
			}
			bool isBlueBookOn=PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.BlueBook;
			FeeScheduleType feeScheduleTypeSelected=(FeeScheduleType)listType.SelectedIndex;
			//Prevent Out of Network fee schedules from changing while BlueBook is turned on, and vice versa.
			if(isBlueBookOn	&& feeScheduleTypeSelected==FeeScheduleType.OutNetwork) {
				MessageBox.Show(Lan.g(this,"Cannot change Out of Network fee schedules while the Blue Book feature is turned on."));
				return false;
			}
			else if(!isBlueBookOn	&& feeScheduleTypeSelected==FeeScheduleType.ManualBlueBook) {
				MessageBox.Show(Lan.g(this,"Cannot change Manual Blue Book fee schedules while the Blue Book feature is turned off."));
				return false;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Change the fee schedule for all selected plans to the new fee schedule?")) {
				return false;
			}
			long countPlansToDisable=GetCountPlansToDisableBlueBook(newFeeSchedNum,listInsPlanRowsToChange);
			_disableBlueBook=false;
			if(countPlansToDisable>0) {
				//If Blue Book is off, we don't want to prompt the user, but we still want to disable blue book for the plans.
				if(!isBlueBookOn || MsgBox.Show(MsgBoxButtons.YesNo,countPlansToDisable+" "+
					Lan.g(this,"plan(s) currently have Blue Book enabled. Do you want to disable Blue Book for these plans and delete the associated Blue Book data?")))
				{
					_disableBlueBook=true;
				}
			}
			long countPlansToEnable=GetCountPlansToEnableBlueBook(newFeeSchedNum,listInsPlanRowsToChange);
			_enableBlueBook=false;
			if(countPlansToEnable>0) {
				//If Blue Book is off, we don't want to prompt the user, but we still want to enable blue book for the plans.
				if(!isBlueBookOn || MsgBox.Show(this, MsgBoxButtons.YesNo,countPlansToEnable+" "+
					Lan.g(this,"plan(s) currently do not have Blue Book enabled that are eligible for Blue Book. Do you want to enable Blue Book for these plans?")))
				{
					_enableBlueBook=true;
				}
			}
			//passed all user warnings, ok to proceed with changing
			return true;
		}

		private bool GetPasswordFromUser() {
			using InputBox passBox=new InputBox("To prevent accidental changes, please enter password.  It can be found in our manual.");
			passBox.ShowDialog();
			if(passBox.DialogResult!=DialogResult.OK) {
				return false;
			}
			if(passBox.textResult.Text!="fee") {
				MsgBox.Show(this,"Incorrect password.");
				return false;
			}
			return true;
		}

		private long GetCountPlansToDisableBlueBook(long newFeeSchedNum,List<InsPlanRow> listInsPlansToChange) {
			if(newFeeSchedNum>0) {
				return listInsPlansToChange.FindAll(x => x.PlanType=="" && x.IsBlueBookEnabled && x.FeeSched==0).Count;
			}
			return 0;
		}

		private long GetCountPlansToEnableBlueBook(long newFeeSchedNum,List<InsPlanRow> listInsPlansToChange) {
			if(newFeeSchedNum==0) {
				return listInsPlansToChange.FindAll(plan => plan.PlanType=="" && !plan.IsBlueBookEnabled && plan.FeeSched!=0).Count;
			}
			return 0;
		}

		private void textCarrier_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textCarrierNot_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboFeeSchedWithout_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboFeeSchedWith_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butSelectAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);
		}

		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Check Insurance Plan Fees list printed"),PrintoutOrientation.Portrait);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,"Check Insurance Plan Fees");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				//Add a header for each search term used
				text="Fee Schedule Type: "+listType.SelectedItem.ToString();
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				if(!string.IsNullOrEmpty(textCarrier.Text)) {
					text="Carrier Like: "+textCarrier.Text;
					yPos+=(int)g.MeasureString(text,headingFont).Height;
					g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				}
				if(!string.IsNullOrEmpty(textCarrierNot.Text)) {
					text="Carrier Not Like: "+textCarrier.Text;
					yPos+=(int)g.MeasureString(text,headingFont).Height;
					g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				}
				if(comboFeeSchedWith.SelectedIndex!=0) {
					text="With Fee Schedule: "+comboFeeSchedWith.SelectedItem.ToString();
					yPos+=(int)g.MeasureString(text,headingFont).Height;
					g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				}
				if(comboFeeSchedWithout.SelectedIndex!=0) {
					text="Without Fee Schedule: "+comboFeeSchedWithout.SelectedItem.ToString();
					yPos+=(int)g.MeasureString(text,headingFont).Height;
					g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				}
				yPos+=20;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butChange_Click(object sender,System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				gridMain.SetAll(true);
			}
			long newFeeSchedNum=0;
			if(comboFeeSchedNew.SelectedIndex!=0) {
				newFeeSchedNum=FeeSchedsForType[comboFeeSchedNew.SelectedIndex-1].FeeSchedNum;
			}
			List<InsPlanRow> listInsPlanRowsToChange=gridMain.SelectedTags<InsPlanRow>().FindAll(x => x.FeeSched!=newFeeSchedNum);
			if(!ChangePlansWarningsOk(newFeeSchedNum,listInsPlanRowsToChange)) {
				return;
			}
			if(!GetPasswordFromUser()) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			List<long> listInsPlanNumsToChange=listInsPlanRowsToChange.Select(x => x.PlanNum).ToList();
			long rowsChanged=InsPlans.ChangeFeeScheds(listInsPlanNumsToChange,newFeeSchedNum,(FeeScheduleType)listType.SelectedIndex,_disableBlueBook,_enableBlueBook);
			FillGrid();
			Cursor=Cursors.Default;
			MessageBox.Show(Lan.g(this,"Plans changed: ")+rowsChanged.ToString());
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		///<summary>Holds InsPlan data needed in this form.</summary>
		private class InsPlanRow {
			public long PlanNum;
			public bool IsBlueBookEnabled;
			public string PlanType;
			public long FeeSched;

			///<summary>Creates an InsPlanRow from a DataRow.</summary>
			public InsPlanRow(DataRow dataRow) {
				PlanNum=PIn.Long(dataRow["PlanNum"].ToString());
				IsBlueBookEnabled=PIn.Bool(dataRow["IsBlueBookEnabled"].ToString());
				PlanType=PIn.String(dataRow["PlanType"].ToString());
				FeeSched=PIn.Long(dataRow["FeeSched"].ToString());
			}
		}

		

		

		

		

		

		


	}
}





















