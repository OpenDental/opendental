using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using System.Text;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormMedical : FormODBase {
		///<summary>In-memory-only ODgrid used for printing.</summary>
		private OpenDental.UI.GridOD gridMedsPrint;
		private ToolTip _comboToolTip;
		private Patient PatCur;
		private List<Disease> DiseaseList;
		private PatientNote PatientNoteCur;
		private List<Allergy> allergyList;
		private List<MedicationPat> medList;
		private int pagesPrinted;
		private bool headingPrinted;
		private List<FamilyHealth> ListFamHealth;
		private int headingPrintH;
		private long _EhrMeasureEventNum;
		private long _EhrNotPerfNum;
		private List<Vitalsign> _listVitalSigns;
		///<summary>A copy of the original patient object, as it was when this form was first opened.</summary>
		private Patient _patOld;
		///<summary>List of tobacco use screening type codes.  Currently only 3 allowed SNOMED codes as of 2014.</summary>
		private List<EhrCode> _listAssessmentCodes;
		///<summary>All EhrCodes in the tobacco cessation counseling value set (2.16.840.1.113883.3.526.3.509).
		///When comboInterventionType has selected index 0, load the counseling intervention codes into comboInterventionCode.</summary>
		private List<EhrCode> _listCounselInterventionCodes;
		///<summary>All EhrCodes in the tobacco cessation medication value set (2.16.840.1.113883.3.526.3.1190).
		///When comboInterventionType has selected index 1, load the medication intervention codes into comboInterventionCode.</summary>
		private List<EhrCode> _listMedInterventionCodes;
		private List<EhrCode> _listRecentIntvCodes;
		///<summary>This list contains all of the intervention codes in the comboInterventionCode, counsel, medication, both.</summary>
		private List<EhrCode> _listInterventionCodes;
		///<summary>All EhrCodes in the tobacco user value set (2.16.840.1.113883.3.526.3.1170).
		///When radioAll or radioUser is selected, comboTobaccoStatuses will be filled with this list.</summary>
		private List<EhrCode> _listUserCodes;
		///<summary>All EhrCodes in the tobacco non-user value set (2.16.840.1.113883.3.526.3.1189).
		///When radioAll or radioNonUser is selected, comboTobaccoStatuses will be filled with this list.</summary>
		private List<EhrCode> _listNonUserCodes;
		///<summary>List of tobacco statuses selected from the SNOMED list for this pat that aren't in the list of EHR user and non-user codes</summary>
		private List<EhrCode> _listCustomTobaccoCodes;
		///<summary>List of recently used tobacco statuses, ordered by a date used weighted sum of number of times used.  Codes used the most will be
		///first in the list, with more recent EhrMeasureEvents having a heavier weight.</summary>
		private List<EhrCode> _listRecentTobaccoCodes;
		///<summary>This list contains all of the tobacco statuses in the comboTobaccoStatus, user, non-user, or both.  This list may also contain
		///statuses that the user has selected from the SNOMED list that are not a user or non-user code.</summary>
		private List<EhrCode> _listTobaccoStatuses;
		///<summary>Tab name to pre-select when form loads.
		///i.e. "tabMedical", "tabProblems", "tabMedications", "tabAllergies", "tabFamHealthHist", "tabVitalSigns", or "tabTobaccoUse".</summary>
		private string _selectedTab;

		///<summary></summary>
		public FormMedical(PatientNote patientNoteCur,Patient patCur,string selectedTab=""){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			PatCur=patCur;
			PatientNoteCur=patientNoteCur;
			_selectedTab=selectedTab;
			Lan.F(this);
		}

		private void FormMedical_Load(object sender, System.EventArgs e){
			SecurityLogs.MakeLogEntry(Permissions.MedicalInfoViewed,PatCur.PatNum,"Patient medical information viewed");
			_patOld=PatCur.Copy();
			checkPremed.Checked=PatCur.Premed;
			textMedUrgNote.Text=PatCur.MedUrgNote;
			textMedical.Text=PatientNoteCur.Medical;
			textMedicalComp.Text=PatientNoteCur.MedicalComp;
			textService.Text=PatientNoteCur.Service;
			FillMeds();
			FillProblems();
			FillAllergies();
			FillVitalSigns();
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				FillFamilyHealth();
				TobaccoUseTabLoad();
			}
			else {
				//remove EHR only tabs if ShowFeatureEHR is not enabled.
				tabControlFormMedical.TabPages.RemoveByKey("tabFamHealthHist");
				tabControlFormMedical.TabPages.RemoveByKey("tabTobaccoUse");
			}
			List<EhrMeasureEvent> listDocumentedMedEvents=EhrMeasureEvents.RefreshByType(PatCur.PatNum,EhrMeasureEventType.CurrentMedsDocumented);
			_EhrMeasureEventNum=0;
			for(int i=0;i<listDocumentedMedEvents.Count;i++) {
				if(listDocumentedMedEvents[i].DateTEvent.Date==DateTime.Today) {
					radioMedsDocumentedYes.Checked=true;
					_EhrMeasureEventNum=listDocumentedMedEvents[i].EhrMeasureEventNum;
					break;
				}
			}
			_EhrNotPerfNum=0;
			List<EhrNotPerformed> listNotPerfs=EhrNotPerformeds.Refresh(PatCur.PatNum);
			for(int i=0;i<listNotPerfs.Count;i++) {
				if(listNotPerfs[i].CodeValue!="428191000124101") {//this is the only allowed code for Current Meds Documented procedure
					continue;
				}
				if(listNotPerfs[i].DateEntry.Date==DateTime.Today) {
					radioMedsDocumentedNo.Checked=!radioMedsDocumentedYes.Checked;//only check the No radio button if the Yes radio button is not already set
					_EhrNotPerfNum=listNotPerfs[i].EhrNotPerformedNum;
					break;
				}
			}
			//_selectedTab is set and tab wasn't removed from TabPages, i.e. EHR show feature enabled
			if(_selectedTab!="" && tabControlFormMedical.TabPages.ContainsKey(_selectedTab)) {
				//If tab is disabled, i.e. tabTobaccoUse disabled due to LOINC table missing, tabControlFormMedical_Selecting event handler will cancel
				tabControlFormMedical.SelectTab(_selectedTab);
			}
		}

		#region Medications Tab
		private void FillMeds(bool isForPrinting = false) {
			GridOD gridToFill=isForPrinting?gridMedsPrint:gridMeds;
			Medications.RefreshCache();
			medList=MedicationPats.Refresh(PatCur.PatNum,checkDiscontinued.Checked);
			gridToFill.BeginUpdate();
			gridToFill.ListGridColumns.Clear();
			GridColumn col;
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton && !isForPrinting) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				col=new GridColumn("",18);//infoButton
				col.ImageList=imageListInfoButton;
				gridToFill.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g("TableMedications","Medication"),200);
			gridToFill.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableMedications","Notes"),100){ IsWidthDynamic=true };
			gridToFill.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableMedications","Notes for Patient"),100){ IsWidthDynamic=true };
			gridToFill.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableMedications","Status"),45,HorizontalAlignment.Center);
			gridToFill.ListGridColumns.Add(col);
			if(!isForPrinting) {
				col=new GridColumn(Lan.g("TableMedications","Source"),60);
				gridToFill.ListGridColumns.Add(col);
			}
			gridToFill.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<medList.Count;i++) {
				row=new GridRow();
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton && !isForPrinting) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
					row.Cells.Add("0");//index of infobutton
				}
				if(medList[i].MedicationNum==0) {
					row.Cells.Add(medList[i].MedDescript);
					row.Cells.Add("");//generic notes
				}
				else {
					Medication generic=Medications.GetGeneric(medList[i].MedicationNum);
					string medName=Medications.GetMedication(medList[i].MedicationNum).MedName;
					if(generic.MedicationNum!=medList[i].MedicationNum) {//not generic
						medName+=" ("+generic.MedName+")";
					}
					row.Cells.Add(medName);
					row.Cells.Add(Medications.GetGeneric(medList[i].MedicationNum).Notes);
				}
				row.Cells.Add(medList[i].PatNote);
				if(MedicationPats.IsMedActive(medList[i])) {
					row.Cells.Add("Active");
				}
				else {
					row.Cells.Add("Inactive");
				}
				if(!isForPrinting) {
					if(Erx.IsFromNewCrop(medList[i].ErxGuid)) {
						row.Cells.Add("Legacy");
					}
					else if(Erx.IsFromDoseSpot(medList[i].ErxGuid) || Erx.IsDoseSpotPatReported(medList[i].ErxGuid)) {
						row.Cells.Add("DoseSpot");
					}
					else {
						row.Cells.Add("");
					}
				}
				gridToFill.ListGridRows.Add(row);
			}
			gridToFill.EndUpdate();
		}

		private void gridMeds_CellClick(object sender,ODGridClickEventArgs e) {
			if(!CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				return;
			}
			if(e.Col!=0) {
				return;
			}
			List<KnowledgeRequest> listKnowledgeRequests;
			MedicationPat medPat=medList[e.Row];
			if(medPat.MedicationNum==0) {//Medication orders returned from NewCrop
				listKnowledgeRequests=new List<KnowledgeRequest> {
					new KnowledgeRequest {
						Type="Medication",
						Code=POut.Long(medPat.RxCui),
						CodeSystem=CodeSyst.RxNorm,
						Description=medPat.MedDescript
					}
				};
			}
			else {
				listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(medList[e.Row]);				
			}
			using FormInfobutton FormIB=new FormInfobutton(listKnowledgeRequests);
			FormIB.PatCur=PatCur;
			//FormInfoButton allows MedicationCur to be null, so this will still work for medication orders returned from NewCrop (because MedicationNum will be 0).
			FormIB.ShowDialog();
			//Nothing to do with Dialog Result yet.
		}

		private void gridMeds_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormMedPat FormMP=new FormMedPat();
			FormMP.MedicationPatCur=medList[e.Row];
			FormMP.ShowDialog();
			if(FormMP.DialogResult==DialogResult.OK 
				&& FormMP.MedicationPatCur!=null //Can get be null if the user removed the medication from the patient.
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS 
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).MedicationCDS) 
			{
				using FormCDSIntervention FormCDSI=new FormCDSIntervention();
				if(FormMP.MedicationPatCur.MedicationNum > 0) {//0 indicats the med is from NewCrop.
					Medication medication=Medications.GetMedication(FormMP.MedicationPatCur.MedicationNum);
					if(medication!=null) {
						FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(medication,PatCur);
						FormCDSI.ShowIfRequired(false);
					}
				}
				else if(FormMP.MedicationPatCur.RxCui > 0) {//Meds from NewCrop might have a valid RxNorm.
					RxNorm rxNorm=RxNorms.GetByRxCUI(FormMP.MedicationPatCur.RxCui.ToString());
					if(rxNorm!=null) {
						FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(rxNorm,PatCur);
						FormCDSI.ShowIfRequired(false);
					}
				}
			}
			FillMeds();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			//select medication from list.  Additional meds can be added to the list from within that dlg
			using FormMedications FormM=new FormMedications();
			FormM.IsSelectionMode=true;
			FormM.ShowDialog();
			if(FormM.DialogResult!=DialogResult.OK){
				return;
			} 
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).MedicationCDS) {
				using FormCDSIntervention FormCDSI=new FormCDSIntervention();
				FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(Medications.GetMedication(FormM.SelectedMedicationNum),PatCur);
				FormCDSI.ShowIfRequired();
				if(FormCDSI.DialogResult==DialogResult.Abort) {
					return;//do not add medication
				}
			}
			MedicationPat MedicationPatCur=new MedicationPat();
			MedicationPatCur.PatNum=PatCur.PatNum;
			MedicationPatCur.MedicationNum=FormM.SelectedMedicationNum;
			MedicationPatCur.RxCui=Medications.GetMedication(FormM.SelectedMedicationNum).RxCui;
			//MedicationPatCur.ProvNum=PatCur.PriProv;//Medications are not prescribed by dentists, so this wouldn't make sense.
			using FormMedPat FormMP=new FormMedPat();
			FormMP.MedicationPatCur=MedicationPatCur;
			FormMP.IsNew=true;
			FormMP.ShowDialog();
			if(FormMP.DialogResult!=DialogResult.OK){
				return;
			}
			FillMeds();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			gridMedsPrint=new GridOD() { Width=800,TranslationName="" };
			FillMeds(isForPrinting:true);//not nessecary to explicity name parameter but makes code easier to read.
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Medications printed"),auditPatNum:PatCur.PatNum);
			Cursor=Cursors.Default;
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
				text=Lan.g(this,"Medications List For ")+PatCur.FName+" "+PatCur.LName;
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=Lan.g(this,"Created ")+DateTime.Now.ToString();
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=gridMedsPrint.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void checkShowDiscontinuedMeds_MouseUp(object sender,MouseEventArgs e) {
			FillMeds();
		}

		private void checkDiscontinued_KeyUp(object sender,KeyEventArgs e) {
			FillMeds();
		}

		private void butMedicationReconcile_Click(object sender,EventArgs e) {
			using FormMedicationReconcile FormMR=new FormMedicationReconcile();
			FormMR.PatCur=PatCur;
			FormMR.ShowDialog();
			FillMeds();
		}
		#endregion Medications Tab

		#region Medical Info Tab
		/// <summary>This report is a brute force, one page medical history report. It is not designed to handle more than one page. It does not print service notes or medications.</summary>
		private void butPrintMedical_Click(object sender,EventArgs e) {
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPageMedical,
				Lan.g(this,"Medications information printed"),
				auditPatNum:PatCur.PatNum,
				printoutOrigin:PrintoutOrigin.AtMargin
			);
		}

		private void pd_PrintPageMedical(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			Font bodyFont=new Font(FontFamily.GenericSansSerif,10);
			StringFormat format=new StringFormat();
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			int textHeight;
			RectangleF textRect;
			text=Lan.g(this,"Medical History For ")+PatCur.FName+" "+PatCur.LName;
			g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
			yPos+=(int)g.MeasureString(text,headingFont).Height;
			text=Lan.g(this,"Birthdate: ")+PatCur.Birthdate.ToShortDateString();
			g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
			yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
			text=Lan.g(this,"Created ")+DateTime.Now.ToString();
			g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
			yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
			yPos+=25;
			if(gridDiseases.ListGridRows.Count>0) {
				text=Lan.g(this,"Problems");
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				yPos+=2;
				yPos=gridDiseases.PrintPage(g,0,bounds,yPos);
				yPos+=25;
			}
			if(gridAllergies.ListGridRows.Count>0) {
				text=Lan.g(this,"Allergies");
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				yPos+=2;
				yPos=gridAllergies.PrintPage(g,0,bounds,yPos);
				yPos+=25;
			}
			text=Lan.g(this,"Premedicate (PAC or other): ")+(checkPremed.Checked?"Y":"N");
			textHeight=(int)g.MeasureString(text,bodyFont,bounds.Width).Height;
			textRect=new Rectangle(bounds.Left,yPos,bounds.Width,textHeight);
			g.DrawString(text,subHeadingFont,Brushes.Black,textRect);
			yPos+=textHeight;
			yPos+=10;
			text=Lan.g(this,"Medical Urgent Note");
			g.DrawString(text,subHeadingFont,Brushes.Black,bounds.Left,yPos);
			yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
			text=textMedUrgNote.Text;
			textHeight=(int)g.MeasureString(text,bodyFont,bounds.Width).Height;
			textRect=new Rectangle(bounds.Left,yPos,bounds.Width,textHeight);
			g.DrawString(text,bodyFont,Brushes.Black,textRect);//maybe red?
			yPos+=textHeight;
			yPos+=10;
			text=Lan.g(this,"Medical Summary");
			g.DrawString(text,subHeadingFont,Brushes.Black,bounds.Left,yPos);
			yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
			text=textMedical.Text;
			textHeight=(int)g.MeasureString(text,bodyFont,bounds.Width).Height;
			textRect=new Rectangle(bounds.Left,yPos,bounds.Width,textHeight);
			g.DrawString(text,bodyFont,Brushes.Black,textRect);
			yPos+=textHeight;
			yPos+=10;
			text=Lan.g(this,"Medical History - Complete and Detailed");
			g.DrawString(text,subHeadingFont,Brushes.Black,bounds.Left,yPos);
			yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
			text=textMedicalComp.Text;
			textHeight=(int)g.MeasureString(text,bodyFont,bounds.Width).Height;
			textRect=new Rectangle(bounds.Left,yPos,bounds.Width,textHeight);
			g.DrawString(text,bodyFont,Brushes.Black,textRect);
			yPos+=textHeight;
			g.Dispose();
		}
		#endregion Medical Info Tab

		#region Family Health History Tab
		private void FillFamilyHealth() {
			ListFamHealth=FamilyHealths.GetFamilyHealthForPat(PatCur.PatNum);
			gridFamilyHealth.BeginUpdate();
			gridFamilyHealth.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableFamilyHealth","Relationship"),150,HorizontalAlignment.Center);
			gridFamilyHealth.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableFamilyHealth","Name"),150);
			gridFamilyHealth.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableFamilyHealth","Problem"),180);
			gridFamilyHealth.ListGridColumns.Add(col);
			gridFamilyHealth.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListFamHealth.Count;i++) {
				row=new GridRow();
				row.Cells.Add(Lan.g("enumFamilyRelationship",ListFamHealth[i].Relationship.ToString()));
				row.Cells.Add(ListFamHealth[i].PersonName);
				row.Cells.Add(DiseaseDefs.GetName(ListFamHealth[i].DiseaseDefNum));
				gridFamilyHealth.ListGridRows.Add(row);
			}
			gridFamilyHealth.EndUpdate();
		}

		private void gridFamilyHealth_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormFamilyHealthEdit FormFHE=new FormFamilyHealthEdit();
			FormFHE.FamilyHealthCur=ListFamHealth[e.Row];
			FormFHE.ShowDialog();
			FillFamilyHealth();
		}

		private void butAddFamilyHistory_Click(object sender,EventArgs e) {
			using FormFamilyHealthEdit FormFHE=new FormFamilyHealthEdit();
			FamilyHealth famH=new FamilyHealth();
			famH.PatNum=PatCur.PatNum;
			famH.IsNew=true;
			FormFHE.FamilyHealthCur=famH;
			FormFHE.ShowDialog();
			if(FormFHE.DialogResult!=DialogResult.OK) {
				return;
			}
			FillFamilyHealth();
		}
		#endregion Family Health History Tab

		#region Problems Tab
		private void FillProblems(){
			DiseaseList=Diseases.Refresh(checkShowInactiveProblems.Checked,PatCur.PatNum);
			gridDiseases.BeginUpdate();
			gridDiseases.ListGridColumns.Clear();
			GridColumn col;
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				col=new GridColumn("",18);//infoButton
				col.ImageList=imageListInfoButton;
				gridDiseases.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g("TableDiseases","Name"),200);//total is about 325
			gridDiseases.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDiseases","Patient Note"),450);
			gridDiseases.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDisease","Status"),40,HorizontalAlignment.Center);
			gridDiseases.ListGridColumns.Add(col);
			gridDiseases.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<DiseaseList.Count;i++){
				row=new GridRow();
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
					row.Cells.Add("0");//index of infobutton
				}
				if(DiseaseList[i].DiseaseDefNum!=0) {
					row.Cells.Add(DiseaseDefs.GetName(DiseaseList[i].DiseaseDefNum));
				}
				else {
					row.Cells.Add(DiseaseDefs.GetName(DiseaseList[i].DiseaseDefNum));
				}
				row.Cells.Add(DiseaseList[i].PatNote);
				row.Cells.Add(DiseaseList[i].ProbStatus.ToString());
				gridDiseases.ListGridRows.Add(row);
			}
			gridDiseases.EndUpdate();
		}

		private void butAddProblem_Click(object sender,EventArgs e) {
			//get the list of disease def nums that should be highlighted in FormDiseaseDefs.
			List<long> listDiseaseDefNums=DiseaseList.Where(x => x.ProbStatus == ProblemStatus.Active).ToList().Select(x => x.DiseaseDefNum).ToList();
			using FormDiseaseDefs FormDD=new FormDiseaseDefs(listDiseaseDefNums);
			FormDD.IsSelectionMode=true;
			FormDD.IsMultiSelect=true;
			FormDD.ListDiseaseDefsSelected=new List<DiseaseDef>();
			FormDD.ShowDialog();
			if(FormDD.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<FormDD.ListDiseaseDefsSelected.Count;i++) {
				Disease disease=new Disease();
				disease.PatNum=PatCur.PatNum;
				disease.DiseaseDefNum=FormDD.ListDiseaseDefsSelected[i].DiseaseDefNum;
				Diseases.Insert(disease);
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).ProblemCDS){
					using FormCDSIntervention FormCDSI=new FormCDSIntervention();
					FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(FormDD.ListDiseaseDefsSelected[i],PatCur);
					FormCDSI.ShowIfRequired();
					if(FormCDSI.DialogResult==DialogResult.Abort) {
						Diseases.Delete(disease);
						continue;//cancel 
					}
				}
				SecurityLogs.MakeLogEntry(Permissions.PatProblemListEdit,PatCur.PatNum,FormDD.ListDiseaseDefsSelected[i].DiseaseName+" added"); //Audit log made outside form because the form is just a list of problems and is called from many places.
			}
			FillProblems();
		}

		/*private void butIcd9_Click(object sender,EventArgs e) {
			using FormIcd9s formI=new FormIcd9s();
			formI.IsSelectionMode=true;
			formI.ShowDialog();
			if(formI.DialogResult!=DialogResult.OK) {
				return;
			}
			Disease disease=new Disease();
			disease.PatNum=PatCur.PatNum;
			disease.ICD9Num=formI.SelectedIcd9Num;
			Diseases.Insert(disease);
			FillProblems();
		}*/

		private void gridDiseases_CellClick(object sender,ODGridClickEventArgs e) {
			if(!CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				return;
			}
			if(e.Col!=0) {
				return;
			}
			List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(DiseaseDefs.GetItem(DiseaseList[e.Row].DiseaseDefNum));
			using FormInfobutton FormIB=new FormInfobutton(listKnowledgeRequests);
			FormIB.PatCur=PatCur;
			FormIB.ShowDialog();
			//Nothing to do with Dialog Result yet.
		}

		private void gridDiseases_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(DiseaseDefs.GetItem(DiseaseList[e.Row].DiseaseDefNum)==null) {
				MessageBox.Show(Lan.g(this,"Invalid disease.  Please run database maintenance method")+" "
					+nameof(DatabaseMaintenances.DiseaseWithInvalidDiseaseDef));
				return;
			}
			using FormDiseaseEdit FormD=new FormDiseaseEdit(DiseaseList[e.Row]);
			FormD.ShowDialog();
			if(FormD.DialogResult==DialogResult.OK 
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS 
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).ProblemCDS) 
			{
				using FormCDSIntervention FormCDSI=new FormCDSIntervention();
				FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(DiseaseDefs.GetItem(DiseaseList[e.Row].DiseaseDefNum),PatCur);
				FormCDSI.ShowIfRequired(false);
			}
			FillProblems();
		}

		private void checkShowInactiveProblems_CheckedChanged(object sender,EventArgs e) {
			FillProblems();
		}
		#endregion Problems Tab

		#region Allergies Tab
		private void FillAllergies() {
			allergyList=Allergies.GetAll(PatCur.PatNum,checkShowInactiveAllergies.Checked);
			gridAllergies.BeginUpdate();
			gridAllergies.ListGridColumns.Clear();
			GridColumn col;
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				col=new GridColumn("",18);//infoButton
				col.ImageList=imageListInfoButton;
				gridAllergies.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g("TableAllergies","Allergy"),150);
			gridAllergies.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableAllergies","Reaction"),500);
			gridAllergies.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableAllergies","Status"),40,HorizontalAlignment.Center);
			gridAllergies.ListGridColumns.Add(col);
			gridAllergies.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<allergyList.Count;i++){
				row=new GridRow();
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
					row.Cells.Add("0");//index of infobutton
				}
				AllergyDef allergyDef=AllergyDefs.GetOne(allergyList[i].AllergyDefNum);
				row.Cells.Add(allergyDef==null ? "MISSING ALLERGY" : allergyDef.Description);
				if(allergyList[i].DateAdverseReaction<DateTime.Parse("1-1-1800")) {
					row.Cells.Add(allergyList[i].Reaction);
				}
				else {
					row.Cells.Add(allergyList[i].Reaction+" "+allergyList[i].DateAdverseReaction.ToShortDateString());
				}
				if(allergyList[i].StatusIsActive) {
					row.Cells.Add("Active");
				}
				else {
					row.Cells.Add("Inactive");
				}
				gridAllergies.ListGridRows.Add(row);
			}
			gridAllergies.EndUpdate();
		}

		private void gridAllergies_CellClick(object sender,ODGridClickEventArgs e) {
			if(!CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				return;
			}
			if(e.Col!=0) {
				return;
			}
			List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(AllergyDefs.GetOne(allergyList[e.Row].AllergyDefNum));
			using FormInfobutton FormIB=new FormInfobutton(listKnowledgeRequests);
			FormIB.PatCur=PatCur;
			FormIB.ShowDialog();
			//Nothing to do with Dialog Result yet.
		}

		private void gridAllergies_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormAllergyEdit FAE=new FormAllergyEdit();
			FAE.AllergyCur=allergyList[gridAllergies.GetSelectedIndex()];
			FAE.ShowDialog();
			if(FAE.DialogResult==DialogResult.OK 
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS 
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).AllergyCDS) 
			{
				using FormCDSIntervention FormCDSI=new FormCDSIntervention();
				FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(AllergyDefs.GetOne(FAE.AllergyCur.AllergyDefNum),PatCur);
				FormCDSI.ShowIfRequired(false);
			}
			FillAllergies();
		}
		
		private void checkShowInactiveAllergies_CheckedChanged(object sender,EventArgs e) {
			FillAllergies();
		}

		private void butAddAllergy_Click(object sender,EventArgs e) {
			using FormAllergyEdit formA=new FormAllergyEdit();
			formA.AllergyCur=new Allergy();
			formA.AllergyCur.StatusIsActive=true;
			formA.AllergyCur.PatNum=PatCur.PatNum;
			formA.AllergyCur.IsNew=true;
			formA.ShowDialog();
			if(formA.DialogResult!=DialogResult.OK) {
				return;
			}
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).AllergyCDS) {
				using FormCDSIntervention FormCDSI=new FormCDSIntervention();
				FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(AllergyDefs.GetOne(formA.AllergyCur.AllergyDefNum),PatCur);
				FormCDSI.ShowIfRequired(false);
			}
			FillAllergies();
		}
		#endregion Allergies Tab

		#region Vital Signs Tab
		private void FillVitalSigns() {
			gridVitalSigns.BeginUpdate();
			gridVitalSigns.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Date",80);
			gridVitalSigns.ListGridColumns.Add(col);
			col=new GridColumn("Pulse",55);
			gridVitalSigns.ListGridColumns.Add(col);
			col=new GridColumn("Height",55);
			gridVitalSigns.ListGridColumns.Add(col);
			col=new GridColumn("Weight",55);
			gridVitalSigns.ListGridColumns.Add(col);
			col=new GridColumn("BP",55);
			gridVitalSigns.ListGridColumns.Add(col);
			col=new GridColumn("BMI",55);
			gridVitalSigns.ListGridColumns.Add(col);
			col=new GridColumn("Documentation for Followup or Ineligible",150);
			gridVitalSigns.ListGridColumns.Add(col);
			_listVitalSigns=Vitalsigns.Refresh(PatCur.PatNum);
			gridVitalSigns.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listVitalSigns.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listVitalSigns[i].DateTaken.ToShortDateString());
				row.Cells.Add(_listVitalSigns[i].Pulse.ToString()+" bpm");
				row.Cells.Add(_listVitalSigns[i].Height==0 ? "" : _listVitalSigns[i].Height+" in.");
				row.Cells.Add(_listVitalSigns[i].Weight==0 ? "" : _listVitalSigns[i].Weight+" lbs.");
				string bp="";
				if(_listVitalSigns[i].BpSystolic!=0 || _listVitalSigns[i].BpDiastolic!=0) {
					bp=_listVitalSigns[i].BpSystolic.ToString()+"/"+_listVitalSigns[i].BpDiastolic.ToString();
				}
				row.Cells.Add(bp);
				//BMI = (lbs*703)/(in^2)
				float bmi=Vitalsigns.CalcBMI(_listVitalSigns[i].Weight,_listVitalSigns[i].Height);
				if(bmi!=0) {
					row.Cells.Add(bmi.ToString("n1"));
				}
				else {//leave cell blank because there is not a valid bmi
					row.Cells.Add("");
				}
				row.Cells.Add(_listVitalSigns[i].Documentation);
				gridVitalSigns.ListGridRows.Add(row);
			}
			gridVitalSigns.EndUpdate();
		}

		private void butAddVitalSign_Click(object sender,EventArgs e) {
			using FormVitalsignEdit2014 FormVSE=new FormVitalsignEdit2014();
			FormVSE.VitalsignCur=new Vitalsign();
			FormVSE.VitalsignCur.PatNum=PatCur.PatNum;
			FormVSE.VitalsignCur.DateTaken=DateTime.Today;
			FormVSE.VitalsignCur.IsNew=true;
			FormVSE.ShowDialog();
			FillVitalSigns();
		}

		private void gridVitalSigns_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormVitalsignEdit2014 FormVSE=new FormVitalsignEdit2014();
			FormVSE.VitalsignCur=_listVitalSigns[e.Row];
			FormVSE.ShowDialog();
			FillVitalSigns();
		}

		private void butGrowthChart_Click(object sender,EventArgs e) {
			using FormEhrGrowthCharts FormGC=new FormEhrGrowthCharts();
			FormGC.PatNum=PatCur.PatNum;
			FormGC.ShowDialog();
		}
		#endregion Vital Signs Tab

		#region Tobacco Use Tab
		private void TobaccoUseTabLoad() {
			textDateAssessed.Text=DateTime.Now.ToString();
			textDateIntervention.Text=DateTime.Now.ToString();
			#region ComboSmokeStatus
			comboSmokeStatus.Items.Add("None");//First and default index
			//Smoking statuses add in the same order as they appear in the SmokingSnoMed enum (Starting at comboSmokeStatus index 1).
			//Changes to the enum order will change the order added so they will always match
			for(int i=0;i<Enum.GetNames(typeof(SmokingSnoMed)).Length;i++) {
				//if snomed code exists in the snomed table, use the snomed description for the combo box, otherwise use the original abbreviated description
				Snomed smokeCur=Snomeds.GetByCode(((SmokingSnoMed)i).ToString().Substring(1));
				if(smokeCur!=null) {
					comboSmokeStatus.Items.Add(smokeCur.Description);
				}
				else {
					switch((SmokingSnoMed)i) {
						case SmokingSnoMed._266927001:
							comboSmokeStatus.Items.Add("UnknownIfEver");
							break;
						case SmokingSnoMed._77176002:
							comboSmokeStatus.Items.Add("SmokerUnknownCurrent");
							break;
						case SmokingSnoMed._266919005:
							comboSmokeStatus.Items.Add("NeverSmoked");
							break;
						case SmokingSnoMed._8517006:
							comboSmokeStatus.Items.Add("FormerSmoker");
							break;
						case SmokingSnoMed._428041000124106:
							comboSmokeStatus.Items.Add("CurrentSomeDay");
							break;
						case SmokingSnoMed._449868002:
							comboSmokeStatus.Items.Add("CurrentEveryDay");
							break;
						case SmokingSnoMed._428061000124105:
							comboSmokeStatus.Items.Add("LightSmoker");
							break;
						case SmokingSnoMed._428071000124103:
							comboSmokeStatus.Items.Add("HeavySmoker");
							break;
					}
				}
			}
			comboSmokeStatus.SelectedIndex=0;//None
			try {
				comboSmokeStatus.SelectedIndex=(int)Enum.Parse(typeof(SmokingSnoMed),"_"+PatCur.SmokingSnoMed,true)+1;
			}
			catch {
				//if not one of the statuses in the enum, get the Snomed object from the patient's current smoking snomed code
				Snomed smokeCur=Snomeds.GetByCode(PatCur.SmokingSnoMed);
				if(smokeCur!=null) {//valid snomed code, set the combo box text to this snomed description
					comboSmokeStatus.SelectedIndex=-1;
					comboSmokeStatus.Text=smokeCur.Description;
				}
			}
			#endregion
			//This takes a while the first time the window loads due to Code Systems.
			Cursor=Cursors.WaitCursor;
			FillGridAssessments();
			FillGridInterventions();
			Cursor=Cursors.Default;
			#region ComboAssessmentType
			_listAssessmentCodes=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1278" },true);//'Tobacco Use Screening' value set
			//Should only happen if the EHR.dll doesn't exist or the codes in the ehrcode list don't exist in the corresponding table
			if(_listAssessmentCodes.Count==0) {
				//disable the tobacco use tab, message box will show if the user tries to select it
				((Control)tabControlFormMedical.TabPages[tabControlFormMedical.TabPages.IndexOfKey("tabTobaccoUse")]).Enabled=false;
				return;
			}
			_listAssessmentCodes.ForEach(x => comboAssessmentType.Items.Add(x.Description));
			string mostRecentAssessmentCode="";
			if(gridAssessments.ListGridRows.Count>1) {
				//gridAssessments.Rows are tagged with all TobaccoUseAssessed events for the patient ordered by DateTEvent, last is most recent
				mostRecentAssessmentCode=((EhrMeasureEvent)gridAssessments.ListGridRows[gridAssessments.ListGridRows.Count-1].Tag).CodeValueResult;
			}
			//use Math.Max so that if _listAssessmentCodes doesn't contain the mostRecentAssessment code the combobox will default to the first in the list
			comboAssessmentType.SelectedIndex=Math.Max(0,_listAssessmentCodes.FindIndex(x => x.CodeValue==mostRecentAssessmentCode));
			#endregion ComboAssessmentType
			#region ComboTobaccoStatus
			//list is filled with the EhrCodes for all tobacco user statuses using the CQM value set
			_listUserCodes=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1170" },true).OrderBy(x => x.Description).ToList();
			//list is filled with the EhrCodes for all tobacco non-user statuses using the CQM value set
			_listNonUserCodes=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1189" },true).OrderBy(x => x.Description).ToList();
			_listRecentTobaccoCodes=EhrCodes.GetForEventTypeByUse(EhrMeasureEventType.TobaccoUseAssessed);
			//list is filled with any SNOMEDCT codes that are attached to EhrMeasureEvents for the patient that are not in the User and NonUser lists
			_listCustomTobaccoCodes=new List<EhrCode>();
			//codeValues is an array of all user and non-user tobacco codes
			string[] codeValues=_listUserCodes.Concat(_listNonUserCodes).Concat(_listRecentTobaccoCodes).Select(x => x.CodeValue).ToArray();
			//listEventCodes will contain all unique tobacco codes that are not in the user and non-user lists
			List<string> listEventCodes=new List<string>();
			foreach(GridRow row in gridAssessments.ListGridRows) {
				string eventCodeCur=((EhrMeasureEvent)row.Tag).CodeValueResult;
				if(codeValues.Contains(eventCodeCur) || listEventCodes.Contains(eventCodeCur)) {
					continue;
				}
				listEventCodes.Add(eventCodeCur);
			}
			Snomed sCur;
			foreach(string eventCode in listEventCodes.OrderBy(x => x)) {
				sCur=Snomeds.GetByCode(eventCode);
				if(sCur==null) {//don't add invalid SNOMEDCT codes
					continue;
				}
				_listCustomTobaccoCodes.Add(new EhrCode { CodeValue=sCur.SnomedCode,Description=sCur.Description });
			}
			_listCustomTobaccoCodes=_listCustomTobaccoCodes.OrderBy(x => x.Description).ToList();
			//list will contain all of the tobacco status EhrCodes currently in comboTobaccoStatus
			_listTobaccoStatuses=new List<EhrCode>();
			//default to all tobacco statuses (custom, user, and non-user) in the status dropdown box
			radioRecentStatuses.Checked=true;//causes combo box and _listTobaccoStatuses to be filled with all statuses
			#endregion ComboTobaccoStatus
			#region ComboInterventionType and ComboInterventionCode
			//list is filled with EhrCodes for counseling interventions using the CQM value set
			_listCounselInterventionCodes=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.509" },true).OrderBy(x => x.Description).ToList();
			//list is filled with EhrCodes for medication interventions using the CQM value set
			_listMedInterventionCodes=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1190" },true).OrderBy(x => x.Description).ToList();
			_listRecentIntvCodes=EhrCodes.GetForIntervAndMedByUse(InterventionCodeSet.TobaccoCessation,new List<string> { "2.16.840.1.113883.3.526.3.1190" });
			_listInterventionCodes=new List<EhrCode>();
			//default to all interventions (couseling and medication) in the intervention dropdown box
			radioRecentInterventions.Checked=true;//causes combo box and _listInterventionCodes to be filled with all intervention codes
			#endregion ComboInterventionType and ComboInterventionCode
			_comboToolTip=new ToolTip() { InitialDelay=1000,ReshowDelay=1000,ShowAlways=true };
		}

		private void FillGridAssessments() {
			gridAssessments.BeginUpdate();
			gridAssessments.ListGridColumns.Clear();
			gridAssessments.ListGridColumns.Add(new GridColumn("Date",70));
			gridAssessments.ListGridColumns.Add(new GridColumn("Type",170));
			gridAssessments.ListGridColumns.Add(new GridColumn("Description",170));
			gridAssessments.ListGridColumns.Add(new GridColumn("Documentation",170));
			gridAssessments.ListGridRows.Clear();
			GridRow row;
			Loinc lCur;
			Snomed sCur;
			List<EhrMeasureEvent> listEvents=EhrMeasureEvents.RefreshByType(PatCur.PatNum,EhrMeasureEventType.TobaccoUseAssessed);
			foreach(EhrMeasureEvent eventCur in listEvents) {
				row=new GridRow();
				row.Cells.Add(eventCur.DateTEvent.ToShortDateString());
				lCur=Loincs.GetByCode(eventCur.CodeValueEvent);//TobaccoUseAssessed events can be one of three types, all LOINC codes
				row.Cells.Add(lCur!=null?lCur.NameLongCommon:eventCur.EventType.ToString());
				sCur=Snomeds.GetByCode(eventCur.CodeValueResult);
				row.Cells.Add(sCur!=null?sCur.Description:"");
				row.Cells.Add(eventCur.MoreInfo);
				row.Tag=eventCur;
				gridAssessments.ListGridRows.Add(row);
			}
			gridAssessments.EndUpdate();
		}

		private void FillGridInterventions() {
			gridInterventions.BeginUpdate();
			gridInterventions.ListGridColumns.Clear();
			gridInterventions.ListGridColumns.Add(new GridColumn("Date",70));
			gridInterventions.ListGridColumns.Add(new GridColumn("Type",150));
			gridInterventions.ListGridColumns.Add(new GridColumn("Description",160));
			gridInterventions.ListGridColumns.Add(new GridColumn("Declined",60) { TextAlign=HorizontalAlignment.Center });
			gridInterventions.ListGridColumns.Add(new GridColumn("Documentation",140));
			gridInterventions.ListGridRows.Clear();
			//build list of rows of CessationInterventions and CessationMedications so we can order the list by date and type before filling the grid
			List<GridRow> listRows=new List<GridRow>();
			GridRow row;
			#region CessationInterventions
			Cpt cptCur;
			Snomed sCur;
			RxNorm rCur;
			string type;
			string descript;
			List<Intervention> listInterventions=Interventions.Refresh(PatCur.PatNum,InterventionCodeSet.TobaccoCessation);
			foreach(Intervention iCur in listInterventions) {
				row=new GridRow();
				row.Cells.Add(iCur.DateEntry.ToShortDateString());
				type=InterventionCodeSet.TobaccoCessation.ToString()+" Counseling";
				descript="";
				switch(iCur.CodeSystem) {
					case "CPT":
						cptCur=Cpts.GetByCode(iCur.CodeValue);
						descript=cptCur!=null?cptCur.Description:"";
						break;
					case "SNOMEDCT":
						sCur=Snomeds.GetByCode(iCur.CodeValue);
						descript=sCur!=null?sCur.Description:"";
						break;
					case "RXNORM":
						//if the user checks the "Patient Declined" checkbox, we enter the tobacco cessation medication as an intervention that was declined
						type=InterventionCodeSet.TobaccoCessation.ToString()+" Medication";
						rCur=RxNorms.GetByRxCUI(iCur.CodeValue);
						descript=rCur!=null?rCur.Description:"";
						break;
				}
				row.Cells.Add(type);
				row.Cells.Add(descript);
				row.Cells.Add(iCur.IsPatDeclined?"X":"");
				row.Cells.Add(iCur.Note);
				row.Tag=iCur;
				listRows.Add(row);
			}
			#endregion
			#region CessationMedications
			//Tobacco Use Cessation Pharmacotherapy Value Set
			string[] arrayRxCuiStrings=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1190" },true)
				.Select(x => x.CodeValue).ToArray();
			//arrayRxCuiStrings will contain 41 RxCui strings for tobacco cessation medications if those exist in the rxnorm table
			List<MedicationPat> listMedPats=MedicationPats.Refresh(PatCur.PatNum,true).FindAll(x => arrayRxCuiStrings.Contains(x.RxCui.ToString()));
			foreach(MedicationPat medPatCur in listMedPats) {
				row=new GridRow();
				List<string> listMedDates=new List<string>();
				if(medPatCur.DateStart.Year>1880) {
					listMedDates.Add(medPatCur.DateStart.ToShortDateString());
				}
				if(medPatCur.DateStop.Year>1880) {
					listMedDates.Add(medPatCur.DateStop.ToShortDateString());
				}
				if(listMedDates.Count==0) {
					listMedDates.Add(medPatCur.DateTStamp.ToShortDateString());
				}
				row.Cells.Add(listMedDates.Count==0?"":string.Join(" - ",listMedDates));
				row.Cells.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Medication");
				row.Cells.Add(RxNorms.GetDescByRxCui(medPatCur.RxCui.ToString()));
				row.Cells.Add(medPatCur.PatNote);
				row.Tag=medPatCur;
				listRows.Add(row);
			}
			#endregion
			listRows.OrderBy(x => PIn.Date(x.Cells[0].Text))//rows ordered by date, oldest first
				.ThenBy(x => x.Cells[3].Text!="")
				//interventions at the top, declined med interventions below normal interventions
				.ThenBy(x => x.Tag.GetType().Name!="Intervention" || ((Intervention)x.Tag).CodeSystem=="RXNORM").ToList()
				.ForEach(x => gridInterventions.ListGridRows.Add(x));//then add rows to gridInterventions
			gridInterventions.EndUpdate();
		}

		private void gridAssessments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//we will allow them to change the DateTEvent, but not the status or more info box
			using FormEhrMeasureEventEdit FormM=new FormEhrMeasureEventEdit((EhrMeasureEvent)gridAssessments.ListGridRows[e.Row].Tag);
			FormM.ShowDialog();
			FillGridAssessments();
		}

		private void gridInterventions_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Object objCur=gridInterventions.ListGridRows[e.Row].Tag;
			//the intervention grid will be filled with Interventions and MedicationPats, load form accordingly
			if(objCur is Intervention) {
				using FormInterventionEdit FormI=new FormInterventionEdit();
				FormI.InterventionCur=(Intervention)objCur;
				FormI.IsAllTypes=false;
				FormI.IsSelectionMode=false;
				FormI.InterventionCur.IsNew=false;
				FormI.ShowDialog();
			}
			else if(objCur is MedicationPat) {
				using FormMedPat FormMP=new FormMedPat();
				FormMP.MedicationPatCur=(MedicationPat)objCur;
				FormMP.IsNew=false;
				FormMP.ShowDialog();
			}
			FillGridInterventions();
		}

		private void comboSmokeStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboSmokeStatus.SelectedIndex<1) {//If None or text set to other selected Snomed code so -1, do not create an event
				return;
			}
			//Insert measure event if one does not already exist for this date
			DateTime dateTEntered=PIn.DateT(textDateAssessed.Text);//will be set to DateTime.Now when form loads
			EhrMeasureEvent eventCur;
			foreach(GridRow row in gridAssessments.ListGridRows) {
				eventCur=(EhrMeasureEvent)row.Tag;
				if(eventCur.DateTEvent.Date==dateTEntered.Date) {//one already exists for this date, don't auto insert event
					return;
				}
			}
			//no entry for the date entered, so insert one
			eventCur=new EhrMeasureEvent();
			eventCur.DateTEvent=dateTEntered;
			eventCur.EventType=EhrMeasureEventType.TobaccoUseAssessed;
			eventCur.PatNum=PatCur.PatNum;
			eventCur.CodeValueEvent=_listAssessmentCodes[comboAssessmentType.SelectedIndex].CodeValue;
			eventCur.CodeSystemEvent=_listAssessmentCodes[comboAssessmentType.SelectedIndex].CodeSystem;
			//SelectedIndex guaranteed to be greater than 0
			eventCur.CodeValueResult=((SmokingSnoMed)comboSmokeStatus.SelectedIndex-1).ToString().Substring(1);
			eventCur.CodeSystemResult="SNOMEDCT";//only allow SNOMEDCT codes for now.
			eventCur.MoreInfo="";
			EhrMeasureEvents.Insert(eventCur);
			FillGridAssessments();
		}

		private void comboTobaccoStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboTobaccoStatus.SelectedIndex<_listTobaccoStatuses.Count) {//user selected a code in the list, just return
				return;
			}
			if(comboTobaccoStatus.SelectedIndex==_listTobaccoStatuses.Count
				&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Selecting a code that is not in the recommended list of codes may make "
					+"it more difficult to meet CQM's."))
			{
				comboTobaccoStatus.SelectedIndex=-1;
				return;
			}
			//user wants to select a custom status from the SNOMED list
			using FormSnomeds FormS=new FormSnomeds();
			FormS.IsSelectionMode=true;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				comboTobaccoStatus.SelectedIndex=-1;
				return;
			}
			if(!_listTobaccoStatuses.Any(x => x.CodeValue==FormS.SelectedSnomed.SnomedCode)) {
				_listCustomTobaccoCodes.Add(new EhrCode() { CodeValue=FormS.SelectedSnomed.SnomedCode,Description=FormS.SelectedSnomed.Description });
				_listCustomTobaccoCodes=_listCustomTobaccoCodes.OrderBy(x => x.Description).ToList();
				radioTobaccoStatuses_CheckedChanged(new[] { radioUserStatuses,radioNonUserStatuses }.Where(x => x.Checked)
					.DefaultIfEmpty(radioAllStatuses).FirstOrDefault()
					,new EventArgs());//refills drop down with newly added custom code
			}
			//selected code guaranteed to exist in the drop down at this point
			comboTobaccoStatus.Items.Clear();
			comboTobaccoStatus.Items.AddRange(_listTobaccoStatuses.Select(x => x.Description).ToArray());
			comboTobaccoStatus.Items.Add(Lan.g(this,"Choose from all SNOMED CT codes")+"...");
			comboTobaccoStatus.SelectedIndex=_listTobaccoStatuses.FindIndex(x => x.CodeValue==FormS.SelectedSnomed.SnomedCode);//add 1 for ...choose from
		}

		private void comboInterventionCode_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboInterventionCode.SelectedIndex>=0) {
				_comboToolTip.SetToolTip(comboInterventionCode,_listInterventionCodes[comboInterventionCode.SelectedIndex].Description);
			}
		}

		///<summary>Fill comboInterventionCode with counseling and medication intervention codes using _listCounselInterventionCodes
		///and/or _listMedInterventionCodes depending on which radio button is selected.</summary>
		private void radioInterventions_CheckedChanged(object sender,EventArgs e) {
			RadioButton radButCur=(RadioButton)sender;
			if(!radButCur.Checked) {//if not checked, do nothing, caused by another radio button being checked
				return;
			}
			_listInterventionCodes.Clear();
			if(radButCur.Name==radioRecentInterventions.Name) {
				_listInterventionCodes.AddRange(_listRecentIntvCodes);
			}
			if(new[] { radioAllInterventions.Name,radioCounselInterventions.Name }.Contains(radButCur.Name)) {
				_listInterventionCodes.AddRange(_listCounselInterventionCodes);
			}
			if(new[] { radioAllInterventions.Name,radioMedInterventions.Name }.Contains(radButCur.Name)) {
				_listInterventionCodes.AddRange(_listMedInterventionCodes);
			}
			_listInterventionCodes=_listInterventionCodes.OrderBy(x => x.Description).ToList();
			comboInterventionCode.Items.Clear();
			//this is the max width of the description, minus the width of "..." and, if > 30 items in the list, the width of the vertical scroll bar
			int maxItemWidth=comboInterventionCode.DropDownWidth-(_listInterventionCodes.Count>30?25:8);//8 for just "...", 25 for scroll bar plus "..."
			foreach(EhrCode code in _listInterventionCodes) {
				if(TextRenderer.MeasureText(code.Description,comboInterventionCode.Font).Width<comboInterventionCode.DropDownWidth-15
					|| code.Description.Length<3)
				{
					comboInterventionCode.Items.Add(code.Description);
					continue;
				}
				StringBuilder abbrDesc=new StringBuilder();
				foreach(char c in code.Description) {
					if(TextRenderer.MeasureText(abbrDesc.ToString()+c,comboInterventionCode.Font).Width<maxItemWidth) {
						abbrDesc.Append(c);
						continue;
					}
					comboInterventionCode.Items.Add(abbrDesc.ToString()+"...");
					break;
				}
			}
		}

		///<summary>Fill comboTobaccoStatus with user and non-user tobacco status codes using _listUserCodes and/or _listNonUserCodes
		///depending on which radio button is selected.</summary>
		private void radioTobaccoStatuses_CheckedChanged(object sender,EventArgs e) {
			RadioButton radButCur=(RadioButton)sender;
			if(!radButCur.Checked) {
				return;
			}
			_listTobaccoStatuses.Clear();
			if(_listCustomTobaccoCodes.Count>0) {
				_listTobaccoStatuses.AddRange(_listCustomTobaccoCodes);
			}
			if(radButCur.Name==radioRecentStatuses.Name) {
				_listTobaccoStatuses.AddRange(_listRecentTobaccoCodes);
			}
			else {
				if(new[] { radioAllStatuses.Name,radioUserStatuses.Name }.Contains(radButCur.Name)) {
					_listTobaccoStatuses.AddRange(_listUserCodes);
				}
				if(new[] { radioAllStatuses.Name,radioNonUserStatuses.Name }.Contains(radButCur.Name)) {
					_listTobaccoStatuses.AddRange(_listNonUserCodes);
				}
			}
			_listTobaccoStatuses=_listTobaccoStatuses.OrderBy(x => x.Description).ToList();
			comboTobaccoStatus.Items.Clear();
			comboTobaccoStatus.Items.AddRange(_listTobaccoStatuses.Select(x => x.Description).ToArray());
			comboTobaccoStatus.Items.Add(Lan.g(this,"Choose from all SNOMED CT codes")+"...");
		}

		///<summary>If the LOINC table has not been imported, the Tobacco Use tab is disabled, but we want it to remain visible like the other EHR show
		///feature enabled tabs.  But since the combo boxes etc. cannot be filled without the LOINC table, don't allow selecting the tab.</summary>
		private void tabControlFormMedical_Selecting(object sender,TabControlCancelEventArgs e) {
			if(!((Control)e.TabPage).Enabled) {
				e.Cancel=true;
				MsgBox.Show(this,"The codes used for Tobacco Use Screening assessments do not exist in the LOINC table in your database.  You must run the "
					+"Code System Importer tool in Setup | Chart | EHR to import this code set before accessing the Tobacco Use Tab.");
			}
		}

		private void butAssessed_Click(object sender,EventArgs e) {
			if(comboTobaccoStatus.SelectedIndex<0 || comboTobaccoStatus.SelectedIndex>=_listTobaccoStatuses.Count) {
				MsgBox.Show(this,"You must select a tobacco status.");
				return;
			}
			DateTime dateTEntered=PIn.DateT(textDateAssessed.Text);
			EhrMeasureEvent meas=new EhrMeasureEvent();
			meas.DateTEvent=dateTEntered;
			meas.EventType=EhrMeasureEventType.TobaccoUseAssessed;
			meas.PatNum=PatCur.PatNum;
			meas.CodeValueEvent=_listAssessmentCodes[comboAssessmentType.SelectedIndex].CodeValue;
			meas.CodeSystemEvent=_listAssessmentCodes[comboAssessmentType.SelectedIndex].CodeSystem;
			meas.CodeValueResult=_listTobaccoStatuses[comboTobaccoStatus.SelectedIndex].CodeValue;
			meas.CodeSystemResult="SNOMEDCT";//only allow SNOMEDCT codes for now.
			meas.MoreInfo="";
			EhrMeasureEvents.Insert(meas);
			comboTobaccoStatus.SelectedIndex=-1;
			FillGridAssessments();
		}

		private void butIntervention_Click(object sender,EventArgs e) {
			if(comboInterventionCode.SelectedIndex<0) {
				MsgBox.Show(this,"You must select an intervention code.");
				return;
			}
			EhrCode iCodeCur=_listInterventionCodes[comboInterventionCode.SelectedIndex];
			DateTime dateCur=PIn.Date(textDateIntervention.Text);
			if(iCodeCur.CodeSystem=="RXNORM" && !checkPatientDeclined.Checked) {//if patient declines the medication, enter as a declined intervention
				//codeVal will be RxCui of medication, see if it already exists in Medication table
				Medication medCur=Medications.GetMedicationFromDbByRxCui(PIn.Long(iCodeCur.CodeValue));
				if(medCur==null) {//no med with this RxCui, create one
					medCur=new Medication();
					Medications.Insert(medCur);//so that we will have the primary key
					medCur.GenericNum=medCur.MedicationNum;
					medCur.RxCui=PIn.Long(iCodeCur.CodeValue);
					medCur.MedName=RxNorms.GetDescByRxCui(iCodeCur.CodeValue);
					Medications.Update(medCur);
					Medications.RefreshCache();//refresh cache to include new medication
				}
				MedicationPat medPatCur=new MedicationPat();
				medPatCur.PatNum=PatCur.PatNum;
				medPatCur.ProvNum=PatCur.PriProv;
				medPatCur.MedicationNum=medCur.MedicationNum;
				medPatCur.RxCui=medCur.RxCui;
				medPatCur.DateStart=dateCur;
				using FormMedPat FormMP=new FormMedPat();
				FormMP.MedicationPatCur=medPatCur;
				FormMP.IsNew=true;
				FormMP.ShowDialog();
				if(FormMP.DialogResult!=DialogResult.OK) {
					return;
				}
				if(FormMP.MedicationPatCur.DateStart.Date<dateCur.AddMonths(-6).Date || FormMP.MedicationPatCur.DateStart.Date>dateCur.Date) {
					MsgBox.Show(this,"The medication order just entered is not within the 6 months prior to the date of this intervention.  You can modify the "
						+"date of the medication order in the patient's medical history section.");
				}
			}
			else {
				Intervention iCur=new Intervention();
				iCur.PatNum=PatCur.PatNum;
				iCur.ProvNum=PatCur.PriProv;
				iCur.DateEntry=dateCur;
				iCur.CodeValue=iCodeCur.CodeValue;
				iCur.CodeSystem=iCodeCur.CodeSystem;
				iCur.CodeSet=InterventionCodeSet.TobaccoCessation;
				iCur.IsPatDeclined=checkPatientDeclined.Checked;
				Interventions.Insert(iCur);
			}
			comboInterventionCode.SelectedIndex=-1;
			FillGridInterventions();
		}
		#endregion Tobacco Use Tab

		private void FormMedical_ResizeEnd(object sender,EventArgs e) {
			FillMeds();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(comboSmokeStatus.SelectedIndex==0) {//None
				PatCur.SmokingSnoMed="";
			}
			else {
				PatCur.SmokingSnoMed=((SmokingSnoMed)comboSmokeStatus.SelectedIndex-1).ToString().Substring(1);
			}
			PatCur.Premed=checkPremed.Checked;
			PatCur.MedUrgNote=textMedUrgNote.Text;
			Patients.Update(PatCur,_patOld);
			PatientNoteCur.Medical=textMedical.Text;
			PatientNoteCur.Service=textService.Text;
			PatientNoteCur.MedicalComp=textMedicalComp.Text;
			PatientNotes.Update(PatientNoteCur, PatCur.Guarantor);
			//Insert an ehrmeasureevent for CurrentMedsDocumented if user selected Yes and there isn't one with today's date
			if(radioMedsDocumentedYes.Checked && _EhrMeasureEventNum==0) {
				EhrMeasureEvent ehrMeasureEventCur=new EhrMeasureEvent();
				ehrMeasureEventCur.PatNum=PatCur.PatNum;
				ehrMeasureEventCur.DateTEvent=DateTime.Now;
				ehrMeasureEventCur.EventType=EhrMeasureEventType.CurrentMedsDocumented;
				ehrMeasureEventCur.CodeValueEvent="428191000124101";//SNOMEDCT code for document current meds procedure
				ehrMeasureEventCur.CodeSystemEvent="SNOMEDCT";
				EhrMeasureEvents.Insert(ehrMeasureEventCur);
			}
			//No is selected, if no EhrNotPerformed item for current meds documented, launch not performed edit window to allow user to select valid reason.
			if(radioMedsDocumentedNo.Checked) {
				if(_EhrNotPerfNum==0) {
					using FormEhrNotPerformedEdit FormNP=new FormEhrNotPerformedEdit();
					FormNP.EhrNotPerfCur=new EhrNotPerformed();
					FormNP.EhrNotPerfCur.IsNew=true;
					FormNP.EhrNotPerfCur.PatNum=PatCur.PatNum;
					FormNP.EhrNotPerfCur.ProvNum=PatCur.PriProv;
					FormNP.SelectedItemIndex=(int)EhrNotPerformedItem.DocumentCurrentMeds;
					FormNP.EhrNotPerfCur.DateEntry=DateTime.Today;
					FormNP.IsDateReadOnly=true;
					FormNP.ShowDialog();
					if(FormNP.DialogResult==DialogResult.OK) {//if they just inserted a not performed item, set the private class-wide variable for the next if statement
						_EhrNotPerfNum=FormNP.EhrNotPerfCur.EhrNotPerformedNum;
					}
				}
				if(_EhrNotPerfNum>0 && _EhrMeasureEventNum>0) {//if not performed item is entered with today's date, delete existing performed item
					EhrMeasureEvents.Delete(_EhrMeasureEventNum);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
