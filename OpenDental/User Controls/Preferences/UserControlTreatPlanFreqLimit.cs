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

		#region Methods - Event Handlers
		private void butFrequencyLimitationsDetails_Click(object sender,EventArgs e) {
			string html=@"See <a href='https://www.opendental.com/manual/insfrequencylimitations.html' target='_blank' rel='noopener noreferrer'>Frequency Limitations.</a> 
				The default codes are:
				<br><br>-BWs: D0272, D0274
				<br>-Pano/FMX: D0210, D0330
				<br>-Exams: D0120, D0150
				<br>-Cancer Screening: D0431
				<br>-Prophylaxis: D1110, D1120
				<br>-Fluoride: D1206, D1208
				<br>-Sealant: D1351
				<br>-Crown: D2740,D2750,D2751,D2752,D2780,D2781,D2782,D2783,D2790,D2791,D2792,D2794
				<br>-SRP: D4341,D4342
				<br>-Full Debridement: D4355
				<br>-Perio Maintenance: D4910
				<br>-Dentures: D5110,D5120,D5130,D5140,D5211,D5212,D5213,D5214,D5221,D5222,D5223,D5224,D5225,D5226
				<br>-Implant: D6010";
			using FormWebBrowserPrefs formWebBrowserPrefs=new FormWebBrowserPrefs();
			formWebBrowserPrefs.HtmlContent=html;
			formWebBrowserPrefs.SizeWindow=new Size(625,325);
			formWebBrowserPrefs.ShowDialog();
		}

		private void checkFrequency_Click(object sender,EventArgs e) {
			textInsBW.Enabled=checkFrequency.Checked;
			textInsPano.Enabled=checkFrequency.Checked;
			textInsExam.Enabled=checkFrequency.Checked;
			textInsCancerScreen.Enabled=checkFrequency.Checked;
			textInsProphy.Enabled=checkFrequency.Checked;
			textInsFlouride.Enabled=checkFrequency.Checked;
			textInsSealant.Enabled=checkFrequency.Checked;
			textInsCrown.Enabled=checkFrequency.Checked;
			textInsSRP.Enabled=checkFrequency.Checked;
			textInsDebridement.Enabled=checkFrequency.Checked;
			textInsPerioMaint.Enabled=checkFrequency.Checked;
			textInsDentures.Enabled=checkFrequency.Checked;
			textInsImplant.Enabled=checkFrequency.Checked;
		}
		#endregion Methods - Event Handlers

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
			textInsBW.Text=PrefC.GetString(PrefName.InsBenBWCodes);
			textInsPano.Text=PrefC.GetString(PrefName.InsBenPanoCodes);
			textInsExam.Text=PrefC.GetString(PrefName.InsBenExamCodes);
			textInsCancerScreen.Text=PrefC.GetString(PrefName.InsBenCancerScreeningCodes);
			textInsProphy.Text=PrefC.GetString(PrefName.InsBenProphyCodes);
			textInsFlouride.Text=PrefC.GetString(PrefName.InsBenFlourideCodes);
			textInsSealant.Text=PrefC.GetString(PrefName.InsBenSealantCodes);
			textInsCrown.Text=PrefC.GetString(PrefName.InsBenCrownCodes);
			textInsSRP.Text=PrefC.GetString(PrefName.InsBenSRPCodes);
			textInsDebridement.Text=PrefC.GetString(PrefName.InsBenFullDebridementCodes);
			textInsPerioMaint.Text=PrefC.GetString(PrefName.InsBenPerioMaintCodes);
			textInsDentures.Text=PrefC.GetString(PrefName.InsBenDenturesCodes);
			textInsImplant.Text=PrefC.GetString(PrefName.InsBenImplantCodes);
			if(!checkFrequency.Checked) {
				textInsBW.Enabled=false;
				textInsPano.Enabled=false;
				textInsExam.Enabled=false;
				textInsCancerScreen.Enabled=false;
				textInsProphy.Enabled=false;
				textInsFlouride.Enabled=false;
				textInsSealant.Enabled=false;
				textInsCrown.Enabled=false;
				textInsSRP.Enabled=false;
				textInsDebridement.Enabled=false;
				textInsPerioMaint.Enabled=false;
				textInsDentures.Enabled=false;
				textInsImplant.Enabled=false;
			}
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

		private List<string> GetDuplicateFrequencyLimitationCodes() {
			List<string> listProcCodes=textInsBW.Text.Split(",",StringSplitOptions.RemoveEmptyEntries).Concat(
				textInsPano.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).Concat(
				textInsExam.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).Concat(
				textInsCancerScreen.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).Concat(
				textInsProphy.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).Concat(
				textInsFlouride.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).Concat(
				textInsSealant.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).Concat(
				textInsCrown.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).Concat(
				textInsSRP.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).Concat(
				textInsDebridement.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).Concat(
				textInsPerioMaint.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).Concat(
				textInsDentures.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).Concat(
				textInsImplant.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)).ToList();
			return listProcCodes.GroupBy(x => x).Where(x => x.Count()>1).Select(x => x.Key).ToList();
		}

		public bool SaveTreatPlanFreqLimit() {
			List<string> listProcCodesDuplicate=GetDuplicateFrequencyLimitationCodes();
			if(!listProcCodesDuplicate.IsNullOrEmpty()) {
				StringBuilder stringBuilder=new StringBuilder();
				stringBuilder.AppendLine(Lan.g(this,"Frequency Limitation preferences are invalid. The following codes are present multiple times:"));
				stringBuilder.AppendLine(string.Join(",",listProcCodesDuplicate));
				MsgBox.Show(stringBuilder.ToString());
				return false;
			}
			string errorMessage="";
			//Should we somehow be returning false once all are updated and there were code errors so the window doesn't close and they can edit the errors? Or do we not care enough?
			Changed|=Prefs.UpdateBool(PrefName.InsChecksFrequency,checkFrequency.Checked);
			Changed|=UpdateProcCodesPref(PrefName.InsBenBWCodes,textInsBW.Text,labelInsBW.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenPanoCodes,textInsPano.Text,labelInsPano.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenExamCodes,textInsExam.Text,labelInsExam.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenCancerScreeningCodes,textInsCancerScreen.Text,labelInsCancerScreen.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenProphyCodes,textInsProphy.Text,labelInsProphy.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenFlourideCodes,textInsFlouride.Text,labelInsFlouride.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenSealantCodes,textInsSealant.Text,labelInsSealant.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenCrownCodes,textInsCrown.Text,labelInsCrown.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenSRPCodes,textInsSRP.Text,labelInsSRP.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenFullDebridementCodes,textInsDebridement.Text,labelInsDebridement.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenPerioMaintCodes,textInsPerioMaint.Text,labelInsPerioMaint.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenDenturesCodes,textInsDentures.Text,labelInsDentures.Text,ref errorMessage);
			Changed|=UpdateProcCodesPref(PrefName.InsBenImplantCodes,textInsImplant.Text,labelInsImplant.Text,ref errorMessage);
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
