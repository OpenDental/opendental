using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormDefEditWebSchedApptTypes:FormODBase {
		private Def _def;
		///<summary>Every Web Sched reason is required to be associated to one (and only one) appointment type.
		///This is where the length and procedures of the appointment are retrieved.</summary>
		private AppointmentType _appointmentType;
		///<summary>The blockout types that this WSNPA reason is restricted to.  Can be empty.</summary>
		private List<Def> _listDefsRestrictToBlockoutTypes;
		///<summary>Set within class only.</summary>
		public bool IsDeleted;

		public FormDefEditWebSchedApptTypes(Def defCur,string title) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			this.Text=title;
			_def=defCur;
			checkHidden.Checked=_def.IsHidden;
			textName.Text=_def.ItemName;
			//Look for an associated appointment type.
			List<DefLink> listDefLinksAppointmentType=DefLinks.GetDefLinksByType(DefLinkType.AppointmentType);
			DefLink defLink=listDefLinksAppointmentType.FirstOrDefault(x => x.DefNum==_def.DefNum);
			if(defLink!=null) {
				_appointmentType=AppointmentTypes.GetFirstOrDefault(x => x.AppointmentTypeNum==defLink.FKey);
			}
			List<DefLink> listDefLinksBlockoutType=DefLinks.GetDefLinksByType(DefLinkType.BlockoutType,_def.DefNum);
			List<long> listDefLinkFKeys=listDefLinksBlockoutType.Select(x => x.FKey).ToList();
			_listDefsRestrictToBlockoutTypes=Defs.GetDefs(DefCat.BlockoutTypes,listDefLinkFKeys);
			FillApptTypeValue();
			FillBlockoutTypeValues();
		}

		private void FillApptTypeValue() {
			textApptType.Clear();
			if(_appointmentType!=null) {
				textApptType.Text=_appointmentType.AppointmentTypeName;
			}
		}

		private void FillBlockoutTypeValues() {
			textRestrictToBlockouts.Clear();
			textRestrictToBlockouts.Text=string.Join(",",_listDefsRestrictToBlockoutTypes.Select(x => x.ItemName));
		}

		private void butSelect_Click(object sender,EventArgs e) {
			using FormApptTypes formApptTypes=new FormApptTypes();
			formApptTypes.IsSelectionMode=true;
			formApptTypes.AppointmentTypeSelected=_appointmentType;
			if(formApptTypes.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_appointmentType=formApptTypes.AppointmentTypeSelected;
			FillApptTypeValue();
		}

		private void butColor_Click(object sender,EventArgs e) {
			colorDialog1.Color=butColor.BackColor;
			colorDialog1.ShowDialog();
			butColor.BackColor=colorDialog1.Color;
		}

		private void butSelectBlockouts_Click(object sender,EventArgs e) {
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.BlockoutTypes,_listDefsRestrictToBlockoutTypes);
			formDefinitionPicker.IsMultiSelectionMode=true;
			formDefinitionPicker.ShowDialog();
			if(formDefinitionPicker.DialogResult!=DialogResult.OK) {
				return;
			}
			_listDefsRestrictToBlockoutTypes=GenericTools.DeepCopy<List<Def>,List<Def>>(formDefinitionPicker.ListDefsSelected);
			FillBlockoutTypeValues();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(string.IsNullOrEmpty(textName.Text.Trim())) {
				MsgBox.Show(this,"Reason required.");
				return;
			}
			if(_appointmentType==null) {
				MsgBox.Show(this,"Appointment Type required.");
				return;
			}
			_def.ItemName=PIn.String(textName.Text);
			if(_def.IsNew) {
				DefL.Insert(_def);
			}
			else {
				DefL.Update(_def);
			}
			DefLinks.SetFKeyForDef(_def.DefNum,_appointmentType.AppointmentTypeNum,DefLinkType.AppointmentType);
			DefLinks.DeleteAllForDef(_def.DefNum,DefLinkType.BlockoutType);//Remove all blockouts before inserting the new set
			List<long> listDefNums=_listDefsRestrictToBlockoutTypes.Select(x => x.DefNum).ToList();
			DefLinks.InsertDefLinksForFKeys(_def.DefNum,listDefNums,DefLinkType.BlockoutType);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			string msgText="Are you sure you want to delete this definition? References to it will be deleted as well.";
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,msgText)){
				return;
			}
			try {
				Defs.Delete(_def);				
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
			}
			//Web Sched New appointment type defs can be associated to multiple types of deflinks.  Clean them up.
			DefLinks.DeleteAllForDef(_def.DefNum);
			IsDeleted=true;
			DialogResult=DialogResult.OK;
		}
	}
}