using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormMedical : FormODBase {
		///<summary>In-memory-only ODgrid used for printing.</summary>
		private GridOD gridMedsPrint;
		private ToolTip toolTipCombo;
		private Patient _patient;
		private List<Disease> _listDiseases;
		private PatientNote _patientNote;
		private List<Allergy> _listAllergies;
		private List<MedicationPat> _listMedicationPats;
		private int _pagesPrinted;
		private bool _isHeadingPrinted;
		private List<FamilyHealth> _listFamilyHealths;
		private int _isHeadingPrintH;
		private long _EhrMeasureEventNum;
		private long _EhrNotPerfNum;
		private List<Vitalsign> _listVitalSigns;
		///<summary>A copy of the original patient object, as it was when this form was first opened.</summary>
		private Patient _patOld;
		///<summary>List of tobacco use screening type codes.  Currently only 3 allowed SNOMED codes as of 2014.</summary>
		private List<EhrCode> _listEhrCodesAssessments;
		///<summary>All EhrCodes in the tobacco cessation counseling value set (2.16.840.1.113883.3.526.3.509).
		///When comboInterventionType has selected index 0, load the counseling intervention codes into comboInterventionCode.</summary>
		private List<EhrCode> _listEhrCodesCounselInterventions;
		///<summary>All EhrCodes in the tobacco cessation medication value set (2.16.840.1.113883.3.526.3.1190).
		///When comboInterventionType has selected index 1, load the medication intervention codes into comboInterventionCode.</summary>
		private List<EhrCode> _listEhrCodesMedInterventions;
		private List<EhrCode> _listEhrCodesRecentIntvs;
		///<summary>This list contains all of the intervention codes in the comboInterventionCode, counsel, medication, both.</summary>
		private List<EhrCode> _listEhrCodesInterventions;
		///<summary>All EhrCodes in the tobacco user value set (2.16.840.1.113883.3.526.3.1170).
		///When radioAll or radioUser is selected, comboTobaccoStatuses will be filled with this list.</summary>
		private List<EhrCode> _listEhrCodesUser;
		///<summary>All EhrCodes in the tobacco non-user value set (2.16.840.1.113883.3.526.3.1189).
		///When radioAll or radioNonUser is selected, comboTobaccoStatuses will be filled with this list.</summary>
		private List<EhrCode> _listEhrCodesNonUser;
		///<summary>List of tobacco statuses selected from the SNOMED list for this pat that aren't in the list of EHR user and non-user codes</summary>
		private List<EhrCode> _listEhrCodesCustomTobacco;
		///<summary>List of recently used tobacco statuses, ordered by a date used weighted sum of number of times used.  Codes used the most will be
		///first in the list, with more recent EhrMeasureEvents having a heavier weight.</summary>
		private List<EhrCode> _listEhrCodesRecentTobacco;
		///<summary>This list contains all of the tobacco statuses in the comboTobaccoStatus, user, non-user, or both.  This list may also contain
		///statuses that the user has selected from the SNOMED list that are not a user or non-user code.</summary>
		private List<EhrCode> _listEhrCodesTobaccoStatuses;
		///<summary>Tab name to pre-select when form loads.
		///i.e. "tabMedical", "tabProblems", "tabMedications", "tabAllergies", "tabFamHealthHist", "tabVitalSigns", or "tabTobaccoUse".</summary>
		private string _selectedTab;

		///<summary></summary>
		public FormMedical(PatientNote patientNote,Patient patient,string selectedTab=""){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_patient=patient;
			_patientNote=patientNote;
			_selectedTab=selectedTab;
			Lan.F(this);
		}

		private void FormMedical_Load(object sender, EventArgs e){
			SecurityLogs.MakeLogEntry(EnumPermType.MedicalInfoViewed,_patient.PatNum,"Patient medical information viewed");
			_patOld=_patient.Copy();
			checkPremed.Checked=_patient.Premed;
			textMedUrgNote.Text=_patient.MedUrgNote;
			textMedical.Text=_patientNote.Medical;
			textMedicalComp.Text=_patientNote.MedicalComp;
			textService.Text=_patientNote.Service;
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
				tabControlFormMedical.TabPages.Remove(tabFamHealthHist);
				tabControlFormMedical.TabPages.Remove(tabTobaccoUse);
			}
			List<EhrMeasureEvent> listEhrMeasureEventsDocumentedMeds=EhrMeasureEvents.RefreshByType(_patient.PatNum,EhrMeasureEventType.CurrentMedsDocumented);
			_EhrMeasureEventNum=0;
			for(int i=0;i<listEhrMeasureEventsDocumentedMeds.Count;i++) {
				if(listEhrMeasureEventsDocumentedMeds[i].DateTEvent.Date==DateTime.Today) {
					radioMedsDocumentedYes.Checked=true;
					_EhrMeasureEventNum=listEhrMeasureEventsDocumentedMeds[i].EhrMeasureEventNum;
					break;
				}
			}
			_EhrNotPerfNum=0;
			List<EhrNotPerformed> listEhrNotPerformeds=EhrNotPerformeds.Refresh(_patient.PatNum);
			for(int i=0;i<listEhrNotPerformeds.Count;i++) {
				if(listEhrNotPerformeds[i].CodeValue!="428191000124101") {//this is the only allowed code for Current Meds Documented procedure
					continue;
				}
				if(listEhrNotPerformeds[i].DateEntry.Date==DateTime.Today) {
					radioMedsDocumentedNo.Checked=!radioMedsDocumentedYes.Checked;//only check the No radio button if the Yes radio button is not already set
					_EhrNotPerfNum=listEhrNotPerformeds[i].EhrNotPerformedNum;
					break;
				}
			}
			//_selectedTab is set and tab wasn't removed from TabPages, i.e. EHR show feature enabled
			if(_selectedTab!="" && tabControlFormMedical.TabExists(_selectedTab)) {
				//If tab is disabled, i.e. tabTobaccoUse disabled due to LOINC table missing, tabControlFormMedical_Selecting event handler will cancel
				tabControlFormMedical.SelectTab(_selectedTab);
			}
		}

		#region Medications Tab
		private void FillMeds(bool isForPrinting = false) {
			GridOD gridToFill=gridMeds;
			if(isForPrinting) {
				gridToFill=gridMedsPrint;
			}
			Medications.RefreshCache();
			_listMedicationPats=MedicationPats.Refresh(_patient.PatNum,checkDiscontinued.Checked);
			gridToFill.BeginUpdate();
			gridToFill.Columns.Clear();
			GridColumn col;
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton && !isForPrinting) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				col=new GridColumn("",18);//infoButton
				col.ImageList=imageListInfoButton;
				gridToFill.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("TableMedications","Medication"),200);
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableMedications","Notes"),100);
			col.IsWidthDynamic=true;
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableMedications","Notes for Patient"),100);
			col.IsWidthDynamic=true;
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableMedications","Status"),45,HorizontalAlignment.Center);
			gridToFill.Columns.Add(col);
			if(!isForPrinting) {
				col=new GridColumn(Lan.g("TableMedications","Source"),60);
				gridToFill.Columns.Add(col);
			}
			gridToFill.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listMedicationPats.Count;i++) {
				row=new GridRow();
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton && !isForPrinting) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
					row.Cells.Add("0");//index of infobutton
				}
				if(_listMedicationPats[i].MedicationNum==0) {
					row.Cells.Add(_listMedicationPats[i].MedDescript);
					row.Cells.Add("");//generic notes
				}
				else {
					Medication medicationGeneric=Medications.GetGeneric(_listMedicationPats[i].MedicationNum);
					string medName=Medications.GetMedication(_listMedicationPats[i].MedicationNum).MedName;
					if(medicationGeneric.MedicationNum!=_listMedicationPats[i].MedicationNum) {//not generic
						medName+=" ("+medicationGeneric.MedName+")";
					}
					row.Cells.Add(medName);
					row.Cells.Add(Medications.GetGeneric(_listMedicationPats[i].MedicationNum).Notes);
				}
				row.Cells.Add(_listMedicationPats[i].PatNote);
				if(MedicationPats.IsMedActive(_listMedicationPats[i])) {
					row.Cells.Add("Active");
				}
				else {
					row.Cells.Add("Inactive");
				}
				if(!isForPrinting) {
					if(Erx.IsFromNewCrop(_listMedicationPats[i].ErxGuid)) {
						row.Cells.Add("NewCrop");
					}
					else if(Erx.IsFromDoseSpot(_listMedicationPats[i].ErxGuid) || Erx.IsDoseSpotPatReported(_listMedicationPats[i].ErxGuid)) {
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
			MedicationPat medicationPat=_listMedicationPats[e.Row];
			if(medicationPat.MedicationNum==0) {//Medication orders returned from NewCrop
				listKnowledgeRequests=new List<KnowledgeRequest>();
				KnowledgeRequest knowledgeRequest=new KnowledgeRequest();
				knowledgeRequest.Type="Medication";
				knowledgeRequest.Code=POut.Long(medicationPat.RxCui);
				knowledgeRequest.CodeSystem=CodeSyst.RxNorm;
				knowledgeRequest.Description=medicationPat.MedDescript;
				listKnowledgeRequests.Add(knowledgeRequest);
			}
			else {
				listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(_listMedicationPats[e.Row]);				
			}
			using FormInfobutton formInfoButton=new FormInfobutton(listKnowledgeRequests);
			formInfoButton.PatientCur=_patient;
			//FormInfoButton allows MedicationCur to be null, so this will still work for medication orders returned from NewCrop (because MedicationNum will be 0).
			formInfoButton.ShowDialog();
			//Nothing to do with Dialog Result yet.
		}

		private void gridMeds_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormMedPat formMedPat=new FormMedPat();
			formMedPat.MedicationPatCur=_listMedicationPats[e.Row];
			formMedPat.ShowDialog();
			if(formMedPat.DialogResult==DialogResult.OK 
				&& formMedPat.MedicationPatCur!=null //Can get be null if the user removed the medication from the patient.
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS 
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).MedicationCDS) 
			{
				using FormCDSIntervention formCDSIntervention=new FormCDSIntervention();
				if(formMedPat.MedicationPatCur.MedicationNum > 0) {//0 indicats the med is from NewCrop.
					Medication medication=Medications.GetMedication(formMedPat.MedicationPatCur.MedicationNum);
					if(medication!=null) {
						formCDSIntervention.ListCDSInterventions=EhrTriggers.TriggerMatch(medication,_patient);
						formCDSIntervention.ShowIfRequired();
					}
				}
				else if(formMedPat.MedicationPatCur.RxCui > 0) {//Meds from NewCrop might have a valid RxNorm.
					RxNorm rxNorm=RxNorms.GetByRxCUI(formMedPat.MedicationPatCur.RxCui.ToString());
					if(rxNorm!=null) {
						formCDSIntervention.ListCDSInterventions=EhrTriggers.TriggerMatch(rxNorm,_patient);
						formCDSIntervention.ShowIfRequired();
					}
				}
			}
			FillMeds();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.PatMedicationListEdit)) {
				return;
			}
			//select medication from list.  Additional meds can be added to the list from within that dlg
			using FormMedications formMedications=new FormMedications();
			formMedications.IsSelectionMode=true;
			formMedications.ShowDialog();
			if(formMedications.DialogResult!=DialogResult.OK){
				return;
			} 
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).MedicationCDS) {
				using FormCDSIntervention formCDSIntervention=new FormCDSIntervention();
				formCDSIntervention.ListCDSInterventions=EhrTriggers.TriggerMatch(Medications.GetMedication(formMedications.SelectedMedicationNum),_patient);
				if(formCDSIntervention.ListCDSInterventions!=null && formCDSIntervention.ListCDSInterventions.Count>0) {
					formCDSIntervention.ShowIfRequired();
					if(formCDSIntervention.DialogResult==DialogResult.Cancel) {
						return;//do not add medication
					}
				}
			}
			MedicationPat medicationPat=new MedicationPat();
			medicationPat.PatNum=_patient.PatNum;
			medicationPat.MedicationNum=formMedications.SelectedMedicationNum;
			medicationPat.RxCui=Medications.GetMedication(formMedications.SelectedMedicationNum).RxCui;
			//MedicationPatCur.ProvNum=PatCur.PriProv;//Medications are not prescribed by dentists, so this wouldn't make sense.
			using FormMedPat formMedPat=new FormMedPat();
			formMedPat.MedicationPatCur=medicationPat;
			formMedPat.IsNew=true;
			formMedPat.ShowDialog();
			if(formMedPat.DialogResult!=DialogResult.OK){
				return;
			}
			FillMeds();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			gridMedsPrint=new GridOD();
			gridMedsPrint.Width=800;
			gridMedsPrint.TranslationName="";
			FillMeds(isForPrinting:true);//not nessecary to explicity name parameter but makes code easier to read.
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Medications printed"),auditPatNum:_patient.PatNum);
			Cursor=Cursors.Default;
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangleBounds.Top;
			int center=rectangleBounds.X+rectangleBounds.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Medications List For ")+_patient.FName+" "+_patient.LName;
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				text=Lan.g(this,"Created ")+DateTime.Now.ToString();
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=20;
				_isHeadingPrinted=true;
				_isHeadingPrintH=yPos;
			}
			#endregion
			yPos=gridMedsPrint.PrintPage(g,_pagesPrinted,rectangleBounds,_isHeadingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void checkDiscontinued_CheckChanged(object sender,EventArgs e) {
			FillMeds();
		}

		#endregion Medications Tab

		#region Medical Info Tab
		/// <summary>This report is a brute force, one page medical history report. It is not designed to handle more than one page. It does not print service notes or medications.</summary>
		private void butPrintMedical_Click(object sender,EventArgs e) {
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPageMedical,
				Lan.g(this,"Medications information printed"),
				auditPatNum:_patient.PatNum,
				printoutOrigin:PrintoutOrigin.AtMargin
			);
		}

		private void pd_PrintPageMedical(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			Font fontBody=new Font(FontFamily.GenericSansSerif,10);
			StringFormat stringFormat=new StringFormat();
			int yPos=rectangleBounds.Top;
			int center=rectangleBounds.X+rectangleBounds.Width/2;
			int textHeight;
			RectangleF rectangleFText;
			text=Lan.g(this,"Medical History For ")+_patient.FName+" "+_patient.LName;
			g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
			yPos+=(int)g.MeasureString(text,fontHeading).Height;
			text=Lan.g(this,"Birthdate: ")+_patient.Birthdate.ToShortDateString();
			g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
			yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
			text=Lan.g(this,"Created ")+DateTime.Now.ToString();
			g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
			yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
			yPos+=25;
			if(gridDiseases.ListGridRows.Count>0) {
				text=Lan.g(this,"Problems");
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
				yPos+=2;
				yPos=gridDiseases.PrintPage(g,0,rectangleBounds,yPos);
				yPos+=25;
			}
			if(gridAllergies.ListGridRows.Count>0) {
				text=Lan.g(this,"Allergies");
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
				yPos+=2;
				yPos=gridAllergies.PrintPage(g,0,rectangleBounds,yPos);
				yPos+=25;
			}
			text=Lan.g(this,"Premedicate (PAC or other): ")+(checkPremed.Checked?"Y":"N");
			textHeight=(int)g.MeasureString(text,fontBody,rectangleBounds.Width).Height;
			rectangleFText=new Rectangle(rectangleBounds.Left,yPos,rectangleBounds.Width,textHeight);
			g.DrawString(text,fontSubHeading,Brushes.Black,rectangleFText);
			yPos+=textHeight;
			yPos+=10;
			text=Lan.g(this,"Medical Urgent Note");
			g.DrawString(text,fontSubHeading,Brushes.Black,rectangleBounds.Left,yPos);
			yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
			text=textMedUrgNote.Text;
			textHeight=(int)g.MeasureString(text,fontBody,rectangleBounds.Width).Height;
			rectangleFText=new Rectangle(rectangleBounds.Left,yPos,rectangleBounds.Width,textHeight);
			g.DrawString(text,fontBody,Brushes.Black,rectangleFText);//maybe red?
			yPos+=textHeight;
			yPos+=10;
			text=Lan.g(this,"Medical Summary");
			g.DrawString(text,fontSubHeading,Brushes.Black,rectangleBounds.Left,yPos);
			yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
			text=textMedical.Text;
			textHeight=(int)g.MeasureString(text,fontBody,rectangleBounds.Width).Height;
			rectangleFText=new Rectangle(rectangleBounds.Left,yPos,rectangleBounds.Width,textHeight);
			g.DrawString(text,fontBody,Brushes.Black,rectangleFText);
			yPos+=textHeight;
			yPos+=10;
			text=Lan.g(this,"Medical History - Complete and Detailed");
			g.DrawString(text,fontSubHeading,Brushes.Black,rectangleBounds.Left,yPos);
			yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
			text=textMedicalComp.Text;
			textHeight=(int)g.MeasureString(text,fontBody,rectangleBounds.Width).Height;
			rectangleFText=new Rectangle(rectangleBounds.Left,yPos,rectangleBounds.Width,textHeight);
			g.DrawString(text,fontBody,Brushes.Black,rectangleFText);
			yPos+=textHeight;
			g.Dispose();
		}
		#endregion Medical Info Tab

		#region Family Health History Tab
		private void FillFamilyHealth() {
			_listFamilyHealths=FamilyHealths.GetFamilyHealthForPat(_patient.PatNum);
			gridFamilyHealth.BeginUpdate();
			gridFamilyHealth.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableFamilyHealth","Relationship"),150,HorizontalAlignment.Center);
			gridFamilyHealth.Columns.Add(col);
			col=new GridColumn(Lan.g("TableFamilyHealth","Name"),150);
			gridFamilyHealth.Columns.Add(col);
			col=new GridColumn(Lan.g("TableFamilyHealth","Problem"),180);
			gridFamilyHealth.Columns.Add(col);
			gridFamilyHealth.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listFamilyHealths.Count;i++) {
				row=new GridRow();
				row.Cells.Add(Lan.g("enumFamilyRelationship",_listFamilyHealths[i].Relationship.ToString()));
				row.Cells.Add(_listFamilyHealths[i].PersonName);
				row.Cells.Add(DiseaseDefs.GetName(_listFamilyHealths[i].DiseaseDefNum));
				gridFamilyHealth.ListGridRows.Add(row);
			}
			gridFamilyHealth.EndUpdate();
		}

		private void gridFamilyHealth_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormFamilyHealthEdit formFamilyHealthEdit=new FormFamilyHealthEdit();
			formFamilyHealthEdit.FamilyHealthCur=_listFamilyHealths[e.Row];
			formFamilyHealthEdit.ShowDialog();
			FillFamilyHealth();
		}

		private void butAddFamilyHistory_Click(object sender,EventArgs e) {
			using FormFamilyHealthEdit formFamilyHealthEdit=new FormFamilyHealthEdit();
			FamilyHealth familyHealth=new FamilyHealth();
			familyHealth.PatNum=_patient.PatNum;
			familyHealth.IsNew=true;
			formFamilyHealthEdit.FamilyHealthCur=familyHealth;
			formFamilyHealthEdit.ShowDialog();
			if(formFamilyHealthEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillFamilyHealth();
		}
		#endregion Family Health History Tab

		#region Problems Tab
		private void FillProblems(){
			_listDiseases=Diseases.Refresh(checkShowInactiveProblems.Checked,_patient.PatNum);
			gridDiseases.BeginUpdate();
			gridDiseases.Columns.Clear();
			GridColumn col;
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				col=new GridColumn("",18);//infoButton
				col.ImageList=imageListInfoButton;
				gridDiseases.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("TableDiseases","Name"),200);//total is about 325
			gridDiseases.Columns.Add(col);
			col=new GridColumn(Lan.g("TableDiseases","Patient Note"),450);
			gridDiseases.Columns.Add(col);
			col=new GridColumn(Lan.g("TableDisease","Status"),40,HorizontalAlignment.Center);
			gridDiseases.Columns.Add(col);
			gridDiseases.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listDiseases.Count;i++){
				row=new GridRow();
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
					row.Cells.Add("0");//index of infobutton
				}
				if(_listDiseases[i].DiseaseDefNum!=0) {
					row.Cells.Add(DiseaseDefs.GetName(_listDiseases[i].DiseaseDefNum));
				}
				else {
					row.Cells.Add(DiseaseDefs.GetName(_listDiseases[i].DiseaseDefNum));
				}
				row.Cells.Add(_listDiseases[i].PatNote);
				row.Cells.Add(_listDiseases[i].ProbStatus.ToString());
				gridDiseases.ListGridRows.Add(row);
			}
			gridDiseases.EndUpdate();
		}

		private void butAddProblem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.PatProblemListEdit)) {
				return;
			}
			//get the list of disease def nums that should be highlighted in FormDiseaseDefs.
			List<long> listDiseaseDefNums=_listDiseases.Where(x => x.ProbStatus == ProblemStatus.Active).ToList().Select(x => x.DiseaseDefNum).ToList();
			using FormDiseaseDefs formDiseaseDefs=new FormDiseaseDefs();
			formDiseaseDefs.ListDiseaseDefNumsColored=listDiseaseDefNums;
			formDiseaseDefs.IsSelectionMode=true;
			formDiseaseDefs.IsMultiSelect=true;
			formDiseaseDefs.ListDiseaseDefsSelected=new List<DiseaseDef>();
			formDiseaseDefs.ShowDialog();
			if(formDiseaseDefs.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<formDiseaseDefs.ListDiseaseDefsSelected.Count;i++) {
				Disease disease=new Disease();
				disease.PatNum=_patient.PatNum;
				disease.DiseaseDefNum=formDiseaseDefs.ListDiseaseDefsSelected[i].DiseaseDefNum;
				Diseases.Insert(disease);
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).ProblemCDS){
					using FormCDSIntervention formCDSIntervention=new FormCDSIntervention();
					formCDSIntervention.ListCDSInterventions=EhrTriggers.TriggerMatch(formDiseaseDefs.ListDiseaseDefsSelected[i],_patient);
					if(formCDSIntervention.ListCDSInterventions!=null && formCDSIntervention.ListCDSInterventions.Count>0) {
						formCDSIntervention.ShowIfRequired();
						if(formCDSIntervention.DialogResult==DialogResult.Cancel) {
							Diseases.Delete(disease);
							continue;//cancel 
						}
					}
				}
				SecurityLogs.MakeLogEntry(EnumPermType.PatProblemListEdit,_patient.PatNum,formDiseaseDefs.ListDiseaseDefsSelected[i].DiseaseName+" added"); //Audit log made outside form because the form is just a list of problems and is called from many places.
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
			List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(DiseaseDefs.GetItem(_listDiseases[e.Row].DiseaseDefNum));
			using FormInfobutton formInfoButton=new FormInfobutton(listKnowledgeRequests);
			formInfoButton.PatientCur=_patient;
			formInfoButton.ShowDialog();
			//Nothing to do with Dialog Result yet.
		}

		private void gridDiseases_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(DiseaseDefs.GetItem(_listDiseases[e.Row].DiseaseDefNum)==null) {
				MessageBox.Show(Lan.g(this,"Invalid disease.  Please run database maintenance method")+" "
					+nameof(DatabaseMaintenances.DiseaseWithInvalidDiseaseDef));
				return;
			}
			FrmDiseaseEdit frmDiseaseEdit=new FrmDiseaseEdit(_listDiseases[e.Row]);
			frmDiseaseEdit.ShowDialog();
			if(frmDiseaseEdit.IsDialogOK 
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS 
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).ProblemCDS) 
			{
				using FormCDSIntervention formCDSIntervention=new FormCDSIntervention();
				formCDSIntervention.ListCDSInterventions=EhrTriggers.TriggerMatch(DiseaseDefs.GetItem(_listDiseases[e.Row].DiseaseDefNum),_patient);
				formCDSIntervention.ShowIfRequired();
			}
			FillProblems();
		}

		private void checkShowInactiveProblems_CheckedChanged(object sender,EventArgs e) {
			FillProblems();
		}
		#endregion Problems Tab

		#region Allergies Tab
		private void FillAllergies() {
			_listAllergies=Allergies.GetAll(_patient.PatNum,checkShowInactiveAllergies.Checked);
			gridAllergies.BeginUpdate();
			gridAllergies.Columns.Clear();
			GridColumn col;
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				col=new GridColumn("",18);//infoButton
				col.ImageList=imageListInfoButton;
				gridAllergies.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("TableAllergies","Allergy"),150);
			gridAllergies.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAllergies","Reaction"),500);
			gridAllergies.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAllergies","Status"),40,HorizontalAlignment.Center);
			gridAllergies.Columns.Add(col);
			gridAllergies.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listAllergies.Count;i++){
				row=new GridRow();
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
					row.Cells.Add("0");//index of infobutton
				}
				AllergyDef allergyDef=AllergyDefs.GetOne(_listAllergies[i].AllergyDefNum);
				string strAllergyDescription = "MISSING ALLERGY";
				if(allergyDef!=null) {
					strAllergyDescription = allergyDef.Description;
				}
				row.Cells.Add(strAllergyDescription);
				if(_listAllergies[i].DateAdverseReaction<DateTime.Parse("1-1-1800")) {
					row.Cells.Add(_listAllergies[i].Reaction);
				}
				else {
					row.Cells.Add(_listAllergies[i].Reaction+" "+_listAllergies[i].DateAdverseReaction.ToShortDateString());
				}
				if(_listAllergies[i].StatusIsActive) {
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
			List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(AllergyDefs.GetOne(_listAllergies[e.Row].AllergyDefNum));
			using FormInfobutton formInfoButton=new FormInfobutton(listKnowledgeRequests);
			formInfoButton.PatientCur=_patient;
			formInfoButton.ShowDialog();
			//Nothing to do with Dialog Result yet.
		}

		private void gridAllergies_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormAllergyEdit formAllergyEdit=new FormAllergyEdit();
			formAllergyEdit.AllergyCur=_listAllergies[gridAllergies.GetSelectedIndex()];
			formAllergyEdit.ShowDialog();
			if(formAllergyEdit.DialogResult==DialogResult.OK 
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS 
				&& CDSPermissions.GetForUser(Security.CurUser.UserNum).AllergyCDS) 
			{
				using FormCDSIntervention formCDSIntervetion=new FormCDSIntervention();
				formCDSIntervetion.ListCDSInterventions=EhrTriggers.TriggerMatch(AllergyDefs.GetOne(formAllergyEdit.AllergyCur.AllergyDefNum),_patient);
				formCDSIntervetion.ShowIfRequired();
			}
			FillAllergies();
		}
		
		private void checkShowInactiveAllergies_CheckedChanged(object sender,EventArgs e) {
			FillAllergies();
		}

		private void butAddAllergy_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.PatAllergyListEdit)) {
				return;
			}
			using FormAllergyEdit formAllergyEdit=new FormAllergyEdit();
			formAllergyEdit.AllergyCur=new Allergy();
			formAllergyEdit.AllergyCur.StatusIsActive=true;
			formAllergyEdit.AllergyCur.PatNum=_patient.PatNum;
			formAllergyEdit.AllergyCur.IsNew=true;
			formAllergyEdit.ShowDialog();
			if(formAllergyEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).AllergyCDS) {
				using FormCDSIntervention formCDSIntervention=new FormCDSIntervention();
				formCDSIntervention.ListCDSInterventions=EhrTriggers.TriggerMatch(AllergyDefs.GetOne(formAllergyEdit.AllergyCur.AllergyDefNum),_patient);
				formCDSIntervention.ShowIfRequired();
			}
			FillAllergies();
		}
		#endregion Allergies Tab

		#region Vital Signs Tab
		private void FillVitalSigns() {
			gridVitalSigns.BeginUpdate();
			gridVitalSigns.Columns.Clear();
			GridColumn col=new GridColumn("Date",80);
			gridVitalSigns.Columns.Add(col);
			col=new GridColumn("Pulse",55);
			gridVitalSigns.Columns.Add(col);
			col=new GridColumn("Height",55);
			gridVitalSigns.Columns.Add(col);
			col=new GridColumn("Weight",55);
			gridVitalSigns.Columns.Add(col);
			col=new GridColumn("BP",55);
			gridVitalSigns.Columns.Add(col);
			col=new GridColumn("BMI",55);
			gridVitalSigns.Columns.Add(col);
			col=new GridColumn("Documentation for Followup or Ineligible",150);
			gridVitalSigns.Columns.Add(col);
			_listVitalSigns=Vitalsigns.Refresh(_patient.PatNum);
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
			using FormVitalsignEdit2014 formVitalSignEdit2014=new FormVitalsignEdit2014();
			formVitalSignEdit2014.VitalsignCur=new Vitalsign();
			formVitalSignEdit2014.VitalsignCur.PatNum=_patient.PatNum;
			formVitalSignEdit2014.VitalsignCur.DateTaken=DateTime.Today;
			formVitalSignEdit2014.VitalsignCur.IsNew=true;
			formVitalSignEdit2014.ShowDialog();
			FillVitalSigns();
		}

		private void gridVitalSigns_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormVitalsignEdit2014 formVitalSignEdit2014=new FormVitalsignEdit2014();
			formVitalSignEdit2014.VitalsignCur=_listVitalSigns[e.Row];
			formVitalSignEdit2014.ShowDialog();
			FillVitalSigns();
		}

		private void butGrowthChart_Click(object sender,EventArgs e) {
			using FormEhrGrowthCharts formEhrGrowthCharts=new FormEhrGrowthCharts();
			formEhrGrowthCharts.PatNum=_patient.PatNum;
			formEhrGrowthCharts.ShowDialog();
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
				Snomed snomedSmoke=Snomeds.GetByCode(((SmokingSnoMed)i).ToString().Substring(1));
				if(snomedSmoke!=null) {
					comboSmokeStatus.Items.Add(snomedSmoke.Description);
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
				comboSmokeStatus.SelectedIndex=(int)Enum.Parse(typeof(SmokingSnoMed),"_"+_patient.SmokingSnoMed,true)+1;
			}
			catch {
				//if not one of the statuses in the enum, get the Snomed object from the patient's current smoking snomed code
				Snomed snomedSmoke=Snomeds.GetByCode(_patient.SmokingSnoMed);
				if(snomedSmoke!=null) {//valid snomed code, set the combo box text to this snomed description
					comboSmokeStatus.SelectedIndex=-1;
					comboSmokeStatus.Text=snomedSmoke.Description;
				}
			}
			#endregion
			//This takes a while the first time the window loads due to Code Systems.
			Cursor=Cursors.WaitCursor;
			FillGridAssessments();
			FillGridInterventions();
			Cursor=Cursors.Default;
			#region ComboAssessmentType
			_listEhrCodesAssessments=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1278" },true);//'Tobacco Use Screening' value set
			//Should only happen if the EHR.dll doesn't exist or the codes in the ehrcode list don't exist in the corresponding table
			if(_listEhrCodesAssessments.Count==0) {
				//disable the tobacco use tab, message box will show if the user tries to select it
				tabTobaccoUse.Enabled=false;
				return;
			}
			for(int i=0;i<_listEhrCodesAssessments.Count;i++) {
				comboAssessmentType.Items.Add(_listEhrCodesAssessments[i].Description);
			}
			string mostRecentAssessmentCode="";
			if(gridAssessments.ListGridRows.Count>1) {
				//gridAssessments.Rows are tagged with all TobaccoUseAssessed events for the patient ordered by DateTEvent, last is most recent
				mostRecentAssessmentCode=((EhrMeasureEvent)gridAssessments.ListGridRows[gridAssessments.ListGridRows.Count-1].Tag).CodeValueResult;
			}
			//use Math.Max so that if _listAssessmentCodes doesn't contain the mostRecentAssessment code the combobox will default to the first in the list
			comboAssessmentType.SelectedIndex=Math.Max(0,_listEhrCodesAssessments.FindIndex(x => x.CodeValue==mostRecentAssessmentCode));
			#endregion ComboAssessmentType
			#region ComboTobaccoStatus
			//list is filled with the EhrCodes for all tobacco user statuses using the CQM value set
			_listEhrCodesUser=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1170" },true).OrderBy(x => x.Description).ToList();
			//list is filled with the EhrCodes for all tobacco non-user statuses using the CQM value set
			_listEhrCodesNonUser=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1189" },true).OrderBy(x => x.Description).ToList();
			_listEhrCodesRecentTobacco=EhrCodes.GetForEventTypeByUse(EhrMeasureEventType.TobaccoUseAssessed);
			//list is filled with any SNOMEDCT codes that are attached to EhrMeasureEvents for the patient that are not in the User and NonUser lists
			_listEhrCodesCustomTobacco=new List<EhrCode>();
			//codeValues is an array of all user and non-user tobacco codes
			string[] stringArrayCodeValues=_listEhrCodesUser.Concat(_listEhrCodesNonUser).Concat(_listEhrCodesRecentTobacco).Select(x => x.CodeValue).ToArray();
			//listEventCodes will contain all unique tobacco codes that are not in the user and non-user lists
			List<string> listEventCodes=new List<string>();
			for(int i=0;i<gridAssessments.ListGridRows.Count;i++) {
				if(stringArrayCodeValues.Contains(((EhrMeasureEvent)gridAssessments.ListGridRows[i].Tag).CodeValueResult) 
					|| listEventCodes.Contains(((EhrMeasureEvent)gridAssessments.ListGridRows[i].Tag).CodeValueResult)) 
				{
					continue;
				}
				listEventCodes.Add(((EhrMeasureEvent)gridAssessments.ListGridRows[i].Tag).CodeValueResult);
			}
			Snomed snomed;
			List<string> listEventCodesOrderBy=listEventCodes.OrderBy(x => x).ToList();
			for(int i=0;i<listEventCodesOrderBy.Count;i++) {
				snomed=Snomeds.GetByCode(listEventCodesOrderBy[i]);
				if(snomed==null) {//don't add invalid SNOMEDCT codes
					continue;
				}
				EhrCode ehrCode = new EhrCode();
				ehrCode.CodeValue=snomed.SnomedCode;
				ehrCode.Description=snomed.Description;
				_listEhrCodesCustomTobacco.Add(ehrCode);
			}
			_listEhrCodesCustomTobacco=_listEhrCodesCustomTobacco.OrderBy(x => x.Description).ToList();
			//list will contain all of the tobacco status EhrCodes currently in comboTobaccoStatus
			_listEhrCodesTobaccoStatuses=new List<EhrCode>();
			//default to all tobacco statuses (custom, user, and non-user) in the status dropdown box
			radioRecentStatuses.Checked=true;//causes combo box and _listTobaccoStatuses to be filled with all statuses
			#endregion ComboTobaccoStatus
			#region ComboInterventionType and ComboInterventionCode
			//list is filled with EhrCodes for counseling interventions using the CQM value set
			_listEhrCodesCounselInterventions=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.509" },true).OrderBy(x => x.Description).ToList();
			//list is filled with EhrCodes for medication interventions using the CQM value set
			_listEhrCodesMedInterventions=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1190" },true).OrderBy(x => x.Description).ToList();
			_listEhrCodesRecentIntvs=EhrCodes.GetForIntervAndMedByUse(InterventionCodeSet.TobaccoCessation,new List<string> { "2.16.840.1.113883.3.526.3.1190" });
			_listEhrCodesInterventions=new List<EhrCode>();
			//default to all interventions (couseling and medication) in the intervention dropdown box
			radioRecentInterventions.Checked=true;//causes combo box and _listInterventionCodes to be filled with all intervention codes
			#endregion ComboInterventionType and ComboInterventionCode
			toolTipCombo=new ToolTip();
			toolTipCombo.InitialDelay=1000;
			toolTipCombo.ReshowDelay=1000;
			toolTipCombo.ShowAlways=true;
		}

		private void FillGridAssessments() {
			gridAssessments.BeginUpdate();
			gridAssessments.Columns.Clear();
			GridColumn col=new GridColumn("Date",70);
			gridAssessments.Columns.Add(col);
			col=new GridColumn("Type",170);
			gridAssessments.Columns.Add(col);
			col=new GridColumn("Description",170);
			gridAssessments.Columns.Add(col);
			col=new GridColumn("Documentation",170);
			gridAssessments.Columns.Add(col);
			gridAssessments.ListGridRows.Clear();
			GridRow row;
			Loinc loinc;
			Snomed snomed;
			List<EhrMeasureEvent> listEhrMeasureEvents=EhrMeasureEvents.RefreshByType(_patient.PatNum,EhrMeasureEventType.TobaccoUseAssessed);
			for(int i=0;i<listEhrMeasureEvents.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listEhrMeasureEvents[i].DateTEvent.ToShortDateString());
				loinc=Loincs.GetByCode(listEhrMeasureEvents[i].CodeValueEvent);//TobaccoUseAssessed events can be one of three types, all LOINC codes
				row.Cells.Add(loinc!=null?loinc.NameLongCommon:listEhrMeasureEvents[i].EventType.ToString());
				snomed=Snomeds.GetByCode(listEhrMeasureEvents[i].CodeValueResult);
				row.Cells.Add(snomed!=null?snomed.Description:"");
				row.Cells.Add(listEhrMeasureEvents[i].MoreInfo);
				row.Tag=listEhrMeasureEvents[i];
				gridAssessments.ListGridRows.Add(row);
			}
			gridAssessments.EndUpdate();
		}

		private void FillGridInterventions() {
			gridInterventions.BeginUpdate();
			gridInterventions.Columns.Clear();
			GridColumn col=new GridColumn("Date",70);
			gridInterventions.Columns.Add(col);
			col=new GridColumn("Type",150);
			gridInterventions.Columns.Add(col);
			col=new GridColumn("Description",160);
			gridInterventions.Columns.Add(col);
			col=new GridColumn("Declined",60);
			col.TextAlign=HorizontalAlignment.Center;
			gridInterventions.Columns.Add(col);
			col=new GridColumn("Documentation",140);
			gridInterventions.Columns.Add(col);
			gridInterventions.ListGridRows.Clear();
			//build list of rows of CessationInterventions and CessationMedications so we can order the list by date and type before filling the grid
			List<GridRow> listRows=new List<GridRow>();
			GridRow row;
			#region CessationInterventions
			Cpt cpt;
			Snomed snomed;
			RxNorm rxNorm;
			string type;
			string descript;
			List<Intervention> listInterventions=Interventions.Refresh(_patient.PatNum,InterventionCodeSet.TobaccoCessation);
			for(int i=0;i<listInterventions.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listInterventions[i].DateEntry.ToShortDateString());
				type=InterventionCodeSet.TobaccoCessation.ToString()+" Counseling";
				descript="";
				switch(listInterventions[i].CodeSystem) {
					case "CPT":
						cpt=Cpts.GetByCode(listInterventions[i].CodeValue);
						descript=cpt!=null?cpt.Description:"";
						break;
					case "SNOMEDCT":
						snomed=Snomeds.GetByCode(listInterventions[i].CodeValue);
						descript=snomed!=null?snomed.Description:"";
						break;
					case "RXNORM":
						//if the user checks the "Patient Declined" checkbox, we enter the tobacco cessation medication as an intervention that was declined
						type=InterventionCodeSet.TobaccoCessation.ToString()+" Medication";
						rxNorm=RxNorms.GetByRxCUI(listInterventions[i].CodeValue);
						descript=rxNorm!=null?rxNorm.Description:"";
						break;
				}
				row.Cells.Add(type);
				row.Cells.Add(descript);
				row.Cells.Add(listInterventions[i].IsPatDeclined?"X":"");
				row.Cells.Add(listInterventions[i].Note);
				row.Tag=listInterventions[i];
				listRows.Add(row);
			}
			#endregion
			#region CessationMedications
			List<string> listValueSetOIDsRxCuis=new List<string>();
			listValueSetOIDsRxCuis.Add("2.16.840.1.113883.3.526.3.1190");
			//Tobacco Use Cessation Pharmacotherapy Value Set
			string[] stringArrayRxCuis=EhrCodes.GetForValueSetOIDs(listValueSetOIDsRxCuis,true)
				.Select(x => x.CodeValue).ToArray();
			//arrayRxCuiStrings will contain 41 RxCui strings for tobacco cessation medications if those exist in the rxnorm table
			List<MedicationPat> listMedicationPats=MedicationPats.Refresh(_patient.PatNum,true).FindAll(x => stringArrayRxCuis.Contains(x.RxCui.ToString()));
			for(int i = 0;i<listMedicationPats.Count;i++) {
				row=new GridRow();
				List<string> listMedDates=new List<string>();
				if(listMedicationPats[i].DateStart.Year>1880) {
					listMedDates.Add(listMedicationPats[i].DateStart.ToShortDateString());
				}
				if(listMedicationPats[i].DateStop.Year>1880) {
					listMedDates.Add(listMedicationPats[i].DateStop.ToShortDateString());
				}
				if(listMedDates.Count==0) {
					listMedDates.Add(listMedicationPats[i].DateTStamp.ToShortDateString());
				}
				row.Cells.Add(listMedDates.Count==0?"":string.Join(" - ",listMedDates));
				row.Cells.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Medication");
				row.Cells.Add(RxNorms.GetDescByRxCui(listMedicationPats[i].RxCui.ToString()));
				row.Cells.Add(listMedicationPats[i].PatNote);
				row.Tag=listMedicationPats[i];
				listRows.Add(row);
			}
			#endregion
			List<GridRow> listRowsOrderByOldest=listRows.OrderBy(x => PIn.Date(x.Cells[0].Text))//rows ordered by date, oldest first
				.ThenBy(x => x.Cells[3].Text!="")
				//interventions at the top, declined med interventions below normal interventions
				.ThenBy(x => x.Tag.GetType().Name!="Intervention" || ((Intervention)x.Tag).CodeSystem=="RXNORM").ToList();
			for(int i=0;i<listRowsOrderByOldest.Count;i++) {
				gridInterventions.ListGridRows.Add(listRowsOrderByOldest[i]);//then add rows to gridInterventions
			}
			gridInterventions.EndUpdate();
		}

		private void gridAssessments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//we will allow them to change the DateTEvent, but not the status or more info box
			using FormEhrMeasureEventEdit formEhrMeasureEventEdit=new FormEhrMeasureEventEdit((EhrMeasureEvent)gridAssessments.ListGridRows[e.Row].Tag);
			formEhrMeasureEventEdit.ShowDialog();
			FillGridAssessments();
		}

		private void gridInterventions_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Object objCur=gridInterventions.ListGridRows[e.Row].Tag;
			//the intervention grid will be filled with Interventions and MedicationPats, load form accordingly
			if(objCur is Intervention) {
				using FormInterventionEdit formIntervetionEdit=new FormInterventionEdit();
				formIntervetionEdit.InterventionCur=(Intervention)objCur;
				formIntervetionEdit.IsAllTypes=false;
				formIntervetionEdit.IsSelectionMode=false;
				formIntervetionEdit.InterventionCur.IsNew=false;
				formIntervetionEdit.ShowDialog();
			}
			else if(objCur is MedicationPat) {
				using FormMedPat formMedPat=new FormMedPat();
				formMedPat.MedicationPatCur=(MedicationPat)objCur;
				formMedPat.IsNew=false;
				formMedPat.ShowDialog();
			}
			FillGridInterventions();
		}

		private void comboSmokeStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboSmokeStatus.SelectedIndex<1) {//If None or text set to other selected Snomed code so -1, do not create an event
				return;
			}
			//Insert measure event if one does not already exist for this date
			DateTime dateTEntered=PIn.DateT(textDateAssessed.Text);//will be set to DateTime.Now when form loads
			EhrMeasureEvent ehrMeasureEvent;
			for(int i=0;i<gridAssessments.ListGridRows.Count;i++) {
				ehrMeasureEvent=(EhrMeasureEvent)gridAssessments.ListGridRows[i].Tag;
				if(ehrMeasureEvent.DateTEvent.Date==dateTEntered.Date) {//one already exists for this date, don't auto insert event
					return;
				}
			}
			//no entry for the date entered, so insert one
			ehrMeasureEvent=new EhrMeasureEvent();
			ehrMeasureEvent.DateTEvent=dateTEntered;
			ehrMeasureEvent.EventType=EhrMeasureEventType.TobaccoUseAssessed;
			ehrMeasureEvent.PatNum=_patient.PatNum;
			ehrMeasureEvent.CodeValueEvent=_listEhrCodesAssessments[comboAssessmentType.SelectedIndex].CodeValue;
			ehrMeasureEvent.CodeSystemEvent=_listEhrCodesAssessments[comboAssessmentType.SelectedIndex].CodeSystem;
			//SelectedIndex guaranteed to be greater than 0
			ehrMeasureEvent.CodeValueResult=((SmokingSnoMed)comboSmokeStatus.SelectedIndex-1).ToString().Substring(1);
			ehrMeasureEvent.CodeSystemResult="SNOMEDCT";//only allow SNOMEDCT codes for now.
			ehrMeasureEvent.MoreInfo="";
			EhrMeasureEvents.Insert(ehrMeasureEvent);
			FillGridAssessments();
		}

		private void comboTobaccoStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboTobaccoStatus.SelectedIndex<_listEhrCodesTobaccoStatuses.Count) {//user selected a code in the list, just return
				return;
			}
			if(comboTobaccoStatus.SelectedIndex==_listEhrCodesTobaccoStatuses.Count
				&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Selecting a code that is not in the recommended list of codes may make "
					+"it more difficult to meet CQM's."))
			{
				comboTobaccoStatus.SelectedIndex=-1;
				return;
			}
			//user wants to select a custom status from the SNOMED list
			using FormSnomeds formSnomeds=new FormSnomeds();
			formSnomeds.IsSelectionMode=true;
			formSnomeds.ShowDialog();
			if(formSnomeds.DialogResult!=DialogResult.OK) {
				comboTobaccoStatus.SelectedIndex=-1;
				return;
			}
			if(!_listEhrCodesTobaccoStatuses.Any(x => x.CodeValue==formSnomeds.SnomedSelected.SnomedCode)) {
				EhrCode ehrCode=new EhrCode();
				ehrCode.CodeValue=formSnomeds.SnomedSelected.SnomedCode;
				ehrCode.Description=formSnomeds.SnomedSelected.Description;
				_listEhrCodesCustomTobacco.Add(ehrCode);
				_listEhrCodesCustomTobacco=_listEhrCodesCustomTobacco.OrderBy(x => x.Description).ToList();
				List<RadioButton> listRadioButtons=new List<RadioButton>();
				listRadioButtons.Add(radioUserStatuses);
				listRadioButtons.Add(radioNonUserStatuses);
				RadioButton radioButtonChecked=listRadioButtons.Where(x => x.Checked).DefaultIfEmpty(radioAllStatuses).FirstOrDefault();
				radioTobaccoStatuses_CheckedChanged(radioButtonChecked,new EventArgs());//refills drop down with newly added custom code
			}
			//selected code guaranteed to exist in the drop down at this point
			comboTobaccoStatus.Items.Clear();
			comboTobaccoStatus.Items.AddRange(_listEhrCodesTobaccoStatuses.Select(x => x.Description).ToArray());
			comboTobaccoStatus.Items.Add(Lan.g(this,"Choose from all SNOMED CT codes")+"...");
			comboTobaccoStatus.SelectedIndex=_listEhrCodesTobaccoStatuses.FindIndex(x => x.CodeValue==formSnomeds.SnomedSelected.SnomedCode);//add 1 for ...choose from
		}

		private void comboInterventionCode_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboInterventionCode.SelectedIndex>=0) {
				toolTipCombo.SetToolTip(comboInterventionCode,_listEhrCodesInterventions[comboInterventionCode.SelectedIndex].Description);
			}
		}

		///<summary>Fill comboInterventionCode with counseling and medication intervention codes using _listCounselInterventionCodes
		///and/or _listMedInterventionCodes depending on which radio button is selected.</summary>
		private void radioInterventions_CheckedChanged(object sender,EventArgs e) {
			RadioButton radioButton=(RadioButton)sender;
			if(!radioButton.Checked) {//if not checked, do nothing, caused by another radio button being checked
				return;
			}
			_listEhrCodesInterventions.Clear();
			if(radioButton.Name==radioRecentInterventions.Name) {
				_listEhrCodesInterventions.AddRange(_listEhrCodesRecentIntvs);
			}
			if(radioButton.Name.In(radioAllInterventions.Name,radioCounselInterventions.Name)) {
				_listEhrCodesInterventions.AddRange(_listEhrCodesCounselInterventions);
			}
			if(radioButton.Name.In(radioAllInterventions.Name,radioMedInterventions.Name)) {
				_listEhrCodesInterventions.AddRange(_listEhrCodesMedInterventions);
			}
			_listEhrCodesInterventions=_listEhrCodesInterventions.OrderBy(x => x.Description).ToList();
			comboInterventionCode.Items.Clear();
			//this is the max width of the description, minus the width of "..." and, if > 30 items in the list, the width of the vertical scroll bar
			int maxItemWidth=comboInterventionCode.DropDownWidth-(_listEhrCodesInterventions.Count>30?25:8);//8 for just "...", 25 for scroll bar plus "..."
			for(int i=0;i<_listEhrCodesInterventions.Count;i++) {
				if(TextRenderer.MeasureText(_listEhrCodesInterventions[i].Description,
					comboInterventionCode.Font).Width<comboInterventionCode.DropDownWidth-15
					|| _listEhrCodesInterventions[i].Description.Length<3)
				{
					comboInterventionCode.Items.Add(_listEhrCodesInterventions[i].Description);
					continue;
				}
				StringBuilder stringBuilderAbbrDesc=new StringBuilder();
				for(int c=0;c<_listEhrCodesInterventions[i].Description.Length;c++) {
					if(TextRenderer.MeasureText(stringBuilderAbbrDesc.ToString()+_listEhrCodesInterventions[i].Description[c],
						comboInterventionCode.Font).Width<maxItemWidth) 
					{
						stringBuilderAbbrDesc.Append(_listEhrCodesInterventions[i].Description[c]);
						continue;
					}
					comboInterventionCode.Items.Add(stringBuilderAbbrDesc.ToString()+"...");
					break;
				}
			}
		}

		///<summary>Fill comboTobaccoStatus with user and non-user tobacco status codes using _listUserCodes and/or _listNonUserCodes
		///depending on which radio button is selected.</summary>
		private void radioTobaccoStatuses_CheckedChanged(object sender,EventArgs e) {
			RadioButton radioButton=(RadioButton)sender;
			if(!radioButton.Checked) {
				return;
			}
			_listEhrCodesTobaccoStatuses.Clear();
			if(_listEhrCodesCustomTobacco.Count>0) {
				_listEhrCodesTobaccoStatuses.AddRange(_listEhrCodesCustomTobacco);
			}
			if(radioButton.Name==radioRecentStatuses.Name) {
				_listEhrCodesTobaccoStatuses.AddRange(_listEhrCodesRecentTobacco);
			}
			else {
				if(radioButton.Name.In(radioAllStatuses.Name,radioUserStatuses.Name)) {
					_listEhrCodesTobaccoStatuses.AddRange(_listEhrCodesUser);
				}
				if(radioButton.Name.In(radioAllStatuses.Name,radioNonUserStatuses.Name)) {
					_listEhrCodesTobaccoStatuses.AddRange(_listEhrCodesNonUser);
				}
			}
			_listEhrCodesTobaccoStatuses=_listEhrCodesTobaccoStatuses.OrderBy(x => x.Description).ToList();
			comboTobaccoStatus.Items.Clear();
			comboTobaccoStatus.Items.AddRange(_listEhrCodesTobaccoStatuses.Select(x => x.Description).ToArray());
			comboTobaccoStatus.Items.Add(Lan.g(this,"Choose from all SNOMED CT codes")+"...");
		}

		///<summary>If the LOINC table has not been imported, the Tobacco Use tab is disabled, but we want it to remain visible like the other EHR show
		///feature enabled tabs.  But since the combo boxes etc. cannot be filled without the LOINC table, don't allow selecting the tab.</summary>
		private void tabControlFormMedical_Selecting(object sender,int idxClicked) {
			if(tabControlFormMedical.SelectedTab.Enabled){
				return;
			}
			MsgBox.Show(this,"The codes used for Tobacco Use Screening assessments do not exist in the LOINC table in your database.  You must run the "
					+"Code System Importer tool in Setup | Chart | EHR to import this code set before accessing the Tobacco Use Tab.");
		}

		private void butAssessed_Click(object sender,EventArgs e) {
			if(comboTobaccoStatus.SelectedIndex<0 || comboTobaccoStatus.SelectedIndex>=_listEhrCodesTobaccoStatuses.Count) {
				MsgBox.Show(this,"You must select a tobacco status.");
				return;
			}
			DateTime dateTEntered=PIn.DateT(textDateAssessed.Text);
			EhrMeasureEvent ehrMeasureEvent=new EhrMeasureEvent();
			ehrMeasureEvent.DateTEvent=dateTEntered;
			ehrMeasureEvent.EventType=EhrMeasureEventType.TobaccoUseAssessed;
			ehrMeasureEvent.PatNum=_patient.PatNum;
			ehrMeasureEvent.CodeValueEvent=_listEhrCodesAssessments[comboAssessmentType.SelectedIndex].CodeValue;
			ehrMeasureEvent.CodeSystemEvent=_listEhrCodesAssessments[comboAssessmentType.SelectedIndex].CodeSystem;
			ehrMeasureEvent.CodeValueResult=_listEhrCodesTobaccoStatuses[comboTobaccoStatus.SelectedIndex].CodeValue;
			ehrMeasureEvent.CodeSystemResult="SNOMEDCT";//only allow SNOMEDCT codes for now.
			ehrMeasureEvent.MoreInfo="";
			EhrMeasureEvents.Insert(ehrMeasureEvent);
			comboTobaccoStatus.SelectedIndex=-1;
			FillGridAssessments();
		}

		private void butIntervention_Click(object sender,EventArgs e) {
			if(comboInterventionCode.SelectedIndex<0) {
				MsgBox.Show(this,"You must select an intervention code.");
				return;
			}
			EhrCode ehrCode=_listEhrCodesInterventions[comboInterventionCode.SelectedIndex];
			DateTime dateTEntry=PIn.Date(textDateIntervention.Text);
			if(ehrCode.CodeSystem=="RXNORM" && !checkPatientDeclined.Checked) {//if patient declines the medication, enter as a declined intervention
				//codeVal will be RxCui of medication, see if it already exists in Medication table
				Medication medication=Medications.GetMedicationFromDbByRxCui(PIn.Long(ehrCode.CodeValue));
				if(medication==null) {//no med with this RxCui, create one
					medication=new Medication();
					Medications.Insert(medication);//so that we will have the primary key
					medication.GenericNum=medication.MedicationNum;
					medication.RxCui=PIn.Long(ehrCode.CodeValue);
					medication.MedName=RxNorms.GetDescByRxCui(ehrCode.CodeValue);
					Medications.Update(medication);
					Medications.RefreshCache();//refresh cache to include new medication
				}
				MedicationPat medicationPat=new MedicationPat();
				medicationPat.PatNum=_patient.PatNum;
				medicationPat.ProvNum=_patient.PriProv;
				medicationPat.MedicationNum=medication.MedicationNum;
				medicationPat.RxCui=medication.RxCui;
				medicationPat.DateStart=dateTEntry;
				using FormMedPat formMedPat=new FormMedPat();
				formMedPat.MedicationPatCur=medicationPat;
				formMedPat.IsNew=true;
				formMedPat.ShowDialog();
				if(formMedPat.DialogResult!=DialogResult.OK) {
					return;
				}
				if(formMedPat.MedicationPatCur.DateStart.Date<dateTEntry.AddMonths(-6).Date || formMedPat.MedicationPatCur.DateStart.Date>dateTEntry.Date) {
					MsgBox.Show(this,"The medication order just entered is not within the 6 months prior to the date of this intervention.  You can modify the "
						+"date of the medication order in the patient's medical history section.");
				}
			}
			else {
				Intervention intervention=new Intervention();
				intervention.PatNum=_patient.PatNum;
				intervention.ProvNum=_patient.PriProv;
				intervention.DateEntry=dateTEntry;
				intervention.CodeValue=ehrCode.CodeValue;
				intervention.CodeSystem=ehrCode.CodeSystem;
				intervention.CodeSet=InterventionCodeSet.TobaccoCessation;
				intervention.IsPatDeclined=checkPatientDeclined.Checked;
				Interventions.Insert(intervention);
			}
			comboInterventionCode.SelectedIndex=-1;
			FillGridInterventions();
		}
		#endregion Tobacco Use Tab

		private void FormMedical_ResizeEnd(object sender,EventArgs e) {
			FillMeds();
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(comboSmokeStatus.SelectedIndex==0) {//None
				_patient.SmokingSnoMed="";
			}
			else {
				_patient.SmokingSnoMed=((SmokingSnoMed)comboSmokeStatus.SelectedIndex-1).ToString().Substring(1);
			}
			_patient.Premed=checkPremed.Checked;
			_patient.MedUrgNote=textMedUrgNote.Text;
			Patients.Update(_patient,_patOld);
			_patientNote.Medical=textMedical.Text;
			_patientNote.Service=textService.Text;
			_patientNote.MedicalComp=textMedicalComp.Text;
			PatientNotes.Update(_patientNote, _patient.Guarantor);
			//Insert an ehrmeasureevent for CurrentMedsDocumented if user selected Yes and there isn't one with today's date
			if(radioMedsDocumentedYes.Checked && _EhrMeasureEventNum==0) {
				EhrMeasureEvent ehrMeasureEvent=new EhrMeasureEvent();
				ehrMeasureEvent.PatNum=_patient.PatNum;
				ehrMeasureEvent.DateTEvent=DateTime.Now;
				ehrMeasureEvent.EventType=EhrMeasureEventType.CurrentMedsDocumented;
				ehrMeasureEvent.CodeValueEvent="428191000124101";//SNOMEDCT code for document current meds procedure
				ehrMeasureEvent.CodeSystemEvent="SNOMEDCT";
				EhrMeasureEvents.Insert(ehrMeasureEvent);
			}
			//No is selected, if no EhrNotPerformed item for current meds documented, launch not performed edit window to allow user to select valid reason.
			if(radioMedsDocumentedNo.Checked) {
				if(_EhrNotPerfNum==0) {
					using FormEhrNotPerformedEdit formEhrNotPerformedEdit=new FormEhrNotPerformedEdit();
					formEhrNotPerformedEdit.EhrNotPerfCur=new EhrNotPerformed();
					formEhrNotPerformedEdit.EhrNotPerfCur.IsNew=true;
					formEhrNotPerformedEdit.EhrNotPerfCur.PatNum=_patient.PatNum;
					formEhrNotPerformedEdit.EhrNotPerfCur.ProvNum=_patient.PriProv;
					formEhrNotPerformedEdit.SelectedItemIndex=(int)EhrNotPerformedItem.DocumentCurrentMeds;
					formEhrNotPerformedEdit.EhrNotPerfCur.DateEntry=DateTime.Today;
					formEhrNotPerformedEdit.IsDateReadOnly=true;
					formEhrNotPerformedEdit.ShowDialog();
					if(formEhrNotPerformedEdit.DialogResult==DialogResult.OK) {//if they just inserted a not performed item, set the private class-wide variable for the next if statement
						_EhrNotPerfNum=formEhrNotPerformedEdit.EhrNotPerfCur.EhrNotPerformedNum;
					}
				}
				if(_EhrNotPerfNum>0 && _EhrMeasureEventNum>0) {//if not performed item is entered with today's date, delete existing performed item
					EhrMeasureEvents.Delete(_EhrMeasureEventNum);
				}
			}
			DialogResult=DialogResult.OK;
		}

	}
}