using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	public partial class UserControlScreenTooth:UserControl {
		///<summary>If true, represents the back teeth.</summary>
		public bool IsMolar=true;
		public bool IsScreening=true;
		///<summary>If true, represents the top row of teeth</summary>
		public bool IsLing=false;
		///<summary>If true, represents the right side of the patient's mouth</summary>
		public bool IsRightSide=false;
		public bool IsPrimary=false;


		[Category("Layout"),Description("Set true if this is a molar tooth.")]
		public bool Molar {get { return IsMolar; } set { IsMolar=value; } }
		[Category("Layout"),Description("Set true if this is a lingual tooth.")]
		public bool Lingual {get {return IsLing; } set { IsLing=value; } }
		[Category("Layout"),Description("Set true if this is a tooth on the patient's right side.")]
		public bool RightSide {get {return IsRightSide; } set { IsRightSide=value; } }

		public UserControlScreenTooth() {
			InitializeComponent();
		}

		private void UserControlTooth_Load(object sender,EventArgs e) {
			if(IsScreening) {
				if(IsPrimary || !IsMolar) {
					sheetComboBox1.Visible=false;
					sheetComboBox2.Visible=false;
					sheetComboBox3.Height=this.Height;
					sheetComboBox3.Width=this.Width;
					sheetComboBox3.Location=new Point(0,0);
				}
				else if(IsRightSide) {
					sheetComboBox1.SelectedOption="d";
					sheetComboBox1.DefaultOption="d";
					sheetComboBox2.SelectedOption="m";
					sheetComboBox2.DefaultOption="m";
					if(IsLing) {
						sheetComboBox3.SelectedOption="ling";
						sheetComboBox3.DefaultOption="ling";
					}
					else {
						sheetComboBox3.SelectedOption="buc";
						sheetComboBox3.DefaultOption="buc";
					}
				}
				else {
					sheetComboBox1.SelectedOption="m";
					sheetComboBox1.DefaultOption="m";
					sheetComboBox2.SelectedOption="d";
					sheetComboBox2.DefaultOption="d";
					if(IsLing) {
						sheetComboBox3.SelectedOption="ling";
						sheetComboBox3.DefaultOption="ling";
					}
					else {
						sheetComboBox3.SelectedOption="buc";
						sheetComboBox3.DefaultOption="buc";
					}
				}
			}
		}
		
		public List<string> GetSelected() {
			return new List<string>() { sheetComboBox1.SelectedOption,sheetComboBox2.SelectedOption,sheetComboBox3.SelectedOption };
		}

		public void SetSelected(string[] listSelectedOptions) {
			sheetComboBox2.SelectedOption=listSelectedOptions[1];
			sheetComboBox3.SelectedOption=listSelectedOptions[2];
			sheetComboBox1.SelectedOption=listSelectedOptions[0];
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
		}

	}
}
