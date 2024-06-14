using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEquipment:FormODBase {
		private List<Equipment> _listEquipments;
		private int _pagesPrinted;
		private bool _isHeadingPrinted;
		private int _heightHeading;

		public FormEquipment() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEquipment_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void radioPurchased_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void radioSold_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void radioAll_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void textSn_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			if(!textDateStart.IsValid() || !textDateEnd.IsValid()) {
				MsgBox.Show(this,"Invalid date.");
				return;
			}
			FillGrid();
		}

		private DateTime GetDateStart() {
			if(textDateStart.Text=="") {
				return DateTime.MinValue.AddDays(1);//because we don't want to include 010101
			}
			return PIn.Date(textDateStart.Text);
		}

		private DateTime GetDateEnd() {
			if(textDateEnd.Text=="") {
				return DateTime.MaxValue;
			}
			return PIn.Date(textDateEnd.Text);
		}

		private void FillGrid(){
			if(!textDateStart.IsValid() || !textDateEnd.IsValid()) {
				return;
			}
			DateTime dateFrom=GetDateStart();
			DateTime dateTo=GetDateEnd();
			EnumEquipmentDisplayMode equipmentDisplayMode=EnumEquipmentDisplayMode.All;
			if(radioPurchased.Checked){
				equipmentDisplayMode=EnumEquipmentDisplayMode.Purchased;
			}
			if(radioSold.Checked){
				equipmentDisplayMode=EnumEquipmentDisplayMode.Sold;
			}
			_listEquipments=Equipments.GetList(dateFrom,dateTo,equipmentDisplayMode,textSnDesc.Text);
			gridMain.BeginUpdate();
			if(radioPurchased.Checked) {
				gridMain.HScrollVisible=true;
			}
			else {
			}
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Description"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"SerialNumber"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Yr"),40);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"DatePurchased"),90);
			gridMain.Columns.Add(col);
			if(equipmentDisplayMode!=EnumEquipmentDisplayMode.Purchased) {//Purchased mode is designed for submission to tax authority, only certain columns
				col=new GridColumn(Lan.g(this,"DateSold"),90);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Cost"),80,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Est Value"),80,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			if(equipmentDisplayMode!=EnumEquipmentDisplayMode.Purchased) {
				col=new GridColumn(Lan.g(this,"Location"),80);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Status"),160);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEquipments.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listEquipments[i].Description);
				row.Cells.Add(_listEquipments[i].SerialNumber);
				row.Cells.Add(_listEquipments[i].ModelYear);
				row.Cells.Add(_listEquipments[i].DatePurchased.ToShortDateString());
				if(equipmentDisplayMode!=EnumEquipmentDisplayMode.Purchased) {
					if(_listEquipments[i].DateSold.Year<1880) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(_listEquipments[i].DateSold.ToShortDateString());
					}
				}
				row.Cells.Add(_listEquipments[i].PurchaseCost.ToString("f"));
				row.Cells.Add(_listEquipments[i].MarketValue.ToString("f"));
				if(equipmentDisplayMode!=EnumEquipmentDisplayMode.Purchased) {
					row.Cells.Add(_listEquipments[i].Location);
				}
				row.Cells.Add(_listEquipments[i].Status.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			Equipment equipment=new Equipment();
			equipment.SerialNumber=Equipments.GenerateSerialNum();
			equipment.DateEntry=DateTime.Today;
			equipment.DatePurchased=DateTime.Today;
			using FormEquipmentEdit formEquipmentEdit=new FormEquipmentEdit();
			formEquipmentEdit.IsNew=true;
			formEquipmentEdit.EquipmentCur=equipment;
			formEquipmentEdit.ShowDialog();
			if(formEquipmentEdit.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEquipmentEdit formEquipmentEdit=new FormEquipmentEdit();
			formEquipmentEdit.EquipmentCur=_listEquipments[e.Row];
			formEquipmentEdit.ShowDialog();
			if(formEquipmentEdit.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Equipment list printed"),CodeBase.PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangle=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			using Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			using Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangle.Top;
			int center=rectangle.X+rectangle.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Equipment List");
				if(radioPurchased.Checked) {
					text+=" - "+Lan.g(this,"Purchased");
				}
				if(radioSold.Checked) {
					text+=" - "+Lan.g(this,"Sold");
				}
				if(radioAll.Checked) {
					text+=" - "+Lan.g(this,"All");
				}
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				DateTime dateStart=GetDateStart();
				DateTime dateEnd=GetDateEnd();
				text=dateStart.ToShortDateString()+" "+Lan.g(this,"to")+" "+dateEnd.ToShortDateString();
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=20;
				_isHeadingPrinted=true;
				_heightHeading=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,rectangle,_heightHeading);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
				double total=0;
				for(int i=0;i<_listEquipments.Count;i++){
					total+=_listEquipments[i].MarketValue;
				}
				g.DrawString(Lan.g(this,"Total Est Value:")+" "+total.ToString("c"),Font,Brushes.Black,550,yPos);
			}
			g.Dispose();
		}

	}
}