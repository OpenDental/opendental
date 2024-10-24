using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.UI {
/*
Jordan is the only one allowed to edit this file.

How signature encryption works:
	A signature starts as a collection of straight line segments that look like curved lines when chained together.
	The OD signature format looks like this: 45,68;48,70;49,72;0,0;55,88;etc.  It's simply a sequence of points, separated by semicolons.  0,0 represents pen up.
	We then need to encrypt this string with symmetric encryption using a key of some sort.
	The key that we use for this symmetric encryption is a 16 char hash of the data that the signature is tied to.
	For example, we might want to tie the signature to "This is a note", PriKeyNum=123, and ProvNum=7. 
	Our key data then looks like this: "This is a note1237". 
	Once we pick a signature data strategy, we cannot ever change it for that particular table, although we can for other tables.
	The hash might then look like {4A 6F 68 6E 44 6F 65 44 6B 9A A6 27 D6 9E 61 6C}. We never see this. It's just stored in memory as 16 bytes while we manipulate the sig.
	The hash is our symmetric key. The signature string gets encrypted and stored in the db.
	To get the signature back out of the database as vectors, we must have the same hash.
	We must recreate the hash at that time from the original key data.
	If we can't unencrypt the signature, we can't draw it and must show "invalid sig" or something like that.
	This strategy hides the sig from prying eyes in the db, and it also proves that the data has not changed since signing.

Topaz hardware vs our SignatureBox:
	Topaz is the 'tablet' hardware that a patient signs on with a stylus.
	They also provide a SigPlusNET control that we can place on the screen to show a signature that was signed on their hardware.
	We use their dll to perform the encryption and we don't get to see what format they use. Their .sig format is proprietary.
	https://www.topazsystems.com/software/download/sigplusnet.pdf
	We also have our own SignatureBox control built by us. Our strategy is the one discussed above.
	KeyString and AutoKeyData are terms that originated with Topaz and which we have adopted for use in our SignatureBox.
	KeyString: This is the hash of the key data. We use 16 bytes. Topaz likes to create their own, so we don't generally use this with Topaz.
	AutoKeyData: This is the unhashed key data and can be quite long.
	The idea is that we should use one or the other, depending on if we had already hashed the key data.
	The way we know if a signature in the db is ours or Topaz, we typically have another db column along with Signature called SigIsTopaz.
	Sometimes, like in Sheets, the first char is 1 or 0 to indicate Topaz or not. The remainder of the signature is the actual signature.
	The OD sigs are not compressed, and the Topaz sigs are.  But there is very little difference in their sizes.  It would be very rare for a signature to be larger than 1000 bytes.

Control nesting
	This control is SignatureBoxWrapper.cs. It is a wrapper with two controls inside of it:
	1. Topaz SigPlusNET control
	2. SignatureBox control, our own internal control.
	For WPF, this control is wrapped in with a WindowsFormsHost container because Topaz does not have a WPF version, and wrapping is what they recommend.
	This SignatureBoxWrapper also contains two buttons that we need. The buttons are ugly and need an overhaul.

Size
	The control was designed originally to always be exactly 362x79
	It can be set larger if proportional and if scale and zoom match.
	In other words, it's not yet designed to be set to arbitrary sizes other than as described above.

Historical changes:
	There have been a series of changes and bugs over the years, both from us and from Topaz. Examples:
		We had a bug for a while where we were "encrypting" sigs with 16 zeros.
		There have been newline bugs due to RichTextBox and MiddleTier issues.
		Around 2010, Topaz added an enumeration value in the middle of an enumeration, breaking our code.
		Our internal SignatureBox sometimes uses the entire key string as the symmetric key instead of a hashed 16 byte version of the key string.
	Each time we have a bug or a variation on the original strategy, we must forever be able to handle that unique situation without breaking others.
	This leads to many "if" statements. We end up trying a chain of decryptions until we get one that works.
	At some point in the future, we may decide to re-encrypt all of the signatures with one single known good strategy.
	But because Topaz is still a black box, we can't ever really completely fix the complexity.

Guideline for usage:

In the load event handler or similar:
	string keyData=str1+str2+long1.ToString(), etc.
	signatureBoxWrapper.FillSignature(someTable.SigIsTopaz,keyData,someTable.Signature);

In the save click:
	if(sigBoxWrapper.GetSigChanged()){
		string keyData=Procedures.GetSignatureKeyData(_procedure, textNotes.Text);
		someTable.Signature=signatureBoxWrapper.GetSignature(keyData);
		someTable.SigIsTopaz=signatureBoxWrapper.GetSigIsTopaz();
	}

In FormClosing:
	signatureBoxWrapper?.SetTabletState(0);
*/
	///<summary>See the notes at the top of this file. Must always be exactly 362x79.</summary>
	public partial class SignatureBoxWrapper:UserControl {
		///<summary>Default 1. Typical value 1.5</summary>
		private float _scaleMS = 1;
		///<summary>Default 1. Typical value 1.2</summary>
		private float _zoomLocal=1;
		///<summary>This is set when someone has started drawing a signature. When true, this will be used to stop people from accidentally 
		///drawing on the screen and invalidating their signature on FormSheetFillEdit.</summary>
		public bool IsSigStarted;
		public bool SigChanged;
		//private bool allowTopaz;
		private Control sigBoxTopaz;
		///<summary>The reason for this event is so that if a different user is signing, that it properly records the change in users.  See the example pattern in FormProcGroup.</summary>
		[Category("Action"),Description("Event raised when signature is cleared or altered.")]
		public event EventHandler SignatureChanged=null;
		///<summary>Event raised when the X button is clicked.</summary>
		[Category("Action"),Description("Event raised when the X button is clicked.")]
		public event EventHandler ClearSignatureClicked=null;
		///<summary>Event raised when the Sign Topaz button is clicked.</summary>
		[Category("Action"),Description("Event raised when the Sign Topaz button is clicked.")]
		public event EventHandler SignTopazClicked=null;
		///<summary>Used for special cases where signature logic varies from our default.</summary>
		private SigMode _signatureMode=SigMode.Default;

		[Category("Property"),Description("Set the text that shows in the invalid signature label"),DefaultValue("Invalid Signature")]
		///<summary>Usually "Invalid Signature", but this can be changed for different situations.</summary>
		public string LabelText {
			get {
				return labelInvalidSig.Text;
			}
			set {
				labelInvalidSig.Text=value;
			}
		}

		protected override void OnLayout(LayoutEventArgs e){
			base.OnLayout(e);
			butESign.Size=new Size(ScaleI(20),ScaleI(20));
			butTopazSign.Size=new Size(ScaleI(20),ScaleI(20));
			butClearSig.Size=new Size(ScaleI(20),ScaleI(20));
			butClearSig.Location=new Point(Width-ScaleI(20),0);
			butTopazSign.Location=new Point(Width-ScaleI(40),0);
			signatureBox.Width=Width-2;
			signatureBox.Height=Height-2;
			if(sigBoxTopaz!=null){
				//sigBoxTopaz just shows an image that user signed on the hardware. There is no pen input.
				//In testing, the image just scales to fit the size of the control.
				//Copy the size and location that were just set above for our sigbox.
				sigBoxTopaz.Location=signatureBox.Location;
				sigBoxTopaz.Size=signatureBox.Size;//sigBoxTopaz needs to be the same as or smaller than its container so the signature is not cut off.
			}
		}

		///<summary>Converts from 96dpi to current total scale.</summary>
		private float ScaleF(float val96){
			float scaleTotal=_scaleMS*_zoomLocal;//example 1.5*1.2
			return val96*scaleTotal;
		}

		///<summary>Converts from 96dpi to current total scale.</summary>
		private int ScaleI(float val96){
			float scaleTotal=_scaleMS*_zoomLocal;//example 1.5*1.2
			return (int)(Math.Round(val96*scaleTotal));
		}

		///<summary>Used for special cases where signature logic varies from our default.</summary>
		public SigMode SignatureMode {
			get {
				return _signatureMode;
			}
			set {
				_signatureMode=value;
			}
		}

		public SignatureBoxWrapper() {
			InitializeComponent();
			signatureBox.SetTabletState(1);
			try {
				InitializeTopaz();
			}
			catch(Exception ex) {
				ex.DoNothing();//Most offices don't use Topaz so we won't let the user know yet that it didn't initialize.
			}
			butTopazSign.BringToFront();
			butClearSig.BringToFront();
		}

		private void InitializeTopaz() {
			//Add signature box for Topaz signatures.
			sigBoxTopaz=TopazWrapper.GetTopaz();
			sigBoxTopaz.Location=signatureBox.Location;//this puts both boxes in the same spot.
			sigBoxTopaz.Name="sigBoxTopaz";
			sigBoxTopaz.Size=signatureBox.Size;//See comments in OnLayout() above.
			signatureBox.Anchor=(AnchorStyles)(AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
			sigBoxTopaz.TabIndex=92;
			sigBoxTopaz.Text="sigPlusNET1";
			sigBoxTopaz.Visible=false;
			sigBoxTopaz.Leave+=new EventHandler(sigBoxTopaz_Leave);
			Controls.Add(sigBoxTopaz);
		}

		///<summary>Returns true if Topaz is initialized. If it is not initialized, it will attempt to do so.</summary>
		public bool CheckTopaz() {
			if(sigBoxTopaz==null) {
				try {
					InitializeTopaz();
				}
				catch(Exception ex) {
					//FriendlyException.Show(Lans.g(this,"Unable to initialize Topaz."),ex);
					return false;
				}
			}
			return true;
		}

		public void SetAllowDigitalSig(bool allowDigitalSig,bool doSkipProcSignaturePref=false) {
			if(doSkipProcSignaturePref) {
				butESign.Visible=allowDigitalSig;
			}
			else {
				butESign.Visible=allowDigitalSig && PrefC.GetBool(PrefName.SignatureAllowDigital);
			}
		}

		///<summary>Defaults are 1,1.</summary>
		public void SetScaleAndZoom(float scaleMS,float zoomLocal){
			if(scaleMS==_scaleMS && zoomLocal==_zoomLocal){
				return;
			}
			_scaleMS = scaleMS;
			_zoomLocal=zoomLocal;
			signatureBox.SetScaleAndZoom(scaleMS,zoomLocal);
		}


		protected void OnSignatureChanged() {
			SigChanged=true;
			if(SignatureChanged!=null){
				SignatureChanged(this,new EventArgs());
			}
		}

		public void FillSignature(bool sigIsTopaz,string keyData,string signature) {
			if(signature==null || signature=="") {
				return;
			}
			//This does 3 checks for both topaz and normal signatures.  These keyData replacements are due to MiddleTier & RichTextBox newline issues.
			//Most cases will not get past check 1.  Some of the following checks are taken care of by things like POut.StringNote().
			//Check 1:  keyData.Replace("\r\n","\n").  This reverts any changes middle tier made to the keydata.  
			//					Middle tier converts "\r\n" -> "\n" -> "\r\n" so we change it back to "normal".
			//Check 2:  Normal keydata.  This is for cases that had "\r\n" as the original note.  These are keydata that were captured with a textbox.
			//Check 3:  keyData.Replace("\r\n","\n").Replace("\n","\r\n").  This is for cases where the original note was captured with a textbox, and then
			//					was filled into an ODTextBox(richtextbox) and the "\r\n" was changed to "\n" then the user clicked ok changing the note.
			//Note that if the keydata was hashed before this point, these replacements have to be dealt with outside of this function.
			if(sigIsTopaz) {
				if(!CheckTopaz()) {
					labelInvalidSig.Visible=true;
					return;
				}
				FillSignatureHelperTopaz(keyData.Replace("\r\n","\n"),signature);
				if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0) {
					FillSignatureHelperTopaz(keyData,signature);
				}
				if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0) {
					FillSignatureHelperTopaz(keyData.Replace("\r\n","\n").Replace("\n","\r\n"),signature);
				}
			}
			else {
				FillSignatureHelper(keyData.Replace("\r\n","\n"),signature);
				if(signatureBox.NumberOfTabletPoints()==0) {
					FillSignatureHelper(keyData,signature);
				}
				if(signatureBox.NumberOfTabletPoints()==0) {
					FillSignatureHelper(keyData.Replace("\r\n","\n").Replace("\n","\r\n"),signature);
				}
			}
		}

		private void FillSignatureHelperTopaz(string keyData,string signature) {
			if(!CheckTopaz()) {
				return;
			}
			//According to Jeff Robertson from Topaz, the method SetKeyString() should be used to clear out the key string before loading a signature. 
			//For that reason, it should only be called with 16 zeros as its arguments. The actual data that the signature will be bound to should be passed 
			//to the method SetAutoKeyData().
			signatureBox.Visible=false;
			sigBoxTopaz.Visible=true;
			labelInvalidSig.Visible=false;
			TopazWrapper.ClearTopaz(sigBoxTopaz);
			TopazWrapper.SetTopazCompressionMode(sigBoxTopaz,0);
			TopazWrapper.SetTopazEncryptionMode(sigBoxTopaz,0);
			TopazWrapper.SetTopazKeyString(sigBoxTopaz,"0000000000000000");//Clear out the key string
			TopazWrapper.SetTopazEncryptionMode(sigBoxTopaz,2);//high encryption
			TopazWrapper.SetTopazCompressionMode(sigBoxTopaz,2);//high compression
			string hashedKeyData;
			switch(_signatureMode) {
				case SigMode.TreatPlan:
					hashedKeyData=TreatPlans.GetHashStringForSignature(keyData);//Passed in key data has not been hashed yet.
					TopazWrapper.SetTopazAutoKeyData(sigBoxTopaz,hashedKeyData);
					break;
				case SigMode.OrthoChart:
					hashedKeyData=OrthoCharts.GetHashStringForSignature(keyData);//Passed in key data has not been hashed yet.
					TopazWrapper.SetTopazAutoKeyData(sigBoxTopaz,hashedKeyData);
					break;
				case SigMode.Document:
				case SigMode.Default:
				default:
					TopazWrapper.SetTopazAutoKeyData(sigBoxTopaz,keyData);
					break;
			}
			TopazWrapper.SetTopazSigString(sigBoxTopaz,signature);
			if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0) {
				//Try reading in the signature using the old way that we used to handle signatures.
				FillSignatureTopazOld(keyData,signature);
			}
			if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0) {
				//Try reading in the signature using the ANSI paradigm.
				TopazWrapper.FillSignatureANSI(sigBoxTopaz,keyData,signature,_signatureMode);
			}
			if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0) {
				//Try reading in the signature using different encodings for keyData.
				TopazWrapper.FillSignatureEncodings(sigBoxTopaz,keyData,signature,_signatureMode);
			}
			//If sig still not showing it must be invalid.
			if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0) {
				labelInvalidSig.Visible=true;
			}
			TopazWrapper.SetTopazState(sigBoxTopaz,0);
		}

		///<summary>Some places used different logic for keystring and autokeydata in the past.  This must be maintained to keep signatures valid.
		///</summary>
		private void FillSignatureTopazOld(string keyData,string signature) {
			if(!CheckTopaz()) {
				return;
			}
			string hashedKeyData;
			switch(_signatureMode) {
				case SigMode.Document:
					TopazWrapper.SetTopazKeyString(sigBoxTopaz,keyData);
					TopazWrapper.SetTopazEncryptionMode(sigBoxTopaz,2);//high encryption
					TopazWrapper.SetTopazCompressionMode(sigBoxTopaz,2);//high compression
					TopazWrapper.SetTopazSigString(sigBoxTopaz,signature);
					break;
				case SigMode.TreatPlan:
					hashedKeyData=OpenDentBusiness.TreatPlans.GetHashStringForSignature(keyData);//Passed in KeyData still needs to be hashed.
					TopazWrapper.SetTopazKeyString(sigBoxTopaz,hashedKeyData);
					TopazWrapper.SetTopazEncryptionMode(sigBoxTopaz,2);//high encryption
					TopazWrapper.SetTopazCompressionMode(sigBoxTopaz,2);//high encryption
					TopazWrapper.SetTopazSigString(sigBoxTopaz,signature);
					sigBoxTopaz.Refresh();
					//If sig is not showing, then try encryption mode 3 for signatures signed with old SigPlusNet.dll.
					if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0) {
						TopazWrapper.SetTopazEncryptionMode(sigBoxTopaz,3);//Unknown mode (told to use via TopazSystems)
						TopazWrapper.SetTopazSigString(sigBoxTopaz,signature);
					}
					break;
				case SigMode.OrthoChart:
					hashedKeyData=OpenDentBusiness.OrthoCharts.GetHashStringForSignature(keyData);//Passed in KeyData still needs to be hashed.
					TopazWrapper.SetTopazKeyString(sigBoxTopaz,hashedKeyData);
					TopazWrapper.SetTopazEncryptionMode(sigBoxTopaz,2);//high encryption
					TopazWrapper.SetTopazCompressionMode(sigBoxTopaz,2);//high compression
					TopazWrapper.SetTopazSigString(sigBoxTopaz,signature);
					//older items may have been signed with zeros due to a bug.  We still want to show the sig in that case.
					//but if a sig is not showing, then set the key string to try to get it to show.
					if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0) {
						TopazWrapper.SetTopazAutoKeyData(sigBoxTopaz,hashedKeyData);
						TopazWrapper.SetTopazSigString(sigBoxTopaz,signature);
					}
					//If sig is not showing, then try encryption mode 3 for signatures signed with old SigPlusNet.dll.
					if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0) {
						TopazWrapper.SetTopazEncryptionMode(sigBoxTopaz,3);//Unknown mode (told to use via TopazSystems)
						TopazWrapper.SetTopazSigString(sigBoxTopaz,signature);
					}
					break;
				case SigMode.Default:
					TopazWrapper.SetTopazKeyString(sigBoxTopaz,"0000000000000000");
					TopazWrapper.SetTopazEncryptionMode(sigBoxTopaz,2);//high encryption
					TopazWrapper.SetTopazCompressionMode(sigBoxTopaz,2);//high compression
					TopazWrapper.SetTopazSigString(sigBoxTopaz,signature);
					//older items may have been signed with zeros due to a bug.  We still want to show the sig in that case.
					//but if a sig is not showing, then set the key string to try to get it to show.
					if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0) {
						TopazWrapper.SetTopazAutoKeyData(sigBoxTopaz,keyData);
						TopazWrapper.SetTopazSigString(sigBoxTopaz,signature);
					}
					//If sig is not showing, then try encryption mode 3 for signatures signed with old SigPlusNet.dll.
					if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0) {
						TopazWrapper.SetTopazEncryptionMode(sigBoxTopaz,3);//Unknown mode (told to use via TopazSystems)
						TopazWrapper.SetTopazSigString(sigBoxTopaz,signature);
					}
					break;
			}
		}

		private void FillSignatureHelper(string keyData,string signature) {
			signatureBox.Visible=true;
			if(sigBoxTopaz!=null) {
				sigBoxTopaz.Visible=false;
			}
			labelInvalidSig.Visible=false;
			signatureBox.ClearTablet();
			//sigBox.SetSigCompressionMode(0);
			//sigBox.SetEncryptionMode(0);
			//Some places used different logic for keystring and autokeydata in the past.  This must be maintained to keep signatures valid.
			switch(_signatureMode) {
				case SigMode.Document:
					//No auto key data set, only key string.
					signatureBox.SetKeyString(keyData);
					break;
				case SigMode.TreatPlan:
					string hashedKeyData=OpenDentBusiness.TreatPlans.GetHashStringForSignature(keyData);//Passed in KeyData still needs to be hashed.
					//No auto key data set, only key string.
					signatureBox.SetKeyString(hashedKeyData);
					break;
				case SigMode.OrthoChart:
					hashedKeyData=OpenDentBusiness.OrthoCharts.GetHashStringForSignature(keyData);//Passed in KeyData still needs to be hashed.
					//No auto key data set, only key string.
					signatureBox.SetKeyString(hashedKeyData);
					break;
				case SigMode.Default:
					signatureBox.SetKeyString("0000000000000000");
					signatureBox.SetAutoKeyData(keyData);
					break;
			}
			//sigBox.SetEncryptionMode(2);//high encryption
			//sigBox.SetSigCompressionMode(2);//high compression
			signatureBox.SetSigString(signature);
			if(signatureBox.IsDigitalSignature()) {
				labelInvalidSig.Visible=false;
			}
			else if(signatureBox.NumberOfTabletPoints()==0) {  //Both signature checks were invalid.
				labelInvalidSig.Visible=true;
			}
			signatureBox.SetTabletState(0);//not accepting input.  To accept input, change the note, or clear the sig.
		}

		///<summary>Helper method to manipulate the visibility of all buttons on the signature wrapper.</summary>
		public void SetButtonVisibility(bool isVisible) {
			butClearSig.Visible=isVisible;
			butTopazSign.Visible=isVisible;
			butESign.Visible=isVisible;
		}

		public int GetNumberOfTabletPoints(bool sigIsTopaz) {
			if(sigIsTopaz) {
				if(!CheckTopaz()) {
					return 0;
				}
				return TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz);
			}
			return signatureBox.NumberOfTabletPoints();
		}

		///<summary>This can be used to determine whether the signature has changed since the control was created.  It is, however, preferrable to have the parent form use the SignatureChanged event to track changes.</summary>
		public bool GetSigChanged(){
			return SigChanged;
		}

		///<summary>This should NOT be used unless GetSigChanged returns true.</summary>
		public bool GetSigIsTopaz(){
			//if(allowTopaz && sigBoxTopaz.Visible){
			if(sigBoxTopaz!=null && sigBoxTopaz.Visible){
				return true;
			}
			return false;
		}

		public bool GetIsTypedFromWebForms() {
			string pointString=signatureBox.GetPointString();
			return pointString.Equals("{X=1,Y=1};{X=15,Y=15};{X=0,Y=0};{X=1,Y=15};{X=15,Y=1}"); //This is the exact point string we expect to get from a digital signature in webforms
		}

		/*
		///<summary></summary>
		public bool GetSigIsValid(){
			if(labelInvalidSig.Visible){
				return false;
			}
			return true;
		}*/

		///<summary>This should happen a lot before the box is signed.  Once it's signed, if this happens, then the signature will be invalidated.  The user would have to clear the invalidation manually.  This "invalidation" is just a visual cue; nothing actually happens to the data.</summary>
		public void SetInvalid(){
			if(sigBoxTopaz!=null && sigBoxTopaz.Visible){
				if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0){
					return;//no need to do anything because no signature
				}
			}
			else{
				if(signatureBox.NumberOfTabletPoints()==0) {
					return;//no need to do anything because no signature
				}
			}
			labelInvalidSig.Visible=true;
			labelInvalidSig.BringToFront();
		}

		public bool IsValid{
			get { return (!labelInvalidSig.Visible); }
		}

		public bool SigIsBlank{
			get{ 
				if(sigBoxTopaz!=null && sigBoxTopaz.Visible){
					return(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0);
				}
				return(signatureBox.NumberOfTabletPoints()==0);
			}
		}

		///<summary>This should NOT be used unless GetSigChanged returns true.</summary>
		public string GetSignature(string keyData){
			//Topaz boxes are written in Windows native code.
			if(sigBoxTopaz!=null && sigBoxTopaz.Visible) {
				if(TopazWrapper.GetTopazNumberOfTabletPoints(sigBoxTopaz)==0){
					return "";
				}
				TopazWrapper.SetTopazKeyString(sigBoxTopaz,"0000000000000000");//Clear out key string
				TopazWrapper.SetTopazAutoKeyData(sigBoxTopaz,keyData);
				TopazWrapper.SetTopazEncryptionMode(sigBoxTopaz,2);
				TopazWrapper.SetTopazCompressionMode(sigBoxTopaz,2);
				return TopazWrapper.GetTopazString(sigBoxTopaz);
			}
			else{
				//ProcCur.SigIsTopaz=false;
				if(signatureBox.NumberOfTabletPoints()==0) {
					return "";
				}
				//sigBox.SetSigCompressionMode(0);
				//sigBox.SetEncryptionMode(0);
				//Some places used different logic for keystring and autokeydata in the past.  This must be maintained to keep signatures valid.
				switch(_signatureMode) {
					case SigMode.Document:
					case SigMode.TreatPlan:
					case SigMode.OrthoChart:
						//No auto key data set, only key string.
						signatureBox.SetKeyString(keyData);
						break;
					case SigMode.Default:
						signatureBox.SetKeyString("0000000000000000");
						signatureBox.SetAutoKeyData(keyData);
						break;
				}
				//sigBox.SetEncryptionMode(2);
				//sigBox.SetSigCompressionMode(2);
				return signatureBox.GetSigString();
			}
		}

		private void butClearSig_Click(object sender,EventArgs e) {
			if(ClearSignatureClicked!=null) {
				ClearSignatureClicked(this,new EventArgs());
			}
			ClearSignature();
			OnSignatureChanged();
		}

		private void butTopazSign_Click(object sender,EventArgs e) {
			//this button is not even visible if Topaz is not allowed
			if(!CheckTopaz()) {
				return;
			}
			if(SignTopazClicked!=null) {
				SignTopazClicked(this,new EventArgs());
			}
			signatureBox.Visible=false;
			sigBoxTopaz.Visible=true;
			//if(allowTopaz){
				TopazWrapper.ClearTopaz(sigBoxTopaz);
				TopazWrapper.SetTopazEncryptionMode(sigBoxTopaz,0);
				TopazWrapper.SetTopazState(sigBoxTopaz,1);
			//}
			labelInvalidSig.Visible=false;
			sigBoxTopaz.Focus();
			OnSignatureChanged();
		}

		///<summary>Explicitly set focus on this control.</summary>
		private void sigBox_MouseDown(object sender,MouseEventArgs e) {
			Focus();
			IsSigStarted=true;
		}

		private void sigBox_MouseUp(object sender,MouseEventArgs e) {
			//this is done on mouse up so that the initial pen capture won't be delayed.
			if(signatureBox.GetTabletState()==1//if accepting input.
				&& !SigChanged)//and sig not changed yet
			{
				//sigBox handles its own pen input.
				OnSignatureChanged();
			}
		}

		private void sigBoxTopaz_Leave(object sender,EventArgs e) {
			if(!CheckTopaz()) {
				return;
			}
			if(TopazWrapper.GetTopazState(sigBoxTopaz)==1) {//if accepting input.
				TopazWrapper.SetTopazState(sigBoxTopaz,0);
			}
		}

		///<summary>Must set width and height of control and run FillSignature first.</summary>
		public Bitmap GetSigImage(){
			Bitmap bitmap=new Bitmap(Width-2,Height-2);
			//no outline
			if(sigBoxTopaz!=null && sigBoxTopaz.Visible) {
				sigBoxTopaz.DrawToBitmap(bitmap,new Rectangle(0,0,Width-2,Height-2));//GetBitmap would probably work.
			}
			else {
				bitmap=(Bitmap)signatureBox.GetSigImage(false);
			}
			return bitmap;
		}

		///<summary>If this is called externally, then the event SignatureChanged will also fire.</summary>
		public void ClearSignature(bool clearTopazTablet=true){
			signatureBox.Enabled=true;
			signatureBox.ClearTablet();
			signatureBox.Visible=true;
			if(sigBoxTopaz!=null) {
				if(clearTopazTablet) {
					TopazWrapper.ClearTopaz(sigBoxTopaz);
				}
				sigBoxTopaz.Visible=false;//until user explicitly starts it.
			}
			signatureBox.SetTabletState(1);//on-screen box is now accepting input.
			SigChanged=true;
			labelInvalidSig.Visible=false;
			OnSignatureChanged();
		}

		public void SetControlSigBoxTopaz(Control sigBoxTopaz) {
			this.sigBoxTopaz=signatureBox;
		}

		///<summary>Set to 1 to activate it to start accepting signatures.  Set to 0 to no longer accept input.  Should be called with a state of '0' in FormClosing events.</summary>
		public void SetTabletState(int state) {
			if(sigBoxTopaz!=null) {
				TopazWrapper.SetTopazState(sigBoxTopaz,state);
			}
			signatureBox?.SetTabletState(state);
		}

		private Userod _userSig;
		public Userod UserSig {
			get {
				return _userSig;
			}
			set {
				_userSig=value;
			}
		}

		private void butESign_Click(object sender,EventArgs e) {
			Userod curUser=_userSig??Security.CurUser;
			Provider provCur=Providers.GetProv(curUser.ProvNum);
			string digitalSignature=Lans.g(this,"Digitally Signed by ");
			if(provCur!=null) {
				digitalSignature+=provCur.GetLongDesc();
			}
			else if(curUser!=null) {
				digitalSignature+=curUser.UserName+" (UserNum:"+curUser.UserNum+")";
			}
			else {
				//should never happen
				digitalSignature= Lans.g(this,"Digitally signed by unknown user.");
			}
			digitalSignature+="\r\n"+Lans.g(this,"Date Signed")+": "+MiscData.GetNowDateTime().ToString();
			List<Point> pList = signatureBox.EncryptString(digitalSignature);
			signatureBox.SetPointList(pList);
			OnSignatureChanged();
			signatureBox.Enabled=false;
		}









		///<summary>You do not need to extend this. You should use Default for new use cases of signatures.</summary>
		public enum SigMode {
			///<summary>Default case</summary>
			Default,
			///<summary>Signatures used for documents in the Images module.</summary>
			Document,
			///<summary>Signatures used for treatment plans.</summary>
			TreatPlan,
			///<summary>Signatures used for ortho charts.</summary>
			OrthoChart
		}
	}
}
