using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using WpfControls.UI;
using OpenDental.Drawing;
using WpfControls;
namespace OpenDental {
	public partial class FrmPatientAddAll:FrmODBase {
		public string LName;
		public string FName;
		public DateTime Birthdate;
		public long PatNumSelected;
		private string _lnameOld1;
		private string _fnameOld2="";
		private string _fnameOld3="";
		private string _fnameOld4="";
		private string _fnameOld5="";
		private string _emailOld1="";
		private string _emailOld2="";
		private string _emailOld3="";
		private string _emailOld4="";
		private string _emailOld5="";
		private string _phoneOld1="";
		/// <summary>displayed from within code, not designer</summary>
		private ListBox listEmps1;
		private string _empOriginal1;
		/// <summary>displayed from within code, not designer</summary>
		private ListBox listEmps2;
		private string _empOriginal2;
		private List<Carrier> _listCarriersSimilar1;
		private string _carOriginal1;
		private ListBox listCars1;
		private Carrier _carrierSelected1;
		private List<Carrier> _listCarriersSimilar2;
		private string _carOriginal2;
		private ListBox listCars2;
		private Carrier _carrierSelected2;
		///<summary>If user picks a plan from list, but then changes one of the critical fields, this will be ignored.  Keep in mind that the plan here is just a copy.  It can't be updated, but must instead be inserted.</summary>
		private InsPlan _insPlanSelected1;
		private InsPlan _insPlanSelected2;
		private Referral _referral;
		/// <summary>displayed from within code, not designer</summary>
		private ListBox listStates;
		/// <summary>used in the medicaidState dropdown logic</summary>
		private string _stateOriginal;
		private List<RequiredField> _listRequiredFields;
		private bool _isMissingRequiredFields;
		private bool _isValidating=false;
		private Commlog _commlog;
		private List<Provider> _listProviders;
		private List<ZipCode> _listZipCodes;
		private List<Def> _listDefsBillingType;
		/// <summary>Used to replace error provider. This will show a tooltip over boxes that have an error that needs to be resolved, along with highlighting said boxes.</summary>
		private ToolTip _toolTip = new ToolTip();

		public FrmPatientAddAll() {
			InitializeComponent();
			Load+=FrmPatientAddAll_Load;
			#region Subscribing to events
			//Comboboxes
			comboSubscriber1.SelectionChangeCommitted+=ComboBox_SelectionChangeCommitted;
			comboPriProv2.SelectionChangeCommitted+=ComboBox_SelectionChangeCommitted;
			comboPriProv3.SelectionChangeCommitted+=ComboBox_SelectionChangeCommitted;
			comboPriProv4.SelectionChangeCommitted+=ComboBox_SelectionChangeCommitted;
			comboPriProv5.SelectionChangeCommitted+=ComboBox_SelectionChangeCommitted;
			comboSecProv2.SelectionChangeCommitted+=ComboBox_SelectionChangeCommitted;
			comboSecProv3.SelectionChangeCommitted+=ComboBox_SelectionChangeCommitted;
			comboSecProv4.SelectionChangeCommitted+=ComboBox_SelectionChangeCommitted;
			comboSecProv5.SelectionChangeCommitted+=ComboBox_SelectionChangeCommitted;
			//Listbox
			listPosition1.SelectionChangeCommitted+=ListBox_SelectionChangeCommitted;
			listGender1.SelectionChangeCommitted+=ListBox_SelectionChangeCommitted;
			listPosition2.SelectionChangeCommitted+=ListBox_SelectionChangeCommitted;
			listGender2.SelectionChangeCommitted+=ListBox_SelectionChangeCommitted;
			listGender3.SelectionChangeCommitted+=ListBox_SelectionChangeCommitted;
			listGender4.SelectionChangeCommitted+=ListBox_SelectionChangeCommitted;
			listGender5.SelectionChangeCommitted+=ListBox_SelectionChangeCommitted;
			//Textbox
			textLName1.LostFocus+=textBox_LostFocus;
			textLName2.LostFocus+=textBox_LostFocus;
			textLName3.LostFocus+=textBox_LostFocus;
			textLName4.LostFocus+=textBox_LostFocus;
			textLName5.LostFocus+=textBox_LostFocus;
			textFName1.LostFocus+=textBox_LostFocus;
			textFName2.LostFocus+=textBox_LostFocus;
			textFName3.LostFocus+=textBox_LostFocus;
			textFName4.LostFocus+=textBox_LostFocus;
			textFName5.LostFocus+=textBox_LostFocus;
			textBirthdate1.LostFocus+=textBox_LostFocus;
			textBirthdate2.LostFocus+=textBox_LostFocus;
			textBirthdate3.LostFocus+=textBox_LostFocus;
			textBirthdate4.LostFocus+=textBox_LostFocus;
			textBirthdate5.LostFocus+=textBox_LostFocus;
			textEmail1.LostFocus+=textBox_LostFocus;
			textEmail2.LostFocus+=textBox_LostFocus;
			textEmail3.LostFocus+=textBox_LostFocus;
			textEmail4.LostFocus+=textBox_LostFocus;
			textEmail5.LostFocus+=textBox_LostFocus;
			textSSN1.LostFocus+=textBox_LostFocus;
			textSSN2.LostFocus+=textBox_LostFocus;
			textSSN3.LostFocus+=textBox_LostFocus;
			textSSN4.LostFocus+=textBox_LostFocus;
			textSSN5.LostFocus+=textBox_LostFocus;
			textWirelessPhone1.LostFocus+=textBox_LostFocus;
			textWirelessPhone2.LostFocus+=textBox_LostFocus;
			textWirelessPhone3.LostFocus+=textBox_LostFocus;
			textWirelessPhone4.LostFocus+=textBox_LostFocus;
			textWirelessPhone5.LostFocus+=textBox_LostFocus;
			textHmPhone.LostFocus+=textBox_LostFocus;
			textZip.LostFocus+=textBox_LostFocus;
			textCountry.LostFocus+=textBox_LostFocus;
			textAddress.LostFocus+=textBox_LostFocus;
			textAddress2.LostFocus+=textBox_LostFocus;
			textAddrNotes.LostFocus+=textBox_LostFocus;
			textCity.LostFocus+=textBox_LostFocus;
			textPhone1.LostFocus+=textBox_LostFocus;
			textGroupName1.LostFocus+=textBox_LostFocus;
			textGroupNum1.LostFocus+=textBox_LostFocus;
			textSubscriberID1.LostFocus+=textBox_LostFocus;
			//FName & LName textchanged events
			textLName1.TextChanged+=textLName1_TextChanged;
			textLName2.TextChanged+=textLName2_TextChanged;
			textLName3.TextChanged+=textLName3_TextChanged;
			textLName4.TextChanged+=textLName4_TextChanged;
			textLName5.TextChanged+=textLName5_TextChanged;
			textFName1.TextChanged+=textFName1_TextChanged;
			textFName2.TextChanged+=textFName2_TextChanged;
			textFName3.TextChanged+=textFName3_TextChanged;
			textFName4.TextChanged+=textFName4_TextChanged;
			textFName5.TextChanged+=textFName5_TextChanged;
			//Birthdate validated
			textBirthdate1.LostFocus+=textBirthdate1_Validated;
			textBirthdate2.LostFocus+=textBirthdate2_Validated;
			textBirthdate3.LostFocus+=textBirthdate3_Validated;
			textBirthdate4.LostFocus+=textBirthdate4_Validated;
			textBirthdate5.LostFocus+=textBirthdate5_Validated;
			//InsCheckProvAutomation
			comboPriProv1.SelectionChangeCommitted+=comboPriProv1_SelectionChangeCommitted;
			comboSecProv1.SelectionChangeCommitted+=comboSecProv1_SelectionChangeCommitted;
			comboBillType1.SelectionChangeCommitted+=comboBillType1_SelectionChangeCommitted;
			comboClinic1.SelectionChangeCommitted+=comboClinic_SelectionChangeCommitted;
			comboClinic2.SelectionChangeCommitted+=comboClinic_SelectionChangeCommitted;
			comboClinic3.SelectionChangeCommitted+=comboClinic_SelectionChangeCommitted;
			comboClinic4.SelectionChangeCommitted+=comboClinic_SelectionChangeCommitted;
			comboClinic5.SelectionChangeCommitted+=comboClinic_SelectionChangeCommitted;
			//Email
			textEmail1.TextChanged+=textEmail1_TextChanged;
			//Wireless Phone
			textWirelessPhone1.TextChanged+=textWirelessPhone1_TextChanged;
			//SSN
			textSSN1.LostFocus+=textSSN_Validating;
			textSSN2.LostFocus+=textSSN_Validating;
			textSSN3.LostFocus+=textSSN_Validating;
			textSSN4.LostFocus+=textSSN_Validating;
			textSSN5.LostFocus+=textSSN_Validating;
			//Address
			textAddress.TextChanged+=textAddress_TextChanged;
			textAddress2.TextChanged+=textAddress2_TextChanged;
			textCity.TextChanged+=textCity_TextChanged;
			textState.TextChanged+=textState_TextChanged;
			textCountry.TextChanged+=textState_TextChanged;
			textZip.TextChanged+=textZip_TextChanged;
			comboZip.SelectionChangeCommitted+=comboZip_SelectionChangeCommitted;
			textZip.LostFocus+=textZip_Validating;
			textState.KeyUp+=textState_KeyUp;
			textState.LostFocus+=textState_LostFocus;
			//Employer
			textEmployer1.KeyUp+=textEmployer1_KeyUp;
			textEmployer2.KeyUp+=textEmployer2_KeyUp;
			textEmployer1.LostFocus+=TextEmployer1_LostFocus;
			textEmployer2.LostFocus+=TextEmployer2_LostFocus;
			//Carrier
			textCarrier1.KeyUp+=textCarrier1_KeyUp;
			textCarrier1.LostFocus+=textCarrier1_LostFocus;
			textCarrier2.KeyUp+=textCarrier2_KeyUp;
			textCarrier2.LostFocus+=textCarrier2_LostFocus;
			//InsPlanPick
			PreviewKeyDown+=FrmPatientAddAll_PreviewKeyDown;
			FormClosing+=FrmPatientAddAll_FormClosing;
			#endregion
		}

