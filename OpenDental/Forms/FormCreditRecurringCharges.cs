using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDental.Bridges;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCreditRecurringCharges:FormODBase {
		private int _pagesPrinted;
		private int _heightPrintHeader;
		private bool _isHeadingPrinted;
		///<summary>List of clinics for which the current user is authorized.  If user is not restricted, list will also contain a dummy clinic with
		///ClinicNum=0 for HQ.  List will be empty if the current user is not allowed to access any clinics or clinics are not enabled.</summary>
		private List<Clinic> _listClinics;
		///<summary>True if we are programmatically selecting indexes in listClinics.  This allows us to use the SelectedIndexChanged event handler and
		///disable the event when we are setting the selected indexes programmatically.</summary>
		private bool _isSelecting;
		///<summary>The object that charges the recurring charges.</summary>
		RecurringChargerator _recurringChargerator;

		///<summary>Only works for XCharge,PayConnect, and PaySimple so far.</summary>
		public FormCreditRecurringCharges() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRecurringCharges_Load(object sender,EventArgs e) {
			if(Programs.HasMultipleCreditCardProgramsEnabled()) {
				gridMain.HScrollVisible=true;
			}
			if(!PrefC.IsODHQ) {
				checkHideBold.Checked=true;
				checkHideBold.Visible=false;
			}
			Program program=null;
			if(Programs.IsEnabled(ProgramName.PaySimple)) {
				program=Programs.GetCur(ProgramName.PaySimple);
				labelUpdated.Visible=false;
				checkForceDuplicates.Checked=false;
				checkForceDuplicates.Visible=false;//PaySimple always rejects identical transactions made within 5 minutes of eachother.
			}
			if(Programs.IsEnabled(ProgramName.PayConnect)) {
				program=Programs.GetCur(ProgramName.PayConnect);
				labelUpdated.Visible=false;
				checkForceDuplicates.Visible=true;
				checkForceDuplicates.Checked=PIn.Bool(ProgramProperties.GetPropValForClinicOrDefault(program.ProgramNum,
					PayConnect.ProgramProperties.PayConnectForceRecurringCharge,Clinics.ClinicNum));
			}
			if(Programs.IsEnabled(ProgramName.EdgeExpress)) {
				program=Programs.GetCur(ProgramName.EdgeExpress);
				labelUpdated.Visible=false;
				checkForceDuplicates.Visible=true;
				checkForceDuplicates.Checked=PIn.Bool(ProgramProperties.GetPropValForClinicOrDefault(program.ProgramNum,
					ProgramProperties.PropertyDescs.EdgeExpress.ForceRecurringCharge,Clinics.ClinicNum));
			}
			if(Programs.IsEnabled(ProgramName.Xcharge)) {
				program=Programs.GetCur(ProgramName.Xcharge);
				labelUpdated.Visible=true;
				checkForceDuplicates.Visible=true;
				string xPath=Programs.GetProgramPath(program);
				checkForceDuplicates.Checked=PIn.Bool(ProgramProperties.GetPropValForClinicOrDefault(program.ProgramNum,
					ProgramProperties.PropertyDescs.XCharge.XChargeForceRecurringCharge,Clinics.ClinicNum));
				if(!File.Exists(xPath)) {//program path is invalid
					//if user has setup permission and they want to edit the program path, show the X-Charge setup window
					if(Security.IsAuthorized(EnumPermType.Setup)
						&& MsgBox.Show(this,MsgBoxButtons.YesNo,"The X-Charge path is not valid.  Would you like to edit the path?"))
					{
						using FormXchargeSetup formXchargeSetup=new FormXchargeSetup();
						formXchargeSetup.ShowDialog();
						if(formXchargeSetup.DialogResult==DialogResult.OK) {
							//The user could have correctly enabled the X-Charge bridge, we need to update our local _programCur and _xPath variable2
							program=Programs.GetCur(ProgramName.Xcharge);
							xPath=Programs.GetProgramPath(program);
						}
					}
					//if the program path still does not exist, whether or not they attempted to edit the program link, tell them to edit and close the form
					if(!File.Exists(xPath)) {
						MsgBox.Show(this,"The X-Charge program path is not valid.  Edit the program link in order to use the CC Recurring Charges feature.");
						Close();
						return;
					}
				}
			}
			if(program==null) {
				MsgBox.Show(this,"The PayConnect, PaySimple, or EdgeExpress program link must be enabled in order to use the CC Recurring Charges feature.");
				Close();
				return;
			}
			_isSelecting=true;
			_listClinics=new List<Clinic>();
			if(PrefC.HasClinicsEnabled) {
				if(!Security.CurUser.ClinicIsRestricted) {
					_listClinics.Add(new Clinic() { Description=Lan.g(this,"Unassigned") });
				}
				List<Clinic> listClinicsForUserod=Clinics.GetForUserod(Security.CurUser);
				for(int i = 0;i<listClinicsForUserod.Count;i++) {
					_listClinics.Add(listClinicsForUserod[i]);
				}
				for(int i=0;i<_listClinics.Count;i++) {
					listClinics.Items.Add(_listClinics[i].Description);
					listClinics.SetSelected(i,true);
				}
				//checkAllClin.Checked=true;//checked true by default in designer so we don't trigger the event to select all and fill grid
			}
			else {
				groupClinics.Visible=false;
			}
			_recurringChargerator=new RecurringChargerator(new ShowErrors(this),true);
			_recurringChargerator.SingleCardFinished=new Action(() => {
				this.Invoke(() => {
					labelCharged.Text=Lans.g(this,"Charged=")+_recurringChargerator.Success;
					labelFailed.Text=Lans.g(this,"Failed=")+_recurringChargerator.Failed;
					labelUpdated.Text=Lans.g(this,"Updated=")+_recurringChargerator.Updated;
				});
			});
			ODEvent.Fired+=StopRecurringCharges;//This is so we'll be alerted in case of a shutdown.
			labelCharged.Text=Lan.g(this,"Charged=")+"0";
			labelFailed.Text=Lan.g(this,"Failed=")+"0";
			checkShowInactive.Checked=PrefC.GetBool(PrefName.RecurringChargesShowInactive);
			FillGrid(true);
			gridMain.SetAll(true);
			labelSelected.Text=Lan.g(this,"Selected=")+gridMain.SelectedIndices.Length.ToString();
		}

		///<summary>The DataTable used to fill the grid will only be refreshed from the db if isFromDb is true.  Otherwise the grid will be refilled using
		///the existing table.  Only get from the db on load or if the Refresh button is pressed, not when the user is selecting the clinic(s).</summary>
		private void FillGrid(bool isFromDb=false) {
			Cursor=Cursors.WaitCursor;
			if(isFromDb) {
				_recurringChargerator.FillCharges(_listClinics);
			}
			List<long> listSelectedClinicNums=listClinics.SelectedIndices.OfType<int>().Select(x => _listClinics[x].ClinicNum).ToList();
			List<RecurringChargeData> listRecurringChargeDatas;
			if(PrefC.HasClinicsEnabled) {
				listRecurringChargeDatas=_recurringChargerator.ListRecurringChargeData.Where(x => listSelectedClinicNums.Contains(x.RecurringCharge.ClinicNum)).ToList();
			}
			else {
				listRecurringChargeDatas=_recurringChargerator.ListRecurringChargeData;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","PatNum"),55));
			if(PrefC.HasClinicsEnabled) {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Name"),190));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Clinic"),75));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Date"),80,HorizontalAlignment.Right));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Family Bal"),70,HorizontalAlignment.Right));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","PayPlan Due"),75,HorizontalAlignment.Right));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Total Due"),65,HorizontalAlignment.Right));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Repeat Amt"),75,HorizontalAlignment.Right));//RptChrgAmt
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Charge Amt"),85,HorizontalAlignment.Right));
				if(Programs.HasMultipleCreditCardProgramsEnabled()) {
					if(Programs.IsEnabled(ProgramName.EdgeExpress)) {
						gridMain.Columns.Add(new GridColumn("EdgeExpress",90,HorizontalAlignment.Center));
					}
					if(Programs.IsEnabled(ProgramName.Xcharge)) {
						gridMain.Columns.Add(new GridColumn("X-Charge",70,HorizontalAlignment.Center));
					}
					if(Programs.IsEnabled(ProgramName.PayConnect)) {
						gridMain.Columns.Add(new GridColumn("PayConnect",85,HorizontalAlignment.Center));
					}
					if(Programs.IsEnabled(ProgramName.PaySimple)) {
						gridMain.Columns.Add(new GridColumn("PaySimple",80,HorizontalAlignment.Center));
					}
				}
			}
			else { //Clinics disabled
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Name"),220));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Date"),80,HorizontalAlignment.Right));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Family Bal"),85,HorizontalAlignment.Right));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","PayPlan Due"),85,HorizontalAlignment.Right));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Total Due"),80,HorizontalAlignment.Right));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Repeat Amt"),90,HorizontalAlignment.Right));//RptChrgAmt
				gridMain.Columns.Add(new GridColumn(Lan.g("TableRecurring","Charge Amt"),100,HorizontalAlignment.Right));
				if(Programs.HasMultipleCreditCardProgramsEnabled()) {
					if(Programs.IsEnabled(ProgramName.EdgeExpress)) {
						gridMain.Columns.Add(new GridColumn("EdgeExpress",80,HorizontalAlignment.Center));
					}
					if(Programs.IsEnabled(ProgramName.Xcharge)) {
						gridMain.Columns.Add(new GridColumn("X-Charge",80,HorizontalAlignment.Center));
					}
					if(Programs.IsEnabled(ProgramName.PayConnect)) {
						gridMain.Columns.Add(new GridColumn("PayConnect",95,HorizontalAlignment.Center));
					}
					if(Programs.IsEnabled(ProgramName.PaySimple)) {
						gridMain.Columns.Add(new GridColumn("PaySimple",90,HorizontalAlignment.Center));
					}
				}
			}
			gridMain.Columns.Add(new GridColumn("IsActive",70,HorizontalAlignment.Center));
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listRecurringChargeDatas.Count;i++){
				if(!checkShowInactive.Checked && !listRecurringChargeDatas[i].CCIsRecurringActive) {
					continue;
				}
				row=new GridRow();
				double famBalTotal=listRecurringChargeDatas[i].RecurringCharge.FamBal;//pat bal+payplan due, but if pat bal<0 and payplan due>0 then just payplan due
				double payPlanDue=listRecurringChargeDatas[i].RecurringCharge.PayPlanDue;
				double chargeAmt=listRecurringChargeDatas[i].RecurringCharge.ChargeAmt;
				double rptChargeAmt=listRecurringChargeDatas[i].RecurringCharge.RepeatAmt;//includes repeat charge (from procs if ODHQ) and attached payplan
				row.Cells.Add(listRecurringChargeDatas[i].RecurringCharge.PatNum.ToString());
				row.Cells.Add(listRecurringChargeDatas[i].PatName);
				if(PrefC.HasClinicsEnabled) {
					Clinic clinic=_listClinics.FirstOrDefault(x => x.ClinicNum==listRecurringChargeDatas[i].RecurringCharge.ClinicNum);
					row.Cells.Add(clinic!=null?clinic.Description:"");//get description from cache if clinics are enabled
				}
				int billingDay=0;
				if(PrefC.GetBool(PrefName.BillingUseBillingCycleDay)) {
					billingDay=listRecurringChargeDatas[i].BillingCycleDay;
				}
				else {
					billingDay=listRecurringChargeDatas[i].RecurringChargeDate.Day;
				}
				DateTime dateStart=DateTimeOD.GetMostRecentValidDate(DateTime.Today.Year,DateTime.Today.Month,billingDay);
				if(dateStart>DateTime.Today) {
					dateStart=dateStart.AddMonths(-1);//Won't give a date with incorrect day.  AddMonths will give the end of the month if needed.
				}
				DateTime dateExcludeBefore=PIn.Date(textDate.Text);//If entry is invalid, all charges will be included because this will be MinDate.
				if(dateStart < dateExcludeBefore) {
					continue;//Don't show row in grid
				}
				row.Cells.Add(dateStart.ToShortDateString());
				row.Cells.Add(famBalTotal.ToString("c"));
				if(!CompareDouble.IsZero(payPlanDue)) {
					row.Cells.Add(payPlanDue.ToString("c"));
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(listRecurringChargeDatas[i].RecurringCharge.TotalDue.ToString("c"));
				row.Cells.Add(rptChargeAmt.ToString("c"));
				row.Cells.Add(chargeAmt.ToString("c"));
				if(!checkHideBold.Checked) {
					double diff=(Math.Max(famBalTotal,0)+Math.Max(payPlanDue,0))-rptChargeAmt;
					if(CompareDouble.IsZero(diff) || (diff<0 && RecurringCharges.CanChargeWhenNoBal(listRecurringChargeDatas[i].CanChargeWhenNoBal))) {
						//don't bold anything
					}
					else if(diff>0) {
						row.Cells[6].Bold=YN.Yes;//"Repeating Amt"
						row.Cells[7].Bold=YN.Yes;//"Charge Amt"
					}
					else if(diff<0) {
						row.Cells[5].Bold=YN.Yes;//"Total Due"
						row.Cells[7].Bold=YN.Yes;//"Charge Amt"
					}
				}
				if(Programs.HasMultipleCreditCardProgramsEnabled()) {
					if(Programs.IsEnabled(ProgramName.EdgeExpress)) {
						if(!string.IsNullOrEmpty(listRecurringChargeDatas[i].XChargeToken) && (listRecurringChargeDatas[i].CCSource==CreditCardSource.EdgeExpressCNP || listRecurringChargeDatas[i].CCSource==CreditCardSource.EdgeExpressRCM)) {
							row.Cells.Add("X");
						}
						else {
							row.Cells.Add("");
						}
					}
					if(Programs.IsEnabled(ProgramName.Xcharge)) {
						if(!string.IsNullOrEmpty(listRecurringChargeDatas[i].XChargeToken) && listRecurringChargeDatas[i].CCSource==CreditCardSource.XServer) {
							row.Cells.Add("X");
						}
						else {
							row.Cells.Add("");
						}
					}
					if(Programs.IsEnabled(ProgramName.PayConnect)) {
						row.Cells.Add(!string.IsNullOrEmpty(listRecurringChargeDatas[i].PayConnectToken) ? "X" : "");
					}
					if(Programs.IsEnabled(ProgramName.PaySimple)) {
						row.Cells.Add(!string.IsNullOrEmpty(listRecurringChargeDatas[i].PaySimpleToken) ? "X" : "");
					}
				}
				row.Cells.Add(listRecurringChargeDatas[i].CCIsRecurringActive?"X":"");
				row.Tag=listRecurringChargeDatas[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			labelTotal.Text=Lan.g(this,"Total=")+gridMain.ListGridRows.Count.ToString();
			labelSelected.Text=Lan.g(this,"Selected=")+gridMain.SelectedIndices.Length.ToString();
			Cursor=Cursors.Default;
		}
				
		private void listClinics_SelectedIndexChanged(object sender,EventArgs e) {
			//if the selected indices have not changed, don't refill the grid
			if(_isSelecting) {
				return;
			}
			RefreshRecurringCharges(isSelectAll:true,isFromDb:false);
			checkAllClin.Checked=(listClinics.SelectedIndices.Count==listClinics.Items.Count);
		}
		
		private void checkAllClin_Click(object sender,EventArgs e) {
			if(!checkAllClin.Checked) {
				return;
			}
			_isSelecting=true;
			for(int i=0;i<listClinics.Items.Count;i++) {
				listClinics.SetSelected(i,true);
			}
			_isSelecting=false;
			RefreshRecurringCharges(isSelectAll:true,isFromDb:false);
		}
		
		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			labelSelected.Text=Lan.g(this,"Selected=")+gridMain.SelectedIndices.Length.ToString();
		}
		
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AccountModule)) {
				return;
			}
			if(e.Row<0) {
				MsgBox.Show(this,"Must select at least one recurring charge.");
				return;
			}
			long patNum=((RecurringChargeData)gridMain.ListGridRows[e.Row].Tag).RecurringCharge.PatNum;
			GlobalFormOpenDental.GotoAccount(patNum);
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshRecurringCharges(isSelectAll:false,isFromDb:true);
		}

		private void RefreshRecurringCharges(bool isSelectAll,bool isFromDb) {
			Cursor=Cursors.WaitCursor;
			List<long> listSelectedCreditCardNums=gridMain.SelectedTags<RecurringChargeData>()
				.Select(x => x.RecurringCharge.CreditCardNum).ToList();
			FillGrid(isFromDb);
			labelCharged.Text=Lan.g(this,"Charged=")+"0";
			labelFailed.Text=Lan.g(this,"Failed=")+"0";
			labelUpdated.Text=Lan.g(this,"Updated=")+"0";
			if(isSelectAll) {
				gridMain.SetAll(true);
			}
			else {
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {
					long creditCardNum=((RecurringChargeData)gridMain.ListGridRows[i].Tag).RecurringCharge.CreditCardNum;
					if(listSelectedCreditCardNums.Contains(creditCardNum)) {
						gridMain.SetSelected(i,true);
					}
				}
			}
			labelSelected.Text=Lan.g(this,"Selected=")+gridMain.SelectedIndices.Length.ToString();
			Cursor=Cursors.Default;
		}

		private void textDate_TextChanged(object sender,EventArgs e) {
			RefreshRecurringCharges(isSelectAll:false,isFromDb:false);
		}

		private void butToday_Click(object sender,EventArgs e) {
			textDate.Text=DateTime.Today.ToShortDateString();
		}

		private void checkHideBold_Click(object sender,EventArgs e) {
			RefreshRecurringCharges(isSelectAll:false,isFromDb:false);
		}

		private void checkShowInactive_Click(object sender,EventArgs e) {
			RefreshRecurringCharges(isSelectAll:false,isFromDb:false);
		}

		private void butPrintList_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"CreditCard recurring charges list printed"),PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangle=e.MarginBounds;
				//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			using Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			using Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangle.Top;
			int center=rectangle.X+rectangle.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Recurring Charges");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				yPos+=20;
				_isHeadingPrinted=true;
				_heightPrintHeader=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,rectangle,_heightPrintHeader);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);
			labelSelected.Text=Lan.g(this,"Selected=")+gridMain.SelectedIndices.Length.ToString();
		}

		private void butNone_Click(object sender,EventArgs e) {
			gridMain.SetAll(false);
			labelSelected.Text=Lan.g(this,"Selected=")+gridMain.SelectedIndices.Length.ToString();
		}

		private void butHistory_Click(object sender,EventArgs e) {
			FormRecurringChargesHistory formRecurringChargesHistory=new FormRecurringChargesHistory();
			formRecurringChargesHistory.Show();
		}

		///<summary>Will process payments for all authorized charges for each CC stored and marked for recurring charges.
		///X-Charge or PayConnect or PaySimple must be enabled.
		///Program validation done on load and if properties are not valid the form will close and exit.</summary>
		private void butSend_Click(object sender,EventArgs e) {
			if(_recurringChargerator.IsCharging) {
				return;
			}
			RefreshRecurringCharges(isSelectAll:false,isFromDb:true);
			int deselectedCount=0;
			for(int i=gridMain.SelectedIndices.Length-1;i>=0;i--) {
				int selectedIndex=gridMain.SelectedIndices[i];
				RecurringChargeData recurringChargeData=(RecurringChargeData)gridMain.ListGridRows[selectedIndex].Tag;
				if(!recurringChargeData.CCIsRecurringActive) {
					gridMain.SetSelected(selectedIndex,false);
					deselectedCount++;
				}
			}
			if(deselectedCount > 0) {
				MessageBox.Show(Lan.g(this,"Number of cards deselected because they are inactive")+": "+deselectedCount);
			}
			if(gridMain.SelectedIndices.Length<1) {
				MsgBox.Show(this,"Must select at least one recurring charge.");
				return;
			}
			//Security.IsAuthorized will default to the minimum DateTime if none is set so we need to specify that the charge is being run today
			if(!Security.IsAuthorized(EnumPermType.PaymentCreate,DateTime.Today)) {
				return;
			}
			_recurringChargerator.IsCharging=true;//Doing this on the main thread in case another click event gets here before the thread can start
			//Not checking DatePay for FutureTransactionDate pref violation because these should always have a date <= today
			Cursor=Cursors.WaitCursor;
			List<RecurringChargeData> listRecurringChargeData=gridMain.SelectedTags<RecurringChargeData>();
			bool doForceDuplicates=checkForceDuplicates.Checked;
			ODThread thread=new ODThread(o => {
				_recurringChargerator.SendCharges(listRecurringChargeData,doForceDuplicates);
				if(IsDisposed) {
					return;
				}
				try {
					this.Invoke(() => {
						FillGrid(true);
						Cursor=Cursors.Default;
						MsgBox.Show(this,"Done charging cards.\r\nIf there are any patients remaining in list, print the list and handle each one manually.");
					});
				}
				catch(ObjectDisposedException) {
					//This likely occurred if the user clicked Close before the charges were done processing. Since we don't need to display the grid, we can
					//do nothing.
				}
			});
			thread.Name="RecurringChargeProcessor";
			thread.Start();
		}

		private void StopRecurringCharges(ODEventArgs args) {
			if(args.EventType==ODEventType.Shutdown) {
				_recurringChargerator.StopCharges(true);
			}
		}

		private void FormCreditRecurringCharges_FormClosing(object sender,FormClosingEventArgs e) {
			_recurringChargerator?.DeleteNotYetCharged();
			_recurringChargerator?.StopCharges(true);//This will still allow the current card to finish.
			ODEvent.Fired-=StopRecurringCharges;
		}

	}
}