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
		private Document _document;
		///<summary>This keeps the noteChanged event from erasing the signature when first loading.</summary>
		private bool _isStartingUp;
		private bool _isSignatureChanged;
		private Patient _patient;
		private string _patFolderName;
		
		///<summary></summary>
		public FormDocSign(Document document,Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			_document=document;
			_patient=patient;
			_patFolderName=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
			Lan.F(this);
		}

		///<summary></summary>
		public void FormDocSign_Load(object sender, System.EventArgs e){
			_isStartingUp=true;
			textNote.Text=_document.Note;
			signatureBoxWrapper.SignatureMode=UI.SignatureBoxWrapper.SigMode.Document;
			string keyData=ImageStore.GetHashString(_document,_patFolderName);
			signatureBoxWrapper.FillSignature(_document.SigIsTopaz,keyData,_document.Signature);
			_isStartingUp=false;
		}

		private void textNote_TextChanged(object sender,EventArgs e) {
			if(!_isStartingUp//so this happens only if user changes the note
				&& !_isSignatureChanged)//and the original signature is still showing.
			{
				signatureBoxWrapper.ClearSignature();
				//this will call OnSignatureChanged to set UserNum, textUser, and SigChanged
			}
		}

		private void SaveSignature() {
			if(_isSignatureChanged) {
				string keyData=ImageStore.GetHashString(_document,_patFolderName);
				_document.Signature=signatureBoxWrapper.GetSignature(keyData);
				_document.SigIsTopaz=signatureBoxWrapper.GetSigIsTopaz();
			}
		}

		private void signatureBoxWrapper_SignatureChanged(object sender,EventArgs e) {
			_isSignatureChanged=true;
		}

		private void butOK_Click(object sender, System.EventArgs e){
			_document.Note=textNote.Text;
			SaveSignature();
			Documents.Update(_document);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}