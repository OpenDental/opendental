using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrHash:FormODBase {
		public FormEhrHash() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void butTransmit_Click(object sender,EventArgs e) {
			if(textHash.Text.Trim()=="" || textMessage.Text.Trim()=="") {
				MessageBox.Show("Data or hash should not be blank.");
				return;
			}
			string attachContents="Original message:\r\n"+textMessage.Text+"\r\n\r\n\r\nHash:\r\n"+textHash.Text;
			Cursor=Cursors.WaitCursor;
			try {
				EmailMessages.SendTestUnsecure("Hash","hash.txt",attachContents);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			MessageBox.Show("Sent");
		}

		private void butGenerate_Click(object sender,EventArgs e) {
			byte[] data=Encoding.ASCII.GetBytes(textMessage.Text);
			HashAlgorithm algorithm=SHA1.Create();
			byte[] hashbytes=algorithm.ComputeHash(data);
			byte digit1;
			byte digit2;
			string char1;
			string char2;
			StringBuilder strbuild=new StringBuilder();
			for(int i=0;i<hashbytes.Length;i++) {
				if(hashbytes[i]==0) {
					digit1=0;
					digit2=0;
				}
				else {
					digit1=(byte)Math.Floor((double)hashbytes[i]/16d);
					//double remainder=Math.IEEERemainder((double)hashbytes[i],16d);
					digit2=(byte)(hashbytes[i]-(byte)(16*digit1));
				}
				char1=ByteToStr(digit1);
				char2=ByteToStr(digit2);
				strbuild.Append(char1);
				strbuild.Append(char2);
			}
			//return strbuild.ToString();
			//string strHash="";
			//for(int i=0;i<hash.Length;i++) {
			//	strHash+=ByteToStr(hash[i]);
			//}
			textHash.Text=strbuild.ToString();
		}

		///<summary>The only valid input is a value between 0 and 15.  Text returned will be 1-9 or a-f.</summary>
		private static string ByteToStr(Byte byteVal) {
			//No need to check RemotingRole; no call to db.
			switch(byteVal) {
				case 10:
					return "a";
				case 11:
					return "b";
				case 12:
					return "c";
				case 13:
					return "d";
				case 14:
					return "e";
				case 15:
					return "f";
				default:
					return byteVal.ToString();
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}
