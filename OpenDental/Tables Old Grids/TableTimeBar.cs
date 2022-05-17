using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

using System.Windows.Forms;


namespace OpenDental{
///<summary></summary>
	public class TableTimeBar : OpenDental.ContrTable{
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		public TableTimeBar(){
			InitializeComponent();// This call is required by the Windows Form Designer.
			MaxRows=40;
			MaxCols=1;
			ShowScroll=false;
			FieldsArePresent=false;
			HeadingIsPresent=false;
			InstantClassesPar();
			SetRowHeight(0,39,14);
			ColWidth[0]=13;
			ColAlign[0]=HorizontalAlignment.Center;
			SetGridColor(Color.LightGray);
			/*TopBorder[0,6]=Color.Black;
			TopBorder[0,12]=Color.Black;
			TopBorder[0,18]=Color.Black;
			TopBorder[0,24]=Color.Black;
			TopBorder[0,30]=Color.Black;
			TopBorder[0,36]=Color.Black;*/
			LayoutTables();
		}

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if (components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code

		private void InitializeComponent(){
			this.SuspendLayout();
			// 
			// TableTimeBar
			// 
			this.Name = "TableTimeBar";
			this.Load += new System.EventHandler(this.TableTimeBar_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void TableTimeBar_Load(object sender, System.EventArgs e) {
			LayoutTables();
		}

		private void butSlider_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
		
		}

		private void butSlider_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
		
		}

		private void butSlider_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
		
		}

		

	}
}

