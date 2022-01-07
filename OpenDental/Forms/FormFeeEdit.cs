using System;
using System.Drawing;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormFeeEdit : FormODBase {
		///<summary></summary>
		public Fee FeeCur;
		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FormFeeEdit(){//(Fee feeCur){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			//FeeCur=feeCur;
			//checkDefFee.Checked=FeeCur.UseDefaultFee;
			//checkDefCov.Checked=FeeCur.UseDefaultCov;
			Lan.F(this, new System.Windows.Forms.Control[] {
				//exclude:
				this.checkDefCov,
				this.checkDefFee,
			});
		}

		private void FormFeeEdit_Load(object sender, System.EventArgs e) {
			FeeSched feeSched=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==FeeCur.FeeSched);
			if(!FeeL.CanEditFee(feeSched,FeeCur.ProvNum,FeeCur.ClinicNum)) {
				DialogResult=DialogResult.Cancel;
				Close();
				return;
			}
			Location=new Point(Location.X-190,Location.Y-20);
			textFee.Text=FeeCur.Amount.ToString("F");
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textFee.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry error first."));
				return;
			}
			DateTime datePrevious=FeeCur.SecDateTEdit;
			if(textFee.Text==""){
				Fees.Delete(FeeCur);
			}
			else if(CompareDouble.IsEqual(FeeCur.Amount,PIn.Double(textFee.Text))) {
				DialogResult=DialogResult.OK;
				return;
			}
			else{
				Fee oldFee=FeeCur.Copy();
				FeeCur.Amount=PIn.Double(textFee.Text);
				Fees.Update(FeeCur,oldFee);//Fee object always created and inserted externally first
			}
			SecurityLogs.MakeLogEntry(Permissions.ProcFeeEdit,0,Lan.g(this,"Procedure")+": "+ProcedureCodes.GetStringProcCode(FeeCur.CodeNum)
				+", "+Lan.g(this,"Fee: ")+""+FeeCur.Amount.ToString("c")+", "+Lan.g(this,"Fee Schedule")+": "+FeeScheds.GetDescription(FeeCur.FeeSched)
				+". "+Lan.g(this,"Manual edit in Edit Fee window."),FeeCur.CodeNum,DateTime.MinValue);
			SecurityLogs.MakeLogEntry(Permissions.LogFeeEdit,0,Lan.g(this,"Fee Updated"),FeeCur.FeeNum,datePrevious);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormFeeEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(IsNew){
				Fees.Delete(FeeCur);
			}
		}

	}
}
