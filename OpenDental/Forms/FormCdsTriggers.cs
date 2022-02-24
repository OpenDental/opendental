using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using OpenDentBusiness;
using CodeBase;
using System.Globalization;
using System.Xml.XPath;
using System.IO;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormCdsTriggers:FormODBase {
		public List<EhrTrigger> ListEhrTriggers;

		public FormCdsTriggers() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEhrTriggers_Load(object sender,EventArgs e) {
			LayoutMenu();
			menuMain.Enabled=false;
			butAddTrigger.Enabled=false;
			gridMain.Enabled=false;
			if(CDSPermissions.GetForUser(Security.CurUser.UserNum).SetupCDS || Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
				menuMain.Enabled=true;
				butAddTrigger.Enabled=true;
				gridMain.Enabled=true;
			}
			FillGrid();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSettings_Click));
			menuMain.EndUpdate();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableCDSTriggers","Description"),200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableCDSTriggers","Cardinality"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableCDSTriggers","Trigger Categories"),200);
			gridMain.ListGridColumns.Add(col);
			ListEhrTriggers=EhrTriggers.GetAll();
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListEhrTriggers.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ListEhrTriggers[i].Description);
				row.Cells.Add(ListEhrTriggers[i].Cardinality.ToString());
				row.Cells.Add(ListEhrTriggers[i].GetTriggerCategories());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAddTrigger_Click(object sender,EventArgs e) {
			using FormCdsTriggerEdit FormETE=new FormCdsTriggerEdit();
			FormETE.EhrTriggerCur=new EhrTrigger();
			FormETE.IsNew=true;
			FormETE.ShowDialog();
			if(FormETE.DialogResult!=DialogResult.OK) {
				return;
			}
			ListEhrTriggers=EhrTriggers.GetAll();
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormCdsTriggerEdit FormETE=new FormCdsTriggerEdit();
			FormETE.EhrTriggerCur=ListEhrTriggers[e.Row];
			FormETE.ShowDialog();
			if(FormETE.DialogResult!=DialogResult.OK) {
				return;
			}
			ListEhrTriggers=EhrTriggers.GetAll();
			FillGrid();
		}

		private void menuItemSettings_Click(object sender,EventArgs e) {
			using FormCDSSetup FormCS=new FormCDSSetup();
			FormCS.ShowDialog();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormEhrTriggers_FormClosing(object sender,FormClosingEventArgs e) {
			EhrMeasureEvent measureEvent=new EhrMeasureEvent();
			measureEvent.DateTEvent=DateTime.Now;
			measureEvent.EventType=EhrMeasureEventType.ClinicalInterventionRules;
			measureEvent.MoreInfo=Lan.g(this,"Triggers currently enabled")+": "+ListEhrTriggers.Count;
			EhrMeasureEvents.Insert(measureEvent);
		}


	}
}