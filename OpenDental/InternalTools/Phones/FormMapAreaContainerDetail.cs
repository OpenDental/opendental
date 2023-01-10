using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormMapAreaContainerDetail:FormODBase {
		public MapAreaContainer MapAreaContainerCur;

		public FormMapAreaContainerDetail() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMapAreaContainers_Load(object sender,EventArgs e) {
			textDescription.Text=MapAreaContainerCur.Description;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show("Please enter a description.");
				return;
			}
			MapAreaContainerCur.Description=textDescription.Text;
			MapAreaContainers.Update(MapAreaContainerCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			/*
			if(!MapAreaContainerCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			//new
			if(mapPanel.ListMapAreas.Count==0){
				MapAreaContainers.Delete(MapAreaContainerCur.MapAreaContainerNum);
				DialogResult=DialogResult.Cancel;
				return;
			}
			//new, but they already added cubicles
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,"This will delete the entire container and all cubicles within it.  Are you sure you want to delete?"))
			{
				return;
			}
			MapAreas.DeleteAll(MapAreaContainerCur.MapAreaContainerNum);
			MapAreaContainers.Delete(MapAreaContainerCur.MapAreaContainerNum);
			DialogResult=DialogResult.OK;*/
		}
	}
}