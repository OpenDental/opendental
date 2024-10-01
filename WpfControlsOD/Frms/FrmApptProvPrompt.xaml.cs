using System;
using System.Collections.Generic;
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
	/// <summary></summary>
	public partial class FrmApptProvPrompt:FrmODBase {
		public EnumApptProvPrompt EnumApptProvPrompt_;

		///<summary></summary>
		public FrmApptProvPrompt() {
			InitializeComponent();
			Load+=FrmApptProvPrompt_Load;
			PreviewKeyDown+=Frm_PreviewKeyDown;
		}

		private void FrmApptProvPrompt_Load(object sender,EventArgs e) {
			Lang.F(this);
		}

		///<summary>Automatically selects Yes or No based on the ApptModuleProviderPrompt preference setting without prompting the user.</summary>
		public void AutomateSelection() {
			if(EnumApptProvPrompt_==EnumApptProvPrompt.NoPromptChange) {
				butYes_Click(this,new EventArgs());
			}
			if(EnumApptProvPrompt_==EnumApptProvPrompt.NoPromptNoChange) {
				butNo_Click(this,new EventArgs());
			}
		}

		private void Frm_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter){
				if(EnumApptProvPrompt_==EnumApptProvPrompt.PromptDefaultYes) {
					//Pressing enter selects the yes button
					butYes_Click(this,new EventArgs());
				}
				if(EnumApptProvPrompt_==EnumApptProvPrompt.PromptDefaultNo) {
					//Pressing enter selects the no button
					butNo_Click(this,new EventArgs());
				}
			}
		}

		private void butYes_Click(object sender,EventArgs e) {
			IsDialogOK=true;
		}

		private void butNo_Click(object sender,EventArgs e) {
			IsDialogOK=false;
		}
	}
}