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

namespace OpenDental {
	public partial class FormEtrans270Edit:FormODBase {
		public Etrans EtransCur;
		private Etrans EtransAck271;
		private string MessageText;
		private string MessageTextAck;
		//public bool IsNew;//this makes no sense.  A 270 will never be new.  Always created, saved, and sent ahead of time.
		///<summary>True if the 270 and response have just been created and are being viewed for the first time.</summary>
		public bool IsInitialResponse;
		///<summary>The list of EB objects parsed from the 270.</summary>
		private List<EB271> listEB;
		///<summary>List of user imported EB segments. Reset each time a user click's 'Import'.</summary>
		public List<EB271> ListImportedEbs;
		private List<DTP271> listDTP;
		private X271 x271;
		///<summary>A list of the current benefits for the plan.
		///This list is designed to be a shallow copy this window will directly manipulate this list on purpose (direct reference).
		///Set this list to a deep copy if the calling method does not want this list to be manipulated by this window.</summary>
		public List<Benefit> benList;
		private long PatPlanNum;
		private long PlanNum;
		private bool headingPrinted;
		private int pagesPrinted;
		private int headingPrintH=0;
		private long SubNum;
		private bool _isDependent;
		private long _subPatNum;
		private bool _isConinsuranceInverted;
		private string _htmlResponse=null;

		public List<DTP271> ListDTP{
			get { return listDTP; }
		}

		public FormEtrans270Edit(long patPlanNum,long planNum,long subNum,bool isDependent,long subPatNum,bool isCoinsuranceInverted) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			PatPlanNum=patPlanNum;
			PlanNum=planNum;
			SubNum=subNum;
			_isDependent=isDependent;
			_subPatNum=subPatNum;
			_isConinsuranceInverted=isCoinsuranceInverted;
			textCurrentGroupNum.Text=InsPlans.GetPlan(planNum,new List<InsPlan>()).GroupNum;
		}

