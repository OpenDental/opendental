using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using OpenDental.UI;
using System.Drawing.Printing;
using System.IO;
using CodeBase;
using OpenDental.Thinfinity;

namespace OpenDental {
	public partial class FormRpUnfinalizedInsPay:FormODBase {
		private bool _headingPrinted;
		private int _pagesPrinted;
		private int _headingPrintH;

		private List<RpUnfinalizedInsPay.UnfinalizedInsPay> _listUnfinalizedInsPay;

		public FormRpUnfinalizedInsPay() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
    }

    private void FormRpInsPayPlansPastDue_Load(object sender,EventArgs e) {
      FillType();
      if(!LoadData()) {
        DialogResult=DialogResult.Cancel;
        return;
			}
			FillGrid();
    }

    private void FillType() {
			comboBoxMultiType.Items.AddEnums<RpUnfinalizedInsPay.UnfinalizedInsPay.UnfinalizedPaymentType>();
			comboBoxMultiType.IsAllSelected=true;
		}

		///<summary>Retrieves data and uses them to create new UnfinalizedInsPay objects.
		///Heavy lifting done here once upon load.  This also gets called if the user clicks "Refresh Data".</summary>
		private bool LoadData() {
			_listUnfinalizedInsPay=RpUnfinalizedInsPay.GetUnfinalizedInsPay(textCarrier.Text);
      return true;
		}

