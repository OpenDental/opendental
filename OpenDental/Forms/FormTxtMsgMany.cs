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
		private SmsMessageSource _messageSource;
		///<summary>If true, patients with the same number will be combined into one message.</summary>
		public bool DoCombineNumbers;
		
		public FormTxtMsgMany(List<PatComm> listPatComms,string textMessageText,long clinicNum,SmsMessageSource messageSource) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listPatComms=listPatComms;
			textMessage.Text=textMessageText;
			_clinicNum=clinicNum;
			_messageSource=messageSource;
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
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Phone Number"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient"),200);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			foreach(IGrouping<string,PatComm> patCommGroup in _listPatComms.GroupBy(x => DoCombineNumbers ? x.WirelessPhone : x.PatNum.ToString())) {
				GridRow row=new GridRow();
				row.Cells.Add(patCommGroup.First().WirelessPhone);
				row.Cells.Add(string.Join("\r\n",patCommGroup.Select(x => x.LName+", "+x.FName)));
				row.Tag=patCommGroup.ToList();
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
			SmsToMobiles.SendSmsSingle(patComm.PatNum,patComm.SmsPhone,message,clinicNum,_messageSource,true,Security.CurUser);
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
				}
				else {
					//Should never happen. This message is precautionary.
					MsgBox.Show(this,"The default texting clinic has not been set.");
				}
				return;
			}
			Cursor=Cursors.WaitCursor;
			int numTextsSent=0;
			foreach(List<PatComm> listPatComms in gridMain.ListGridRows.Select(x => x.Tag).Cast<List<PatComm>>()) {
				try {
					//Use the guarantor if in the list, otherwise use the first name alphabetically.
					PatComm patComm=listPatComms.OrderByDescending(x => x.PatNum==x.Guarantor).ThenBy(x => x.FName).First();
					string textMsgText=textMessage.Text.Replace("[NameF]",patComm.FName);
					if(SendText(patComm,_clinicNum,textMsgText)) {
						numTextsSent++;
					}
				}
				catch(ODException odex) {
					Cursor=Cursors.Default;
					string errorMsg=Lan.g(this,"There was an error sending to")+" "+listPatComms.First().WirelessPhone+". "
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
					string errorMsg=Lan.g(this,"There was an error sending to")+" "+listPatComms.First().WirelessPhone+". "
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