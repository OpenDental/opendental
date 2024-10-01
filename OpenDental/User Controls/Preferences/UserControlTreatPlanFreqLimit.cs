using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlTreatPlanFreqLimit:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlTreatPlanFreqLimit() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Methods - Private
		///<summary>The preference passed is assumed to be comma-delimited list of procedure codes.
		///Updates and returns true if all proc codes in textProcCodes are valid. Otherwise, we add these codes to errorMessage and returns false.</summary>
		private bool UpdateProcCodesPref(PrefName prefName,string textProcCodes,string labelText,ref string errorMessage) {
			List<string> listProcCodesInvalid=new List<string>();
			List<string> listProcCodes=textProcCodes
				.Split(",",StringSplitOptions.RemoveEmptyEntries)
				.ToList();
			for(int i=0;i<listProcCodes.Count;i++) {
				if(!ProcedureCodes.GetContainsKey(listProcCodes[i])) {
					listProcCodesInvalid.Add($"'{listProcCodes[i]}'");
				}
			}
			if(listProcCodesInvalid.Count > 0) {
				errorMessage+=$"\r\n  - {labelText}: {string.Join(",",listProcCodesInvalid)}";
				return false;
			}
			//All valid codes in text box.
			return Prefs.UpdateString(prefName,textProcCodes);
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillTreatPlanFreqLimit() {
			checkFrequency.Checked=PrefC.GetBool(PrefName.InsChecksFrequency);
			#region Discount Plan Frequency Codes
			textDiscountExamCodes.Text=PrefC.GetString(PrefName.DiscountPlanExamCodes);
			textDiscountFluorideCodes.Text=PrefC.GetString(PrefName.DiscountPlanFluorideCodes);
			textDiscountLimitedCodes.Text=PrefC.GetString(PrefName.DiscountPlanLimitedCodes);
			textDiscountPACodes.Text=PrefC.GetString(PrefName.DiscountPlanPACodes);
			textDiscountPerioCodes.Text=PrefC.GetString(PrefName.DiscountPlanPerioCodes);
			textDiscountProphyCodes.Text=PrefC.GetString(PrefName.DiscountPlanProphyCodes);
			textDiscountXrayCodes.Text=PrefC.GetString(PrefName.DiscountPlanXrayCodes);
			#endregion
		}

		public bool SaveTreatPlanFreqLimit() {
			string errorMessage="";
			//Should we somehow be returning false once all are updated and there were code errors so the window doesn't close and they can edit the errors? Or do we not care enough?
			Changed|=Prefs.UpdateBool(PrefName.InsChecksFrequency,checkFrequency.Checked);
			Changed|=UpdateProcCodesPref(PrefName.DiscountPlanExamCodes,textDiscountExamCodes.Text,labelDiscountExamFreq.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.DiscountPlanProphyCodes,textDiscountProphyCodes.Text,labelDiscountProphyFreq.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.DiscountPlanFluorideCodes,textDiscountFluorideCodes.Text,labelDiscountFluorideFreq.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.DiscountPlanPerioCodes,textDiscountPerioCodes.Text,labelDiscountPerioFreq.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.DiscountPlanLimitedCodes,textDiscountLimitedCodes.Text,labelDiscountLimitedFreq.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.DiscountPlanXrayCodes,textDiscountXrayCodes.Text,labelDiscountXrayFreq.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.DiscountPlanPACodes,textDiscountPACodes.Text,labelDiscountPAFreq.Text,ref errorMessage);
			if(!string.IsNullOrEmpty(errorMessage)) {//Keep them in the window if invalid codes found.
				if(MsgBox.Show(MsgBoxButtons.YesNo,Lan.g(this,"Invalid Treat' Plan procedure codes were detected and should be corrected before saving. The following codes need to be fixed:")+$"\n{errorMessage}\n\nDo you want to save anyway?")) {
					return true;
				}
				return false;
			}
			return true;
		}
		#endregion Methods - Public
	}
}
