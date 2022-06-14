using System;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Drawing;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormMapSetup:FormODBase {

		#region Init
		public List<MapAreaContainer> ListMapAreaContainers;
		private MapAreaContainer _mapAreaContainer;
		private List<Site> _listSites;

		public FormMapSetup() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormMapSetup_Load(object sender,EventArgs e) {
			//Get latest prefs. We will use them to setup our clinic panel.
			Cache.Refresh(InvalidType.Prefs);
			FillCombo();
			//fill the employee list			
			FillEmployees();
			FillSettings();
			//map panel
			mapAreaPanel_MapCubicleEdited(this,new EventArgs());
		}

		private void FillCombo() {
			ListMapAreaContainers=PhoneMapJSON.GetFromDb();
			_mapAreaContainer=ListMapAreaContainers[0];
			foreach(MapAreaContainer mapCur in ListMapAreaContainers) {
				comboRoom.Items.Add(mapCur.Description);
			}
			comboRoom.SelectedIndex=0;
			_listSites=Sites.GetDeepCopy();
			comboSite.Items.Clear();
			comboSite.Items.AddList(_listSites,x=>x.Description);
		}

		private void FillSettings() {
			numFloorWidthFeet.Value=_mapAreaContainer.FloorWidthFeet;
			numFloorHeightFeet.Value=_mapAreaContainer.FloorHeightFeet;
			numPixelsPerFoot.Value=_mapAreaContainer.PixelsPerFoot;
			checkShowGrid.Checked=_mapAreaContainer.ShowGrid;
			checkShowOutline.Checked=_mapAreaContainer.ShowOutline;
			comboSite.SetSelectedKey<Site>(_mapAreaContainer.SiteNum,x=>x.SiteNum);
			FillMap();
		}

		private void FillEmployees() {
			List<Employee> listEmployees=Employees.GetDeepCopy(true);
			listEmployees.Sort(new Employees.EmployeeComparer(Employees.EmployeeComparer.SortBy.ext));
			gridEmployees.BeginUpdate();
			gridEmployees.Columns.Clear();
			GridColumn col=new GridColumn("Ext. - Name",20){ IsWidthDynamic=true };
			col.TextAlign=HorizontalAlignment.Left;
			gridEmployees.Columns.Add(col);
			gridEmployees.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listEmployees.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listEmployees[i].PhoneExt+" - "+listEmployees[i].FName+" "+listEmployees[i].LName);
				row.Tag=listEmployees[i];
				//row.ColorBackG=gridEmployees.Rows.Count%2==0?Color.LightGray:Color.White;
				gridEmployees.ListGridRows.Add(row);
			}
			gridEmployees.EndUpdate();
		}

		private void FillMap() {
			mapAreaPanel.Clear(false);
			//fill the panel
			List<MapArea> listMapAreasClinicItems=MapAreas.Refresh();
			listMapAreasClinicItems=listMapAreasClinicItems.OrderByDescending(x => (int)(x.ItemType)).ToList();
			for(int i=0;i<listMapAreasClinicItems.Count;i++) {
				if(listMapAreasClinicItems[i].MapAreaContainerNum!=_mapAreaContainer.MapAreaContainerNum) {
					continue;
				}
				if(listMapAreasClinicItems[i].ItemType==MapItemType.Cubicle) {
					mapAreaPanel.AddCubicle(listMapAreasClinicItems[i],false);
				}
				else if(listMapAreasClinicItems[i].ItemType==MapItemType.Label) {
					mapAreaPanel.AddDisplayLabel(listMapAreasClinicItems[i]);
				}
			}
			mapAreaPanel.ShowGrid=_mapAreaContainer.ShowGrid;
			mapAreaPanel.ShowOutline=_mapAreaContainer.ShowOutline;
			mapAreaPanel.WidthFloorFeet=_mapAreaContainer.FloorWidthFeet;
			mapAreaPanel.HeightFloorFeet=_mapAreaContainer.FloorHeightFeet;
			mapAreaPanel.PixelsPerFoot=LayoutManager.Scale(_mapAreaContainer.PixelsPerFoot);//_mapCur.PixelsPerFoot;
			textDescription.Text=_mapAreaContainer.Description;
			mapAreaPanel.Invalidate();
		}

		private void mapAreaPanel_MapCubicleEdited(object sender,EventArgs e) {
			FillMap();
		}

		private void FormMapSetup_ResizeEnd(object sender,EventArgs e) {
			FillMap();
		}

		#endregion

		#region Check Boxes

		private void checkShowGrid_CheckedChanged(object sender,EventArgs e) {
			_mapAreaContainer.ShowGrid=checkShowGrid.Checked;
			FillMap();
		}

		private void checkShowOutline_CheckedChanged(object sender,EventArgs e) {
			_mapAreaContainer.ShowOutline=checkShowOutline.Checked;
			FillMap();
		}

		#endregion

		#region Numerics

		private void numericFloorWidthFeet_ValueChanged(object sender,EventArgs e) {
			_mapAreaContainer.FloorWidthFeet=(int)numFloorWidthFeet.Value;
			FillMap();
		}

		private void numericFloorHeightFeet_ValueChanged(object sender,EventArgs e) {
			_mapAreaContainer.FloorHeightFeet=(int)numFloorHeightFeet.Value;
			FillMap();
		}

		private void numericPixelsPerFoot_ValueChanged(object sender,EventArgs e) {
			_mapAreaContainer.PixelsPerFoot=(int)numPixelsPerFoot.Value;
			FillMap();
		}

		private void textDescription_TextChanged(object sender,EventArgs e) {
			_mapAreaContainer.Description=textDescription.Text;
			comboRoom.Items[comboRoom.SelectedIndex]=_mapAreaContainer.Description;
		}
		#endregion

		#region Menus

		private void newCubicleToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormMapAreaEdit formMapAreaEdit=new FormMapAreaEdit();
			formMapAreaEdit.MapAreaItem=new MapArea();
			formMapAreaEdit.MapAreaItem.IsNew=true;
			formMapAreaEdit.MapAreaItem.Width=6;
			formMapAreaEdit.MapAreaItem.Height=6;
			formMapAreaEdit.MapAreaItem.ItemType=MapItemType.Cubicle;
			PointF pointFXY=GetXYFromMenuLocation(sender);
			formMapAreaEdit.MapAreaItem.XPos=Math.Round(pointFXY.X,3);
			formMapAreaEdit.MapAreaItem.YPos=Math.Round(pointFXY.Y,3);
			formMapAreaEdit.MapAreaItem.MapAreaContainerNum=_mapAreaContainer.MapAreaContainerNum;
			if(formMapAreaEdit.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			FillMap();
		}

		private void newLabelToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormMapAreaEdit formMapAreaEdit=new FormMapAreaEdit();
			formMapAreaEdit.MapAreaItem=new MapArea();
			formMapAreaEdit.MapAreaItem.IsNew=true;
			formMapAreaEdit.MapAreaItem.Width=6;
			formMapAreaEdit.MapAreaItem.Height=6;
			formMapAreaEdit.MapAreaItem.ItemType=MapItemType.Label;
			PointF pointFXY=GetXYFromMenuLocation(sender);
			formMapAreaEdit.MapAreaItem.XPos=Math.Round(pointFXY.X,3);
			formMapAreaEdit.MapAreaItem.YPos=Math.Round(pointFXY.Y,3);
			formMapAreaEdit.MapAreaItem.MapAreaContainerNum=_mapAreaContainer.MapAreaContainerNum;
			if(formMapAreaEdit.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			FillMap();
		}

		private void comboRoom_SelectionChangeCommitted(object sender,EventArgs e) {
			_mapAreaContainer=ListMapAreaContainers[comboRoom.SelectedIndex];
			FillSettings();
		}

		private void comboSite_SelectionChangeCommitted(object sender,EventArgs e) {
			_mapAreaContainer.SiteNum=comboSite.GetSelectedKey<Site>(x=>x.SiteNum);
		}

		#endregion

				
		#region Map Panel

		private void mapAreaPanel_MouseUp(object sender,MouseEventArgs e) {
			if(e.Button!=MouseButtons.Right) {
				return;
			}
			newCubicleToolStripMenuItem.Tag=e.Location;
			newLabelToolStripMenuItem.Tag=e.Location;
			menu.Show(mapAreaPanel,e.Location);
		}

		private PointF GetXYFromMenuLocation(object sender) {
			PointF pointFXY=PointF.Empty;
			if(sender!=null
				&& sender is ToolStripMenuItem
				&& ((ToolStripMenuItem)sender).Tag!=null
				&& ((ToolStripMenuItem)sender).Tag is System.Drawing.Point) {
				pointFXY=MapAreaPanel.ConvertScreenLocationToXY(((System.Drawing.Point)((ToolStripMenuItem)sender).Tag),mapAreaPanel.PixelsPerFoot);
			}
			return pointFXY;
		}


		#endregion

		#region Buttons

		private void butBuildFromPhoneTable_Click(object sender,EventArgs e) {
			if(MessageBox.Show("This action will clear all information from clinicmapitem table and recreated it from current phone table rows. Would you like to continue?","",MessageBoxButtons.YesNo)!=System.Windows.Forms.DialogResult.Yes) {
				return;
			} 
			mapAreaPanel.Clear(true);
			List<Phone> listPhones=Phones.GetPhoneList();
			int defaultSizeFeet=6;
			int row=1;
			int column=0;
			for(int i = 0;i<78;i++) {
				if(row>=7) {
					if(++column>8) {
						column=3;
						row++;
					}
				}
				else {
					if(++column>10) {
						column=1;
						row++;
					}
					if(row==7) {
						column=3;
						//row=8;
					}
				}

				//Phone phone=phones[i];
				MapArea mapAreaClinicItem=new MapArea();
				mapAreaClinicItem.Description="";
				mapAreaClinicItem.Extension=i; //phone.Extension;
				mapAreaClinicItem.Width=defaultSizeFeet;
				mapAreaClinicItem.Height=defaultSizeFeet;
				mapAreaClinicItem.XPos=(1*column)+((column-1)*defaultSizeFeet);
				mapAreaClinicItem.YPos=1+((row-1)*defaultSizeFeet);
				mapAreaPanel.AddCubicle(mapAreaClinicItem);
				MapAreas.Insert(mapAreaClinicItem);
			}
		}

		private void butAddRoom_Click(object sender,EventArgs e) {
			long mapAreaContainerNum=ListMapAreaContainers.Max(x => x.MapAreaContainerNum)+1;
			MapAreaContainer mapAreaContainer=new MapAreaContainer(mapAreaContainerNum,71,57,17,false,true,"New Room");
			ListMapAreaContainers.Add(mapAreaContainer);
			comboRoom.Items.Clear();
			foreach(MapAreaContainer mapCur in ListMapAreaContainers) {
				comboRoom.Items.Add(mapCur.Description);
			}
			comboRoom.SelectedIndex=comboRoom.Items.Count-1;
			_mapAreaContainer=ListMapAreaContainers[ListMapAreaContainers.Count-1];
			FillSettings();
			FillMap();
		}

		private void butAddMapArea_Click(object sender,EventArgs e) {
			//edit this entry
			if(gridEmployees.SelectedIndices==null
				|| gridEmployees.SelectedIndices.Length<=0
				|| gridEmployees.ListGridRows[gridEmployees.SelectedIndices[0]].Tag==null
				|| !(gridEmployees.ListGridRows[gridEmployees.SelectedIndices[0]].Tag is Employee)) {
				MsgBox.Show(this,"Select an employee");
				return;
			}
			Employee employee=(Employee)gridEmployees.ListGridRows[gridEmployees.SelectedIndices[0]].Tag;
			using FormMapAreaEdit formMapAreaEdit=new FormMapAreaEdit();
			formMapAreaEdit.MapAreaItem=new MapArea();
			formMapAreaEdit.MapAreaItem.IsNew=true;
			formMapAreaEdit.MapAreaItem.Width=6;
			formMapAreaEdit.MapAreaItem.Height=6;
			formMapAreaEdit.MapAreaItem.ItemType=MapItemType.Cubicle;
			formMapAreaEdit.MapAreaItem.Extension=employee.PhoneExt;
			formMapAreaEdit.MapAreaItem.Description="";
			formMapAreaEdit.MapAreaItem.MapAreaContainerNum=_mapAreaContainer.MapAreaContainerNum;
			if(formMapAreaEdit.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			FillMap();
		}

		private void ButAddSmall_Click(object sender,EventArgs e) {
			//edit this entry
			if(gridEmployees.SelectedIndices==null
				|| gridEmployees.SelectedIndices.Length<=0
				|| gridEmployees.ListGridRows[gridEmployees.SelectedIndices[0]].Tag==null
				|| !(gridEmployees.ListGridRows[gridEmployees.SelectedIndices[0]].Tag is Employee)) {
				MsgBox.Show(this,"Select an employee");
				return;
			}
			Employee employee=(Employee)gridEmployees.ListGridRows[gridEmployees.SelectedIndices[0]].Tag;
			using FormMapAreaEdit formMapAreaEdit=new FormMapAreaEdit();
			formMapAreaEdit.MapAreaItem=new MapArea {
				IsNew=true,
				Width=3,
				Height=3,
				ItemType=MapItemType.Cubicle,
				Extension=employee.PhoneExt,
				Description="",
				MapAreaContainerNum=_mapAreaContainer.MapAreaContainerNum
			};
			if(formMapAreaEdit.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			FillMap();
		}

		private void butSave_Click(object sender,EventArgs e) {
			PhoneMapJSON.SaveToDb(ListMapAreaContainers);
			DataValid.SetInvalid(InvalidType.PhoneMap);
			DialogResult=DialogResult.OK;
			Close();
		}

		private void ButDelete_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			if(MsgBox.Show(MsgBoxButtons.YesNo,"This will IMMEDIATELY delete the displayed room from the database.  Continue?")) {//Not translating because HQ only.
				if(MsgBox.Show(MsgBoxButtons.YesNo,"Are you sure?")) {
					string mapName=_mapAreaContainer.Description;
					//Delete cubicles
					mapAreaPanel.Clear(true);
					//Delete room
					ListMapAreaContainers.Remove(_mapAreaContainer);
					PhoneMapJSON.SaveToDb(ListMapAreaContainers);
					//reset combobox
					comboRoom.Items.Clear();
					foreach(MapAreaContainer mapCur in ListMapAreaContainers) {
						comboRoom.Items.Add(mapCur.Description);
					}
					comboRoom.SelectedIndex=0;
					if(!ListMapAreaContainers.IsNullOrEmpty()) {
						_mapAreaContainer=ListMapAreaContainers[0];
					}
					FillSettings();
					SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,mapName+" deleted from call center map by user.");
				}
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			this.Close();
		}

		#endregion

		
	}

}
