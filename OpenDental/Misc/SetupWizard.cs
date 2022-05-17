using System;
using System.Drawing;
using OpenDentBusiness;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using OpenDental.User_Controls.SetupWizard;
using System.Linq;

namespace OpenDental {

	///<summary>All the Setup Wizard methods that should show up in FormSetupWizard need to have a SetupCatAttr() attribute.
	///This will cause the method to show up in FormSetupWizard.</summary>
	public partial class SetupWizard {
		public abstract class SetupWizClass {
			///<summary>The name of the setup item that will appear in the list of setup items. This should be one or a few words. 
			///It needs to make sense in the following sentences (where XX is the Name):
			/// "Let's set up your XX..."
			/// "Congratulations! You have finished setting up your XX!"
			/// "You can always go back through this setup wizard if you need to make any modifications to your XX."</summary>
			public abstract string Name { get; }
			///<summary>The description and explanation for this setup item.</summary>
			public abstract string GetDescript { get; }
			///<summary>A category for this setup item.</summary>
			public abstract ODSetupCategory GetCategory { get; }
			///<summary>The status of this setup item at any given moment. Should return one of the three setup statuses conditionally.</summary>
			public abstract ODSetupStatus GetStatus { get; }
			///<summary>Most of the time, you should return a control that is stored in an instance of the setup class.</summary>
			public abstract SetupWizControl SetupControl { get; }
			/* //eventually we probably want to move over to a list of controls so one setup item can have multiple controls.
			public abstract List<SetupWizControl> SetupControl { get; }
			public SetupWizControl GetNextControl {
				get {
					return SetupControl[_ctrlIncrement++];
				}
			}
			*/
		}

		#region Intro and Complete
		public class SetupIntro:SetupWizClass {
			private SetupWizControl _ctrl;
			private string _name;

			public SetupIntro(string name, string descript) {
				_name = name;
				_ctrl = new UserControlSetupWizIntro(name, descript);
			}

			public override ODSetupCategory GetCategory {
				get {
					throw new Exception("This should not get called.");
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					throw new Exception("This should not get called.");
				}
			}

			public override string Name {
				get {
					return _name;
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl ;
				}
			}

			public override string GetDescript {
				get {
					throw new Exception("This should not get called.");
				}
			}
		}


		public class SetupComplete:SetupWizClass {
			private SetupWizControl _ctrl;
			private string _name;

			public SetupComplete(string name) {
				_name = name;
				_ctrl = new UserControlSetupWizComplete(name);
			}

			public override ODSetupCategory GetCategory {
				get {
					throw new Exception("This should not get called.");
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					throw new Exception("This should not get called.");
				}
			}

			public override string Name {
				get {
					return _name;
				}
			}

			public override string GetDescript {
				get {
					throw new Exception("This should not get called.");
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl;
				}
			}
		}
		#endregion

		#region PreSetup
		public class RegKeySetup:SetupWizClass {
			private SetupWizControl _ctrl = new UserControlSetupWizRegKey();
			public override ODSetupCategory GetCategory {
				get {
					return ODSetupCategory.PreSetup;
				}
			}

			public override string GetDescript {
				get {
					string retVal = "Some items need to be set up before the program can be used effectively. "
						+"\r\nThis wizard's purpose is to help you quickly set those items up so that you can get started using the program.";
					if(GetStatus != ODSetupStatus.Complete) {
						retVal+="\r\n\r\nIt looks like you have yet to enter your Registration Key. ";
					}
					retVal+="\r\nEntering your Registration Key is a necessary first step in order for the program to function.";
					return retVal;
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					if(string.IsNullOrEmpty(PrefC.GetString(PrefName.RegistrationKey))) {
						return ODSetupStatus.NotStarted;
					}
						return ODSetupStatus.Complete;
				}
			}

			public override string Name {
				get {
					return "Registration Key";
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl;
				}
			}
		}
		#endregion

