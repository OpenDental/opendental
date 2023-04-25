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
		private List<Site> _listSites;

		public FormMapAreaContainerDetail() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMapAreaContainers_Load(object sender,EventArgs e) {
			textDescription.Text=MapAreaContainerCur.Description;
			textWidth.Value=MapAreaContainerCur.FloorWidthFeet;
			textHeight.Value=MapAreaContainerCur.FloorHeightFeet;
			_listSites=Sites.GetDeepCopy();
			comboSite.Items.AddNone<Site>();
			comboSite.Items.AddList(_listSites,x=>x.Description);
			comboSite.SetSelectedKey<Site>(MapAreaContainerCur.SiteNum,x=>x.SiteNum);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show("Please enter a description.");
				return;
			}
			if(!textWidth.IsValid()
				|| !textHeight.IsValid())
			{
				MsgBox.Show("Please enter a valid width and height.");
				return;
			}
			MapAreaContainerCur.Description=textDescription.Text;
			MapAreaContainerCur.FloorWidthFeet=(int)textWidth.Value;
			MapAreaContainerCur.FloorHeightFeet=(int)textHeight.Value;
			MapAreaContainerCur.SiteNum=comboSite.GetSelectedKey<Site>(x=>x.SiteNum);
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