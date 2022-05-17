using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormEtransEdit : FormODBase {
		public Etrans EtransCur;
		private Etrans _etransAck;
		//private bool headingPrinted;
		private int _linesPrinted;
		private string _msgText;

		///<summary></summary>
		public FormEtransEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEtransEdit_Load(object sender,EventArgs e) {
			_msgText=EtransMessageTexts.GetMessageText(EtransCur.EtransMessageTextNum);
			textMessageText.Text=_msgText;
			textDateTimeTrans.Text=EtransCur.DateTimeTrans.ToString();
			textClaimNum.Text=EtransCur.ClaimNum.ToString();
			textBatchNumber.Text=EtransCur.BatchNumber.ToString();
			textTransSetNum.Text=EtransCur.TransSetNum.ToString();
			textAckCode.Text=EtransCur.AckCode;
			textNote.Text=EtransCur.Note;
			if(EtransCur.Etype==EtransType.ClaimSent){
				if(X12object.IsX12(_msgText)) {
					X12object x12object=new X12object(_msgText);
					if(x12object.IsFormat4010()) {
						X837_4010 x837_4010=new X837_4010(_msgText);
						checkAttachments.Checked=x837_4010.AttachmentsWereSent(EtransCur.ClaimNum);//This function does not currently work, so the corresponding checkbox is hidden on the form as well.
					}
					else if(x12object.IsFormat5010()) {
						X837_5010 x837_5010=new X837_5010(_msgText);
						checkAttachments.Checked=x837_5010.AttachmentsWereSent(EtransCur.ClaimNum);//This function does not currently work, so the corresponding checkbox is hidden on the form as well.
					}
				}
			}
			if(EtransCur.AckEtransNum>0){
				_etransAck=Etranss.GetEtrans(EtransCur.AckEtransNum);
				if(_etransAck!=null){
					textAckMessage.Text=EtransMessageTexts.GetMessageText(_etransAck.EtransMessageTextNum);
					textAckDateTime.Text=_etransAck.DateTimeTrans.ToString();
					textAckNote.Text=_etransAck.Note;
				}
			}
			else{
				_etransAck=null;
				groupAck.Visible=false;
			}
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Not Canadian.
				butPrintAck.Visible=false;
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_linesPrinted=0;
			bool isPrinted=PrinterL.TryPrintOrDebugRpPreview(pd2_PrintPage,
				Lan.g(this,"Etrans message text from")+" "+EtransCur.DateTimeTrans.ToShortDateString()+" "+Lan.g(this,"printed"),
				auditPatNum:EtransCur.PatNum,
				margins:new Margins(75,75,50,100)
			);
			if(!isPrinted){
				return;
			}
			EtransCur.Note=Lan.g(this,"Printed")+textNote.Text;
			Etranss.Update(EtransCur);
			DialogResult=DialogResult.OK;
		}

		private void pd2_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			float yPos=rectangleBounds.Top;
			SolidBrush solidBrush=new SolidBrush(Color.Black);
			using Font font=new Font(FontFamily.GenericMonospace,9);
			float heightTxt;
			RectangleF rectangleF;
			while(yPos<rectangleBounds.Bottom && _linesPrinted<textMessageText.Lines.Length){
				text=textMessageText.Lines[_linesPrinted];
				heightTxt=g.MeasureString(text,font,rectangleBounds.Width).Height;
				rectangleF=new RectangleF(rectangleBounds.X,yPos,rectangleBounds.Width,heightTxt);
				g.DrawString(text,font,solidBrush,rectangleF);
				yPos+=rectangleF.Height;
				_linesPrinted++;
				if(textMessageText.Lines[_linesPrinted-1].EndsWith("\f")) {//Page break.
					break;
				}
			}
			if(_linesPrinted<textMessageText.Lines.Length) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		///<summary>only visible if Canadian</summary>
		private void butPrintAck_Click(object sender,EventArgs e) {
			try {
				new FormCCDPrint(_etransAck,textAckMessage.Text,isPAutoPrint:false);//Show the form on screen and make the user print manually if they desire to print.
			}
			catch(Exception ex) {
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Lan.g(this,"Failed to preview acknowledgment.")+"\r\n"+ex.Message);
				msgBoxCopyPaste.ShowDialog();
			}
		}

		//private void butDelete_Click(object sender,EventArgs e) {
			//if(!MsgBox.Show(this,true,"Permanently delete the data for this transaction?  This does not alter actual claims.")){
			//	return;
			//}
			//Etranss.Delete(
		//}

		private void butOK_Click(object sender, System.EventArgs e) {
			//EtransCur.AckCode=textAckCode.Text;
			EtransCur.Note=textNote.Text;
			Etranss.Update(EtransCur);
			if(_etransAck!=null){
				_etransAck.Note=textAckNote.Text;
				Etranss.Update(_etransAck);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

		

		

		


	}
}





















