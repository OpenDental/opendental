using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormEtrans277Edit:FormODBase {

		public Etrans EtransCur;
		private string _msgText;
		private X277 _x277;

		public FormEtrans277Edit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEtrans277Edit_Load(object sender,EventArgs e) {
			_msgText=EtransMessageTexts.GetMessageText(EtransCur.EtransMessageTextNum);
			try {
				_x277=new X277(_msgText);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Claim Status Response not valid.  An error occurred while loading: ")+"\r\n"+ex.Message);
				DialogResult=DialogResult.Cancel;
				return;
			}
			FillHeader();
			FillGrid();
		}

		private void FormEtrans277Edit_Resize(object sender,EventArgs e) {
			//This funciton is called before FormEtrans277Edit_Load() when using ShowDialog(). Therefore, x277 is null the first time FormEtrans277Edit_Resize() is called.
			if(_x277==null) {
				return;
			}
			FillGrid();
		}

		private void FillHeader() {
			//Set the title of the window to include he reponding entity type and name (i.e. payor delta, clearinghouse emdeon, etc...)
			Text+=_x277.GetInformationSourceType()+" "+_x277.GetInformationSourceName();
			//Fill the textboxes in the upper portion of the window.
			textReceiptDate.Text=_x277.GetInformationSourceReceiptDate().ToShortDateString();
			textProcessDate.Text=_x277.GetInformationSourceProcessDate().ToShortDateString();
			//textQuantityAccepted.Text=x277.GetQuantityAccepted().ToString();
			//textQuantityRejected.Text=x277.GetQuantityRejected().ToString();
			//textAmountAccepted.Text=x277.GetAmountAccepted().ToString("F");
			//textAmountRejected.Text=x277.GetAmountRejected().ToString("F");
		}

		private void FillGrid() {
			int numAccepted=0;
			int numRejected=0;
			decimal amountAccepted=0;
			decimal amountRejected=0;
			List<string> listClaimTrackingNumbers=_x277.GetClaimTrackingNumbers();
			//bool showInstBillType=false;
			bool showServiceDateRange=false;
			for(int i=0;i<listClaimTrackingNumbers.Count;i++) {
				string[] stringArrayClaimInfo=_x277.GetClaimInfo(listClaimTrackingNumbers[i]);
				//if(claimInfo[5]!="") { //institutional type of bill
				//  showInstBillType=true;
				//}
				if(stringArrayClaimInfo[7]!="") {//service date end
					showServiceDateRange=true;
				}
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			int width=this.Width-2*gridMain.Left-10;
			if(showServiceDateRange) {
				const int serviceDateFromWidth=86;
				col=new GridColumn(Lan.g(this,"ServDateFrom"),serviceDateFromWidth,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				width-=serviceDateFromWidth;
				const int serviceDateToWidth=80;
				col=new GridColumn(Lan.g(this,"ServDateTo"),serviceDateToWidth,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				width-=serviceDateToWidth;
			}
			else {
				const int serviceDateWidth=80;
				col=new GridColumn(Lan.g(this,"ServiceDate"),serviceDateWidth,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
				width-=serviceDateWidth;
			}
			const int amountWidth=80;
			col=new GridColumn(Lan.g(this,"Amount"),amountWidth,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			width-=amountWidth;
			const int statusWidth=54;
			col=new GridColumn(Lan.g(this,"Status"),statusWidth,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			width-=statusWidth;
			const int lnameWidth=150;
			const int fnameWidth=100;
			const int claimIdWidth=100;
			const int payorControlNumWidth=126;
			width+=-lnameWidth-fnameWidth-claimIdWidth-payorControlNumWidth;
			col=new GridColumn(Lan.g(this,"Reason"),width);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"LName"),lnameWidth);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"FName"),fnameWidth);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"ClaimIdentifier"),claimIdWidth);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"PayorControlNum"),payorControlNumWidth);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listClaimTrackingNumbers.Count;i++) {
				string[] stringArrayClaimInfo=_x277.GetClaimInfo(listClaimTrackingNumbers[i]);
			  GridRow row=new GridRow();
				row.Cells.Add(new GridCell(stringArrayClaimInfo[6]));//service date start
				if(showServiceDateRange) {					
					row.Cells.Add(new GridCell(stringArrayClaimInfo[7]));//service date end
				}
				string claimStatus="";
				decimal claimAmount=PIn.Decimal(stringArrayClaimInfo[9]);
				if(stringArrayClaimInfo[3]=="A") {
					claimStatus="Accepted";
					numAccepted++;
					amountAccepted+=claimAmount;
				}
				else if(stringArrayClaimInfo[3]=="R") {
					claimStatus="Rejected";
					numRejected++;
					amountRejected+=claimAmount;
				}
				row.Cells.Add(new GridCell(claimAmount.ToString("F")));//amount
				row.Cells.Add(new GridCell(claimStatus));//status
				row.Cells.Add(new GridCell(stringArrayClaimInfo[8]));//reason
				row.Cells.Add(new GridCell(stringArrayClaimInfo[0]));//lname
				row.Cells.Add(new GridCell(stringArrayClaimInfo[1]));//fname
				row.Cells.Add(new GridCell(listClaimTrackingNumbers[i]));//claim identifier
				row.Cells.Add(new GridCell(stringArrayClaimInfo[4]));//payor control number
			  gridMain.ListGridRows.Add(row);
			}			
			gridMain.EndUpdate();
			textQuantityAccepted.Text=numAccepted.ToString();
			textQuantityRejected.Text=numRejected.ToString();
			textAmountAccepted.Text=amountAccepted.ToString("F");
			textAmountRejected.Text=amountRejected.ToString("F");
		}

		private void butRawMessage_Click(object sender,EventArgs e) {
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(_msgText);
			msgBoxCopyPaste.ShowDialog();
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}
		
	}
}