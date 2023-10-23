using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormDisplayFieldEdit : FormODBase {
		private Font _font=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Bold);
		public DisplayField DisplayFieldCur;
		///<summary>True when we allow the user to set a display field width to 0 to allow dynamic grid widths.</summary>
		public bool AllowZeroWidth=false;

		///<summary></summary>
		public FormDisplayFieldEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDisplayFieldEdit_Load(object sender,EventArgs e) {
			textInternalName.Text=DisplayFieldCur.InternalName;
			textDescription.Text=DisplayFieldCur.Description;
			textWidth.Text=DisplayFieldCur.ColumnWidth.ToString();
			if(AllowZeroWidth) {
				//textWidth.MinVal=0;
				//labelZeroWidth.Visible=true;
				//jordan 2020-08-05-This feature is just too hard to explain to users. It would only work if layout set to fill right.  
				//It's good enough that the right column does that.  We don't need a middle column to be dynamic.
				//Furthermore, it would require changing ODGridColumn.ColWidth to handle IsWidthDynamic
			}
			FillWidth();
		}

		private void FillWidth(){
			Graphics g=this.CreateGraphics();
			int width;
			//Add a 5 pixel buffer for slight variations of font scaling, fonts do not scale with the same ratios that other controls do, so we need some additional space to compensate. Also we don't need to use the LayoutManager.Scale for these 5 pixels since the Grid is already scaling.
			if(textDescription.Text==""){
				width=(int)g.MeasureString(textInternalName.Text,_font).Width+5;
			}
			else{
				width=(int)g.MeasureString(textDescription.Text,_font).Width+5;
			}
			textWidthMin.Text=width.ToString();
			g.Dispose();
		}

		private void textDescription_TextChanged(object sender,EventArgs e) {
			FillWidth();
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!textWidth.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			DisplayFieldCur.Description=textDescription.Text;
			DisplayFieldCur.ColumnWidth=PIn.Int(textWidth.Text);
			DialogResult=DialogResult.OK;
		}

	}
}