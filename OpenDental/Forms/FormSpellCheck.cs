using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormSpellCheck:FormODBase {
		private List<DictCustom> _listDictCustoms;

		public FormSpellCheck() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSpellCheck_Load(object sender,EventArgs e) {
			//If the Ime Composition Compatibility preference is enabled, disable the spell check, and display a label notify the user 
			//that the spell check is disabled when the foreign language input method editor is enabled.
			if(PrefC.GetBool(PrefName.ImeCompositionCompatibility)) {
				checkBox1.Checked=false;
				checkBox1.Enabled=false;
				label1.Visible=true;
			}
			else {
				checkBox1.Checked=PrefC.GetBool(PrefName.SpellCheckIsEnabled);
			}
			FillGrid();
		}

		private void FillGrid() {
			_listDictCustoms=DictCustoms.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("",200,false);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listDictCustoms.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listDictCustoms[i].WordText);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(textCustom.Text=="") {
				MsgBox.Show(this,"Please enter a custom word first.");
				return;
			}
			string newWord=Regex.Replace(textCustom.Text,"[\\s]|[\\p{P}\\p{S}-['-]]","");//don't allow words with spaces or punctuation except ' and - in them
			for(int i=0;i<_listDictCustoms.Count;i++) {//Make sure it's not already in the custom list
				if(_listDictCustoms[i].WordText.ToLower()==newWord.ToLower()) {
					MsgBox.Show(this,"The word "+newWord+" is already in the custom word list.");
					textCustom.Clear();
					return;
				}
			}
			DictCustom dictCustomWord=new DictCustom();
			dictCustomWord.WordText=newWord;
			DictCustoms.Insert(dictCustomWord);
			DataValid.SetInvalid(InvalidType.DictCustoms);
			textCustom.Clear();
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			InputBoxParam inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.TextBox;
			inputBoxParam.LabelText="Edit word";
			DictCustom dictCustomOrigWord=_listDictCustoms[e.Row];
			inputBoxParam.Text=dictCustomOrigWord.WordText;
			InputBox inputBoxEditWord=new InputBox(inputBoxParam);
			inputBoxEditWord.ShowDialog();
			if(inputBoxEditWord.IsDialogCancel) {
				return;
			}
			if(inputBoxEditWord.StringResult==dictCustomOrigWord.WordText) {
				return;
			}
			if(inputBoxEditWord.StringResult=="") {
				DictCustoms.Delete(dictCustomOrigWord.DictCustomNum);
				DataValid.SetInvalid(InvalidType.DictCustoms);
				FillGrid();
				return;
			}
			string newWord=Regex.Replace(inputBoxEditWord.StringResult,"[\\s]|[\\p{P}\\p{S}-['-]]","");//don't allow words with spaces or punctuation except ' and - in them
			for(int i=0;i<_listDictCustoms.Count;i++) {//Make sure it's not already in the custom list
				if(_listDictCustoms[i].WordText==newWord) {
					MsgBox.Show(this,"The word "+newWord+" is already in the custom word list.");
					//inputBoxEditWord.TextBoxResult.Text=dictCustomOrigWord.WordText;//this line was doing nothing, so I commented it out.
					return;
				}
			}
			dictCustomOrigWord.WordText=newWord;
			DictCustoms.Update(dictCustomOrigWord);
			DataValid.SetInvalid(InvalidType.DictCustoms);
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Select a word to delete first.");
				return;
			}
			DictCustoms.Delete(_listDictCustoms[gridMain.GetSelectedIndex()].DictCustomNum);
			DataValid.SetInvalid(InvalidType.DictCustoms);
			FillGrid();
		}

		private void FormSpellCheck_FormClosing(object sender,FormClosingEventArgs e) {
			if(Prefs.UpdateBool(PrefName.SpellCheckIsEnabled,checkBox1.Checked)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

	}
}