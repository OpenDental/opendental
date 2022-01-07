using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Drawing.Printing;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormSupplyOrders:FormODBase {
		private List<Supplier> _listSuppliers;
		private List<SupplyOrder> _listSupplyOrders;
		///<summary>Order Items are easier to process as a DataTable, with columns from multiple db tables.</summary>
		private DataTable _tableOrderItems;
		private int _pagesPrinted;
		private bool _headingPrinted;
		private int _headingPrintH;
		///<summary>This user pref remembers whether the user has checked the "Show received" checkbox.</summary>
		private UserOdPref _userPrefShowReceived;

		public FormSupplyOrders() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSupplyOrders_Load(object sender,EventArgs e) {
			Height=SystemInformation.WorkingArea.Height;//max height
			Location=new Point(Location.X,0);//move to top of screen
			_listSupplyOrders = new List<SupplyOrder>();
			_listSuppliers=Suppliers.GetAll();
			comboSupplier.IncludeAll=true;
			comboSupplier.Items.AddList(_listSuppliers,x=>x.Name);
			comboSupplier.IsAllSelected=true;
			_userPrefShowReceived=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ReceivedSupplyOrders).FirstOrDefault();
			if(_userPrefShowReceived!=null && PIn.Bool(_userPrefShowReceived.ValueString)) {
				checkShowReceived.Checked=true;
			} 
			else { 
				checkShowReceived.Checked=false;
			}
			FillGridOrders();
			gridOrders.ScrollToEnd();
		}

		private void comboSupplier_SelectionChangeCommitted(object sender, EventArgs e){
			FillGridOrders();
			gridOrders.ScrollToEnd();
			FillGridOrderItem();//to clear it
		}

		private void gridOrder_CellClick(object sender,ODGridClickEventArgs e) {
			FillGridOrderItem();
		}

		private void gridOrder_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormSupplyOrderEdit formSupplyOrderEdit = new FormSupplyOrderEdit();
			formSupplyOrderEdit.ListSuppliersAll = _listSuppliers;
			formSupplyOrderEdit.SupplyOrderCur = _listSupplyOrders[e.Row];
			formSupplyOrderEdit.ShowDialog();
			if(formSupplyOrderEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGridOrders();
			FillGridOrderItem();
		}

		private void butAddSupply_Click(object sender,EventArgs e) {
			if(gridOrders.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a supply order to add items to first.");
				return;
			}
			using FormSupplies formSupplies = new FormSupplies();
			formSupplies.IsSelectMode = true;
			formSupplies.SelectedSupplierNum = _listSupplyOrders[gridOrders.GetSelectedIndex()].SupplierNum;
			formSupplies.ShowDialog();
			if(formSupplies.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<formSupplies.ListSuppliesSelected.Count;i++) {
				//check for existing----			
				if(_tableOrderItems.Rows.OfType<DataRow>().Any(x => PIn.Long(x["SupplyNum"].ToString())==formSupplies.ListSuppliesSelected[i].SupplyNum)) {
					//MsgBox.Show(this,"Selected item already exists in currently selected order. Please edit quantity instead.");
					continue;
				}
				SupplyOrderItem supplyOrderItem = new SupplyOrderItem();
				supplyOrderItem.SupplyNum = formSupplies.ListSuppliesSelected[i].SupplyNum;
				supplyOrderItem.Qty=1;
				supplyOrderItem.Price = formSupplies.ListSuppliesSelected[i].Price;
				supplyOrderItem.SupplyOrderNum = _listSupplyOrders[gridOrders.GetSelectedIndex()].SupplyOrderNum;
				SupplyOrderItems.Insert(supplyOrderItem);
			}
			UpdatePriceAndRefresh();
		}

		private void FillGridOrders() {
			long supplierNum=0;
			if(!comboSupplier.IsAllSelected){
				supplierNum=comboSupplier.GetSelectedKey<Supplier>(x=>x.SupplierNum);
			}
			_listSupplyOrders=SupplyOrders.GetList(supplierNum);
			if(!checkShowReceived.Checked) {
				_listSupplyOrders=_listSupplyOrders.FindAll(x => x.DateReceived.Year < 1880);
			} 
			//Show the not received items at the bottom, then order by date placed.
			_listSupplyOrders=_listSupplyOrders.OrderBy(x => x.DateReceived.Year < 1880)
				.ThenBy(x => x.DatePlaced).ToList();
			gridOrders.BeginUpdate();
			gridOrders.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date Placed"),80);
			gridOrders.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Date Received"),90);
			gridOrders.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Amount"),70,HorizontalAlignment.Right);
			gridOrders.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Shipping"),70,HorizontalAlignment.Right);
			gridOrders.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Supplier"),120);
			gridOrders.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Note"),200);
			gridOrders.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Placed By"),100);
			gridOrders.ListGridColumns.Add(col);
			gridOrders.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listSupplyOrders.Count;i++) {
				row=new GridRow();
				bool isPending=false;
				if(_listSupplyOrders[i].DatePlaced.Year>2200) {
					isPending=true;
				}
				if(isPending) {
					row.Cells.Add(Lan.g(this,"pending"));
				}
				else {
					row.Cells.Add(_listSupplyOrders[i].DatePlaced.ToShortDateString());
				}
				if(_listSupplyOrders[i].DateReceived.Year < 1880) {
					row.Cells.Add("");
				} 
				else { 
					row.Cells.Add(_listSupplyOrders[i].DateReceived.ToShortDateString());
				}
				row.Cells.Add(_listSupplyOrders[i].AmountTotal.ToString("c"));
				row.Cells.Add(_listSupplyOrders[i].ShippingCharge.ToString("c"));
				row.Cells.Add(Suppliers.GetName(_listSuppliers,_listSupplyOrders[i].SupplierNum));
				row.Cells.Add(_listSupplyOrders[i].Note);
				if(isPending || _listSupplyOrders[i].UserNum==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Userods.GetName(_listSupplyOrders[i].UserNum));
				}
				row.Tag=_listSupplyOrders[i];
				gridOrders.ListGridRows.Add(row);
			}
			gridOrders.EndUpdate();
		}

		private void FillGridOrderItem(bool refresh=true) {
			long orderNum=0;
			if(gridOrders.GetSelectedIndex()!=-1) {//an order is selected
				orderNum=_listSupplyOrders[gridOrders.GetSelectedIndex()].SupplyOrderNum;
			}
			if(refresh){
				_tableOrderItems=SupplyOrderItems.GetItemsForOrder(orderNum);
			}
			gridItems.BeginUpdate();
			gridItems.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Catalog #"),80);
			gridItems.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),320);
			gridItems.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Qty"),60,HorizontalAlignment.Center);
			col.IsEditable=true;
			gridItems.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Price/Unit"),70,HorizontalAlignment.Right);
			col.IsEditable=true;
			gridItems.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Subtotal"),70,HorizontalAlignment.Right);
			gridItems.ListGridColumns.Add(col);
			gridItems.ListGridRows.Clear();
			GridRow row;
			double price;
			int qty;
			double subtotal;
			for(int i=0;i<_tableOrderItems.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_tableOrderItems.Rows[i]["CatalogNumber"].ToString());
				row.Cells.Add(_tableOrderItems.Rows[i]["Descript"].ToString());
				qty=PIn.Int(_tableOrderItems.Rows[i]["Qty"].ToString());
				row.Cells.Add(qty.ToString());
				price=PIn.Double(_tableOrderItems.Rows[i]["Price"].ToString());
				row.Cells.Add(price.ToString("n"));
				subtotal=((double)qty)*price;
				row.Cells.Add(subtotal.ToString("n"));
				gridItems.ListGridRows.Add(row);
			}
			gridItems.EndUpdate();
		}

		private void checkShowReceived_MouseClick(object sender,MouseEventArgs e) {
			FillGridOrders();
		}

		private void gridOrderItem_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormSupplyOrderItemEdit formSupplyOrderItemEdit = new FormSupplyOrderItemEdit();
			formSupplyOrderItemEdit.SupplyOrderItemCur = SupplyOrderItems.SelectOne(PIn.Long(_tableOrderItems.Rows[e.Row]["SupplyOrderItemNum"].ToString()));
			formSupplyOrderItemEdit.ListSuppliersAll = Suppliers.GetAll();
			formSupplyOrderItemEdit.ShowDialog();
			if(formSupplyOrderItemEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			//SupplyOrderItems.Update(formSupplyOrderItemEdit.SupplyOrderItemCur);
			UpdatePriceAndRefresh();
		}

		private void butNewOrder_Click(object sender,EventArgs e) {
			if(comboSupplier.IsAllSelected){
				MsgBox.Show(this,"Please select a supplier first.");
				return;
			}
			for(int i=0;i<_listSupplyOrders.Count;i++) {
				if(_listSupplyOrders[i].DatePlaced.Year>2200) {
					MsgBox.Show(this,"Not allowed to add a new order when there is already one pending.  Please finish the other order instead.");
					return;
				}
			}
			SupplyOrder order=new SupplyOrder();
			if(comboSupplier.IsAllSelected){
				order.SupplierNum=0;
			}
			else {//Specific supplier selected.
				order.SupplierNum=comboSupplier.GetSelectedKey<Supplier>(x=>x.SupplierNum);
			}
			order.IsNew=true;
			order.DatePlaced=new DateTime(2300,1,1);
			order.Note="";
			order.UserNum=0;//This will get set when the order is placed.
			SupplyOrders.Insert(order);
			FillGridOrders();
			gridOrders.SetSelected(_listSupplyOrders.Count-1,true);
			gridOrders.ScrollToEnd();
			FillGridOrderItem();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(_tableOrderItems.Rows.Count<1) {
				MsgBox.Show(this,"Supply list is Empty.");
				return;
			}
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(
				pd2_PrintPage,
				Lan.g(this,"Supplies order from")+" "+_listSupplyOrders[gridOrders.GetSelectedIndex()].DatePlaced.ToShortDateString()+" "+Lan.g(this,"printed"),
				margins:new Margins(50,50,40,30)
			);
		}

		private void pd2_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			Font mainFont=new Font("Arial",9);
			int yPos=bounds.Top;
			#region printHeading
			if(!_headingPrinted) {
				text=Lan.g(this,"Supply Order");
				g.DrawString(text,headingFont,Brushes.Black,425-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=Lan.g(this,"Order Number")+": "+_listSupplyOrders[gridOrders.SelectedIndices[0]].SupplyOrderNum;
				g.DrawString(text,subHeadingFont,Brushes.Black,425-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				text=Lan.g(this,"Date")+": "+_listSupplyOrders[gridOrders.SelectedIndices[0]].DatePlaced.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,425-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				Supplier supplier=Suppliers.GetOne(_listSupplyOrders[gridOrders.SelectedIndices[0]].SupplierNum);
				text=supplier.Name;
				g.DrawString(text,subHeadingFont,Brushes.Black,425-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				text=supplier.Phone;
				g.DrawString(text,subHeadingFont,Brushes.Black,425-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				text=supplier.Note;
				g.DrawString(text,subHeadingFont,Brushes.Black,425-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				yPos+=15;
				_headingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=gridItems.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void gridItems_CellLeave(object sender,ODGridClickEventArgs e) {
			int qtyOld=PIn.Int(_tableOrderItems.Rows[e.Row]["Qty"].ToString(),false);
			int qtyNew=0;
			try {
				qtyNew=PIn.Int(gridItems.ListGridRows[e.Row].Cells[2].Text);//0 if not valid input
			}
			catch { }
			double priceOld=PIn.Double(_tableOrderItems.Rows[e.Row]["Price"].ToString());
			double priceNew=PIn.Double(gridItems.ListGridRows[e.Row].Cells[3].Text);//0 if not valid input
			//if(e.Col==2){//Qty
				//gridItems.ListGridRows[e.Row].Cells[2].Text=qtyNew.ToString();//Fix the cell formatting
				//if(qtyOld==qtyNew){
					//don't go to db.  
					//gridItems.Invalidate();
					//return;
				//}
			//}
			//if(e.Col==3){//price
				//gridItems.ListGridRows[e.Row].Cells[3].Text=priceNew.ToString("n");//Fix the cell formatting
				//if(priceOld==priceNew){
					//don't go to db.  
					//gridItems.Invalidate();
					//return;
				//}
			//}
			//gridItems.ListGridRows[e.Row].Cells[4].Text=(qtyNew*priceNew).ToString("n");
			//gridItems.Invalidate();
			if(qtyOld==qtyNew && priceOld==priceNew){
				FillGridOrderItem(false);//no refresh
			}
			else{
				SupplyOrderItem supplyOrderItem=SupplyOrderItems.SelectOne(PIn.Long(_tableOrderItems.Rows[e.Row]["SupplyOrderItemNum"].ToString()));
				supplyOrderItem.Qty=qtyNew;
				supplyOrderItem.Price=priceNew;
				SupplyOrderItems.Update(supplyOrderItem);
				SupplyOrder updatedSupplyOrderItem=SupplyOrders.UpdateOrderPrice(supplyOrderItem.SupplyOrderNum);//this might be an expensive query that we could avoid
				FillGridOrderItem();
				int index=_listSupplyOrders.FindIndex(x=>x.SupplyOrderNum==supplyOrderItem.SupplyOrderNum);
				if(index<0) {//Just in case, shouldn't happen
					FillGridOrders();
					return;
				}
				_listSupplyOrders[index]=updatedSupplyOrderItem;
				gridOrders.SelectedGridRows[0].Cells[2].Text=updatedSupplyOrderItem.AmountTotal.ToString("c2");
				gridOrders.Invalidate();
			}
		}

		private void UpdatePriceAndRefresh() {
			SupplyOrder gridSelect=gridOrders.SelectedTag<SupplyOrder>();
			SupplyOrders.UpdateOrderPrice(_listSupplyOrders[gridOrders.GetSelectedIndex()].SupplyOrderNum);
			FillGridOrders();
			for(int i=0;i<gridOrders.ListGridRows.Count;i++) {
				if(gridSelect!=null && ((SupplyOrder)gridOrders.ListGridRows[i].Tag).SupplyOrderNum==gridSelect.SupplyOrderNum) {
					gridOrders.SetSelected(i,true);
				}
			}
			FillGridOrderItem();
		}

		private void butClose_Click(object sender,EventArgs e) {
			if(_userPrefShowReceived==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.ReceivedSupplyOrders,
					ValueString=POut.Bool(checkShowReceived.Checked)
				});
			}
			else {
				if(_userPrefShowReceived.ValueString!=POut.Bool(checkShowReceived.Checked)) {//The user preference has changed.
					_userPrefShowReceived.ValueString=POut.Bool(checkShowReceived.Checked);
					UserOdPrefs.Update(_userPrefShowReceived);
				}
			}
			Close();
		}

		
	}
}