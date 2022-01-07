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
		private Etrans AckCur;
		//private bool headingPrinted;
		private int linesPrinted;
		private string MessageText;

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
			MessageText=EtransMessageTexts.GetMessageText(EtransCur.EtransMessageTextNum);
			textMessageText.Text=MessageText;
			textDateTimeTrans.Text=EtransCur.DateTimeTrans.ToString();
			textClaimNum.Text=EtransCur.ClaimNum.ToString();
			textBatchNumber.Text=EtransCur.BatchNumber.ToString();
			textTransSetNum.Text=EtransCur.TransSetNum.ToString();
			textAckCode.Text=EtransCur.AckCode;
			textNote.Text=EtransCur.Note;
			if(EtransCur.Etype==EtransType.ClaimSent){
				if(X12object.IsX12(MessageText)) {
					X12object x12obj=new X12object(MessageText);
					if(x12obj.IsFormat4010()) {
						X837_4010 x837=new X837_4010(MessageText);
						checkAttachments.Checked=x837.AttachmentsWereSent(EtransCur.ClaimNum);//This function does not currently work, so the corresponding checkbox is hidden on the form as well.
					}
					else if(x12obj.IsFormat5010()) {
						X837_5010 x837=new X837_5010(MessageText);
						checkAttachments.Checked=x837.AttachmentsWereSent(EtransCur.ClaimNum);//This function does not currently work, so the corresponding checkbox is hidden on the form as well.
					}
				}
			}
			if(EtransCur.AckEtransNum>0){
				AckCur=Etranss.GetEtrans(EtransCur.AckEtransNum);
				if(AckCur!=null){
					textAckMessage.Text=EtransMessageTexts.GetMessageText(AckCur.EtransMessageTextNum);
					textAckDateTime.Text=AckCur.DateTimeTrans.ToString();
					textAckNote.Text=AckCur.Note;
				}
			}
			else{
				AckCur=null;
				groupAck.Visible=false;
			}
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Not Canadian.
				butPrintAck.Visible=false;
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			linesPrinted=0;
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
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			float yPos=bounds.Top;
			SolidBrush brush=new SolidBrush(Color.Black);
			Font font=new Font(FontFamily.GenericMonospace,9);
			float txtH;
			RectangleF rect;
			while(yPos<bounds.Bottom && linesPrinted<textMessageText.Lines.Length){
				text=textMessageText.Lines[linesPrinted];
				txtH=g.MeasureString(text,font,bounds.Width).Height;
				rect=new RectangleF(bounds.X,yPos,bounds.Width,txtH);
				g.DrawString(text,font,brush,rect);
				yPos+=rect.Height;
				linesPrinted++;
				if(textMessageText.Lines[linesPrinted-1].EndsWith("\f")) {//Page break.
					break;
				}
			}
			if(linesPrinted<textMessageText.Lines.Length) {
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
				new FormCCDPrint(AckCur,textAckMessage.Text,false);//Show the form on screen and make the user print manually if they desire to print.
			}
			catch(Exception ex) {
				using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(Lan.g(this,"Failed to preview acknowledgment.")+"\r\n"+ex.Message);
				msgBox.ShowDialog();
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
			if(AckCur!=null){
				AckCur.Note=textAckNote.Text;
				Etranss.Update(AckCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

		

		

		


	}
}





