		private void FrmPatientAddAll_Load(object sender,EventArgs e) {
			Lang.F(this);
			labelRequiredFields.Visible=false;
			textPhone1.TextChanged+=PatientL.ValidPhone_TextChanged;
			textPhone2.TextChanged+=PatientL.ValidPhone_TextChanged;
			textHmPhone.TextChanged+=PatientL.ValidPhone_TextChanged;
			textWirelessPhone1.TextChanged+=PatientL.ValidPhone_TextChanged;
			textWirelessPhone2.TextChanged+=PatientL.ValidPhone_TextChanged;
			textWirelessPhone3.TextChanged+=PatientL.ValidPhone_TextChanged;
			textWirelessPhone4.TextChanged+=PatientL.ValidPhone_TextChanged;
			textWirelessPhone5.TextChanged+=PatientL.ValidPhone_TextChanged;
			listEmps1=new ListBox();
			listEmps1.Margin=new Thickness(groupIns1.Margin.Left+textEmployer1.Margin.Left,groupIns1.Margin.Top+textCarrier1.Margin.Top,0,0);
			listEmps1.Size=new Size(254,100);
			listEmps1.Visible=false;
			listEmps1.MouseDown+=listEmps1_Click;
			grid.Children.Add(listEmps1);
			listEmps2=new ListBox();
			listEmps2.Margin=new Thickness(groupIns2.Margin.Left+textEmployer2.Margin.Left,groupIns2.Margin.Top+textCarrier2.Margin.Top,0,0);
			listEmps2.Size=new Size(254,100);
			listEmps2.Visible=false;
			listEmps2.MouseDown+=listEmps2_Click;
			grid.Children.Add(listEmps2);
			listCars1=new ListBox();
			listCars1.Margin=new Thickness(groupIns1.Margin.Left+textCarrier1.Margin.Left,groupIns1.Margin.Top+textPhone1.Margin.Top,0,0);
			listCars1.Size=new Size(700,100);
			listCars1.Visible=false;
			listCars1.MouseDown+=listCars1_Click;
			grid.Children.Add(listCars1);
			listCars2=new ListBox();
			listCars2.Margin=new Thickness(groupIns2.Margin.Left+textCarrier2.Margin.Left,groupIns2.Margin.Top+textPhone2.Margin.Top,0,0);
			listCars2.Size=new Size(700,100);
			listCars2.Visible=false;
			listCars2.MouseDown+=listCars2_Click;
			grid.Children.Add(listCars2);
			listStates=new ListBox();
			listStates.Margin=new Thickness(textState.Margin.Left+groupBox1.Margin.Left,textZip.Margin.Top+groupBox1.Margin.Top,0,0);
			listStates.Size=new Size(textState.Width,100);
			listStates.Visible=false;
			listStates.MouseDown+=listStates_Click;
			grid.Children.Add(listStates);
			if(!Security.IsAuthorized(EnumPermType.InsPlanEdit,true)) {
				textEmployer1.ReadOnly=true;
				textEmployer2.ReadOnly=true;
				textPhone1.ReadOnly=true;
				textPhone2.ReadOnly=true;
				textCarrier1.ReadOnly=true;
				textCarrier2.ReadOnly=true;
				textGroupName1.ReadOnly=true;
				textGroupName2.ReadOnly=true;
				textGroupNum1.ReadOnly=true;
				textGroupNum2.ReadOnly=true;
			}
			textLName1.Text=LName;
			textFName1.Text=FName;
			if(Birthdate.Year<1880) {
				textBirthdate1.Text="";
			}
			else {
				textBirthdate1.Text=Birthdate.ToShortDateString();
			}
			textBirthdate1_Validated(this,null);
			SetGenderListBox(listGender1);
			SetGenderListBox(listGender2);
			SetGenderListBox(listGender3);
			SetGenderListBox(listGender4);
			SetGenderListBox(listGender5);
			listPosition1.SelectedIndex=1;
			listPosition2.SelectedIndex=1;
			if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
				comboPriProv1.Items.Add(Lang.g(this,"Select Provider"));
				comboPriProv2.Items.Add(Lang.g(this,"Select Provider"));
				comboPriProv3.Items.Add(Lang.g(this,"Select Provider"));
				comboPriProv4.Items.Add(Lang.g(this,"Select Provider"));
				comboPriProv5.Items.Add(Lang.g(this,"Select Provider"));
			}
			comboSecProv1.Items.Add(Lang.g(this,"none"));
			comboSecProv1.SelectedIndex=0;
			comboSecProv2.Items.Add(Lang.g(this,"none"));
			comboSecProv2.SelectedIndex=0;
			comboSecProv3.Items.Add(Lang.g(this,"none"));
			comboSecProv3.SelectedIndex=0;
			comboSecProv4.Items.Add(Lang.g(this,"none"));
			comboSecProv4.SelectedIndex=0;
			comboSecProv5.Items.Add(Lang.g(this,"none"));
			comboSecProv5.SelectedIndex=0;
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++){
				comboPriProv1.Items.Add(_listProviders[i].GetLongDesc());
				comboSecProv1.Items.Add(_listProviders[i].GetLongDesc());
				comboPriProv2.Items.Add(_listProviders[i].GetLongDesc());
				comboSecProv2.Items.Add(_listProviders[i].GetLongDesc());
				comboPriProv3.Items.Add(_listProviders[i].GetLongDesc());
				comboSecProv3.Items.Add(_listProviders[i].GetLongDesc());
				comboPriProv4.Items.Add(_listProviders[i].GetLongDesc());
				comboSecProv4.Items.Add(_listProviders[i].GetLongDesc());
				comboPriProv5.Items.Add(_listProviders[i].GetLongDesc());
				comboSecProv5.Items.Add(_listProviders[i].GetLongDesc());
			}
			int defaultindex=0;
			if(!PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
				if(PrefC.HasClinicsEnabled && Clinics.ClinicNum!=0) {
					defaultindex=Providers.GetIndex((Clinics.GetFirstOrDefault(x => x.ClinicNum==Clinics.ClinicNum).DefaultProv));
				}
				else {
					defaultindex=Providers.GetIndex(PrefC.GetLong(PrefName.PracticeDefaultProv));
				}
				if(defaultindex==-1) {//default provider hidden
					defaultindex=0;
				}
			}
			comboPriProv1.SelectedIndex=defaultindex;
			comboPriProv2.SelectedIndex=defaultindex;
			comboPriProv3.SelectedIndex=defaultindex;
			comboPriProv4.SelectedIndex=defaultindex;
			comboPriProv5.SelectedIndex=defaultindex;
			FillListPatStatus(listStatus1);
			FillListPatStatus(listStatus2);
			FillListPatStatus(listStatus3);
			FillListPatStatus(listStatus4);
			FillListPatStatus(listStatus5);
			FillComboBillTypes(comboBillType1,Clinics.ClinicNum);
			FillComboBillTypes(comboBillType2,Clinics.ClinicNum);
			FillComboBillTypes(comboBillType3,Clinics.ClinicNum);
			FillComboBillTypes(comboBillType4,Clinics.ClinicNum);
			FillComboBillTypes(comboBillType5,Clinics.ClinicNum);
			if(!PrefC.HasClinicsEnabled) {
				labelClinic.Visible=false;
			}
			else if(PrefC.GetBool(PrefName.ClinicAllowPatientsAtHeadquarters)){
				comboClinic1.IncludeUnassigned=true;
				comboClinic2.IncludeUnassigned=true;
				comboClinic3.IncludeUnassigned=true;
				comboClinic4.IncludeUnassigned=true;
				comboClinic5.IncludeUnassigned=true;
			}
			if(!Security.IsAuthorized(EnumPermType.RefAttachAdd,true)) {
				butClearReferralSource.IsEnabled=false;
				butReferredFrom.IsEnabled=false;
			}
			if(!PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				labelST.Text="ST";
				textCountry.Visible=false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				labelSSN.Text=Lang.g(this,"SIN");
				labelZip.Text=Lang.g(this,"Postal Code");
				labelST.Text=Lang.g(this,"Province");
				labelGroupNum1.Text=Lang.g(this,"Plan Number");
				labelGroupNum2.Text=Lang.g(this,"Plan Number");
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("GB")) {//en-GB
				labelZip.Text=Lang.g(this,"Postcode");
				labelST.Text="";//no such thing as state in GB
			}
			_listZipCodes=ZipCodes.GetDeepCopy(true);
			FillComboZip();
			ResetSubscriberLists();
			_listRequiredFields=RequiredFields.GetWhere(x => x.FieldType==RequiredFieldType.PatientInfo);
			RemoveUnnecessaryRequiredFields();
			ValidateFields();
			#region Setting Tooltip
			//Textboxes
			_toolTip.SetControlAndAction(textLName1,ToolTipSetString);
			_toolTip.SetControlAndAction(textLName2,ToolTipSetString);
			_toolTip.SetControlAndAction(textLName3,ToolTipSetString);
			_toolTip.SetControlAndAction(textLName4,ToolTipSetString);
			_toolTip.SetControlAndAction(textLName5,ToolTipSetString);
			_toolTip.SetControlAndAction(textFName1,ToolTipSetString);
			_toolTip.SetControlAndAction(textFName2,ToolTipSetString);
			_toolTip.SetControlAndAction(textFName3,ToolTipSetString);
			_toolTip.SetControlAndAction(textFName4,ToolTipSetString);
			_toolTip.SetControlAndAction(textFName5,ToolTipSetString);
			_toolTip.SetControlAndAction(textBirthdate1,ToolTipSetString);
			_toolTip.SetControlAndAction(textBirthdate2,ToolTipSetString);
			_toolTip.SetControlAndAction(textBirthdate3,ToolTipSetString);
			_toolTip.SetControlAndAction(textBirthdate4,ToolTipSetString);
			_toolTip.SetControlAndAction(textBirthdate5,ToolTipSetString);
			_toolTip.SetControlAndAction(textEmail1,ToolTipSetString);
			_toolTip.SetControlAndAction(textEmail2,ToolTipSetString);
			_toolTip.SetControlAndAction(textEmail3,ToolTipSetString);
			_toolTip.SetControlAndAction(textEmail4,ToolTipSetString);
			_toolTip.SetControlAndAction(textEmail5,ToolTipSetString);
			_toolTip.SetControlAndAction(textWirelessPhone1,ToolTipSetString);
			_toolTip.SetControlAndAction(textWirelessPhone2,ToolTipSetString);
			_toolTip.SetControlAndAction(textWirelessPhone3,ToolTipSetString);
			_toolTip.SetControlAndAction(textWirelessPhone4,ToolTipSetString);
			_toolTip.SetControlAndAction(textWirelessPhone5,ToolTipSetString);
			_toolTip.SetControlAndAction(textSSN1,ToolTipSetString);
			_toolTip.SetControlAndAction(textSSN2,ToolTipSetString);
			_toolTip.SetControlAndAction(textSSN3,ToolTipSetString);
			_toolTip.SetControlAndAction(textSSN4,ToolTipSetString);
			_toolTip.SetControlAndAction(textSSN5,ToolTipSetString);
			_toolTip.SetControlAndAction(textHmPhone,ToolTipSetString);
			_toolTip.SetControlAndAction(textAddress,ToolTipSetString);
			_toolTip.SetControlAndAction(textAddress2,ToolTipSetString);
			_toolTip.SetControlAndAction(textCity,ToolTipSetString);
			_toolTip.SetControlAndAction(textState,ToolTipSetString);
			_toolTip.SetControlAndAction(textCountry,ToolTipSetString);
			_toolTip.SetControlAndAction(textAddrNotes,ToolTipSetString);
			_toolTip.SetControlAndAction(textSubscriberID1,ToolTipSetString);
			_toolTip.SetControlAndAction(textEmployer1,ToolTipSetString);
			_toolTip.SetControlAndAction(textCarrier1,ToolTipSetString);
			_toolTip.SetControlAndAction(textPhone1,ToolTipSetString);
			_toolTip.SetControlAndAction(textGroupName1,ToolTipSetString);
			_toolTip.SetControlAndAction(textGroupNum1,ToolTipSetString);
			_toolTip.SetControlAndAction(textReferredFrom, ToolTipSetString);
			//Checkboxes
			_toolTip.SetControlAndAction(checkTextingY1,ToolTipSetString);
			_toolTip.SetControlAndAction(checkTextingN1,ToolTipSetString);
			_toolTip.SetControlAndAction(checkTextingY2,ToolTipSetString);
			_toolTip.SetControlAndAction(checkTextingN2,ToolTipSetString);
			_toolTip.SetControlAndAction(checkTextingY3,ToolTipSetString);
			_toolTip.SetControlAndAction(checkTextingN3,ToolTipSetString);
			_toolTip.SetControlAndAction(checkTextingY4,ToolTipSetString);
			_toolTip.SetControlAndAction(checkTextingN4,ToolTipSetString);
			_toolTip.SetControlAndAction(checkTextingY5,ToolTipSetString);
			_toolTip.SetControlAndAction(checkTextingN5,ToolTipSetString);
			//Comboboxes
			_toolTip.SetControlAndAction(comboPriProv1,ToolTipSetString);
			_toolTip.SetControlAndAction(comboPriProv2,ToolTipSetString);
			_toolTip.SetControlAndAction(comboPriProv3,ToolTipSetString);
			_toolTip.SetControlAndAction(comboPriProv4,ToolTipSetString);
			_toolTip.SetControlAndAction(comboPriProv5,ToolTipSetString);
			_toolTip.SetControlAndAction(comboSecProv1,ToolTipSetString);
			_toolTip.SetControlAndAction(comboSecProv2,ToolTipSetString);
			_toolTip.SetControlAndAction(comboSecProv3,ToolTipSetString);
			_toolTip.SetControlAndAction(comboSecProv4,ToolTipSetString);
			_toolTip.SetControlAndAction(comboSecProv5,ToolTipSetString);
			_toolTip.SetControlAndAction(comboSubscriber1,ToolTipSetString);
			//Listboxes
			_toolTip.SetControlAndAction(listGender1,ToolTipSetString);
			_toolTip.SetControlAndAction(listGender2,ToolTipSetString);
			_toolTip.SetControlAndAction(listGender3,ToolTipSetString);
			_toolTip.SetControlAndAction(listGender4,ToolTipSetString);
			_toolTip.SetControlAndAction(listGender5,ToolTipSetString);
			#endregion
			Plugins.HookAddCode(this,"FormPatientAddAll.FormPatientAddAll_Load_end");
		}

		private void SetGenderListBox(ListBox listBox) {
			listBox.Items.Add(PatientGender.Male.ToString(),PatientGender.Male);
			listBox.Items.Add(PatientGender.Female.ToString(),PatientGender.Female);
			listBox.Items.Add(PatientGender.Other.ToString(),PatientGender.Other);
			listBox.Items.Add(PatientGender.Unknown.ToString(),PatientGender.Unknown);
		}

		///<summary>Removes required fields that are not used in this window.</summary>
		private void RemoveUnnecessaryRequiredFields() {
			//Remove the ones that are only on the Edit Patient Information window
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.AdmitDate);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.AskArriveEarly);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.ChartNumber);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.CollegeName);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.County);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.CreditType);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.DateFirstVisit);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.DateTimeDeceased);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.DischargeDate);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.EligibilityExceptCode);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Ethnicity);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.FeeSchedule);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.GradeLevel);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Language);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MedicaidID);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MedicaidState);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MiddleInitial);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MothersMaidenFirstName);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.MothersMaidenLastName);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.PreferConfirmMethod);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.PreferContactMethod);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.PreferRecallMethod);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.PreferredName);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Race);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.ResponsibleParty);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Salutation);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Site);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.StudentStatus);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Title);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.TreatmentUrgency);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.TrophyFolder);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.Ward);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.WorkPhone);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.EmergencyName);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.EmergencyPhone);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.SexualOrientation);
			_listRequiredFields.RemoveAll(x => x.FieldName==RequiredFieldName.GenderIdentity);
		}

		//<summary>Puts an asterisk next to the label and highlights the textbox/listbox/combobox/radiobutton background for all RequiredFields that
		///have their conditions met.</summary>butok_
		private void ValidateFields() {
			_isMissingRequiredFields=false;
			bool areConditionsMet;
			if(_listRequiredFields==null) {
				return;
			}
			for(int i=0;i<_listRequiredFields.Count;i++) {
				areConditionsMet=ConditionsAreMet(_listRequiredFields[i]);
				if(areConditionsMet) {
					labelRequiredFields.Visible=true;
				}
				switch(_listRequiredFields[i].FieldName) {
					case RequiredFieldName.Address:
						SetRequiredTextBox(labelAddress,textAddress,areConditionsMet);
						break;
					case RequiredFieldName.Address2:
						SetRequiredTextBox(labelAddress2,textAddress2,areConditionsMet);
						break;
					case RequiredFieldName.AddressPhoneNotes:
						if(areConditionsMet) {
							if(!labelAddrNotes.Text.Contains("*")) {
								labelAddrNotes.Text+="*";
							}
							if(textAddrNotes.Text=="") {
								_isMissingRequiredFields=true;
								if(_isValidating) {
									SetError(textAddrNotes,Lang.g(this,"Text box cannot be blank"));
								}
							}
							else {
								SetError(textAddrNotes,"");
							}
						}
						else {
							if(labelAddrNotes.Text.Contains("*")) {
								labelAddrNotes.Text=labelAddrNotes.Text.Replace("*","");
							}
							SetError(textAddrNotes,"");
						}
						break;
					case RequiredFieldName.BillingType:
						SetRequiredComboBoxOD(labelBillType,comboBillType1,areConditionsMet,-1,"A billing type must be selected for the guarantor.");
						SetRequiredListOrComboControlNonGuarantor(labelBillType,textFName2,textLName2,comboBillType2,areConditionsMet,-1,"A billing type must be selected.");
						SetRequiredListOrComboControlNonGuarantor(labelBillType,textFName3,textLName3,comboBillType3,areConditionsMet,-1,"A billing type must be selected.");
						SetRequiredListOrComboControlNonGuarantor(labelBillType,textFName4,textLName4,comboBillType4,areConditionsMet,-1,"A billing type must be selected.");
						SetRequiredListOrComboControlNonGuarantor(labelBillType,textFName5,textLName5,comboBillType5,areConditionsMet,-1,"A billing type must be selected.");
						break;
					case RequiredFieldName.Birthdate:
						SetRequiredDate(labelBirthAge,textBirthdate1,areConditionsMet);
						SetRequiredDateNonGuarantor(labelBirthAge,textFName2,textLName2,textBirthdate2,areConditionsMet);
						SetRequiredDateNonGuarantor(labelBirthAge,textFName3,textLName3,textBirthdate3,areConditionsMet);
						SetRequiredDateNonGuarantor(labelBirthAge,textFName4,textLName4,textBirthdate4,areConditionsMet);
						SetRequiredDateNonGuarantor(labelBirthAge,textFName5,textLName5,textBirthdate5,areConditionsMet);
						break;
					case RequiredFieldName.Carrier:
						SetRequiredTextBox(labelCarrier1,textCarrier1,areConditionsMet);
						break;
					case RequiredFieldName.City:
						SetRequiredTextBox(labelCity,textCity,areConditionsMet);
						break;
					case RequiredFieldName.Clinic:
						SetRequiredComboClinicPicker(labelClinic,comboClinic1,areConditionsMet,0,"Selection cannot be 'Unassigned'.");
						SetRequiredComboClinicPicker(labelClinic,comboClinic2,areConditionsMet,0,"Selection cannot be 'Unassigned'.");
						SetRequiredComboClinicPicker(labelClinic,comboClinic3,areConditionsMet,0,"Selection cannot be 'Unassigned'.");
						SetRequiredComboClinicPicker(labelClinic,comboClinic4,areConditionsMet,0,"Selection cannot be 'Unassigned'.");
						SetRequiredComboClinicPicker(labelClinic,comboClinic5,areConditionsMet,0,"Selection cannot be 'Unassigned'.");
						break;
					case RequiredFieldName.EmailAddress:
						SetRequiredTextBox(labelEmail,textEmail1,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelEmail,textFName2,textLName2,textEmail2,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelEmail,textFName3,textLName3,textEmail3,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelEmail,textFName4,textLName4,textEmail4,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelEmail,textFName5,textLName5,textEmail5,areConditionsMet);
						break;
					case RequiredFieldName.Employer:
						SetRequiredTextBox(labelEmployer1,textEmployer1,areConditionsMet);
						break;
					case RequiredFieldName.FirstName:
						SetRequiredTextBox(labelFName,textFName1,areConditionsMet);
						if(textLName2.Text!="") {
							SetRequiredTextBox(labelFName,textFName2,areConditionsMet);
						}
						else {
							SetError(textFName2,"");
						}
						if(textLName3.Text!="") {
							SetRequiredTextBox(labelFName,textFName3,areConditionsMet);
						}
						else {
							SetError(textFName3,"");
						}
						if(textLName4.Text!="") {
							SetRequiredTextBox(labelFName,textFName4,areConditionsMet);
						}
						else {
							SetError(textFName4,"");
						}
						if(textLName5.Text!="") {
							SetRequiredTextBox(labelFName,textFName5,areConditionsMet);
						}
						else {
							SetError(textFName5,"");
						}
						break;
					case RequiredFieldName.Gender:
						string strErrorMsg=Lang.g(this,"Gender cannot be 'Unknown'.");
						int disallowedIdx=3;//Unknown (the listbox is out of order)
						SetRequiredListBoxOD(labelGenPos,listGender1,areConditionsMet,disallowedIdx,strErrorMsg);
						SetRequiredListBoxODNonGuarantor(labelGenPos,textFName2,textLName2,listGender2,areConditionsMet,disallowedIdx,strErrorMsg);
						SetRequiredListBoxODNonGuarantor(labelGenPos,textFName3,textLName3,listGender3,areConditionsMet,disallowedIdx,strErrorMsg);
						SetRequiredListBoxODNonGuarantor(labelGenPos,textFName4,textLName4,listGender4,areConditionsMet,disallowedIdx,strErrorMsg);
						SetRequiredListBoxODNonGuarantor(labelGenPos,textFName5,textLName5,listGender5,areConditionsMet,disallowedIdx,strErrorMsg);
						break;
					case RequiredFieldName.GroupName:
						SetRequiredTextBox(labelGroupName1,textGroupName1,areConditionsMet);
						break;
					case RequiredFieldName.GroupNum:
						SetRequiredTextBox(labelGroupNum1,textGroupNum1,areConditionsMet);
						break;
					case RequiredFieldName.HomePhone:
						SetRequiredTextBox(labelHmPhone,textHmPhone,areConditionsMet);
						break;
					case RequiredFieldName.InsurancePhone:
						SetRequiredTextBox(labelPhone1,textPhone1,areConditionsMet);
						break;
					case RequiredFieldName.InsuranceSubscriber:
						SetRequiredComboBoxOD(labelSubscriber1,comboSubscriber1,areConditionsMet,0,"Selection cannot be 'none'");
						break;
					case RequiredFieldName.InsuranceSubscriberID:
						SetRequiredTextBox(labelSubscriberID1,textSubscriberID1,areConditionsMet);
						break;
					case RequiredFieldName.LastName:
						SetRequiredTextBox(labelLName,textLName1,areConditionsMet);
						if(textFName2.Text!="") {
							SetRequiredTextBox(labelLName,textLName2,areConditionsMet);
						}
						else {
							SetError(textLName2,"");
						}
						if(textFName3.Text!="") {
							SetRequiredTextBox(labelLName,textLName3,areConditionsMet);
						}
						else {
							SetError(textLName3,"");
						}
						if(textFName4.Text!="") {
							SetRequiredTextBox(labelLName,textLName4,areConditionsMet);
						}
						else {
							SetError(textLName4,"");
						}
						if(textFName5.Text!="") {
							SetRequiredTextBox(labelLName,textLName5,areConditionsMet);
						}
						else {
							SetError(textLName5,"");
						}
						break;
					case RequiredFieldName.PrimaryProvider:
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							SetRequiredComboBoxOD(labelPriProv,comboPriProv1,areConditionsMet,0,"Selection cannot be 'Select Provider'");
							SetRequiredListOrComboControlNonGuarantor(labelPriProv,textFName2,textLName2,comboPriProv2,areConditionsMet,0,"Selection cannot be 'Select Provider'");
							SetRequiredListOrComboControlNonGuarantor(labelPriProv,textFName3,textLName3,comboPriProv3,areConditionsMet,0,"Selection cannot be 'Select Provider'");
							SetRequiredListOrComboControlNonGuarantor(labelPriProv,textFName4,textLName4,comboPriProv4,areConditionsMet,0,"Selection cannot be 'Select Provider'");
							SetRequiredListOrComboControlNonGuarantor(labelPriProv,textFName5,textLName5,comboPriProv5,areConditionsMet,0,"Selection cannot be 'Select Provider'");
						}
						break;
					case RequiredFieldName.ReferredFrom:
						SetRequiredTextBox(labelReferred,textReferredFrom,areConditionsMet);
						break;
					case RequiredFieldName.SecondaryProvider:
						SetRequiredComboBoxOD(labelSecProv,comboSecProv1,areConditionsMet,0,"Selection cannot be 'none'");
						SetRequiredListOrComboControlNonGuarantor(labelSecProv,textFName2,textLName2,comboSecProv2,areConditionsMet,0,"Selection cannot be 'none'");
						SetRequiredListOrComboControlNonGuarantor(labelSecProv,textFName3,textLName3,comboSecProv3,areConditionsMet,0,"Selection cannot be 'none'");
						SetRequiredListOrComboControlNonGuarantor(labelSecProv,textFName4,textLName4,comboSecProv4,areConditionsMet,0,"Selection cannot be 'none'");
						SetRequiredListOrComboControlNonGuarantor(labelSecProv,textFName5,textLName5,comboSecProv5,areConditionsMet,0,"Selection cannot be 'none'");
						break;
					case RequiredFieldName.SocialSecurityNumber:
						SetRequiredTextBox(labelSSN,textSSN1,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelSSN,textFName2,textLName2,textSSN2,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelSSN,textFName3,textLName3,textSSN3,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelSSN,textFName4,textLName4,textSSN4,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelSSN,textFName5,textLName5,textSSN5,areConditionsMet);
						break;
					case RequiredFieldName.State:
						SetRequiredTextBox(labelST,textState,areConditionsMet);
						if(textState.Text!=""	&& !StateAbbrs.IsValidAbbr(textState.Text)) {
							_isMissingRequiredFields=true;
							if(_isValidating) {
								SetError(textState,Lang.g(this,"Invalid state abbreviation"));
							}
						}
						break;
					case RequiredFieldName.TextOK:
						SetRequiredCheckBoxYN(labelTextOk,checkTextingY1,checkTextingN1,areConditionsMet,"Either Y or N must be selected.");
						SetRequiredCheckBoxNonGuarantorYN(labelTextOk,textFName2,textLName2,checkTextingY2,checkTextingN2,areConditionsMet,"Either Y or N must be selected.");
						SetRequiredCheckBoxNonGuarantorYN(labelTextOk,textFName3,textLName3,checkTextingY3,checkTextingN3,areConditionsMet,"Either Y or N must be selected.");
						SetRequiredCheckBoxNonGuarantorYN(labelTextOk,textFName4,textLName4,checkTextingY4,checkTextingN4,areConditionsMet,"Either Y or N must be selected.");
						SetRequiredCheckBoxNonGuarantorYN(labelTextOk,textFName5,textLName5,checkTextingY5,checkTextingN5,areConditionsMet,"Either Y or N must be selected.");
						break;
					case RequiredFieldName.WirelessPhone:
						SetRequiredTextBox(labelWirelessPhone,textWirelessPhone1,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelWirelessPhone,textFName2,textLName2,textWirelessPhone2,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelWirelessPhone,textFName3,textLName3,textWirelessPhone3,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelWirelessPhone,textFName4,textLName4,textWirelessPhone4,areConditionsMet);
						SetRequiredTextBoxNonGuarantor(labelWirelessPhone,textFName5,textLName5,textWirelessPhone5,areConditionsMet);
						break;
					case RequiredFieldName.Zip:
						SetRequiredTextBox(labelZip,textZip,areConditionsMet);
						break;
				}
			}
		}

		///<summary>Returns true if all the conditions for the RequiredField are met.</summary>
		private bool ConditionsAreMet(RequiredField requiredField) {
			List<RequiredFieldCondition> listRequiredFieldConditions=requiredField.ListRequiredFieldConditions;
			if(listRequiredFieldConditions.Count==0) {//This RequiredField is always required
				return true;
			}
			bool areConditionsMet=false;
			int previousFieldName=-1;
			for(int i=0;i<listRequiredFieldConditions.Count;i++) {
				if(areConditionsMet && (int)listRequiredFieldConditions[i].ConditionType==previousFieldName) {
					continue;//A condition of this type has already been met
				}
				if(!areConditionsMet && previousFieldName!=-1) {
					if((int)listRequiredFieldConditions[i].ConditionType!=previousFieldName) {
						return false;//None of the conditions of the previous type were met
					}
				}
				areConditionsMet=false;
				switch(listRequiredFieldConditions[i].ConditionType) {
					case RequiredFieldName.Birthdate://But actually using Age for calculations	
						//All Birthdate conditions will be true if any of the ages meet the conditions.
						List<TextVDate> listValidDates=new List<TextVDate>();
						listValidDates.Add(textBirthdate1);
						listValidDates.Add(textBirthdate2);
						listValidDates.Add(textBirthdate3);
						listValidDates.Add(textBirthdate4);
						listValidDates.Add(textBirthdate5);
						for(int j=0;j<listValidDates.Count;j++) {
							if(listValidDates[j].Text=="" || !listValidDates[j].IsValid()) {
								continue;
							}
							DateTime birthdate=PIn.Date(listValidDates[j].Text);
							if(birthdate>DateTime.Today) {
								birthdate=birthdate.AddYears(-100);
							}
							int ageEntered=DateTime.Today.Year-birthdate.Year;
							if(birthdate>DateTime.Today.AddYears(-ageEntered)) {
								ageEntered--;
							}
							List<RequiredFieldCondition> listRequiredFieldConditionsAge=listRequiredFieldConditions.FindAll(x => x.ConditionType==RequiredFieldName.Birthdate);
							List<bool> listAreCondsMet=new List<bool>();
							for(int k=0;k<listRequiredFieldConditionsAge.Count;k++) {
								listAreCondsMet.Add(CondOpComparer(ageEntered,listRequiredFieldConditionsAge[k].Operator,PIn.Int(listRequiredFieldConditionsAge[k].ConditionValue)));
							}
							if(listAreCondsMet.Count<2 || listRequiredFieldConditionsAge[1].ConditionRelationship==LogicalOperator.And) {
								areConditionsMet=!listAreCondsMet.Contains(false);
							}
							else {
								areConditionsMet=listAreCondsMet.Contains(true);
							}
							if(areConditionsMet) {
								break;//From the for loop
							}
						}
						break;
					case RequiredFieldName.Clinic:
						if(!PrefC.HasClinicsEnabled) {
							areConditionsMet=true;
							break;
						}
						List<ComboClinic> listComboBoxClinicPickers=new List<ComboClinic>();
						listComboBoxClinicPickers.Add(comboClinic1);
						listComboBoxClinicPickers.Add(comboClinic2);
						listComboBoxClinicPickers.Add(comboClinic3);
						listComboBoxClinicPickers.Add(comboClinic4);
						listComboBoxClinicPickers.Add(comboClinic5);
						for(int j = 0;j<listComboBoxClinicPickers.Count;j++) {
							long selectedClinicNum=listComboBoxClinicPickers[j].ClinicNumSelected;
							if(selectedClinicNum<0) {
								continue;
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.Equals
								&& PIn.Long(listRequiredFieldConditions[i].ConditionValue)==selectedClinicNum) 
							{
								areConditionsMet=true;
								break;
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.NotEquals
								&& !listRequiredFieldConditions.FindAll(x => x.ConditionType==RequiredFieldName.Clinic)
										.Any(x => x.ConditionValue==selectedClinicNum.ToString())) 
							{
								areConditionsMet=true;
								break;
							}
						}
						break;
					case RequiredFieldName.Gender:
						//All gender conditions will be true if any gender list box meets the condition.
						List<ListBox> listBoxesGender = new List<ListBox>();
						listBoxesGender.Add(listGender1);
						listBoxesGender.Add(listGender2);
						listBoxesGender.Add(listGender3);
						listBoxesGender.Add(listGender4);
						listBoxesGender.Add(listGender5);
						for(int j=0;j<listBoxesGender.Count;j++) {
							if(listBoxesGender[j].SelectedItem==null) {
							 continue;
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.Equals
								&& listRequiredFieldConditions[i].ConditionValue==listBoxesGender[j].SelectedItem.ToString()) 
							{
								areConditionsMet=true;
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.NotEquals
								&& !listRequiredFieldConditions.FindAll(x => x.ConditionType==RequiredFieldName.Gender)
										.Any(x => x.ConditionValue==listBoxesGender[j].SelectedItem.ToString()))
							{
								areConditionsMet=true;
							}
						}
						break;
					case RequiredFieldName.PrimaryProvider:
						//Conditions of type PrimaryProvider store the ProvNum as the ConditionValue.
						//All Primary Provider conditions will be true if any of the Primary Providers meet the condition.
						List<ComboBox> listComboBoxesProvider = new List<ComboBox>();
						listComboBoxesProvider.Add(comboPriProv1);
						listComboBoxesProvider.Add(comboPriProv2);
						listComboBoxesProvider.Add(comboPriProv3);
						listComboBoxesProvider.Add(comboPriProv4);
						listComboBoxesProvider.Add(comboPriProv5);
						for(int j=0;j<listComboBoxesProvider.Count;j++) {
							int provIdx=listComboBoxesProvider[j].SelectedIndex;
							if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
								provIdx--;//To account for 'Select Provider'
							}
							if(provIdx<0) {
								continue;
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.Equals
								&& PIn.Long(listRequiredFieldConditions[i].ConditionValue)==_listProviders[provIdx].ProvNum) 
							{
								areConditionsMet=true;
								break;//From the for loop
							}
							if(listRequiredFieldConditions[i].Operator==ConditionOperator.NotEquals
								&& !listRequiredFieldConditions.FindAll(x => x.ConditionType==RequiredFieldName.PrimaryProvider)
										.Any(x => x.ConditionValue==_listProviders[provIdx].ProvNum.ToString())) 
							{
								areConditionsMet=true;
								break;//From the for loop
							}
						}
						break;
					default://The field is not on this form
						areConditionsMet=true;
						break;
				}
				previousFieldName=(int)listRequiredFieldConditions[i].ConditionType;
			}
			return areConditionsMet;
		}

		/// <summary>This is a replacement for ErrorProvider. Takes in a framework element and error message, and sets a tooltip on the control with the message. Also turns the background color of it yellow if there is an error message, and switches back to white when a blank string is passed in.</summary>
		private void SetError(FrameworkElement frameworkElement,string msg){
			if(frameworkElement is ComboBox comboBox){
				comboBox.Tag=msg;
				if(msg==""){
					comboBox.ColorBack=Colors.White;
					return;
				}
				comboBox.ColorBack=Colors.Yellow;
				return;
			}
			if(frameworkElement is ListBox listBox){
				listBox.Tag=msg;
				if(msg==""){
					listBox.ColorBack=Colors.White;
					//Not normally white like most colors, this one is a grayish color since it's the highlight color
					listBox.ColorSelectedBack=Color.FromRgb(186,199,219);
					return;
				}
				//This message will only trigger if the user hits save and there is no gender selected.
				if(listBox.SelectedIndex==-1){
					listBox.ColorBack=Colors.Yellow;
					return;
				}
				//Only highlight color, keep background the same.
				listBox.ColorBack=Colors.White;
				listBox.ColorSelectedBack=Colors.Yellow;
				return;
			}
			if(frameworkElement is TextBox textBox){
				textBox.Tag=msg;
				if(msg==""){
					textBox.ColorBack=Colors.White;
					return;
				}
				textBox.ColorBack=Colors.Yellow;
				return;
			}
			if(frameworkElement is CheckBox checkBox){
				checkBox.Tag=msg;
				if(msg==""){
					checkBox.ColorBack=Colors.White;
					return;
				}
				checkBox.ColorBack=Colors.Yellow;
				return;
			}
			if(frameworkElement is TextRich textRich){
				textRich.Tag=msg;
				if(msg==""){
					textRich.ColorBack=Colors.White;
					return;
				}
				textRich.ColorBack=Colors.Yellow;
				return;
			}
			if(frameworkElement is TextVDate textVDate){
				textVDate.Tag=msg;
				if(msg==""){
					textVDate.ColorBack=Colors.White;
					return;
				}
				textVDate.ColorBack=Colors.Yellow;
				return;
			}
		}

		///<summary></summary>
		private void ToolTipSetString(FrameworkElement frameworkElement,Point point) {
			if(frameworkElement.Tag is null){
				return;
			}
			_toolTip.SetString(frameworkElement, frameworkElement.Tag.ToString());
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If it's also blank, the textbox background gets highlighted.</summary>
		private void SetRequiredTextBox(Label label,TextBox textBox,bool areConditionsMet) {
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				if(textBox.Text=="") {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						SetError(textBox,"Text box cannot be blank.");
					}
				}
				else {
					SetError(textBox,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","");
				SetError(textBox,"");
			}
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If it's also blank, the date's background gets highlighted.</summary>
		private void SetRequiredDate(Label label, TextVDate textVDate, bool areConditionsMet){
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				if(textVDate.Text=="") {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						SetError(textVDate,"Text box cannot be blank.");
					}
				}
				else {
					SetError(textVDate,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","");
				SetError(textVDate,"");
			}
		}

		private void SetRequiredTextBoxNonGuarantor(Label label,TextBox textBoxFName,TextBox textBoxLName,TextBox textBoxRequired,
			bool areConditionsMet) 
		{
			if(!string.IsNullOrWhiteSpace(textBoxFName.Text) || !string.IsNullOrWhiteSpace(textBoxLName.Text)) {
				SetRequiredTextBox(label,textBoxRequired,areConditionsMet);
			}
			else {
				SetError(textBoxRequired,"");
			}
		}

		private void SetRequiredDateNonGuarantor(Label label,TextBox textBoxFName,TextBox textBoxLName,TextVDate dateRequired,
			bool areConditionsMet) 
		{
			if(!string.IsNullOrWhiteSpace(textBoxFName.Text) || !string.IsNullOrWhiteSpace(textBoxLName.Text)) {
				SetRequiredDate(label,dateRequired,areConditionsMet);
			}
			else {
				SetError(dateRequired,"");
			}
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If the disallowedIdx is also selected, 
		///highlights the listbox background.</summary>
		private void SetRequiredListBox(Label label,ListBox listBox,bool areConditionsMet,int disallowedIdx,string errorMsg)
		{
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				if(listBox.SelectedIndex==disallowedIdx) {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						SetError(listBox,errorMsg);
					}
				}
				else {
					SetError(listBox,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","");
				SetError(listBox,"");
			} 
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If the disallowedIdx is also selected, 
		///highlights the combobox background.</summary>
		private void SetRequiredComboBoxOD(Label label,ComboBox comboBox,bool areConditionsMet,int disallowedIdx,string errorMsg)
		{
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				if(comboBox.SelectedIndex==disallowedIdx) {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						SetError(comboBox,errorMsg);
					}
				}
				else {
					SetError(comboBox,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","");
				SetError(comboBox,"");
			} 
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If the disallowedIdx is also selected, 
		///highlights the listbox background.</summary>
		private void SetRequiredListBoxOD(Label label,ListBox listBox,bool areConditionsMet,int disallowedIdx,string errorMsg) {
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				if(listBox.SelectedIndex==disallowedIdx) {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						SetError(listBox,errorMsg);
					}
				}
				//This will only trigger if the user hits save and they don't have a patient gender selected.
				else if(listBox.SelectedIndex==-1 && _isValidating){
					SetError(listBox, "Please select a patient gender");
				}
				else {
					SetError(listBox,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","");
				SetError(listBox,"");
			}
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met.</summary>
		private void SetRequiredCheckBoxYN(Label label,CheckBox checkBoxY,CheckBox checkBoxN,bool areConditionsMet,string errorMsg) {
			label.Text=labelTextOk.Text.Replace("*","");	
			if(areConditionsMet) {
				label.Text=label.Text+"*";
				if(checkBoxY.Checked==true || checkBoxN.Checked==true) {
					SetError(checkBoxY,"");
					SetError(checkBoxN,"");
				}
				else{
					_isMissingRequiredFields=true;
					if(_isValidating) {
						SetError(checkBoxY,Lang.g(this,errorMsg));
						SetError(checkBoxN,Lang.g(this,errorMsg));
					}
				}
			}
			else {
				SetError(checkBoxY,"");
				SetError(checkBoxN,"");
			}
		}

		///<summary>Puts an asterisk next to the label if the field is required and the conditions are met. If the disallowedIdx is also selected, 
		///highlights the combobox background.</summary>
		private void SetRequiredComboClinicPicker(Label label,ComboClinic comboBoxClinic,bool areConditionsMet,int disallowedClinicNum,string errorMsg){
			if(areConditionsMet) {
				label.Text=label.Text.Replace("*","")+"*";
				if(comboBoxClinic.ClinicNumSelected==disallowedClinicNum) {
					_isMissingRequiredFields=true;
					if(_isValidating) {
						SetError(comboBoxClinic,errorMsg);
					}
				}
				else {
					SetError(comboBoxClinic,"");
				}
			}
			else {
				label.Text=label.Text.Replace("*","");
				SetError(comboBoxClinic,"");
			} 
		}

		private void SetRequiredListOrComboControlNonGuarantor(Label label,TextBox textBoxFName,TextBox textBoxLName,FrameworkElement frameworkElement,
			bool areConditionsMet,int disallowedIdx,string errorMsg)
		{
			if(string.IsNullOrWhiteSpace(textBoxFName.Text) && string.IsNullOrWhiteSpace(textBoxLName.Text)) {
				SetError(frameworkElement,"");
				return;
			}
			//Either ListBox or ComboBoxOD.
			if(frameworkElement is ListBox listBox) {
				SetRequiredListBox(label,listBox,areConditionsMet,disallowedIdx,errorMsg);
			}
			else {
				SetRequiredComboBoxOD(label,(ComboBox)frameworkElement,areConditionsMet,disallowedIdx,errorMsg);
			}
		}

		///<summary>Calls the set required method that puts an asterisk next to the label if the field is required if the textbox first or last
		///name are empty when one is filled.</summary>
		private void SetRequiredListBoxODNonGuarantor(Label label,TextBox textBoxFName,TextBox textBoxLName,ListBox listBoxRequired,
			bool areConditionsMet,int disallowedIdx,string errorMsg) {
			if(!string.IsNullOrWhiteSpace(textBoxFName.Text) || !string.IsNullOrWhiteSpace(textBoxLName.Text)) {
				SetRequiredListBoxOD(label,listBoxRequired,areConditionsMet,disallowedIdx,errorMsg);
			}
			else {
				SetError(listBoxRequired,"");
			}
		}

		///<summary>Calls the set required method that puts an asterisk next to the label if the field is required and if the textbox first or last name are empty when one is filled.</summary>
		private void SetRequiredCheckBoxNonGuarantorYN(Label label,TextBox textBoxFName,TextBox textBoxLName,CheckBox checkBoxY,CheckBox checkBoxN,
			bool areConditionsMet,string errorMsg) 
		{
			if(!string.IsNullOrWhiteSpace(textBoxFName.Text) || !string.IsNullOrWhiteSpace(textBoxLName.Text)) {
				SetRequiredCheckBoxYN(label,checkBoxY,checkBoxN,areConditionsMet,errorMsg);
			}
			else {
				SetError(checkBoxY,"");
				SetError(checkBoxN,"");
			}
		}

		///<summary>Evaluates two integers using the provided operator.</summary>
		private bool CondOpComparer(int value1,ConditionOperator conditionOperator,int value2) {
			switch(conditionOperator) {
				case ConditionOperator.Equals:
					return value1==value2;
				case ConditionOperator.NotEquals:
					return value1!=value2;
				case ConditionOperator.GreaterThan:
					return value1>value2;
				case ConditionOperator.GreaterThanOrEqual:
					return value1>=value2;
				case ConditionOperator.LessThan:
					return value1<value2;
				case ConditionOperator.LessThanOrEqual:
					return value1<=value2;
			}
			return false;
		}
		
		private void textBox_LostFocus(object sender,System.EventArgs e) {
			ValidateFields();
		}
		
		private void ListBox_SelectionChangeCommitted(object sender,System.EventArgs e) {
			ValidateFields();
		}

		private void ComboBox_SelectionChangeCommitted(object sender,System.EventArgs e) {
			ValidateFields();
		}
		
		private void butAddComm_Click(object sender,EventArgs e) {
			//if there is no commlog associated with this 
			if(_commlog==null) {
				_commlog=new Commlog();
				_commlog.CommDateTime=DateTime.Now;
				_commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
				_commlog.UserNum=Security.CurUser.UserNum;
				_commlog.IsNew=true;
			}
			FrmCommItem frmCommItem=new FrmCommItem(_commlog);
			frmCommItem.ShowDialog();
			if(frmCommItem.IsDialogOK) {
				//if the commlog was deleted, clear the stored object
				if(_commlog==null || Commlogs.GetOne(_commlog.CommlogNum)==null) {
					_commlog=null;
				}
				//otherwise, update IsNew to prevent saving multiple entries
				else {
					_commlog.IsNew=false;
				}
			}
		}

		#region Names
		private void textLName1_TextChanged(object sender,EventArgs e) {
			if(textLName1.Text.Length==1){
				textLName1.Text=textLName1.Text.ToUpper();
				textLName1.SelectionStart=1;
			}
			SetLnames();
			_lnameOld1=textLName1.Text;
		}

		private void textLName2_TextChanged(object sender,EventArgs e) {
			if(textLName2.Text.Length==1){
				textLName2.Text=textLName2.Text.ToUpper();
				textLName2.SelectionStart=1;
			}
		}

		private void textLName3_TextChanged(object sender,EventArgs e) {
			if(textLName3.Text.Length==1){
				textLName3.Text=textLName3.Text.ToUpper();
				textLName3.SelectionStart=1;
			}
		}

		private void textLName4_TextChanged(object sender,EventArgs e) {
			if(textLName4.Text.Length==1){
				textLName4.Text=textLName4.Text.ToUpper();
				textLName4.SelectionStart=1;
			}
		}

		private void textLName5_TextChanged(object sender,EventArgs e) {
			if(textLName5.Text.Length==1){
				textLName5.Text=textLName5.Text.ToUpper();
				textLName5.SelectionStart=1;
			}
		}

		private void textFName1_TextChanged(object sender,EventArgs e) {
			if(textFName1.Text.Length==1){
				textFName1.Text=textFName1.Text.ToUpper();
				textFName1.SelectionStart=1;
			}
			SetLnames();
			SetEmail();
			SetWirelessPhone();
		}

		private void textFName2_TextChanged(object sender,EventArgs e) {
			if(textFName2.Text.Length==1){
				textFName2.Text=textFName2.Text.ToUpper();
				textFName2.SelectionStart=1;
			}
			SetLnames();
			SetEmail();
			SetWirelessPhone();
			_fnameOld2=textFName2.Text;
			_emailOld2=textEmail2.Text;
		}

		private void textFName3_TextChanged(object sender,EventArgs e) {
			if(textFName3.Text.Length==1){
				textFName3.Text=textFName3.Text.ToUpper();
				textFName3.SelectionStart=1;
			}
			SetLnames();
			SetEmail();
			SetWirelessPhone();
			_fnameOld3=textFName3.Text;
			_emailOld3=textEmail3.Text;
		}

		private void textFName4_TextChanged(object sender,EventArgs e) {
			if(textFName4.Text.Length==1){
				textFName4.Text=textFName4.Text.ToUpper();
				textFName4.SelectionStart=1;
			}
			SetLnames();
			SetEmail();
			SetWirelessPhone();
			_fnameOld4=textFName4.Text;
			_emailOld4=textEmail4.Text;
		}

		private void textFName5_TextChanged(object sender,EventArgs e) {
			if(textFName5.Text.Length==1){
				textFName5.Text=textFName5.Text.ToUpper();
				textFName5.SelectionStart=1;
			}
			SetLnames();
			SetEmail();
			SetWirelessPhone();
			_fnameOld5=textFName5.Text;
			_emailOld5=textEmail5.Text;
		}

		private void SetLnames() {
			SetLname(textLName2,textFName2,_fnameOld2);
			SetLname(textLName3,textFName3,_fnameOld3);
			SetLname(textLName4,textFName4,_fnameOld4);
			SetLname(textLName5,textFName5,_fnameOld5);
			ResetSubscriberLists();
		}

		private void SetLname(TextBox textLname,TextBox textFname,string fnameOld) {
			if(textLname.Text=="" && textFname.Text=="") {
				textLname.Text="";
			}
			else if(textLname.Text=="" && textFname.Text!="") {
				textLname.Text=textLName1.Text;
			}
			else if(textLname.Text==_lnameOld1 && textFname.Text=="") {
				if(fnameOld!="" && textFname.Text=="") {
					textLname.Text="";
				}
				else {
					textLname.Text=textLName1.Text;
				}
			}
			else if(textLname.Text==_lnameOld1 && textFname.Text!="") {
				textLname.Text=textLName1.Text;
			}
		}
		#endregion Names

		#region BirthdateAndAge
		private void textBirthdate1_Validated(object sender,EventArgs e) {
			if(!textBirthdate1.IsValid()){
				textAge1.Text="";
				return;
			}
			DateTime birthdate=PIn.Date(textBirthdate1.Text);
			if(birthdate>DateTime.Today){
				birthdate=birthdate.AddYears(-100);
			}
			textAge1.Text=PatientLogic.DateToAgeString(birthdate);
		}

		private void textBirthdate2_Validated(object sender,EventArgs e) {
			if(!textBirthdate2.IsValid()){
				textAge2.Text="";
				return;
			}
			DateTime birthdate=PIn.Date(textBirthdate2.Text);
			if(birthdate>DateTime.Today){
				birthdate=birthdate.AddYears(-100);
			}
			textAge2.Text=PatientLogic.DateToAgeString(birthdate);
		}

		private void textBirthdate3_Validated(object sender,EventArgs e) {
			if(!textBirthdate3.IsValid()){
				textAge3.Text="";
				return;
			}
			DateTime birthdate=PIn.Date(textBirthdate3.Text);
			if(birthdate>DateTime.Today){
				birthdate=birthdate.AddYears(-100);
			}
			textAge3.Text=PatientLogic.DateToAgeString(birthdate);
		}

		private void textBirthdate4_Validated(object sender,EventArgs e) {
			if(!textBirthdate4.IsValid()){
				textAge4.Text="";
				return;
			}
			DateTime birthdate=PIn.Date(textBirthdate4.Text);
			if(birthdate>DateTime.Today){
				birthdate=birthdate.AddYears(-100);
			}
			textAge4.Text=PatientLogic.DateToAgeString(birthdate);
		}

		private void textBirthdate5_Validated(object sender,EventArgs e) {
			if(!textBirthdate5.IsValid()){
				textAge5.Text="";
				return;
			}
			DateTime birthdate=PIn.Date(textBirthdate5.Text);
			if(birthdate>DateTime.Today){
				birthdate=birthdate.AddYears(-100);
			}
			textAge5.Text=PatientLogic.DateToAgeString(birthdate);
		}
		#endregion BirthdateAndAge

		#region InsCheckProvAutomation
		private void checkInsOne1_Click(object sender,EventArgs e) {
			if(textFName2.Text!="" && checkInsOne1.Checked==true){
				checkInsOne2.Checked=true;
			}
			else{
				checkInsOne2.Checked=false;
			}
			if(textFName3.Text!="" && checkInsOne1.Checked==true){
				checkInsOne3.Checked=true;
			}
			else{
				checkInsOne3.Checked=false;
			}
			if(textFName4.Text!="" && checkInsOne1.Checked==true){
				checkInsOne4.Checked=true;
			}
			else{
				checkInsOne4.Checked=false;
			}
			if(textFName5.Text!="" && checkInsOne1.Checked==true){
				checkInsOne5.Checked=true;
			}
			else{
				checkInsOne5.Checked=false;
			}
		}

		private void checkInsTwo1_Click(object sender,EventArgs e) {
			if(textFName2.Text!="" && checkInsTwo1.Checked==true){
				checkInsTwo2.Checked=true;
			}
			else{
				checkInsTwo2.Checked=false;
			}
			if(textFName3.Text!="" && checkInsTwo1.Checked==true){
				checkInsTwo3.Checked=true;
			}
			else{
				checkInsTwo3.Checked=false;
			}
			if(textFName4.Text!="" && checkInsTwo1.Checked==true){
				checkInsTwo4.Checked=true;
			}
			else{
				checkInsTwo4.Checked=false;
			}
			if(textFName5.Text!="" && checkInsTwo1.Checked==true){
				checkInsTwo5.Checked=true;
			}
			else{
				checkInsTwo5.Checked=false;
			}
		}

		private void comboPriProv1_SelectionChangeCommitted(object sender,EventArgs e) {
			comboPriProv2.SelectedIndex=comboPriProv1.SelectedIndex;
			comboPriProv3.SelectedIndex=comboPriProv1.SelectedIndex;
			comboPriProv4.SelectedIndex=comboPriProv1.SelectedIndex;
			comboPriProv5.SelectedIndex=comboPriProv1.SelectedIndex;
			ValidateFields();
		}

		private void comboSecProv1_SelectionChangeCommitted(object sender,EventArgs e) {
			comboSecProv2.SelectedIndex=comboSecProv1.SelectedIndex;
			comboSecProv3.SelectedIndex=comboSecProv1.SelectedIndex;
			comboSecProv4.SelectedIndex=comboSecProv1.SelectedIndex;
			comboSecProv5.SelectedIndex=comboSecProv1.SelectedIndex;
			ValidateFields();
		}

		private void comboBillType1_SelectionChangeCommitted(object sender,EventArgs e) {
			comboBillType2.SelectedIndex=comboBillType1.SelectedIndex;
			comboBillType3.SelectedIndex=comboBillType1.SelectedIndex;
			comboBillType4.SelectedIndex=comboBillType1.SelectedIndex;
			comboBillType5.SelectedIndex=comboBillType1.SelectedIndex;
			ValidateFields();
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			if(sender==comboClinic1) {
				if(comboClinic1.ClinicNumSelected>-1) {
					comboClinic2.ClinicNumSelected=comboClinic1.ClinicNumSelected;
					comboClinic3.ClinicNumSelected=comboClinic1.ClinicNumSelected;
					comboClinic4.ClinicNumSelected=comboClinic1.ClinicNumSelected;
					comboClinic5.ClinicNumSelected=comboClinic1.ClinicNumSelected;
				}
				//Also pre-select the default billing type for all patients since guarantor's clinic has changed.
				FillComboBillTypes(comboBillType1,comboClinic1.ClinicNumSelected);
				comboBillType2.SelectedIndex=comboBillType1.SelectedIndex;
				comboBillType3.SelectedIndex=comboBillType1.SelectedIndex;
				comboBillType4.SelectedIndex=comboBillType1.SelectedIndex;
				comboBillType5.SelectedIndex=comboBillType1.SelectedIndex;
			}
			else if(sender==comboClinic2) {
				FillComboBillTypes(comboBillType2,comboClinic2.ClinicNumSelected);
			}
			else if(sender==comboClinic3) {
				FillComboBillTypes(comboBillType3,comboClinic3.ClinicNumSelected);
			}
			else if(sender==comboClinic4) {
				FillComboBillTypes(comboBillType4,comboClinic4.ClinicNumSelected);
			}
			else if(sender==comboClinic5) {
				FillComboBillTypes(comboBillType5,comboClinic5.ClinicNumSelected);
			}
		}
		#endregion InsCheckProvAutomation

		#region Email
		
		private void textEmail1_TextChanged(object sender,EventArgs e) {
			SetEmail();
			_emailOld1=textEmail1.Text;
		}

		private void SetEmail() {
			if(PIn.Bool(PrefName.AddFamilyInheritsEmail.ToString())) {
				SetEmail(textEmail2,textFName2,_emailOld2);
				SetEmail(textEmail3,textFName3,_emailOld3);
				SetEmail(textEmail4,textFName4,_emailOld4);
				SetEmail(textEmail5,textFName5,_emailOld5);
			}
		}

		private void SetEmail(TextBox textEmail,TextBox textFname,string emailOld) {
			if(textEmail.Text=="" && textFname.Text=="") {
				textEmail.Text="";
			}
			else if(textEmail.Text=="" && textFname.Text!="") {
				textEmail.Text=textEmail1.Text;
			}
			else if(textEmail.Text==_emailOld1 && textFname.Text=="") {
				if(emailOld!="") {
					textEmail.Text="";
				}
				else {
					textEmail.Text=textEmail1.Text;
				}
			}
			else if(textEmail.Text==_emailOld1 && textFname.Text!="") {
				textEmail.Text=textEmail1.Text;
			}
		}
		#endregion Email

		#region Wireless Phone

		private void textWirelessPhone1_TextChanged(object sender,EventArgs e) {
			SetWirelessPhone();
			_phoneOld1=textWirelessPhone1.Text;
		}

		private void SetWirelessPhone() {
			SetWirelessPhone(textWirelessPhone2,textFName2);
			SetWirelessPhone(textWirelessPhone3,textFName3);
			SetWirelessPhone(textWirelessPhone4,textFName4);
			SetWirelessPhone(textWirelessPhone5,textFName5);
		}

		private void SetWirelessPhone(TextBox textPhone,TextBox textFname) {
			if(textFname.Text=="") {
				textPhone.Text="";
			}
			else if(textPhone.Text=="" || textPhone.Text==_phoneOld1) {
				textPhone.Text=textWirelessPhone1.Text;
			}
		}

		#endregion Wireless Phone

		#region TxtMsgOK

		private YN GetTxtMsgOK(CheckBox checkBoxY,CheckBox checkBoxN) {
			YN yNTxtMsgOk=YN.Unknown;
			if(checkBoxY.Checked==true) {
				yNTxtMsgOk=YN.Yes;
			}
			if(checkBoxN.Checked==true) {
				yNTxtMsgOk=YN.No;
			}
			return yNTxtMsgOk;
		}

		private void checkTextingY1_Click(object sender,EventArgs e) {
			if(checkTextingN1.Checked==true && checkTextingY1.Checked==true) {
				checkTextingN1.Checked=false;
			}
			if(checkTextingY1.Checked==true) {
				checkTextingY2.Checked=true;
				checkTextingN2.Checked=false;
				checkTextingY3.Checked=true;
				checkTextingN3.Checked=false;
				checkTextingY4.Checked=true;
				checkTextingN4.Checked=false;
				checkTextingY5.Checked=true;
				checkTextingN5.Checked=false;
			}
			ValidateFields();
		}

		private void checkTextingN1_Click(object sender,EventArgs e) {
			if(checkTextingN1.Checked==true && checkTextingY1.Checked==true) {
				checkTextingY1.Checked=false;
			}
			if(checkTextingN1.Checked==true) {
				checkTextingN2.Checked=true;
				checkTextingY2.Checked=false;
				checkTextingN3.Checked=true;
				checkTextingY3.Checked=false;
				checkTextingN4.Checked=true;
				checkTextingY4.Checked=false;
				checkTextingN5.Checked=true;
				checkTextingY5.Checked=false;
			}
			ValidateFields();
		}

		private void checkTextingY2_Click(object sender,EventArgs e) {
			if(checkTextingN2.Checked==true && checkTextingY2.Checked==true) {
				checkTextingN2.Checked=false;
			}
			ValidateFields();
		}

		private void checkTextingN2_Click(object sender,EventArgs e) {
			if(checkTextingN2.Checked==true && checkTextingY2.Checked==true) {
				checkTextingY2.Checked=false;
			}
			ValidateFields();
		}

		private void checkTextingY3_Click(object sender,EventArgs e) {
			if(checkTextingN3.Checked==true && checkTextingY3.Checked==true) {
				checkTextingN3.Checked=false;
			}
			ValidateFields();
		}

		private void checkTextingN3_Click(object sender,EventArgs e) {
			if(checkTextingN3.Checked==true && checkTextingY3.Checked==true) {
				checkTextingY3.Checked=false;
			}
			ValidateFields();
		}

		private void checkTextingY4_Click(object sender,EventArgs e) {
			if(checkTextingN4.Checked==true && checkTextingY4.Checked==true) {
				checkTextingN4.Checked=false;
			}
			ValidateFields();
		}

		private void checkTextingN4_Click(object sender,EventArgs e) {
			if(checkTextingN4.Checked==true && checkTextingY4.Checked==true) {
				checkTextingY4.Checked=false;
			}
			ValidateFields();
		}

		private void checkTextingY5_Click(object sender,EventArgs e) {
			if(checkTextingN5.Checked==true && checkTextingY5.Checked==true) {
				checkTextingN5.Checked=false;
			}
			ValidateFields();
		}

		private void checkTextingN5_Click(object sender,EventArgs e) {
			if(checkTextingN5.Checked==true && checkTextingY5.Checked==true) {
				checkTextingY5.Checked=false;
			}
			ValidateFields();
		}
		#endregion

		#region SSN
		private void textSSN_Validating(object sender,RoutedEventArgs e) {
			TextBox textSSN=(TextBox)sender;
			if(CultureInfo.CurrentCulture.Name != "en-US" || textSSN.Text==""){
				return;
			}
			string formattedSSN;
			if(!Patients.IsValidSSN(textSSN.Text,out formattedSSN)){
				MsgBox.Show("SSN not valid.");
				return;
			}
			textSSN.Text=formattedSSN;
		}
		
		#endregion

		#region PatientStatus and BillingTypes
		private void FillListPatStatus(ListBox listBox) {
			listBox.Items.Clear();
			listBox.Items.AddEnums<PatientStatus>();
			for(int i=listBox.Items.Count-1;i>=0;i--){//We iterate backwards here so that when the target is removed, we do not run past the loop bounds
				if(listBox.Items.GetTextShowingAt(i) == "Deleted"){
					listBox.Items.RemoveAt(i);
				}
			}
			listBox.SelectedIndex=0;
		}

		private void FillComboBillTypes(ComboBox comboBox,long clinicNum) {
			_listDefsBillingType=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			comboBox.Items.Clear();
			long defNum=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,clinicNum));
			for(int i=0;i<_listDefsBillingType.Count;i++){
				comboBox.Items.Add(_listDefsBillingType[i].ItemName);
				if(_listDefsBillingType[i].DefNum==defNum) {
					comboBox.SelectedIndex=i;
				}
			}
		}		
		#endregion

		#region Address

		private void textAddress_TextChanged(object sender, System.EventArgs e) {
			if(textAddress.Text.Length==1){
				textAddress.Text=textAddress.Text.ToUpper();
				textAddress.SelectionStart=1;
			}
		}

		private void textAddress2_TextChanged(object sender, System.EventArgs e) {
			if(textAddress2.Text.Length==1){
				textAddress2.Text=textAddress2.Text.ToUpper();
				textAddress2.SelectionStart=1;
			}
		}

		private void textCity_TextChanged(object sender, System.EventArgs e) {
			if(textCity.Text.Length==1){
				textCity.Text=textCity.Text.ToUpper();
				textCity.SelectionStart=1;
			}
		}

		private void textState_TextChanged(object sender, System.EventArgs e) {
			if(CultureInfo.CurrentCulture.Name=="en-US" //if USA or Canada, capitalize first 2 letters
				|| CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(textState.Text.Length==1 || textState.Text.Length==2){
					textState.Text=textState.Text.ToUpper();
					textState.SelectionStart=2;
				}
			}
			else{
				if(textState.Text.Length==1){
					textState.Text=textState.Text.ToUpper();
					textState.SelectionStart=1;
				}
			}
		}

		private void textZip_TextChanged(object sender, System.EventArgs e) {
			comboZip.SelectedIndex=-1;
		}

		private void comboZip_SelectionChangeCommitted(object sender, System.EventArgs e) {
			//this happens when a zipcode is selected from the combobox of frequent zips.
			//The combo box is tucked under textZip because Microsoft makes stupid controls.
			if(comboZip.SelectedIndex==-1) {
				ValidateFields();
				return;
			}
			textCity.Text=(_listZipCodes[comboZip.SelectedIndex]).City;
			textState.Text=(_listZipCodes[comboZip.SelectedIndex]).State;
			textZip.Text=(_listZipCodes[comboZip.SelectedIndex]).ZipCodeDigits;
			ValidateFields();
		}

		private void textZip_Validating(object sender, RoutedEventArgs e) {
			//fired as soon as control loses focus.
			//it's here to validate if zip is typed in to text box instead of picked from list.
			//if(textZip.Text=="" && (textCity.Text!="" || textState.Text!="")){
			//	if(MessageBox.Show(Lan.g(this,"Delete the City and State?"),"",MessageBoxButtons.OKCancel)
			//		==DialogResult.OK){
			//		textCity.Text="";
			//		textState.Text="";
			//	}	
			//	return;
			//}
			if(textZip.Text.Length<5){
				return;
			}
			if(comboZip.SelectedIndex!=-1){
				return;
			}
			//the autofill only works if both city and state are left blank
			if(textCity.Text!="" || textState.Text!=""){
				return;
			}
			List<ZipCode> listZipCodes=ZipCodes.GetALMatches(textZip.Text);
			if(listZipCodes.Count==0){
				//No match found. Must enter info for new zipcode
				ZipCode zipCode=new ZipCode();
				zipCode.ZipCodeDigits=textZip.Text;
				FrmZipCodeEdit frmZipCodeEdit=new FrmZipCodeEdit();
				frmZipCodeEdit.ZipCodeCur=zipCode;
				frmZipCodeEdit.IsNew=true;
				frmZipCodeEdit.ShowDialog();
				if(!frmZipCodeEdit.IsDialogOK){
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);//FormZipCodeEdit does not contain internal refresh
				FillComboZip();
				textCity.Text=zipCode.City;
				textState.Text=zipCode.State;
				textZip.Text=zipCode.ZipCodeDigits;
				return;
			}
			if(listZipCodes.Count==1){
				//only one match found.  Use it.
				textCity.Text=listZipCodes[0].City;
				textState.Text=listZipCodes[0].State;
				return;
			}
			//multiple matches found.  Pick one
			FrmZipSelect frmZipSelect=new FrmZipSelect();
			frmZipSelect.ShowDialog();
			FillComboZip();
			if(!frmZipSelect.IsDialogOK){
				return;
			}
			DataValid.SetInvalid(InvalidType.ZipCodes);
			textCity.Text=frmZipSelect.ZipCodeSelected.City;
			textState.Text=frmZipSelect.ZipCodeSelected.State;
			textZip.Text=frmZipSelect.ZipCodeSelected.ZipCodeDigits;
		}

		private void FillComboZip(){
			comboZip.Items.Clear();
			for(int i=0;i<_listZipCodes.Count;i++){
				comboZip.Items.Add((_listZipCodes[i]).ZipCodeDigits
					+"("+(_listZipCodes[i]).City+")");
			}
		}

		private void textState_KeyUp(object sender,KeyEventArgs e) {
			//key up is used because that way it will trigger AFTER the textBox has been changed.
			if(e.Key==Key.Return) {
				listStates.Visible=false;
				return;
			}
			if(textState.Text=="") {
				listStates.Visible=false;
				return;
			}
			if(e.Key==Key.Down) {
				if(listStates.Items.Count==0) {
					return;
				}
				if(listStates.SelectedIndex==-1) {
					listStates.SelectedIndex=0;
					textState.Text=listStates.SelectedItem.ToString();
				}
				else if(listStates.SelectedIndex==listStates.Items.Count-1) {
					listStates.SelectedIndex=-1;
					textState.Text=_stateOriginal;
				}
				else {
					listStates.SelectedIndex++;
					textState.Text=listStates.SelectedItem.ToString();
				}
				textState.SelectionStart=textState.Text.Length;
				return;
			}
			if(e.Key==Key.Up) {
				if(listStates.Items.Count==0) {
					return;
				}
				if(listStates.SelectedIndex==-1) {
					listStates.SelectedIndex=listStates.Items.Count-1;
					textState.Text=listStates.SelectedItem.ToString();
				}
				else if(listStates.SelectedIndex==0) {
					listStates.SelectedIndex=-1;
					textState.Text=_stateOriginal;
				}
				else {
					listStates.SelectedIndex--;
					textState.Text=listStates.SelectedItem.ToString();
				}
				textState.SelectionStart=textState.Text.Length;
				return;
			}
			if(textState.Text.Length==1) {
				textState.Text=textState.Text.ToUpper();
				textState.SelectionStart=1;
			}
			_stateOriginal=textState.Text;//the original text is preserved when using up and down arrows
			listStates.Items.Clear();
			List<StateAbbr> listStateAbbrsSimilar=StateAbbrs.GetSimilarAbbrs(textState.Text);
			for(int i=0;i<listStateAbbrsSimilar.Count;i++) {
				listStates.Items.Add(listStateAbbrsSimilar[i].Abbr);
			}
			double h=15.5*listStateAbbrsSimilar.Count+2;
			if(h > Height-listStates.Margin.Top) {
				h=Height-listStates.Margin.Top;
			}
			listStates.Size=new Size(textState.Width,h);
			listStates.Visible=true;
		}

		private void textState_LostFocus(object sender,System.EventArgs e) {
			listStates.Visible=false;
		}

		private void listStates_Click(object sender,System.EventArgs e) {
			textState.Text=listStates.SelectedItem.ToString();
			textState.Focus();
			textState.SelectionStart=textState.Text.Length;
			listStates.Visible=false;
		}
		#endregion Address

		#region Referral
		private void butReferredFrom_Click(object sender,EventArgs e) {
			Referral referral=new Referral();
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Is the referral source an existing patient?")) {//patient referral
				FrmPatientSelect frmPatientSelect=new FrmPatientSelect();
				frmPatientSelect.ShowDialog();
				if(frmPatientSelect.IsDialogCancel) {
					return;
				}
				referral.PatNum=frmPatientSelect.PatNumSelected;
				bool isReferralNew=true;
				Referral referralMatch=Referrals.GetFirstOrDefault(x => x.PatNum==frmPatientSelect.PatNumSelected);
				if(referralMatch!=null) {
					referral=referralMatch;
					isReferralNew=false;
				}
				FrmReferralEdit frmReferralEdit=new FrmReferralEdit(referral);//the ReferralNum must be added here
				frmReferralEdit.IsNew=isReferralNew;
				frmReferralEdit.ShowDialog();
				if(!frmReferralEdit.IsDialogOK) {
					return;
				}
				referral=frmReferralEdit.ReferralCur;//not needed, but it makes it clear that we are editing the ref in FormRefEdit
				_referral=referral;
				FillReferredFrom();
				return;
			}
			//not a patient referral, must be a doctor or marketing/other so show the referral select window with doctor and other check boxes checked
			FrmReferralSelect frmReferralSelect=new FrmReferralSelect();
			frmReferralSelect.IsSelectionMode=true;
			frmReferralSelect.IsShowPat=false;
			frmReferralSelect.ShowDialog();
			if(frmReferralSelect.IsDialogCancel) {
				if(_referral!=null && _referral.ReferralNum>0) {
					//_refCur.ReferralNum could be invalid if user deleted from FormReferralSelect, _refCur will be null
					_referral=Referrals.GetFromList(_referral.ReferralNum);
					FillReferredFrom();//may have edited a referral and then cancelled attaching to the patient, refill the text box to reflect any changes
				}
				return;
			}
			referral=frmReferralSelect.ReferralSelected;
			_referral=referral;
			FillReferredFrom();
		}

		private void butClearReferralSource_Click(object sender,EventArgs e) {
			_referral=null;
			textReferredFrom.Text="";
			ValidateFields();
		}

		///<summary>Fills the Referred From text box with the name and referral type from the private classwide variable _refCur.</summary>
		private void FillReferredFrom() {
			string firstRefNameTypeAbbr="";
			string firstRefType="";
			string firstRefFullName="";
			if(_referral==null) {
				textReferredFrom.Text="";
				ValidateFields();
				return;
			}
			firstRefFullName=Referrals.GetNameLF(_referral.ReferralNum);
			if(_referral.PatNum>0) {
				firstRefType=" (patient)";
			}
			else if(_referral.IsDoctor) {
				firstRefType=" (doctor)";
			}
			firstRefNameTypeAbbr=firstRefFullName;
			for(int i=1;i<firstRefFullName.Length+1;i++) {//i is used as the length to substring, not an index, so i<firstRefName.Length+1 is safe
				Graphics g=Graphics.MeasureBegin();
				if(g.MeasureString(firstRefFullName.Substring(0,i)+firstRefType).Width<textReferredFrom.Width) {
					continue;
				}
				firstRefNameTypeAbbr=firstRefFullName.Substring(0,i-1);
				break;
			}
			firstRefNameTypeAbbr+=firstRefType;//firstRefType could be blank, but it will show regardless of the length of firstRefName
			//Example: Schmidt, John Jacob Jingleheimer, DDS (doctor) (+5 more) 
			//might be shortened to : Schmidt, John Jaco (doctor) (+5 more) 
			textReferredFrom.Text=firstRefNameTypeAbbr;//text box might be something like: Schmidt, John Jaco (doctor) (+5 more)
			ValidateFields();
		}
		#endregion Referral

		#region Subscriber
		///<summary>Resets the text for each of the six options in the dropdown.  Does this without changing the selected index.</summary>
		private void ResetSubscriberLists(){
			int selectedIndex1=comboSubscriber1.SelectedIndex;
			int selectedIndex2=comboSubscriber2.SelectedIndex;
			comboSubscriber1.Items.Clear();
			comboSubscriber2.Items.Clear();
			comboSubscriber1.Items.Add(Lang.g(this,"none"));
			comboSubscriber2.Items.Add(Lang.g(this,"none"));
			string str;
			for(int i=0;i<5;i++){
				str=(i+1).ToString()+" - ";
				switch(i){
					case 0:
						str+=textLName1.Text+", "+textFName1.Text;
						break;
					case 1:
						str+=textLName2.Text+", "+textFName2.Text;
						break;
					case 2:
						str+=textLName3.Text+", "+textFName3.Text;
						break;
					case 3:
						str+=textLName4.Text+", "+textFName4.Text;
						break;
					case 4:
						str+=textLName5.Text+", "+textFName5.Text;
						break;
				}
				comboSubscriber1.Items.Add(str);
				comboSubscriber2.Items.Add(str);
			}
			if(selectedIndex1==-1){
				comboSubscriber1.SelectedIndex=0;
			}
			else{
				comboSubscriber1.SelectedIndex=selectedIndex1;
			}
			if(selectedIndex2==-1){
				comboSubscriber2.SelectedIndex=0;
			}
			else{
				comboSubscriber2.SelectedIndex=selectedIndex2;
			}
		}
		#endregion Subscriber

		#region Employer
		private void textEmployer1_KeyUp(object sender,KeyEventArgs e) {
			//key up is used because that way it will trigger AFTER the textBox has been changed.
			if(!Security.IsAuthorized(EnumPermType.InsPlanEdit,true)) {
				return;
			}
			if(e.Key==Key.Return) {
				listEmps1.Visible=false;
				textCarrier1.Focus();
				return;
			}
			if(textEmployer1.Text=="") {
				listEmps1.Visible=false;
				return;
			}
			if(e.Key==Key.Down) {
				if(listEmps1.Items.Count==0) {
					return;
				}
				if(listEmps1.SelectedIndex==-1) {
					listEmps1.SelectedIndex=0;
					textEmployer1.Text=listEmps1.SelectedItem.ToString();
				}
				else if(listEmps1.SelectedIndex==listEmps1.Items.Count-1) {
					listEmps1.SelectedIndex=-1;
					textEmployer1.Text=_empOriginal1;
				}
				else {
					listEmps1.SelectedIndex++;
					textEmployer1.Text=listEmps1.SelectedItem.ToString();
				}
				textEmployer1.SelectionStart=textEmployer1.Text.Length;
				return;
			}
			if(e.Key==Key.Up) {
				if(listEmps1.Items.Count==0) {
					return;
				}
				if(listEmps1.SelectedIndex==-1) {
					listEmps1.SelectedIndex=listEmps1.Items.Count-1;
					textEmployer1.Text=listEmps1.SelectedItem.ToString();
				}
				else if(listEmps1.SelectedIndex==0) {
					listEmps1.SelectedIndex=-1;
					textEmployer1.Text=_empOriginal1;
				}
				else {
					listEmps1.SelectedIndex--;
					textEmployer1.Text=listEmps1.SelectedItem.ToString();
				}
				textEmployer1.SelectionStart=textEmployer1.Text.Length;
				return;
			}
			if(textEmployer1.Text.Length==1) {
				textEmployer1.Text=textEmployer1.Text.ToUpper();
				textEmployer1.SelectionStart=1;
			}
			_empOriginal1=textEmployer1.Text;//the original text is preserved when using up and down arrows
			listEmps1.Items.Clear();
			List<Employer> listEmployersSimilar=Employers.GetSimilarNames(textEmployer1.Text);
			for(int i=0;i<listEmployersSimilar.Count;i++) {
				listEmps1.Items.Add(listEmployersSimilar[i].EmpName);
			}
			double h=14.5*listEmployersSimilar.Count+5;
			if(h > Height-listEmps1.Margin.Top){
				h=Height-listEmps1.Margin.Top;
			}
			listEmps1.Size=new Size(231,h);
			listEmps1.Visible=true;
		}

		private void TextEmployer1_LostFocus(object sender,RoutedEventArgs e) {
			listEmps1.Visible=false;
		}

		private void listEmps1_Click(object sender,System.EventArgs e) {
			textEmployer1.Text=listEmps1.SelectedItem.ToString();
			textEmployer1.Focus();
			textEmployer1.SelectionStart=textEmployer1.Text.Length;
			listEmps1.Visible=false;
		}

		private void textEmployer2_KeyUp(object sender,KeyEventArgs e) {
			//key up is used because that way it will trigger AFTER the textBox has been changed.
			if(!Security.IsAuthorized(EnumPermType.InsPlanEdit,true)) {
				return;
			}
			if(e.Key==Key.Return) {
				listEmps2.Visible=false;
				textCarrier2.Focus();
				return;
			}
			if(textEmployer2.Text=="") {
				listEmps2.Visible=false;
				return;
			}
			if(e.Key==Key.Down) {
				if(listEmps2.Items.Count==0) {
					return;
				}
				if(listEmps2.SelectedIndex==-1) {
					listEmps2.SelectedIndex=0;
					textEmployer2.Text=listEmps2.SelectedItem.ToString();
				}
				else if(listEmps2.SelectedIndex==listEmps2.Items.Count-1) {
					listEmps2.SelectedIndex=-1;
					textEmployer2.Text=_empOriginal2;
				}
				else {
					listEmps2.SelectedIndex++;
					textEmployer2.Text=listEmps2.SelectedItem.ToString();
				}
				textEmployer2.SelectionStart=textEmployer2.Text.Length;
				return;
			}
			if(e.Key==Key.Up) {
				if(listEmps2.Items.Count==0) {
					return;
				}
				if(listEmps2.SelectedIndex==-1) {
					listEmps2.SelectedIndex=listEmps2.Items.Count-1;
					textEmployer2.Text=listEmps2.SelectedItem.ToString();
				}
				else if(listEmps2.SelectedIndex==0) {
					listEmps2.SelectedIndex=-1;
					textEmployer2.Text=_empOriginal2;
				}
				else {
					listEmps2.SelectedIndex--;
					textEmployer2.Text=listEmps2.SelectedItem.ToString();
				}
				textEmployer2.SelectionStart=textEmployer2.Text.Length;
				return;
			}
			if(textEmployer2.Text.Length==1) {
				textEmployer2.Text=textEmployer2.Text.ToUpper();
				textEmployer2.SelectionStart=1;
			}
			_empOriginal2=textEmployer2.Text;//the original text is preserved when using up and down arrows
			listEmps2.Items.Clear();
			List<Employer> listEmployersSimilar2=Employers.GetSimilarNames(textEmployer2.Text);
			for(int i=0;i<listEmployersSimilar2.Count;i++) {
				listEmps2.Items.Add(listEmployersSimilar2[i].EmpName);
			}
			double h=14.5*listEmployersSimilar2.Count+5;
			if(h > Height-listEmps2.Margin.Top){
				h=Height-listEmps2.Margin.Top;
			}
			if(h>(RenderSize.Height-listEmps2.Margin.Top)){//if the height will spill over, shrink the listbox to the edge of the form
				h=(RenderSize.Height-listEmps2.Margin.Top)-2;
			}
			listEmps2.Size=new Size(231,h);
			listEmps2.Visible=true;
		}

		private void TextEmployer2_LostFocus(object sender,RoutedEventArgs e) {
			listEmps2.Visible=false;
		}

		private void listEmps2_Click(object sender,System.EventArgs e) {
			textEmployer2.Text=listEmps2.SelectedItem.ToString();
			textEmployer2.Focus();
			textEmployer2.SelectionStart=textEmployer2.Text.Length;
			listEmps2.Visible=false;
		}

		#endregion Employer

		#region Carrier
		///<summary>Fills the carrier fields on the form based on the specified carrierNum.</summary>
		private void FillCarrier1(long carrierNum) {
			_carrierSelected1=Carriers.GetCarrier(carrierNum);
			textCarrier1.Text=_carrierSelected1.CarrierName;
			textPhone1.Text=_carrierSelected1.Phone;
		}

		private void textCarrier1_KeyUp(object sender,KeyEventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.InsPlanEdit,true)) {
				return;
			}
			if(e.Key==Key.Return) {
				if(listCars1.SelectedIndex==-1) {
					textPhone1.Focus();
				}
				else {
					FillCarrier1(_listCarriersSimilar1[listCars1.SelectedIndex].CarrierNum);
					textCarrier1.Focus();
					textCarrier1.SelectionStart=textCarrier1.Text.Length;
				}
				listCars1.Visible=false;
				return;
			}
			if(textCarrier1.Text=="") {
				listCars1.Visible=false;
				return;
			}
			if(e.Key==Key.Down) {
				if(listCars1.Items.Count==0) {
					return;
				}
				if(listCars1.SelectedIndex==-1) {
					listCars1.SelectedIndex=0;
					textCarrier1.Text=_listCarriersSimilar1[listCars1.SelectedIndex].CarrierName;
				}
				else if(listCars1.SelectedIndex==listCars1.Items.Count-1) {
					listCars1.SelectedIndex=-1;
					textCarrier1.Text=_carOriginal1;
				}
				else {
					listCars1.SelectedIndex++;
					textCarrier1.Text=_listCarriersSimilar1[listCars1.SelectedIndex].CarrierName;
				}
				textCarrier1.SelectionStart=textCarrier1.Text.Length;
				return;
			}
			if(e.Key==Key.Up) {
				if(listCars1.Items.Count==0) {
					return;
				}
				if(listCars1.SelectedIndex==-1) {
					listCars1.SelectedIndex=listCars1.Items.Count-1;
					textCarrier1.Text=_listCarriersSimilar1[listCars1.SelectedIndex].CarrierName;
				}
				else if(listCars1.SelectedIndex==0) {
					listCars1.SelectedIndex=-1;
					textCarrier1.Text=_carOriginal1;
				}
				else {
					listCars1.SelectedIndex--;
					textCarrier1.Text=_listCarriersSimilar1[listCars1.SelectedIndex].CarrierName;
				}
				textCarrier1.SelectionStart=textCarrier1.Text.Length;
				return;
			}
			if(textCarrier1.Text.Length==1) {
				textCarrier1.Text=textCarrier1.Text.ToUpper();
				textCarrier1.SelectionStart=1;
			}
			_carOriginal1=textCarrier1.Text;//the original text is preserved when using up and down arrows
			listCars1.Items.Clear();
			_listCarriersSimilar1=Carriers.GetSimilarNames(textCarrier1.Text);
			for(int i=0;i<_listCarriersSimilar1.Count;i++) {
				string carrier=_listCarriersSimilar1[i].CarrierName;
				if(!_listCarriersSimilar1[i].Phone.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar1[i].Phone);
				}
				if(!_listCarriersSimilar1[i].Address.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar1[i].Address);
				}
				if(!_listCarriersSimilar1[i].Address2.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar1[i].Address2);
				}
				if(!_listCarriersSimilar1[i].City.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar1[i].City);
				}
				if(!_listCarriersSimilar1[i].State.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar1[i].State);
				}
				if(!_listCarriersSimilar1[i].Zip.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar1[i].Zip);
				}
				listCars1.Items.Add(carrier);
			}
			double h=14.5*_listCarriersSimilar1.Count+5;
			if(h > Height-listCars1.Margin.Top){//if the height will spill over, shrink the listbox to the edge of the form
				h=Height-listCars1.Margin.Top;
			}
			double width=listCars1.Width;
			if(width>(RenderSize.Width-listCars1.Margin.Left)){//if the width will spill over, shrink the listbox to the edge of the form
				width=(RenderSize.Width-listCars1.Margin.Left)-14;
			}
			listCars1.Size=new Size(width,h);
			listCars1.Visible=true;
		}

		private void textCarrier1_LostFocus(object sender,RoutedEventArgs e) {
			listCars1.Visible=false;
		}

		private void listCars1_Click(object sender,System.EventArgs e) {
			FillCarrier1(_listCarriersSimilar1[listCars1.SelectedIndex].CarrierNum);
			textCarrier1.Focus();
			textCarrier1.SelectionStart=textCarrier1.Text.Length;
			listCars1.Visible=false;
		}

		///<summary>Fills the carrier fields on the form based on the specified carrierNum.</summary>
		private void FillCarrier2(long carrierNum) {
			_carrierSelected2=Carriers.GetCarrier(carrierNum);
			textCarrier2.Text=_carrierSelected2.CarrierName;
			textPhone2.Text=_carrierSelected2.Phone;
		}

		private void textCarrier2_KeyUp(object sender,KeyEventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.InsPlanEdit,true)) {
				return;
			}
			if(e.Key==Key.Return) {
				if(listCars2.SelectedIndex==-1) {
					textPhone2.Focus();
				}
				else {
					FillCarrier2(_listCarriersSimilar2[listCars2.SelectedIndex].CarrierNum);
					textCarrier2.Focus();
					textCarrier2.SelectionStart=textCarrier2.Text.Length;
				}
				listCars2.Visible=false;
				return;
			}
			if(textCarrier2.Text=="") {
				listCars2.Visible=false;
				return;
			}
			if(e.Key==Key.Down) {
				if(listCars2.Items.Count==0) {
					return;
				}
				if(listCars2.SelectedIndex==-1) {
					listCars2.SelectedIndex=0;
					textCarrier2.Text=_listCarriersSimilar2[listCars2.SelectedIndex].CarrierName;
				}
				else if(listCars2.SelectedIndex==listCars2.Items.Count-1) {
					listCars2.SelectedIndex=-1;
					textCarrier2.Text=_carOriginal2;
				}
				else {
					listCars2.SelectedIndex++;
					textCarrier2.Text=_listCarriersSimilar2[listCars2.SelectedIndex].CarrierName;
				}
				textCarrier2.SelectionStart=textCarrier2.Text.Length;
				return;
			}
			if(e.Key==Key.Up) {
				if(listCars2.Items.Count==0) {
					return;
				}
				if(listCars2.SelectedIndex==-1) {
					listCars2.SelectedIndex=listCars2.Items.Count-1;
					textCarrier2.Text=_listCarriersSimilar2[listCars2.SelectedIndex].CarrierName;
				}
				else if(listCars2.SelectedIndex==0) {
					listCars2.SelectedIndex=-1;
					textCarrier2.Text=_carOriginal2;
				}
				else {
					listCars2.SelectedIndex--;
					textCarrier2.Text=_listCarriersSimilar2[listCars2.SelectedIndex].CarrierName;
				}
				textCarrier2.SelectionStart=textCarrier2.Text.Length;
				return;
			}
			if(textCarrier2.Text.Length==1) {
				textCarrier2.Text=textCarrier2.Text.ToUpper();
				textCarrier2.SelectionStart=1;
			}
			_carOriginal2=textCarrier2.Text;//the original text is preserved when using up and down arrows
			listCars2.Items.Clear();
			_listCarriersSimilar2=Carriers.GetSimilarNames(textCarrier2.Text);
			for(int i=0;i<_listCarriersSimilar2.Count;i++) {
				string carrier=_listCarriersSimilar2[i].CarrierName;
				if(!_listCarriersSimilar2[i].Phone.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar2[i].Phone);
				}
				if(!_listCarriersSimilar2[i].Address.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar2[i].Address);
				}
				if(!_listCarriersSimilar2[i].Address2.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar2[i].Address2);
				}
				if(!_listCarriersSimilar2[i].City.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar2[i].City);
				}
				if(!_listCarriersSimilar2[i].State.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar2[i].State);
				}
				if(!_listCarriersSimilar2[i].Zip.IsNullOrEmpty()){
					carrier+=(", "+_listCarriersSimilar2[i].Zip);
				}
				listCars2.Items.Add(carrier);
			}
			double h=14.5*_listCarriersSimilar2.Count+5;
			if(h > Height-listCars2.Margin.Top){
				h=Height-listCars2.Margin.Top;
			}
			if(h>(RenderSize.Height-listCars2.Margin.Top)){//if the height will spill over, shrink the listbox to the edge of the form
				h=(RenderSize.Height-listCars2.Margin.Top)-2;
			}
			double width=listCars2.Width;
			if(width>(RenderSize.Width-listCars2.Margin.Left)){//if the width will spill over, shrink the listbox to the edge of the form
				width=(RenderSize.Width-listCars2.Margin.Left)-14;
			}
			listCars2.Size=new Size(width,h);
			listCars2.Visible=true;
		}

		private void textCarrier2_LostFocus(object sender,RoutedEventArgs e) {
			listCars2.Visible=false;
		}

		private void listCars2_Click(object sender,System.EventArgs e) {
			FillCarrier2(_listCarriersSimilar2[listCars2.SelectedIndex].CarrierNum);
			textCarrier2.Focus();
			textCarrier2.SelectionStart=textCarrier2.Text.Length;
			listCars2.Visible=false;
		}
		#endregion Carrier

		#region InsPlanPick
		private void butPick1_Click(object sender,EventArgs e) {
			FrmInsPlanSelect frmInsPlanSelect=new FrmInsPlanSelect();
			frmInsPlanSelect.empText=textEmployer1.Text;
			frmInsPlanSelect.carrierText=textCarrier1.Text;
			frmInsPlanSelect.ShowDialog();
			if(!frmInsPlanSelect.IsDialogOK) {
				return;
			}
			_insPlanSelected1=frmInsPlanSelect.InsPlanSelected.Copy();
			//Non-synched fields:
			//selectedPlan1.SubscriberID=textSubscriberID.Text;//later
			//selectedPlan1.DateEffective=DateTime.MinValue;
			//selectedPlan1.DateTerm=DateTime.MinValue;
			//PlanCur.ReleaseInfo=checkRelease.Checked;
			//PlanCur.AssignBen=checkAssign.Checked;
			//PlanCur.SubscNote=textSubscNote.Text;
			//Benefits will be created when click OK.
			textEmployer1.Text=Employers.GetName(_insPlanSelected1.EmployerNum);
			FillCarrier1(_insPlanSelected1.CarrierNum);
			textGroupName1.Text=_insPlanSelected1.GroupName;
			textGroupNum1.Text=_insPlanSelected1.GroupNum;
			ValidateFields();
		}

		private void butPick2_Click(object sender,EventArgs e) {
			FrmInsPlanSelect frmInsPlanSelect=new FrmInsPlanSelect();
			frmInsPlanSelect.empText=textEmployer2.Text;
			frmInsPlanSelect.carrierText=textCarrier2.Text;
			frmInsPlanSelect.ShowDialog();
			if(!frmInsPlanSelect.IsDialogOK) {
				return;
			}
			_insPlanSelected2=frmInsPlanSelect.InsPlanSelected.Copy();
			//Non-synched fields:
			//selectedPlan2.SubscriberID=textSubscriberID.Text;//later
			//selectedPlan2.DateEffective=DateTime.MinValue;
			//selectedPlan2.DateTerm=DateTime.MinValue;
			//PlanCur.ReleaseInfo=checkRelease.Checked;
			//PlanCur.AssignBen=checkAssign.Checked;
			//PlanCur.SubscNote=textSubscNote.Text;
			//Benefits will be created when click OK.
			textEmployer2.Text=Employers.GetName(_insPlanSelected2.EmployerNum);
			FillCarrier2(_insPlanSelected2.CarrierNum);
			textGroupName2.Text=_insPlanSelected2.GroupName;
			textGroupNum2.Text=_insPlanSelected2.GroupNum;
		}
		#endregion InsPlanPick
		
		///<summary>Adds a new patient to the passed-in list based on passed-in strings.</summary>
		private void AddPatToList(string lname, string fname,string birthday,long clinicNum,List<Patient> listPatients) {
			if(lname=="" && fname=="") { //validation should prevent this from happening, but just in case.
				return; //dont add patient to list
			}
			Patient patient=new Patient();
			patient.LName=lname;
			patient.FName=fname;
			patient.ClinicNum=clinicNum;
			if(birthday!=""){
				patient.Birthdate=PIn.Date(birthday);
			}
			listPatients.Add(patient);
		}

		///<summary>Gets the clinic num from the corresponding comboClinic using the passed in index, which matches the order of the Patients in the form.</summary>
		private long getClinicNum(long index) {
			long clinicNum;
			switch(index) {
				case 0://guarantor
					clinicNum=comboClinic1.ClinicNumSelected;
					break;
				case 1:
					clinicNum=comboClinic2.ClinicNumSelected;
					break;
				case 2:
					clinicNum=comboClinic3.ClinicNumSelected;
					break;
				case 3:
					clinicNum=comboClinic4.ClinicNumSelected;
					break;
				case 4:
					clinicNum=comboClinic5.ClinicNumSelected;
					break;
				default:
					clinicNum=comboClinic1.ClinicNumSelected;
					break;
			}
			return clinicNum;
		}
		
		private void FrmPatientAddAll_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)){
				butSave_Click(this,new EventArgs());
			}
			if(butAddComm.IsAltKey(Key.M,e)){
				butAddComm_Click(this, new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(Plugins.HookMethod(this,"FormPatientAddAll.butOK_Click_start")) {
				return;
			}
			#region Validation		
			if(!textBirthdate1.IsValid()
				|| !textBirthdate2.IsValid()
				|| !textBirthdate3.IsValid()
				|| !textBirthdate4.IsValid()
				|| !textBirthdate5.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			//no validation on birthdate reasonableness.
			if(textLName1.Text=="" || textFName1.Text==""){
				MsgBox.Show(this,"Guarantor name must be entered.");
				return;
			}
			if(PrefC.HasClinicsEnabled
				&& (comboClinic1.IsNothingSelected || comboClinic2.IsNothingSelected
				|| comboClinic3.IsNothingSelected ||comboClinic4.IsNothingSelected
				|| comboClinic5.IsNothingSelected)) 
			{
				MsgBox.Show(this,"Valid clinic must be selected.");
				return;
			}
			#region Validate Insurance Subscribers
			if((comboSubscriber1.SelectedIndex==2 || comboSubscriber2.SelectedIndex==2) && (textFName2.Text=="" || textLName2.Text=="")) {
				MsgBox.Show(this,"Subscriber must have name entered.");
				return;
			}
			if((comboSubscriber1.SelectedIndex==3 || comboSubscriber2.SelectedIndex==3) && (textFName3.Text=="" || textLName3.Text=="")) {
				MsgBox.Show(this,"Subscriber must have name entered.");
				return;
			}
			if((comboSubscriber1.SelectedIndex==4 || comboSubscriber2.SelectedIndex==4) && (textFName4.Text=="" || textLName4.Text=="")) {
				MsgBox.Show(this,"Subscriber must have name entered.");
				return;
			}
			if((comboSubscriber1.SelectedIndex==5 || comboSubscriber2.SelectedIndex==5) && (textFName5.Text=="" || textLName5.Text=="")) {
				MsgBox.Show(this,"Subscriber must have name entered.");
				return;
			}
			#endregion Validate Insurance Subscribers
			#region Validate Insurance Plans
			bool isInsComplete1=false;
			bool isInsComplete2=false;
			if(comboSubscriber1.SelectedIndex>0
				&& textSubscriberID1.Text!=""
				&& textCarrier1.Text!="")
			{
				isInsComplete1=true;
			}
			if(comboSubscriber2.SelectedIndex>0
				&& textSubscriberID2.Text!=""
				&& textCarrier2.Text!="")
			{
				isInsComplete2=true;
			}
			//test for insurance having only some of the critical fields filled in
			if(comboSubscriber1.SelectedIndex>0
				|| textSubscriberID1.Text!=""
				|| textCarrier1.Text!="")
			{
				if(!isInsComplete1) {
					MsgBox.Show(this,"Subscriber, Subscriber ID, and Carrier are all required fields if adding insurance.");
					return;
				}
			}
			if(comboSubscriber2.SelectedIndex>0
				|| textSubscriberID2.Text!=""
				|| textCarrier2.Text!="")
			{
				if(!isInsComplete2) {
					MsgBox.Show(this,"Subscriber, Subscriber ID, and Carrier are all required fields if adding insurance.");
					return;
				}
			}
			if(checkInsOne1.Checked==true
				|| checkInsOne2.Checked==true
				|| checkInsOne3.Checked==true
				|| checkInsOne4.Checked==true
				|| checkInsOne5.Checked==true)
			{
				if(!isInsComplete1) {
					MsgBox.Show(this,"Subscriber, Subscriber ID, and Carrier are all required fields if adding insurance.");
					return;
				}
			}
			if(checkInsTwo1.Checked==true
				|| checkInsTwo2.Checked==true
				|| checkInsTwo3.Checked==true
				|| checkInsTwo4.Checked==true
				|| checkInsTwo5.Checked==true)
			{
				if(!isInsComplete2) {
					MsgBox.Show(this,"Subscriber, Subscriber ID, and Carrier are all required fields if adding insurance.");
					return;
				}
			}
			#endregion Validate Insurance Plans
			#region Validate Insurance Subscriptions
			if(isInsComplete1) {
				if(checkInsOne1.Checked==false
					&& checkInsOne2.Checked==false
					&& checkInsOne3.Checked==false
					&& checkInsOne4.Checked==false
					&& checkInsOne5.Checked==false)
				{
					MsgBox.Show(this,"Insurance information has been filled in, but has not been assigned to any patients.");
					return;
				}
				if(checkInsOne1.Checked==true && (textLName1.Text=="" || textFName1.Text=="")//Insurance1 assigned to invalid patient1
					|| checkInsOne2.Checked==true && (textLName2.Text=="" || textFName2.Text=="")//Insurance1 assigned to invalid patient2
					|| checkInsOne3.Checked==true && (textLName3.Text=="" || textFName3.Text=="")//Insurance1 assigned to invalid patient3
					|| checkInsOne4.Checked==true && (textLName4.Text=="" || textFName4.Text=="")//Insurance1 assigned to invalid patient4
					|| checkInsOne5.Checked==true && (textLName5.Text=="" || textFName5.Text=="")) //Insurance1 assigned to invalid patient5
				{
					MsgBox.Show(this,"Insurance information 1 has been filled in, but has been assigned to a patient with no name.");
					return;
				}
			}
			if(isInsComplete2) {
				if(checkInsTwo1.Checked==false
					&& checkInsTwo2.Checked==false
					&& checkInsTwo3.Checked==false
					&& checkInsTwo4.Checked==false
					&& checkInsTwo5.Checked==false) {
					MsgBox.Show(this,"Insurance information 2 has been filled in, but has not been assigned to any patients.");
					return;
				}
				if(checkInsTwo1.Checked==true && (textLName1.Text=="" || textFName1.Text=="")//Insurance2 assigned to invalid patient1
					|| checkInsTwo2.Checked==true && (textLName2.Text=="" || textFName2.Text=="")//Insurance2 assigned to invalid patient2
					|| checkInsTwo3.Checked==true && (textLName3.Text=="" || textFName3.Text=="")//Insurance2 assigned to invalid patient3
					|| checkInsTwo4.Checked==true && (textLName4.Text=="" || textFName4.Text=="")//Insurance2 assigned to invalid patient4
					|| checkInsTwo5.Checked==true && (textLName5.Text=="" || textFName5.Text=="")) //Insurance2 assigned to invalid patient5
				{
					MsgBox.Show(this,"Insurance information 2 has been filled in, but has been assigned to a patient with no name.");
					return;
				}
			}
			#endregion Validate Insurance Subscriptions
			#region Validate Clinics
			if(PrefC.HasClinicsEnabled && !PrefC.GetBool(PrefName.ClinicAllowPatientsAtHeadquarters)) {
				if((comboClinic1.ClinicNumSelected==0 && textLName1.Text!="" && textFName1.Text!="")
					|| (comboClinic2.ClinicNumSelected==0 && textLName2.Text!="" && textFName2.Text!="")
					|| (comboClinic3.ClinicNumSelected==0 && textLName3.Text!="" && textFName3.Text!="")
					|| (comboClinic4.ClinicNumSelected==0 && textLName4.Text!="" && textFName4.Text!="")
					|| (comboClinic5.ClinicNumSelected==0 && textLName5.Text!="" && textFName5.Text!="")) 
				{
					MsgBox.Show(this,"Current settings for clinics do not allow patients to be added to the 'Unassigned' clinic. Please select a clinic.");
					return;
				}
			}
			#endregion
			if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
				if((comboPriProv1.SelectedIndex==0 && textLName1.Text!="" && textFName1.Text!="")//Patient has 'Select Provider' as Primary Provider
					|| (comboPriProv2.SelectedIndex==0 && textLName2.Text!="" && textFName2.Text!="")
					|| (comboPriProv3.SelectedIndex==0 && textLName3.Text!="" && textFName3.Text!="")
					|| (comboPriProv4.SelectedIndex==0 && textLName4.Text!="" && textFName4.Text!="")
					|| (comboPriProv5.SelectedIndex==0 && textLName5.Text!="" && textFName5.Text!="")) 
				{
					MsgBox.Show(this,"Primary provider must be set.");
					return;
				}
			}
			bool hasSavedMissingFields=false;
			if(_isMissingRequiredFields) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Required fields are missing or incorrect.  Click OK to save anyway or Cancel to return and "
						+"finish editing patient information.")) {
					_isValidating=true;
					ValidateFields();
					return;
				}
				else {
					hasSavedMissingFields=true;
					//Will make an audit trail further down once we know the guarantor's PatNum
				}
			}
			//check for duplicate patients
			List<Patient> listPatientsAdding = new List<Patient>();
			AddPatToList(textLName1.Text.Trim(),textFName1.Text.Trim(),textBirthdate1.Text,comboClinic1.ClinicNumSelected,listPatientsAdding);
			AddPatToList(textLName2.Text.Trim(),textFName2.Text.Trim(),textBirthdate2.Text,comboClinic2.ClinicNumSelected,listPatientsAdding);
			AddPatToList(textLName3.Text.Trim(),textFName3.Text.Trim(),textBirthdate3.Text,comboClinic3.ClinicNumSelected,listPatientsAdding);
			AddPatToList(textLName4.Text.Trim(),textFName4.Text.Trim(),textBirthdate4.Text,comboClinic4.ClinicNumSelected,listPatientsAdding);
			AddPatToList(textLName5.Text.Trim(),textFName5.Text.Trim(),textBirthdate5.Text,comboClinic5.ClinicNumSelected,listPatientsAdding);
			for(int i=0;i<listPatientsAdding.Count;i++) {//Check all the patients that we're trying to add.
				//get a list of all current patients that have the same name. PatNum here will be 0 which is fine.
				List<Patient> listPatients = Patients.GetListByName(listPatientsAdding[i].LName,listPatientsAdding[i].FName,listPatientsAdding[i].PatNum);
				for(int j=0;j<listPatients.Count;j++) {
					//If dates match or aren't entered there might be a duplicate patient.
					if(listPatients[j].Birthdate==listPatientsAdding[i].Birthdate
						|| listPatients[j].Birthdate.Year<1880
						|| listPatientsAdding[i].Birthdate.Year<1880) 
					{
						string msgText=Lang.g(this,"Patient")+" '"+listPatientsAdding[i].LName+", "+listPatientsAdding[i].FName+"' "
							+Lang.g(this,"may already exist. Continue anyway?");
						if(!MsgBox.Show(MsgBoxButtons.OKCancel,msgText,"Potential Duplicate Patient")) {
							return;
						}
						break;
					}
				}
			}
			#endregion Validation
			#region Create Family
			Patient[] patientArrayInFam=new Patient[5];
			Patient patient=new Patient();
			patient.PatStatus=PatientStatus.Patient;
			patient.HmPhone=textHmPhone.Text;
			patient.Address=textAddress.Text;
			patient.Address2=textAddress2.Text;
			patient.City=textCity.Text;
			patient.State=textState.Text.Trim();
			patient.Country=textCountry.Text;
			patient.Zip=textZip.Text.Trim();
			patient.AddrNote=textAddrNotes.Text;
			RefAttach refAttach=new RefAttach();
			if(_referral!=null) {
				refAttach.ReferralNum=_referral.ReferralNum;
				refAttach.RefType=ReferralType.RefFrom;
				refAttach.RefDate=DateTime.Today;
				if(_referral.IsDoctor) {//whether using ehr or not
					refAttach.IsTransitionOfCare=true;
				}
				refAttach.ItemOrder=1;
			}
			for(int i=0;i<patientArrayInFam.Length;i++) {
				//this is just in case, since we are using the same Patient object for every family member inserted
				//probably not necessary since inserting will assign a new PatNum
				patient.PatNum=0;
				patient.ImageFolder=null; //Need to reset image folder to null so we can overwrite for next patient.
				if(PrefC.HasClinicsEnabled) {
					patient.ClinicNum=getClinicNum(i);//Assign clinic num for the currently iterated patient if clinics is enabled.
				}
				switch(i) {
					case 0://guarantor
						patient.LName=textLName1.Text;
						patient.FName=textFName1.Text;
						if(listGender1.SelectedIndex==-1) { 
							listGender1.SetSelectedEnum(PatientGender.Unknown);	
						}
						patient.Gender=listGender1.GetSelected<PatientGender>();
						patient.Position=(PatientPosition)listPosition1.SelectedIndex;
						patient.Birthdate=PIn.Date(textBirthdate1.Text);
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							if(comboPriProv1.SelectedIndex>0) {//'Select Provider'
								patient.PriProv=_listProviders[comboPriProv1.SelectedIndex-1].ProvNum;
							}
						}
						else {
							patient.PriProv=_listProviders[comboPriProv1.SelectedIndex].ProvNum;
						}
						if(comboSecProv1.SelectedIndex>0) {
							patient.SecProv=_listProviders[comboSecProv1.SelectedIndex-1].ProvNum;//comboSecProv# contains 'none' so selected index -1
						}
						patient.SSN=Patients.SSNRemoveDashes(textSSN1.Text);
						patient.Email=textEmail1.Text;
						patient.WirelessPhone=textWirelessPhone1.Text;
						patient.TxtMsgOk=GetTxtMsgOK(checkTextingY1,checkTextingN1);
						patient.PatStatus=listStatus1.GetSelected<PatientStatus>();
						patient.BillingType=_listDefsBillingType[comboBillType1.SelectedIndex].DefNum;
						break;
					case 1://patient 2
						if(textFName2.Text=="" || textLName2.Text=="") {
							continue;
						}
						patient.PatNum=0;//may not be necessary, insert pat again with new values, insert will assign new PatNum
						patient.LName=textLName2.Text;
						patient.FName=textFName2.Text;
						if(listGender2.SelectedIndex==-1) { 
							listGender2.SetSelectedEnum(PatientGender.Unknown);	
						}
						patient.Gender=listGender2.GetSelected<PatientGender>();
						patient.Position=(PatientPosition)listPosition2.SelectedIndex;
						patient.Birthdate=PIn.Date(textBirthdate2.Text);
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							if(comboPriProv2.SelectedIndex>0) {//'Select Provider'
								patient.PriProv=_listProviders[comboPriProv2.SelectedIndex-1].ProvNum;
							}
						}
						else {
							patient.PriProv=_listProviders[comboPriProv2.SelectedIndex].ProvNum;
						}
						if(comboSecProv2.SelectedIndex>0) {
							patient.SecProv=_listProviders[comboSecProv2.SelectedIndex-1].ProvNum;//comboSecProv# contains 'none' so selected index -1
						}
						patient.SSN=Patients.SSNRemoveDashes(textSSN2.Text);
						patient.Email=textEmail2.Text;
						patient.WirelessPhone=textWirelessPhone2.Text;
						patient.TxtMsgOk=GetTxtMsgOK(checkTextingY2,checkTextingN2);
						patient.PatStatus=listStatus2.GetSelected<PatientStatus>();
						patient.BillingType=_listDefsBillingType[comboBillType2.SelectedIndex].DefNum;
						break;
					case 2://patient 3
						if(textFName3.Text=="" || textLName3.Text=="") {
							continue;
						}
						patient.PatNum=0;//may not be necessary, insert pat again with new values, insert will assign new PatNum
						patient.LName=textLName3.Text;
						patient.FName=textFName3.Text;
						if(listGender3.SelectedIndex==-1) { 
							listGender3.SetSelectedEnum(PatientGender.Unknown);	
						}
						patient.Gender=listGender3.GetSelected<PatientGender>();
						patient.Position=PatientPosition.Child;
						patient.Birthdate=PIn.Date(textBirthdate3.Text);
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							if(comboPriProv3.SelectedIndex>0) {//'Select Provider'
								patient.PriProv=_listProviders[comboPriProv3.SelectedIndex-1].ProvNum;
							}
						}
						else {
							patient.PriProv=_listProviders[comboPriProv3.SelectedIndex].ProvNum;
						}
						if(comboSecProv3.SelectedIndex>0) {
							patient.SecProv=_listProviders[comboSecProv3.SelectedIndex-1].ProvNum;//comboSecProv# contains 'none' so selected index -1
						}
						patient.SSN=Patients.SSNRemoveDashes(textSSN3.Text);
						patient.Email=textEmail3.Text;
						patient.WirelessPhone=textWirelessPhone3.Text;
						patient.TxtMsgOk=GetTxtMsgOK(checkTextingY3,checkTextingN3);
						patient.PatStatus=listStatus3.GetSelected<PatientStatus>();
						patient.BillingType=_listDefsBillingType[comboBillType3.SelectedIndex].DefNum;
						break;
					case 3://patient 4
						if(textFName4.Text=="" || textLName4.Text=="") {
							continue;
						}
						patient.PatNum=0;//may not be necessary, insert pat again with new values, insert will assign new PatNum
						patient.LName=textLName4.Text;
						patient.FName=textFName4.Text;
						if(listGender4.SelectedIndex==-1) { 
							listGender4.SetSelectedEnum(PatientGender.Unknown);	
						}
						patient.Gender=listGender4.GetSelected<PatientGender>();
						patient.Position=PatientPosition.Child;
						patient.Birthdate=PIn.Date(textBirthdate4.Text);
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							if(comboPriProv4.SelectedIndex>0) {//'Select Provider'
								patient.PriProv=_listProviders[comboPriProv4.SelectedIndex-1].ProvNum;
							}
						}
						else {
							patient.PriProv=_listProviders[comboPriProv4.SelectedIndex].ProvNum;
						}
						if(comboSecProv4.SelectedIndex>0) {
							patient.SecProv=_listProviders[comboSecProv4.SelectedIndex-1].ProvNum;//comboSecProv# contains 'none' so selected index -1
						}
						patient.SSN=Patients.SSNRemoveDashes(textSSN4.Text);
						patient.Email=textEmail4.Text;
						patient.WirelessPhone=textWirelessPhone4.Text;
						patient.TxtMsgOk=GetTxtMsgOK(checkTextingY4,checkTextingN4);
						patient.PatStatus=listStatus4.GetSelected<PatientStatus>();
						patient.BillingType=_listDefsBillingType[comboBillType4.SelectedIndex].DefNum;
						break;
					case 4://patient 5
						if(textFName5.Text=="" || textLName5.Text=="") {
							continue;
						}
						patient.PatNum=0;//may not be necessary, insert pat again with new values, insert will assign new PatNum
						patient.LName=textLName5.Text;
						patient.FName=textFName5.Text;
						if(listGender5.SelectedIndex==-1) { 
							listGender5.SetSelectedEnum(PatientGender.Unknown);	
						}
						patient.Gender=listGender5.GetSelected<PatientGender>();
						patient.Position=PatientPosition.Child;
						patient.Birthdate=PIn.Date(textBirthdate5.Text);
						if(PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
							if(comboPriProv5.SelectedIndex>0) {//'Select Provider'
								patient.PriProv=_listProviders[comboPriProv5.SelectedIndex-1].ProvNum;
							}
						}
						else {
							patient.PriProv=_listProviders[comboPriProv5.SelectedIndex].ProvNum;
						}
						if(comboSecProv5.SelectedIndex>0) {
							patient.SecProv=_listProviders[comboSecProv5.SelectedIndex-1].ProvNum;//comboSecProv# contains 'none' so selected index -1
						}
						patient.SSN=Patients.SSNRemoveDashes(textSSN5.Text);
						patient.Email=textEmail5.Text;
						patient.WirelessPhone=textWirelessPhone5.Text;
						patient.TxtMsgOk=GetTxtMsgOK(checkTextingY5,checkTextingN5);
						patient.PatStatus=listStatus5.GetSelected<PatientStatus>();
						patient.BillingType=_listDefsBillingType[comboBillType5.SelectedIndex].DefNum;
						break;
				}
				long patNum=Patients.Insert(patient,false);
				EhrPatients.Refresh(patNum);
				ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
				SecurityLogs.MakeLogEntry(EnumPermType.PatientCreate,patient.PatNum,"Created from Add Family window.");
				//if this is the first family member it is the guarantor, so set pat.Guarantor=pat.PatNum and update
				//if this is not the first family member, the guarantor has been inserted and pat.Guarantor will already be set before inserting
				if(i==0) {
					Patient patientOld=patient.Copy();
					patient.Guarantor=patient.PatNum;
					Patients.Update(patient,patientOld);
					//if this is the guarantor, also check if there is a commlog to be saved
					if(_commlog != null && _commlog.CommlogNum>0) {
						_commlog.PatNum=patient.PatNum;
						Commlogs.Update(_commlog);
					}
				}
				patientArrayInFam[i]=patient.Copy();//add patient to local patient array of family members, arrayPatsInFam[0] will be the guarantor
				if(_referral!=null) {
					refAttach.PatNum=patient.PatNum;
					RefAttaches.Insert(refAttach);
					SecurityLogs.MakeLogEntry(EnumPermType.RefAttachAdd,patient.PatNum,"Referred From "+Referrals.GetNameFL(refAttach.ReferralNum));
				}
				CustReference custReference=new CustReference();
				custReference.PatNum=patient.PatNum;
				CustReferences.Insert(custReference);
			}
			#endregion Create Family
			#region Insurance
			InsSub insSub1=null;
			InsSub insSub2=null;
			List<CovCat> listCovCats=CovCats.GetWhere(x => x.DefaultPercent!=-1,true);
			#region Validate Plans
			//validate the ins fields.  If they don't match perfectly, then set the selected plan to null
			if(_insPlanSelected1!=null) {
				if(Employers.GetName(_insPlanSelected1.EmployerNum)!=textEmployer1.Text
					|| Carriers.GetName(_insPlanSelected1.CarrierNum)!=textCarrier1.Text
					|| _insPlanSelected1.GroupName!=textGroupName1.Text
					|| _insPlanSelected1.GroupNum!=textGroupNum1.Text)
				{
					_insPlanSelected1=null;
				}
			}
			if(_insPlanSelected2!=null) {
				if(Employers.GetName(_insPlanSelected2.EmployerNum)!=textEmployer2.Text
					|| Carriers.GetName(_insPlanSelected2.CarrierNum)!=textCarrier2.Text
					|| _insPlanSelected2.GroupName!=textGroupName2.Text
					|| _insPlanSelected2.GroupNum!=textGroupNum2.Text)
				{
					_insPlanSelected2=null;
				}
			}
			//validate the carrier fields.  If they don't match perfectly, then set the selected plan to null
			if(_carrierSelected1!=null) {
				if(_carrierSelected1.CarrierName!=textCarrier1.Text || _carrierSelected1.Phone!=textPhone1.Text) {
					_carrierSelected1=null;
				}
			}
			if(_carrierSelected2!=null) {
				if(_carrierSelected2.CarrierName!=textCarrier2.Text || _carrierSelected2.Phone!=textPhone2.Text) {
					_carrierSelected2=null;
				}
			}
			#endregion Validate Plans
			#region Insert InsPlans, Benefits, and InsSubs
			#region Primary Ins
			if(isInsComplete1) {
				if(_carrierSelected1==null) {
					//get a carrier, possibly creating a new one if needed.
					_carrierSelected1=Carriers.GetByNameAndPhone(textCarrier1.Text,textPhone1.Text);
				}
				if(_insPlanSelected1==null) {
					//don't try to get a copy of an existing plan. Instead, start from scratch.
					_insPlanSelected1=new InsPlan();
					_insPlanSelected1.EmployerNum=Employers.GetEmployerNum(textEmployer1.Text);
					_insPlanSelected1.CarrierNum=_carrierSelected1.CarrierNum;
					_insPlanSelected1.GroupName=textGroupName1.Text;
					_insPlanSelected1.GroupNum=textGroupNum1.Text;
					_insPlanSelected1.PlanType="";
					if(PrefC.GetBool(PrefName.InsDefaultPPOpercent)) {
						_insPlanSelected1.PlanType="p";
					}
					if(_insPlanSelected1.PlanType=="" && PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims)) {
						_insPlanSelected1.ClaimsUseUCR=true;
					}
					_insPlanSelected1.CobRule=(EnumCobRule)PrefC.GetInt(PrefName.InsDefaultCobRule);
					InsPlans.Insert(_insPlanSelected1);
					Benefit benefit=new Benefit();
					benefit.PlanNum=_insPlanSelected1.PlanNum;//same for all benefits inserted
					benefit.BenefitType=InsBenefitType.CoInsurance;//same for all benefits inserted from CovCats.ListShort
					benefit.MonetaryAmt=-1;//same for all benefits inserted from CovCats.ListShort
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;//same for all benefits inserted
					benefit.CodeNum=0;//same for all benefits inserted
					benefit.CoverageLevel=BenefitCoverageLevel.None;//same for all benefits inserted from CovCats.ListShort
					for(int i=0;i<listCovCats.Count;i++) {
						benefit.CovCatNum=listCovCats[i].CovCatNum;
						benefit.Percent=listCovCats[i].DefaultPercent;
						Benefits.Insert(benefit);
					}
					benefit.BenefitType=InsBenefitType.Deductible;//same for Diagnostic and RoutinePreventive
					benefit.Percent=-1;//same for Diagnostic and RoutinePreventive
					benefit.MonetaryAmt=0;//same for Diagnostic and RoutinePreventive
					benefit.CoverageLevel=BenefitCoverageLevel.Individual;//same for Diagnostic and RoutinePreventive
					//Zero deductible diagnostic
					if(CovCats.GetForEbenCat(EbenefitCategory.Diagnostic)!=null) {
						benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
						Benefits.Insert(benefit);
					}
					//Zero deductible preventive
					if(CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive)!=null) {
						benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
						Benefits.Insert(benefit);
					}
				}
				insSub1=new InsSub();
				insSub1.PlanNum=_insPlanSelected1.PlanNum;
				insSub1.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
				insSub1.ReleaseInfo=true;
				insSub1.DateEffective=DateTime.MinValue;
				insSub1.DateTerm=DateTime.MinValue;
				if(comboSubscriber1.SelectedIndex>0) {//comboSubscriber has been validated to contain the same number of indexes as the family list
					insSub1.Subscriber=patientArrayInFam[comboSubscriber1.SelectedIndex-1].PatNum;
				}
				insSub1.SubscriberID=textSubscriberID1.Text;
				InsSubs.Insert(insSub1);
			}
			#endregion Primary Ins
			#region Secondary Ins
			if(isInsComplete2) {
				if(_carrierSelected2==null) {
					_carrierSelected2=Carriers.GetByNameAndPhone(textCarrier2.Text,textPhone2.Text);
				}
				if(_insPlanSelected2==null) {
					//don't try to get a copy of an existing plan. Instead, start from scratch.
					_insPlanSelected2=new InsPlan();
					_insPlanSelected2.EmployerNum=Employers.GetEmployerNum(textEmployer2.Text);
					_insPlanSelected2.CarrierNum=_carrierSelected2.CarrierNum;
					_insPlanSelected2.GroupName=textGroupName2.Text;
					_insPlanSelected2.GroupNum=textGroupNum2.Text;
					_insPlanSelected2.PlanType="";
					if(PrefC.GetBool(PrefName.InsDefaultPPOpercent)) {
						_insPlanSelected2.PlanType="p";
					}
					if(_insPlanSelected2.PlanType=="" && PrefC.GetBool(PrefName.InsDefaultShowUCRonClaims)) {
						_insPlanSelected2.ClaimsUseUCR=true;
					}
					_insPlanSelected2.CobRule=(EnumCobRule)PrefC.GetInt(PrefName.InsDefaultCobRule);
					InsPlans.Insert(_insPlanSelected2);
					Benefit benefit=new Benefit();
					benefit.PlanNum=_insPlanSelected2.PlanNum;//same for all benefits inserted
					benefit.BenefitType=InsBenefitType.CoInsurance;//same for all benefits inserted from CovCats.ListShort
					benefit.MonetaryAmt=-1;//same for all benefits inserted from CovCats.ListShort
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;//same for all benefits inserted
					benefit.CodeNum=0;//same for all benefits inserted
					benefit.CoverageLevel=BenefitCoverageLevel.None;//same for all benefits inserted from CovCats.ListShort
					for(int i=0;i<listCovCats.Count;i++) {
						benefit.CovCatNum=listCovCats[i].CovCatNum;
						benefit.Percent=listCovCats[i].DefaultPercent;
						Benefits.Insert(benefit);
					}
					benefit.BenefitType=InsBenefitType.Deductible;//same for Diagnostic and RoutinePreventive
					benefit.Percent=-1;//same for Diagnostic and RoutinePreventive
					benefit.MonetaryAmt=0;//same for Diagnostic and RoutinePreventive
					benefit.CoverageLevel=BenefitCoverageLevel.Individual;//same for Diagnostic and RoutinePreventive
					//Zero deductible diagnostic
					if(CovCats.GetForEbenCat(EbenefitCategory.Diagnostic)!=null) {
						benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
						Benefits.Insert(benefit);
					}
					//Zero deductible preventive
					if(CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive)!=null) {
						benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
						Benefits.Insert(benefit);
					}
				}
				insSub2=new InsSub();
				insSub2.PlanNum=_insPlanSelected2.PlanNum;
				insSub2.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
				insSub2.ReleaseInfo=true;
				insSub2.DateEffective=DateTime.MinValue;
				insSub2.DateTerm=DateTime.MinValue;
				if(comboSubscriber2.SelectedIndex>0) {//comboSubscriber has been validated to contain the same number of indexes as the family list
					insSub2.Subscriber=patientArrayInFam[comboSubscriber2.SelectedIndex-1].PatNum;
				}
				insSub2.SubscriberID=textSubscriberID2.Text;
				InsSubs.Insert(insSub2);
			}
			#endregion Secondary Ins
			#endregion Insert InsPlans, Benefits, and InsSubs
			#region Create PatPlans
			PatPlan patPlan1=new PatPlan();
			if(isInsComplete1) {
				patPlan1.InsSubNum=insSub1.InsSubNum;
			}
			PatPlan patPlan2=new PatPlan();
			if(isInsComplete2) {
				patPlan2.InsSubNum=insSub2.InsSubNum;
			}
			bool hasPatPlanAdded=false;
			for(int i=0;i<patientArrayInFam.Length;i++) {//loop for each possible family member. Position 0 is the guarantor and is required but all others could be null
				if(!isInsComplete1 && !isInsComplete2) {
					break;
				}
				if(patientArrayInFam[i]==null) {
					continue;
				}
				patPlan1.PatNum=patientArrayInFam[i].PatNum;
				patPlan1.Ordinal=1;
				patPlan1.Relationship=Relat.Child;//default realtionship to child, will be set to Self or Spouse for the two adults in the family
				patPlan2.PatNum=patientArrayInFam[i].PatNum;
				patPlan2.Ordinal=2;
				patPlan2.Relationship=Relat.Child;//default realtionship to child, will be set to Self or Spouse for the two adults in the family
				switch(i) {
					case 0://guarantor, 1st adult
						if(isInsComplete1 && checkInsOne1.Checked==true) {
							//the only situation where ordinal would be 2 is if ins2 is checked and ins1 does not have this pat as the subscriber and ins2 does
							if(checkInsTwo1.Checked==true && comboSubscriber1.SelectedIndex!=1 && comboSubscriber2.SelectedIndex==1) {//both combo boxes contain 'none'
								patPlan1.Ordinal=2;
							}
							patPlan1.Relationship=Relat.Self;//the subscriber would never be a child, so default to Self
							if(comboSubscriber1.SelectedIndex==2) {
								patPlan1.Relationship=Relat.Spouse;
							}
							PatPlans.Insert(patPlan1);
							hasPatPlanAdded=true;
						}
						if(isInsComplete2 && checkInsTwo1.Checked==true) {
							//the only situations where ordinal would be 1 is if ins1 is not checked or if ins2 has this patient as subscriber and ins1 does not.
							if(checkInsOne1.Checked==false || (comboSubscriber2.SelectedIndex==1 && comboSubscriber1.SelectedIndex!=1))	{
								patPlan2.Ordinal=1;
							}
							patPlan2.Relationship=Relat.Self;//the subscriber would never be a child, so default to Self
							if(comboSubscriber2.SelectedIndex==2) {
								patPlan2.Relationship=Relat.Spouse;
							}
							PatPlans.Insert(patPlan2);
							hasPatPlanAdded=true;
						}
						//Set the insurance flag for this patient
						Patients.SetHasIns(patientArrayInFam[i].PatNum);
						continue;
					case 1://patient 1, 2nd adult
						if(isInsComplete1 && checkInsOne2.Checked==true) {
							//the only situation where ordinal would be 2 is if ins2 is checked and ins1 does not have this pat as the subscriber and ins2 does
							if(checkInsTwo2.Checked==true && comboSubscriber1.SelectedIndex!=2 && comboSubscriber2.SelectedIndex==2) {
								patPlan1.Ordinal=2;
							}
							patPlan1.Relationship=Relat.Self;//the subscriber would never be a child, so default to Self
							if(comboSubscriber1.SelectedIndex==1) {
								patPlan1.Relationship=Relat.Spouse;
							}
							PatPlans.Insert(patPlan1);
							hasPatPlanAdded=true;
						}
						if(isInsComplete2 && checkInsTwo2.Checked==true) {
							//the only situations where ordinal would be 1 is if ins1 is not checked or if ins2 has this patient as subscriber and ins1 does not.
							if(checkInsOne2.Checked==false || (comboSubscriber2.SelectedIndex==2 && comboSubscriber1.SelectedIndex!=2))	{
								patPlan2.Ordinal=1;
							}
							patPlan2.Relationship=Relat.Self;//the subscriber would never be a child, so default to Self
							if(comboSubscriber2.SelectedIndex==1) {
								patPlan2.Relationship=Relat.Spouse;
							}
							PatPlans.Insert(patPlan2);
							hasPatPlanAdded=true;
						}
						//Set the insurance flag for this patient
						Patients.SetHasIns(patientArrayInFam[i].PatNum);
						continue;
					case 2://patient 2, 1st child
						if(isInsComplete1 && checkInsOne3.Checked==true) {
							PatPlans.Insert(patPlan1);
							hasPatPlanAdded=true;
						}
						if(isInsComplete2 && checkInsTwo3.Checked==true) {
							//the only situation where ordinal would be 1 is if ins1 is not checked.
							if(checkInsOne3.Checked==false) {
								patPlan2.Ordinal=1;
							}
							PatPlans.Insert(patPlan2);
							hasPatPlanAdded=true;
						}
						//Set the insurance flag for this patient
						Patients.SetHasIns(patientArrayInFam[i].PatNum);
						continue;
					case 3://patient 3, 2nd child
						if(isInsComplete1 && checkInsOne4.Checked==true) {
							PatPlans.Insert(patPlan1);
							hasPatPlanAdded=true;
						}
						if(isInsComplete2 && checkInsTwo4.Checked==true) {
							//the only situation where ordinal would be 1 is if ins1 is not checked.
							if(checkInsOne4.Checked==false) {
								patPlan2.Ordinal=1;
							}
							PatPlans.Insert(patPlan2);
							hasPatPlanAdded=true;
						}
						//Set the insurance flag for this patient
						Patients.SetHasIns(patientArrayInFam[i].PatNum);
						continue;
					case 4://patient 4, 3rd child
						if(isInsComplete1 && checkInsOne5.Checked==true) {
							PatPlans.Insert(patPlan1);
							hasPatPlanAdded=true;
						}
						if(isInsComplete2 && checkInsTwo5.Checked==true) {
							//the only situation where ordinal would be 1 is if ins1 is not checked.
							if(checkInsOne5.Checked==false) {
								patPlan2.Ordinal=1;
							}
							PatPlans.Insert(patPlan2);
							hasPatPlanAdded=true;
						}
						//Set the insurance flag for this patient
						Patients.SetHasIns(patientArrayInFam[i].PatNum);
						continue;
				}
			}
			if(hasPatPlanAdded) {
				SecurityLogs.MakeLogEntry(EnumPermType.PatPlanCreate,patientArrayInFam[0].PatNum,"Multiple PatPlans created when adding multiple patients.");
			}
			#endregion Create PatPlans
			#endregion Insurance
			PatNumSelected=patientArrayInFam[0].PatNum;//Guarantor
			if(hasSavedMissingFields) {
				SecurityLogs.MakeLogEntry(EnumPermType.RequiredFields,PatNumSelected,"Saved patient with required fields missing.");
			}
			#region Send HL7 if Applicable
			//If there is an existing HL7 def enabled, send an ADT message for each patient inserted if there is an outbound ADT message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				for(int i=0;i<5;i++) {
					if(patientArrayInFam[i]==null) {
						continue;
					}
					//new patients get the A04 ADT, updating existing patients we send an A08
					MessageHL7 messageHL7=MessageConstructor.GenerateADT(patientArrayInFam[i],patientArrayInFam[0],EventTypeHL7.A04);//arrayPatsInFam[0] is guar, never null
					//Will be null if there is no outbound ADT message defined, so do nothing
					if(messageHL7==null) {
						continue;
					}
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=0;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=patientArrayInFam[i].PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						MsgBox.Show(hl7Msg.ToString());
					}
				}
			}
			#endregion Send HL7 if Applicable
			#region Insert HieQueue if Applicable
			if(HieClinics.IsEnabled()) {
				for(int i=0;i<5;i++) {
					if(patientArrayInFam[i]==null) {
						continue;
					}
					HieQueues.Insert(new HieQueue(patientArrayInFam[i].PatNum));
				}
			}
			#endregion
			MessageBox.Show("Done");
			IsDialogOK=true;
		}

		private void FrmPatientAddAll_FormClosing(object sender,EventArgs e) {
			if(IsDialogOK) {
				return;
			}
			if(_commlog == null || _commlog.CommlogNum <= 0) {
				return;
			}
			try {
				Commlogs.Delete(_commlog);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

	}
}