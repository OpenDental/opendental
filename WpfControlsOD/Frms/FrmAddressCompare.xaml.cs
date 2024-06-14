using Bridges;
using CodeBase;
using System;

namespace OpenDental {
	///<summary> Shows an OpenDentBusiness.Address, and a USPSAddressValidationResponse, and allows the user to select between the two, or cancelling. </summary>
	public partial class FrmAddressCompare:FrmODBase {
		///<summary> Address entered by the user </summary>
		public OpenDentBusiness.Address AddressFromUserInput;
		///<summary> Set true when the user has accepted an address. False when cancelling. </summary>
		public bool UserChoseAddress=false;
		///<summary> Response From USPS with corrected address. </summary>
		public USPSAddressValidationResponse USPSAddressValidationResponse_;

		public FrmAddressCompare() {
			InitializeComponent();
			this.Load+=FrmAddressCompare_Load;
		}

		public void FrmAddressCompare_Load(object sender,EventArgs e) {
			Lang.F(this);
			textAddressUser.Text=AddressFromUserInput.Address1;
			textAddress2User.Text=AddressFromUserInput.Address2;
			textCityUser.Text=AddressFromUserInput.City;
			textStateUser.Text=AddressFromUserInput.State;
			textZIPUser.Text=AddressFromUserInput.Zip;
			if(USPSAddressValidationResponse_.address!=null) {
				textAddressUSPS.Text=USPSAddressValidationResponse_.address.streetAddress;
				textAddress2USPS.Text=USPSAddressValidationResponse_.address.secondaryAddress;
				textCityUSPS.Text=USPSAddressValidationResponse_.address.city;
				textStateUSPS.Text=USPSAddressValidationResponse_.address.state;
				textZIPUSPS.Text=USPSAddressValidationResponse_.address.ZIPCode;
				if(!string.IsNullOrWhiteSpace(USPSAddressValidationResponse_.address.ZIPPlus4)) {
					textZIPUSPS.Text=textZIPUSPS.Text+"-"+USPSAddressValidationResponse_.address.ZIPPlus4;
				}
			}
			EnumDPVConfirmation dPVConfirmation;
			if(USPSAddressValidationResponse_.additionalInfo==null || USPSAddressValidationResponse_.additionalInfo.DPVConfirmation==null) {
				dPVConfirmation=EnumDPVConfirmation.None;
			}
			else {
				try {
					dPVConfirmation=(EnumDPVConfirmation)Enum.Parse(typeof(EnumDPVConfirmation), USPSAddressValidationResponse_.additionalInfo.DPVConfirmation);
				}
				catch {
					//if we cant parse the DPVConfirmation status, thats a bad sign. Show the error status.
					dPVConfirmation=EnumDPVConfirmation.None;
				}
			}
			if(dPVConfirmation==EnumDPVConfirmation.Y) {//the normal response
				labelDeliverabilityStatus.Visible=false;
			}
			else {
				labelDeliverabilityStatus.Text=Lang.g(this,dPVConfirmation.GetDescription());
				labelDeliverabilityStatus.Visible=true;
			}
		}

		private void butYes_Click(object sender,EventArgs e) {
			UserChoseAddress=true;
			IsDialogOK=true;
		}

		private void butNo_Click(object sender,EventArgs e) {
			UserChoseAddress=true;
			IsDialogCancel=true;
		}

	}
}
