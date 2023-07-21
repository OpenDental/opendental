using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormXchargeTokenTool:FormODBase {
		private List<CreditCard> _listCreditCards;
		///<summary>The X-Charge Username for the FormOpenDental.ClinicNum clinic.  Validated on load.</summary>
		private string _xUsername;
		///<summary>The X-Charge Password for the FormOpenDental.ClinicNum clinic.  Validated on load.</summary>
		private string _xPassword;

		public FormXchargeTokenTool() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormXchargeTokenTool_Load(object sender,EventArgs e) {
			Program program=Programs.GetCur(ProgramName.Xcharge);
			if(program==null || !program.Enabled) {
				MsgBox.Show(this,"X-Charge program link is not set up.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			string path=Programs.GetProgramPath(program);
			if(!File.Exists(path)) {
				MsgBox.Show(this,"X-Charge path is not valid.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			//In order for X-Charge to be enabled, the enabled flag must be set and there must be a valid Username, Password, and PaymentType
			//If clinics are enabled, the Username, Password, and PaymentType fields are allowed to be blank/invalid for any clinic not using X-Charge
			//Therefore, we will validate the credentials and payment type using FormOpenDental.ClinicNum
			string paymentType=ProgramProperties.GetPropVal(program.ProgramNum,"PaymentType",Clinics.ClinicNum);
			List<Def> _listDefsPayTypes=Defs.GetDefsForCategory(DefCat.PaymentTypes,true).FindAll(x => x.DefNum.ToString()==paymentType);//should be a list of 0 or 1
			_xUsername=ProgramProperties.GetPropVal(program.ProgramNum,"Username",Clinics.ClinicNum);
			_xPassword=ProgramProperties.GetPropVal(program.ProgramNum,"Password",Clinics.ClinicNum);
			if(string.IsNullOrEmpty(_xUsername) || string.IsNullOrEmpty(_xPassword) || _listDefsPayTypes.Count<1) {
				MsgBox.Show(this,"X-Charge username, password, or payment type for this clinic is invalid.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			_listCreditCards=CreditCards.GetCardsWithTokenBySource(
				new List<CreditCardSource> { CreditCardSource.XServer,CreditCardSource.XServerPayConnect });
			textTotal.Text= _listCreditCards.Count.ToString();
			textVerified.Text="0";
			textInvalid.Text="0";
			if(_listCreditCards.Count==0) {
				MsgBox.Show(this,"There are no credit cards with stored X-Charge tokens in the database.");
				return;
			}
		}

		private void FillGrid() {
			Cursor=Cursors.WaitCursor;
			FilterCardList();
			Cursor=Cursors.Default;
			if(_listCreditCards.Count==0) {
				MsgBox.Show(this,"There are no invalid tokens in the database.");
				return;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormXChargeTest","PatNum"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormXChargeTest","First"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormXChargeTest","Last"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormXChargeTest","CCNumberMasked"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormXChargeTest","Exp"),50);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormXChargeTest","Token"),100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listCreditCards.Count;i++) {
				row=new GridRow();
				Patient patient=Patients.GetLim(_listCreditCards[i].PatNum);
				row.Cells.Add(_listCreditCards[i].PatNum.ToString());
				row.Cells.Add(patient.FName);
				row.Cells.Add(patient.LName);
				string ccNum=_listCreditCards[i].CCNumberMasked;
				if(Regex.IsMatch(ccNum,"^\\d{12}(\\d{0,7})")) {	//Minimum of 12 digits, maximum of 19
					int idxLast4Digits=(ccNum.Length-4);
					ccNum=(new string('X',12))+ccNum.Substring(idxLast4Digits);//replace the first 12 with 12 X's
				}
				row.Cells.Add(ccNum);
				row.Cells.Add(_listCreditCards[i].CCExpiration.ToString("MMyy"));
				row.Cells.Add(_listCreditCards[i].XChargeToken);
				row.Tag=_listCreditCards[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FilterCardList() {
			int verified=0;
			int invalid=0;
			textVerified.Text="0";
			textInvalid.Text="0";
			for(int i=_listCreditCards.Count-1;i>=0;i--) {//looping backwards to remove cards that are valid
				Program program=Programs.GetCur(ProgramName.Xcharge);
				string path=Programs.GetProgramPath(program);
				ProcessStartInfo processStartInfo=new ProcessStartInfo(path);
				string resultFile=PrefC.GetRandomTempFile("txt");
				try {
					File.Delete(resultFile);//delete the old result file.
				}
				catch {
					MsgBox.Show(this,"Could not delete XResult.txt file.  It may be in use by another program, flagged as read-only, or you might not have sufficient permissions.");
					break;
				}
				processStartInfo.Arguments+="/TRANSACTIONTYPE:ARCHIVEVAULTQUERY ";
				processStartInfo.Arguments+="/XCACCOUNTID:"+_listCreditCards[i].XChargeToken+" ";
				processStartInfo.Arguments+="/RESULTFILE:\""+resultFile+"\" ";
				processStartInfo.Arguments+="/USERID:"+ProgramProperties.GetPropVal(program.ProgramNum,"Username",Clinics.ClinicNum)+" ";
				processStartInfo.Arguments+="/PASSWORD:"+CodeBase.MiscUtils.Decrypt(ProgramProperties.GetPropVal(program.ProgramNum,"Password",Clinics.ClinicNum))+" ";
				processStartInfo.Arguments+="/AUTOPROCESS ";
				processStartInfo.Arguments+="/AUTOCLOSE ";
				processStartInfo.Arguments+="/NORESULTDIALOG ";
				Process process=new Process();
				process.StartInfo=processStartInfo;
				process.EnableRaisingEvents=true;
				process.Start();
				while(!process.HasExited) {
					Application.DoEvents();
				}
				Thread.Sleep(200);//Wait 2/10 second to give time for file to be created.
				string resulttext="";
				string account="";
				string exp="";
				using TextReader textReader=new StreamReader(resultFile);
				while(true) {
					string line=null;
					try {
						line=textReader.ReadLine();
					}
					catch {
						MsgBox.Show(this,"Something went wrong validating X-Charge tokens.  Please try again.");
						break;
					}
					if(line is null) {
						break;
					}
					if(resulttext!="") {
						resulttext+="\r\n";
					}
					resulttext+=line;
					if(line.StartsWith("ACCOUNT=") && line.Length>8) {
						account=line.Substring(8);
					}
					else if(line.StartsWith("EXPIRATION=") && line.Length>11) {
						exp=line.Substring(11);
					}
				}
				if(_listCreditCards[i].CCNumberMasked.Length>4 && account.Length>4
					&& _listCreditCards[i].CCNumberMasked.Substring(_listCreditCards[i].CCNumberMasked.Length-4)==account.Substring(account.Length-4)
					&& _listCreditCards[i].CCExpiration.ToString("MMyy")==exp) 
				{
					//The credit card on file matches the one in X-Charge, so remove from the list.
					_listCreditCards.RemoveAt(i);
					verified++;
				}
				else {
					invalid++;
				}
				textVerified.Text=verified.ToString();
				textInvalid.Text=invalid.ToString();
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			CreditCard creditCard=(CreditCard)gridMain.ListGridRows[e.Row].Tag;
			using FormCreditCardManage formCreditCardManage=new FormCreditCardManage(Patients.GetPat(creditCard.PatNum));
			formCreditCardManage.ShowDialog();
			int totalCCs=PIn.Int(textTotal.Text);
			int invalidCCs=PIn.Int(textInvalid.Text);
			List<CreditCard> listCreditCardsPatNums=CreditCards.Refresh(creditCard.PatNum);
			gridMain.BeginUpdate();
			for(int i=gridMain.ListGridRows.Count-1;i>-1;i--) {//loop through backwards and remove any cards that are no longer in the list
				CreditCard creditCardGrid=(CreditCard)gridMain.ListGridRows[i].Tag;
				if(creditCard.PatNum!=creditCardGrid.PatNum) {
					continue;
				}
				if(!listCreditCardsPatNums.Any(x => x.CreditCardNum==creditCardGrid.CreditCardNum)) {//this row is one of the cards for the patient
					//if the card is no longer in the list of cards for the patient, it must have been deleted from FormCreditCardManage, remove from grid
					gridMain.ListGridRows.RemoveAt(i);
					_listCreditCards.RemoveAt(i);//so the list and grid contain the same number of items
					//Valid cards may have been deleted as well, but we only maintain the count of invalids changing.
					//Valid card count and total card count will be refreshed if/when the user presses the Check button again.
					totalCCs--;//update total card count
					invalidCCs--;//update invalid count
				}
			}
			gridMain.EndUpdate();
			textTotal.Text=totalCCs.ToString();
			textInvalid.Text=invalidCCs.ToString();
		}

		private void butCheck_Click(object sender,EventArgs e) {
			gridMain.BeginUpdate();
			gridMain.ListGridRows.Clear();
			gridMain.EndUpdate();
			List<CreditCardSource> listCreditCardSources=new List<CreditCardSource>();
			listCreditCardSources.Add(CreditCardSource.XServer);
			listCreditCardSources.Add(CreditCardSource.XServerPayConnect);
			_listCreditCards=CreditCards.GetCardsWithTokenBySource(listCreditCardSources);
			textTotal.Text=_listCreditCards.Count.ToString();
			textVerified.Text="0";
			textInvalid.Text="0";
			if(_listCreditCards.Count==0) {
				MsgBox.Show(this,"There are no credit cards with stored X-Charge tokens in the database.");
				return;
			}
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}