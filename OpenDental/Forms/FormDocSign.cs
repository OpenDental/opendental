/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormDocSign:FormODBase {
		//<summary></summary>
		//public bool IsNew;
		//private Patient PatCur;
		private Document DocCur;
		///<summary>This keeps the noteChanged event from erasing the signature when first loading.</summary>
		private bool IsStartingUp;
		private bool SigChanged;
		private Patient PatCur;
		private string PatFolder;
		
		///<summary></summary>
		public FormDocSign(Document docCur,Patient pat) {
			InitializeComponent();
			InitializeLayoutManager();
			DocCur=docCur;
			PatCur=pat;
			PatFolder=ImageStore.GetPatientFolder(pat,ImageStore.GetPreferredAtoZpath());
			Lan.F(this);
		}

		///<summary></summary>
		public void FormDocSign_Load(object sender, System.EventArgs e){
			IsStartingUp=true;
			textNote.Text=DocCur.Note;
			signatureBoxWrapper.SignatureMode=UI.SignatureBoxWrapper.SigMode.Document;
			string keyData=ImageStore.GetHashString(DocCur,PatFolder);
			signatureBoxWrapper.FillSignature(DocCur.SigIsTopaz,keyData,DocCur.Signature);
			IsStartingUp=false;
		}

		private void textNote_TextChanged(object sender,EventArgs e) {
			if(!IsStartingUp//so this happens only if user changes the note
				&& !SigChanged)//and the original signature is still showing.
			{
				signatureBoxWrapper.ClearSignature();
				//this will call OnSignatureChanged to set UserNum, textUser, and SigChanged
			}
		}

		private void SaveSignature() {
			if(SigChanged) {
				string keyData=ImageStore.GetHashString(DocCur,PatFolder);
				DocCur.Signature=signatureBoxWrapper.GetSignature(keyData);
				DocCur.SigIsTopaz=signatureBoxWrapper.GetSigIsTopaz();
			}
		}

		private void signatureBoxWrapper_SignatureChanged(object sender,EventArgs e) {
			SigChanged=true;
		}

		private void butOK_Click(object sender, System.EventArgs e){
			DocCur.Note=textNote.Text;
			SaveSignature();
			Documents.Update(DocCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}