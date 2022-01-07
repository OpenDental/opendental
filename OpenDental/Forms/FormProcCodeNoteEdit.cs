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
		private StringBuilder strBTime;
		public ProcCodeNote NoteCur;
		private bool mouseIsDown;
		private Point mouseOrigin;
		private Point sliderOrigin;
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
			strBTime=new StringBuilder(NoteCur.ProcTime);
			for(int i=0;i<_listProviders.Count;i++){
				listProv.Items.Add(_listProviders[i].GetLongDesc());
				if(NoteCur.ProvNum==_listProviders[i].ProvNum){
					listProv.SelectedIndex=i;
				}
			}
			textNote.Text=NoteCur.Note;
			FillTime();
		}

		private void FillTime() {
			for(int i=0;i<strBTime.Length;i++) {
				tbTime.Cell[0,i]=strBTime.ToString(i,1);
				tbTime.BackGColor[0,i]=Color.White;
			}
			for(int i=strBTime.Length;i<tbTime.MaxRows;i++) {
				tbTime.Cell[0,i]="";
				tbTime.BackGColor[0,i]=Color.FromName("Control");
			}
			tbTime.Refresh();
			butSlider.Location=new Point(tbTime.Location.X+2
				,(tbTime.Location.Y+strBTime.Length*14+1));
			textTime2.Text=(strBTime.Length*PrefC.GetInt(PrefName.AppointmentTimeIncrement)).ToString();
		}

		private void tbTime_CellClicked(object sender,CellEventArgs e) {
			if(e.Row<strBTime.Length) {
				if(strBTime[e.Row]=='/') {
					strBTime.Replace('/','X',e.Row,1);
				}
				else {
					strBTime.Replace(strBTime[e.Row],'/',e.Row,1);
				}
			}
			FillTime();
		}

		private void butSlider_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			mouseIsDown=true;
			mouseOrigin=new Point(e.X+butSlider.Location.X
				,e.Y+butSlider.Location.Y);
			sliderOrigin=butSlider.Location;

		}

		private void butSlider_MouseMove(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(!mouseIsDown)
				return;
			//tempPoint represents the new location of button of smooth dragging.
			Point tempPoint=new Point(sliderOrigin.X
				,sliderOrigin.Y+(e.Y+butSlider.Location.Y)-mouseOrigin.Y);
			int step=(int)(Math.Round((Decimal)(tempPoint.Y-tbTime.Location.Y)/14));
			if(step==strBTime.Length)
				return;
			if(step<1)
				return;
			if(step>tbTime.MaxRows-1)
				return;
			if(step>strBTime.Length) {
				strBTime.Append('/');
			}
			if(step<strBTime.Length) {
				strBTime.Remove(step,1);
			}
			FillTime();
		}

		private void butSlider_MouseUp(object sender,System.Windows.Forms.MouseEventArgs e) {
			mouseIsDown=false;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			ProcCodeNotes.Delete(NoteCur.ProcCodeNoteNum);
			_hasChanged=true;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listProv.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a provider first.");
				return;
			}
			NoteCur.ProcTime=strBTime.ToString();
			NoteCur.ProvNum=_listProviders[listProv.SelectedIndex].ProvNum;
			NoteCur.Note=textNote.Text;
			if(_isTp) {
				NoteCur.ProcStatus=ProcStat.TP;
			}
			else{
				NoteCur.ProcStatus=ProcStat.C;
			}
			if(IsNew){
				ProcCodeNotes.Insert(NoteCur);
			}
			else{
				ProcCodeNotes.Update(NoteCur);
			}
			_hasChanged=true;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormProcCodeNoteEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(_hasChanged) {
				DataValid.SetInvalid(InvalidType.ProcCodes);
			}
		}
	}
}





















