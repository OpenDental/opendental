using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEtrans835PickEob:FormODBase {

		private string _messageText835;
		private List<string> _listEobTranIds;
		private Etrans _etrans;
		///<summary>When true, double clicking an EOB in the grid will not open FormEtrans835Edit. It just sets the TransSetIdSelected for use outside the form.</summary>
		private bool _doOpenEtrans835;
		///<summary>The TransSetId for the selected EOB.</summary>
		public string TransSetIdSelected=null;

		public FormEtrans835PickEob(List<string> listEobTranIds,string messageText835,Etrans etrans,bool doOpenEtrans835) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listEobTranIds=listEobTranIds;
			_messageText835=messageText835;
			_etrans=etrans;
			_doOpenEtrans835=doOpenEtrans835;
		}
		
		private void FormEtrans835PickEob_Load(object sender,EventArgs e) {
			FillGridEobs();
		}

		private void FillGridEobs() {
			gridEobs.BeginUpdate();
			gridEobs.Columns.Clear();
			UI.GridColumn col=new UI.GridColumn("",20);
			col.IsWidthDynamic=true;
			gridEobs.Columns.Add(col);
			gridEobs.ListGridRows.Clear();
			for(int i=0;i<_listEobTranIds.Count;i++) {
				UI.GridRow row=new UI.GridRow();
				row.Cells.Add(_listEobTranIds[i]);
				gridEobs.ListGridRows.Add(row);
			}
			gridEobs.EndUpdate();
		}

		private void gridEobs_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			TransSetIdSelected=_listEobTranIds[gridEobs.SelectedIndices[0]];
			if(!_doOpenEtrans835) {
				DialogResult=DialogResult.OK;
				Close();
				return;
			}
			FormEtrans835Edit formEtrans835Edit=new FormEtrans835Edit();
			formEtrans835Edit.EtransCur=_etrans;
			formEtrans835Edit.MessageText835=_messageText835;
			formEtrans835Edit.TranSetId835=TransSetIdSelected;
			formEtrans835Edit.Show();//Not attached to parent window because the user may have to close parent window to navigate other areas of the program.
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}