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
			PreviewKeyDown+=FrmApptProvPrompt_PreviewKeyDown;
		}

		private void FrmApptProvPrompt_Load(object sender,EventArgs e) {
			Lang.F(this);
			System.Drawing.Point drawing_PointScreen=new System.Drawing.Point(_formFrame.Location.X,_formFrame.Location.Y);
			System.Drawing.Rectangle drawing_RectangleBoundsScreen=System.Windows.Forms.Screen.GetWorkingArea(drawing_PointScreen);
			int x=(drawing_RectangleBoundsScreen.Width/2)-((int)this.ActualWidth/2);
			int y=(drawing_RectangleBoundsScreen.Height/2)-((int)this.ActualHeight/2);
			//Position the window relative to the center of the screen
			_formFrame.Location=new System.Drawing.Point(x,y+ScaleFormValue(10));
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

		private void FrmApptProvPrompt_PreviewKeyDown(object sender,KeyEventArgs e) {
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
			if(e.Key==Key.Y) {
				butYes_Click(this,new EventArgs());
			}
			if(e.Key==Key.N) {
				butNo_Click(this,new EventArgs());
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