using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDental{
	///<summary></summary>
	public class TableClaimPay : OpenDental.ContrTable{
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		public TableClaimPay(){
			InitializeComponent();// This call is required by the Windows Form Designer.
			MaxRows=20;
			MaxCols=5;
			ShowScroll=true;
			FieldsArePresent=true;
			HeadingIsPresent=true;
			InstantClassesPar();
			SetRowHeight(0,19,14);
			Heading=Lan.g("TableClaimPay","Insurance Checks");
			Fields[0]=Lan.g("TableClaimPay","Date");
			Fields[1]=Lan.g("TableClaimPay","Amount");
			Fields[2]=Lan.g("TableClaimPay","Check Num");
			Fields[3]=Lan.g("TableClaimPay","Bank/Branch");
			Fields[4]=Lan.g("TableClaimPay","Note");
			ColAlign[1]=HorizontalAlignment.Right;
			ColWidth[0]=70;
			ColWidth[1]=80;
			ColWidth[2]=100;
			ColWidth[3]=100;
			ColWidth[4]=180;

			DefaultGridColor=Color.LightGray;
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
			// TableClaimProc
			// 
			this.Name = "TableClaimProc";
			this.Load += new System.EventHandler(this.TableClaimProc_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void TableClaimProc_Load(object sender, System.EventArgs e) {
			LayoutTables();
		}

	}
}

