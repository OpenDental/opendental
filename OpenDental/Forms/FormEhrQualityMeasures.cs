using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Drawing.Printing;
using OpenDental.UI;
using System.Xml;
using System.Xml.XPath;
using CodeBase;
#if EHRTEST
using EHR;
#endif

namespace OpenDental {
	public partial class FormEhrQualityMeasures:FormODBase {
		private List<QualityMeasure> listQ;
		private List<Provider> listProvsKeyed;
		private List<Provider> _listProviders;

		public FormEhrQualityMeasures() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormQualityMeasures_Load(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			listProvsKeyed=new List<Provider>();
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				string ehrKey="";
				int yearValue=0;
				List<EhrProvKey> listProvKeys=EhrProvKeys.GetKeysByFLName(_listProviders[i].LName,_listProviders[i].FName);
				if(listProvKeys.Count!=0) {
					ehrKey=listProvKeys[0].ProvKey;
					yearValue=listProvKeys[0].YearValue;
				}
				if(FormEHR.ProvKeyIsValid(_listProviders[i].LName,_listProviders[i].FName,yearValue,ehrKey)) {
					//EHR has been valid.
					listProvsKeyed.Add(_listProviders[i]);
				}
			}
			if(listProvsKeyed.Count==0) {
				Cursor=Cursors.Default;
				MessageBox.Show("No providers found with ehr keys.");
				return;
			}
			for(int i=0;i<listProvsKeyed.Count;i++) {
				comboProv.Items.Add(listProvsKeyed[i].GetLongDesc());
				if(Security.CurUser.ProvNum==listProvsKeyed[i].ProvNum) {
					comboProv.SelectedIndex=i;
				}
			}
			textDateStart.Text=(new DateTime(DateTime.Now.Year,1,1)).ToShortDateString();
			textDateEnd.Text=(new DateTime(DateTime.Now.Year,12,31)).ToShortDateString();
			FillGrid();
			Cursor=Cursors.Default;
		}

		private void FillGrid() {
			if(comboProv.SelectedIndex==-1) {
				return;
			}
			try {
				DateTime.Parse(textDateStart.Text);
				DateTime.Parse(textDateEnd.Text);
			}
			catch {
				return;
			}
			DateTime dateStart=PIn.Date(textDateStart.Text);
			DateTime dateEnd=PIn.Date(textDateEnd.Text);
			long provNum=listProvsKeyed[comboProv.SelectedIndex].ProvNum;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Id",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Description",200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Denom",60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Numerator",60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Exclusion",60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("NotMet",60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("PerformanceRate",110,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			listQ=QualityMeasures.GetAll(dateStart,dateEnd,provNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listQ.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listQ[i].Id);
				row.Cells.Add(listQ[i].Descript);
				row.Cells.Add(listQ[i].Denominator.ToString());
				row.Cells.Add(listQ[i].Numerator.ToString());
				row.Cells.Add(listQ[i].Exclusions.ToString());
				row.Cells.Add(listQ[i].NotMet.ToString());
				row.Cells.Add(listQ[i].Numerator.ToString()+"/"+(listQ[i].Numerator+listQ[i].NotMet).ToString()
					+"  = "+listQ[i].PerformanceRate.ToString()+"%");
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}
		
