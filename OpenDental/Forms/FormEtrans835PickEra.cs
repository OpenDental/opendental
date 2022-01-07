using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEtrans835PickEra:FormODBase {
		private List<Etrans> _listEtrans;
		private long _specificClaimNum;

		public FormEtrans835PickEra(List<Etrans> listEtrans,long specificClaimNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listEtrans=listEtrans;
			_specificClaimNum=specificClaimNum;
		}
		
		private void FormEtrans835PickEra_Load(object sender,EventArgs e) {
			FillGridEras();
		}

		private void FillGridEras() {
			gridEras.BeginUpdate();
			gridEras.ListGridColumns.Clear();
			gridEras.ListGridColumns.Add(new UI.GridColumn(Lan.g(this,"Carrier"),200));
			gridEras.ListGridColumns.Add(new UI.GridColumn(Lan.g(this,"Date Rec'd"),70,HorizontalAlignment.Center));
			gridEras.ListGridColumns.Add(new UI.GridColumn(Lan.g(this,"Note"),70){ IsWidthDynamic=true });
			gridEras.ListGridRows.Clear();
			for(int i=0;i<_listEtrans.Count;i++) {
				UI.GridRow row=new UI.GridRow();
				row.Cells.Add(_listEtrans[i].CarrierNameRaw);
				row.Cells.Add(_listEtrans[i].DateTimeTrans.ToShortDateString());
				row.Cells.Add(_listEtrans[i].Note);
				gridEras.ListGridRows.Add(row);
			}
			gridEras.EndUpdate();
		}

		private void gridEras_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			EtransL.ViewFormForEra(_listEtrans[gridEras.SelectedIndices[0]],this,_specificClaimNum);
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}