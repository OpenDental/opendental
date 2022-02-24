
/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormDefEditBlockout:FormODBase {
		///<summary></summary>
		public bool IsNew;
		private Def DefCur;
		
		///<summary></summary>
		public FormDefEditBlockout(Def defCur) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
			DefCur=defCur.Copy();
		}

		private void FormDefEdit_Load(object sender,System.EventArgs e) {
			textName.Text=DefCur.ItemName;
			if(DefCur.ItemValue.Contains(BlockoutType.DontCopy.GetDescription())) {
				checkCutCopyPaste.Checked=true;
			}
			if(DefCur.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription())) {
				checkOverlap.Checked=true;
			}
			checkHidden.Checked=DefCur.IsHidden;
			butColor.BackColor=DefCur.ItemColor;
		}

		private void butColor_Click(object sender,EventArgs e) {
			colorDialog1.Color=butColor.BackColor;
			colorDialog1.ShowDialog();
			butColor.BackColor=colorDialog1.Color;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textName.Text==""){
				MsgBox.Show(this,"Name required.");
				return;
			}
			DefCur.ItemName=textName.Text;
			List<string> itemVal=new List<string>();
			if(checkCutCopyPaste.Checked) {
				itemVal.Add(BlockoutType.DontCopy.GetDescription());
			}
			if(checkOverlap.Checked) {
				itemVal.Add(BlockoutType.NoSchedule.GetDescription());
			}
			DefCur.ItemValue=string.Join(",", itemVal);
			DefCur.IsHidden=checkHidden.Checked;
			DefCur.ItemColor=butColor.BackColor;
			if(IsNew){
				Defs.Insert(DefCur);
			}
			else{
				Defs.Update(DefCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}