		#region Basic Setup
		public class FeatureSetup:SetupWizClass {
			private SetupWizControl _ctrl = new UserControlSetupWizFeatures();
			public override ODSetupCategory GetCategory {
				get {
					return ODSetupCategory.Basic;
				}
			}

			public override string GetDescript {
				get {
					return "Turn features that your office uses on/off. Settings will affect all computers using the same database.";
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					return ODSetupStatus.Optional;
				}
			}

			public override string Name {
				get {
					return "Basic Features";
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl;
				}
			}
		}

		public class ClinicSetup:SetupWizClass {
			private SetupWizControl _ctrl =new UserControlSetupWizClinic();
			public override string Name {
				get {
					return "Clinics";
				}
			}

			public override string GetDescript {
				get {
					return "You have indicated that you will be using the Clinics feature. "
						+"Clinics can be used when you have multiple locations. Once clinics are set up, you can assign clinics throughout Open Dental. "
						+"If you follow basic guidelines, default clinic assignments for patient information should be accurate, thus reducing data entry.";
				}
			}

			public override ODSetupCategory GetCategory {
				get {
					return ODSetupCategory.Basic;
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					List<Clinic> listClinics = Clinics.GetDeepCopy(true);
					if(listClinics.Count == 0) {
						return ODSetupStatus.NotStarted;
					}
					foreach(Clinic clin in listClinics) {
						if(string.IsNullOrEmpty(clin.Abbr)
							|| string.IsNullOrEmpty(clin.Description)
							|| string.IsNullOrEmpty(clin.Phone)
							|| string.IsNullOrEmpty(clin.Address)
							) {
							return ODSetupStatus.NeedsAttention;
						}
					}
					return ODSetupStatus.Complete;
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl;
				}
			}
		}

		public class DefinitionSetup:SetupWizClass {
			private SetupWizControl _ctrl=new UserControlSetupWizDefinitions();

			public override string Name {
				get {
					return "Definitions";
				}
			}

			public override string GetDescript {
				get {
					return "Definitions are an easy way to customize your software experience. Setup the colors, categories, and other customizable areas "
						+"within the program from this window.\r\n We've selected some of the definitions you may be interested in customizing for this Setup Wizard. "
						+"You may view the entire list of definitions by going to Setup -> Definitions from the main tool bar.";
				}
			}

			public override ODSetupCategory GetCategory {
				get {
					return ODSetupCategory.Basic;
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					return ODSetupStatus.Optional; //nothing is required
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl;
				}
			}
		}

		public class ProvSetup:SetupWizClass {
			private SetupWizControl _ctrl= new UserControlSetupWizProvider();
			public override string Name {
				get {
					return "Providers";
				}
			}
			public override string GetDescript {
				get {
					return "Providers will show up in almost every part of OpenDental. "
						+"It is important that all provider information is up-to-date so that "
						+"claims, reports, procedures, fee schedules, and estimates will function correctly.";
				}
			}

			public override ODSetupCategory GetCategory {
				get {
					return ODSetupCategory.Basic;
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					List<Provider> listProviders = Providers.GetDeepCopy(true);
					if(listProviders.Count == 0) {
						return ODSetupStatus.NotStarted;
					}
					foreach(Provider prov in listProviders) {
						bool isDentist = IsPrimary(prov);
						bool isHyg = prov.IsSecondary;
						if(((isDentist || isHyg) && string.IsNullOrEmpty(prov.Abbr))
							|| ((isDentist || isHyg) && string.IsNullOrEmpty(prov.LName))
							|| ((isDentist || isHyg) && string.IsNullOrEmpty(prov.FName))
							|| ((isDentist) && string.IsNullOrEmpty(prov.Suffix))
							|| ((isDentist) && string.IsNullOrEmpty(prov.SSN))
							|| ((isDentist) && string.IsNullOrEmpty(prov.NationalProvID))
							) {
							return ODSetupStatus.NeedsAttention;
						}
					}
					return ODSetupStatus.Complete;
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl;
				}
			}

