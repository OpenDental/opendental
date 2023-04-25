using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Globalization;
using System.IO;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
///<summary></summary>
	public partial class FormTranslation : FormODBase {
		private List<Language> _listLanguages;
		private string _classType;
		private List<LanguageForeign> _listLanguageForeigns;

		///<summary></summary>
		public FormTranslation(string classType){
			InitializeComponent();
			InitializeLayoutManager();
			gridLan.Title=classType+" Words";
			
			//tbLan.Fields[2]=CultureInfo.CurrentCulture.Parent.DisplayName;
			//tbLan.Fields[3]=CultureInfo.CurrentCulture.Parent.DisplayName + " Comments";
			//no need to translate much here
			Lan.C("All", new System.Windows.Forms.Control[] {
				butClose,
			});
			_classType=classType;
		}

		private void FormLanguage_Load(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			_listLanguages=Lans.GetListForCat(_classType);
			_listLanguageForeigns=LanguageForeigns.GetListForType(_classType);
			LanguageForeigns.RefreshCache();
			gridLan.BeginUpdate();
			gridLan.Columns.Clear();
			GridColumn col=new GridColumn("English",220);
			gridLan.Columns.Add(col);
			col=new GridColumn(CultureInfo.CurrentCulture.DisplayName,220);
			gridLan.Columns.Add(col);
			col=new GridColumn("Other "+CultureInfo.CurrentCulture.Parent.DisplayName+" Translation",220);
			gridLan.Columns.Add(col);
			col=new GridColumn(CultureInfo.CurrentCulture.DisplayName+" Comments",220);
			gridLan.Columns.Add(col);
			//gridLan.Columns[1].Heading=;
			//gridLan.Columns[2].Heading="Other "+CultureInfo.CurrentCulture.Parent.DisplayName+" Translation";
			//gridLan.Columns[3].Heading=CultureInfo.CurrentCulture.DisplayName+" Comments";
			gridLan.ListGridRows.Clear();
			UI.GridRow row;
			LanguageForeign languageForeign;
			LanguageForeign languageForeignOther;
			for(int i=0;i<_listLanguages.Count;i++){
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(_listLanguages[i].English);
				languageForeign=LanguageForeigns.GetForCulture(_listLanguageForeigns,_listLanguages[i].English,CultureInfo.CurrentCulture.Name);
				languageForeignOther=LanguageForeigns.GetOther(_listLanguageForeigns,_listLanguages[i].English,CultureInfo.CurrentCulture.Name);
				if(languageForeign==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(languageForeign.Translation);
				}
				if(languageForeignOther==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(languageForeignOther.Translation);
				}
				if(languageForeign==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(languageForeign.Comments);
				}
				gridLan.ListGridRows.Add(row);
			}
			gridLan.EndUpdate();
		}

		private void gridLan_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			Language language=_listLanguages[e.Row];
			LanguageForeign languageForeign=LanguageForeigns.GetForCulture(_listLanguageForeigns,language.English,CultureInfo.CurrentCulture.Name);
			LanguageForeign languageForeignOther=LanguageForeigns.GetOther(_listLanguageForeigns,language.English,CultureInfo.CurrentCulture.Name);
			string otherTrans="";
			if(languageForeignOther!=null){
				otherTrans=languageForeignOther.Translation;
			}
			using FormTranslationEdit formTranslationEdit=new FormTranslationEdit(language,languageForeign,otherTrans);
			formTranslationEdit.ShowDialog();
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridLan.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			List<string> listStrings_=new List<string>();
			for(int i=0;i<gridLan.SelectedIndices.Length;i++){
				listStrings_.Add(_listLanguages[gridLan.SelectedIndices[i]].English);
			}
			Lans.DeleteItems(_classType,listStrings_);
			FillGrid();
		}

		private void butDeleteUnused_Click(object sender,EventArgs e) {
			List<string> listStrings_=new List<string>();
			LanguageForeign languageForeign;
			LanguageForeign languageForeignOther;
			for(int i=0;i<_listLanguages.Count;i++){
				languageForeign=LanguageForeigns.GetForCulture(_listLanguageForeigns,_listLanguages[i].English,CultureInfo.CurrentCulture.Name);
				languageForeignOther=LanguageForeigns.GetOther(_listLanguageForeigns,_listLanguages[i].English,CultureInfo.CurrentCulture.Name);
				if(languageForeign==null && languageForeignOther==null){
					listStrings_.Add(_listLanguages[i].English);
				}
			}
			if(listStrings_.Count==0){
				MsgBox.Show(this,"All unused rows have already been deleted.");
				return;
			}
			Lans.DeleteItems(_classType,listStrings_);
			FillGrid();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















