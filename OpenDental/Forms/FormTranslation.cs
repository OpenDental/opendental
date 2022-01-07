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
		private Language[] LanList;
		private string ClassType;
		private LanguageForeign[] ListForType;

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
			ClassType=classType;
		}

		private void FormLanguage_Load(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			LanList=Lans.GetListForCat(ClassType);
			ListForType=LanguageForeigns.GetListForType(ClassType);
			LanguageForeigns.RefreshCache();
			gridLan.BeginUpdate();
			gridLan.ListGridColumns.Clear();
			GridColumn column=new GridColumn("English",220);
			gridLan.ListGridColumns.Add(column);
			column=new GridColumn(CultureInfo.CurrentCulture.DisplayName,220);
			gridLan.ListGridColumns.Add(column);
			column=new GridColumn("Other "+CultureInfo.CurrentCulture.Parent.DisplayName+" Translation",220);
			gridLan.ListGridColumns.Add(column);
			column=new GridColumn(CultureInfo.CurrentCulture.DisplayName+" Comments",220);
			gridLan.ListGridColumns.Add(column);
			//gridLan.Columns[1].Heading=;
			//gridLan.Columns[2].Heading="Other "+CultureInfo.CurrentCulture.Parent.DisplayName+" Translation";
			//gridLan.Columns[3].Heading=CultureInfo.CurrentCulture.DisplayName+" Comments";
			gridLan.ListGridRows.Clear();
			UI.GridRow row;
			LanguageForeign lanForeign;
			LanguageForeign lanForeignOther;
			for(int i=0;i<LanList.Length;i++){
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(LanList[i].English);
				lanForeign=LanguageForeigns.GetForCulture(ListForType,LanList[i].English,CultureInfo.CurrentCulture.Name);
				lanForeignOther=LanguageForeigns.GetOther(ListForType,LanList[i].English,CultureInfo.CurrentCulture.Name);
				if(lanForeign==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(lanForeign.Translation);
				}
				if(lanForeignOther==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(lanForeignOther.Translation);
				}
				if(lanForeign==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(lanForeign.Comments);
				}
				gridLan.ListGridRows.Add(row);
			}
			gridLan.EndUpdate();
		}

		private void gridLan_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			Language LanCur=LanList[e.Row];
			LanguageForeign lanForeign=LanguageForeigns.GetForCulture(ListForType,LanCur.English,CultureInfo.CurrentCulture.Name);
			LanguageForeign lanForeignOther=LanguageForeigns.GetOther(ListForType,LanCur.English,CultureInfo.CurrentCulture.Name);
			string otherTrans="";
			if(lanForeignOther!=null){
				otherTrans=lanForeignOther.Translation;
			}
			using FormTranslationEdit FormTE=new FormTranslationEdit(LanCur,lanForeign,otherTrans);
			FormTE.ShowDialog();
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridLan.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			List<string> strList=new List<string>();
			for(int i=0;i<gridLan.SelectedIndices.Length;i++){
				strList.Add(LanList[gridLan.SelectedIndices[i]].English);
			}
			Lans.DeleteItems(ClassType,strList);
			FillGrid();
		}

		private void butDeleteUnused_Click(object sender,EventArgs e) {
			List<string> strList=new List<string>();
			LanguageForeign lanForeign;
			LanguageForeign lanForeignOther;
			for(int i=0;i<LanList.Length;i++){
				lanForeign=LanguageForeigns.GetForCulture(ListForType,LanList[i].English,CultureInfo.CurrentCulture.Name);
				lanForeignOther=LanguageForeigns.GetOther(ListForType,LanList[i].English,CultureInfo.CurrentCulture.Name);
				if(lanForeign==null && lanForeignOther==null){
					strList.Add(LanList[i].English);
				}
			}
			if(strList.Count==0){
				MsgBox.Show(this,"All unused rows have already been deleted.");
				return;
			}
			Lans.DeleteItems(ClassType,strList);
			FillGrid();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

	}
}





















