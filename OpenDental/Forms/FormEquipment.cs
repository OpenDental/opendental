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
		private List<Equipment> listEquip;
		private int pagesPrinted;
		private bool headingPrinted;
		private int headingPrintH;

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

		private void FillGrid(){
			if(!textDateStart.IsValid() || !textDateEnd.IsValid()) {
				return;
			}
			DateTime fromDate;
			DateTime toDate;
			if(textDateStart.Text=="") {
				fromDate=DateTime.MinValue.AddDays(1);//because we don't want to include 010101
			}
			else {
				fromDate=PIn.Date(textDateStart.Text);
			}
			if(textDateEnd.Text=="") {
				toDate=DateTime.MaxValue;
			}
			else {
				toDate=PIn.Date(textDateEnd.Text);
			}
			EnumEquipmentDisplayMode display=EnumEquipmentDisplayMode.All;
			if(radioPurchased.Checked){
				display=EnumEquipmentDisplayMode.Purchased;
			}
			if(radioSold.Checked){
				display=EnumEquipmentDisplayMode.Sold;
			}
			listEquip=Equipments.GetList(fromDate,toDate,display,textSnDesc.Text);
			gridMain.BeginUpdate();
			if(radioPurchased.Checked) {
				gridMain.HScrollVisible=true;
			}
			else {
			}
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Description"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"SerialNumber"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Yr"),40);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"DatePurchased"),90);
			gridMain.ListGridColumns.Add(col);
			if(display!=EnumEquipmentDisplayMode.Purchased) {//Purchased mode is designed for submission to tax authority, only certain columns
				col=new GridColumn(Lan.g(this,"DateSold"),90);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Cost"),80,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Est Value"),80,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			if(display!=EnumEquipmentDisplayMode.Purchased) {
				col=new GridColumn(Lan.g(this,"Location"),80);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Status"),160);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listEquip.Count;i++){
				row=new GridRow();
				row.Cells.Add(listEquip[i].Description);
				row.Cells.Add(listEquip[i].SerialNumber);
				row.Cells.Add(listEquip[i].ModelYear);
				row.Cells.Add(listEquip[i].DatePurchased.ToShortDateString());
				if(display!=EnumEquipmentDisplayMode.Purchased) {
					if(listEquip[i].DateSold.Year<1880) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(listEquip[i].DateSold.ToShortDateString());
					}
				}
				row.Cells.Add(listEquip[i].PurchaseCost.ToString("f"));
				row.Cells.Add(listEquip[i].MarketValue.ToString("f"));
				if(display!=EnumEquipmentDisplayMode.Purchased) {
					row.Cells.Add(listEquip[i].Location);
				}
				row.Cells.Add(listEquip[i].Status.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			Equipment equip=new Equipment();
			equip.SerialNumber=Equipments.GenerateSerialNum();
			equip.DateEntry=DateTime.Today;
			equip.DatePurchased=DateTime.Today;
			using FormEquipmentEdit form=new FormEquipmentEdit();
			form.IsNew=true;
			form.Equip=equip;
			form.ShowDialog();
			if(form.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEquipmentEdit form=new FormEquipmentEdit();
			form.Equip=listEquip[e.Row];
			form.ShowDialog();
			if(form.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Equipment list printed"));
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!headingPrinted) {
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
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=textDateStart.Text+" "+Lan.g(this,"to")+" "+textDateEnd.Text;
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
				double total=0;
				for(int i=0;i<listEquip.Count;i++){
					total+=listEquip[i].MarketValue;
				}
				g.DrawString(Lan.g(this,"Total Est Value:")+" "+total.ToString("c"),Font,Brushes.Black,550,yPos);
			}
			g.Dispose();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		

		

	

		

		

		
	}
}