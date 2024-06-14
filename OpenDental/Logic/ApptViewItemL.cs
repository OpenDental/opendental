using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenDentBusiness;
using OpenDentBusiness.UI;

namespace OpenDental{
	public class ApptViewItemL{
		///<summary>Gets the 'visible' operatories for todays date with the currently selected appointment view.  This is strictly used when filtering the
		///waiting room for clinics.  Pass in null for the dailySched if this is a weekly view.</summary>
		public static List<Operatory> GetOpsForApptView(ApptView apptView,bool isWeekly,List<Schedule> listSchedulesDaily) {
			List<ApptViewItem> listApptViewItemsForCurView;
			List<Provider> listProvidersVis;
			List<Operatory> listOperatoriesVis;
			List<ApptViewItem> listApptViewItemsApptRows;
			int rowsPerIncr;
			FillForApptView(isWeekly,apptView,out listProvidersVis,out listOperatoriesVis,out listApptViewItemsForCurView,out listApptViewItemsApptRows,out rowsPerIncr,false);
			AddOpsForScheduledProvs(isWeekly,listSchedulesDaily,apptView,ref listOperatoriesVis);
			listOperatoriesVis.Sort(CompareOps);
			return listOperatoriesVis;
		}

		///<summary>Fills visProvs, visOps, forCurView, apptRows, and rowsPerIncr based on the appointment view passed in and whether it is for the week view or not.  This method uses 'out' variables so that the encompassing logic doesn't ALWAYS affect the global static variables used to draw the appointment views.  We don't want the following logic to affect the global static variables in the case where we are trying to get information needed to filter the waiting room.</summary>
		public static void FillForApptView(bool isWeekly,ApptView apptView,out List<Provider> listProvidersVis,out List<Operatory> listOperatoriesVis,
			out List<ApptViewItem> listApptViewItemsForCurView,out List<ApptViewItem> listApptViewItemsApptRows,out int rowsPerIncr,bool isFillVisProvs=true)
		{
			listApptViewItemsForCurView=new List<ApptViewItem>();
			listProvidersVis=new List<Provider>();
			listOperatoriesVis=new List<Operatory>();
			listApptViewItemsApptRows=new List<ApptViewItem>();
			//If there are no appointment views set up (therefore, none selected), then use a hard-coded default view.
			if(ApptViews.IsNoneView(apptView)) {
				//make visible ops exactly the same as the short ops list (all except hidden)
				listOperatoriesVis.AddRange(
					Operatories.GetWhere(x => !PrefC.HasClinicsEnabled //if clinics disabled
							|| Clinics.ClinicNum==0 //or if program level ClinicNum set to Headquarters
							|| x.ClinicNum==Clinics.ClinicNum //or this is the program level ClinicNum
						,true)
				);
				if(isFillVisProvs) {
					if(PrefC.HasClinicsEnabled) {
						for(int o=0;o<listOperatoriesVis.Count;o++){
							Provider providerDent=Providers.GetProv(listOperatoriesVis[o].ProvDentist);
							Provider providerHyg=Providers.GetProv(listOperatoriesVis[o].ProvHygienist);
							if(providerDent!=null) {
								listProvidersVis.Add(providerDent);
							}
							if(providerHyg!=null) {
								listProvidersVis.Add(providerHyg);
							}
						}
					}
					else {
						//make visible provs exactly the same as the prov list (all except hidden)
						listProvidersVis.AddRange(Providers.GetDeepCopy(true));
					}
				}
				//Hard coded elements showing
				listApptViewItemsApptRows.Add(new ApptViewItem("PatientName",0,Color.Black));
				listApptViewItemsApptRows.Add(new ApptViewItem("ASAP",1,Color.DarkRed));
				listApptViewItemsApptRows.Add(new ApptViewItem("MedUrgNote",2,Color.DarkRed));
				listApptViewItemsApptRows.Add(new ApptViewItem("PremedFlag",3,Color.DarkRed));
				listApptViewItemsApptRows.Add(new ApptViewItem("Lab",4,Color.DarkRed));
				listApptViewItemsApptRows.Add(new ApptViewItem("Procs",5,Color.Black));
				listApptViewItemsApptRows.Add(new ApptViewItem("Note",6,Color.Black));
				rowsPerIncr=1;
			}
			//An appointment view is selected, so add provs and ops from the view to our lists of indexes.
			else {
				List<ApptViewItem> listApptViewItems=ApptViewItems.GetWhere(x => x.ApptViewNum==apptView.ApptViewNum && !x.IsMobile);
				for(int i=0;i<listApptViewItems.Count;i++) {
					listApptViewItemsForCurView.Add(listApptViewItems[i]);
					if(listApptViewItems[i].OpNum>0) {//op
						if(apptView.OnlyScheduledProvs && !isWeekly) {
							continue;//handled below in AddOpsForScheduledProvs 
						}
						Operatory operatory=Operatories.GetFirstOrDefault(x => x.OperatoryNum==listApptViewItems[i].OpNum,true);
						if(operatory!=null) {
							listOperatoriesVis.Add(operatory);
						}
					}
					else if(listApptViewItems[i].ProvNum>0) {//prov
						if(!isFillVisProvs) {
							continue;
						}
						Provider provider=Providers.GetFirstOrDefault(x => x.ProvNum==listApptViewItems[i].ProvNum,true);
						if(provider!=null) {
							listProvidersVis.Add(provider);
						}
					}
					else {//element or apptfielddef
						listApptViewItemsApptRows.Add(listApptViewItems[i]);
					}
				}
				rowsPerIncr=apptView.RowsPerIncr;
			}
			//Remove any duplicates before return.
			listOperatoriesVis=listOperatoriesVis.GroupBy(x => x.OperatoryNum).Select(x => x.First()).ToList();
			if(isFillVisProvs) {
				listProvidersVis=listProvidersVis.GroupBy(x => x.ProvNum).Select(x => x.First()).ToList();
			}
		}

