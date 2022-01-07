using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;


namespace OpenDental {
	public partial class FormAlerts:FormODBase {
		#region Fields Public
		public AlertItem AlertItemCur;
		public ActionType ActionTypeCur;
		public List<AlertItem> ListAlertItems;
		public List<AlertRead> ListAlertReads;
		#endregion Fields Public

		#region Fields Private
		///<summary>Used to maintain selected griditem after button events.</summary>
		private int _gridNum;
		#endregion Fields Private

		#region Constructor
		public FormAlerts() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		#endregion Constructor

		#region Events - Raise
		protected void OnClick() {
			ClickAlert?.Invoke(this,new EventArgs());
		}

		public event EventHandler ClickAlert=null;
		#endregion Events - Raise

		#region Methods - Event Handlers
		private void butOpenForm_Click(object sender,EventArgs e) {
			ActionTypeCur=ActionType.OpenForm;
			Close();
			//Don't fire event. We want this form to close first.
		}

		private void FormAlerts_Load(object sender,EventArgs e) {
			labelOpenForm.Text="";
			FillGrid();
		}

		private void gridAlerts_CellClick(object sender,ODGridClickEventArgs e) {
			AlertItemCur=ListAlertItems[e.Row];
			_gridNum=e.Row;
			List<ActionType> listActionTypes=Enum.GetValues(typeof(ActionType)).Cast<ActionType>().ToList();
			listActionTypes.Sort(AlertItem.CompareActionType);
			butDelete.Enabled=false;
			butMarkAsRead.Enabled=false;
			butOpenForm.Enabled=false;
			butViewDetails.Enabled=false;
			labelOpenForm.Text="";
			foreach(AlertItem alertItemCur in ListAlertItems) {
				foreach(ActionType actionTypeCur in listActionTypes) {
					if(!AlertItemCur.Actions.HasFlag(actionTypeCur)){//Current AlertItem does not have this ActionType associated with it.
						continue;
					}
					if(actionTypeCur==ActionType.Delete) {
						butDelete.Enabled=true;
					}
					if(actionTypeCur==ActionType.MarkAsRead) {
						butMarkAsRead.Enabled=true;
					}
					if(actionTypeCur==ActionType.ShowItemValue) {
						butViewDetails.Enabled=true;
					}
					if(actionTypeCur==ActionType.OpenForm) {
						butOpenForm.Enabled=true;
						labelOpenForm.Text=Lan.g(this,AlertItemCur.FormToOpen.GetDescription())+" window.";
					}
				}
			}
		}

		private void butMarkAsRead_Click(object sender,EventArgs e) {
			ActionTypeCur=ActionType.MarkAsRead;
			OnClick();
			gridMain.SetSelected(_gridNum,true);
		}

		private void butViewDetails_Click(object sender,EventArgs e) {
			ActionTypeCur=ActionType.ShowItemValue;
			OnClick();
			gridMain.SetSelected(_gridNum,true);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			ActionTypeCur=ActionType.Delete;
			OnClick();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
		#endregion Methods - Event Handlers

		#region Methods - Public
		public void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableAlerts","Alert #"),45,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableAlerts","Read"),45,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableAlerts","Description"),0,HorizontalAlignment.Left);
			col.IsWidthDynamic=true;
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListAlertItems.Count;i++) {
				row=new GridRow();
				row.Cells.Add(Lan.g(this,(i+1).ToString()));
				if(ListAlertReads.Select(x=>x.AlertItemNum).Contains(ListAlertItems[i].AlertItemNum)) {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(AlertMenuItemHelper(ListAlertItems[i])+ListAlertItems[i].Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}
		#endregion Methods - Public

		#region Methods - Helpers
		///<summary>Helper function to translate the title for the given alertItem.</summary>
		private string AlertMenuItemHelper(AlertItem alertItem) {
			string value="";
			switch(alertItem.Type) {
				case AlertType.Generic:
				case AlertType.ClinicsChangedInternal:
					break;
				case AlertType.OnlinePaymentsPending:
					value+=Lan.g(this,"Pending Online Payments")+": ";
					break;
				case AlertType.VoiceMailMonitor:
					value+=Lan.g(this,"Voice Mail Monitor")+": ";
					break;
				case AlertType.RadiologyProcedures:
					value+=Lan.g(this,"Radiology Orders")+": ";
					break;
				case AlertType.CallbackRequested:
					value+=Lan.g(this,"Patient would like a callback regarding this appointment")+": ";
					break;
				case AlertType.WebSchedNewPat:
					value+=Lan.g(this,"eServices")+": ";
					break;
				case AlertType.WebSchedNewPatApptCreated:
					value+=Lan.g(this,"New Web Sched New Patient Appointment")+": ";
					break;
				case AlertType.MaxConnectionsMonitor:
					value+=Lan.g(this,"MySQL Max Connections")+": ";
					break;
				case AlertType.WebSchedASAPApptCreated:
					value+=Lan.g(this,"New Web Sched ASAP Appointment")+": ";
					break;
				case AlertType.AsteriskServerMonitor:
					value+=Lan.g(this,"Phone Tracking Server")+": ";
					break;
				case AlertType.WebSchedRecallApptCreated:
					value+=Lan.g(this,"New Web Sched Recall Appointment")+": ";
					break;
				case AlertType.WebMailRecieved:
					value+=Lan.g(this,"Unread Web Mails")+": ";
					break;
				case AlertType.WebFormsReady:
					value+=Lan.g(this,"Web Forms Ready to Retrieve")+": ";
					break;
				case AlertType.EconnectorEmailTooManySendFails:
				case AlertType.NumberBarredFromTexting:
				case AlertType.MultipleEConnectors:
				case AlertType.EConnectorDown:
				case AlertType.EConnectorError:
				case AlertType.DoseSpotProviderRegistered:
				case AlertType.DoseSpotClinicRegistered:
				case AlertType.ClinicsChanged:
				case AlertType.CloudAlertWithinLimit:
				default:
					value+=Lan.g(this,alertItem.Type.GetDescription())+": ";
					break;
			}
			return value;
		}
		#endregion Methods - Helpers

	}
}