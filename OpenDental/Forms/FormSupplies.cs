using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Drawing.Printing;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormSupplies:FormODBase {
		///<summary>A list of all suppliers obtained on window load.</summary>
		private List<Supplier> _listSuppliers;
		/// <summary>Supplies currently being displayed</summary>
		private List<Supply> _listSupplies;
		private int pagesPrinted;
		private bool headingPrinted;
		private int headingPrintH;
		public bool IsSelectMode;
		///<summary>If selecting supplies, this is used after this form closes.</summary>
		public List<Supply> ListSuppliesSelected;
		///<summary>If selecting supplies, this is passed in before.</summary>
		public long SelectedSupplierNum;

		public FormSupplies() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSupplies_Load(object sender,EventArgs e) {
			this.Height=SystemInformation.WorkingArea.Height;//Resize height
			this.Location=new Point(Location.X,0);//Move window to compensate for changed height
			_listSuppliers=Suppliers.GetAll();
			comboSuppliers.Items.Clear();
			comboSuppliers.IncludeAll=true;
			comboSuppliers.IsAllSelected=true;
			comboSuppliers.Items.AddList(_listSuppliers,x=>x.Name);
			if(IsSelectMode){
				butAdd.Visible=false;
				labelSuppliers.Visible=false;
				comboSuppliers.Visible=false;
				//comboSupplier.SetSelectedKey<Supply>(SelectedSupplierNum,x=>x.SupplierNum,x=>Suppliers.GetName(_listSuppliers,x)); 
				//comboSupplier.Enabled=false;
				checkEnterQty.Visible=false;
				butUp.Visible=false;
				butDown.Visible=false;
				groupCreateOrders.Visible=false;
				labelCreateOrder.Visible=false;
				butPrint.Visible=false;
				labelPrint.Visible=false;
			}
			else{
				butOK.Visible=false;
				butCancel.Text="Close";
			}
			comboCategories.IncludeAll=true;
			comboCategories.Items.AddDefs(Defs.GetDefsForCategory(DefCat.SupplyCats,true));//not showing hidden categories
			comboCategories.IsAllSelected=true;
			FillGrid();
		}

		private void FillGrid(bool refresh=true){
			List<long> listSupplierNums=null;
			if(IsSelectMode){
				listSupplierNums=new List<long>();
				listSupplierNums.Add(SelectedSupplierNum);
			}
			else if(comboSuppliers.IsAllSelected){
				listSupplierNums=null;
			}
			else{
				listSupplierNums=comboSuppliers.GetListSelected<Supplier>().Select(x=>x.SupplierNum).ToList();
			}
			List<long> listCategories=null;
			if(!comboCategories.IsAllSelected){
				listCategories=comboCategories.GetListSelected<Def>().Select(x=>x.DefNum).ToList();
			}
			if(refresh){
				_listSupplies=Supplies.GetList(listSupplierNums,checkShowHidden.Checked,textFind.Text,listCategories);
			}
			if(textFind.Text=="" && comboSuppliers.IsAllSelected){
				//check orders.  Simply look for any two sequential itemOrders that are the same.  This indicates fix required for that category.
				//Works for hidden or not, but won't fire repeatedly unless user is switching back and forth between hidden and unhidden.
				bool didFix=false;
				for(int i=1;i<_listSupplies.Count;i++){//starts at 1
					if(_listSupplies[i].Category != _listSupplies[i-1].Category){
						//if previous item is different category, then we don't need to compare
						continue;
					}
					if(_listSupplies[i].ItemOrder == _listSupplies[i-1].ItemOrder){//duplicate found
						didFix=true;
						FixItemOrders(_listSupplies[i].Category);
						while(i<_listSupplies.Count
							&& _listSupplies[i].Category == _listSupplies[i-1].Category)
						{
							i++;//skip ahead to next category
						}
					}
				}
				if(didFix){
					//get list again
					_listSupplies=Supplies.GetList(listSupplierNums,checkShowHidden.Checked,textFind.Text,listCategories);
				}
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Category"),130);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Catalog #"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Supplier"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Price"),60,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Stock"),50,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"On Hand"),60,HorizontalAlignment.Center);
			if(checkEnterQty.Checked) {
				col.IsEditable=true;
			}
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Order"),50,HorizontalAlignment.Center);
			if(checkEnterQty.Checked){
				col.IsEditable=true;
			}
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Hidden"),50,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listSupplies.Count;i++) {
				row=new GridRow();
				if(gridMain.ListGridRows.Count==0 || _listSupplies[i].Category != _listSupplies[i-1].Category) {
					row.Cells.Add(Defs.GetName(DefCat.SupplyCats,_listSupplies[i].Category));//Add the new category header in this row if it doesn't match the previous row's category.
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(_listSupplies[i].CatalogNumber);
				row.Cells.Add(Suppliers.GetName(_listSuppliers,_listSupplies[i].SupplierNum));
				row.Cells.Add(_listSupplies[i].Descript);//_listSupplies[i].ItemOrder.ToString()+"  "+_listSupplies[i].Descript);
				if(_listSupplies[i].Price==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listSupplies[i].Price.ToString("n"));
				}
				//stock:
				if(_listSupplies[i].LevelDesired==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listSupplies[i].LevelDesired.ToString());
				}
				row.Cells.Add(_listSupplies[i].LevelOnHand.ToString());
				//order:
				if(_listSupplies[i].OrderQty==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listSupplies[i].OrderQty.ToString());
				}
				//hidden:
				row.Cells.Add(_listSupplies[i].IsHidden?"X":"");
				//row.Tag=_listSupplies[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			//for(int i=0;i<gridMain.ListGridRows.Count;i++) {
			//	if(_listSelectedSupplies.Contains(((Supply)gridMain.ListGridRows[i].Tag))) {
			//		gridMain.SetSelected(i,true);
			//	}
			//}
		}

		///<summary>Fixes all ItemOrders for a category where a duplicate was found.  Includes IsHidden so that it works for both hidden and unhidden.</summary>
		private void FixItemOrders(long category){
			List<long> listCategories=new List<long>();
			listCategories.Add(category);
			List<Supply> listSuppliesCat=Supplies.GetList(null,true,"",listCategories);
			for(int i=0;i<listSuppliesCat.Count;i++){
				if(listSuppliesCat[i].ItemOrder!=i){
					listSuppliesCat[i].ItemOrder=i;
					Supplies.Update(listSuppliesCat[i]);
				}
			}
		}

		private void checkEnterQty_Click(object sender, EventArgs e){
			if(checkEnterQty.Checked){
				gridMain.SelectionMode=GridSelectionMode.OneCell;
			}
			else{
				gridMain.SelectionMode=GridSelectionMode.MultiExtended;
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(Defs.GetDefsForCategory(DefCat.SupplyCats,true).Count==0) {
				MsgBox.Show(this,"No supply categories have been created.  Go to the supply inventory window, select categories, and enter at least one supply category first.");
				return;
			}
			if(_listSuppliers.Count==0) {
				MsgBox.Show(this,"No suppliers have been created.  Go to the suppliers window to add suppliers first.");
				return;
			}
			if(comboSuppliers.IsAllSelected || comboSuppliers.SelectedIndices.Count==0){
				MsgBox.Show(this,"Please select one supplier, first.");//because supplier can't ever change, so they need to be looking at just one supplier.
				//an enhancement would be to let them pick a supplier for a new supply, but then not edit.
				return;
			}
			using FormSupplyEdit formSupplyEdit=new FormSupplyEdit();
			formSupplyEdit.ListSuppliers=_listSuppliers;
			Supply supply=new Supply();
			//category:
			if(gridMain.SelectedIndices.Length>0){
				supply.Category=_listSupplies[gridMain.SelectedIndices[0]].Category;
			}
			else if(comboCategories.IsAllSelected || comboCategories.SelectedIndices.Count==0){
				supply.Category=((Def)comboCategories.Items.GetObjectAt(0)).DefNum;
			}
			else{
				supply.Category=((Def)comboCategories.Items.GetObjectAt(comboCategories.SelectedIndices[0])).DefNum;
			}
			//supplier:
			supply.SupplierNum=_listSuppliers[comboSuppliers.SelectedIndices[0]].SupplierNum;
			//itemOrder:
			if(gridMain.SelectedIndices.Length>0){
				supply.ItemOrder=_listSupplies[gridMain.SelectedIndices[0]].ItemOrder;
				//even if many items are hidden for various reasons, this works.  Query at bottom of this method blindly moves all the others down.
			}
			else{
				//nothing is selected, so we'll just add it to the end of whatever category we picked above, typically the first category.
				supply.ItemOrder=Supplies.GetLastItemOrder(supply.Category)+1;
				//The query below will then just do nothing because no ItemOrders below
			}
			supply.IsNew=true;
			formSupplyEdit.SupplyCur=supply;
			formSupplyEdit.ShowDialog();
			if(formSupplyEdit.DialogResult!=DialogResult.OK) {
				return;	
			}
			//move other item orders down
			Supplies.OrderAddOneGreater(supply.ItemOrder,formSupplyEdit.SupplyCur.Category,formSupplyEdit.SupplyCur.SupplyNum);//example added at 3, so ++ item orders >=3
			int selected=-1;
			if(gridMain.SelectedIndices.Length>0){
				selected=gridMain.SelectedIndices[0];
			}
			FillGrid();
			if(selected!=-1){
				gridMain.SetSelected(selected,true);
				return;
			}
			//none was selected, but we want to select the new item.
			for(int i=0;i<_listSupplies.Count;i++){
				if(_listSupplies[i].SupplyNum==formSupplyEdit.SupplyCur.SupplyNum){
					gridMain.SetSelected(i,true);
					return;
				}
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectMode){
				ListSuppliesSelected=new List<Supply>();
				ListSuppliesSelected.Add(_listSupplies[e.Row]);
				DialogResult=DialogResult.OK;
				return;
			}
			using FormSupplyEdit formSupplyEdit=new FormSupplyEdit();
			formSupplyEdit.ListSuppliers=_listSuppliers;
			formSupplyEdit.SupplyCur=_listSupplies[e.Row];
			formSupplyEdit.ShowDialog();
			if(formSupplyEdit.DialogResult!=DialogResult.OK) {
				return;	
			}
			FillGrid();
			for(int i=0;i<_listSupplies.Count;i++){
				if(_listSupplies[i].SupplyNum==formSupplyEdit.SupplyCur.SupplyNum){
					gridMain.SetSelected(i,true);
					return;
				}
			}
		}

		private void comboSupplier_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboCategories_SelectionChangeCommitted(object sender, EventArgs e){
			FillGrid();
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillGrid();
		}

		/*
		private void checkShowShoppingList_Click(object sender,EventArgs e) {
			_listSelectedSupplies.Clear();
			foreach(int index in gridMain.SelectedIndices) {
				_listSelectedSupplies.Add((Supply)gridMain.ListGridRows[index].Tag);
			}
			FillGrid();
		}*/

		private void butUp_Click(object sender,EventArgs e) {
			if(textFind.Text!=""){
				MsgBox.Show(this,"Not allowed unless search text is empty.");
				return;
			}
			if(!comboSuppliers.IsAllSelected){
				MsgBox.Show(this,"Not allowed unless All suppliers are selected.");
				return;
			}
			List<int> listSelected=gridMain.SelectedIndices.ToList<int>();
			if(listSelected.Count==0){
				MsgBox.Show(this,"Please select at least one supply, first.");
				return;
			}
			for(int i=1;i<listSelected.Count;i++) {
				if(_listSupplies[listSelected[0]].Category != _listSupplies[listSelected[i]].Category){
					MsgBox.Show(this,"All selected supplies must be in the same category.");
					return;
				}
				if(listSelected[i-1]+1 != listSelected[i]){
					MsgBox.Show(this,"Selection must not have gaps.");//makes my math too hard
					return;
				}
			}
			if(listSelected[0]==0){
				return;//already at top
			}
			if(_listSupplies[listSelected[0]].Category != _listSupplies[listSelected[0]-1].Category){
				MsgBox.Show(this,"Already at top of category.");
				return;
			}
			//we should only have to move them up one, but there could be hidden supplies that would require moving up more.
			//There can also be hidden supplies that cause gaps in our selected list.  
			//After considering many options, the best solution is to move group up one, and then set the item above to the ItemOrder of the bottom in group.


			int countMove=_listSupplies[listSelected[0]].ItemOrder-_listSupplies[listSelected[0]-1].ItemOrder;//example 2-1=1
//todo: fix gaps caused by hidden supplies



			List<long> listSupplyNums=new List<long>();
			for(int i=0;i<listSelected.Count;i++) {
				listSupplyNums.Add(_listSupplies[listSelected[i]].SupplyNum);
			}
			Supplies.OrderSubtract(listSupplyNums,countMove);
			_listSupplies[listSelected[0]-1].ItemOrder+=listSelected.Count+countMove-1;//Example 0+5+1-1=5, while the 5 selected items moved up to occupy 0 through 4.
			Supplies.Update(_listSupplies[listSelected[0]-1]);	
			int previousSelectedColumn=gridMain.SelectedCell.X;
			FillGrid();
			int[] indicesNew=new int[listSelected.Count];
			for(int i=0;i<listSelected.Count;i++){
				indicesNew[i]=listSelected[i]-1;
			}
			if(gridMain.SelectionMode==GridSelectionMode.OneCell) {
				gridMain.SetSelected(new Point(previousSelectedColumn,indicesNew[0]));
			}
			else {
				gridMain.SetSelected(indicesNew,true);
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(textFind.Text!=""){
				MsgBox.Show(this,"Not allowed unless search text is empty.");
				return;
			}
			if(!comboSuppliers.IsAllSelected){
				MsgBox.Show(this,"Not allowed unless All suppliers are selected.");
				return;
			}
			List<int> listSelected=gridMain.SelectedIndices.ToList<int>();
			if(listSelected.Count==0){
				MsgBox.Show(this,"Please select at least one supply, first.");
				return;
			}
			for(int i=1;i<listSelected.Count;i++) {
				if(_listSupplies[listSelected[0]].Category != _listSupplies[listSelected[i]].Category){
					MsgBox.Show(this,"All selected supplies must be in the same category.");
					return;
				}
				if(listSelected[i-1]+1 != listSelected[i]){
					MsgBox.Show(this,"Selection must not have gaps.");//makes my math too hard
					return;
				}
			}
			if(listSelected[listSelected.Count-1]==_listSupplies.Count-1){
				return;//already at bottom
			}
			if(_listSupplies[listSelected[listSelected.Count-1]].Category 
				!= _listSupplies[listSelected[listSelected.Count-1]+1].Category){
				MsgBox.Show(this,"Already at bottom of category.");
				return;
			}
			int countMove=_listSupplies[listSelected[listSelected.Count-1]+1].ItemOrder-_listSupplies[listSelected[listSelected.Count-1]].ItemOrder;//example 5-4=1
			List<long> listSupplyNums=new List<long>();
			for(int i=0;i<listSelected.Count;i++) {
				listSupplyNums.Add(_listSupplies[listSelected[i]].SupplyNum);
			}
			Supplies.OrderAdd(listSupplyNums,countMove);
			_listSupplies[listSelected[listSelected.Count-1]+1].ItemOrder
				-=listSelected.Count+countMove-1;//Example 5-5+1-1=0, while the 5 selected items moved down to occupy 1 through 5.
			Supplies.Update(_listSupplies[listSelected[listSelected.Count-1]+1]);	
			int previousSelectedColumn=gridMain.SelectedCell.X;
			FillGrid();
			int[] indicesNew=new int[listSelected.Count];
			for(int i=0;i<listSelected.Count;i++){
				indicesNew[i]=listSelected[i]+1;
			}
			if(gridMain.SelectionMode==GridSelectionMode.OneCell) {
				gridMain.SetSelected(new Point(previousSelectedColumn,indicesNew[0]));
			}
			else {
				gridMain.SetSelected(indicesNew,true);
			}
		}

		private void timerFillGrid_Tick(object sender=null,EventArgs e=null) {
			timerFillGrid.Stop();//Stop the timer, otherwise the timer tick will just fire this again.
			FillGrid();
		}
		
		private void textFind_TextChanged(object sender,EventArgs e) {
			//Does not get hit when opening.
			timerFillGrid.Stop();//So that typing more letters resets with each letter.
			timerFillGrid.Start();
		}

		private void butSearch_Click(object sender, EventArgs e){
			FillGrid();
		}

		private void gridMain_CellLeave(object sender, ODGridClickEventArgs e){
			if(!checkEnterQty.Checked){
				return;
			}
			if(e.Col!=6 && e.Col!=7){
				return;
			}
			float onHandOld=_listSupplies[e.Row].LevelOnHand;
			float onHandNew=0;
			try {
				onHandNew=PIn.Float(gridMain.ListGridRows[e.Row].Cells[6].Text);
			}
			catch { }
			int qtyOld=_listSupplies[e.Row].OrderQty;
			int qtyNew=0;
			try {
				qtyNew=PIn.Int(gridMain.ListGridRows[e.Row].Cells[7].Text);//0 if not valid input
			}
			catch { }
			if(qtyOld==qtyNew){
				FillGrid(false);//no refresh
			}
			else{
				_listSupplies[e.Row].OrderQty=qtyNew;
				Supplies.Update(_listSupplies[e.Row]);
				FillGrid();
			}
			if(onHandOld==onHandNew) {
				FillGrid(false);
			}
			else {
				_listSupplies[e.Row].LevelOnHand=onHandNew;
				Supplies.Update(_listSupplies[e.Row]);
				FillGrid();
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(gridMain.ListGridRows.Count<1) {
				MsgBox.Show(this,"Supply list is Empty.");
				return;
			}
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd2_PrintPage,Lan.g(this,"Supplies list printed"),margins:new Margins(50,50,40,30));
		}

		/// <summary>Creates a new order with all the items currently highlighted as a new pending order.</summary>
		private void butCreateOrders_Click(object sender,EventArgs e) {
			//Not visible in IsSelectMode
			if(gridMain.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select supplies, first.");
				return;
			}
			//they are not ordered by supplier, so we need to keep track of a local list of orders
			List<SupplyOrder> listSupplyOrders=new List<SupplyOrder>();
			SupplyOrder supplyOrder=null;
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				supplyOrder=listSupplyOrders.FirstOrDefault(x=>x.SupplierNum==_listSupplies[gridMain.SelectedIndices[i]].SupplierNum);
				if(supplyOrder==null){
					supplyOrder=new SupplyOrder();
					supplyOrder.SupplierNum=_listSupplies[gridMain.SelectedIndices[i]].SupplierNum;
					supplyOrder.IsNew=true;
					supplyOrder.DatePlaced=new DateTime(2500,1,1); //date used for new 'pending' orders. 
					supplyOrder.Note="";
					supplyOrder.UserNum=Security.CurUser.UserNum;
					supplyOrder.SupplyOrderNum=SupplyOrders.Insert(supplyOrder);
					listSupplyOrders.Add(supplyOrder);
				}
				SupplyOrderItem supplyOrderItem=new SupplyOrderItem();
				supplyOrderItem.SupplyNum=_listSupplies[gridMain.SelectedIndices[i]].SupplyNum;
				supplyOrderItem.Qty=_listSupplies[gridMain.SelectedIndices[i]].OrderQty;
				supplyOrderItem.Price=_listSupplies[gridMain.SelectedIndices[i]].Price;
				supplyOrderItem.SupplyOrderNum=supplyOrder.SupplyOrderNum;
				SupplyOrderItems.Insert(supplyOrderItem);
			}
			for(int i=0;i<listSupplyOrders.Count;i++){
				SupplyOrders.UpdateOrderPrice(listSupplyOrders[i].SupplyOrderNum);
			}
			MessageBox.Show(Lan.g(this,"Done. Added ")+listSupplyOrders.Count.ToString()+Lan.g(this," orders.  Manage orders from Orders window"));
			DialogResult=DialogResult.OK;
		}

		/// <summary>Creates a new order with all the items that have a Qty entered as a new pending order.</summary>
		private void butCreateOrdersQty_Click(object sender, EventArgs e){
			//Not visible in IsSelectMode
			List<SupplyOrder> listSupplyOrders=new List<SupplyOrder>();
			SupplyOrder supplyOrder=null;
			for(int i=0;i<_listSupplies.Count;i++){
				if(_listSupplies[i].OrderQty==0){
					continue;
				}
				supplyOrder=listSupplyOrders.FirstOrDefault(x=>x.SupplierNum==_listSupplies[i].SupplierNum);
				if(supplyOrder==null){
					supplyOrder=new SupplyOrder();
					supplyOrder.SupplierNum=_listSupplies[i].SupplierNum;
					supplyOrder.IsNew=true;
					supplyOrder.DatePlaced=new DateTime(2500,1,1); //date used for new 'pending' orders. 
					supplyOrder.Note="";
					supplyOrder.UserNum=Security.CurUser.UserNum;
					supplyOrder.SupplyOrderNum=SupplyOrders.Insert(supplyOrder);
					listSupplyOrders.Add(supplyOrder);
				}
				SupplyOrderItem supplyOrderItem=new SupplyOrderItem();
				supplyOrderItem.SupplyNum=_listSupplies[i].SupplyNum;
				supplyOrderItem.Qty=_listSupplies[i].OrderQty;
				supplyOrderItem.Price=_listSupplies[i].Price;
				supplyOrderItem.SupplyOrderNum=supplyOrder.SupplyOrderNum;
				SupplyOrderItems.Insert(supplyOrderItem);
				//Supply has been added to order.  Now, zero out qty on supply.
				_listSupplies[i].OrderQty=0;
				Supplies.Update(_listSupplies[i]);
			}
			if(listSupplyOrders.Count==0){
				MsgBox.Show("Please enter quantities for supplies first.");
				return;
			}
			for(int i=0;i<listSupplyOrders.Count;i++){
				SupplyOrders.UpdateOrderPrice(listSupplyOrders[i].SupplyOrderNum);
			}
			MessageBox.Show(Lan.g(this,"Done. Added ")+listSupplyOrders.Count.ToString()+Lan.g(this," orders.  Manage orders from Orders window"));
			DialogResult=DialogResult.OK;
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
			if(!headingPrinted) {
				text=Lan.g(this,"Supply List");
				g.DrawString(text,headingFont,Brushes.Black,425-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=Lan.g(this,"Date")+": "+DateTime.Today.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,425-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				yPos+=5;
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
			}
			g.Dispose();
		}

		private void butOK_Click(object sender,EventArgs e) {
			//only visible in select mode
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a supply from the list first.");
				return;
			}
			ListSuppliesSelected=new List<Supply>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				ListSuppliesSelected.Add(_listSupplies[gridMain.SelectedIndices[i]]);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormSupplies_FormClosing(object sender,FormClosingEventArgs e) {
			timerFillGrid?.Dispose();//Dispose of the timer if it is not null.
		}
	}
}