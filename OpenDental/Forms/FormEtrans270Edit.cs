using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Linq;
using OpenDentBusiness.Eclaims;

namespace OpenDental {
	public partial class FormEtrans270Edit:FormODBase {
		public Etrans EtransCur;
		private Etrans _etransAck271;
		private string _msgText;
		private string _msgTextAck;
		//public bool IsNew;//this makes no sense.  A 270 will never be new.  Always created, saved, and sent ahead of time.
		///<summary>True if the 270 and response have just been created and are being viewed for the first time.</summary>
		public bool IsInitialResponse;
		///<summary>The list of EB objects parsed from the 270.</summary>
		private List<EB271> _listEB271s;
		///<summary>List of user imported EB segments. Reset each time a user click's 'Import'.</summary>
		public List<EB271> ListEB271sImported;
		public List<DTP271> ListDTP271s;
		private X271 _x271;
		///<summary>A list of the current benefits for the plan.
		///This list is designed to be a shallow copy this window will directly manipulate this list on purpose (direct reference).
		///Set this list to a deep copy if the calling method does not want this list to be manipulated by this window.</summary>
		public List<Benefit> ListBenefits;
		private long _patPlanNum;
		private long _planNum;
		private bool _isHeadingPrinted;
		private int _pagesPrinted;
		private int _heightHeading=0;
		private long _subNum;
		private bool _isDependent;
		private long _patNum;
		private bool _isConinsuranceInverted;
		private string _htmlResponse=null;

		public FormEtrans270Edit(long patPlanNum,long planNum,long subNum,bool isDependent,long subPatNum,bool isCoinsuranceInverted) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patPlanNum=patPlanNum;
			_planNum=planNum;
			_subNum=subNum;
			_isDependent=isDependent;
			_patNum=subPatNum;
			_isConinsuranceInverted=isCoinsuranceInverted;
			textCurrentGroupNum.Text=InsPlans.GetPlan(planNum,new List<InsPlan>()).GroupNum;
		}

		private void FormEtrans270Edit_Load(object sender,EventArgs e) {
			_msgText=EtransMessageTexts.GetMessageText(EtransCur.EtransMessageTextNum);
			_msgTextAck="";
			//textMessageText.Text=MessageText;
			textNote.Text=EtransCur.Note;
			_etransAck271=Etranss.GetEtrans(EtransCur.AckEtransNum);
			_x271=null;
			if(_etransAck271!=null) {
				_msgTextAck=EtransMessageTexts.GetMessageText(_etransAck271.EtransMessageTextNum);//.Replace("~","~\r\n");
				if(_etransAck271.Etype==EtransType.BenefitResponse271) {
					_x271=new X271(_msgTextAck);
				}
			}
			ListDTP271s=new List<DTP271>();
			if(_x271 != null) {
				ListDTP271s=_x271.GetListDtpSubscriber();
				textResponseGroupNum.Text=_x271.GetGroupNum();
			}
			if(textCurrentGroupNum.Text!=textResponseGroupNum.Text) {
				errorProviderGroupNum.SetError(textCurrentGroupNum,Lan.g(this,"Mismatched group number."));
			}
			radioBenefitSendsPat.Checked=(!_isConinsuranceInverted);
			radioBenefitSendsIns.Checked=(_isConinsuranceInverted);
			FillGridDates();
			CreateListOfBenefits();
			FillGrid();
			FillGridBen();
			if(IsInitialResponse) {
				SelectForImport();
			}
			long patNum=(EtransCur.PatNum==0?_patNum:EtransCur.PatNum);//Older 270/217s were always for the subscriber and have etrans.PatNum of 0.
			this.Text+=": "+Patients.GetNameLF(patNum);
			Clearinghouse clearinghouse=Clearinghouses.GetFirstOrDefault(x => x.ClearinghouseNum==EtransCur.ClearingHouseNum);
			bool isClaimConnect=clearinghouse?.CommBridge==EclaimsCommBridge.ClaimConnect;
			bool isEds=clearinghouse?.CommBridge==EclaimsCommBridge.EDS;
			if((isClaimConnect || isEds) && _etransAck271!=null && _etransAck271.AckEtransNum!=0) {
				_htmlResponse=EtransMessageTexts.GetMessageText(Etranss.GetEtrans(_etransAck271.AckEtransNum).EtransMessageTextNum);
			}
			if(!Security.IsAuthorized(EnumPermType.InsPlanEdit,suppressMessage:true)) {
				groupImport.Enabled=false;
				groupImportBenefit.Enabled=false;
				butImport.Enabled=false;
				gridBen.Enabled=false;
				butDelete.Enabled=false;
			}
		}

