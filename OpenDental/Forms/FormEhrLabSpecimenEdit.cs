using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormEhrLabSpecimenEdit:FormODBase {
		public EhrLabSpecimen EhrLabSpecimenCur;
		public bool IsImport;
		public bool IsViewOnly;

		public FormEhrLabSpecimenEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormEhrLabOrders_Load(object sender,EventArgs e) {
			IsImport=true;//this can be changed later, but for now there is never a need to edit a lab specimen.
			if(IsImport || IsViewOnly) {
				foreach(Control c in Controls) {
					c.Enabled=false;
				}
				butCancel.Text="Close";
				butCancel.Enabled=true;
			}
			FillGridCondition();
			FillGridReject();
			//Times 
			textCollectionDateTimeStart.Text=EhrLab.formatDateFromHL7(EhrLabSpecimenCur.CollectionDateTimeStart);
			textCollectionDateTimeEnd.Text=EhrLab.formatDateFromHL7(EhrLabSpecimenCur.CollectionDateTimeEnd);
			//Coded Value
			textSpecimenTypeID.Text=EhrLabSpecimenCur.SpecimenTypeID;
			textSpecimenTypeText.Text=EhrLabSpecimenCur.SpecimenTypeText;
			textSpecimenTypeCodeSystemName.Text=EhrLabSpecimenCur.SpecimenTypeCodeSystemName;
			textSpecimenTypeIDAlt.Text=EhrLabSpecimenCur.SpecimenTypeIDAlt;
			textSpecimenTypeTextAlt.Text=EhrLabSpecimenCur.SpecimenTypeTextAlt;
			textSpecimenTypeCodeSystemNameAlt.Text=EhrLabSpecimenCur.SpecimenTypeCodeSystemNameAlt;
		}

		private void FillGridCondition() {
			gridCondition.BeginUpdate();
			gridCondition.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Description",80);
			gridCondition.ListGridColumns.Add(col);
			gridCondition.ListGridRows.Clear();
				GridRow row;
			for(int i=0;i<EhrLabSpecimenCur.ListEhrLabSpecimenCondition.Count;i++) {
				row=new GridRow();
				if(EhrLabSpecimenCur.ListEhrLabSpecimenCondition[i].SpecimenConditionText!="") {
					row.Cells.Add(EhrLabSpecimenCur.ListEhrLabSpecimenCondition[i].SpecimenConditionText);
				}
				else if(EhrLabSpecimenCur.ListEhrLabSpecimenCondition[i].SpecimenConditionTextAlt!="") {
					row.Cells.Add(EhrLabSpecimenCur.ListEhrLabSpecimenCondition[i].SpecimenConditionTextAlt);
				}
				else if(EhrLabSpecimenCur.ListEhrLabSpecimenCondition[i].SpecimenConditionTextOriginal!="") {
					row.Cells.Add(EhrLabSpecimenCur.ListEhrLabSpecimenCondition[i].SpecimenConditionTextOriginal);
				}
				else {
					row.Cells.Add("Unkown Condition Code.");//should never happen
				}
				gridCondition.ListGridRows.Add(row);
			}
			gridCondition.EndUpdate();
		}

		private void FillGridReject() {
			gridReject.BeginUpdate();
			gridReject.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Description",80);
			gridReject.ListGridColumns.Add(col);
			gridReject.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<EhrLabSpecimenCur.ListEhrLabSpecimenRejectReason.Count;i++) {
				row=new GridRow();
				if(EhrLabSpecimenCur.ListEhrLabSpecimenRejectReason[i].SpecimenRejectReasonText!="") {
					row.Cells.Add(EhrLabSpecimenCur.ListEhrLabSpecimenRejectReason[i].SpecimenRejectReasonText);
				}
				else if(EhrLabSpecimenCur.ListEhrLabSpecimenRejectReason[i].SpecimenRejectReasonTextAlt!="") {
					row.Cells.Add(EhrLabSpecimenCur.ListEhrLabSpecimenRejectReason[i].SpecimenRejectReasonTextAlt);
				}
				else if(EhrLabSpecimenCur.ListEhrLabSpecimenRejectReason[i].SpecimenRejectReasonTextOriginal!="") {
					row.Cells.Add(EhrLabSpecimenCur.ListEhrLabSpecimenRejectReason[i].SpecimenRejectReasonTextOriginal);
				}
				else {
					row.Cells.Add("Unkown Reject Reason Code.");//should never happen
				}
				gridReject.ListGridRows.Add(row);
			}
			gridReject.EndUpdate();
		}

		private void butSave_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		






	}
}
