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
		public static List<Operatory> GetOpsForApptView(ApptView apptViewCur,bool isWeekly,List<Schedule> dailySched) {
			List<ApptViewItem> forCurView;
			List<Provider> visProvs;
			List<Operatory> visOps;
			List<ApptViewItem> apptRows;
			int rowsPerIncr;
			FillForApptView(isWeekly,apptViewCur,out visProvs,out visOps,out forCurView,out apptRows,out rowsPerIncr,false);
			AddOpsForScheduledProvs(isWeekly,dailySched,apptViewCur,ref visOps);
			visOps.Sort(CompareOps);
			return visOps;
		}

		///<summary>Fills visProvs, visOps, forCurView, apptRows, and rowsPerIncr based on the appointment view passed in and whether it is for the week view or not.  This method uses 'out' variables so that the encompassing logic doesn't ALWAYS affect the global static variables used to draw the appointment views.  We don't want the following logic to affect the global static variables in the case where we are trying to get information needed to filter the waiting room.</summary>
		public static void FillForApptView(bool isWeekly,ApptView apptViewCur,out List<Provider> visProvs,out List<Operatory> visOps,
			out List<ApptViewItem> forCurView,out List<ApptViewItem> apptRows,out int rowsPerIncr,bool isFillVisProvs=true)
		{
			forCurView=new List<ApptViewItem>();
			visProvs=new List<Provider>();
			visOps=new List<Operatory>();
			apptRows=new List<ApptViewItem>();
			//If there are no appointment views set up (therefore, none selected), then use a hard-coded default view.
			if(ApptViews.IsNoneView(apptViewCur)) {
				//make visible ops exactly the same as the short ops list (all except hidden)
				visOps.AddRange(
					Operatories.GetWhere(x => !PrefC.HasClinicsEnabled //if clinics disabled
							|| Clinics.ClinicNum==0 //or if program level ClinicNum set to Headquarters
							|| x.ClinicNum==Clinics.ClinicNum //or this is the program level ClinicNum
						,true)
				);
				if(isFillVisProvs) {
					if(PrefC.HasClinicsEnabled) {
						foreach(Operatory op in visOps) {
							Provider provDent=Providers.GetProv(op.ProvDentist);
							Provider provHyg=Providers.GetProv(op.ProvHygienist);
							if(provDent!=null) {
								visProvs.Add(provDent);
							}
							if(provHyg!=null) {
								visProvs.Add(provHyg);
							}
						}
					}
					else {
						//make visible provs exactly the same as the prov list (all except hidden)
						visProvs.AddRange(Providers.GetDeepCopy(true));
					}
				}
				//Hard coded elements showing
				apptRows.Add(new ApptViewItem("PatientName",0,Color.Black));
				apptRows.Add(new ApptViewItem("ASAP",1,Color.DarkRed));
				apptRows.Add(new ApptViewItem("MedUrgNote",2,Color.DarkRed));
				apptRows.Add(new ApptViewItem("PremedFlag",3,Color.DarkRed));
				apptRows.Add(new ApptViewItem("Lab",4,Color.DarkRed));
				apptRows.Add(new ApptViewItem("Procs",5,Color.Black));
				apptRows.Add(new ApptViewItem("Note",6,Color.Black));
				rowsPerIncr=1;
			}
			//An appointment view is selected, so add provs and ops from the view to our lists of indexes.
			else {
				List<ApptViewItem> listApptViewItems=ApptViewItems.GetWhere(x => x.ApptViewNum==apptViewCur.ApptViewNum);
				for(int i=0;i<listApptViewItems.Count;i++) {
					forCurView.Add(listApptViewItems[i]);
					if(listApptViewItems[i].OpNum>0) {//op
						if(apptViewCur.OnlyScheduledProvs && !isWeekly) {
							continue;//handled below in AddOpsForScheduledProvs 
						}
						Operatory op=Operatories.GetFirstOrDefault(x => x.OperatoryNum==listApptViewItems[i].OpNum,true);
						if(op!=null) {
							visOps.Add(op);
						}
					}
					else if(listApptViewItems[i].ProvNum>0) {//prov
						if(!isFillVisProvs) {
							continue;
						}
						Provider prov=Providers.GetFirstOrDefault(x => x.ProvNum==listApptViewItems[i].ProvNum,true);
						if(prov!=null) {
							visProvs.Add(prov);
						}
					}
					else {//element or apptfielddef
						apptRows.Add(listApptViewItems[i]);
					}
				}
				rowsPerIncr=apptViewCur.RowsPerIncr;
			}
			//Remove any duplicates before return.
			visOps=visOps.GroupBy(x => x.OperatoryNum).Select(x => x.First()).ToList();
			if(isFillVisProvs) {
				visProvs=visProvs.GroupBy(x => x.ProvNum).Select(x => x.First()).ToList();
			}
		}

		///<summary>When looking at a daily appointment module and the current appointment view is has 'OnlyScheduleProvs' turned on, this method will dynamically add additional operatories to visOps for providers that are scheduled to work.</summary>
		public static void AddOpsForScheduledProvs(bool isWeekly,List<Schedule> dailySched,ApptView apptViewCur,ref List<Operatory> visOps) {
			//if this appt view has the option to show only scheduled providers and this is daily view.
			//Remember that there is no intelligence in weekly view for this option, and it behaves just like it always did.
			if(ApptViews.IsNoneView(apptViewCur) 
				|| dailySched==null
				|| visOps==null
				|| !apptViewCur.OnlyScheduledProvs
				|| isWeekly) 
			{
				return;
			}
			//intelligently decide what ops to show.  It's based on the schedule for the day.
			//visOps will be totally empty right now because it looped out of the above section of code.
			List<long> listSchedOps;
			bool opAdded;
			int indexOp;
			List<Operatory> listOpsShort=Operatories.GetDeepCopy(true);
			List<long> listApptViewOpNums=ApptViewItems.GetOpsForView(apptViewCur.ApptViewNum);
			for(int i=0;i<listOpsShort.Count;i++) {//loop through all ops for all views (except the hidden ones, of course)
				//If this operatory was not one of the selected Ops from the Appt View Edit window, skip it.
				if(!listApptViewOpNums.Contains(listOpsShort[i].OperatoryNum)) {
					continue;
				}
				//find any applicable sched for the op
				opAdded=false;
				for(int s=0;s<dailySched.Count;s++) {
					if(dailySched[s].SchedType!=ScheduleType.Provider) {
						continue;
					}
					if(dailySched[s].StartTime==new TimeSpan(0)) {//skip if block starts at midnight.
						continue;
					}
					if(dailySched[s].StartTime==dailySched[s].StopTime) {//skip if block has no length.
						continue;
					}
					if(apptViewCur.OnlySchedAfterTime > new TimeSpan(0,0,0)) {
						if(dailySched[s].StartTime < apptViewCur.OnlySchedAfterTime
								|| dailySched[s].StopTime < apptViewCur.OnlySchedAfterTime) 
						{
							continue;
						}
					}
					if(apptViewCur.OnlySchedBeforeTime > new TimeSpan(0,0,0)) {
						if(dailySched[s].StartTime > apptViewCur.OnlySchedBeforeTime
								|| dailySched[s].StopTime > apptViewCur.OnlySchedBeforeTime) 
						{
							continue;
						}
					}
					//this 'sched' must apply to this situation.
					//listSchedOps is the ops for this 'sched'.
					listSchedOps=dailySched[s].Ops;
					//Add all the ops for this 'sched' to the list of visible ops
					for(int p=0;p<listSchedOps.Count;p++) {
						//Filter the ops if the clinic option was set for the appt view.
						if(apptViewCur.ClinicNum>0 && apptViewCur.ClinicNum!=Operatories.GetOperatory(listSchedOps[p]).ClinicNum) {
							continue;
						}
						if(listSchedOps[p]==listOpsShort[i].OperatoryNum) {
							Operatory op=listOpsShort[i];
							indexOp=Operatories.GetOrder(listSchedOps[p]);
							if(indexOp!=-1 && !visOps.Contains(op)) {//prevents adding duplicate ops
								visOps.Add(op);
								opAdded=true;
								break;
							}
						}
					}
					//If the provider is not scheduled to any op(s), add their default op(s).
					if(listOpsShort[i].ProvDentist==dailySched[s].ProvNum && listSchedOps.Count==0) {//only if the sched does not specify any ops
						//Only add the op if the clinic option was not set in the appt view or if the op is assigned to that clinic.
						if(apptViewCur.ClinicNum==0 || apptViewCur.ClinicNum==listOpsShort[i].ClinicNum) {
							indexOp=Operatories.GetOrder(listOpsShort[i].OperatoryNum);
							if(indexOp!=-1 && !visOps.Contains(listOpsShort[i])) {
								visOps.Add(listOpsShort[i]);
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
			visOps=visOps.GroupBy(x => x.OperatoryNum).Select(x => x.First()).ToList();
		}

		///<summary>Sorts list of operatories by ItemOrder.</summary>
		public static int CompareOps(Operatory op1,Operatory op2) {
			if(op1.ItemOrder<op2.ItemOrder) {
				return -1;
			}
			else if(op1.ItemOrder>op2.ItemOrder) {
				return 1;
			}
			return 0;
		}

		///<summary>Sorts list of providers by ItemOrder.</summary>
		public static int CompareProvs(Provider prov1,Provider prov2) {
			if(prov1.ItemOrder<prov2.ItemOrder) {
				return -1;
			}
			else if(prov1.ItemOrder>prov2.ItemOrder) {
				return 1;
			}
			return 0;
		}

		

	



	}
}