		public string GeneratePQRS_xml() {
			//provider and dates already validated at button push
			Provider prov=listProvsKeyed[comboProv.SelectedIndex];
			DateTime dateStart=PIn.Date(textDateStart.Text);
			DateTime dateEnd=PIn.Date(textDateEnd.Text);
			List<QualityType> typesToReport=new List<QualityType>();
			typesToReport.Add(QualityType.WeightOver65);
			typesToReport.Add(QualityType.Hypertension);
			typesToReport.Add(QualityType.TobaccoUse);
			typesToReport.Add(QualityType.InfluenzaAdult);
			typesToReport.Add(QualityType.WeightChild_1_1);
			typesToReport.Add(QualityType.ImmunizeChild_1);
			typesToReport.Add(QualityType.Pneumonia);
			typesToReport.Add(QualityType.DiabetesBloodPressure);
			typesToReport.Add(QualityType.BloodPressureManage);
			XmlWriterSettings xmlSettings=new XmlWriterSettings();
			xmlSettings.Encoding=Encoding.UTF8;
			xmlSettings.OmitXmlDeclaration=true;
			xmlSettings.Indent=true;
			xmlSettings.IndentChars="   ";
			StringBuilder strBuilder=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strBuilder,xmlSettings)){				                
				writer.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n");
				writer.WriteStartElement("submission");
						writer.WriteAttributeString("type","PQRI-REGISTRY");
						writer.WriteAttributeString("option","PAYMENT");
						writer.WriteAttributeString("version","2.0");
						writer.WriteAttributeString("xmlns","xsi",null,"http://www.w3.org/2001/XMLSchema-instance");
						writer.WriteAttributeString("xsi","noNamespaceSchemaLocation",null,"Registry_Payment.xsd");
					writer.WriteStartElement("file-audit-data");
						writer.WriteElementString("create-date",DateTime.Today.ToString("MM-dd-yyyy"));
						writer.WriteElementString("create-time",DateTime.Now.ToString("HH:mm"));//military time
						writer.WriteElementString("create-by","RegistryA");//many values are hard-coded for test because they don't explain them
						writer.WriteElementString("version","1.0");
						writer.WriteElementString("file-number","1");
						writer.WriteElementString("number-of-files","1");
					writer.WriteEndElement();//file-audit-data
					writer.WriteStartElement("registry");
						writer.WriteElementString("registry-name","Model Registry");
						writer.WriteElementString("registry-id","125789123");
						writer.WriteElementString("submission-method","A");
					writer.WriteEndElement();//registry
					writer.WriteStartElement("measure-group");
							writer.WriteAttributeString("ID","X");
						writer.WriteStartElement("provider");
							writer.WriteElementString("npi",prov.NationalProvID);
							writer.WriteElementString("tin",prov.SSN);
							writer.WriteElementString("waiver-signed","Y");
							writer.WriteElementString("encounter-from-date",dateStart.ToString("MM-dd-yyyy"));
							writer.WriteElementString("encounter-to-date",dateEnd.ToString("MM-dd-yyyy"));
							//measure-group-stat must be omitted because measure-group ID is X
							for(int i=0;i<listQ.Count;i++){
								if(!typesToReport.Contains(listQ[i].Type)){
									continue;
								}
								writer.WriteStartElement("pqri-measure");
									writer.WriteElementString("pqri-measure-number",QualityMeasures.GetPQRIMeasureNumber(listQ[i].Type));
									writer.WriteElementString("eligible-instances",listQ[i].Denominator.ToString());
									writer.WriteElementString("meets-performance-instances",listQ[i].Numerator.ToString());
									writer.WriteElementString("performance-exclusion-instances",listQ[i].Exclusions.ToString());
									writer.WriteElementString("performance-not-met-instances",listQ[i].NotMet.ToString());
									writer.WriteElementString("reporting-rate",listQ[i].ReportingRate.ToString("f2"));
									if(listQ[i].Denominator==0){//rate is null
										writer.WriteStartElement("performance-rate");
										writer.WriteAttributeString("xsi", "nil", System.Xml.Schema.XmlSchema.InstanceNamespace, "true");
										writer.WriteEndElement();
									}
									else{
										writer.WriteElementString("performance-rate",listQ[i].PerformanceRate.ToString("f2"));
									}
								writer.WriteEndElement();//pqri-measure
							}
						writer.WriteEndElement();//provider
					writer.WriteEndElement();//measure-group
				writer.WriteEndElement();//submission
			}
			return strBuilder.ToString();
		}

		///<summary>Launches edit window for double clicked item.</summary>
		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			try {
				DateTime.Parse(textDateStart.Text);
				DateTime.Parse(textDateEnd.Text);
			}
			catch {
				MessageBox.Show("Please fix dates first.");
				return;
			}
			using FormEhrQualityMeasureEdit formQe=new FormEhrQualityMeasureEdit();
			formQe.DateStart=PIn.Date(textDateStart.Text);
			formQe.DateEnd=PIn.Date(textDateEnd.Text);
			formQe.ProvNum=listProvsKeyed[comboProv.SelectedIndex].ProvNum;
			formQe.Qcur=listQ[e.Row];
			formQe.ShowDialog();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butShow_Click(object sender,EventArgs e) {
			if(comboProv.SelectedIndex==-1) {
				MessageBox.Show("Please select a provider first.");
				return;
			}
			try {
				DateTime.Parse(textDateStart.Text);
				DateTime.Parse(textDateEnd.Text);
			}
			catch {
				MessageBox.Show("Invalid dates.");
				return;
			}
			if(listQ==null) {
				MessageBox.Show("Click Refresh first.");
				return;
			}
			using MsgBoxCopyPaste MsgBoxCP = new MsgBoxCopyPaste(GeneratePQRS_xml());
			MsgBoxCP.ShowDialog();
		}

		private void butSubmit_Click(object sender,EventArgs e) {
			if(comboProv.SelectedIndex==-1) {
				MessageBox.Show("Please select a provider first.");
				return;
			}
			try {
				DateTime.Parse(textDateStart.Text);
				DateTime.Parse(textDateEnd.Text);
			}
			catch {
				MessageBox.Show("Invalid dates.");
				return;
			}
			if(listQ==null) {
				MessageBox.Show("Click Refresh first.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			try {
				EmailMessages.SendTestUnsecure("PQRI","pqri.xml",GeneratePQRS_xml());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			MessageBox.Show("Sent");
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}

	

	

		

	}
}
