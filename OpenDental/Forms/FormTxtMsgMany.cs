using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTxtMsgMany:FormODBase {

		private List<PatComm> _listPatComms;
		private long _clinicNum;
		private SmsMessageSource _smsMessageSource;
		///<summary>If true, patients with the same number will be combined into one message.</summary>
		public bool DoCombineNumbers;
		
		public FormTxtMsgMany(List<PatComm> listPatComms,string textMessageText,long clinicNum,SmsMessageSource smsMessageSource) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listPatComms=listPatComms;
			textMessage.Text=textMessageText;
			_clinicNum=clinicNum;
			_smsMessageSource=smsMessageSource;
		}

		private void FormTxtMsgMany_Load(object sender,EventArgs e) {
			FillGrid();
			SetFilterControlsAndAction(() => SetMessageCounts(),0,textMessage);
		}

		private void SetMessageCounts() {
			textCharCount.Text=textMessage.TextLength.ToString();
			textMsgCountPerPatient.Text=SmsPhones.CalculateMessagePartsNumber(textMessage.Text).ToString();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Phone Number"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient"),200);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			//Another possible way to do it. Completely untested:
			//if(DoCombineNumbers) {
			//	_listPatComms = _listPatComms.OrderBy(x => x.WirelessPhone).ToList();
			//}
			////only need a new list when combining phone numbers
			//List<PatComm> patCommGroup = new List<PatComm>();
			//string currentPhone="";
			//for(int i = 0;i<_listPatComms.Count;i++) {
			//	currentPhone=_listPatComms[i].WirelessPhone;
			//	patCommGroup.Add(_listPatComms[i]);
			//	if(DoCombineNumbers
			//		&& i<_listPatComms.Count-1
			//		&& _listPatComms[i+1].WirelessPhone == currentPhone) {
			//		continue;
			//	}
			//	GridRow row=new GridRow();
			//	row.Cells.Add(patCommGroup.First().WirelessPhone);
			//	row.Cells.Add(string.Join("\r\n",patCommGroup.Select(x => x.LName+", "+x.FName))); // add cell with all names in it
			//	row.Tag=patCommGroup;
			//	gridMain.ListGridRows.Add(row);
			//	patCommGroup.Clear();
			//}
			if(DoCombineNumbers) {
				// Get list of phone numbers
				List<string> listWirelessPhones = _listPatComms.Select(x => x.WirelessPhone).Distinct().ToList();
				for(int i = 0;i<listWirelessPhones.Count;i++) {
					//get list of all PatComms for this phone number
					List<PatComm> listPatComms = _listPatComms.FindAll(x => x.WirelessPhone==listWirelessPhones[i]);
					GridRow row=new GridRow();
					row.Cells.Add(listWirelessPhones[i]); 
					row.Cells.Add(string.Join("\r\n",listPatComms.Select(x => x.LName+", "+x.FName))); // Add all names to this cell
					row.Tag=listPatComms.ToList(); // This is a list of PatComms for a single phone number
					gridMain.ListGridRows.Add(row);
				}
				gridMain.EndUpdate();
				return;
			}
			// Get list of PatNums
			List<long> listPatNums = _listPatComms.Select(x => x.PatNum).Distinct().ToList();
			for(int i = 0;i<listPatNums.Count;i++) {
				//get list of all PatComms for this PatNum (should be unique, but just in case)
				List<PatComm> listPatComms = _listPatComms.FindAll(x => x.PatNum==listPatNums[i]);
				GridRow row=new GridRow();
				row.Cells.Add(listPatComms[0].WirelessPhone); 
				//==Jordan Since we're grouping by PatNum, if there are 2 patComms with same patNum,
				//the next row will duplicate the name. I'm assuming that this won't happen because
				//there are no duplicates. But this could probably use some improvement.
				row.Cells.Add(string.Join("\r\n",listPatComms.Select(x => x.LName+", "+x.FName))); 
				row.Tag=listPatComms.ToList();  
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		///<summary>Sends a text message to this patient if it is feasible.</summary>
		private bool SendText(PatComm patComm,long clinicNum,string message) {	
			if(!patComm.IsSmsAnOption)	{
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"It is not OK to text patient")+" "+patComm.FName+" "+patComm.LName+".");
				Cursor=Cursors.WaitCursor;
				return false;
			}
			SmsToMobiles.SendSmsSingle(patComm.PatNum,patComm.SmsPhone,message,clinicNum,_smsMessageSource,true,Security.CurUser);
			return true;
		}

		private void butSend_Click(object sender,EventArgs e) {
			if(!SmsPhones.IsIntegratedTextingEnabled()) {
				MsgBox.Show(this,"Integrated Texting has not been enabled.");
				return;
			}
			if(textMessage.Text=="") {
				MsgBox.Show(this,"Please enter a message first.");
				return;
			}
			if(textMessage.Text.ToLower().Contains("[date]") || textMessage.Text.ToLower().Contains("[time]")) {
				MsgBox.Show(this,"Please replace or remove the [Date] and [Time] tags.");
				return;
			}
			if(PrefC.HasClinicsEnabled && !Clinics.IsTextingEnabled(_clinicNum)) { //Checking for specific clinic.
				if(_clinicNum!=0) {
					MessageBox.Show(Lans.g(this,"Integrated Texting has not been enabled for the following clinic")+":\r\n"+Clinics.GetClinic(_clinicNum).Description+".");
					return;
				}
				//Should never happen. This message is precautionary.
				MsgBox.Show(this,"The default texting clinic has not been set.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			int numTextsSent=0;
			List<List<PatComm>> listListsPatComms = gridMain.ListGridRows.Select(x => x.Tag).Cast<List<PatComm>>().ToList();
			for(int i=0;i<listListsPatComms.Count();i++) {
				//Use the guarantor if in the list, otherwise use the first name alphabetically.
				PatComm patComm = listListsPatComms[i].OrderByDescending(x => x.PatNum==x.Guarantor).ThenBy(x => x.FName).First();
				string textMsgText=textMessage.Text.Replace("[NameF]",patComm.FName);
				try {
					if(SendText(patComm,_clinicNum,textMsgText)) {
						numTextsSent++;
					}
				}
				catch(ODException odex) {
					Cursor=Cursors.Default;
					string errorMsg=Lan.g(this,"There was an error sending to")+" "+listListsPatComms[i].First().WirelessPhone+". "
						+odex.Message+" "
						+Lan.g(this,"Do you want to continue sending messages?");
					if(MessageBox.Show(errorMsg,"",MessageBoxButtons.YesNo)==DialogResult.No) {
						break;
					}
					Cursor=Cursors.WaitCursor;
				}
				catch(Exception ex) {
					ex.DoNothing();
					Cursor=Cursors.Default;
					string errorMsg=Lan.g(this,"There was an error sending to")+" "+listListsPatComms[i].First().WirelessPhone+". "
						+Lan.g(this,"Do you want to continue sending messages?");
					if(MessageBox.Show(errorMsg,"",MessageBoxButtons.YesNo)==DialogResult.No) {
						break;
					}
					Cursor=Cursors.WaitCursor;
				}
			}
			Cursor=Cursors.Default;
			MessageBox.Show(numTextsSent+" "+Lan.g(this,"texts sent successfully."));
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}
	}
}