		private void FormEtrans270Edit_Load(object sender,EventArgs e) {
			MessageText=EtransMessageTexts.GetMessageText(EtransCur.EtransMessageTextNum);
			MessageTextAck="";
			//textMessageText.Text=MessageText;
			textNote.Text=EtransCur.Note;
			EtransAck271=Etranss.GetEtrans(EtransCur.AckEtransNum);
			x271=null;
			if(EtransAck271!=null) {
				MessageTextAck=EtransMessageTexts.GetMessageText(EtransAck271.EtransMessageTextNum);//.Replace("~","~\r\n");
				if(EtransAck271.Etype==EtransType.BenefitResponse271) {
					x271=new X271(MessageTextAck);
				}
			}
			listDTP=new List<DTP271>();
			if(x271 != null) {
				listDTP=x271.GetListDtpSubscriber();
				textResponseGroupNum.Text=x271.GetGroupNum();
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
			long patNum=(EtransCur.PatNum==0?_subPatNum:EtransCur.PatNum);//Older 270/217s were always for the subscriber and have etrans.PatNum of 0.
			this.Text+=": "+Patients.GetNameLF(patNum);
			if(Clearinghouses.GetFirstOrDefault(x => x.ClearinghouseNum==EtransCur.ClearingHouseNum)?.CommBridge==EclaimsCommBridge.EDS
			 && EtransAck271!=null && EtransAck271.AckEtransNum!=0)
			{
				_htmlResponse=EtransMessageTexts.GetMessageText(Etranss.GetEtrans(EtransAck271.AckEtransNum).EtransMessageTextNum);
				butPrint.Visible=true;
			}
		}

		private void FormEtrans270Edit_Shown(object sender,EventArgs e) {
			//The 997, 999, 277, or 835 would only exist for a failure.  A success would be a 271.
			if(EtransAck271!=null && (EtransAck271.Etype==EtransType.Acknowledge_997 || EtransAck271.Etype==EtransType.Acknowledge_999 || EtransAck271.Etype==EtransType.StatusNotify_277 || EtransAck271.Etype==EtransType.ERA_835)) {
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
			butPrint.Visible=(_htmlResponse!=null);
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
			butPrint.Visible=true;
			LayoutManager.MoveLocation(butPrint,new Point(gridMain.Right-butPrint.Width,butPrint.Location.Y));
			FillGrid();
		}

		private void SelectForImport() {
			for(int i=0;i<listEB.Count;i++) {
				if(listEB[i].Benefitt !=null) {
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void CreateListOfBenefits() {
			listEB=new List<EB271>();
			if(x271 != null) {
				listEB=x271.GetListEB(radioInNetwork.Checked,radioBenefitSendsIns.Checked);
			}
		}

		///<summary>Take a list of existing benefits (from the DB or could have been entered manually) and a list of imported benefits.
		///First we remove duplicates from the imported benefits, and then add them to our existing list of benefits if they do not already exist.
		///This method preserves existing duplicate benefits.</summary>
		private void RemoveDuplicateBenefits(List<EB271> listImportedBenefits) {
			List<EB271> listImportedBensNoDupes=RemoveDuplicateBenefitsFrom271(listImportedBenefits);
			//We can't rely on Contains() with the benefit class because it has had its .Equals() and .HashCode() overridden and does not behave as expected.
			//We can't use a dictionary here either as we need to preserve existing duplicates. Thus we use a list of benefit.ToString() values.
			List<string> listExistingBensToString=benList.Select(x => x.ToString()).ToList();
			foreach(EB271 eb in listImportedBensNoDupes) {
				if(eb.Benefitt!=null&&!listExistingBensToString.Contains(eb.Benefitt.ToString())) {
					benList.Add(eb.Benefitt);
				}
			}
		}

		///<summary>Helper method that compares EB271.ToString() values (ignoring the free form text element) and removes any duplicates found.</summary>
		private List<EB271> RemoveDuplicateBenefitsFrom271(List<EB271> listEb271) {
			Dictionary<string,EB271> dictBenefits=new Dictionary<string,EB271>();
			foreach(EB271 eb in listEb271) {
				if(eb.Benefitt==null) {//use EB271.ToString() as the key
									   //Exclude free-from text from key because we do not import it anywhere,
									   //thus two benefits which are identical except for free-form text will appear to be duplicates to the user.
					dictBenefits[eb.ToString(false)]=eb;
				}
				else {//use Benefit.ToString() as the key
					dictBenefits[eb.Benefitt.ToString()]=eb;
				}
			}
			return dictBenefits.Values.ToList();
		}

		private void FillGridDates() {
			gridDates.BeginUpdate();
			gridDates.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),150);
			gridDates.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Qualifier"),230);
			gridDates.ListGridColumns.Add(col);
			gridDates.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listDTP.Count;i++) {
				row=new GridRow();
				row.Cells.Add(DTP271.GetDateStr(listDTP[i].Segment.Get(2),listDTP[i].Segment.Get(3)));
				row.Cells.Add(DTP271.GetQualifierDescript(listDTP[i].Segment.Get(1)));
				gridDates.ListGridRows.Add(row);
			}
			gridDates.EndUpdate();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Response"),360);
			gridMain.ListGridColumns.Add(col);
			if(radioModeElect.Checked) {
				col=new GridColumn(Lan.g(this,"Note"),212);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Import As Benefit"),360);
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listEB.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listEB[i].GetDescription(radioModeMessage.Checked,radioBenefitSendsPat.Checked));
				if(radioModeElect.Checked) {
					row.Cells.Add(listEB[i].Segment.Get(5));
					if(listEB[i].Benefitt==null) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(listEB[i].Benefitt.ToString(true));
					}
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Col<2) {//raw benefit
				using FormEtrans270EBraw FormE=new FormEtrans270EBraw();
				FormE.EB271val=listEB[e.Row];
				FormE.ShowDialog();
				//user can't make changes, so no need to refresh grid.
			}
			else {//generated benefit
				if(listEB[e.Row].Benefitt==null) {//create new benefit
					listEB[e.Row].Benefitt=new Benefit();
					using FormBenefitEdit FormB=new FormBenefitEdit(0,PlanNum);
					FormB.IsNew=true;
					FormB.BenefitCur=listEB[e.Row].Benefitt;
					FormB.ShowDialog();
					if(FormB.BenefitCur==null) {//user deleted or cancelled
						listEB[e.Row].Benefitt=null;
					}
				}
				else {//edit existing benefit
					using FormBenefitEdit FormB=new FormBenefitEdit(0,PlanNum);
					FormB.BenefitCur=listEB[e.Row].Benefitt;
					FormB.ShowDialog();
					if(FormB.BenefitCur==null) {//user deleted
						listEB[e.Row].Benefitt=null;
					}
				}
				FillGrid();
			}
		}

		private void FillGridBen() {
			gridBen.BeginUpdate();
			gridBen.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",420);
			gridBen.ListGridColumns.Add(col);
			gridBen.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<benList.Count;i++) {
				row=new GridRow();
				row.Cells.Add(benList[i].ToString());
				gridBen.ListGridRows.Add(row);
			}
			gridBen.EndUpdate();
		}