		///<summary>Actually fill the grid with the data. Filtering based on the user-defined criteria gets done here.</summary>
		private void FillGrid() {
			//get the user-entered filter values.
			//carrier filter is already taken care of in LoadData()
			List<long> listClinicNums=comboClinics.ListSelectedClinicNums;//Only clinics this person has access to.
			//fill the grid
			gridMain.BeginUpdate();
			//columns
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableUnfinalizedInsPay","Type"),120,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnfinalizedInsPay","Patient"),200,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnfinalizedInsPay","Carrier"),200,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableUnfinalizedInsPay","Clinic"),160,GridSortingStrategy.StringCompare);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g("TableUnfinalizedInsPay","Date"),90,GridSortingStrategy.DateParse);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnfinalizedInsPay","Date of Service"),100,GridSortingStrategy.DateParse);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnfinalizedInsPay","Amount"),90,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			//rows
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(RpUnfinalizedInsPay.UnfinalizedInsPay unfinalCur in _listUnfinalizedInsPay) {
				if(!comboBoxMultiType.IsAllSelected 
					&& !comboBoxMultiType.GetListSelected<RpUnfinalizedInsPay.UnfinalizedInsPay.UnfinalizedPaymentType>().Contains(unfinalCur.Type)) 
				{
					continue;
				}
				else {
					//no filter.
				}
				if(PrefC.HasClinicsEnabled && (!listClinicNums.Contains(unfinalCur.ClinicCur.ClinicNum))) {
					continue;
				}
				row=new GridRow();
				string patName=unfinalCur.PatientCur.GetNameLFnoPref();
				if(unfinalCur.CountPats > 1) {
					patName+=" "+Lan.g(this,"(multiple)");
				}
				string carrierName=unfinalCur.CarrierCur.CarrierName;
				row.Cells.Add(Lan.g(this,unfinalCur.Type.GetDescription()));
				row.Cells.Add(patName);
				row.Cells.Add(carrierName);
				if(PrefC.HasClinicsEnabled) {
					string clinicAbbr=unfinalCur.ClinicCur.Abbr;
					if(unfinalCur.ClinicCur.ClinicNum==0) {//Unassigned, see RpUnfinalizedInsPay.UnfinalizedInsPay(...)
						clinicAbbr=Lan.g(this,clinicAbbr);
					}
					row.Cells.Add(clinicAbbr);
				}
				row.Cells.Add(unfinalCur.Date.ToShortDateString());
				row.Cells.Add((unfinalCur.DateOfService.Year < 1880)?"": unfinalCur.DateOfService.ToShortDateString());
				row.Cells.Add(unfinalCur.Amount.ToString("f"));
				row.Tag=unfinalCur;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void comboBoxMulti_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void ComboClinics_SelectionChangeCommitted(object sender, EventArgs e){
			FillGrid();
		}

		private void gridMain_MouseUp(object sender,MouseEventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				contextMenuStrip1.Hide();
				return;
			}
			if(e.Button==MouseButtons.Right) {
				RpUnfinalizedInsPay.UnfinalizedInsPay unfinalPay=(RpUnfinalizedInsPay.UnfinalizedInsPay)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag;
				switch(unfinalPay.Type) {
					case RpUnfinalizedInsPay.UnfinalizedInsPay.UnfinalizedPaymentType.PartialPayment:
						goToAccountToolStripMenuItem.Visible=unfinalPay.CountPats!=0;
						openClaimToolStripMenuItem.Visible=false;
						createCheckToolStripMenuItem.Visible=false;
						openEOBToolStripMenuItem.Visible=true;
						deleteEOBToolStripMenuItem.Visible=true;
						break;
					case RpUnfinalizedInsPay.UnfinalizedInsPay.UnfinalizedPaymentType.UnfinalizedPayment:
						openEOBToolStripMenuItem.Visible=false;
						deleteEOBToolStripMenuItem.Visible=false;
						goToAccountToolStripMenuItem.Visible=true;
						openClaimToolStripMenuItem.Visible=true;
						createCheckToolStripMenuItem.Visible=true;
						break;
					default://should never happen.
						goToAccountToolStripMenuItem.Visible=false;
						openClaimToolStripMenuItem.Visible=false;
						createCheckToolStripMenuItem.Visible=false;
						openEOBToolStripMenuItem.Visible=false;
						deleteEOBToolStripMenuItem.Visible=false;
						break;
				}
			}
		}

		/// <summary>Creates a check for the claim selected. Copied logic from FormClaimEdit.cs</summary>
		private void createCheckToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)) {//date not checked here, but it will be checked when saving the check to prevent backdating
				return;
			}
			if(PrefC.GetBool(PrefName.ClaimPaymentBatchOnly)) {
				//Is there a permission in the manage module that would block this behavior? Are we sending the user into a TRAP?!
				MsgBox.Show(this,"Please use Batch Insurance in Manage Module to Finalize Payments.");
				return;
			}
			RpUnfinalizedInsPay.UnfinalizedInsPay unfinalPay=(RpUnfinalizedInsPay.UnfinalizedInsPay)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag;
			if(unfinalPay.ClaimCur==null) {
				MsgBox.Show(this,"Unable to find claim for this partial payment.");
				return;
			}
			List<ClaimProc> listClaimProcForClaim=ClaimProcs.RefreshForClaim(unfinalPay.ClaimCur.ClaimNum);
			if(!listClaimProcForClaim.Any(x => ListTools.In(x.Status,ClaimProcs.GetInsPaidStatuses()))) {
				MessageBox.Show(Lan.g(this,"There are no valid received payments for this claim."));
				return;
			}
			ClaimPayment claimPayment=new ClaimPayment();
			claimPayment.CheckDate=MiscData.GetNowDateTime().Date;//Today's date for easier tracking by the office and to avoid backdating before accounting lock dates.
			claimPayment.IsPartial=true;
			claimPayment.ClinicNum=unfinalPay.ClinicCur.ClinicNum;
			Family famCur=Patients.GetFamily(unfinalPay.PatientCur.PatNum);
			List<InsSub> listInsSub=InsSubs.RefreshForFam(famCur);
			List<InsPlan> listInsPlan=InsPlans.RefreshForSubList(listInsSub);
			claimPayment.CarrierName=Carriers.GetName(InsPlans.GetPlan(unfinalPay.ClaimCur.PlanNum,listInsPlan).CarrierNum);
			ClaimPayments.Insert(claimPayment);
			double amt=ClaimProcs.AttachAllOutstandingToPayment(claimPayment.ClaimPaymentNum,PrefC.DateClaimReceivedAfter);
			claimPayment.CheckAmt=amt;
			try {
				ClaimPayments.Update(claimPayment);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			FormClaimEdit.FormFinalizePaymentHelper(claimPayment,unfinalPay.ClaimCur,unfinalPay.PatientCur,famCur);
			LoadData();
			FillGrid();
		}

		/// <summary>Opens the current selected claim.</summary>
		private void openClaimToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimView)) {
				return;
			}
			RpUnfinalizedInsPay.UnfinalizedInsPay unfinalPay=(RpUnfinalizedInsPay.UnfinalizedInsPay)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag;
			//Refresh claim from database.  Will return null if not in db, or unfinalPay.ClaimCur already null.
			unfinalPay.ClaimCur=Claims.GetClaim(unfinalPay.ClaimCur?.ClaimNum??0);
			if(unfinalPay.ClaimCur==null) {
				MsgBox.Show(this,"Claim has been deleted by another user.");
			}
			else {
				Family famCur=Patients.GetFamily(unfinalPay.PatientCur.PatNum);
				using FormClaimEdit FormCE=new FormClaimEdit(unfinalPay.ClaimCur,unfinalPay.PatientCur,famCur);
				FormCE.ShowDialog();
			}
			LoadData();
			FillGrid();
		}

		/// <summary>Go to the selected patient's account.</summary>
		private void goToAccountToolStripMenuItem_Click(object sender,EventArgs e) {
			RpUnfinalizedInsPay.UnfinalizedInsPay unfinalPay=(RpUnfinalizedInsPay.UnfinalizedInsPay)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag;
			GotoModule.GotoAccount(unfinalPay.PatientCur.PatNum);
		}

		/// <summary>Opens the selected insurance payment.</summary>
		private void openEOBToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)) {
				return;
			}
			RpUnfinalizedInsPay.UnfinalizedInsPay unfinalPay=(RpUnfinalizedInsPay.UnfinalizedInsPay)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag;
			if(unfinalPay.ClaimPaymentCur==null) {
				MsgBox.Show(this,"This claim payment has been deleted.");
				return;
			}
			using FormClaimPayBatch FormCPB=new FormClaimPayBatch(unfinalPay.ClaimPaymentCur);
			FormCPB.ShowDialog();
			LoadData();
			FillGrid();
		}

		/// <summary>Deletes the selected insurance payment selected.</summary>
		private void deleteEOBToolStripMenuItem_Click(object sender,EventArgs e) {
			RpUnfinalizedInsPay.UnfinalizedInsPay unfinalPay=(RpUnfinalizedInsPay.UnfinalizedInsPay)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag;
			if(unfinalPay.ClaimPaymentCur==null) {
				MsgBox.Show(this,"This claim payment has been deleted.");
				return;
			}
			//Most likely this claim payment is marked as partial. Everyone should have permission to delete a partial payment.
			//Added a check to make sure user has permission and claimpayment is not partial.
			if(!Security.IsAuthorized(Permissions.InsPayEdit,unfinalPay.ClaimPaymentCur.CheckDate) && !unfinalPay.ClaimPaymentCur.IsPartial) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this insurance check?")) {
				return;
			}
			try {
				ClaimPayments.Delete(unfinalPay.ClaimPaymentCur);
				SecurityLogs.MakeLogEntry(Permissions.InsPayEdit,0,"Claim Payment Deleted: "+unfinalPay.ClaimPaymentCur.ClaimPaymentNum);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			LoadData();
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			LoadData();
			FillGrid();
		}

		//Copied from FormRpOutstandingIns.cs
		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;	
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Unfinalized Payment report printed"),PrintoutOrientation.Landscape);
		}

		//Copied from FormRpOutstandingIns.cs
		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_headingPrinted) {
				text=Lan.g(this,"Unfinalized Insurance Payment Report");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				if(comboBoxMultiType.IsAllSelected) {
					text="For All Types";
				}
				else {
					text="For Types: ";
					for(int i=0;i<comboBoxMultiType.SelectedIndices.Count;i++) {
						if(i!=0) {
							text+=", ";
						}
						text+=comboBoxMultiType.Items.GetTextShowingAt(i);
					}
				}
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				if(PrefC.HasClinicsEnabled) {
					text="For Clinics: "+comboClinics.GetStringSelectedClinics();
					g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
					yPos+=20;
				}
				_headingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		//Copied from FormRpOutstandingIns.cs
		private void butExport_Click(object sender,System.EventArgs e) {
			string fileName=Lan.g(this,"Unfinalized Insurance Payments");
			string filePath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
			if(ODBuild.IsWeb()) {
				//file download dialog will come up later, after file is created.
				filePath+=".txt";//Provide the filepath an extension so that Thinfinity can offer as a download.
			}
			else {
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.AddExtension=true;
				saveFileDialog.FileName=fileName;
				if(!Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
					try {
						Directory.CreateDirectory(PrefC.GetString(PrefName.ExportPath));
						saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
					}
					catch {
						//initialDirectory will be blank
					}
				}
				else {
					saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				}
				saveFileDialog.Filter="Text files(*.txt)|*.txt|Excel Files(*.xls)|*.xls|All files(*.*)|*.*";
				saveFileDialog.FilterIndex=0;
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				filePath=saveFileDialog.FileName;
			}
			try {
				using(StreamWriter sw=new StreamWriter(filePath,false))
				{
					String line="";
					for(int i=0;i<gridMain.ListGridColumns.Count;i++) {
						line+=gridMain.ListGridColumns[i].Heading+"\t";
					}
					sw.WriteLine(line);
					for(int i=0;i<gridMain.ListGridRows.Count;i++) {
						line="";
						for(int j=0;j<gridMain.ListGridColumns.Count;j++) {
							line+=gridMain.ListGridRows[i].Cells[j].Text;
							if(j<gridMain.ListGridColumns.Count-1) {
								line+="\t";
							}
						}
						sw.WriteLine(line);
					}
				}
			}
			catch {
				MessageBox.Show(Lan.g(this,"File in use by another program.  Close and try again."));
				return;
			}
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(filePath);
			}
			else {
				MessageBox.Show(Lan.g(this,"File created successfully"));
			}
		}
		
		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		
	}


}