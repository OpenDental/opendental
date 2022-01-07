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
		private List<APIKey> _listApiKeys;

		public FormFHIRSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormFHIRSetup_Load(object sender,EventArgs e) {
			Program prog=Programs.GetCur(ProgramName.FHIR);
			checkEnabled.Checked=prog.Enabled;
			textSubInterval.Text=ProgramProperties.GetPropVal(prog.ProgramNum,"SubscriptionProcessingFrequency");
			comboPayType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaymentTypes,true));
			comboPayType.SetSelectedDefNum(PrefC.GetLong(PrefName.ApiPaymentType));
			//Let the load finish before we call HQ
			this.BeginInvoke(() => {
				Cursor=Cursors.WaitCursor;
				Application.DoEvents();
				_listApiKeys=GetApiKeys();
				Cursor=Cursors.Default;
				if(_listApiKeys==null) {
					DialogResult=DialogResult.Cancel;//We have already shown them an error message.
					return;
				}
				FillGrid();
			});
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Developer"),200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"API Key"),180);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Enabled"),80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listApiKeys.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listApiKeys[i].DeveloperName);
				row.Cells.Add(_listApiKeys[i].Key);
				switch(_listApiKeys[i].KeyStatus) {
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
				row.Tag=_listApiKeys[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private List<APIKey> GetApiKeys() {
			List<APIKey> listApiKeys=new List<APIKey>();
			//prepare the xml document to send--------------------------------------------------------------------------------------
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars=("    ");
			StringBuilder strbuild=new StringBuilder();
			//Send the message and get the result-------------------------------------------------------------------------------------
			string result="";
			try {
				string officeData=PayloadHelper.CreatePayload(strbuild.ToString(),eServiceCode.FHIR);
				result=WebServiceMainHQProxy.GetWebServiceMainHQInstance().GetFHIRAPIKeysForOffice(officeData);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return null;
			}
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			XPathNavigator nav=doc.CreateNavigator();
			//Process errors------------------------------------------------------------------------------------------------------------
			XPathNavigator node=nav.SelectSingleNode("//Error");
			if(node!=null) {
				MessageBox.Show(node.Value);
				return null;
			}
			//Process a valid return value------------------------------------------------------------------------------------------------{
			node=nav.SelectSingleNode("//ListAPIKeys");
			if(node!=null && node.MoveToFirstChild()) {
				do {
					APIKey apiKey=new APIKey();
					apiKey.Key=node.SelectSingleNode("APIKeyValue").Value;
					apiKey.FHIRAPIKeyNum=PIn.Long(node.SelectSingleNode("FHIRAPIKeyNum").Value);
					apiKey.DateDisabled=DateTime.Parse(node.SelectSingleNode("DateDisabled").Value);
					if(!Enum.TryParse(node.SelectSingleNode("KeyStatus").Value,out apiKey.KeyStatus)) {
						APIKeyStatus status;
						if(Enum.TryParse(node.SelectSingleNode("KeyStatus").Value,out status)) {
							apiKey.KeyStatus=FHIRUtils.ToFHIRKeyStatus(status);
						}
						else {
							apiKey.KeyStatus=FHIRKeyStatus.DisabledByHQ;
						}
					}
					apiKey.DeveloperName=node.SelectSingleNode("DeveloperName").Value;
					apiKey.DeveloperEmail=node.SelectSingleNode("DeveloperEmail").Value;
					apiKey.DeveloperPhone=node.SelectSingleNode("DeveloperPhone").Value;
					apiKey.FHIRDeveloperNum=PIn.Long(node.SelectSingleNode("FHIRDeveloperNum").Value);
					XPathNavigator nodePerms=node.SelectSingleNode("ListAPIPermissions");
					if(nodePerms!=null && nodePerms.MoveToFirstChild()) {
						do {
							APIPermission perm;
							if(Enum.TryParse(nodePerms.Value,out perm)) {
								apiKey.ListPermissions.Add(perm);
							}
						} while(nodePerms.MoveToNext());
					}
					listApiKeys.Add(apiKey);
				} while(node.MoveToNext());
			}
			return listApiKeys;
		}

		private void butAddKey_Click(object sender,EventArgs e) {
			using FormFHIRAssignKey formFHIRAssignKey=new FormFHIRAssignKey();
			if(formFHIRAssignKey.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			_listApiKeys=GetApiKeys();
			Cursor=Cursors.Default;
			if(_listApiKeys==null) {
				DialogResult=DialogResult.Cancel;//We have already shown them an error message.
				return;
			}
			FillGrid();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			SetPermissions();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormFHIRAPIKeyEdit formFHIRAPIKeyEdit=new FormFHIRAPIKeyEdit((APIKey)gridMain.ListGridRows[e.Row].Tag);
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
			APIKey apiKey=(APIKey)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag;
			listPermissions.Items.AddList(apiKey.ListPermissions, x => x.GetDescription());
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
			Program prog=Programs.GetCur(ProgramName.FHIR);
			prog.Enabled=checkEnabled.Checked;
			Programs.Update(prog);
			ProgramProperty progProp=ProgramProperties.GetPropByDesc("SubscriptionProcessingFrequency",ProgramProperties.GetForProgram(prog.ProgramNum));
			ProgramProperties.UpdateProgramPropertyWithValue(progProp,textSubInterval.Text);
			DataValid.SetInvalid(InvalidType.Programs);
			if(Prefs.UpdateLong(PrefName.ApiPaymentType,comboPayType.GetSelectedDefNum())) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			Close();
		}

	}

	///<summary>A developer's API key. Provided by ODHQ.</summary>
	public class APIKey {
		///<summary>The string representation of the key.</summary>
		public string Key;
		///<summary>The status of the key.</summary>
		public FHIRKeyStatus KeyStatus;
		///<summary>The permissions that an APIKey possesses.</summary>
		public List<APIPermission> ListPermissions=new List<APIPermission>();
		///<summary>The name of the developer that owns this key.</summary>
		public string DeveloperName;
		///<summary>The email of the developer that owns this key.</summary>
		public string DeveloperEmail;
		///<summary>The phone number of the developer that owns this key.</summary>
		public string DeveloperPhone;
		///<summary>The phone number of the developer that owns this key.</summary>
		public DateTime DateDisabled;
		///<summary>FK to fhirdeveloper.FHIRDeveloperNum. This table only exists in serviceshq database.</summary>
		public long FHIRDeveloperNum;
		///<summary>FK to fhirapikey.FHIRAPIKeyNum. This table only exists in serviceshq database.</summary>
		public long FHIRAPIKeyNum;

		public APIKey Copy() {
			APIKey apiKey=(APIKey)MemberwiseClone();
			apiKey.ListPermissions=ListPermissions.ToList();
			return apiKey;
		}
	}
}