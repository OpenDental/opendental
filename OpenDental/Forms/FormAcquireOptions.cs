using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormAcquireOptions:FormODBase {
		///<summary>Item must also be empty.</summary>
		public bool IsMountItemSelectedInitially;
		///<summary>Filtered list of the imaging devices available to this workstation.</summary>
		private List<ImagingDevice> _listImagingDevices;
		public ImagingDevice ImagingDeviceSelectedWhenClosing;
		public bool IsMountSelectedWhenClosing;

		public FormAcquireOptions() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAcquireOptions_Load(object sender,EventArgs e) {
			if(IsMountItemSelectedInitially){
				radioMount.Checked=true;
				labelMountUnavail.Visible=false;
			}
			else{
				radioSingle.Checked=true;
				radioMount.Enabled=false;
				labelMountUnavail.Visible=true;
			}
			FillGrid();
		}

		private void FillGrid(){
			List<ImagingDevice> listImagingDevicesAll=ImagingDevices.GetDeepCopy();
			string workstation=Environment.MachineName;
			_listImagingDevices=listImagingDevicesAll.FindAll(x=>x.ComputerName=="" || x.ComputerName==workstation);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableImagingDevices","Description"),100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listImagingDevices.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listImagingDevices[i].Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(_listImagingDevices.Count>0){
				gridMain.SetSelected(0);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a device first.");
				return;
			}
			IsMountSelectedWhenClosing=radioMount.Checked;
			ImagingDeviceSelectedWhenClosing=_listImagingDevices[gridMain.GetSelectedIndex()];
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}