		private void gridBen_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int benefitListI=benList.IndexOf(benList[e.Row]);
			using FormBenefitEdit FormB=new FormBenefitEdit(0,PlanNum);
			FormB.BenefitCur=benList[e.Row];
			FormB.ShowDialog();
			if(FormB.BenefitCur==null) {//user deleted
				benList.RemoveAt(benefitListI);
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
				if(listEB[gridMain.SelectedIndices[i]].Benefitt==null){
					MsgBox.Show(this,"All selected rows must contain benefits to import.");
					return;
				}
			}
			Benefit ben;
			ListImportedEbs=new List<EB271>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				EB271 ebSegment=listEB[gridMain.SelectedIndices[i]];
				ben=ebSegment.Benefitt;
				if(_isDependent && ben.CoverageLevel!=BenefitCoverageLevel.Family) {//Dependent level benefit, set all benefits as patient overrides.
					ben.PlanNum=0;//Must be 0 when setting PatPlanNum.
					ben.PatPlanNum=PatPlanNum;
				}
				else {
					ben.PlanNum=PlanNum;
				}
				ListImportedEbs.Add(ebSegment);
			}
			RemoveDuplicateBenefits(ListImportedEbs);
			FillGridBen();
		}

		private void butShowRequest_Click(object sender,EventArgs e) {
			using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(MessageText);
			msgbox.ShowDialog();
		}

		private void butShowResponse_Click(object sender,EventArgs e) {
			using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(MessageTextAck);
			msgbox.ShowDialog();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(!string.IsNullOrEmpty(_htmlResponse)) {
				FormWebBrowser formWB=new FormWebBrowser(htmlContent:_htmlResponse);
				formWB.Show();
				return;
			}
			//only visible in Message mode.
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,
				Lan.g(this,"Electronic benefit request from")+" "+EtransCur.DateTimeTrans.ToShortDateString()+" "+Lan.g(this,"printed"),
				auditPatNum:EtransCur.PatNum,
				margins:new Margins(25,25,40,80)
			);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",12,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,"Electronic Benefits Response");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				InsSub sub=InsSubs.GetSub(this.SubNum,new List<InsSub>());
				InsPlan plan=InsPlans.GetPlan(this.PlanNum,new List<InsPlan>());
				Patient subsc=Patients.GetPat(sub.Subscriber);
				text=Lan.g(this,"Subscriber: ")+subsc.GetNameFL();
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				Carrier carrier=Carriers.GetCarrier(plan.CarrierNum);
				if(carrier.CarrierNum!=0) {//not corrupted
					text=carrier.CarrierName;
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
			if(EtransAck271!=null) {
				EtransMessageTexts.Delete(EtransAck271.EtransMessageTextNum);
				Etranss.Delete(EtransAck271.EtransNum);
			}
			EtransMessageTexts.Delete(EtransCur.EtransMessageTextNum);
			Etranss.Delete(EtransCur.EtransNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			EtransCur.Note=textNote.Text;
			Etranss.Update(EtransCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			//if(IsNew) {
			//	EtransMessageTexts.Delete(EtransCur.EtransMessageTextNum);
			//	Etranss.Delete(EtransCur.EtransNum);
			//}
			DialogResult=DialogResult.Cancel;
		}

	

		

		

		

		

	

		
	}
}