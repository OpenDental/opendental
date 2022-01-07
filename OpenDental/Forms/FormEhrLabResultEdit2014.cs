using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EhrLaboratories;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrLabResultEdit2014:FormODBase {
		public EhrLabResult EhrLabResultCur;
		public bool IsImport;
		public bool IsViewOnly;

		public FormEhrLabResultEdit2014() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormLabResultEdit_Load(object sender,EventArgs e) {
			if(IsImport || IsViewOnly) {
				foreach(Control c in this.Controls) {
					c.Enabled=false;
				}
				butCancel.Enabled=true;
				butCancel.Text="Close";
				gridNotes.Enabled=true;
				//butAddNote.Enabled=false;
				//butObsIdLoinc.Enabled=false;
				//butCodedElementSnomed.Enabled=false;
				//butUnitOfMeasureUCUM.Enabled=false;
				//butOk.Enabled=false;
				//combos
			}
			//textObsDateTime.Text=EhrLab.formatDateFromHL7(EhrLabResultCur.ObservationDateTime);
			//textAnalysisDateTime.Text=EhrLab.formatDateFromHL7(EhrLabResultCur.AnalysisDateTime);
			#region Observation Identifier (LOINC Codes)
			textObsIDCodeSystemName.Text		=EhrLabResultCur.ObservationIdentifierCodeSystemName;
			textObsID.Text									=EhrLabResultCur.ObservationIdentifierID;
			textObsIDText.Text							=EhrLabResultCur.ObservationIdentifierText;
			textObsIDCodeSystemNameAlt.Text	=EhrLabResultCur.ObservationIdentifierCodeSystemNameAlt;
			textObsIDAlt.Text								=EhrLabResultCur.ObservationIdentifierIDAlt;
			textObsIDTextAlt.Text						=EhrLabResultCur.ObservationIdentifierTextAlt;
			textObsIDOrigText.Text					=EhrLabResultCur.ObservationIdentifierTextOriginal;
			textObsSub.Text									=EhrLabResultCur.ObservationIdentifierSub;
			#endregion
			#region Abnormal Flags
			listAbnormalFlags.Items.Clear();
			List<string> listAbnormalFlagsStr=EhrLabResults.GetHL70078Descriptions();
			listAbnormalFlags.Items.AddList(listAbnormalFlagsStr,x => x.ToString());
			string[] abnormalFlags=EhrLabResultCur.AbnormalFlags.Split(',');
			for(int i=0;i<abnormalFlags.Length;i++) {
				if(abnormalFlags[i]=="") {
					continue;
				}
				listAbnormalFlags.SetSelected((int)Enum.Parse(typeof(HL70078),abnormalFlags[i],true));
			}
			#endregion
			#region Observation Value
			textObsDateTime.Text=EhrLab.formatDateFromHL7(EhrLabResultCur.ObservationDateTime);
			textAnalysisDateTime.Text=EhrLab.formatDateFromHL7(EhrLabResultCur.AnalysisDateTime);
			#region Observation Status
			comboObsStatus.Items.Clear();
			comboObsStatus.BeginUpdate();
			//Fill obs status combo with HL70085 enum.  Not sure if blank is acceptable.
			List<string> listObsStatus=EhrLabResults.GetHL70085Descriptions();
			comboObsStatus.Items.AddRange(listObsStatus.ToArray());
			comboObsStatus.EndUpdate();
			comboObsStatus.SelectedIndex=(int)Enum.Parse(typeof(HL70085),EhrLabResultCur.ObservationResultStatus.ToString(),true)+1;
			#endregion
			#region Value Type
			comboObsValueType.Items.Clear();
			comboObsValueType.BeginUpdate();
			//Fill obs value type combo with HL70125 enum.  Not sure if blank is acceptable.
			List<string> listObsValueType=EhrLabResults.GetHL70125Descriptions();
			comboObsValueType.Items.AddRange(listObsValueType.ToArray());
			comboObsValueType.EndUpdate();
			comboObsValueType.SelectedIndex=(int)Enum.Parse(typeof(HL70125),EhrLabResultCur.ValueType.ToString(),true)+1;
			#endregion
			textObsValue.Text=GetObservationText();
			#region Coded Elements
			textObsElementCodeSystem.Text		=EhrLabResultCur.ObservationValueCodedElementCodeSystemName;
			textObsElementID.Text						=EhrLabResultCur.ObservationValueCodedElementID;
			textObsElementText.Text					=EhrLabResultCur.ObservationValueCodedElementText;
			textObsElementCodeSystemAlt.Text=EhrLabResultCur.ObservationValueCodedElementCodeSystemNameAlt;
			textObsElementIDAlt.Text				=EhrLabResultCur.ObservationValueCodedElementIDAlt;
			textObsElementTextAlt.Text			=EhrLabResultCur.ObservationValueCodedElementTextAlt;
			textObsElementOrigText.Text			=EhrLabResultCur.ObservationValueCodedElementTextOriginal;
			#endregion
			#region Structured Numeric
			textStructNumComp.Text			=EhrLabResultCur.ObservationValueComparator;
			textStructNumFirst.Text			=EhrLabResultCur.ObservationValueNumber1.ToString();
			textStructNumSeparator.Text	=EhrLabResultCur.ObservationValueSeparatorOrSuffix;
			textStructNumSecond.Text		=EhrLabResultCur.ObservationValueNumber2.ToString();
			#endregion
			#region Unit of Measure
			textObsUnitsCodeSystem.Text		=EhrLabResultCur.UnitsCodeSystemName;
			textObsUnitsID.Text				=EhrLabResultCur.UnitsID;
			textObsUnitsText.Text			=EhrLabResultCur.UnitsText;
			textObsUnitsCodeSystemAlt.Text	=EhrLabResultCur.UnitsCodeSystemNameAlt;
			textObsUnitsIDAlt.Text		=EhrLabResultCur.UnitsIDAlt;
			textObsUnitsTextAlt.Text	=EhrLabResultCur.UnitsTextAlt;
			textObsUnitsTextOrig.Text	=EhrLabResultCur.UnitsTextOriginal;
			#endregion
			#endregion
			#region Performing Organization
			#region Name
			textPerfOrgName.Text=EhrLabResultCur.PerformingOrganizationName;
			#region Identifier Type
			comboPerfOrgIdType.Items.Clear();
			comboPerfOrgIdType.BeginUpdate();
				//Fill identifier type combo with HL70203 enum.  Not sure if blank is acceptable.
			List<string> listPerfOrgIdType=EhrLabs.GetHL70203Descriptions();
			comboPerfOrgIdType.Items.AddRange(listPerfOrgIdType.ToArray());
			comboPerfOrgIdType.EndUpdate();
			comboPerfOrgIdType.SelectedIndex=(int)Enum.Parse(typeof(HL70203),EhrLabResultCur.PerformingOrganizationIdentifierTypeCode.ToString(),true)+1;
			#endregion
			textPerfOrgIdentifier.Text=EhrLabResultCur.PerformingOrganizationIdentifier;
			#region Assigning Authority
			textPerfOrgAssignIdType.Text=EhrLabResultCur.PerformingOrganizationNameAssigningAuthorityUniversalIdType;
			textPerfOrgNamespaceID.Text=EhrLabResultCur.PerformingOrganizationNameAssigningAuthorityNamespaceId;
			textPerfOrgUniversalID.Text=EhrLabResultCur.PerformingOrganizationNameAssigningAuthorityUniversalId;
			#endregion
			#endregion
			#region Address
			#region Address Type
			comboPerfOrgAddressType.Items.Clear();
			comboPerfOrgAddressType.BeginUpdate();
			//Fill address type combo with HL70190 enum.  Not sure if blank is acceptable.
			List<string> listPerfOrgAddressType=EhrLabResults.GetHL70190Descriptions();
			comboPerfOrgAddressType.Items.AddRange(listPerfOrgAddressType.ToArray());
			comboPerfOrgAddressType.EndUpdate();
			comboPerfOrgAddressType.SelectedIndex=(int)Enum.Parse(typeof(HL70190),EhrLabResultCur.PerformingOrganizationAddressAddressType.ToString(),true)+1;
			#endregion
			textPerfOrgStreet.Text					=EhrLabResultCur.PerformingOrganizationAddressStreet;
			textPerfOrgOtherDesignation.Text=EhrLabResultCur.PerformingOrganizationAddressOtherDesignation;
			textPerfOrgCity.Text						=EhrLabResultCur.PerformingOrganizationAddressCity;
			#region State or Province
			comboPerfOrgState.Items.Clear();
			comboPerfOrgState.BeginUpdate();
			//Fill state combo with USPSAlphaStateCode enum.  Not sure if blank is acceptable.
			List<string> listPerfOrgState=EhrLabResults.GetUSPSAlphaStateCodeDescriptions();
			comboPerfOrgState.Items.AddRange(listPerfOrgState.ToArray());
			comboPerfOrgState.EndUpdate();
			comboPerfOrgState.SelectedIndex=(int)Enum.Parse(typeof(USPSAlphaStateCode),EhrLabResultCur.PerformingOrganizationAddressStateOrProvince.ToString(),true)+1;
			#endregion
			textPerfOrgZip.Text			=EhrLabResultCur.PerformingOrganizationAddressZipOrPostalCode;
			textPerfOrgCountry.Text	=EhrLabResultCur.PerformingOrganizationAddressCountryCode;
			textPerfOrgCounty.Text	=EhrLabResultCur.PerformingOrganizationAddressCountyOrParishCode;
			#endregion
			#region Medical Director
			#region Identifier Type
			comboMedDirIdType.Items.Clear();
			comboMedDirIdType.BeginUpdate();
			//Fill medical director type combo with HL70203 enum.  Not sure if blank is acceptable.
			List<string> listMedDirIdType=EhrLabs.GetHL70203Descriptions();
			comboMedDirIdType.Items.AddRange(listMedDirIdType.ToArray());
			comboMedDirIdType.EndUpdate();
			comboMedDirIdType.SelectedIndex=(int)Enum.Parse(typeof(HL70203),EhrLabResultCur.MedicalDirectorIdentifierTypeCode.ToString(),true)+1;
			#endregion
			textMedDirIdentifier.Text=EhrLabResultCur.MedicalDirectorID;
			#region Name Type
			comboMedDirNameType.Items.Clear();
			comboMedDirNameType.BeginUpdate();
			//Fill medical director name combo with HL70200 enum.  Not sure if blank is acceptable.
			List<string> listMedDirNameType=EhrLabResults.GetHL70200Descriptions();
			comboMedDirNameType.Items.AddRange(listMedDirIdType.ToArray());
			comboMedDirNameType.EndUpdate();
			comboMedDirNameType.SelectedIndex=(int)Enum.Parse(typeof(HL70200),EhrLabResultCur.MedicalDirectorNameTypeCode.ToString(),true)+1;
			#endregion
			textMedDirLastName.Text		=EhrLabResultCur.MedicalDirectorLName;
			textMedDirFirstName.Text	=EhrLabResultCur.MedicalDirectorFName;
			textMedDirMiddleName.Text	=EhrLabResultCur.MedicalDirectorMiddleNames;
			textMedDirSuffix.Text			=EhrLabResultCur.MedicalDirectorSuffix;
			textMedDirPrefix.Text			=EhrLabResultCur.MedicalDirectorPrefix;
			#region Assigning Authority
			textMedDirAssignIdType.Text=EhrLabResultCur.MedicalDirectorAssigningAuthorityIDType;
			textMedDirNamespaceID.Text=EhrLabResultCur.MedicalDirectorAssigningAuthorityNamespaceID;
			textMedDirUniversalID.Text=EhrLabResultCur.MedicalDirectorAssigningAuthorityUniversalID;
			#endregion
			#endregion
			#endregion
			textReferenceRange.Text=EhrLabResultCur.referenceRange;
			FillGridNotes();
		}

		///<summary>Gets the observation text dynamically from the result passed in.  Returns empty string if unknown value type.</summary>
		private string GetObservationText() {
			//No need to check RemotingRole;
			switch(EhrLabResultCur.ValueType) {
				case HL70125.CE:
				case HL70125.CWE:
				case HL70125.SN:
					return "";//Handled later in Load.
				case HL70125.DT:
				case HL70125.TS:
					return EhrLabResultCur.ObservationValueDateTime;
				case HL70125.NM:
					return EhrLabResultCur.ObservationValueNumeric.ToString();
				case HL70125.FT:
				case HL70125.ST:
				case HL70125.TX:
					return EhrLabResultCur.ObservationValueText;
				case HL70125.TM:
					return EhrLabResultCur.ObservationValueTime.ToShortTimeString();
			}
			return "";//Unknown value type.
		}

		private void FillGridNotes() {
			gridNotes.BeginUpdate();
			gridNotes.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Note Num",60);
			gridNotes.ListGridColumns.Add(col);
			col=new GridColumn("Comments",300);
			gridNotes.ListGridColumns.Add(col);
			gridNotes.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<EhrLabResultCur.ListEhrLabResultNotes.Count;i++) {
				for(int j=0;j<EhrLabResultCur.ListEhrLabResultNotes[i].Comments.Split('^').Length;j++) {
					row=new GridRow();
					row.Cells.Add((j==0?(i+1).ToString():""));//add note number if this is first comment for the note, otherwise add blank cell.
					row.Cells.Add(EhrLabResultCur.ListEhrLabResultNotes[i].Comments.Split('^')[j]);//Add each comment.
					gridNotes.ListGridRows.Add(row);
				}
			}
			gridNotes.EndUpdate();
		}

		private void butAddAbnormalFlag_Click(object sender,EventArgs e) {

		}

		private void gridAbnormalFlags_MouseDoubleClick(object sender,MouseEventArgs e) {

		}

		private void butAddNote_Click(object sender,EventArgs e) {
			if(IsImport || IsViewOnly) {
				return;//should never happen. This button shoudl not be enabled if IsImport or IsViewOnly
			}
			using FormEhrLabNoteEdit FormLNE=new FormEhrLabNoteEdit();
			FormLNE.LabNoteCur=new EhrLabNote();
			FormLNE.ShowDialog();
			if(FormLNE.DialogResult!=DialogResult.OK) {
				return;
			}
			FormLNE.LabNoteCur.EhrLabNum=EhrLabResultCur.EhrLabNum;
			FormLNE.LabNoteCur.EhrLabResultNum=EhrLabResultCur.EhrLabResultNum;
			EhrLabResultCur.ListEhrLabResultNotes.Add(FormLNE.LabNoteCur);
			FillGridNotes();
		}

		private void gridNotes_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			using FormEhrLabNoteEdit FormLNE=new FormEhrLabNoteEdit();
			FormLNE.IsImport=IsImport;
			FormLNE.IsViewOnly=IsViewOnly;
			FormLNE.LabNoteCur=EhrLabResultCur.ListEhrLabResultNotes[e.Row];
			FormLNE.ShowDialog();
			if(IsImport || IsViewOnly || FormLNE.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrLabResultCur.ListEhrLabResultNotes[e.Row]=FormLNE.LabNoteCur;
			FillGridNotes();
		}

		private void butObsIdLoinc_Click(object sender,EventArgs e) {
			using FormLoincs FormL=new FormLoincs();
			FormL.IsSelectionMode=true;
			FormL.ShowDialog();
			if(FormL.DialogResult!=DialogResult.OK) {
				return;
			}
			textObsID.Text=FormL.SelectedLoinc.LoincCode;
			textObsIDCodeSystemName.Text="LN";
			textObsIDText.Text=FormL.SelectedLoinc.NameShort;
			textObsIDOrigText.Text=FormL.SelectedLoinc.NameLongCommon;
		}

		private void butCodedElementLoinc_Click(object sender,EventArgs e) {
			using FormSnomeds FormS=new FormSnomeds();
			FormS.IsSelectionMode=true;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			textObsElementID.Text=FormS.SelectedSnomed.SnomedCode;
			textObsElementText.Text=FormS.SelectedSnomed.Description;
			textObsElementCodeSystem.Text="SCT";
			textObsElementOrigText.Text=FormS.SelectedSnomed.Description;
		}

		private void butUnitOfMeasureUCUM_Click(object sender,EventArgs e) {
			using FormUcums FormU=new FormUcums();
			FormU.IsSelectionMode=true;
			FormU.ShowDialog();
			if(FormU.DialogResult!=DialogResult.OK) {
				return;
			}
			textObsUnitsID.Text=FormU.SelectedUcum.UcumCode;
			textObsUnitsCodeSystem.Text="UCUM";
			textObsUnitsText.Text=FormU.SelectedUcum.Description;
			textObsUnitsTextOrig.Text=FormU.SelectedUcum.Description;
		}

		///<summary></summary>
		private bool EntriesAreValid() {
			//TODO: validate the controls
			return true;
		}

		private void butOk_Click(object sender,EventArgs e) {
			if(!EntriesAreValid()) {
				return;
			}
			//Saving happens in the parent form
			EhrLabResultCur.ObservationIdentifierID=textObsID.Text;
			EhrLabResultCur.ObservationIdentifierText=textObsIDText.Text;
			EhrLabResultCur.ObservationIdentifierCodeSystemName=textObsIDCodeSystemName.Text;
			EhrLabResultCur.ObservationIdentifierIDAlt=textObsIDAlt.Text;
			EhrLabResultCur.ObservationIdentifierTextAlt=textObsIDTextAlt.Text;
			EhrLabResultCur.ObservationIdentifierCodeSystemNameAlt=textObsIDCodeSystemNameAlt.Text;
			EhrLabResultCur.ObservationIdentifierTextOriginal=textObsIDOrigText.Text;
			EhrLabResultCur.ObservationIdentifierSub=textObsSub.Text;
			EhrLabResultCur.AbnormalFlags="";
			for(int i=0;i<listAbnormalFlags.SelectedIndices.Count;i++) {
				if(i>0) {
					EhrLabResultCur.AbnormalFlags+=",";
				}
				EhrLabResultCur.AbnormalFlags+=((HL70078)listAbnormalFlags.SelectedIndices[i]).ToString();
			}
			//Observation Value
			EhrLabResultCur.ObservationDateTime=EhrLab.formatDateToHL7(textObsDateTime.Text);
			EhrLabResultCur.AnalysisDateTime=EhrLab.formatDateToHL7(textAnalysisDateTime.Text);
			EhrLabResultCur.ObservationResultStatus=((HL70085)comboObsStatus.SelectedIndex-1);
			EhrLabResultCur.ValueType=((HL70125)comboObsValueType.SelectedIndex-1);
			EhrLabResultCur.referenceRange=textReferenceRange.Text;
			switch(((HL70125)comboObsValueType.SelectedIndex-1)) {
				case HL70125.CE:
				case HL70125.CWE:
					break;//nothing to do here. yet.
				case HL70125.DT:
				case HL70125.TS:
					EhrLabResultCur.ObservationValueDateTime=EhrLab.formatDateToHL7(textObsValue.Text);
					break;
				case HL70125.NM:
					EhrLabResultCur.ObservationValueNumeric=PIn.Double(textObsValue.Text);
					break;
				case HL70125.FT:
				case HL70125.ST:
				case HL70125.TX:
					EhrLabResultCur.ObservationValueText=textObsValue.Text;//should not contain |~^&# characters
					break;
				case HL70125.TM:
					EhrLabResultCur.ObservationValueTime=PIn.Time(textObsValue.Text);
					break;
				case HL70125.SN:
					break;//nothing to do here yet.
			}
			//if(((HL70125)comboObsValueType.SelectedIndex-1)==HL70125.DT
			//	|| ((HL70125)comboObsValueType.SelectedIndex-1)==HL70125.TS
			//	|| ((HL70125)comboObsValueType.SelectedIndex-1)==HL70125.TM) 
			//{
			//	EhrLabResultCur.ObservationValueDateTime=EhrLab.formatDateToHL7(textObsValue.Text);
			//}
			//else {
			//	EhrLabResultCur.ObservationValueNumeric=PIn.Double(textObsValue.Text);
			//}
				//Coded Element
			EhrLabResultCur.ObservationValueCodedElementID=textObsElementID.Text;
			EhrLabResultCur.ObservationValueCodedElementText=textObsElementText.Text;
			EhrLabResultCur.ObservationValueCodedElementCodeSystemName=textObsElementCodeSystem.Text;
			EhrLabResultCur.ObservationValueCodedElementIDAlt=textObsElementIDAlt.Text;
			EhrLabResultCur.ObservationValueCodedElementTextAlt=textObsElementTextAlt.Text;
			EhrLabResultCur.ObservationValueCodedElementCodeSystemNameAlt=textObsElementCodeSystemAlt.Text;
			EhrLabResultCur.ObservationValueCodedElementTextOriginal=textObsElementOrigText.Text;
				//Structured Numeric
			EhrLabResultCur.ObservationValueComparator=textStructNumComp.Text;
			EhrLabResultCur.ObservationValueNumber1=PIn.Double(textStructNumFirst.Text);
			EhrLabResultCur.ObservationValueSeparatorOrSuffix=textStructNumSeparator.Text;
			EhrLabResultCur.ObservationValueNumber2=PIn.Double(textStructNumSecond.Text);
				//Units
			EhrLabResultCur.UnitsID=textObsUnitsID.Text;
			EhrLabResultCur.UnitsText=textObsUnitsText.Text;
			EhrLabResultCur.UnitsCodeSystemName=textObsUnitsCodeSystem.Text;
			EhrLabResultCur.UnitsIDAlt=textObsUnitsIDAlt.Text;
			EhrLabResultCur.UnitsTextAlt=textObsUnitsTextAlt.Text;
			EhrLabResultCur.UnitsCodeSystemNameAlt=textObsUnitsCodeSystemAlt.Text;
			EhrLabResultCur.UnitsTextOriginal=textObsUnitsTextOrig.Text;
			//Performing Organization
			EhrLabResultCur.PerformingOrganizationName=textPerfOrgName.Text;
			EhrLabResultCur.PerformingOrganizationNameAssigningAuthorityNamespaceId=textPerfOrgNamespaceID.Text;
			EhrLabResultCur.PerformingOrganizationNameAssigningAuthorityUniversalId=textPerfOrgUniversalID.Text;
			EhrLabResultCur.PerformingOrganizationNameAssigningAuthorityUniversalIdType=textPerfOrgAssignIdType.Text;
			EhrLabResultCur.PerformingOrganizationIdentifierTypeCode=((HL70203)comboPerfOrgIdType.SelectedIndex-1);
			EhrLabResultCur.PerformingOrganizationIdentifier=textPerfOrgIdentifier.Text;
			EhrLabResultCur.PerformingOrganizationAddressStreet=textPerfOrgStreet.Text;
			EhrLabResultCur.PerformingOrganizationAddressOtherDesignation=textPerfOrgOtherDesignation.Text;
			EhrLabResultCur.PerformingOrganizationAddressCity=textPerfOrgCity.Text;
			EhrLabResultCur.PerformingOrganizationAddressStateOrProvince=((USPSAlphaStateCode)comboPerfOrgState.SelectedIndex-1);
			EhrLabResultCur.PerformingOrganizationAddressZipOrPostalCode=textPerfOrgZip.Text;
			EhrLabResultCur.PerformingOrganizationAddressCountryCode=textPerfOrgCountry.Text;
			EhrLabResultCur.PerformingOrganizationAddressAddressType=((HL70190)comboPerfOrgAddressType.SelectedIndex-1);
			EhrLabResultCur.PerformingOrganizationAddressCountyOrParishCode=textPerfOrgCounty.Text;
			EhrLabResultCur.MedicalDirectorID=textMedDirIdentifier.Text;
			EhrLabResultCur.MedicalDirectorLName=textMedDirLastName.Text;
			EhrLabResultCur.MedicalDirectorFName=textMedDirFirstName.Text;
			EhrLabResultCur.MedicalDirectorMiddleNames=textMedDirMiddleName.Text;
			EhrLabResultCur.MedicalDirectorSuffix=textMedDirSuffix.Text;
			EhrLabResultCur.MedicalDirectorPrefix=textMedDirPrefix.Text;
			EhrLabResultCur.MedicalDirectorAssigningAuthorityNamespaceID=textMedDirNamespaceID.Text;
			EhrLabResultCur.MedicalDirectorAssigningAuthorityUniversalID=textMedDirUniversalID.Text;
			EhrLabResultCur.MedicalDirectorAssigningAuthorityIDType=textMedDirAssignIdType.Text;
			EhrLabResultCur.MedicalDirectorNameTypeCode=((HL70200)comboMedDirNameType.SelectedIndex-1);
			EhrLabResultCur.MedicalDirectorIdentifierTypeCode=((HL70203)comboMedDirIdType.SelectedIndex-1);
			//Saving happens in parent form.
			DialogResult=DialogResult.OK;
		}

		private void comboObsValueType_SelectedIndexChanged(object sender,EventArgs e) {
			textObsValue.Enabled=false;
			groupCE.Enabled=false;
			groupSN.Enabled=false;
			groupUnitsOfMeasure.Enabled=false; 
			LayoutManager.MoveHeight(textObsValue,20);
			switch(((HL70125)comboObsValueType.SelectedIndex-1)) {
				case HL70125.CE:
				case HL70125.CWE:
					groupCE.Enabled=true;
					break;
				case HL70125.DT:
				case HL70125.TS:
					textObsValue.Enabled=true;
					break;
				case HL70125.NM:
					textObsValue.Enabled=true;
					groupUnitsOfMeasure.Enabled=true;
					break;
				case HL70125.FT:
				case HL70125.ST:
				case HL70125.TX:
					LayoutManager.MoveHeight(textObsValue,48);
					textObsValue.Enabled=true;
					break;
				case HL70125.TM:
					textObsValue.Enabled=true;
					break;
				case HL70125.SN:
					groupSN.Enabled=true;
					groupUnitsOfMeasure.Enabled=true;
					break;
				default:
					break;
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(EhrLabResultCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete Lab Result?")) {
				return;
			}
			//TODO: Actually delete lab result?
			DialogResult=DialogResult.OK;
		}

		
	}
}
