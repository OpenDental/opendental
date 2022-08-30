using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Web;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEtrans270EBraw:FormODBase {
		public EB271 EB271Cur;

		public FormEtrans270EBraw() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEtrans270EBraw_Load(object sender,EventArgs e) {
			string rawText=EB271Cur.ToString();
			if(rawText.Contains("%")) {
				rawText=X12Parse.UrlDecode(rawText).ToLower();
				//url detection depends on a few strategically placed spaces
				rawText=rawText.Replace("http"," http");
				rawText=rawText.Replace("~"," ~");
			}
			textRaw.Text=rawText;
			FillGrid();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEtrans270EBraw","#"),50);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormEtrans270EBraw","Raw"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormEtrans270EBraw","Description"),150);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=1;i<14;i++) {
				row=new GridRow();
				row.Cells.Add(i.ToString());
				row.Cells.Add(EB271Cur.Segment.Get(i));
				row.Cells.Add(EB271Cur.GetDescript(i));
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void textRaw_LinkClicked(object sender,LinkClickedEventArgs e) {
			Process.Start(e.LinkText);
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		
	}
}