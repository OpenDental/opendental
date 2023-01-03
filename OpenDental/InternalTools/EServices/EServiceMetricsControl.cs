using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental {
	public partial class EServiceMetricsControl:UserControl {
		private float _accountBalance=0f;
		private Color _colorAlert=Color.Blue;
		private bool _flashUp=true;

		///<summary>When flashing, color of the bubble will alternate between this and the BackColor.</summary>
		public void SetColorAlert(Color color) {
			if(_colorAlert==color) {
				return;
			}
			_colorAlert=color;
			panelAlertColor.BackColor=_colorAlert;
		}

		public void SetAccountBalance(float bal) {
			_accountBalance=bal;
			labelAccountBalance.Text="\u20AC"+_accountBalance.ToString("0");
		}

		private string GetFlashReason() {
			if(labelAccountBalance.Tag==null) {
				return "No errors found";
			}
			if(!(labelAccountBalance.Tag is string)) {
				return "No errors found";
			}
			if(string.IsNullOrEmpty((string)labelAccountBalance.Tag)) {
				return "No errors found";
			}
			return(string)labelAccountBalance.Tag;
		}
		
		public EServiceMetricsControl() {
			InitializeComponent();
		}

		private void timerFlash_Tick(object sender,EventArgs e) {
			//Flip the colors.
			if(_flashUp) {
				panelAlertColor.BackColor=_colorAlert;
			}
			else {
				panelAlertColor.BackColor=this.BackColor;
			}
			_flashUp=!_flashUp;
		}

		public void StartFlashing(string reason) {
			if(timerFlash.Enabled) { //already on
				return;
			}
			//Flash is starting so save the reason so we can look at it.
			labelAccountBalance.Tag=reason;
			timerFlash.Start();
		}

		public void StopFlashing() {
			if(!timerFlash.Enabled) { //already off
				return;
			}
			//Flash is stopping so mark fixed but keep it around so we can still look at it if desired.
			string reason="THIS PROBLEM HAS BEEN FIXED! "+GetFlashReason();
			labelAccountBalance.Tag=reason;
			timerFlash.Stop();
			panelAlertColor.BackColor=_colorAlert;
		}

		private void panelAlertColor_Paint(object sender,PaintEventArgs e) {
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.Clear(this.BackColor);
			Rectangle rectangle=new Rectangle(0,0,panelAlertColor.Width-1,panelAlertColor.Height-1);
			rectangle.Inflate(-2,-2);
			using Pen pen=new Pen(this.ForeColor,4);
			g.DrawEllipse(pen,rectangle);
			rectangle.Inflate(-1,-1);
			using Brush brush=new SolidBrush(panelAlertColor.BackColor);
			g.FillEllipse(brush,rectangle);
		}

		private void labelAccountBalance_DoubleClick(object sender,EventArgs e) {
			MessageBox.Show(GetFlashReason());
		}
	}
}
