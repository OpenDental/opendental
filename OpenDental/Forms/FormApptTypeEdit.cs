using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormApptTypeEdit:FormODBase {
		public AppointmentType AppointmentTypeCur;
		private bool mouseIsDown;
		private Point mouseOrigin;
		private Point sliderOrigin;
		///<summary>The string time pattern in the current increment. Not in the 5 minute increment.</summary>
		private StringBuilder _strBTime;
		private System.Drawing.Color _provColor;
		private List<ProcedureCode> _listProcCodes;

		public FormApptTypeEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormApptTypeEdit_Load(object sender,EventArgs e) {
			textName.Text=AppointmentTypeCur.AppointmentTypeName;
			butColor.BackColor=AppointmentTypeCur.AppointmentTypeColor;
			checkIsHidden.Checked=AppointmentTypeCur.IsHidden;
			_listProcCodes=ProcedureCodes.GetFromCommaDelimitedList(AppointmentTypeCur.CodeStr);
			_provColor=Color.Blue;
			_strBTime=new StringBuilder();
			if(AppointmentTypeCur.Pattern!=null) { //logic copied from FormApptEdit
				_strBTime=new StringBuilder(Appointments.ConvertPatternFrom5(AppointmentTypeCur.Pattern));
			}
			FillTime();
			RefreshListBoxProcCodes();
		}

		private void butColor_Click(object sender,EventArgs e) {
			using ColorDialog colorDialog1=new ColorDialog();
			colorDialog1.Color=butColor.BackColor;
			colorDialog1.ShowDialog();
			butColor.BackColor=colorDialog1.Color;
		}

		private void butColorClear_Click(object sender,EventArgs e) {
			butColor.BackColor=System.Drawing.Color.FromArgb(0);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(AppointmentTypeCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			else {
				string msg=AppointmentTypes.CheckInUse(AppointmentTypeCur.AppointmentTypeNum);
				if(!string.IsNullOrWhiteSpace(msg)) {
					MsgBox.Show(this,msg);
					return;
				}
				AppointmentTypeCur=null;
				DialogResult=DialogResult.OK;
			}
		}

		private void butSlider_MouseUp(object sender,System.Windows.Forms.MouseEventArgs e) {
			mouseIsDown=false;
		}
		private void butSlider_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			mouseIsDown=true;
			mouseOrigin=new Point(e.X+butSlider.Location.X,e.Y+butSlider.Location.Y);
			sliderOrigin=butSlider.Location;
		}

		private void butSlider_MouseMove(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(!mouseIsDown) return;
			//tempPoint represents the new location of button of smooth dragging.
			Point tempPoint=new Point(sliderOrigin.X
				,sliderOrigin.Y+(e.Y+butSlider.Location.Y)-mouseOrigin.Y);
			int step=(int)(Math.Round((Decimal)(tempPoint.Y-tbTime.Location.Y)/14));
			if(step==_strBTime.Length) return;
			if(step<1) return;
			if(step>tbTime.MaxRows-1) return;
			if(step>_strBTime.Length) {
				_strBTime.Append('/');
			}
			if(step<_strBTime.Length) {
				_strBTime.Remove(step,1);
			}
			FillTime();
		}

		///<summary>Logic copied from FormApptEdit</summary>
		private void FillTime() {
			for(int i = 0;i<_strBTime.Length;i++) {
				tbTime.Cell[0,i]=_strBTime.ToString(i,1);
				tbTime.BackGColor[0,i]=Color.White;
			}
			for(int i = _strBTime.Length;i<tbTime.MaxRows;i++) {
				tbTime.Cell[0,i]="";
				tbTime.BackGColor[0,i]=Color.FromName("Control");
			}
			tbTime.Refresh();
			LayoutManager.MoveLocation(butSlider,new Point(tbTime.Location.X+2
				,(tbTime.Location.Y+_strBTime.Length*14+1)));
			if(_strBTime.Length>0) {
				textTime.Text=(_strBTime.Length*PrefC.GetInt(PrefName.AppointmentTimeIncrement)).ToString();
			}
			else {
				textTime.Text="Use procedure time pattern";
			}
		}

		private void tbTime_CellClicked(object sender,CellEventArgs e) {
			if(e.Row<_strBTime.Length) {
				if(_strBTime[e.Row]=='/') {
					_strBTime.Replace('/','X',e.Row,1);
				}
				else {
					_strBTime.Replace(_strBTime[e.Row],'/',e.Row,1);
				}
			}
			FillTime();
		}

		private void RefreshListBoxProcCodes() {
			listBoxProcCodes.Items.Clear();
			foreach(ProcedureCode procCode in _listProcCodes) {
				if(procCode.CodeNum==0) {//shouldn't happen but just in case
					continue;
				}
				listBoxProcCodes.Items.Add(procCode.ProcCode);
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormProcCodes FormPC=new FormProcCodes();
			FormPC.IsSelectionMode=true;
			FormPC.AllowMultipleSelections=true;
			FormPC.ShowDialog();
			if(FormPC.DialogResult==DialogResult.OK) {
				_listProcCodes.AddRange(FormPC.ListSelectedProcCodes.Select(x => x.Copy()).ToList());
			}
			RefreshListBoxProcCodes();
		}

		private void butClear_Click(object sender,EventArgs e) {
			_strBTime.Clear();
			FillTime();
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(listBoxProcCodes.SelectedIndices.Count<1) {
				MsgBox.Show(this,"Please select the procedures you wish to remove.");
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remove selected procedure(s)?")) {
				_listProcCodes.RemoveAll(x => listBoxProcCodes.GetStringSelectedItems().Contains(x.ProcCode));
				RefreshListBoxProcCodes();
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			AppointmentTypeCur.AppointmentTypeName=textName.Text;
			AppointmentTypeCur.AppointmentTypeColor=butColor.BackColor;
			AppointmentTypeCur.IsHidden=checkIsHidden.Checked;
			AppointmentTypeCur.CodeStr=String.Join(",",_listProcCodes.Select(x => x.ProcCode));
			if(_strBTime.Length>0) {
				AppointmentTypeCur.Pattern=Appointments.ConvertPatternTo5(_strBTime.ToString());
			}
			else{
				AppointmentTypeCur.Pattern="";
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}