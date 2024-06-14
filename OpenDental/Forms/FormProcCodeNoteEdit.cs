using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.UI;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormProcCodeNoteEdit : FormODBase {
		public bool IsNew;
		private StringBuilder _stringBuilderTime;
		public ProcCodeNote ProcCodeNoteCur;
		private bool _isMouseDown;
		private Point _pointMouseOrigin;
		private Point _pointSliderOrigin;
		private bool _hasChanged=false;
		///<summary>Set to true to use the corresponding DefaultTPNote, otherwise uses the "complete" default note.</summary>
		private bool _isTp;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormProcCodeNoteEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			tbTime.CellClicked += new OpenDental.ContrTable.CellEventHandler(tbTime_CellClicked);
		}

		public FormProcCodeNoteEdit(bool isTp)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			tbTime.CellClicked+=new OpenDental.ContrTable.CellEventHandler(tbTime_CellClicked);
			_isTp=isTp;
		}

		private void FormProcCodeNoteEdit_Load(object sender,EventArgs e) {
			_listProviders=Providers.GetDeepCopy(true);
			_stringBuilderTime=new StringBuilder(ProcCodeNoteCur.ProcTime);
			for(int i=0;i<_listProviders.Count;i++){
				listProv.Items.Add(_listProviders[i].GetLongDesc());
				if(ProcCodeNoteCur.ProvNum==_listProviders[i].ProvNum){
					listProv.SelectedIndex=i;
				}
			}
			textNote.Text=ProcCodeNoteCur.Note;
			FillTime();
		}

		private void FillTime() {
			for(int i=0;i<_stringBuilderTime.Length;i++) {
				tbTime.Cell[0,i]=_stringBuilderTime.ToString(i,1);
				tbTime.BackGColor[0,i]=Color.White;
			}
			for(int i=_stringBuilderTime.Length;i<tbTime.MaxRows;i++) {
				tbTime.Cell[0,i]="";
				tbTime.BackGColor[0,i]=Color.FromName("Control");
			}
			tbTime.Refresh();
			butSlider.Location=new Point(tbTime.Location.X+2
				,(tbTime.Location.Y+_stringBuilderTime.Length*14+1));
			textTime2.Text=(_stringBuilderTime.Length*PrefC.GetInt(PrefName.AppointmentTimeIncrement)).ToString();
		}

		private void tbTime_CellClicked(object sender,CellEventArgs e) {
			if(e.Row<_stringBuilderTime.Length) {
				if(_stringBuilderTime[e.Row]=='/') {
					_stringBuilderTime.Replace('/','X',e.Row,1);
				}
				else {
					_stringBuilderTime.Replace(_stringBuilderTime[e.Row],'/',e.Row,1);
				}
			}
			FillTime();
		}

		private void butSlider_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			_isMouseDown=true;
			_pointMouseOrigin=new Point(e.X+butSlider.Location.X
				,e.Y+butSlider.Location.Y);
			_pointSliderOrigin=butSlider.Location;

		}

		private void butSlider_MouseMove(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(!_isMouseDown)
				return;
			//tempPoint represents the new location of button of smooth dragging.
			Point point=new Point(_pointSliderOrigin.X
				,_pointSliderOrigin.Y+(e.Y+butSlider.Location.Y)-_pointMouseOrigin.Y);
			int step=(int)(Math.Round((Decimal)(point.Y-tbTime.Location.Y)/14));
			if(step==_stringBuilderTime.Length)
				return;
			if(step<1)
				return;
			if(step>tbTime.MaxRows-1)
				return;
			if(step>_stringBuilderTime.Length) {
				_stringBuilderTime.Append('/');
			}
			if(step<_stringBuilderTime.Length) {
				_stringBuilderTime.Remove(step,1);
			}
			FillTime();
		}

		private void butSlider_MouseUp(object sender,System.Windows.Forms.MouseEventArgs e) {
			_isMouseDown=false;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			ProcCodeNotes.Delete(ProcCodeNoteCur.ProcCodeNoteNum);
			_hasChanged=true;
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(listProv.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a provider first.");
				return;
			}
			ProcCodeNoteCur.ProcTime=_stringBuilderTime.ToString();
			ProcCodeNoteCur.ProvNum=_listProviders[listProv.SelectedIndex].ProvNum;
			ProcCodeNoteCur.Note=textNote.Text;
			if(_isTp) {
				ProcCodeNoteCur.ProcStatus=ProcStat.TP;
			}
			else{
				ProcCodeNoteCur.ProcStatus=ProcStat.C;
			}
			if(IsNew){
				ProcCodeNotes.Insert(ProcCodeNoteCur);
			}
			else{
				ProcCodeNotes.Update(ProcCodeNoteCur);
			}
			_hasChanged=true;
			DialogResult=DialogResult.OK;
		}

		private void FormProcCodeNoteEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(_hasChanged) {
				DataValid.SetInvalid(InvalidType.ProcCodes);
			}
		}

	}
}