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
		private Def _defCur;
		
		///<summary></summary>
		public FormDefEditBlockout(Def defCur) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager(); 
			Lan.F(this);
			_defCur=defCur.Copy();
		}

		private void FormDefEdit_Load(object sender,System.EventArgs e) {
			textName.Text=_defCur.ItemName;
			if(_defCur.ItemValue.Contains(BlockoutType.DontCopy.GetDescription())) {
				checkCutCopyPaste.Checked=true;
			}
			if(_defCur.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription())) {
				checkOverlap.Checked=true;
			}
			checkHidden.Checked=_defCur.IsHidden;
			butColor.BackColor=_defCur.ItemColor;
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
			_defCur.ItemName=textName.Text;
			List<string> listDescriptions=new List<string>();
			if(checkCutCopyPaste.Checked) {
				listDescriptions.Add(BlockoutType.DontCopy.GetDescription());
			}
			if(checkOverlap.Checked) {
				listDescriptions.Add(BlockoutType.NoSchedule.GetDescription());
			}
			_defCur.ItemValue=string.Join(",", listDescriptions);
			_defCur.IsHidden=checkHidden.Checked;
			_defCur.ItemColor=butColor.BackColor;
			if(IsNew){
				Defs.Insert(_defCur);
			}
			else{
				Defs.Update(_defCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}