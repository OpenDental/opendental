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
		public List<MapAreaContainer> ListMaps;
		private MapAreaContainer _mapCur;
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
			mapAreaPanel_MapAreaChanged(this,new EventArgs());
		}

		private void FillCombo() {
			ListMaps=PhoneMapJSON.GetFromDb();
			_mapCur=ListMaps[0];
			foreach(MapAreaContainer mapCur in ListMaps) {
				comboRoom.Items.Add(mapCur.Description);
			}
			comboRoom.SelectedIndex=0;
			_listSites=Sites.GetDeepCopy();
			comboSite.Items.Clear();
			comboSite.Items.AddList(_listSites,x=>x.Description);
		}

		private void FillSettings() {
			numFloorWidthFeet.Value=_mapCur.FloorWidthFeet;
			numFloorHeightFeet.Value=_mapCur.FloorHeightFeet;
			numPixelsPerFoot.Value=_mapCur.PixelsPerFoot;
			checkShowGrid.Checked=_mapCur.ShowGrid;
			checkShowOutline.Checked=_mapCur.ShowOutline;
			comboSite.SetSelectedKey<Site>(_mapCur.SiteNum,x=>x.SiteNum);
			FillMap();
		}

		private void FillEmployees() {
			List<Employee> employees=Employees.GetDeepCopy(true);
			employees.Sort(new Employees.EmployeeComparer(Employees.EmployeeComparer.SortBy.ext));
			gridEmployees.BeginUpdate();
			gridEmployees.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Ext. - Name",20){ IsWidthDynamic=true };
			col.TextAlign=HorizontalAlignment.Left;
			gridEmployees.ListGridColumns.Add(col);
			gridEmployees.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<employees.Count;i++) {
				row=new GridRow();
				row.Cells.Add(employees[i].PhoneExt+" - "+employees[i].FName+" "+employees[i].LName);
				row.Tag=employees[i];
				//row.ColorBackG=gridEmployees.Rows.Count%2==0?Color.LightGray:Color.White;
				gridEmployees.ListGridRows.Add(row);
			}
			gridEmployees.EndUpdate();
		}

		private void FillMap() {
			mapAreaPanel.Clear(false);
			//fill the panel
			List<MapArea> clinicMapItems=MapAreas.Refresh();
			clinicMapItems=clinicMapItems.OrderByDescending(x => (int)(x.ItemType)).ToList();
			for(int i=0;i<clinicMapItems.Count;i++) {
				if(clinicMapItems[i].MapAreaContainerNum!=_mapCur.MapAreaContainerNum) {
					continue;
				}
				if(clinicMapItems[i].ItemType==MapItemType.Room) {
					mapAreaPanel.AddCubicle(clinicMapItems[i],false);
				}
				else if(clinicMapItems[i].ItemType==MapItemType.DisplayLabel) {
					mapAreaPanel.AddDisplayLabel(clinicMapItems[i]);
				}
			}
			mapAreaPanel.ShowGrid=_mapCur.ShowGrid;
			mapAreaPanel.ShowOutline=_mapCur.ShowOutline;
			mapAreaPanel.FloorWidthFeet=_mapCur.FloorWidthFeet;
			mapAreaPanel.FloorHeightFeet=_mapCur.FloorHeightFeet;
			mapAreaPanel.PixelsPerFoot=LayoutManager.Scale(_mapCur.PixelsPerFoot);//_mapCur.PixelsPerFoot;
			textDescription.Text=_mapCur.Description;
			mapAreaPanel.Invalidate();
		}

		private void mapAreaPanel_MapAreaChanged(object sender,EventArgs e) {
			FillMap();
		}

		private void FormMapSetup_ResizeEnd(object sender,EventArgs e) {
			FillMap();
		}

		#endregion

		#region Check Boxes

		private void checkShowGrid_CheckedChanged(object sender,EventArgs e) {
			_mapCur.ShowGrid=checkShowGrid.Checked;
			FillMap();
		}

		private void checkShowOutline_CheckedChanged(object sender,EventArgs e) {
			_mapCur.ShowOutline=checkShowOutline.Checked;
			FillMap();
		}

		#endregion

		#region Numerics

		private void numericFloorWidthFeet_ValueChanged(object sender,EventArgs e) {
			_mapCur.FloorWidthFeet=(int)numFloorWidthFeet.Value;
			FillMap();
		}

		private void numericFloorHeightFeet_ValueChanged(object sender,EventArgs e) {
			_mapCur.FloorHeightFeet=(int)numFloorHeightFeet.Value;
			FillMap();
		}

		private void numericPixelsPerFoot_ValueChanged(object sender,EventArgs e) {
			_mapCur.PixelsPerFoot=(int)numPixelsPerFoot.Value;
			FillMap();
		}

		private void textDescription_TextChanged(object sender,EventArgs e) {
			_mapCur.Description=textDescription.Text;
			comboRoom.Items[comboRoom.SelectedIndex]=_mapCur.Description;
		}
		#endregion

		#region Menus

		private void newCubicleToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormMapAreaEdit FormEP=new FormMapAreaEdit();
			FormEP.MapItem=new MapArea();
			FormEP.MapItem.IsNew=true;
			FormEP.MapItem.Width=6;
			FormEP.MapItem.Height=6;
			FormEP.MapItem.ItemType=MapItemType.Room;
			PointF xy=GetXYFromMenuLocation(sender);
			FormEP.MapItem.XPos=Math.Round(xy.X,3);
			FormEP.MapItem.YPos=Math.Round(xy.Y,3);
			FormEP.MapItem.MapAreaContainerNum=_mapCur.MapAreaContainerNum;
			if(FormEP.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			FillMap();
		}

		private void newLabelToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormMapAreaEdit FormEP=new FormMapAreaEdit();
			FormEP.MapItem=new MapArea();
			FormEP.MapItem.IsNew=true;
			FormEP.MapItem.Width=6;
			FormEP.MapItem.Height=6;
			FormEP.MapItem.ItemType=MapItemType.DisplayLabel;
			PointF xy=GetXYFromMenuLocation(sender);
			FormEP.MapItem.XPos=Math.Round(xy.X,3);
			FormEP.MapItem.YPos=Math.Round(xy.Y,3);
			FormEP.MapItem.MapAreaContainerNum=_mapCur.MapAreaContainerNum;
			if(FormEP.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			FillMap();
		}

		private void comboRoom_SelectionChangeCommitted(object sender,EventArgs e) {
			_mapCur=ListMaps[comboRoom.SelectedIndex];
			FillSettings();
		}

		private void comboSite_SelectionChangeCommitted(object sender,EventArgs e) {
			_mapCur.SiteNum=comboSite.GetSelectedKey<Site>(x=>x.SiteNum);
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
			PointF xy=PointF.Empty;
			if(sender!=null
				&& sender is ToolStripMenuItem
				&& ((ToolStripMenuItem)sender).Tag!=null
				&& ((ToolStripMenuItem)sender).Tag is System.Drawing.Point) {
				xy=MapAreaPanel.ConvertScreenLocationToXY(((System.Drawing.Point)((ToolStripMenuItem)sender).Tag),mapAreaPanel.PixelsPerFoot);
			}
			return xy;
		}


		#endregion

		#region Buttons

		private void butBuildFromPhoneTable_Click(object sender,EventArgs e) {
			if(MessageBox.Show("This action will clear all information from clinicmapitem table and recreated it from current phone table rows. Would you like to continue?","",MessageBoxButtons.YesNo)!=System.Windows.Forms.DialogResult.Yes) {
				return;
			} 
			mapAreaPanel.Clear(true);
			List<Phone> phones=Phones.GetPhoneList();
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
				MapArea clinicMapItem=new MapArea();
				clinicMapItem.Description="";
				clinicMapItem.Extension=i; //phone.Extension;
				clinicMapItem.Width=defaultSizeFeet;
				clinicMapItem.Height=defaultSizeFeet;
				clinicMapItem.XPos=(1*column)+((column-1)*defaultSizeFeet);
				clinicMapItem.YPos=1+((row-1)*defaultSizeFeet);
				mapAreaPanel.AddCubicle(clinicMapItem);
				MapAreas.Insert(clinicMapItem);
			}
		}

		private void butAddRoom_Click(object sender,EventArgs e) {
			long mapAreaContainerNum=ListMaps.Max(x => x.MapAreaContainerNum)+1;
			MapAreaContainer newMap=new MapAreaContainer(mapAreaContainerNum,71,57,17,false,true,"New Room");
			ListMaps.Add(newMap);
			comboRoom.Items.Clear();
			foreach(MapAreaContainer mapCur in ListMaps) {
				comboRoom.Items.Add(mapCur.Description);
			}
			comboRoom.SelectedIndex=comboRoom.Items.Count-1;
			_mapCur=ListMaps[ListMaps.Count-1];
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
			using FormMapAreaEdit FormEP=new FormMapAreaEdit();
			FormEP.MapItem=new MapArea();
			FormEP.MapItem.IsNew=true;
			FormEP.MapItem.Width=6;
			FormEP.MapItem.Height=6;
			FormEP.MapItem.ItemType=MapItemType.Room;
			FormEP.MapItem.Extension=employee.PhoneExt;
			FormEP.MapItem.Description="";
			FormEP.MapItem.MapAreaContainerNum=_mapCur.MapAreaContainerNum;
			if(FormEP.ShowDialog(this)!=DialogResult.OK) {
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
			using FormMapAreaEdit FormEP=new FormMapAreaEdit();
			FormEP.MapItem=new MapArea {
				IsNew=true,
				Width=3,
				Height=3,
				ItemType=MapItemType.Room,
				Extension=employee.PhoneExt,
				Description="",
				MapAreaContainerNum=_mapCur.MapAreaContainerNum
			};
			if(FormEP.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			FillMap();
		}

		private void butSave_Click(object sender,EventArgs e) {
			PhoneMapJSON.SaveToDb(ListMaps);
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
					string mapName=_mapCur.Description;
					//Delete cubicles
					mapAreaPanel.Clear(true);
					//Delete room
					ListMaps.Remove(_mapCur);
					PhoneMapJSON.SaveToDb(ListMaps);
					//reset combobox
					comboRoom.Items.Clear();
					foreach(MapAreaContainer mapCur in ListMaps) {
						comboRoom.Items.Add(mapCur.Description);
					}
					comboRoom.SelectedIndex=0;
					if(!ListMaps.IsNullOrEmpty()) {
						_mapCur=ListMaps[0];
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