		///<summary>When looking at a daily appointment module and the current appointment view is has 'OnlyScheduleProvs' turned on, this method will dynamically add additional operatories to visOps for providers that are scheduled to work.</summary>
		public static void AddOpsForScheduledProvs(bool isWeekly,List<Schedule> listSchedulesDaily,ApptView apptView,ref List<Operatory> listOperatoriesVis) {
			//if this appt view has the option to show only scheduled providers and this is daily view.
			//Remember that there is no intelligence in weekly view for this option, and it behaves just like it always did.
			if(ApptViews.IsNoneView(apptView) 
				|| listSchedulesDaily==null
				|| listOperatoriesVis==null
				|| !apptView.OnlyScheduledProvs
				|| isWeekly) 
			{
				return;
			}
			//intelligently decide what ops to show.  It's based on the schedule for the day.
			//visOps will be totally empty right now because it looped out of the above section of code.
			List<long> listOperatoryNumsSched;
			bool opAdded;
			int indexOperatory;
			List<Operatory> listOperatoriesShort=Operatories.GetDeepCopy(true);
			List<long> listOperatoryNums=ApptViewItems.GetOpsForView(apptView.ApptViewNum);
			for(int i=0;i<listOperatoriesShort.Count;i++) {//loop through all ops for all views (except the hidden ones, of course)
				//If this operatory was not one of the selected Ops from the Appt View Edit window, skip it.
				if(!listOperatoryNums.Contains(listOperatoriesShort[i].OperatoryNum)) {
					continue;
				}
				//find any applicable sched for the op
				opAdded=false;
				for(int s=0;s<listSchedulesDaily.Count;s++) {
					if(listSchedulesDaily[s].SchedType!=ScheduleType.Provider) {
						continue;
					}
					if(listSchedulesDaily[s].StartTime==new TimeSpan(0)) {//skip if block starts at midnight.
						continue;
					}
					if(listSchedulesDaily[s].StartTime==listSchedulesDaily[s].StopTime) {//skip if block has no length.
						continue;
					}
					if(apptView.OnlySchedAfterTime > new TimeSpan(0,0,0)) {
						if(listSchedulesDaily[s].StartTime < apptView.OnlySchedAfterTime
								|| listSchedulesDaily[s].StopTime < apptView.OnlySchedAfterTime) 
						{
							continue;
						}
					}
					if(apptView.OnlySchedBeforeTime > new TimeSpan(0,0,0)) {
						if(listSchedulesDaily[s].StartTime > apptView.OnlySchedBeforeTime
								|| listSchedulesDaily[s].StopTime > apptView.OnlySchedBeforeTime) 
						{
							continue;
						}
					}
					//this 'sched' must apply to this situation.
					//listSchedOps is the ops for this 'sched'.
					listOperatoryNumsSched=listSchedulesDaily[s].Ops;
					//Add all the ops for this 'sched' to the list of visible ops
					for(int p=0;p<listOperatoryNumsSched.Count;p++) {
						//Filter the ops if the clinic option was set for the appt view.
						if(apptView.ClinicNum>0 && apptView.ClinicNum!=Operatories.GetOperatory(listOperatoryNumsSched[p]).ClinicNum) {
							continue;
						}
						if(listOperatoryNumsSched[p]==listOperatoriesShort[i].OperatoryNum) {
							Operatory operatory=listOperatoriesShort[i];
							indexOperatory=Operatories.GetOrder(listOperatoryNumsSched[p]);
							if(indexOperatory!=-1 && !listOperatoriesVis.Contains(operatory)) {//prevents adding duplicate ops
								listOperatoriesVis.Add(operatory);
								opAdded=true;
								break;
							}
						}
					}
					//If the provider is not scheduled to any op(s), add their default op(s).
					if(listOperatoriesShort[i].ProvDentist==listSchedulesDaily[s].ProvNum && listOperatoryNumsSched.Count==0) {//only if the sched does not specify any ops
						//Only add the op if the clinic option was not set in the appt view or if the op is assigned to that clinic.
						if(apptView.ClinicNum==0 || apptView.ClinicNum==listOperatoriesShort[i].ClinicNum) {
							indexOperatory=Operatories.GetOrder(listOperatoriesShort[i].OperatoryNum);
							if(indexOperatory!=-1 && !listOperatoriesVis.Contains(listOperatoriesShort[i])) {
								listOperatoriesVis.Add(listOperatoriesShort[i]);
								opAdded=true;
							}
						}
					}
					if(opAdded) {
						break;//break out of the loop of schedules.  Continue with the next op.
					}
				}
			}
			//Remove any duplicates before return.
			listOperatoriesVis=listOperatoriesVis.GroupBy(x => x.OperatoryNum).Select(x => x.First()).ToList();
		}

		///<summary>Sorts list of operatories by ItemOrder.</summary>
		public static int CompareOps(Operatory operatory1,Operatory operatory2) {
			if(operatory1.ItemOrder<operatory2.ItemOrder) {
				return -1;
			}
			if(operatory1.ItemOrder>operatory2.ItemOrder) {
				return 1;
			}
			return 0;
		}

		///<summary>Sorts list of providers by ItemOrder.</summary>
		public static int CompareProvs(Provider provider1,Provider provider2) {
			if(provider1.ItemOrder<provider2.ItemOrder) {
				return -1;
			}
			if(provider1.ItemOrder>provider2.ItemOrder) {
				return 1;
			}
			return 0;
		}

		

	



	}
}
