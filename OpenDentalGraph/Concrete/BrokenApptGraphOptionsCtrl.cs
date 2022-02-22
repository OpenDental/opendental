using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDentalGraph {
	public partial class BrokenApptGraphOptionsCtrl:BaseGraphOptionsCtrl {

		private List<Def> _listAdjTypes=null;
		public List<Def> ListAdjTypes {
			get {
				if(_listAdjTypes==null) {
					_listAdjTypes=Defs.GetPositiveAdjTypes();
				}
				return _listAdjTypes;
			}
		}
		private List<BrokenApptProcedure> _listBrokenApptProcs=null;
		private List<BrokenApptProcedure> ListBrokenProcOptions {
			get {
				#region Init based on PrefName.BrokenApptProcedure
				if(_listBrokenApptProcs==null) {
					_listBrokenApptProcs=new List<BrokenApptProcedure>();
					switch((BrokenApptProcedure)PrefC.GetInt(PrefName.BrokenApptProcedure)) {
						case BrokenApptProcedure.None:
						case BrokenApptProcedure.Missed:
							_listBrokenApptProcs.Add(BrokenApptProcedure.Missed);
							break;
						case BrokenApptProcedure.Cancelled:
							_listBrokenApptProcs.Add(BrokenApptProcedure.Cancelled);
							break;
						case BrokenApptProcedure.Both:
							_listBrokenApptProcs.Add(BrokenApptProcedure.Missed);
							_listBrokenApptProcs.Add(BrokenApptProcedure.Cancelled);
							_listBrokenApptProcs.Add(BrokenApptProcedure.Both);
							break;
					}
				}
				#endregion
				return _listBrokenApptProcs;
			}
		}
		//public enum Grouping { provider, clinic };
		public enum RunFor { appointment, procedure, adjustment };
		public RunFor CurRunFor
		{
			get
			{
				if(radioRunAdjs.Checked) {
					return RunFor.adjustment;
				}
				else if(radioRunApts.Checked) {
					return RunFor.appointment;
				}
				else {
					return RunFor.procedure;
				}
			}
			set
			{
				switch(value) {
					case RunFor.adjustment:
						radioRunAdjs.Checked=true;
						break;
					case RunFor.appointment:
						radioRunApts.Checked=true;
						break;
					case RunFor.procedure:
						radioRunProcs.Checked=true;
						break;
				}
			}
		}

		public long AdjTypeDefNumCur
		{
			get
			{
				if(comboAdjType.SelectedIndex==-1) {
					return 0;
				}
				else {
					return ListAdjTypes[comboAdjType.SelectedIndex].DefNum;
				}
			}
			set
			{
				for(int i = 0;i<ListAdjTypes.Count;i++) {
					if(ListAdjTypes[i].DefNum==value) {
						comboAdjType.SelectedIndex=i;
						return;
					}
				}
			}
		}
		public BrokenApptProcedure BrokenApptCodeCur {
			get {
				if(comboBrokenProcType.SelectedIndex==-1) {
					return (BrokenApptProcedure)PrefC.GetInt(PrefName.BrokenApptProcedure);
				}
				return ListBrokenProcOptions[comboBrokenProcType.SelectedIndex];
			}
			set {
				for(int i=0;i<ListBrokenProcOptions.Count;i++) {
					if(ListBrokenProcOptions[i]==value) {
						comboBrokenProcType.SelectedIndex=i;
						return;
					}
				}
			}
		}

		public BrokenApptGraphOptionsCtrl() {
			InitializeComponent();
			FillComboAdj();
			FillComboBrokenProc();
		}

		///<summary>Because definitions don't exist in the eservices table, we should only get these if necessary on load.</summary>
		private void BrokenApptGraphOptionsCtrl_Load(object sender,EventArgs e) {
			
		}

		public override int GetPanelHeight() {
			return this.Height;
		}

		private void OnBrokenApptGraphOptionsChanged(object sender,EventArgs e) {
			comboAdjType.Enabled=radioRunAdjs.Checked;
			comboBrokenProcType.Enabled=radioRunProcs.Checked;
			OnBaseInputsChanged(sender,e);
		}

		private void FillComboAdj() {
			try { //Internal tools call this inadvertantly and don't have a connection to OD db. Swallow the exception.
				foreach(Def adjType in ListAdjTypes) {
					comboAdjType.Items.Add(adjType.ItemName);
				}
			} catch{ }			
			if(comboAdjType.Items.Count<=0) {
				comboAdjType.Items.Add(Lans.g(this,"Adj types not setup"));
				radioRunAdjs.Enabled=false;
			}
		}

		private void FillComboBrokenProc() {
			//Mimics FormRpBrokenAppointments.cs
			int index=0;
			BrokenApptProcedure brokenApptCodeDB=(BrokenApptProcedure)PrefC.GetInt(PrefName.BrokenApptProcedure);
			switch(brokenApptCodeDB) {
				case BrokenApptProcedure.None:
				case BrokenApptProcedure.Missed:
					index=comboBrokenProcType.Items.Add(Lans.g(this,brokenApptCodeDB.ToString())+": (D9986)");
					break;
				case BrokenApptProcedure.Cancelled:
					index=comboBrokenProcType.Items.Add(Lans.g(this,brokenApptCodeDB.ToString())+": (D9987)");
					break;
				case BrokenApptProcedure.Both:
					comboBrokenProcType.Items.Add(Lans.g(this,BrokenApptProcedure.Missed.ToString())+": (D9986)");
					comboBrokenProcType.Items.Add(Lans.g(this,BrokenApptProcedure.Cancelled.ToString())+": (D9987)");
					index=comboBrokenProcType.Items.Add(Lans.g(this,brokenApptCodeDB.ToString()));
					break;
			}
			comboBrokenProcType.SelectedIndex=index;
		}

	}
}
