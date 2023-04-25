using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormHL7Msgs:FormODBase {
		private long _patNumSelected;
		private List<HL7Msg> _listHL7Msgs;
		///<summary>This gets set externally beforehand.  Lets user quickly select the HL7 messages for the current patient.</summary>
		public long PatNumCur;

		public FormHL7Msgs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormHL7Msgs_Load(object sender,EventArgs e) {
			textDateStart.Text=DateTime.Today.ToShortDateString();//Set both start and stop date to today to limit the number of messages that load immediately
			textDateEnd.Text=DateTime.Today.ToShortDateString();
			comboHL7Status.Items.Add(Lan.g(this,"All"));
			comboHL7Status.SelectedIndex=0;
			for(int i=0;i<Enum.GetNames(typeof(HL7MessageStatus)).Length;i++) {
				comboHL7Status.Items.Add(Enum.GetName(typeof(HL7MessageStatus),i));
			}
			_patNumSelected=0;
			FillGrid();
		}

		private void FillGrid() {
			if(!textDateStart.IsValid() || !textDateEnd.IsValid()) {
				return;
			}
			if(_patNumSelected>0) {
				Patient patient=Patients.GetLim(_patNumSelected);
				textPatient.Text=patient.GetNameLF();
			}
			else {
				textPatient.Text="";
			}
			_listHL7Msgs=HL7Msgs.GetHL7Msgs(PIn.Date(textDateStart.Text),PIn.Date(textDateEnd.Text),_patNumSelected,comboHL7Status.SelectedIndex);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"DateTime"),180);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient"),170);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"AptNum"),60);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Status"),75);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Note"),400);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			if(_listHL7Msgs==null) {
				gridMain.EndUpdate();
				return;
			}
			for(int i=0;i<_listHL7Msgs.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(_listHL7Msgs[i].DateTStamp.ToString());
				if(_listHL7Msgs[i].PatNum>0) {
					row.Cells.Add(Patients.GetLim(_listHL7Msgs[i].PatNum).GetNameFL());
				}
				else {
					row.Cells.Add("");
				}
				if(_listHL7Msgs[i].AptNum>0) {
					row.Cells.Add(_listHL7Msgs[i].AptNum.ToString());
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(Enum.GetName(typeof(HL7MessageStatus),_listHL7Msgs[i].HL7Status));
				row.Cells.Add(_listHL7Msgs[i].Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			using FormHL7MsgEdit formHL7MsgEdit=new FormHL7MsgEdit();
			formHL7MsgEdit.HL7MsgCur=HL7Msgs.GetOne(_listHL7Msgs[e.Row].HL7MsgNum);
			formHL7MsgEdit.ShowDialog();
			FillGrid();
		}

		private void comboHL7Status_SelectedIndexChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butCurrent_Click(object sender,EventArgs e) {
			_patNumSelected=PatNumCur;
			FillGrid();
		}

		private void butFind_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			_patNumSelected=formPatientSelect.PatNumSelected;
			FillGrid();
		}

		private void butAll_Click(object sender,EventArgs e) {
			_patNumSelected=0;
			FillGrid();
		}
		
		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}