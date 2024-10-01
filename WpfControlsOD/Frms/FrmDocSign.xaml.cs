using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
///<summary></summary>
	public partial class FrmDocSign:FrmODBase {
		//<summary></summary>
		//public bool IsNew;
		//private Patient PatCur;
		private Document _document;
		///<summary>This keeps the noteChanged event from erasing the signature when first loading.</summary>
		private bool _isStartingUp;
		private bool _isSignatureChanged;
		private Patient _patient;
		private string _patFolderName;
		///<summary>In screeen coords. LL because it's 'anchored' LL and it will grow up and right.</summary>
		public System.Drawing.Point PointLLStart;
		
		///<summary></summary>
		public FrmDocSign(Document document,Patient patient) {
			InitializeComponent();
			_document=document;
			_patient=patient;
			Load+=FrmDocSign_Load;
			textNote.TextChanged+=textNote_TextChanged;
			signatureBoxWrapper.SignatureChanged+=signatureBoxWrapper_SignatureChanged;
			PreviewKeyDown+=FrmDocSign_PreviewKeyDown;
		}

		///<summary></summary>
		public void FrmDocSign_Load(object sender, EventArgs e){
			_formFrame.Location=new System.Drawing.Point(PointLLStart.X,PointLLStart.Y-_formFrame.Height+4);
			Lang.F(this);
			_patFolderName=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());
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

		private void FrmDocSign_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, System.EventArgs e){
			_document.Note=textNote.Text;
			SaveSignature();
			Documents.Update(_document);
			IsDialogOK=true;
		}

	}
}