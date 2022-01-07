using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Net.Mail;
using OpenDentBusiness;
using System.Net;

namespace OpenDental {
	public partial class FormEhrEncryption:FormODBase {
		private byte[] key;

		public FormEhrEncryption() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormEncryption_Load(object sender,EventArgs e) {
			UTF8Encoding enc=new UTF8Encoding();
			key=enc.GetBytes("AKQjlLUjlcABVbqp");//Random string will be key
		}

		private void butEncrypt_Click(object sender,EventArgs e) {
			if(textInput.Text.Trim()==string.Empty) {
				MessageBox.Show("No text to encrypt.");
				return;
			}
			textResult.Text=Encryption(textInput.Text);
		}

		private void butDecrypt_Click(object sender,EventArgs e) {
			if(textInput.Text.Trim()==string.Empty) {
				MessageBox.Show("No text to decrypt.");
				return;
			}
			textResult.Text=Decryption();
		}

		///<summary>Encrypts signature text and returns a base 64 string so that it can go directly into the database.</summary>
		public string Encryption(string encrypt){
			byte[] ecryptBytes=Encoding.UTF8.GetBytes(encrypt);
			MemoryStream ms=new MemoryStream();
			CryptoStream cs=null;
			Aes aes=new AesCryptoServiceProvider();
			aes.Key=key;
			aes.IV=new byte[16];
			ICryptoTransform encryptor=aes.CreateEncryptor(aes.Key,aes.IV);
			cs=new CryptoStream(ms,encryptor,CryptoStreamMode.Write);
			cs.Write(ecryptBytes,0,ecryptBytes.Length);
			cs.FlushFinalBlock();
			byte[] encryptedBytes=new byte[ms.Length];
			ms.Position=0;
			ms.Read(encryptedBytes,0,(int)ms.Length);
			cs.Dispose();
			ms.Dispose();
			if(aes!=null) {
				aes.Clear();
			}
			return Convert.ToBase64String(encryptedBytes);			
		}

		public string Decryption() {
			try {
				byte[] encrypted=Convert.FromBase64String(textInput.Text);
				MemoryStream ms=null;
				CryptoStream cs=null;
				StreamReader sr=null;
				Aes aes=new AesCryptoServiceProvider();
				aes.Key=key;
				aes.IV=new byte[16];
				ICryptoTransform decryptor=aes.CreateDecryptor(aes.Key,aes.IV);
				ms=new MemoryStream(encrypted);
				cs=new CryptoStream(ms,decryptor,CryptoStreamMode.Read);
				sr=new StreamReader(cs);
				string decrypted=sr.ReadToEnd();
				ms.Dispose();
				cs.Dispose();
				sr.Dispose();
				if(aes!=null) {
					aes.Clear();
				}
				return decrypted;
			}
			catch { 
				MessageBox.Show("Text entered was not valid encrypted text.");
				return"";
			}
		}

		///<summary></summary>
		private string GenerateHash(string message) {
			byte[] data=Encoding.ASCII.GetBytes(message);
			HashAlgorithm algorithm=SHA1.Create();
			byte[] hashbytes=algorithm.ComputeHash(data);
			byte digit1;
			byte digit2;
			string char1;
			string char2;
			StringBuilder strHash=new StringBuilder();
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
				strHash.Append(char1);
				strHash.Append(char2);
			}
			return strHash.ToString();
		}

		///<summary>The only valid input is a value between 0 and 15.  Text returned will be 1-9 or a-f.</summary>
		private string ByteToStr(Byte byteVal) {
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

		private void butTransmit_Click(object sender,EventArgs e) {
			if(textInput.Text.Trim()==string.Empty) {
				MessageBox.Show("No input text to send.");
				return;
			}
			//Encrypt the message.
			string encryptedMessage=Encryption(textInput.Text);
			//Generate a hash for the message.
			string hashedMessage=GenerateHash(textInput.Text);
			//Encrypt the hash.
			//string encryptedHash=Encryption(hashedMessage);
			string attachContents="Encrypted message:\r\n"+encryptedMessage+"\r\n\r\nHash:\r\n"+hashedMessage;
			//Send the encrypted message followed by encrypted hash.
			Cursor=Cursors.WaitCursor;
			try {
				EmailMessages.SendTestUnsecure("Encryption","encryption.txt",attachContents);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			MessageBox.Show("Sent");
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
