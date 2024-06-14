using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCodeGroupEdit:FormODBase {
		///<summary>The code group being edited. This object in memory will be updated with any changes made in this window if the user clicks OK. Otherwise, it is left alone. Leave null if adding a new code group.</summary>
		public CodeGroup CodeGroup;
		///<summary>The list of EnumCodeGroupFixed values that have already been used in another code group. Users will be blocked from using these values when editing the code group passed in.</summary>
		public List<EnumCodeGroupFixed> ListEnumCodeGroupFixedsInUse;

		public FormCodeGroupEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCodeGroupEdit_Load(object sender,EventArgs e) {
			checkShowInFreq.Checked=!CodeGroup.IsHidden;
			checkShowInAgeLim.Checked=CodeGroup.ShowInAgeLimit;
			textGroupName.Text=CodeGroup.GroupName;
			textProcCodes.Text=CodeGroup.ProcCodes;
			//Start by allowing the user to select any value from the EnumCodeGroupFixed enumeration.
			List<EnumCodeGroupFixed> listEnumCodeGroupFixeds=Enum.GetValues(typeof(EnumCodeGroupFixed)).Cast<EnumCodeGroupFixed>().ToList();
			//Remove enum values that are already in use by other code groups (duplicates are not allowed in the database).
			if(!ListEnumCodeGroupFixedsInUse.IsNullOrEmpty()) {
				//Never remove the 'None' option or the value set to the code group being edited. None is the only duplicate value allowed.
				ListEnumCodeGroupFixedsInUse.RemoveAll(x => x.In(EnumCodeGroupFixed.None,CodeGroup.CodeGroupFixed));
				//Remove every enum value that was passed in by the calling method (indicating values that are already in use).
				listEnumCodeGroupFixeds=listEnumCodeGroupFixeds.Except(ListEnumCodeGroupFixedsInUse).ToList();
			}
			comboCodeGroupFixed.Items.AddListEnum<EnumCodeGroupFixed>(listEnumCodeGroupFixeds);
			comboCodeGroupFixed.SetSelectedEnum(CodeGroup.CodeGroupFixed);
		}

		private void butProcCodesAdd_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.CanAllowMultipleSelections=true;
			if(formProcCodes.ShowDialog()!=DialogResult.OK) {
				return;
			}
			//Preserve the current procedure codes entered into the text box.
			List<string> listProcCodes=GetProcCodesFromUI();
			//Add the ProcCode of every procedure code that was selected in the picker window.
			listProcCodes.AddRange(formProcCodes.ListProcedureCodesSelected.Select(x => x.ProcCode));
			//Override the text in the text box with a new comma delimited list of proc codes.
			textProcCodes.Text=string.Join(",",listProcCodes);
		}

		private List<string> GetProcCodesFromUI() {
			//Users are not allowed to enter duplicate proc codes so we will automatically group duplicates for them.
			//Also, users are allowed to enter values that are not currently present within the database.
			//The following code is case sensitive on purpose.
			List<string> listProcCodes=textProcCodes.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)
				.Select(x => x.Trim())
				.Distinct()
				.ToList();
			listProcCodes.RemoveAll(x => string.IsNullOrWhiteSpace(x));
			return listProcCodes;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(string.IsNullOrWhiteSpace(textGroupName.Text)) {
				MsgBox.Show(this,"Group Name is required.");
				return;
			}
			CodeGroup.CodeGroupFixed=comboCodeGroupFixed.GetSelected<EnumCodeGroupFixed>();
			CodeGroup.GroupName=textGroupName.Text;
			CodeGroup.IsHidden=!checkShowInFreq.Checked;
			CodeGroup.ShowInAgeLimit=checkShowInAgeLim.Checked;
			CodeGroup.ProcCodes=string.Join(",",GetProcCodesFromUI());
			DialogResult=DialogResult.OK;
		}
	}
}