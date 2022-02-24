using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Xml;

namespace OpenDental {
	public partial class FormOIDRegistryInternal:FormODBase {
		private List<OIDInternal> _listOIDInternal;
		///<summary>Used for refence to construct recommended values.</summary>
		private string _rootOIDString;

		public FormOIDRegistryInternal() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormReminders_Load(object sender,EventArgs e) {
			_listOIDInternal=OIDInternals.GetAll();
			_listOIDInternal.Sort(sortOIDsByIDType);
			_rootOIDString=OIDInternals.GetForType(IdentifierType.Root).IDRoot;
			FillGrid();
		}

		private void FillGrid() {
			if(_rootOIDString=="") {
				labelRetrieveStatus.ForeColor=System.Drawing.Color.Red;
				labelRetrieveStatus.Text="There is no OID root stored.  It is recommended that you press the 'Retrieve OIDs' button.";
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Type",100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Recommended Value",220);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Actual Value",220);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listOIDInternal.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listOIDInternal[i].IDType.ToString());
				if(_listOIDInternal[i].IDType==IdentifierType.Root) {
					row.Cells.Add("Press 'Retrieve OIDs' or see manual");
				}
				else {
					//recommended value is root+.1 through root+.4 (will grow as the enum is expanded)
					row.Cells.Add(_rootOIDString+"."+i.ToString());//adds the .1, .2, .3, and .4 (etc...) to the root
				}
				row.Cells.Add(_listOIDInternal[i].IDRoot);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using InputBox ipb=new InputBox("Actual OID");
			ipb.textResult.Text=_listOIDInternal[e.Row].IDRoot;
			ipb.ShowDialog();
			if(ipb.DialogResult!=DialogResult.OK) {
				return;
			}
			if(e.Row==0) {
				_rootOIDString=ipb.textResult.Text;
			}
			_listOIDInternal[e.Row].IDRoot=ipb.textResult.Text;
			FillGrid();
		}

		private int sortOIDsByIDType(OIDInternal a,OIDInternal b) {
			if(a.IDType>b.IDType) {
				return 1;
			}
			if(a.IDType<b.IDType) {
				return -1;
			}
			return 0;//should never happen.
		}

		private void butRetrieveOIDs_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			//prepare the xml document to send--------------------------------------------------------------------------------------
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars=("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,settings)) {
				writer.WriteStartElement("CustomerIdRequest");
				writer.WriteStartElement("RegistrationKey");
				writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
				writer.WriteEndElement();
				writer.WriteStartElement("RegKeyDisabledOverride");
				writer.WriteString("true");
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
#if DEBUG
			OpenDental.localhost.Service1 OIDService=new OpenDental.localhost.Service1();
#else
			OpenDental.customerUpdates.Service1 OIDService=new OpenDental.customerUpdates.Service1();
			OIDService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
			//Send the message and get the result-------------------------------------------------------------------------------------
			string result="";
			try {
				result=OIDService.RequestCustomerID(strbuild.ToString());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show("Error: "+ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			//Process errors------------------------------------------------------------------------------------------------------------
			XmlNode node=doc.SelectSingleNode("//Error");
			if(node!=null) {
				MessageBox.Show(node.InnerText,"Error");
				return;
			}
			//Process a valid return value------------------------------------------------------------------------------------------------
			node=doc.SelectSingleNode("//CustomerIdResponse");
			if(node==null) {
				MsgBox.Show(this,"There was an error requesting your OID or processing the result of the request.  Please try again.");
				return;
			}
			if(node.InnerText=="") {
				labelRetrieveStatus.Text="";
				MsgBox.Show(this,"Invalid registration key.  Your OIDs will have to be set manually.");
				return;
			}
			//CustomerIdResponse has been returned and is not blank, use it for the root displayed as the recommended value
			_rootOIDString="2.16.840.1.113883.3.4337.1486."+node.InnerText;
			labelRetrieveStatus.Text="Connection successful.  Root OID has been retrieved.";
			labelRetrieveStatus.ForeColor=System.Drawing.Color.Black;
			//we will overwrite their root value with the returned value, but we will ask if they want to overwrite all of their other values in case they have manually set them
			_listOIDInternal[0].IDRoot=_rootOIDString;
			bool customOIDsExist=false;
			for(int i=1;i<_listOIDInternal.Count;i++) {//start with 1, root is 0 and is updated above
				if(_listOIDInternal[i].IDRoot!="" && _listOIDInternal[i].IDRoot!=_rootOIDString+"."+i.ToString()) {//if any of the OIDs other than root are filled and not what we are going to fill it with using the root with CustomerID, we will be overwriting their manually entered data, ask first
					customOIDsExist=true;
					break;
				}
			}
			//ask if they want to overwrite the current actual values with the recommended values
			if(customOIDsExist && !MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to update all OIDs using the root provided by Open Dental?")) {
				FillGrid();
				return;
			}
			for(int i=1;i<_listOIDInternal.Count;i++) {//start with 1, root is 0 and is updated above
				_listOIDInternal[i].IDRoot=_rootOIDString+"."+i.ToString();//adds the .1, .2, .3, and .4 to the root
				//database updated on OK click
			}
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			for(int i=0;i<_listOIDInternal.Count;i++) {
				OIDInternals.Update(_listOIDInternal[i]);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}



	}
}