		private void FormEtrans270Edit_Shown(object sender,EventArgs e) {
			//The 997, 999, 277, or 835 would only exist for a failure.  A success would be a 271.
			if(_etransAck271!=null && (_etransAck271.Etype==EtransType.Acknowledge_997 || _etransAck271.Etype==EtransType.Acknowledge_999 || _etransAck271.Etype==EtransType.StatusNotify_277 || _etransAck271.Etype==EtransType.ERA_835)) {
				if(IsInitialResponse) {
					MessageBox.Show(EtransCur.Note);
				}
			}
		}

		private void radioModeElect_Click(object sender,EventArgs e) {
			groupImport.Visible=true;
			butImport.Visible=true;
			labelImport.Visible=true;
			gridBen.Visible=true;
			groupImportBenefit.Visible=true;
			labelCurrentGroupNum.Visible=true;
			textCurrentGroupNum.Visible=true;
			labelResponseGroupNum.Visible=true;
			textResponseGroupNum.Visible=true;
			LayoutManager.MoveHeight(gridMain,gridBen.Top-gridMain.Top-3);
			LayoutManager.MoveWidth(gridMain,gridBen.Right-gridMain.Left);
			LayoutManager.MoveLocation(butPrint,new Point(gridMain.Right-butPrint.Width,butPrint.Location.Y));
			FillGrid();
		}

		private void radioModeMessage_Click(object sender,EventArgs e) {
			groupImport.Visible=false;
			butImport.Visible=false;
			labelImport.Visible=false;
			gridBen.Visible=false;
			groupImportBenefit.Visible=false;
			labelCurrentGroupNum.Visible=false;
			textCurrentGroupNum.Visible=false;
			labelResponseGroupNum.Visible=false;
			textResponseGroupNum.Visible=false;
			LayoutManager.MoveHeight(gridMain,labelNote.Top-gridMain.Top);
			LayoutManager.MoveWidth(gridMain,760);//to fit on a piece of paper.
			LayoutManager.MoveLocation(butPrint,new Point(gridMain.Right-butPrint.Width,butPrint.Location.Y));
			FillGrid();
		}

		private void SelectForImport() {
			for(int i=0;i<_listEB271s.Count;i++) {
				if(_listEB271s[i].Benefitt !=null) {
					gridMain.SetSelected(i,setValue:true);
				}
			}
		}

		private void CreateListOfBenefits() {
			_listEB271s=new List<EB271>();
			if(_x271 != null) {
				_listEB271s=_x271.GetListEB(radioInNetwork.Checked,radioBenefitSendsIns.Checked);
			}
		}

		///<summary>Take a list of existing benefits (from the DB or could have been entered manually) and a list of imported benefits.
		///First we remove duplicates from the imported benefits, and then add them to our existing list of benefits if they do not already exist.
		///This method preserves existing duplicate benefits.</summary>
		private void RemoveDuplicateBenefits(List<EB271> listEB271sImported) {
			List<EB271> listEB271sImportedDistinct=RemoveDuplicateBenefitsFrom271(listEB271sImported);
			//We can't rely on Contains() with the benefit class because it has had its .Equals() and .HashCode() overridden and does not behave as expected.
			//We can't use a dictionary here either as we need to preserve existing duplicates. Thus we use a list of benefit.ToString() values.
			List<string> listBenefitDescriptions=ListBenefits.Select(x => x.ToString()).ToList();
			for(int i=0;i<listEB271sImportedDistinct.Count;i++) {
				if(listEB271sImportedDistinct[i].Benefitt!=null && !listBenefitDescriptions.Contains(listEB271sImportedDistinct[i].Benefitt.ToString())) {
					ListBenefits.Add(listEB271sImportedDistinct[i].Benefitt);
				}
			}
		}

