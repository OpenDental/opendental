using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDental.Bridges;
using System.Linq;
using System.Text;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormCarriers : FormODBase {
		///<summary>Set to true if using this dialog to select a carrier.</summary>
		public bool IsSelectMode;
		private DataTable table;
		public Carrier SelectedCarrier;
		private List<ItransImportFields> _listShownUpdateFields=new List<ItransImportFields>();

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormCarriersCanada";
			}
			return "FormCarriers";
		}

		///<summary></summary>
		public FormCarriers()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCarriers_Load(object sender, System.EventArgs e) {
			//if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
			//No.  Even Canadian users will want to see all their carriers and only use the checkbox for special situations.
			//	checkCDAnet.Checked=true;
			//}
			//else{
			//	checkCDAnet.Visible=false;
			//}
			if(IsSelectMode) {
				butCancel.Text=Lan.g(this,"Cancel");
			}
			else {
				butCancel.Text=Lan.g(this,"Close");
				butOK.Visible=false;
			}
			if(!Security.IsAuthorized(Permissions.CarrierCreate,true)) {
				butAdd.Enabled=false;
			}
			Clearinghouse ch=Clearinghouses.GetDefaultDental();
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && ch.Eformat==ElectronicClaimFormat.Canadian) {
				groupItrans.Visible=true;
				ItransImportFields fieldsToImport=(ItransImportFields)PrefC.GetInt(PrefName.ItransImportFields);
				checkITransPhone.Checked=(fieldsToImport.HasFlag(ItransImportFields.Phone));
				checkItransAddress.Checked=(fieldsToImport.HasFlag(ItransImportFields.Address));
				checkItransName.Checked=(fieldsToImport.HasFlag(ItransImportFields.Name));
				checkItransMissing.Checked=(fieldsToImport.HasFlag(ItransImportFields.AddMissing));
			}
			Carriers.RefreshCache();
			FillGrid();
		}

		private void FillGrid(){
			List<string> selectedCarrierNums=new List<string>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				selectedCarrierNums.Add(table.Rows[gridMain.SelectedIndices[i]]["CarrierNum"].ToString());
			}
			//Carriers.Refresh();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			/*if(checkCDAnet.Checked){
				//gridMain.Size=new Size(745,gridMain.Height);
				col=new ODGridColumn(Lan.g("TableCarriers","Carrier Name"),160);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","EDI Code"),60);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","PMP"),50,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","Network"),50);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","Version"),50);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","02"),50,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","03"),50,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","04"),50,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","05"),50,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","06"),50,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","07"),50,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","08"),50,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableCarriers","Hidden"),50,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
			}
			else{*/
				//gridMain.Size=new Size(839,gridMain.Height);
			col=new GridColumn(Lan.g("TableCarriers","Carrier Name"),160);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableCarriers","Phone"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableCarriers","Address"),130);
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn(Lan.g("TableCarriers","Address2"),120);
			//gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableCarriers","City"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableCarriers","ST"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableCarriers","Zip"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableCarriers","ElectID"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableCarriers","Hidden"),50,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableCarriers","Plans"),50);
			gridMain.ListGridColumns.Add(col);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				col=new GridColumn(Lan.g("TableCarriers","CDAnet"),50);
				gridMain.ListGridColumns.Add(col);
			}
			//}
			gridMain.ListGridRows.Clear();
			GridRow row;
			table=Carriers.GetBigList(checkCDAnet.Checked,checkShowHidden.Checked,textCarrier.Text,textPhone.Text);
			for(int i=0;i<table.Rows.Count;i++){
				row=new GridRow();
				/*if(checkCDAnet.Checked){
					row.Cells.Add(table.Rows[i]["CarrierName"].ToString());
					row.Cells.Add(table.Rows[i]["ElectID"].ToString());
					row.Cells.Add(table.Rows[i]["pMP"].ToString());
					row.Cells.Add(table.Rows[i]["network"].ToString());
					row.Cells.Add(table.Rows[i]["version"].ToString());
					row.Cells.Add(table.Rows[i]["trans02"].ToString());
					row.Cells.Add(table.Rows[i]["trans03"].ToString());
					row.Cells.Add(table.Rows[i]["trans04"].ToString());
					row.Cells.Add(table.Rows[i]["trans05"].ToString());
					row.Cells.Add(table.Rows[i]["trans06"].ToString());
					row.Cells.Add(table.Rows[i]["trans07"].ToString());
					row.Cells.Add(table.Rows[i]["trans08"].ToString());
					row.Cells.Add(table.Rows[i]["isHidden"].ToString());
				}
				else{*/
				row.Cells.Add(table.Rows[i]["CarrierName"].ToString());
				row.Cells.Add(table.Rows[i]["Phone"].ToString());
				if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
					row.Cells[row.Cells.Count-1].ColorText=Color.Blue;
					row.Cells[row.Cells.Count-1].Underline=YN.Yes;
				}
				row.Cells.Add(table.Rows[i]["Address"].ToString());
				//row.Cells.Add(table.Rows[i]["Address2"].ToString());
				row.Cells.Add(table.Rows[i]["City"].ToString());
				row.Cells.Add(table.Rows[i]["State"].ToString());
				row.Cells.Add(table.Rows[i]["Zip"].ToString());
				row.Cells.Add(table.Rows[i]["ElectID"].ToString());
				row.Cells.Add(table.Rows[i]["isHidden"].ToString());
				row.Cells.Add(table.Rows[i]["insPlanCount"].ToString());
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					row.Cells.Add(table.Rows[i]["isCDA"].ToString());
				}
				//}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			for(int i=0;i<table.Rows.Count;i++){
				if(selectedCarrierNums.Contains(table.Rows[i]["CarrierNum"].ToString())){
					gridMain.SetSelected(i,true);
				}
			}
			//if(tbCarriers.SelectedIndices.Length>0){
			//	tbCarriers.ScrollToLine(tbCarriers.SelectedIndices[0]);
			//}
		}

		private void textCarrier_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textPhone_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Carrier carrier=Carriers.GetCarrier(PIn.Long(table.Rows[e.Row]["CarrierNum"].ToString()));
			if(IsSelectMode) {
				SelectedCarrier=carrier;
				DialogResult=DialogResult.OK;
				return;
			}
			using FormCarrierEdit FormCE=new FormCarrierEdit();
			FormCE.CarrierCur=carrier;
			FormCE.ShowDialog();
			if(FormCE.DialogResult!=DialogResult.OK){
				return;
			}
			FillGrid();
			DataValid.SetInvalid(InvalidType.Carriers);
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			GridCell gridCellCur=gridMain.ListGridRows[e.Row].Cells[e.Col];
			//Only grid cells with phone numbers are blue and underlined.
			if(gridCellCur.ColorText==System.Drawing.Color.Blue && gridCellCur.Underline==YN.Yes && Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
				DentalTek.PlaceCall(gridCellCur.Text);
			}
		}

		private void checkCDAnet_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butItransUpdateCarriers_Click(object sender,EventArgs e) {
			List<string> listFields=new List<string>();
			ItransImportFields fieldsToImport=ItransImportFields.None;
			if(checkITransPhone.Checked) {
				fieldsToImport=(fieldsToImport|ItransImportFields.Phone);
				listFields.Add(Lans.g(this,"Phone"));
			}
			if(checkItransAddress.Checked) {
				fieldsToImport=(fieldsToImport|ItransImportFields.Address);
				listFields.Add(Lans.g(this,"Address"));
			}
			if(checkItransName.Checked) {
				fieldsToImport=(fieldsToImport|ItransImportFields.Name);
				listFields.Add(Lans.g(this,"Name"));
			}
			StringBuilder msg=new StringBuilder();
			if(listFields.Count>0) {
				msg.Insert(0,Lans.g(this,"The following carrier fields will be updated and overwritten for carriers matched by their Electronic IDs:")
				+" "+string.Join(", ",listFields)
				+"\r\n");
			}
			if(checkItransMissing.Checked) {
				fieldsToImport=(fieldsToImport|ItransImportFields.AddMissing);
				msg.AppendLine(Lans.g(this,"New carriers will be added if missing from the database based on Electronic ID."));
			}
			if(msg.Length>0) {
				msg.AppendLine(Lans.g(this,"Continue?"));
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,msg.ToString())) {
					return;
				}
			}
			Prefs.UpdateInt(PrefName.ItransImportFields,(int)fieldsToImport);
			DataValid.SetInvalid(InvalidType.Prefs);
			string errorMsg=ItransNCpl.TryCarrierUpdate(false,fieldsToImport);
			if(!string.IsNullOrEmpty(errorMsg)) {
				MsgBox.Show(this,errorMsg);
				return;
			}
			MsgBox.Show(this,"Done.");
			DataValid.SetInvalid(InvalidType.Carriers);
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormCarrierEdit FormCE=new FormCarrierEdit();
			FormCE.IsNew=true;
			Carrier carrier=new Carrier();
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				carrier.IsCDA=true;
			}
			carrier.CarrierName=textCarrier.Text;
			//The phone number will get formated while the user types inside the carrier edit window.
			//However, the user could have typed in a poorly formatted number so we will reformat the number once before load.
			string phoneFormatted=TelephoneNumbers.ReFormat(textPhone.Text);
			carrier.Phone=phoneFormatted;
			FormCE.CarrierCur=carrier;
			FormCE.ShowDialog();
			if(FormCE.DialogResult!=DialogResult.OK){
				return;
			}
			//Load the name and phone number of the newly added carrier to the search fields so that the new carrier shows up in the grid.
			textCarrier.Text=FormCE.CarrierCur.CarrierName;
			textPhone.Text=FormCE.CarrierCur.Phone;
			FillGrid();
			for(int i=0;i<table.Rows.Count;i++){
				if(FormCE.CarrierCur.CarrierNum.ToString()==table.Rows[i]["CarrierNum"].ToString()){
					gridMain.SetSelected(i,true);
				}
			}
			DataValid.SetInvalid(InvalidType.Carriers);
		}

		private void butCombine_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsCarrierCombine)) {
				return;
			}
			if(gridMain.SelectedIndices.Length<2){
				MessageBox.Show(Lan.g(this,"Please select multiple items first while holding down the control key."));
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Combine all these carriers into a single carrier? This will affect all patients using these carriers.  The next window will let you select which carrier to keep when combining."),""
				,MessageBoxButtons.OKCancel)!=DialogResult.OK)
			{
				return;
			}
			List<long> pickedCarrierNums=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				pickedCarrierNums.Add(PIn.Long(table.Rows[gridMain.SelectedIndices[i]]["CarrierNum"].ToString()));
			}
			using FormCarrierCombine FormCB=new FormCarrierCombine();
			FormCB.CarrierNums=pickedCarrierNums;
			FormCB.ShowDialog();
			if(FormCB.DialogResult!=DialogResult.OK){
				return;
			}
			if(!VerifyCarrierCombineData(FormCB.PickedCarrierNum,pickedCarrierNums)) {
				return;
			}
			List<Carrier> listCarriers=Carriers.GetCarriers(pickedCarrierNums);
			string carrierName=listCarriers.FirstOrDefault(x => x.CarrierNum==FormCB.PickedCarrierNum)?.CarrierName??"";
			string carrierNames=string.Join(", ",listCarriers.Where(x => x.CarrierNum!=FormCB.PickedCarrierNum).Select(x => $"'{x.CarrierName}'"));
			try {
				Carriers.Combine(pickedCarrierNums,FormCB.PickedCarrierNum);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DataValid.SetInvalid(InvalidType.Carriers);
			string logText=Lan.g(this,"The following carriers were combined into")+$" '{carrierName}': {carrierNames}";
			SecurityLogs.MakeLogEntry(Permissions.InsCarrierCombine,0,logText);
			FillGrid();
		}

		private bool VerifyCarrierCombineData(long pickedCarrierNum,List<long> pickedCarrierNums) {
			List<Carrier> listCarriers = Carriers.GetCarriers(pickedCarrierNums);
			List<string> listWarnings = new List<string>();
			Carrier carCur = listCarriers.FirstOrDefault(x => x.CarrierNum==pickedCarrierNum);
			if(carCur==null) {//In case it wasn't included in the list of picked carrier nums.
				carCur=Carriers.GetCarrier(pickedCarrierNum);
			}
			if(carCur==null) {//In case it is a completely invalid carrier
				return false;//should never happen.
			}
			//==================== NAME ====================
			if(listCarriers.Any(x=>x.CarrierName!=carCur.CarrierName && !string.IsNullOrWhiteSpace(x.CarrierName))) {
				listWarnings.Add(Lan.g(this,"Carrier Name"));
			}
			//==================== ADDRESS INFO ====================
			if(listCarriers.Any(x => x.Address!=carCur.Address && !string.IsNullOrWhiteSpace(x.Address))
				|| listCarriers.Any(x => x.Address2!=carCur.Address2 && !string.IsNullOrWhiteSpace(x.Address2))
				|| listCarriers.Any(x => x.City!=carCur.City && !string.IsNullOrWhiteSpace(x.City))
				|| listCarriers.Any(x => x.State!=carCur.State && !string.IsNullOrWhiteSpace(x.State))
				|| listCarriers.Any(x => x.Zip!=carCur.Zip && !string.IsNullOrWhiteSpace(x.Zip))) 
			{
				listWarnings.Add(Lan.g(this,"Carrier Address"));
			}
			//==================== PHONE ====================
			if(listCarriers.Any(x => x.Phone!=carCur.Phone && !string.IsNullOrWhiteSpace(x.Phone))) {
				listWarnings.Add(Lan.g(this,"Carrier Phone"));
			}
			//==================== ElectID ====================
			if(listCarriers.Any(x => x.ElectID!=carCur.ElectID && !string.IsNullOrWhiteSpace(x.ElectID))) {
				listWarnings.Add(Lan.g(this,"Carrier ElectID"));
			}
			//==================== TIN ====================
			if(listCarriers.Any(x => x.TIN!=carCur.TIN && !string.IsNullOrWhiteSpace(x.TIN))) {
				listWarnings.Add(Lan.g(this,"Carrier TIN"));
			}
			//==================== CDAnetVersion ====================
			if(listCarriers.Any(x => x.CDAnetVersion!=carCur.CDAnetVersion && !string.IsNullOrWhiteSpace(x.CDAnetVersion))) {
				listWarnings.Add(Lan.g(this,"Carrier CDAnet Version"));
			}
			//==================== IsCDA ====================
			if(listCarriers.Any(x=>x.IsCDA!=carCur.IsCDA)) {
				listWarnings.Add(Lan.g(this,"Carrier Is CDA"));
			}
			//==================== CanadianNetworkNum ====================
			if(listCarriers.Any(x => x.CanadianNetworkNum!=carCur.CanadianNetworkNum)) {
				listWarnings.Add(Lan.g(this,"Canadian Network"));
			}
			//==================== NoSendElect ====================
			if(listCarriers.Any(x => x.NoSendElect!=carCur.NoSendElect)) {
				listWarnings.Add(Lan.g(this,"Send Elect"));
			}
			//==================== IsHidden ====================
			if(listCarriers.Any(x => x.IsHidden!=carCur.IsHidden)) {
				listWarnings.Add(Lan.g(this,"Is Hidden"));
			}
			//==================== CanadianEncryptionMethod ====================
			if(listCarriers.Any(x => x.CanadianEncryptionMethod!=carCur.CanadianEncryptionMethod)) {
				listWarnings.Add(Lan.g(this,"Canadian Encryption Method"));
			}
			//==================== CanadianSupportedTypes ====================
			if(listCarriers.Any(x => x.CanadianSupportedTypes!=carCur.CanadianSupportedTypes)) {
				listWarnings.Add(Lan.g(this,"Canadian Supported Types"));
			}
			//==================== Additional fields ====================
			//If anyone asks for them, these fields can also be checked.
			// public long							SecUserNumEntry;
			// public DateTime					SecDateEntry;
			// public DateTime					SecDateTEdit;
			//====================USER PROMPT====================
			if(listWarnings.Count>0) {
				string warningMessage=Lan.g(this,"WARNING!")+" "+Lan.g(this,"Mismatched data has been detected between selected carriers")+":\r\n\r\n"
					+string.Join("\r\n",listWarnings)+"\r\n\r\n"
					+Lan.g(this,"Would you like to continue combining carriers anyway?");
				if(MessageBox.Show(warningMessage,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return false;
				}
			}
			return true;
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//only visible if IsSelectMode
			if(gridMain.SelectedIndices.Length==0) {
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(gridMain.SelectedIndices.Length>1) {
				MessageBox.Show(Lan.g(this,"Please select only one item first."));
				return;
			}
			SelectedCarrier=Carriers.GetCarrier(PIn.Long(table.Rows[gridMain.SelectedIndices[0]]["CarrierNum"].ToString()));
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}


























