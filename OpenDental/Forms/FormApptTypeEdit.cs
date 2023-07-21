using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.UI;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormApptTypeEdit:FormODBase {
		public AppointmentType AppointmentTypeCur;
		private bool _isMouseDown;
		private Point _pointMouseOrigin;
		private Point _pointSliderOrigin;
		///<summary>The string time pattern in the current increment. Not in the 5 minute increment.</summary>
		private StringBuilder _stringBuilderTime;
		private System.Drawing.Color _colorProv;
		private List<ProcedureCode> _listProcedureCodes;
		private List<ProcedureCode> _listProcedureCodesRequired;

		public FormApptTypeEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormApptTypeEdit_Load(object sender,EventArgs e) {
			textName.Text=AppointmentTypeCur.AppointmentTypeName;
			butColor.BackColor=AppointmentTypeCur.AppointmentTypeColor;
			checkIsHidden.Checked=AppointmentTypeCur.IsHidden;
			_listProcedureCodes=ProcedureCodes.GetFromCommaDelimitedList(AppointmentTypeCur.CodeStr);
			_listProcedureCodesRequired=ProcedureCodes.GetFromCommaDelimitedList(AppointmentTypeCur.CodeStrRequired);
			_colorProv=Color.Blue;
			_stringBuilderTime=new StringBuilder();
			if(AppointmentTypeCur.Pattern!=null) { //logic copied from FormApptEdit
				_stringBuilderTime=new StringBuilder(Appointments.ConvertPatternFrom5(AppointmentTypeCur.Pattern));
			}
			if(AppointmentTypeCur.BlockoutTypes is null) {
				AppointmentTypeCur.BlockoutTypes="";
			}
			//Fill the BlockoutTypes list.
			List<string> listBlockoutTypes=AppointmentTypeCur.BlockoutTypes.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
			List<long> listDefNums=listBlockoutTypes.Select(x => PIn.Long(x,hasExceptions:false)).ToList();
			List<Def> listDefsBlockoutTypes=Defs.GetDefsForCategory(DefCat.BlockoutTypes);
			List<Def> listDefsBlockoutTypesUnrestricted=listDefsBlockoutTypes.FindAll(x => !x.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription()));
			listBoxBlockoutTypes.Items.AddList(listDefsBlockoutTypesUnrestricted,x => x.ItemName);
			//Set the ones from AppointmentTypeCur as selected.
			for(int i=0;i<listBoxBlockoutTypes.Items.Count;i++) {
				Def def=(Def)listBoxBlockoutTypes.Items.GetObjectAt(i);
				if(listDefNums.Contains(def.DefNum)){
					listBoxBlockoutTypes.SelectedIndices.Add(i);
				}
			}
			if(AppointmentTypeCur.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.AtLeastOne) {
				radioButtonAtLeastOne.Checked=true;
			}
			else if(AppointmentTypeCur.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.All) {
				radioButtonAll.Checked=true;
			}
			FillTime();
			RefreshListBoxProcCodes();
			RefreshListBoxProcCodesRequired();
		}

		private void butColor_Click(object sender,EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColor.BackColor;
			colorDialog.ShowDialog();
			butColor.BackColor=colorDialog.Color;
		}

		private void butColorClear_Click(object sender,EventArgs e) {
			butColor.BackColor=System.Drawing.Color.FromArgb(0);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(AppointmentTypeCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			string msg=AppointmentTypes.CheckInUse(AppointmentTypeCur.AppointmentTypeNum);
			if(!string.IsNullOrWhiteSpace(msg)) {
				MsgBox.Show(this,msg);
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Appointment Type?")) {
				return;
			}
			AppointmentTypeCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butSlider_MouseUp(object sender,System.Windows.Forms.MouseEventArgs e) {
			_isMouseDown=false;
		}
		private void butSlider_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			_isMouseDown=true;
			_pointMouseOrigin=new Point(e.X+butSlider.Location.X,e.Y+butSlider.Location.Y);
			_pointSliderOrigin=butSlider.Location;
		}

		private void butSlider_MouseMove(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(!_isMouseDown) {
				return;
			}
			//point represents the new location of button of smooth dragging.
			Point point=new Point(_pointSliderOrigin.X,_pointSliderOrigin.Y+(e.Y+butSlider.Location.Y)-_pointMouseOrigin.Y);
			int step=(int)(Math.Round((Decimal)(point.Y-tbTime.Location.Y)/14));
			if(step==_stringBuilderTime.Length) {
				return;
			}
			if(step<1) {
				return;
			}
			if(step>tbTime.MaxRows-1) {
				return;
			}
			if(step>_stringBuilderTime.Length) {
				_stringBuilderTime.Append('/');
			}
			if(step<_stringBuilderTime.Length) {
				_stringBuilderTime.Remove(step,1);
			}
			FillTime();
		}

		///<summary>Logic copied from FormApptEdit</summary>
		private void FillTime() {
			for(int i = 0;i<_stringBuilderTime.Length;i++) {
				tbTime.Cell[0,i]=_stringBuilderTime.ToString(i,1);
				tbTime.BackGColor[0,i]=Color.White;
			}
			for(int i = _stringBuilderTime.Length;i<tbTime.MaxRows;i++) {
				tbTime.Cell[0,i]="";
				tbTime.BackGColor[0,i]=Color.FromName("Control");
			}
			tbTime.Refresh();
			LayoutManager.MoveLocation(butSlider,new Point(tbTime.Location.X+2
				,(tbTime.Location.Y+_stringBuilderTime.Length*14+1)));
			if(_stringBuilderTime.Length>0) {
				textTime.Text=(_stringBuilderTime.Length*PrefC.GetInt(PrefName.AppointmentTimeIncrement)).ToString();
			}
			else {
				textTime.Text="Use procedure time pattern";
			}
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

		private void RefreshListBoxProcCodes() {
			listBoxProcCodes.Items.Clear();
			for(int i=0;i<_listProcedureCodes.Count;i++) {
				if(_listProcedureCodes[i].CodeNum==0) {//shouldn't happen but just in case
					continue;
				}
				listBoxProcCodes.Items.Add(_listProcedureCodes[i].ProcCode,_listProcedureCodes[i]);
			}
		}

		private void RefreshListBoxProcCodesRequired() {
			listBoxProcCodesRequired.Items.Clear();
			for(int i=0;i<_listProcedureCodesRequired.Count;i++) {
				if(_listProcedureCodesRequired[i].CodeNum==0) {//shouldn't happen but just in case
					continue;
				}
				listBoxProcCodesRequired.Items.Add(_listProcedureCodesRequired[i].ProcCode,_listProcedureCodesRequired[i]);
			}
			if(_listProcedureCodesRequired.Count==0) {
				radioButtonAll.Checked=false;
				radioButtonAtLeastOne.Checked=false;
				radioButtonAll.Enabled=false;
				radioButtonAtLeastOne.Enabled=false;
			}
			else {
				radioButtonAll.Enabled=true;
				radioButtonAtLeastOne.Enabled=true;
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.CanAllowMultipleSelections=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult==DialogResult.OK) {
				_listProcedureCodes.AddRange(formProcCodes.ListProcedureCodesSelected.Select(x => x.Copy()).ToList());
			}
			RefreshListBoxProcCodes();
		}

		private void butClear_Click(object sender,EventArgs e) {
			_stringBuilderTime.Clear();
			FillTime();
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(listBoxProcCodes.SelectedIndices.Count<1) {
				MsgBox.Show(this,"Please select the procedures you wish to remove.");
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remove selected procedure(s)?")) {
				List<ProcedureCode> listProcCodes=listBoxProcCodes.GetListSelected<ProcedureCode>();
				for(int i=0;i<listProcCodes.Count;i++) {
					_listProcedureCodes.Remove(listProcCodes[i]);
				}
				RefreshListBoxProcCodes();
			}
		}

		private void butAddRequired_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.CanAllowMultipleSelections=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult!=DialogResult.OK) {
				return;
			}
			_listProcedureCodesRequired.AddRange(formProcCodes.ListProcedureCodesSelected.Select(x => x.Copy()).ToList());
			if(!radioButtonAtLeastOne.Checked) {
				radioButtonAll.Checked=true; //Default to All
			}
			RefreshListBoxProcCodesRequired();
		}

		private void butRemoveRequired_Click(object sender,EventArgs e) {
			if(listBoxProcCodesRequired.SelectedIndices.Count<1) {
				MsgBox.Show(this,"Please select the procedures you wish to remove.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remove selected procedure(s)?")) {
				return;
			}
			List<ProcedureCode> listProcedureCodes=listBoxProcCodesRequired.GetListSelected<ProcedureCode>();
			for(int i=0;i<listProcedureCodes.Count;i++) {
				_listProcedureCodesRequired.Remove(listProcedureCodes[i]);
			}
			RefreshListBoxProcCodesRequired();
		}

		private void butOK_Click(object sender,EventArgs e) {
			AppointmentTypeCur.AppointmentTypeName=textName.Text;
			if(AppointmentTypeCur.AppointmentTypeColor!=butColor.BackColor) {
				AppointmentTypeCur.AppointmentTypeColor=butColor.BackColor;
				if(AppointmentTypeCur.AppointmentTypeNum != 0 
					&& !AppointmentTypeCur.IsNew 
					&& MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to update all future appointments of this type to the new color?")) 
				{
					Appointments.UpdateFutureApptColorForApptType(AppointmentTypeCur);
				}
			}
			AppointmentTypeCur.IsHidden=checkIsHidden.Checked;
			AppointmentTypeCur.CodeStr=String.Join(",",_listProcedureCodes.Select(x => x.ProcCode));
			AppointmentTypeCur.CodeStrRequired=String.Join(",",_listProcedureCodesRequired.Select(x => x.ProcCode));
			AppointmentTypeCur.RequiredProcCodesNeeded=EnumRequiredProcCodesNeeded.None;
			if(radioButtonAtLeastOne.Checked) {
				AppointmentTypeCur.RequiredProcCodesNeeded=EnumRequiredProcCodesNeeded.AtLeastOne;
			}
			if(radioButtonAll.Checked) {
				AppointmentTypeCur.RequiredProcCodesNeeded=EnumRequiredProcCodesNeeded.All;
			}
			List<long> listBlockoutTypeDefNums=listBoxBlockoutTypes.GetListSelected<Def>().Select(x=>x.DefNum).ToList();
			string blockoutTypes=String.Join(",",listBlockoutTypeDefNums);
			AppointmentTypeCur.BlockoutTypes=blockoutTypes;
			if(_stringBuilderTime.Length>0) {
				AppointmentTypeCur.Pattern=Appointments.ConvertPatternTo5(_stringBuilderTime.ToString());
			}
			else{
				AppointmentTypeCur.Pattern="";
			}
			AppointmentTypeCur.IsNew=false;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}