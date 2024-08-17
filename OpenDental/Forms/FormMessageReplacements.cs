using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	///<summary>If using selection mode, call FormMessageReplacements modally.  If not in selection mode, non-modal instances are fine.</summary>
	public partial class FormMessageReplacements:FormODBase {
		public MessageReplacementSystemType MessageReplacementSystemType=MessageReplacementSystemType.None;
		public bool IsSelectionMode;
		///<summary>If false, then options that are considered PHI such as SSN, Last Name, and Birthdate will not be able to be selected.</summary>
		private bool _allowPHI;
		private List<MessageReplaceType> _listMessageReplaceTypes;

		///<summary>Returns empty string if there is no Replacement String selected in the grid.</summary>
		public string Replacement {
			get {
				if(gridMain==null || gridMain.IsDisposed || gridMain.GetSelectedIndex()==-1) {
					return "";
				}
				return gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[1].Text;
			}
		}

		public FormMessageReplacements(List<MessageReplaceType> listMessageReplaceTypes ,bool allowPHI=true) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listMessageReplaceTypes=listMessageReplaceTypes;
			_allowPHI=allowPHI;
		}

		private void FormMessageReplacements_Load(object sender,EventArgs e) {
			if(IsSelectionMode) {
				butClose.Text=Lan.g(this,"Cancel");
				butOK.Visible=true;
			}
			if(!_allowPHI) {
				labelExplanation.Text+="  "+Lan.g(this,"Fields in grey are protected health information.");
			}
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Type"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Replacement"),155);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),200){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			List<ReplacementField> listReplacementFields=GetReplacementFieldList(_allowPHI,_listMessageReplaceTypes,MessageReplacementSystemType);
			for(int i=0;i<listReplacementFields.Count;i++) {
				AddReplacementRow(listReplacementFields[i]);
			}
			gridMain.EndUpdate();
		}

		public static List<ReplacementField> GetReplacementFieldList(bool isAllowPhi,List<MessageReplaceType> listMessageReplaceTypesSupported,MessageReplacementSystemType messageReplacementSystemType=MessageReplacementSystemType.None) 
		{
			List<ReplacementField> listReplacementFields=new List<ReplacementField>();
			if(messageReplacementSystemType==MessageReplacementSystemType.MassEmail) {
				listReplacementFields.Add(new ReplacementField("[FName]","The patient's first name.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[NamePreferredOrFirst]","The patient's preferred name. This will default to their first name if "
					+"no preferred name exists.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[OfficePhone]","The practice or clinic phone number in standard format.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[OfficeFax]","The practice or clinic fax number in standard format.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[OfficeName]","The practice or clinic name.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[OfficeAddress]","The practice or clinic address.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
				return listReplacementFields;
			}
			if(messageReplacementSystemType==MessageReplacementSystemType.SMS) {
				listReplacementFields.Add(new ReplacementField("[nameF]","The patient's first name.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[namePref]","The patient's preferred name.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[PatNum]","The patient's internal id.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[StatementBalance]","The balance for a given Statement.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[OfficePhone]","The practice or clinic phone number in standard format.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[OfficeName]","The practice or clinic name.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[currentMonth]","The current month.",MessageReplaceType.Misc,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[StatementShort]","The short URL that will link the patient to their statement.",MessageReplaceType.Misc,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[StatementURL]","The long URL that will link the patient to their statement.",MessageReplaceType.Misc,isAllowPhi,listMessageReplaceTypesSupported));
				return listReplacementFields;
			}
			if(messageReplacementSystemType==MessageReplacementSystemType.Mount){
				listReplacementFields.Add(new ReplacementField("[NameFL]","First 'Preferred' Middle Last",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[NameLF]","Last, 'Preferred' First Middle",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[PatNum]","The patient's account number.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[ChartNumber]","The patient's chart number.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[Birthdate]","Format in the US is mm/dd/yyyy.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[MountDate]","Date of the mount.",MessageReplaceType.Mount,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[MountDescript]","Description of the mount.",MessageReplaceType.Mount,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[MountProv]","Provider of the mount, First Last Suffix.",MessageReplaceType.Mount,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[OfficeName]","The practice or clinic name.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
				listReplacementFields.Add(new ReplacementField("[OfficePhone]","The practice or clinic phone number in standard format.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
				return listReplacementFields;
			}
			#region Patient Replacement Rows
			listReplacementFields.Add(new ReplacementField("[FName]","The patient's first name.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[LName]","The patient's last name.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[PatientMiddleInitial]","The patient's middle initial.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[LNameLetter]","The first letter of the patient's last name, capitalized.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[NameF]","The patient's first name.  Same as FName.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[NameFL]","The patient's first name, a space, then the patient's last name.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[NamePreferredOrFirst]","The patient's preferred name. This will default to their first name if "
				+"no preferred name exists.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[PatientTitle]","The patient's title.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[PatNum]","The patient's account number.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ChartNumber]","The patient's chart number.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[WirelessPhone]","The patient's wireless phone number.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[HmPhone]","The patient's home phone number.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[WkPhone]","The patient's work phone number.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[Birthdate]","The patient's birthdate. The format is culture-sensitive.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[Birthdate_yyyyMMdd]","The patient's birthdate in the format yyyyMMdd.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[SSN]","The patient's social security number.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[Address]","The patient's address.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[Address2]","The patient's address2.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[City]","The patient's city.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[State]","The patient's state.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[Zip]","The patient's zip code.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvNameFL]","The first and last name of the provider that referred the patient.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[MonthlyCardsOnFile]","Masked list of the patient's monthly credit cards on file.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[PatientPreferredName]","The patient's preferred name.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[PatientGenderMF]","The patient's gender in the format M-Male and F-Female.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[PrimaryProviderNameFLSuffix]","The patient's primary provider's Name. (ex: Jordan Sparks, DMD)",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			#endregion
			#region Patient's Guarantor Replacement Rows
			listReplacementFields.Add(new ReplacementField("[GuarantorPatNum]","The patient's guarantor's PatNum.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[GuarantorNameF]","The patient's guarantor's first name.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[GuarantorNameL]","The patient's guarantor's last name.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[GuarantorMiddleInitial]","The patient's guarantor's middle initial.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[GuarantorTitle]","The patient's guarantor's title.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[GuarantorHmPhone]","The patient's guarantor's home phone number.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[GuarantorWkPhone]","The patient's guarantor's work phone number.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[GuarantorAddress]","The patient's guarantor's address.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[GuarantorAddress2]","The patient's guarantor's address2.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[GuarantorCity]","The patient's guarantor's city.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[GuarantorState]","The patient's guarantor's state.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[GuarantorZip]","The patient's guarantor's zip code.",MessageReplaceType.Patient,isAllowPhi,listMessageReplaceTypesSupported));
			#endregion
			#region Family Replacement Rows
			//family replacement rows
			listReplacementFields.Add(new ReplacementField("[FamilyList]","List of the patient's family members, one per line.",MessageReplaceType.Family,isAllowPhi,listMessageReplaceTypesSupported));
			#endregion
			#region Appointment Replacement Rows
			//appointment replacement rows
			listReplacementFields.Add(new ReplacementField("[ApptDate]","The appointment date.",MessageReplaceType.Appointment,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ApptTime]","The appointment time.",MessageReplaceType.Appointment,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ApptDayOfWeek]","The day of the week the appointment falls on.",MessageReplaceType.Appointment,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ApptProcsList]","The procedures attached to the appointment, one per line, including procedure date and layman's term.",
				MessageReplaceType.Appointment,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[date]","The appointment date.  Synonym of ApptDate.",MessageReplaceType.Appointment,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[time]","The appointment time.  Synonym of ApptTime.",MessageReplaceType.Appointment,isAllowPhi,listMessageReplaceTypesSupported));
			#endregion
			#region Recall Replacement Rows
			//recall replacement rows
			listReplacementFields.Add(new ReplacementField("[DueDate]","Max selected recall date for the patient.",MessageReplaceType.Recall,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[URL]","The link where a patient can go to schedule a recall from the web.",MessageReplaceType.Recall,isAllowPhi,listMessageReplaceTypesSupported));
			#endregion
			#region User Replacement Rows
			//user replacement rows
			listReplacementFields.Add(new ReplacementField("[UserNameF]","The first name of the person who is currently logged in.",MessageReplaceType.User,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[UserNameL]","The last name of the person who is currently logged in.",MessageReplaceType.User,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[UserNameFL]","The first name, a space, then the last name of the person who is currently logged in.",
				MessageReplaceType.User,isAllowPhi,listMessageReplaceTypesSupported));
			#endregion
			#region Office Replacement Rows
			//office replacement rows
			listReplacementFields.Add(new ReplacementField("[OfficePhone]","The practice or clinic phone number in standard format.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[OfficeFax]","The practice or clinic fax number in standard format.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[OfficeName]","The practice or clinic name.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[OfficeAddress]","The practice or clinic address.",MessageReplaceType.Office,isAllowPhi,listMessageReplaceTypesSupported));
			#endregion
			#region Payment Plan Rows
			listReplacementFields.Add(new ReplacementField("[APR]","Annual Percentage Rate.",MessageReplaceType.PaymentPlan,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[PaymentAmt]","The amount of each payment plan charge.",MessageReplaceType.PaymentPlan,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ChargeFrequency]","The frequency of charges being issued.",MessageReplaceType.PaymentPlan,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[NumOfPayments]","The total number of payments (for dynamic payment plans, always 0).",MessageReplaceType.PaymentPlan,isAllowPhi,listMessageReplaceTypesSupported));
			#endregion
			#region Miscellaneous Replacement Rows
			//misc replacement rows
			listReplacementFields.Add(new ReplacementField("[CurrentMonth]","The text description of the current month (ex December).",MessageReplaceType.Misc,isAllowPhi,listMessageReplaceTypesSupported));
			#endregion
			#region Patient's "From" Referrals-IsDoctor only Replacement Rows
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvInitialReferralNum]","The 'ReferralNum' of the initial (oldest) 'Referred From' provider that is marked as "
				+"'Is Doctor'.",MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvInitialNameF]","The first name of the initial (oldest) 'Referred From' provider that is marked as "
				+"'Is Doctor'.",MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvInitialNameL]","The last name of the initial (oldest) 'Referred From' provider that is marked as "
				+"'Is Doctor'.",MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvInitialPhone]","The 'telephone' of the initial (oldest) 'Referred From' provider that is marked as "
				+"'Is Doctor'",MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvInitialAddress]","The 'Address' of the initial (oldest) 'Referred From' provider that is marked as "
				+"'Is Doctor'.",MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvInitialAddress2]","The 'Address2' of the initial (oldest) 'Referred From' provider that is marked as "
				+"'Is Doctor'.",MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvInitialCity]","The city of the initial (oldest) 'Referred From' provider that is marked as 'Is Doctor'.",
				MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvInitialState]","The state of the initial (oldest) 'Referred From' provider that is marked as 'Is Doctor'.",
				MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvInitialZip]","The zipcode of the initial (oldest) 'Referred From' provider that is marked as 'Is Doctor'."
				,MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvMostRecentReferralNum]","The 'ReferralNum' of the most recent 'Referred From' provider that is marked as "
				+"'Is Doctor'.",MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvMostRecentNameF]","The first name of the most recent 'Referred From' provider that is marked as "
				+"'Is Doctor'.",MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvMostRecentNameL]","The last name of the most recent 'Referred From' provider that is marked as 'Is Doctor'.",
				MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvMostRecentPhone]","The 'Telephone' of the most recent 'Referred From' provider that is marked as "
				+"'Is Doctor'.",MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvMostRecentAddress]","The 'Address' of the most recent 'Referred From' provider that is marked as "
				+"'Is Doctor'.",MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvMostRecentAddress2]","The 'Address2' of the most recent 'Referred From' provider that is marked as "
				+"'Is Doctor'.",MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvMostRecentCity]","The city of the most recent 'Referred From' provider that is marked as 'Is Doctor'.",
				MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvMostRecentState]","The state of the most recent 'Referred From' provider that is marked as 'Is Doctor'.",
				MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			listReplacementFields.Add(new ReplacementField("[ReferredFromProvMostRecentZip]","The zipcode of the most recent 'Referred From' provider that is marked as 'Is Doctor'.",
				MessageReplaceType.Referral,isAllowPhi,listMessageReplaceTypesSupported));
			#endregion
			return listReplacementFields;
		}

		///<summary>Builds and inserts a replacement row into the grid using the passed-in field name, description, and replacement type.</summary>
		private void AddReplacementRow(ReplacementField replacementField) {
			GridRow row=new GridRow();
			row.Cells.Add(Lan.g("enumMessageReplaceType",replacementField.MessageReplaceTypeCur.ToString()));
			row.Cells.Add(replacementField.FieldName);
			row.Cells.Add(Lan.g(this,replacementField.Descript));
			if(replacementField.IsPHI) {
				row.ColorText=Color.DarkSlateGray;
			}
			if(!replacementField.IsSupported) {
				row.ColorText=Color.Red;
			}
			row.Tag=replacementField;
			gridMain.ListGridRows.Add(row);
		}

		///<summary>Replaces all family fields in the given message with the given family's information.  Returns the resulting string.
		///Will Replace: [FamilyList], currently does nothing. </summary>
		public static string ReplaceFamily(string message,Family family) {
			string retVal=message;
			//TODO: mimic pattern in Recalls.GetAddrTable
			return retVal;
		}

		///<summary>Replaces all recall fields in the given message with the given recall list's information.  Returns the resulting string.
		///Will replace: [DueDate], [URL], currently does nothing.</summary>
		public static string ReplaceRecall(string message,List<Recall> listRecallsForPat) {
			string retVal=message;
			//TODO: these replacements are a lot of work. 
			//When we decide to implement, mimic the pattern regarding the other areas of the program where these replacements are already used.
			return retVal;
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();//Because we want the option to open this window non-modal.
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PressOK(e.Row);
		}

		///<summary>Only visible if IsSelectionMode is true.</summary>
		private void butOK_Click(object sender,EventArgs e) {
			PressOK(gridMain.GetSelectedIndex());
		}

		private void PressOK(int index) {
			if(index<0) {
				MsgBox.Show(this,"Please select a field.");
				return;
			}
			if(!((ReplacementField)gridMain.ListGridRows[index].Tag).IsSupported) {
				MsgBox.Show(this,"The selected field is not supported for this method of communication.");
				return;
			}
			if(((ReplacementField)gridMain.ListGridRows[index].Tag).IsPHI && !_allowPHI) {
				MsgBox.Show(this,"The selected field is considered Protected Health Information and this method of communication does not allow PHI.");
				return;
			}
			DialogResult=DialogResult.OK;
			Close();//Because we want the option to open this window non-modal.
		}

		public class ReplacementField {
			public bool IsSupported;
			public bool IsPHI=false;
			public string FieldName;
			public string Descript;
			public MessageReplaceType MessageReplaceTypeCur;

			public ReplacementField(string fieldName,String descript,MessageReplaceType messageReplaceType,bool isAllowPhi,List<MessageReplaceType> listMessageReplaceTypesSupported) {
				if(!isAllowPhi && Patients.IsFieldPHI(fieldName)) {
					IsPHI=true;
				}
				if(listMessageReplaceTypesSupported.Contains(messageReplaceType)){
					IsSupported=true;
				}
				else{
					IsSupported=false;
				}
				FieldName=fieldName;
				Descript=descript;
				MessageReplaceTypeCur=messageReplaceType;
			}
		}
	}

	///<summary></summary>
	public enum MessageReplaceType {
		Patient,
		Family,
		Appointment,
		Recall,
		User,
		Office,
		Misc,
		Referral,
		PaymentPlan,
		Mount
	}

	public enum MessageReplacementSystemType {
		None,
		MassEmail,
		SMS,
		Mount
	}
}