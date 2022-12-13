using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using WebServiceSerializer;

namespace OpenDental {
	public partial class FormFHIRSetup:FormODBase {
		///<summary>The API keys obtained from ODHQ.</summary>
		private List<ApiKeyVisibleInfo> _listApiKeyVisibleInfos;

		public FormFHIRSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormFHIRSetup_Load(object sender,EventArgs e) {
			Program program=Programs.GetCur(ProgramName.FHIR);
			checkEnabled.Checked=program.Enabled;
			textSubInterval.Text=ProgramProperties.GetPropVal(program.ProgramNum,"SubscriptionProcessingFrequency");
			comboPayType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaymentTypes,true));
			comboPayType.SetSelectedDefNum(PrefC.GetLong(PrefName.ApiPaymentType));
			//Let the load finish before we call HQ
			this.BeginInvoke(() => {
				Cursor=Cursors.WaitCursor;
				Application.DoEvents();
				try{
					_listApiKeyVisibleInfos=APIKeys.GetListApiKeyVisibleInfos(forceRefresh:true);
				}
				catch(Exception ex){
					Cursor=Cursors.Default;
					MsgBox.Show(ex.Message);
					DialogResult=DialogResult.Cancel;
					return;
				}
				Cursor=Cursors.Default;
				FillGrid();
			});
		}

		///<summary>FillGrid also replaces apikey database table.</summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Developer"),200);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"API Key"),180);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Enabled"),80,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listApiKeyVisibleInfos.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listApiKeyVisibleInfos[i].DeveloperName);
				row.Cells.Add(_listApiKeyVisibleInfos[i].CustomerKey);
				switch(_listApiKeyVisibleInfos[i].FHIRKeyStatusCur) {
					case FHIRKeyStatus.Enabled:
					case FHIRKeyStatus.EnabledReadOnly:
						row.Cells.Add("X");
						break;
					case FHIRKeyStatus.DisabledByCustomer:
					case FHIRKeyStatus.DisabledByDeveloper:
					case FHIRKeyStatus.DisabledByHQ:
					default:
						row.Cells.Add("");
						break;
				}
				row.Tag=_listApiKeyVisibleInfos[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			//Replace the apikey database table.
			List<APIKey> listAPIKeys=new List<APIKey>();
			for(int i=0;i<_listApiKeyVisibleInfos.Count;i++) {
				APIKey apiKey=new APIKey();
				apiKey.CustApiKey=_listApiKeyVisibleInfos[i].CustomerKey;
				apiKey.DevName=_listApiKeyVisibleInfos[i].DeveloperName;
				listAPIKeys.Add(apiKey);
			}
			APIKeys.SaveListAPIKeys(listAPIKeys);
		}

		private void butAddKey_Click(object sender,EventArgs e) {
			using FormFHIRAssignKey formFHIRAssignKey=new FormFHIRAssignKey();
			if(formFHIRAssignKey.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			try{
				_listApiKeyVisibleInfos=APIKeys.GetListApiKeyVisibleInfos(forceRefresh:true);
			}
			catch(Exception ex){
				Cursor=Cursors.Default;
				MsgBox.Show(ex.Message);
				DialogResult=DialogResult.Cancel;
				return;
			}
			Cursor=Cursors.Default;
			FillGrid();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			SetPermissions();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormFHIRAPIKeyEdit formFHIRAPIKeyEdit=new FormFHIRAPIKeyEdit((ApiKeyVisibleInfo)gridMain.ListGridRows[e.Row].Tag);
			formFHIRAPIKeyEdit.ShowDialog();
			if(formFHIRAPIKeyEdit.HasChanged) {
				FillGrid();
			}
		}

		///<summary>Displays the permissions that are assigned for the selected API key.</summary>
		private void SetPermissions() {
			if(gridMain.SelectedIndices.Length<1) {
				return;
			}
			listPermissions.Items.Clear();
			ApiKeyVisibleInfo apiKeyPair=(ApiKeyVisibleInfo)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag;
			listPermissions.Items.AddList(apiKeyPair.ListAPIPermissions, x => x.GetDescription());
		}

		private void butClose_Click(object sender,EventArgs e) {
			if(!textSubInterval.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(comboPayType.SelectedIndex < 0) {
				MsgBox.Show(this,"Please select a payment type.");
				return;
			}
			Program program=Programs.GetCur(ProgramName.FHIR);
			program.Enabled=checkEnabled.Checked;
			Programs.Update(program);
			ProgramProperty programProperty=ProgramProperties.GetPropByDesc("SubscriptionProcessingFrequency",ProgramProperties.GetForProgram(program.ProgramNum));
			ProgramProperties.UpdateProgramPropertyWithValue(programProperty,textSubInterval.Text);
			DataValid.SetInvalid(InvalidType.Programs);
			if(Prefs.UpdateLong(PrefName.ApiPaymentType,comboPayType.GetSelectedDefNum())) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			Close();
		}

	}

	
}