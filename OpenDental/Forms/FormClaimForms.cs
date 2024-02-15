using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;
using System.Linq;
using CodeBase;
using System.Resources;
using System.Globalization;
using System.Text;
using OpenDental.Thinfinity;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormClaimForms : FormODBase {
		private bool changed;

		///<summary></summary>
		public FormClaimForms()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormClaimForms_Load(object sender, System.EventArgs e) {
			FillGridInternal();
			FillGridCustom();
		}

		private void FillGridInternal() {
			gridInternal.BeginUpdate();
			gridInternal.Columns.Clear();
			gridInternal.Columns.Add(new GridColumn(Lan.g("TableClaimFormsInternal","ClaimForm"),150));
			gridInternal.ListGridRows.Clear();
			List<ClaimForm> listClaimForms=ClaimForms.GetInternalClaims();
			for(int i=0;i<listClaimForms.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listClaimForms[i].Description);
				row.Tag=listClaimForms[i];
				gridInternal.ListGridRows.Add(row);
			}
			gridInternal.EndUpdate();
		}

		///<summary></summary>
		private void FillGridCustom() {
			ClaimFormItems.RefreshCache();
			ClaimForms.RefreshCache();
			comboReassign.Items.Clear();
			gridCustom.BeginUpdate();
			gridCustom.Columns.Clear();
			gridCustom.Columns.Add(new GridColumn(Lan.g("TableClaimFormsCustom","ClaimForm"),145));
			gridCustom.Columns.Add(new GridColumn(Lan.g("TableClaimFormsCustom","Default"),50,HorizontalAlignment.Center));
			gridCustom.Columns.Add(new GridColumn(Lan.g("TableClaimFormsCustom","Hidden"),0,HorizontalAlignment.Center));
			gridCustom.ListGridRows.Clear();
			string description;
			List<ClaimForm> listClaimForms=ClaimForms.GetDeepCopy();
			for(int i=0;i<listClaimForms.Count;i++) {
				description=listClaimForms[i].Description;
				GridRow row=new GridRow();
				row.Cells.Add(listClaimForms[i].Description);
				if(listClaimForms[i].ClaimFormNum==PrefC.GetLong(PrefName.DefaultClaimForm)) {
					description+=" "+Lan.g(this,"(default)");
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				if(listClaimForms[i].IsHidden) {
					description+=" "+Lan.g(this,"(hidden)");
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				row.Tag=listClaimForms[i];
				gridCustom.ListGridRows.Add(row);
				comboReassign.Items.Add(description,listClaimForms[i]);
			}
			gridCustom.EndUpdate();
		}

		///<summary>Copy an internal form over to a new custom form.</summary>
		private void butCopy_Click(object sender,EventArgs e) {
			if(gridInternal.GetSelectedIndex()==-1) {
				MessageBox.Show(Lan.g(this,"Please select an item from the internal grid to copy over to the custom grid."));
				return;
			}
			//just insert it into the db.
			ClaimForm claimFormInternal = (ClaimForm)gridInternal.ListGridRows[gridInternal.GetSelectedIndex()].Tag;
			ClaimForms.Insert(claimFormInternal,includeClaimFormItems:true);
			FillGridCustom();
		}

		private void gridInternal_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Row==-1) {
				return;
			}
			using FormClaimFormEdit formClaimFormEdit=new FormClaimFormEdit((ClaimForm)gridInternal.ListGridRows[e.Row].Tag);
			formClaimFormEdit.ShowDialog();
			if(formClaimFormEdit.DialogResult==DialogResult.OK) {
				changed=true;
			}
		}

		private void gridCustom_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Row==-1) {
				return;
			}
			using FormClaimFormEdit formClaimFormEdit=new FormClaimFormEdit((ClaimForm)gridCustom.ListGridRows[e.Row].Tag);
			formClaimFormEdit.ShowDialog();
			if(formClaimFormEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			changed=true;
			FillGridCustom();
		}

		///<summary>Add a custom claim form.</summary>
		private void butAdd_Click(object sender, System.EventArgs e) {
			ClaimForm claimForm=new ClaimForm();
			ClaimForms.Insert(claimForm,false);
			claimForm.IsNew=true;
			using FormClaimFormEdit formClaimFormEdit=new FormClaimFormEdit(claimForm);
			formClaimFormEdit.ShowDialog();
			if(formClaimFormEdit.DialogResult!=DialogResult.OK){
				return;
			}
			changed=true;
			FillGridCustom();
		}

		///<summary>Delete an unusued custom claim form.</summary>
		private void butDelete_Click(object sender, System.EventArgs e) {
			if(gridCustom.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"Please select a Custom Claim Form first."));
				return;
			}
			ClaimForm claimForm=(ClaimForm)gridCustom.ListGridRows[gridCustom.GetSelectedIndex()].Tag;
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete custom claim form?")) {
				return;
			}
			if(!ClaimForms.Delete(claimForm)){
				MsgBox.Show(this,"Claim form is already in use.");
				return;
			}
			changed=true;
			FillGridCustom();
		}

		///<summary>Duplicate a custom claim form.</summary>
		private void butDuplicate_Click(object sender, System.EventArgs e) {
			if(gridCustom.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a Custom Claim Form first.");
				return;
			}
			ClaimForm claimForm=(ClaimForm)gridCustom.ListGridRows[gridCustom.GetSelectedIndex()].Tag;
			//claimFormCur.UniqueID="";//designates it as a user added claimform
			ClaimForms.Insert(claimForm,includeClaimFormItems:true);//this duplicates the original claimform, but no items.
			changed=true;
			FillGridCustom();
		}

		///<summary>Export a custom claim form. Even though we could probably allow this for internal claim forms as well, 
		///users can always copy over an internal claim form to a custom form and then export it.</summary>
		private void butExport_Click(object sender, System.EventArgs e) {
			if(gridCustom.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a Custom Claim Form first.");
				return;
			}
			ClaimForm claimForm=(ClaimForm)gridCustom.ListGridRows[gridCustom.GetSelectedIndex()].Tag;
			string filename = "ClaimForm"+claimForm.Description+".xml";
			if(ODBuild.IsThinfinity()) {
				StringBuilder stringBuilder=new StringBuilder();
				XmlWriter xmlWriter=XmlWriter.Create(stringBuilder);
				XmlSerializer xmlSerializerWeb=new XmlSerializer(typeof(ClaimForm));
				xmlSerializerWeb.Serialize(xmlWriter,claimForm);
				xmlWriter.Close();
				ThinfinityUtils.ExportForDownload(filename,stringBuilder.ToString());
				return;
			}
			using SaveFileDialog saveFileDialog=new SaveFileDialog();
			try {
				saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
			}
			catch(Exception ex) {
				ex.DoNothing();
				MsgBox.Show(this,"Export failed.  This could be due to lack of permissions in the designated folder.");
				return;
			}
			saveFileDialog.FileName=filename;
			if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			XmlSerializer xmlSerializer=new XmlSerializer(typeof(ClaimForm));
			using TextWriter textWriter=new StreamWriter(saveFileDialog.FileName);
			xmlSerializer.Serialize(textWriter,claimForm);
			MsgBox.Show(this,"Exported");
		}

		///<summary>Import an XML file into the custom claim forms list.</summary>
		private void butImport_Click(object sender, System.EventArgs e) {
			OpenFileDialog openFileDialog=new OpenFileDialog();
			openFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
			ClaimForm claimForm;
			if(openFileDialog.ShowDialog()!=DialogResult.OK){
				return;
			}
			try{
				claimForm=ClaimForms.DeserializeClaimForm(openFileDialog.FileName,"");
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			ClaimForms.Insert(claimForm,includeClaimFormItems:true);//now we have a primary key.
			MsgBox.Show(this,"Imported");
			changed=true;
			FillGridCustom();
		}		

		///<summary>Sets a custom claim form as the default.  We do not currently allow setting internal claim forms as default - users need to copy it over first.</summary>
		private void butDefault_Click(object sender,EventArgs e) {
			if(gridCustom.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a claimform from the list first.");
				return;
			}
			ClaimForm claimForm=(ClaimForm)gridCustom.ListGridRows[gridCustom.GetSelectedIndex()].Tag;
			if(Prefs.UpdateLong(PrefName.DefaultClaimForm,claimForm.ClaimFormNum)){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			FillGridCustom();
		}

		///<summary>Reassigns all current insurance plans using the selected claimform to another claimform.</summary>
		private void butReassign_Click(object sender,EventArgs e) {
			if(gridCustom.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a claimform from the list at the left first.");
				return;
			}
			if(comboReassign.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a claimform from the list below.");
				return;
			}
			ClaimForm claimFormCur = (ClaimForm)gridCustom.ListGridRows[gridCustom.GetSelectedIndex()].Tag;
			ClaimForm claimFormNew = comboReassign.GetSelected<ClaimForm>();
			long result=ClaimForms.Reassign(claimFormCur.ClaimFormNum,claimFormNew.ClaimFormNum);
			MessageBox.Show(result.ToString()+Lan.g(this," plans changed."));
		}

		private void FormClaimForms_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.ClaimForms);
			}
		}
	}
}