			public static bool IsPrimary(Provider prov) {
				if(prov.IsSecondary) {
					return false;
				}
				if(Defs.GetName(DefCat.ProviderSpecialties,prov.Specialty).ToLower() == "hygienist"
					|| Defs.GetName(DefCat.ProviderSpecialties,prov.Specialty).ToLower() == "assistant"
					|| Defs.GetName(DefCat.ProviderSpecialties,prov.Specialty).ToLower() == "labtech"
					|| Defs.GetName(DefCat.ProviderSpecialties,prov.Specialty).ToLower() == "other"
					|| Defs.GetName(DefCat.ProviderSpecialties,prov.Specialty).ToLower() == "notes"
					|| Defs.GetName(DefCat.ProviderSpecialties,prov.Specialty).ToLower() == "none"
					) {
					return false;
				}
				if(prov.IsNotPerson) {
					return false;
				}
				return true;
			}
		}

		public class OperatorySetup:SetupWizClass {
			private SetupWizControl _ctrl = new UserControlSetupWizOperatory();
			public override string Name {
				get {
					return "Operatories";
				}
			}

			public override string GetDescript {
				get {
					return "Operatories define locations in which appointments take place, and are used to organize appointment columns. Normally, every chair in your office will have an unique operatory. ";
				}
			}

			public override ODSetupCategory GetCategory {
				get {
					return ODSetupCategory.Basic;
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					List<Operatory> listOperatories = Operatories.GetDeepCopy(true);
					if(listOperatories.Count == 0) {
						return ODSetupStatus.NotStarted;
					}
					foreach(Operatory op in listOperatories) {
						if(string.IsNullOrEmpty(op.OpName)
							|| string.IsNullOrEmpty(op.Abbrev)) {
							return ODSetupStatus.NeedsAttention;
						}
					}
					return ODSetupStatus.Complete;
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl;
				}
			}
		}

		public class PracticeSetup:SetupWizClass {
			private SetupWizControl _ctrl = new UserControlSetupWizPractice();
			public override ODSetupCategory GetCategory {
				get {
					return ODSetupCategory.Basic;
				}
			}

			public override string GetDescript {
				get {
					return "Practice information includes general contact information, billing and pay-to addresses, and default providers. "
						+"This information will appear on most reports, claims, statements, and letters.";
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					return ODSetupStatus.Complete;
				}
			}

			public override string Name {
				get {
					return "Practice Info";
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl;
				}
			}
		}

		public class EmployeeSetup:SetupWizClass {
			private SetupWizControl _ctrl = new UserControlSetupWizEmployee();
			public override ODSetupCategory GetCategory {
				get {
					return ODSetupCategory.Basic;
				}
			}

			public override string GetDescript {
				get {
					return "The Employee list is used to set up User profiles in Security and to set up Schedules.  This list also determines who can use the Time Clock.";
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					List<Employee> listEmployees=Employees.GetDeepCopy(true);
					if(listEmployees.Count==0) {
						return ODSetupStatus.NotStarted;
					}
					foreach(Employee employee in listEmployees) {
						if((string.IsNullOrEmpty(employee.FName))
							|| (string.IsNullOrEmpty(employee.LName))
							) {
							return ODSetupStatus.NeedsAttention;
						}
					}
					return ODSetupStatus.Complete;
				}
			}

			public override string Name {
				get {
					return "Employees";
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl;
				}
			}
		}

		public class FeeSchedSetup:SetupWizClass {
			private SetupWizControl _ctrl = new UserControlSetupWizFeeSched();
			public override ODSetupCategory GetCategory	{
				get	{
					return ODSetupCategory.Basic;
				}
			}

			public override string GetDescript {
				get	{
					return "Fee Schedules determine the fees billed for each procedure.";
				}
			}

			
			///<summary>Returns Complete if all fee schedules have at least one fee entered.</summary>
			public override ODSetupStatus GetStatus	{
				get	{					
					List<FeeSched> listFeeSched = FeeScheds.GetDeepCopy(true);
					if(listFeeSched.Count==0) {
						return ODSetupStatus.NotStarted;
					}
					List<long> listFeeSchedNums = listFeeSched.Select(x => x.FeeSchedNum).ToList();
					//clinic nums shouldn't matter here, just want basdic default fee schedules. 
					foreach(long schedNum in listFeeSchedNums) {
						if(Fees.GetCountByFeeSchedNum(schedNum) <= 0) {
							return ODSetupStatus.NeedsAttention;
						}
					}
					return ODSetupStatus.Complete;
				}
			}

