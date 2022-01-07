using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental {
	public partial class EServiceMetricsControl:UserControl {
		bool _flashUp=true;
		private Color _alertColor=Color.Black;
		[Category("Appearance")]
		[Description("Alert Bubble Color")]
		public Color AlertColor {
			get {
				return _alertColor;
			}
			set {
				if(_alertColor==value) {
					return;
				}
				_alertColor=value;
				panelAlertColor.BackColor=_alertColor;
			}
		}

		private float _accountBalance=0f;
		[Category("Appearance")]
		[Description("Remaining Balance")]
		public float AccountBalance {
			get {
				return _accountBalance;
			}
			set {
				_accountBalance=value;
				labelAccountBalance.Text=AccountBalanceEuro;
			}
		}

		public string AccountBalanceEuro {
			get {
				return "\u20AC"+AccountBalance.ToString("0");
			}
		}

		public string FlashReason {
			get {
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
		}

		public bool IsFlashing {
			get {
				return timerFlash.Enabled;
			}
		}
		
		public EServiceMetricsControl() {
			InitializeComponent();
		}

		private void timerFlash_Tick(object sender,EventArgs e) {
			//Flip the colors.
			if(_flashUp) {
				panelAlertColor.BackColor=AlertColor;
			}
			else {
				panelAlertColor.BackColor=this.BackColor;
			}
			_flashUp=!_flashUp;
		}

		public void StartFlashing(string reason) {
			if(IsFlashing) { //already on
				return;
			}
			//Flash is starting so save the reason so we can look at it.
			labelAccountBalance.Tag=reason;
			timerFlash.Start();
		}

		public void StopFlashing() {
			if(!IsFlashing) { //already off
				return;
			}
			//Flash is stopping so mark fixed but keep it around so we can still look at it if desired.
			string reason="THIS PROBLEM HAS BEEN FIXED! "+FlashReason;
			labelAccountBalance.Tag=reason;
			timerFlash.Stop();
			panelAlertColor.BackColor=AlertColor;
		}

		private void panelAlertColor_Paint(object sender,PaintEventArgs e) {
			//A generic GDI+ exception can happen within this paint method (see job #7657) and this isn't important enough to crash the whole program.
			ODException.SwallowAnyException(() => {
				e.Graphics.Clear(this.BackColor);
				Rectangle rectDraw=Rectangle.FromLTRB(e.ClipRectangle.Left,e.ClipRectangle.Top,e.ClipRectangle.Right,e.ClipRectangle.Bottom);
				rectDraw.Inflate(-2,-2);
				using(Pen pen=new Pen(this.ForeColor,4)) {
					e.Graphics.DrawEllipse(pen,rectDraw);
				}
				rectDraw.Inflate(-1,-1);
				using(Brush brush=new SolidBrush(panelAlertColor.BackColor)) {
					e.Graphics.FillEllipse(brush,rectDraw);
				}
			});
		}

		private void labelAccountBalance_DoubleClick(object sender,EventArgs e) {
			MessageBox.Show(FlashReason);
		}
	}
}
