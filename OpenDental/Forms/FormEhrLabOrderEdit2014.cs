using System;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;
using EhrLaboratories;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormEhrLabOrderEdit2014:FormODBase {
		//long PatCurNum;//this was always just defaulted to 0, but referenced from whithin this file and used in case EhrLabCur.PatNum==0
		public EhrLab EhrLabCur;
		public bool IsNew;
		public bool IsImport;
		public bool IsViewOnly;

		public FormEhrLabOrderEdit2014() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormLabPanelEdit_Load(object sender,EventArgs e) {
			Height=System.Windows.Forms.Screen.GetWorkingArea(this).Height;
			this.SetDesktopLocation(DesktopLocation.X,0);
			if(IsNew) {
				checkAutoID.Checked=true;
				checkAutoID.Visible=true;
			}
			if(IsImport || IsViewOnly) {
				foreach(Control c in this.Controls) {
					c.Enabled=false;
				}
				butViewParent.Enabled=true;
				gridMain.Enabled=true;
				gridNotes.Enabled=true;
				gridSpecimen.Enabled=true;
				butCancel.Text="Close";
				butCancel.Enabled=true;
				//butAddNote.Enabled=false;
				//butAddCopyTo.Enabled=false;
				//butAddClinicalInfo.Enabled=false;
				//butAdd.Enabled=false;
				//butAddSpecimens.Enabled=false;
				//butPatientPick.Enabled=false;
				//textName.Enabled=false;
				//butProvPicker.Enabled=false;
				//butServicePicker.Enabled=false;
				//butParentPicker.Enabled=false;
				//butDelete.Enabled=false;
				////combos
				//comboOrderingProvIdType.Enabled=false;
				//comboOrderingProvNameType.Enabled=false;
				//comboResultStatus.Enabled=false;
				//comboSpecimenActionCode.Enabled=false;
				//TODO:hide the rest of the controls that shouldn't show if importing or view only.
			}
			//Parent Result
			if(EhrLabCur.ParentPlacerOrderNum!="") {
				textParentOrderNum.Text=EhrLabCur.ParentPlacerOrderNum;
			}
			else if(EhrLabCur.ParentFillerOrderNum!="") {
				textParentOrderNum.Text=EhrLabCur.ParentFillerOrderNum;
			}
			FillOrderNums();
			FillProvInfo();
			//Results Handling
			checkResultsHandlingF.Checked=EhrLabCur.ListEhrLabResultsHandlingF;
			checkResultsHandlingN.Checked=EhrLabCur.ListEhrLabResultsHandlingN;
			//UpdateTime
			textResultDateTime.Text=EhrLab.formatDateFromHL7(EhrLabCur.ResultDateTime);
			FillUsi();//Service Identifier
			FillGrid();//LabResults
			textObservationDateTimeStart.Text=EhrLab.formatDateFromHL7(EhrLabCur.ObservationDateTimeStart);
			textObservationDateTimeEnd.Text=EhrLab.formatDateFromHL7(EhrLabCur.ObservationDateTimeEnd);
			//TQ1
			textTQ1Start.Text=EhrLab.formatDateFromHL7(EhrLabCur.TQ1DateTimeStart);
			textTQ1Stop.Text=EhrLab.formatDateFromHL7(EhrLabCur.TQ1DateTimeEnd);
			FillGridNotes();
			FillGridResultsCopyTo();
			FillGridClinicalInformation();
			FillGridSpecimen();
			//Specimen Action Code
			FillComboSpecimenActionCode();
			//Result Status
			FillComboResultStatus();
			//Only allow image editing on existing labs
			if(EhrLabCur==null || EhrLabCur.EhrLabNum<=0 || EhrLabCur.PatNum <=0) {
				butManageImages.Enabled=false;
				labelManageImages.Visible=true;
			}
			else {
				butManageImages.Enabled=true;
				labelManageImages.Visible=false;
			}
		}

		private void FillComboSpecimenActionCode() {
			comboSpecimenActionCode.Items.Clear();
			comboSpecimenActionCode.BeginUpdate();
			List<string> listSpecActionCodes=EhrLabs.GetHL70065Descriptions();
			comboSpecimenActionCode.Items.AddRange(listSpecActionCodes.ToArray());
			comboSpecimenActionCode.EndUpdate();
			comboSpecimenActionCode.SelectedIndex=(int)Enum.Parse(typeof(HL70065),EhrLabCur.SpecimenActionCode.ToString(),true)+1;
		}

		private void FillComboResultStatus() {
			comboResultStatus.Items.Clear();
			comboResultStatus.BeginUpdate();
			List<string> listResStatCodes=EhrLabs.GetHL70123Descriptions();
			comboResultStatus.Items.AddRange(listResStatCodes.ToArray());
			comboResultStatus.EndUpdate();
			comboResultStatus.SelectedIndex=(int)Enum.Parse(typeof(HL70123),EhrLabCur.ResultStatus.ToString(),true)+1;
		}

		private void FillUsi() {
			textUsiID.Text=EhrLabCur.UsiID;
			textUsiText.Text=EhrLabCur.UsiText;
			textUsiCodeSystemName.Text=EhrLabCur.UsiCodeSystemName;
			textUsiIDAlt.Text=EhrLabCur.UsiIDAlt;
			textUsiTextAlt.Text=EhrLabCur.UsiTextAlt;
			textUsiCodeSystemNameAlt.Text=EhrLabCur.UsiCodeSystemNameAlt;
			textUsiTextOriginal.Text=EhrLabCur.UsiTextOriginal;
		}

		private void FillProvInfo() {
			textOrderingProvIdentifier.Text=EhrLabCur.OrderingProviderID;
			textOrderingProvLastName.Text=EhrLabCur.OrderingProviderLName;
			textOrderingProvFirstName.Text=EhrLabCur.OrderingProviderFName;
			textOrderingProvMiddleName.Text=EhrLabCur.OrderingProviderMiddleNames;
			textOrderingProvSuffix.Text=EhrLabCur.OrderingProviderSuffix;
			textOrderingProvPrefix.Text=EhrLabCur.OrderingProviderPrefix;
			textOrderingProvAANID.Text=EhrLabCur.OrderingProviderAssigningAuthorityNamespaceID;
			textOrderingProvAAUID.Text=EhrLabCur.OrderingProviderAssigningAuthorityUniversalID;
			textOrderingProvAAUIDType.Text=EhrLabCur.OrderingProviderAssigningAuthorityIDType;
			#region Name Type
			comboOrderingProvNameType.Items.Clear();
			comboOrderingProvNameType.BeginUpdate();
			//Fill medical director name combo with HL70200 enum.  Not sure if blank is acceptable.
			List<string> listOrderingProvNameType=EhrLabResults.GetHL70200Descriptions();
			comboOrderingProvNameType.Items.AddRange(listOrderingProvNameType.ToArray());
			comboOrderingProvNameType.EndUpdate();
			comboOrderingProvNameType.SelectedIndex=(int)Enum.Parse(typeof(HL70200),EhrLabCur.OrderingProviderNameTypeCode.ToString(),true)+1;
			#endregion
			#region Identifier Type
			comboOrderingProvIdType.Items.Clear();
			comboOrderingProvIdType.BeginUpdate();
			//Fill medical director type combo with HL70203 enum.  Not sure if blank is acceptable.
			List<string> listOrderingProvIdType=EhrLabs.GetHL70203Descriptions();
			comboOrderingProvIdType.Items.AddRange(listOrderingProvIdType.ToArray());
			comboOrderingProvIdType.EndUpdate();
			comboOrderingProvIdType.SelectedIndex=(int)Enum.Parse(typeof(HL70203),EhrLabCur.OrderingProviderIdentifierTypeCode.ToString(),true)+1;
			#endregion
		}

		private void FillOrderNums() {
			//Placer Order Num
			textPlacerOrderNum.Text=EhrLabCur.PlacerOrderNum;
			textPlacerOrderNamespace.Text=EhrLabCur.PlacerOrderNamespace;
			textPlacerOrderUniversalID.Text=EhrLabCur.PlacerOrderUniversalID;
			textPlacerOrderUniversalIDType.Text=EhrLabCur.PlacerOrderUniversalIDType;
			//Placer Order Group Num
			textPlacerGroupNum.Text=EhrLabCur.PlacerGroupNum;
			textPlacerGroupNamespace.Text=EhrLabCur.PlacerGroupNamespace;
			textPlacerGroupUniversalID.Text=EhrLabCur.PlacerGroupUniversalID;
			textPlacerGroupUniversalIDType.Text=EhrLabCur.PlacerGroupUniversalIDType;
			//Filler Order Num
			textFillerOrderNum.Text=EhrLabCur.FillerOrderNum;
			textFillerOrderNamespace.Text=EhrLabCur.FillerOrderNamespace;
			textFillerOrderUniversalID.Text=EhrLabCur.FillerOrderUniversalID;
			textFillerOrderUniversalIDType.Text=EhrLabCur.FillerOrderUniversalIDType;
			return;
		}

		private void checkAutoID_CheckedChanged(object sender,EventArgs e) {
			textPlacerOrderNum.Enabled=!checkAutoID.Checked;
			textPlacerOrderUniversalID.Enabled=!checkAutoID.Checked;
			textPlacerOrderUniversalIDType.Enabled=!checkAutoID.Checked;
			textPlacerOrderNamespace.Enabled=!checkAutoID.Checked;
		}
	
		///<summary>Lab Results</summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				col=new GridColumn("",18);//infoButton
				col.ImageList=imageListInfoButton;
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn("Test Date",70);
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("LOINC",60);//LoincCode
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Test Performed",230);//ShortDescription
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Result Value",160);//Complicated
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Units",60);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Flags",40);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<EhrLabCur.ListEhrLabResults.Count;i++) {
				row=new GridRow();
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
					row.Cells.Add("0");//index of infobutton
				}
				if(EhrLabCur.ListEhrLabResults[i].ObservationDateTime==null || EhrLabCur.ListEhrLabResults[i].ObservationDateTime=="") {
					row.Cells.Add("");//null date
				}
				else {
					string dateSt=EhrLabCur.ListEhrLabResults[i].ObservationDateTime.Substring(0,8);//stored in DB as yyyyMMdd[hh[mm[ss]]], []==optional components
					DateTime dateT=PIn.Date(dateSt.Substring(4,2)+"/"+dateSt.Substring(6,2)+"/"+dateSt.Substring(0,4));
					row.Cells.Add(dateT.ToShortDateString());//date only
				}
				if(EhrLabCur.ListEhrLabResults[i].ObservationIdentifierID!="") {
					row.Cells.Add(EhrLabCur.ListEhrLabResults[i].ObservationIdentifierID);
					row.Cells.Add(EhrLabCur.ListEhrLabResults[i].ObservationIdentifierText);
				}
				else if(EhrLabCur.ListEhrLabResults[i].ObservationIdentifierIDAlt!="") {
					row.Cells.Add(EhrLabCur.ListEhrLabResults[i].ObservationIdentifierIDAlt);
					row.Cells.Add(EhrLabCur.ListEhrLabResults[i].ObservationIdentifierTextAlt);
				}
				else {
					row.Cells.Add("UNK");
					row.Cells.Add("Unknown, could not find valid test code.");
				}
				switch(EhrLabCur.ListEhrLabResults[i].ValueType) {
					case HL70125.CE:
					case HL70125.CWE:
						row.Cells.Add(EhrLabCur.ListEhrLabResults[i].ObservationValueCodedElementText);
						break;
					case HL70125.DT:
					case HL70125.TS:
						row.Cells.Add(EhrLabCur.ListEhrLabResults[i].ObservationValueDateTime);
						break;
					case HL70125.TM:
						row.Cells.Add(EhrLabCur.ListEhrLabResults[i].ObservationValueTime.ToString());
						break;
					case HL70125.NM:
						row.Cells.Add(EhrLabCur.ListEhrLabResults[i].ObservationValueNumeric.ToString());
						break;
					case HL70125.SN:
						row.Cells.Add(
							EhrLabCur.ListEhrLabResults[i].ObservationValueComparator
							+EhrLabCur.ListEhrLabResults[i].ObservationValueNumber1
							+(EhrLabCur.ListEhrLabResults[i].ObservationValueSeparatorOrSuffix==""
									?"":EhrLabCur.ListEhrLabResults[i].ObservationValueSeparatorOrSuffix+EhrLabCur.ListEhrLabResults[i].ObservationValueNumber2)
							);
						break;
					case HL70125.FT:
					case HL70125.ST:
					case HL70125.TX:
						row.Cells.Add(EhrLabCur.ListEhrLabResults[i].ObservationValueText);
						break;
				}
				row.Cells.Add(EhrLabCur.ListEhrLabResults[i].UnitsID);
				row.Cells.Add(EhrLabCur.ListEhrLabResults[i].AbnormalFlags.Replace("N",""));//abnormal flags, show blank if flag is "Normal"
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		///<summary>Lab Result Notes. Currently includes notes for results too... TODO: seperate notes for labs and results.</summary>
		private void FillGridNotes() {
			gridNotes.BeginUpdate();
			gridNotes.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Note Num",60);
			gridNotes.ListGridColumns.Add(col);
			col=new GridColumn("Comments",300);
			gridNotes.ListGridColumns.Add(col);
			gridNotes.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<EhrLabCur.ListEhrLabNotes.Count;i++) {
				for(int j=0;j<EhrLabCur.ListEhrLabNotes[i].Comments.Split('^').Length;j++) {
					row=new GridRow();
					row.Cells.Add((j==0?(i+1).ToString():""));//add note number if this is first comment for the note, otherwise add blank cell.
					row.Cells.Add(EhrLabCur.ListEhrLabNotes[i].Comments.Split('^')[j]);//Add each comment.
					gridNotes.ListGridRows.Add(row);
				}
			}
			gridNotes.EndUpdate();
		}

		///<summary></summary>
		private void FillGridResultsCopyTo() {
			gridResultsCopyTo.BeginUpdate();
			gridResultsCopyTo.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Name",60);
			gridResultsCopyTo.ListGridColumns.Add(col);
			gridResultsCopyTo.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<EhrLabCur.ListEhrLabResultsCopyTo.Count;i++) {
				row=new GridRow();
				row.Cells.Add(EhrLabCur.ListEhrLabResultsCopyTo[i].CopyToPrefix+" "
					+EhrLabCur.ListEhrLabResultsCopyTo[i].CopyToFName+" "
					+EhrLabCur.ListEhrLabResultsCopyTo[i].CopyToLName+" "
					+EhrLabCur.ListEhrLabResultsCopyTo[i].CopyToSuffix);
				//TODO: Make this neater. Will display extra spaces if missing prefix suffix or middle names.
				gridResultsCopyTo.ListGridRows.Add(row);
			}
			gridResultsCopyTo.EndUpdate();
		}

		private void FillGridClinicalInformation() {
			gridClinicalInformation.BeginUpdate();
			gridClinicalInformation.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",60);//arbitrary width, only column in grid.
			gridClinicalInformation.ListGridColumns.Add(col);
			gridClinicalInformation.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<EhrLabCur.ListRelevantClinicalInformations.Count;i++) {
				row=new GridRow();
				row.Cells.Add(EhrLabCur.ListRelevantClinicalInformations[i].ClinicalInfoText);//may be blank, if so, check the "alt" text
				gridClinicalInformation.ListGridRows.Add(row);
			}
			gridClinicalInformation.EndUpdate();
		}

		///<summary>Lab Results</summary>
		private void FillGridSpecimen() {
			gridSpecimen.BeginUpdate();
			gridSpecimen.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Rej",30,HorizontalAlignment.Center);//arbitrary width, only column in grid.
			gridSpecimen.ListGridColumns.Add(col);
			col=new GridColumn("Specimen Type",60);//arbitrary width, only column in grid.
			gridSpecimen.ListGridColumns.Add(col);
			gridSpecimen.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<EhrLabCur.ListEhrLabSpecimens.Count;i++) {
				row=new GridRow();
				row.Cells.Add((EhrLabCur.ListEhrLabSpecimens[i].ListEhrLabSpecimenRejectReason.Count==0?"":"X"));//X is specimen rejected.
				row.Cells.Add(EhrLabCur.ListEhrLabSpecimens[i].SpecimenTypeText);//may be blank, if so, check the "alt" text
				gridSpecimen.ListGridRows.Add(row);
			}
			gridSpecimen.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEhrLabResultEdit2014 FormLRE=new FormEhrLabResultEdit2014();
			FormLRE.EhrLabResultCur=EhrLabCur.ListEhrLabResults[e.Row];
			FormLRE.IsImport=IsImport;
			FormLRE.IsViewOnly=IsViewOnly;
			FormLRE.ShowDialog();
			if(IsImport || IsViewOnly || FormLRE.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrLabCur.ListEhrLabResults[e.Row]=FormLRE.EhrLabResultCur;
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormEhrLabResultEdit2014 FormLRE=new FormEhrLabResultEdit2014();
			FormLRE.EhrLabResultCur=new EhrLabResult();
			FormLRE.EhrLabResultCur.IsNew=true;
			FormLRE.ShowDialog();
			if(FormLRE.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrLabCur.ListEhrLabResults.Add(FormLRE.EhrLabResultCur);
			FillGrid();
		}

		private void butAddNote_Click(object sender,EventArgs e) {
			using FormEhrLabNoteEdit FormLNE=new FormEhrLabNoteEdit();
			FormLNE.LabNoteCur=new EhrLabNote();
			FormLNE.ShowDialog();
			if(FormLNE.DialogResult!=DialogResult.OK) {
				return;
			}
			FormLNE.LabNoteCur.EhrLabNum=EhrLabCur.EhrLabNum;
			EhrLabCur.ListEhrLabNotes.Add(FormLNE.LabNoteCur);
			FillGridNotes();
		}

		private void butManageImages_Click(object sender,EventArgs e) {
			//EhrLabCur was verifed as valid on the form's load event. No need for validation here.
			using FormEhrLabImageEdit formEhrLabImageEdit=new FormEhrLabImageEdit(EhrLabCur.PatNum,EhrLabCur.EhrLabNum);
			formEhrLabImageEdit.ShowDialog();
		}

		private void gridNotes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEhrLabNoteEdit FormLNE=new FormEhrLabNoteEdit();
			FormLNE.IsViewOnly=IsViewOnly;
			FormLNE.IsImport=IsImport;
			FormLNE.LabNoteCur=EhrLabCur.ListEhrLabNotes[e.Row];
			FormLNE.ShowDialog();
			if(FormLNE.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrLabCur.ListEhrLabNotes[e.Row]=FormLNE.LabNoteCur;
			FillGridNotes();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will delete the entire lab order and all attached lab results. This cannot be undone. Would you like to continue?")) {
				return;
			}
			EhrLabs.Delete(EhrLabCur.EhrLabNum);
			DialogResult=DialogResult.OK;
		}

		private void butViewParent_Click(object sender,EventArgs e) {
			EhrLab ehrLabParent=null;
			ehrLabParent=EhrLabs.GetByGUID(EhrLabCur.ParentPlacerOrderUniversalID,EhrLabCur.ParentPlacerOrderNum);
			if(ehrLabParent==null) {
				ehrLabParent=EhrLabs.GetByGUID(EhrLabCur.ParentFillerOrderUniversalID,EhrLabCur.ParentFillerOrderNum);
			}
			if(ehrLabParent==null) {
				return;
			}
			using FormEhrLabOrderEdit2014 FormELOE=new FormEhrLabOrderEdit2014();
			FormELOE.EhrLabCur=ehrLabParent;
			FormELOE.IsViewOnly=true;
			FormELOE.Text=Lan.g(this,"Parent Lab Order - READ ONLY");
			FormELOE.ShowDialog();
		}

		///<summary></summary>
		private bool EntriesAreValid() {
			StringBuilder errorMessage=new StringBuilder();
			//Order Numbers
			if(checkAutoID.Checked) {
				if(OIDInternals.GetForType(IdentifierType.LabOrder).IDRoot=="") {
					errorMessage.AppendLine("  OID registry must be configured in order to use Automatic Lab Order IDs.");
				}
				//don't validate order numbers... it will be automatically generated when saved.
			}
			else if((textPlacerOrderNum.Text=="" || (textPlacerOrderNum.Text!="" && textPlacerOrderUniversalID.Text==""))//Blank placerOrderNum OR OrderNum w/ blank OID
				&& (textFillerOrderNum.Text=="" || (textFillerOrderNum.Text!="" && textFillerOrderUniversalID.Text==""))) //Blank fillerOrderNum OR OrderNum w/blank OID
			{
				errorMessage.AppendLine("  Order must have valid placer or filler order number with universal ID.");
			}
			//Prov Numbers
			if(textOrderingProvAAUID.Text==OIDInternals.GetForType(IdentifierType.Provider).IDRoot) {
				Provider prov=null;
				try {
					prov=Providers.GetProv(PIn.Long(textOrderingProvIdentifier.Text));
				}
				catch { }
				if(prov==null) {
					errorMessage.AppendLine("  Ordering provider identifier or assigning authority is invalid.");
				}
			}
			else {
				if(!Regex.IsMatch(textOrderingProvIdentifier.Text,@"^(\d|\w)+$")){
					errorMessage.AppendLine("  Ordering Provider identifier must only contain numbers and letters.");
				}
			}
			//TODO: validate the controls
			if(errorMessage.ToString()!="") {
				errorMessage.Insert(0,"Unable to save current Lab Order for the following reasons:\r\n");
				MessageBox.Show(this,errorMessage.ToString());
				return false;
			}
			return true;
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(!CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				return;
			}
			if(e.Col!=0) {
				return;
			}
			List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(EhrLabCur.ListEhrLabResults[e.Row]);
			using FormInfobutton FormIB=new FormInfobutton(listKnowledgeRequests);
			//if(PatCurNum>0) {
			//	FormIB.PatCur=Patients.GetPat(PatCurNum);
			//}
			FormIB.ShowDialog();
		}

		private void gridSpecimen_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEhrLabSpecimenEdit FormLSE=new FormEhrLabSpecimenEdit();
			FormLSE.EhrLabSpecimenCur=EhrLabCur.ListEhrLabSpecimens[e.Row];
			FormLSE.ShowDialog();
		}

		private void butProvPicker_Click(object sender,EventArgs e) {
			using FormProviderPick FormPP=new FormProviderPick();
			FormPP.ShowDialog();
			if(FormPP.DialogResult!=DialogResult.OK) {
				return;
			}
			Provider prov=Providers.GetProv(FormPP.SelectedProvNum);
			if(prov.NationalProvID!="") {
				textOrderingProvIdentifier.Text=prov.NationalProvID;
				comboOrderingProvIdType.SelectedIndex=(int)HL70203.NPI+1;
				textOrderingProvAANID.Text="";
				textOrderingProvAAUID.Text="2.16.840.1.113883.4.6";//NPI OID
				textOrderingProvAAUIDType.Text="ISO";
			}
			else {
				textOrderingProvIdentifier.Text=prov.ProvNum.ToString();
				comboOrderingProvIdType.SelectedIndex=(int)HL70203.PRN+1;
				textOrderingProvAANID.Text="";
				textOrderingProvAAUID.Text=OIDInternals.GetForType(IdentifierType.Provider).IDRoot;//Internal OID
				textOrderingProvAAUIDType.Text="ISO";
			}
			comboOrderingProvNameType.SelectedIndex=(int)HL70200.L+1;
			textOrderingProvFirstName.Text=prov.FName;
			textOrderingProvLastName.Text=prov.LName;
			textOrderingProvMiddleName.Text=prov.MI;
		}

		private void butServicePicker_Click(object sender,EventArgs e) {
			using FormLoincs FormL=new FormLoincs();
			FormL.IsSelectionMode=true;
			FormL.ShowDialog();
			if(FormL.DialogResult!=DialogResult.OK) {
				return;
			}
			textUsiID.Text=FormL.SelectedLoinc.LoincCode;
			textUsiCodeSystemName.Text="LN";
			textUsiText.Text=FormL.SelectedLoinc.NameShort;
			textUsiTextOriginal.Text=FormL.SelectedLoinc.NameLongCommon;
		}

		private void butLastUpdate_Click(object sender,EventArgs e) {

		}

		private void butOk_Click(object sender,EventArgs e) {
			if(IsImport || IsViewOnly) {
				DialogResult=DialogResult.OK;
				return;
			}
			if(!EntriesAreValid()) {
				return;
			} 
			Provider prov=Providers.GetProv(Security.CurUser.ProvNum);
			if(Security.CurUser.ProvNum!=0 && EhrProvKeys.GetKeysByFLName(prov.LName,prov.FName).Count>0) {//The user who is currently logged in is a provider and has a valid EHR key.
				EhrLabCur.IsCpoe=true;
			}
			//if(EhrLabCur.PatNum==0) {
			//	EhrLabCur.PatNum=PatCurNum;
			//}
			//EhrLabCur.OrderControlCode=((HL70119)comb);//TODO:UI and this value.
			if(checkAutoID.Checked) {
				EhrLabCur.PlacerOrderNum=EhrLabs.GetNextOrderNum().ToString();
				EhrLabCur.PlacerOrderNamespace="";
				EhrLabCur.PlacerOrderUniversalID=OIDInternals.GetForType(IdentifierType.LabOrder).IDRoot;
				EhrLabCur.PlacerOrderUniversalIDType="ISO";
			}
			else{
				EhrLabCur.PlacerOrderNum=textPlacerOrderNum.Text;
				EhrLabCur.PlacerOrderNamespace=textPlacerOrderNamespace.Text;
				EhrLabCur.PlacerOrderUniversalID=textPlacerOrderUniversalID.Text;
				EhrLabCur.PlacerOrderUniversalIDType=textPlacerOrderUniversalIDType.Text;
			}
			EhrLabCur.FillerOrderNum=textFillerOrderNum.Text;
			EhrLabCur.FillerOrderNamespace=textFillerOrderNamespace.Text;
			EhrLabCur.FillerOrderUniversalID=textFillerOrderUniversalID.Text;
			EhrLabCur.FillerOrderUniversalIDType=textFillerOrderUniversalIDType.Text;
			EhrLabCur.PlacerGroupNum=textPlacerGroupNum.Text;
			EhrLabCur.PlacerGroupNamespace=textPlacerGroupNamespace.Text;
			EhrLabCur.PlacerGroupUniversalID=textPlacerGroupUniversalID.Text;
			EhrLabCur.PlacerGroupUniversalIDType=textPlacerGroupUniversalIDType.Text;
			EhrLabCur.OrderingProviderID=textOrderingProvIdentifier.Text;
			EhrLabCur.OrderingProviderLName=textOrderingProvLastName.Text;
			EhrLabCur.OrderingProviderFName=textOrderingProvFirstName.Text;
			EhrLabCur.OrderingProviderMiddleNames=textOrderingProvMiddleName.Text;
			EhrLabCur.OrderingProviderSuffix=textOrderingProvSuffix.Text;
			EhrLabCur.OrderingProviderPrefix=textOrderingProvPrefix.Text;
			EhrLabCur.OrderingProviderAssigningAuthorityNamespaceID=textOrderingProvAANID.Text;
			EhrLabCur.OrderingProviderAssigningAuthorityUniversalID=textOrderingProvAAUID.Text;
			EhrLabCur.OrderingProviderAssigningAuthorityIDType=textOrderingProvAAUIDType.Text;
			EhrLabCur.OrderingProviderNameTypeCode=((HL70200)comboOrderingProvNameType.SelectedIndex-1);
			EhrLabCur.OrderingProviderIdentifierTypeCode=((HL70203)comboOrderingProvIdType.SelectedIndex-1);
			//EhrLabCur.SetIdOBR=PIn.Long("");//TODO: UI and Save
			EhrLabCur.UsiID=textUsiID.Text;
			EhrLabCur.UsiText=textUsiText.Text;
			EhrLabCur.UsiCodeSystemName=textUsiCodeSystemName.Text;
			EhrLabCur.UsiIDAlt=textUsiIDAlt.Text;
			EhrLabCur.UsiTextAlt=textUsiTextAlt.Text;
			EhrLabCur.UsiCodeSystemNameAlt=textUsiCodeSystemNameAlt.Text;
			EhrLabCur.UsiTextOriginal=textUsiTextOriginal.Text;
			EhrLabCur.ObservationDateTimeStart=EhrLab.formatDateToHL7(textObservationDateTimeStart.Text.Trim());
			EhrLabCur.ObservationDateTimeEnd=EhrLab.formatDateToHL7(textObservationDateTimeEnd.Text.Trim());
			EhrLabCur.SpecimenActionCode=((HL70065)comboSpecimenActionCode.SelectedIndex-1);
			EhrLabCur.ResultDateTime=EhrLab.formatDateToHL7(textResultDateTime.Text.Trim());//upper right hand corner of form.
			EhrLabCur.ResultStatus=((HL70123)comboResultStatus.SelectedIndex-1);
			//TODO: parent result.
			/*
			EhrLabCur.ParentObservationID=
			EhrLabCur.ParentObservationText=
			EhrLabCur.ParentObservationCodeSystemName=
			EhrLabCur.ParentObservationIDAlt=
			EhrLabCur.ParentObservationTextAlt=
			EhrLabCur.ParentObservationCodeSystemNameAlt=
			EhrLabCur.ParentObservationTextOriginal=
			EhrLabCur.ParentObservationSubID=
			EhrLabCur.ParentPlacerOrderNum=
			EhrLabCur.ParentPlacerOrderNamespace=
			EhrLabCur.ParentPlacerOrderUniversalID=
			EhrLabCur.ParentPlacerOrderUniversalIDType=
			EhrLabCur.ParentFillerOrderNum=
			EhrLabCur.ParentFillerOrderNamespace=
			EhrLabCur.ParentFillerOrderUniversalID=
			EhrLabCur.ParentFillerOrderUniversalIDType=
			*/
			EhrLabCur.ListEhrLabResultsHandlingF=checkResultsHandlingF.Checked;
			EhrLabCur.ListEhrLabResultsHandlingN=checkResultsHandlingN.Checked;
			//EhrLabCur.TQ1SetId=//TODO:this
			EhrLabCur.TQ1DateTimeStart=EhrLab.formatDateToHL7(textTQ1Start.Text);
			EhrLabCur.TQ1DateTimeEnd=EhrLab.formatDateToHL7(textTQ1Stop.Text);
			EhrLabs.SaveToDB(EhrLabCur);
			Patient patCur=Patients.GetPat(EhrLabCur.PatNum);
			for(int i=0;i<EhrLabCur.ListEhrLabResults.Count;i++) {
				if(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowCDS && CDSPermissions.GetForUser(Security.CurUser.UserNum).LabTestCDS) {
					using FormCDSIntervention FormCDSI=new FormCDSIntervention();
					FormCDSI.ListCDSInterventions=EhrTriggers.TriggerMatch(EhrLabCur.ListEhrLabResults[i],patCur);
					FormCDSI.ShowIfRequired(false);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

	}
}