			public override string Name	{
				get	{
					return "Fee Schedules";
				}
			}

			public override SetupWizControl SetupControl {
				get	{
					return _ctrl;
				}
			}
		}

		public class PrinterSetup:SetupWizClass {
			private SetupWizControl _ctrl = new UserControlSetupWizPrinter();
			public override ODSetupCategory GetCategory {
				get {
					return ODSetupCategory.Basic;
				}
			}

			public override string GetDescript {
				get {
					return "Set up print and scan options for the current workstation. "
						+"You can leave all settings to the default, or you can control where specific items are are printed.";
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					return ODSetupStatus.Optional;
				}
			}

			public override string Name {
				get {
					return "Printer/Scanner";
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl;
				}
			}
		}

		public class ScheduleSetup:SetupWizClass {
			private SetupWizControl _ctrl =  new UserControlSetupWizSchedule();
			public override ODSetupCategory GetCategory {
				get {
					return ODSetupCategory.Basic;
				}
			}

			public override string GetDescript {
				get {
					return "Schedule setup lets you enter all Provider and Employee schedules. "
						+"You can define any kind of rotating or alternating schedule you want. "
						+"Enter individual work hours, holidays, lunch hours, and staff meetings. "
						+"Once schedules are entered, open/closed hours will be indicated in the Appointment module.";
				}
			}

			public override ODSetupStatus GetStatus {
				get {
					//if there are no rows in the schedule table, then NotStarted.
					//if there are schedules for each provider for at least one day within the next year, then complete.
					//otherwise, needs attention.
					List<Schedule> listSchedule = Schedules.GetTwoYearPeriod(DateTime.Today.AddYears(-1));
					if(listSchedule.Count == 0) {
						return ODSetupStatus.NotStarted;
					}
					else {
						List<Provider> listProviders=Providers.GetWhere(x => !x.IsNotPerson,true).ToList();
						foreach(Provider prov in listProviders) {
							if(!listSchedule.Select(x => x.ProvNum).Contains(prov.ProvNum)) {
								return ODSetupStatus.NeedsAttention;
							}
						}
						return ODSetupStatus.Complete;
					}
				}
			}

			public override string Name {
				get {
					return "Schedule";
				}
			}

			public override SetupWizControl SetupControl {
				get {
					return _ctrl;
				}
			}
		}

		//public class ClearinghouseSetup:SetupWizClass {
		//	SetupWizControl _ctrl = new UserControlSetupWizProvider();

		//	public override string Name {
		//		get {
		//			return "Clearinghouses";
		//		}
		//	}

		//	public override string GetDescript {
		//		get {
		//			return "Clearinghouses are nice";
		//		}
		//	}

		//	public override ODSetupCategory GetCategory {
		//		get {
		//			return ODSetupCategory.Advanced;
		//		}
		//	}

		//	public override ODSetupStatus GetStatus {
		//		get {
		//			Clearinghouse clearHouseCur = OpenDentBusiness.Clearinghouses.GetDefaultDental();
		//			if(clearHouseCur == null) {
		//				return ODSetupStatus.NotStarted;
		//			}

