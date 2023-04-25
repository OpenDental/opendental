using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormElectIDEdit:FormODBase {

		///<summary>Must be set before calling Show() or ShowDialog().</summary>
		public ElectID ElectIDCur;

		public FormElectIDEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormElectIDEdit_Load(object sender,EventArgs e) {
			textPayerID.Text=ElectIDCur.PayorID;
			textCarrierName.Text=ElectIDCur.CarrierName;
			textComments.Text=ElectIDCur.Comments;
			checkIsMedicaid.Checked=ElectIDCur.IsMedicaid;
			textPayerService.Text=ElectIDCur.CommBridge.ToString();
			if(ElectIDCur.CommBridge!=EclaimsCommBridge.None) {
				textPayerID.ReadOnly=true;
				textCarrierName.ReadOnly=true;
			}
			gridAttributes.BeginUpdate();
			gridAttributes.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableApptProcs","Attribute"),190);
			gridAttributes.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptProcs","Value"),40,HorizontalAlignment.Center);
			gridAttributes.Columns.Add(col);
			FillAttributes();
			gridAttributes.EndUpdate();
		}

		private void FillAttributes() {
			GridRow row;
			EclaimsCommBridge eclaimsCommBridge=ElectIDCur.CommBridge;
			List<long> listAttributes=ElectIDCur.Attributes.Split(",",StringSplitOptions.RemoveEmptyEntries).Select(x=>PIn.Long(x,hasExceptions:false)).ToList();
			switch(eclaimsCommBridge) {
				case EclaimsCommBridge.ClaimConnect:
					for(int i=0;i<Enum.GetValues(typeof(EnumClaimConnectPayerAttributes)).Length;i++) {
						row=new GridRow();
						row.Cells.Add(((EnumClaimConnectPayerAttributes)i).ToString());
						row.Cells.Add(listAttributes.Contains(i)?"X":"");
						gridAttributes.ListGridRows.Add(row);
					}
					break;
				case EclaimsCommBridge.EDS:
					for(int i=0;i<Enum.GetValues(typeof(EnumEDSPayerAttributes)).Length;i++) {
						row=new GridRow();
						row.Cells.Add(((EnumEDSPayerAttributes)i).ToString());
						string value=listAttributes.Contains(i)?"X":"";
						if((EnumEDSPayerAttributes)i==EnumEDSPayerAttributes.DefaultClaimTP) {
							value=listAttributes.Contains(i)?"ELEC":"PAPER";
						}
						if((EnumEDSPayerAttributes)i==EnumEDSPayerAttributes.PayerType) {
							value=listAttributes.Contains(i)?"D":"M";
						}
						row.Cells.Add(value);
						gridAttributes.ListGridRows.Add(row);
					}
					break;
				default:
					break;
			}
			return;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textPayerID.Text=="") {
				MsgBox.Show(this,"Payer ID cannot be blank.");
				return;
			}
			if(textCarrierName.Text=="") {
				MsgBox.Show(this,"Carrier name cannot be blank.");
				return;
			}
			ElectIDCur.PayorID=textPayerID.Text;
			ElectIDCur.CarrierName=textCarrierName.Text;
			ElectIDCur.Comments=textComments.Text;
			ElectIDCur.IsMedicaid=checkIsMedicaid.Checked;
			if(ElectIDCur.ElectIDNum==0) {
				ElectIDs.Insert(ElectIDCur);
			}
			else {
				ElectIDs.Update(ElectIDCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}