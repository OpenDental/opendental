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
			gridInternal.ListGridColumns.Clear();
			gridInternal.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimFormsInternal","ClaimForm"),150));
			gridInternal.ListGridRows.Clear();
			foreach(ClaimForm internalForm in ClaimForms.GetInternalClaims()) {
				GridRow row = new GridRow();
				row.Cells.Add(internalForm.Description);
				row.Tag = internalForm;
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
			gridCustom.ListGridColumns.Clear();
			gridCustom.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimFormsCustom","ClaimForm"),145));
			gridCustom.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimFormsCustom","Default"),50,HorizontalAlignment.Center));
			gridCustom.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimFormsCustom","Hidden"),0,HorizontalAlignment.Center));
			gridCustom.ListGridRows.Clear();
			string description;
			foreach(ClaimForm claimFormCur in ClaimForms.GetDeepCopy()) {
				description=claimFormCur.Description;
				GridRow row = new GridRow();
				row.Cells.Add(claimFormCur.Description);
				if(claimFormCur.ClaimFormNum==PrefC.GetLong(PrefName.DefaultClaimForm)) {
					description+=" "+Lan.g(this,"(default)");
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				if(claimFormCur.IsHidden) {
					description+=" "+Lan.g(this,"(hidden)");
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				row.Tag = claimFormCur;
				gridCustom.ListGridRows.Add(row);
				comboReassign.Items.Add(description,claimFormCur);
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
			long claimFormNum = ClaimForms.Insert(claimFormInternal,true);
			FillGridCustom();
		}

		private void gridInternal_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Row==-1) {
				return;
			}
			using FormClaimFormEdit FormCFE=new FormClaimFormEdit((ClaimForm)gridInternal.ListGridRows[e.Row].Tag);
			FormCFE.ShowDialog();
			if(FormCFE.DialogResult==DialogResult.OK) {
				changed=true;
			}
		}

		private void gridCustom_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Row==-1) {
				return;
			}
			using FormClaimFormEdit FormCFE=new FormClaimFormEdit((ClaimForm)gridCustom.ListGridRows[e.Row].Tag);
			FormCFE.ShowDialog();
			if(FormCFE.DialogResult!=DialogResult.OK) {
				return;
			}
			changed=true;
			FillGridCustom();
		}

		///<summary>Add a custom claim form.</summary>
		private void butAdd_Click(object sender, System.EventArgs e) {
			ClaimForm ClaimFormCur=new ClaimForm();
			ClaimForms.Insert(ClaimFormCur,false);
			ClaimFormCur.IsNew=true;
			using FormClaimFormEdit FormCFE=new FormClaimFormEdit(ClaimFormCur);
			FormCFE.ShowDialog();
			if(FormCFE.DialogResult!=DialogResult.OK){
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
			ClaimForm claimFormCur = (ClaimForm)gridCustom.ListGridRows[gridCustom.GetSelectedIndex()].Tag;
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete custom claim form?")) {
				return;
			}
			if(!ClaimForms.Delete(claimFormCur)){
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
			ClaimForm claimFormCur = (ClaimForm)gridCustom.ListGridRows[gridCustom.GetSelectedIndex()].Tag;
			long oldClaimFormNum=claimFormCur.ClaimFormNum;
			//claimFormCur.UniqueID="";//designates it as a user added claimform
			ClaimForms.Insert(claimFormCur,true);//this duplicates the original claimform, but no items.
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
			ClaimForm claimFormCur = (ClaimForm)gridCustom.ListGridRows[gridCustom.GetSelectedIndex()].Tag;
			string filename = "ClaimForm"+claimFormCur.Description+".xml";
			if(ODBuild.IsWeb()) {
				StringBuilder strbuild=new StringBuilder();
				using(XmlWriter writer=XmlWriter.Create(strbuild)) {
					XmlSerializer serializer=new XmlSerializer(typeof(ClaimForm));
					serializer.Serialize(writer,claimFormCur);
				}
				ThinfinityUtils.ExportForDownload(filename,strbuild.ToString());
				return;
			}
			try {
				using(SaveFileDialog saveDlg=new SaveFileDialog()) {
					saveDlg.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
					saveDlg.FileName=filename;
					if(saveDlg.ShowDialog()!=DialogResult.OK) {
						return;
					}
					XmlSerializer serializer=new XmlSerializer(typeof(ClaimForm));
					using(TextWriter writer=new StreamWriter(saveDlg.FileName)) {
						serializer.Serialize(writer,claimFormCur);
					}
				}
				MsgBox.Show(this,"Exported");
			}
			catch(Exception ex) {
				ex.DoNothing();
				MsgBox.Show(this,"Export failed.  This could be due to lack of permissions in the designated folder.");
			}
		}

		///<summary>Import an XML file into the custom claim forms list.</summary>
		private void butImport_Click(object sender, System.EventArgs e) {
			OpenFileDialog openDlg=new OpenFileDialog();
			openDlg.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
			ClaimForm claimForm;
			if(openDlg.ShowDialog()!=DialogResult.OK){
				return;
			}
			try{
				claimForm=ClaimForms.DeserializeClaimForm(openDlg.FileName,"");
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			ClaimForms.Insert(claimForm,true);//now we have a primary key.
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
			ClaimForm claimFormCur = (ClaimForm)gridCustom.ListGridRows[gridCustom.GetSelectedIndex()].Tag;
			if(Prefs.UpdateLong(PrefName.DefaultClaimForm,claimFormCur.ClaimFormNum)){
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

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormClaimForms_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.ClaimForms);
			}
		}
	}
}





