		///<summary>Helper method that compares EB271.ToString() values (ignoring the free form text element) and removes any duplicates found.</summary>
		private List<EB271> RemoveDuplicateBenefitsFrom271(List<EB271> listEB271s) {
			List<EB271> listEB21sDistinct=new List<EB271>();
			List<string> listEBStrings=new List<string>();
			for(int i=0;i<listEB271s.Count;i++) {
				if(listEB271s[i].Benefitt==null) { //if Benefitt is null, we want to check the EB271's ToString method while ignoring free-form text.
					if(!listEBStrings.Contains(listEB271s[i].ToString(false))) { //If we've seen this string already, skip it. If we haven't, add it and keep note of it.
						listEBStrings.Add(listEB271s[i].ToString(false));
						listEB21sDistinct.Add(listEB271s[i]);
					}
				}
				else { //Benefitt is not null, so we want to check Benefitt's ToString.
					if(!listEBStrings.Contains(listEB271s[i].Benefitt.ToString())) { //If we've seen this string already, skip it. If we haven't, add it and keep note of it.
						listEBStrings.Add(listEB271s[i].Benefitt.ToString());
						listEB21sDistinct.Add(listEB271s[i]);
					}
				}
			}
			return listEB21sDistinct;
		}

		private void FillGridDates() {
			gridDates.BeginUpdate();
			gridDates.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),150);
			gridDates.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Qualifier"),230);
			gridDates.Columns.Add(col);
			gridDates.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListDTP271s.Count;i++) {
				row=new GridRow();
				row.Cells.Add(DTP271.GetDateStr(ListDTP271s[i].Segment.Get(2),ListDTP271s[i].Segment.Get(3)));
				row.Cells.Add(DTP271.GetQualifierDescript(ListDTP271s[i].Segment.Get(1)));
				gridDates.ListGridRows.Add(row);
			}
			gridDates.EndUpdate();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Response"),360);
			gridMain.Columns.Add(col);
			if(radioModeElect.Checked) {
				col=new GridColumn(Lan.g(this,"Note"),212);
				gridMain.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Import As Benefit"),360);
				gridMain.Columns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEB271s.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listEB271s[i].GetDescription(radioModeMessage.Checked,radioBenefitSendsPat.Checked));
				if(radioModeElect.Checked) {
					row.Cells.Add(_listEB271s[i].Segment.Get(5));
					if(_listEB271s[i].Benefitt==null) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(_listEB271s[i].Benefitt.ToString(isLeadingIncluded:true));
					}
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			bool isAuthorized=Security.IsAuthorized(EnumPermType.InsPlanEdit,suppressMessage:true);
			if(e.Col<2) {//raw benefit
				using FormEtrans270EBraw formEtrans270EBraw=new FormEtrans270EBraw();
				formEtrans270EBraw.EB271Cur=_listEB271s[e.Row];
				formEtrans270EBraw.ShowDialog();
				//user can't make changes, so no need to refresh grid.
			}
			else if(isAuthorized){//generated benefit
				if(_listEB271s[e.Row].Benefitt==null) {//create new benefit
					_listEB271s[e.Row].Benefitt=new Benefit();
					_listEB271s[e.Row].Benefitt.IsNew=true;
					using FormBenefitEdit formBenefitEdit=new FormBenefitEdit(0,_planNum);
					formBenefitEdit.BenefitCur=_listEB271s[e.Row].Benefitt;
					formBenefitEdit.ShowDialog();
					if(formBenefitEdit.BenefitCur==null) {//user deleted or cancelled
						_listEB271s[e.Row].Benefitt=null;
					}
				}
				else {//edit existing benefit
					using FormBenefitEdit formBenefitEdit=new FormBenefitEdit(0,_planNum);
					formBenefitEdit.BenefitCur=_listEB271s[e.Row].Benefitt;
					formBenefitEdit.ShowDialog();
					if(formBenefitEdit.BenefitCur==null) {//user deleted
						_listEB271s[e.Row].Benefitt=null;
					}
				}
				FillGrid();
			}
		}

		private void FillGridBen() {
			gridBen.BeginUpdate();
			gridBen.Columns.Clear();
			GridColumn col=new GridColumn("",420);
			gridBen.Columns.Add(col);
			gridBen.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListBenefits.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ListBenefits[i].ToString());
				gridBen.ListGridRows.Add(row);
			}
			gridBen.EndUpdate();
		}

		private void gridBen_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int idxSelectedBenefit=ListBenefits.IndexOf(ListBenefits[e.Row]);
			using FormBenefitEdit formBenefitEdit=new FormBenefitEdit(0,_planNum);
			formBenefitEdit.BenefitCur=ListBenefits[e.Row];
			formBenefitEdit.ShowDialog();
			if(formBenefitEdit.BenefitCur==null) {//user deleted
				ListBenefits.RemoveAt(idxSelectedBenefit);
			}
			FillGridBen();
		}
		
		private void radioBenefitPerct_CheckedChanged(object sender,EventArgs e) {
			_isConinsuranceInverted=radioBenefitSendsIns.Checked;
			CreateListOfBenefits();
			FillGrid();
			SelectForImport();
		}

		private void radioInNetwork_Click(object sender,EventArgs e) {
			CreateListOfBenefits();
			FillGrid();
			SelectForImport();
		}

		private void radioOutNetwork_Click(object sender,EventArgs e) {
			CreateListOfBenefits();
			FillGrid();
			SelectForImport();
		}

		private void butImport_Click(object sender,EventArgs e) {
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				if(_listEB271s[gridMain.SelectedIndices[i]].Benefitt==null){
					MsgBox.Show(this,"All selected rows must contain benefits to import.");
					return;
				}
			}
			Benefit benefit;
			ListEB271sImported=new List<EB271>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				EB271 EB271Selected=_listEB271s[gridMain.SelectedIndices[i]];
				benefit=EB271Selected.Benefitt;
				if(_isDependent && benefit.CoverageLevel!=BenefitCoverageLevel.Family) {//Dependent level benefit, set all benefits as patient overrides.
					benefit.PlanNum=0;//Must be 0 when setting PatPlanNum.
					benefit.PatPlanNum=_patPlanNum;
				}
				else {
					benefit.PlanNum=_planNum;
				}
				ListEB271sImported.Add(EB271Selected);
			}
			RemoveDuplicateBenefits(ListEB271sImported);
			FillGridBen();
		}

		private void butShowRequest_Click(object sender,EventArgs e) {
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(_msgText);
			msgBoxCopyPaste.ShowDialog();
		}

		private void butShowResponse_Click(object sender,EventArgs e) {
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(_msgTextAck);
			msgBoxCopyPaste.ShowDialog();
		}

		///<summary>Default to show HTML in browser if we have a HTML response. Otherwise, it will just print gridMain. ClaimConnect cannot request both EDI and HTML before
		///loading this form, so print button will request HTML Etrans.</summary>
		private void butPrint_Click(object sender,EventArgs e) {
			Clearinghouse clearinghouse=Clearinghouses.GetFirstOrDefault(x => x.ClearinghouseNum==EtransCur.ClearingHouseNum);
			bool isClaimConnect=(clearinghouse?.CommBridge==EclaimsCommBridge.ClaimConnect);
			if(isClaimConnect && string.IsNullOrEmpty(_htmlResponse)) {
				string x12message=EtransMessageTexts.GetMessageText(EtransCur.EtransMessageTextNum);
				Etrans etransHtml=null;
				string x12response="";
				try {//Similar to x270Controller.RequestBenefits().
					x12response=ClaimConnect.Benefits270(clearinghouse,x12message,out etransHtml,OpenDentBusiness.Dentalxchange2016.Format.HTML);
				}
				catch(Exception ex) {
					FrmFriendlyException frmFriendlyException=new FrmFriendlyException(Lans.g("FormInsPlan","Connection Error:")+"\r\n"+ex.GetType().Name,ex,isUnhandledException:false);
					frmFriendlyException.Show();
				}
				if(etransHtml!=null && etransHtml.EtransNum > 0) {
					//Update EDI Etrans AckEtransNum to point to HTML's Etrans.
					_etransAck271.AckEtrans=etransHtml;
					_etransAck271.AckEtransNum=etransHtml.EtransNum;
					Etranss.Update(_etransAck271);
					_htmlResponse=EtransMessageTexts.GetMessageText(etransHtml.EtransMessageTextNum);
				}
			}
			if(!string.IsNullOrEmpty(_htmlResponse)) {
				FormWebBrowser formWebBrowser=new FormWebBrowser(htmlContent:_htmlResponse);
				formWebBrowser.Show();
				return;
			}
			PrintoutOrientation printoutOrientation=PrintoutOrientation.Portrait;
			if(radioModeElect.Checked) {//gridMain is wider for Electronic Import viewing mode, print/view in landscape to fit.
				printoutOrientation=PrintoutOrientation.Landscape;
			}
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,
				Lan.g(this,"Electronic benefit request from")+" "+EtransCur.DateTimeTrans.ToShortDateString()+" "+Lan.g(this,"printed"),
				printoutOrientation:printoutOrientation,
				auditPatNum:EtransCur.PatNum,
				margins:new Margins(25,25,40,80)
			);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangle=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			using Font fontHeading=new Font("Arial",12,FontStyle.Bold);
			using Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangle.Top;
			int center=rectangle.X+rectangle.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Electronic Benefits Response");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				InsSub insSub=InsSubs.GetSub(this._subNum,new List<InsSub>());
				InsPlan insPlan=InsPlans.GetPlan(this._planNum,new List<InsPlan>());
				Patient patient=Patients.GetPat(insSub.Subscriber);
				text=Lan.g(this,"Subscriber: ")+patient.GetNameFL();
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
				Carrier carrier=Carriers.GetCarrier(insPlan.CarrierNum);
				if(carrier.CarrierNum!=0) {//not corrupted
					text=carrier.CarrierName;
					g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				}
				yPos+=20;
				_isHeadingPrinted=true;
				_heightHeading=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,rectangle,_heightHeading);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		/*
		private void butShowResponseDeciph_Click(object sender,EventArgs e) {
			if(!X12object.IsX12(MessageTextAck)) {
				MessageBox.Show("Only works with 997's");
				return;
			}
			X12object x12obj=new X12object(MessageTextAck);
			if(!x12obj.Is997()) {
				MessageBox.Show("Only works with 997's");
				return;
			}
			X997 x997=new X997(MessageTextAck);
			string display=x997.GetHumanReadable();
			MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(display);
			msgbox.ShowDialog();
		}*/

		private void butDelete_Click(object sender,EventArgs e) {
			//This button is not visible if IsNew
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete entire request and response?")) {
				return;
			}
			if(_etransAck271!=null) {
				EtransMessageTexts.Delete(_etransAck271.EtransMessageTextNum);
				Etranss.Delete(_etransAck271.EtransNum);
			}
			EtransMessageTexts.Delete(EtransCur.EtransMessageTextNum);
			Etranss.Delete(EtransCur.EtransNum);
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender,EventArgs e) {
			EtransCur.Note=textNote.Text;
			Etranss.Update(EtransCur);
			DialogResult=DialogResult.OK;
		}

	}
}