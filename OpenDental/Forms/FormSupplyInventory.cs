using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormSupplyInventory:FormODBase {
		private List<SupplyNeeded> _listSupplyNeededs;
		private int _pagesPrinted;
		private bool _isHeadingPrinted;
		private int _heightHeadingPrint;

		public FormSupplyInventory() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInventory_Load(object sender,EventArgs e) {
			LayoutMenu();
			FillGridNeeded();
		}
		private void LayoutMenu() {
			menuMain.BeginUpdate();
			//Suppliers-----------------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItemOD("Suppliers",menuItemSuppliers_Click));
			//Categories-----------------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItemOD("Categories",menuItemCategories_Click));
			menuMain.EndUpdate();
		}

		private void FillGridNeeded(){
			_listSupplyNeededs=SupplyNeededs.CreateObjects();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date Added"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),300);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listSupplyNeededs.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listSupplyNeededs[i].DateAdded.ToShortDateString());
				row.Cells.Add(_listSupplyNeededs[i].Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridNeeded_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormSupplyNeededEdit formSupplyNeededEdit=new FormSupplyNeededEdit();
			formSupplyNeededEdit.SupplyNeededCur=_listSupplyNeededs[e.Row];
			formSupplyNeededEdit.ShowDialog();
			if(formSupplyNeededEdit.DialogResult==DialogResult.OK) {
				FillGridNeeded();
			}
		}

		private void butAddNeeded_Click(object sender,EventArgs e) {
			SupplyNeeded supplyNeeded=new SupplyNeeded();
			supplyNeeded.IsNew=true;
			supplyNeeded.DateAdded=DateTime.Today;
			using FormSupplyNeededEdit formSupplyNeededEdit=new FormSupplyNeededEdit();
			formSupplyNeededEdit.SupplyNeededCur=supplyNeeded;
			formSupplyNeededEdit.ShowDialog();
			if(formSupplyNeededEdit.DialogResult==DialogResult.OK){
				FillGridNeeded();
			}
		}

		private void menuItemSuppliers_Click(object sender,EventArgs e) {
			using FormSuppliers formSuppliers=new FormSuppliers();
			formSuppliers.ShowDialog();
		}

		private void menuItemCategories_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.DefEdit)) {
				return;
			}
			using FormDefinitions formDefinitions=new FormDefinitions(DefCat.SupplyCats);
			formDefinitions.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.DefEdit,0,"Definitions.");
		}

		private void butEquipment_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EquipmentSetup)) {
				return;
			}
			using FormEquipment formEquipment=new FormEquipment();
			formEquipment.ShowDialog();
		}

		private void butOrders_Click(object sender,EventArgs e) {
			using FormSupplyOrders formSupplyOrders=new FormSupplyOrders();
			formSupplyOrders.ShowDialog();
		}

		private void butSupplies_Click(object sender,EventArgs e) {
			using FormSupplies formSupplies=new FormSupplies();
			formSupplies.ShowDialog();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Supplies needed list printed"),PrintoutOrientation.Portrait);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			using Graphics g=e.Graphics;
			string text;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int centerPos=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Supplies Needed");
				g.DrawString(text,fontHeading,Brushes.Black,centerPos-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				//text=Lan.g(this,"Supplies Needed");
				//g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				//yPos+=(int)g.MeasureString(text,headingFont).Height;
				yPos+=20;
				_isHeadingPrinted=true;
				_heightHeadingPrint=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,bounds,_heightHeadingPrint);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		

		
	
	}
}