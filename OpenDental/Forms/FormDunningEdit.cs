using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormDunningEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private Dunning _dunningCur;
		private List<Def> _listBillingTypeDefs;

		///<summary></summary>
		public FormDunningEdit(Dunning dunningCur)
		{
			//
			// Required for Windows Form Designer support
			//
			_dunningCur=dunningCur.Copy();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDunningEdit_Load(object sender, System.EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				comboClinics.SelectedClinicNum=_dunningCur.ClinicNum;
			}
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				checkSuperFamily.Visible=true;
				checkSuperFamily.Checked=_dunningCur.IsSuperFamily;
			}
			listBillType.Items.Add(Lan.g(this,"all"));
			listBillType.SetSelected(0,true);
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			for(int i=0;i<_listBillingTypeDefs.Count;i++){
				listBillType.Items.Add(_listBillingTypeDefs[i].ItemName);
				if(_dunningCur.BillingType==_listBillingTypeDefs[i].DefNum){
					listBillType.SetSelected(i+1,true);
				}
			}
			switch(_dunningCur.AgeAccount){
				case 0:
					radioAny.Checked=true;
					break;
				case 30:
					radio30.Checked=true;
					break;
				case 60:
					radio60.Checked=true;
					break;
				case 90:
					radio90.Checked=true;
					break;
			}
			switch(_dunningCur.InsIsPending){
				case YN.Unknown:
					radioU.Checked=true;
					break;
				case YN.Yes:
					radioY.Checked=true;
					break;
				case YN.No:
					radioN.Checked=true;
					break;
			}
			textDaysInAdvance.Text=_dunningCur.DaysInAdvance.ToString();
			textDunMessage.Text=_dunningCur.DunMessage;
			textMessageBold.Text=_dunningCur.MessageBold;
			textEmailBody.Text=_dunningCur.EmailBody;
			textEmailSubject.Text=_dunningCur.EmailSubject;
		}

		private void radioAny_CheckedChanged(object sender,EventArgs e) {
				labelDaysInAdvance.Visible=!radioAny.Checked;
				textDaysInAdvance.Visible=!radioAny.Checked;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
			}
			else{
				Dunnings.Delete(_dunningCur);
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDaysInAdvance.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDunMessage.Text=="" && textMessageBold.Text==""
				&& textEmailSubject.Text=="" && textEmailBody.Text=="") 
			{
				MsgBox.Show(this,"All messages cannot be blank.");
				return;
			}
			_dunningCur.BillingType=0;
			if(listBillType.SelectedIndex>0) {
				_dunningCur.BillingType=_listBillingTypeDefs[listBillType.SelectedIndex-1].DefNum;
			}
			_dunningCur.AgeAccount=(byte)(30*new List<RadioButton> { radioAny,radio30,radio60,radio90 }.FindIndex(x => x.Checked));//0, 30, 60, or 90
			_dunningCur.InsIsPending=(YN)new List<RadioButton> { radioU,radioY,radioN }.FindIndex(x => x.Checked);//0=Unknown, 1=Yes, 2=No
			_dunningCur.DaysInAdvance=0;//default will be 0
			if(!radioAny.Checked) {
				_dunningCur.DaysInAdvance=PIn.Int(textDaysInAdvance.Text);//blank=0
			}
			_dunningCur.DunMessage=textDunMessage.Text;
			_dunningCur.MessageBold=textMessageBold.Text;
			_dunningCur.EmailBody=textEmailBody.Text;
			_dunningCur.EmailSubject=textEmailSubject.Text;
			_dunningCur.IsSuperFamily=checkSuperFamily.Checked;
			if(PrefC.HasClinicsEnabled) {
				_dunningCur.ClinicNum=comboClinics.SelectedClinicNum;
			}
			if(IsNew){
				Dunnings.Insert(_dunningCur);
			}
			else{
				Dunnings.Update(_dunningCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