		//			if(clearHouseCur == null
		//				|| string.IsNullOrEmpty(clearHouseCur.Description)
		//				|| clearHouseCur.Eformat == ElectronicClaimFormat.None
		//				|| (string.IsNullOrEmpty(clearHouseCur.LoginID)
		//					&& string.IsNullOrEmpty(clearHouseCur.Password)
		//					&& string.IsNullOrEmpty(clearHouseCur.ExportPath)
		//					&& string.IsNullOrEmpty(clearHouseCur.ResponsePath)
		//					&& string.IsNullOrEmpty(clearHouseCur.SenderTIN)
		//					&& string.IsNullOrEmpty(clearHouseCur.SenderName)
		//					&& string.IsNullOrEmpty(clearHouseCur.SenderTelephone))
		//				|| (string.IsNullOrEmpty(clearHouseCur.ISA02)
		//					&& string.IsNullOrEmpty(clearHouseCur.ISA04)
		//					&& string.IsNullOrEmpty(clearHouseCur.ISA05)
		//					&& string.IsNullOrEmpty(clearHouseCur.ISA07)
		//					&& string.IsNullOrEmpty(clearHouseCur.ISA08)
		//					&& string.IsNullOrEmpty(clearHouseCur.GS03)
		//					&& string.IsNullOrEmpty(clearHouseCur.ISA15))
		//				) {
		//				return ODSetupStatus.NeedsAttention;
		//			}
		//			return ODSetupStatus.Complete;
		//		}
		//	}

		//	public override SetupWizControl SetupControl {
		//		get {
		//			return _ctrl;
		//		}
		//	}
		//}


		//public class CarrierSetup:SetupWizClass {
		//	public override string Name {
		//		get {
		//			return "Carriers";
		//		}
		//	}

		//	//public override string GetDescript {
		//	//	get {
		//	//		return "Providers will show up in almost every part of OpenDental. "
		//	//			+"It is important that all provider information is up-to-date so that "
		//	//			+"claims, reports, and procedures, fee schedules, and estimates will function correctly.";
		//	//	}
		//	//}

		//	public override ODSetupCategory GetCategory {
		//		get {
		//			return ODSetupCategory.Advanced;
		//		}
		//	}

		//	public override ODSetupStatus GetStatus {
		//		get {
		//			List<Operatory> listOperatories = Operatories.ListShort;
		//			if(listOperatories.Count == 0) {
		//				return ODSetupStatus.NotStarted;
		//			}
		//			foreach(Operatory op in listOperatories) {
		//				if(string.IsNullOrEmpty(op.OpName)
		//					|| string.IsNullOrEmpty(op.Abbrev)) {
		//					return ODSetupStatus.NeedsAttention;
		//				}
		//			}
		//			return ODSetupStatus.Complete;
		//		}
		//	}

		//	public override SetupWizControl SetupControl {
		//		get {
		//			return null;
		//		}
		//	}
		//}
		#endregion

		#region Helper Methods
		public static Color GetColor(ODSetupStatus stat) {
			switch(stat) {
				case ODSetupStatus.NotStarted:
				case ODSetupStatus.NeedsAttention:
					return Color.FromArgb(255,255,204,204); //red
				//case ODSetupStatus.NeedsAttention:
					//return Color.FromArgb(255,255,255,204); //yellow
				case ODSetupStatus.Complete:
				case ODSetupStatus.Optional:
					return Color.FromArgb(255,204,255,204); //green. hey! it's a stoplight!
				default:
					return Color.White;
			}
		}

		#endregion

	}
	

	#region Enums
	///<summary></summary>
	public enum ODSetupCategory {
		///<summary>0</summary>
		[Description("Misc Setup")]
		Misc,
		///<summary>1</summary>
		None,
		///<summary>2</summary>
		[Description("Pre-Setup")]
		PreSetup,
		///<summary>2</summary>
		[Description("Basic Setup")]
		Basic,
		///<summary>3</summary>
		[Description("Advanced Setup")]
		Advanced,
	}

	///<summary></summary>
	public enum ODSetupStatus {
		///<summary>0 - User hasn't started this setup item.</summary>
		[Description("Needs Input")]
		NotStarted,
		///<summary>1 - User has left this setup item in an incomplete state.</summary>
		[Description("Needs Input")]
		NeedsAttention,
		///<summary>2 - Setup item has been considered and required elements have been filled in.</summary>
		[Description("OK")]
		Complete,
		///<summary>3 - Setup item is not required.</summary>
		[Description("Optional")]
		Optional
	}
	#endregion
}