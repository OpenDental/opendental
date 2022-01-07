using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDentalGraph {
	///<summary>Provides a method to create a DashboardDockContainer. All controls that want to be available for docking in DashboardCellCtrl should implement this interface.</summary>
	public interface IDashboardDockContainer {
		DashboardDockContainer CreateDashboardDockContainer(OpenDentBusiness.TableBase dbItem);
		DashboardCellType GetCellType();
		string GetCellSettings();
	}

	///<summary>Helper class used by DashboardCellCtrl. Holds all necessary input needed for docking to DashboardCellCtrl.</summary>
	public class DashboardDockContainer {
		private Control _contr;
		private EventHandler _onEditClick;
		private EventHandler _onEditOk;
		private EventHandler _onEditCancel;
		private EventHandler _onDropComplete;
		private EventHandler _onRefreshCache;
		private IODGraphPrinter _printer;
		private OpenDentBusiness.TableBase _dbItem;
		public Control Contr {
			get { return _contr; }
		}
		public EventHandler OnEditClick {
			get { return _onEditClick; }
		}
		public EventHandler OnEditOk {
			get { return _onEditOk; }
		}
		public EventHandler OnEditCancel {
			get { return _onEditCancel; }
		}
		public EventHandler OnDropComplete {
			get { return _onDropComplete; }
		}
		public EventHandler OnRefreshCache {
			get { return _onRefreshCache; }
		}
		public IODGraphPrinter Printer {
			get { return _printer; }
		}
		public OpenDentBusiness.TableBase DbItem {
			get { return _dbItem; }
		}
		public DashboardDockContainer(
			Control c,
			IODGraphPrinter printer=null,
			EventHandler onEditClick = null,
			EventHandler onEditOk = null,
			EventHandler onEditCancel = null,
			EventHandler onDropComplete = null,
			EventHandler onRefreshCache = null,
			TableBase dbItem =null) {
			_contr=c;
			_printer=printer;
			_onEditClick=onEditClick;
			_onEditOk=onEditOk;
			_onEditCancel=onEditCancel;
			_onDropComplete=onDropComplete;
			_onRefreshCache=onRefreshCache;
			_dbItem=dbItem;
		}
	}
}
