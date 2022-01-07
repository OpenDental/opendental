using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEhrLabPanelImport:FormODBase {
		private List<MedicalOrder> listLabOrders;
		private long patNum;
		private string fName;
		private string lName;

		public FormEhrLabPanelImport() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormLabPanelImport_Load(object sender,EventArgs e) {
			
		}

		private void butReceive_Click(object sender,EventArgs e) {
			/*string sample=@"MSH|^~\&|KAM|DGI|Y|OU|20100920093000||ORU^R01^ORU_R01|20100920093000|P|2.5.1
PID||405979410 |405979410^^^&2.16.840.1.113883.19.3.2.1&ISO^MR||Lewis^Frank ||19500101|M||2106-3^White^HL70005|622 Chestnut^^Springfield^Tennessee^37026^^M||^^^^^615^3826396|||||405979410 ||||N^Not Hispanic or Latino^HL70189
OBR|1|OrderNum-1001|FillOrder-1001|24331-1^Lipid Panel^LN||20100920083000|20100920083000|20100920083000|||||Hemolyzed ||LNA&Arterial Catheter&Hl70070| ProviderIDNum-100^Crow^Tom^Black^III^Dr.||||Aloha Laboratories 575 Luau Street Honolulu Hawaii 96813 ||||CH|F|
OBX|1|NM|14647-2^Total cholesterol^LN |134465|162|mg/dl |<200| N|||F|||20100920083000 
OBX|2|NM|14646-4^HDL cholesterol^LN|333123|43|mg/dl|>=40| N|||F|||20100920083000
OBX|3|NM|2089-1^LDL cholesterol^LN|333123|84|mg/dl|<100| N|||F|||20100920083000
OBX|4|NM|14927-8^Triglycerides^LN|333123|127|mg/dl|<150| N|||F|||20100920083000";*/
			string sample="";
			Cursor=Cursors.WaitCursor;
			try {
				sample=EmailMessages.ReceiveOneForEhrTest();
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			textHL7Raw.Text=sample;
		}

		private void textHL7Raw_TextChanged(object sender,EventArgs e) {
			textPatName.Text="";
			textPatIDNum.Text="";
			textPatAccountNum.Text="";
			textDateTimeTest.Text="";
			textServiceID.Text="";
			textServiceName.Text="";
			try {
				//ORU-R01
				MessageHL7 msg=new MessageHL7(textHL7Raw.Text);
				SegmentHL7 segPID=msg.GetSegment(SegmentNameHL7.PID,false);
				patNum=0;
				if(segPID!=null) {
					fName=segPID.GetFieldComponent(5,1);
					lName=segPID.GetFieldComponent(5,0);
					//F M L ?
					textPatName.Text=segPID.GetFieldComponent(5,1)+" "+segPID.GetFieldComponent(5,2)+" "+segPID.GetFieldComponent(5,0);
					textPatIDNum.Text=segPID.GetFieldFullText(2);
					patNum=Patients.GetPatNumByName(lName,fName);//could be 0
					//patNum=PIn.Long(segPID.GetFieldFullText(2));
					textPatAccountNum.Text=segPID.GetFieldFullText(18);
				}
				SegmentHL7 segOBR=msg.GetSegment(SegmentNameHL7.OBR,false);
				if(segOBR!=null) {
					textServiceID.Text=segOBR.GetFieldComponent(4,0);
					textServiceName.Text=segOBR.GetFieldComponent(4,1);
				}
				SegmentHL7 segOBX=msg.GetSegment(SegmentNameHL7.OBX,false);
				if(segOBX!=null) {
					DateTime dt=segOBX.GetDateTime(14);
					if(dt.Year>1880) {
						textDateTimeTest.Text=dt.ToShortDateString();// +" "+dt.ToShortTimeString();
					}
				}
				FillPatAndGrid();
			}
			catch {
				patNum=0;
				FillPatAndGrid();
				MessageBox.Show("Error parsing HL7.");
			}
		}

		private void FillPatAndGrid() {
			Patient pat=Patients.GetLim(patNum);
			textPatName2.Text=pat.GetNameFLnoPref();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Date",85);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Order",190);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Results Attached",150,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			listLabOrders=MedicalOrders.GetAllLabs(patNum);//this works for 0
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listLabOrders.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listLabOrders[i].DateTimeOrder.ToShortDateString());
				row.Cells.Add(listLabOrders[i].Description);
				bool hasResultsAttached=MedicalOrders.LabHasResultsAttached(listLabOrders[i].MedicalOrderNum);
				if(hasResultsAttached) {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//if(!CreateLabPanel()) {
			//	return;
			//}
			CreateLabPanel();
			DialogResult=DialogResult.OK;
		}

		private void butChangePat_Click(object sender,EventArgs e) {
			Patient pat=Patients.GetLim(patNum);//doesn't return null
			using FormEhrPatientSelectSimple FormP=new FormEhrPatientSelectSimple();
			if(String.IsNullOrEmpty(pat.LName) && String.IsNullOrEmpty(pat.FName)) {
				//patient could not be located by patnum, so use parsed name
				FormP.LName=lName;
				FormP.FName=fName;
			}
			else {
				FormP.LName=pat.LName;
				FormP.FName=pat.FName;
			}
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK) {
				return;
			}
			patNum=FormP.SelectedPatNum;
			FillPatAndGrid();
		}

		private void CreateLabPanel() {
			MedicalOrder order=listLabOrders[gridMain.GetSelectedIndex()];
			MessageHL7 msg=new MessageHL7(textHL7Raw.Text);
			//SegmentHL7 segOBR=null;
			//SegmentHL7 segOBX=null;
			//int idxPanel=0;
			//int idxResult=0;
			LabPanel panel=null;
			LabResult result=null;
			//loop through all message segments.
			for(int i=0;i<msg.Segments.Count;i++){
				if(msg.Segments[i].Name==SegmentNameHL7.OBR){//if this is the start of a new panel
					panel=new LabPanel();
					panel.PatNum=order.PatNum;
					panel.MedicalOrderNum=order.MedicalOrderNum;
					panel.RawMessage=textHL7Raw.Text;
					panel.LabNameAddress=msg.Segments[i].GetFieldFullText(20);
					panel.SpecimenSource=msg.Segments[i].GetFieldFullText(15);
					panel.SpecimenCondition=msg.Segments[i].GetFieldFullText(13);
					panel.ServiceId=msg.Segments[i].GetFieldComponent(4,0);
					panel.ServiceName=msg.Segments[i].GetFieldComponent(4,1);
					LabPanels.Insert(panel);
				}
				if(msg.Segments[i].Name==SegmentNameHL7.OBX){//if this is a result within a panel
					result=new LabResult();
					result.LabPanelNum=panel.LabPanelNum;
					result.DateTimeTest=msg.Segments[i].GetDateTime(14);
					result.TestID=msg.Segments[i].GetFieldComponent(3,0);
					result.TestName=msg.Segments[i].GetFieldComponent(3,1);
					result.ObsValue=msg.Segments[i].GetFieldFullText(5);
					result.ObsUnits=msg.Segments[i].GetFieldFullText(6);
					result.ObsRange=msg.Segments[i].GetFieldFullText(7);
					LabResults.Insert(result);
				}
				//any other kind of segment, continue.
			}
			//order.IsLabPending=false;
			//MedicalOrders.Update(order);
			//return true;//I guess it's always true?
		}

		private void butOk_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MessageBox.Show("Please select a lab order first.");
				return;
			}
			//if(!CreateLabPanel()) {
			//	return;
			//}
			CreateLabPanel();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	
	

		

		

		

	}
}
