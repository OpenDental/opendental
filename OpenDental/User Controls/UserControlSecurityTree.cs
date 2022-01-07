using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.ComponentModel;
using CodeBase;

namespace OpenDental {
	///<summary>Security Tree control so that any changes to the security tree do not have to be made in multiple places. Only used in UserControlUserGroupSecurity (which itself is implemented in FormSecurity and FormCentralSecurity).</summary>
	public partial class UserControlSecurityTree:UserControl {
		public LayoutManagerForms LayoutManager;
		private TreeNode _clickedPermNode;
		///<summary>This should contain one item when editing permissions. Can contain multiple when viewing permissions for a user.</summary>
		private List<long> _listUserGroupNums=new List<long>();
		///<summary>An eventhandler that returns a DialogResult, so that the form that implements this security tree 
		///can use their own Report Permission and Group Permission forms.</summary>
		public delegate DialogResult SecurityTreeEventHandler(object sender,SecurityEventArgs e);
		[Category("OD")]
		[Description("Occurs when the Report Permission window would be shown. Open the Report Permission form and return its DialogResult.")]
		public event SecurityTreeEventHandler ReportPermissionChecked = null;
		[Category("OD")]
		[Description("Occurs when the Group Permission Edit window would be shown. Open the Group Permission form and return its DialogResult.")]
		public event SecurityTreeEventHandler GroupPermissionChecked = null;

		[Category("OD")]
		[Description("Set to true to disallow users from interacting with the control. The tree will still display correctly.")]
		public bool ReadOnly {
			get;set;
		}

		public UserControlSecurityTree() {
			InitializeComponent();
		}

		private void UserControlSecurityTree_Load(object sender,EventArgs e) {
			if(ReadOnly) {
				treePermissions.BackColor = SystemColors.Control;
			}
			if(CodeBase.ODBuild.IsDebug() && Environment.MachineName.ToLower()=="jordanhome"){
				textXpos.Visible=true;
			}
		}

		///<summary>Fills the tree with the initial permission nodes.</summary>
		public void FillTreePermissionsInitial() {
			TreeNode node;
			TreeNode node2;//second level
			TreeNode node3;//third level
			TreeNode node4;//fourth level
			#region Main Menu
			node=SetNode("Main Menu");
				#region File
				node2=SetNode("File");
					node3=SetNode(Permissions.GraphicsEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ChooseDatabase);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
				#region Setup
				node2=SetNode(Permissions.Setup);
					#region EHR
					node3=SetNode("Chart - EHR");
						node4=SetNode(Permissions.EhrEmergencyAccess);
						node3.Nodes.Add(node4);
						node4=SetNode(Permissions.EhrMeasureEventEdit);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					#endregion
					node3=SetNode("Advanced Setup");
						node4=SetNode(Permissions.ReplicationSetup);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.AutoNoteQuickNoteEdit);
					node2.Nodes.Add(node3);
					if(ODBuild.IsWeb()) {
						node3=SetNode(Permissions.CloseOtherSessions);
						node2.Nodes.Add(node3);
					}
					#region Dental School
					node3=SetNode("Dental School");
						node4=SetNode(Permissions.AdminDentalInstructors);
						node3.Nodes.Add(node4);
						node4=SetNode(Permissions.AdminDentalStudents);
						node3.Nodes.Add(node4);
						node4=SetNode(Permissions.AdminDentalEvaluations);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					#endregion
					node3=SetNode(Permissions.Schedules);
					node2.Nodes.Add(node3);
					node3=SetNode("Security");
						node4=SetNode(Permissions.SecurityAdmin);
						node3.Nodes.Add(node4);
						node4=SetNode(Permissions.AddNewUser);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
				#region Lists
				node2=SetNode("Lists");
					node3=SetNode(Permissions.ProcCodeEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.FeeSchedEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.AllowFeeEditWhileReceivingClaim);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ProviderFeeEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ProblemEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.Providers);
						node4=SetNode(Permissions.ProviderAlphabetize);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					#region Referrals
					node3=SetNode("Referrals");
						node4=SetNode(Permissions.ReferralAdd);
						node3.Nodes.Add(node4);
						node4=SetNode(Permissions.ReferralEdit);
						node3.Nodes.Add(node4);
						node4=SetNode(Permissions.RefAttachAdd);
						node3.Nodes.Add(node4);
						node4=SetNode(Permissions.RefAttachDelete);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					#endregion
				node.Nodes.Add(node2);
				#endregion
				#region Reports
				node2=SetNode(Permissions.Reports);
					node3=SetNode(Permissions.ReportProdIncAllProviders);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ReportDailyAllProviders);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.GraphicalReportSetup);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.GraphicalReports);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.UserQuery);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.UserQueryAdmin);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.CommandQuery);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.NewClaimsProcNotBilled);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
				#region Tools
				node2=SetNode("Tools");
					node3=SetNode("Misc Tools");
						node4=SetNode(Permissions.MedicationMerge);
						node3.Nodes.Add(node4);
						node4=SetNode(Permissions.PatientMerge);
						node3.Nodes.Add(node4);
						node4=SetNode(Permissions.ProviderMerge);
						node3.Nodes.Add(node4);
						node4=SetNode(Permissions.ReferralMerge);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.AuditTrail);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.CertificationEmployee);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.CertificationSetup);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.RepeatChargeTool);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.WikiAdmin);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.WikiListSetup);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.WebFormAccess);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.Zoom);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
				#region Clinics
				node2=SetNode("Clinics");
					node3=SetNode(Permissions.UnrestrictedSearch);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
				#region eServices
				node2=SetNode("eServices");
					node3=SetNode(Permissions.EServicesSetup);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
				#region Help
				node2=SetNode("Help");
					node3=SetNode(Permissions.QueryMonitor);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
			treePermissions.Nodes.Add(node);
			#endregion
			#region Main Toolbar
			node=SetNode("Main Toolbar");
				node2=SetNode(Permissions.CommlogCreate);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.CommlogEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.EmailSend);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.WebMailSend);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.SheetEdit);
				node.Nodes.Add(node2);
					node3=SetNode(Permissions.SheetDelete);
					node2.Nodes.Add(node3);
				node2=SetNode(Permissions.TaskEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.TaskNoteEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.TaskListCreate);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.PopupEdit);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Appts Module
			node=SetNode(Permissions.AppointmentsModule);
				node2=SetNode(Permissions.AppointmentCreate);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.AppointmentMove);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.AppointmentEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.AppointmentCompleteEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.EcwAppointmentRevise);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.InsPlanVerifyList);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.ApptConfirmStatusEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.Blockouts);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Family Module
			node=SetNode(Permissions.FamilyModule);
				node2=SetNode(Permissions.InsPlanEdit);
				node.Nodes.Add(node2);
					node3=SetNode(Permissions.InsPlanPickListExisting);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.InsuranceVerification);
					node2.Nodes.Add(node3);
				node2=SetNode(Permissions.InsPlanChangeAssign);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.InsPlanChangeSubsc);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.InsPlanOrthoEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.CarrierCreate);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.PatientBillingEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.PatPriProvEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.PatientApptRestrict);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.ArchivedPatientEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.PatientSSNView);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.PatientDOBView);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Account Module
			node=SetNode(Permissions.AccountModule);
				node2=SetNode("Claim");
					node3=SetNode(Permissions.ClaimSend);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ClaimSentEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ClaimDelete);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ClaimHistoryEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ClaimView);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ClaimProcClaimAttachedProvEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ClaimProcReceivedEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.UpdateCustomTracking);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.PreAuthSentEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.AccountProcsQuickAdd);
				node.Nodes.Add(node2);
				node2=SetNode("Insurance Payment");
					node3=SetNode(Permissions.InsPayCreate);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.InsPayEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.InsWriteOffEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Payment");
					node3=SetNode(Permissions.PaymentCreate);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.PaymentEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.SplitCreatePastLockDate);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Payment Plan");
					node3=SetNode(Permissions.PayPlanEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.PayPlanChargeDateEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Adjustment");
					node3=SetNode(Permissions.AdjustmentCreate);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.AdjustmentEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.AdjustmentEditZero);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Treat Plan Module
			node=SetNode(Permissions.TPModule);
				node2=SetNode(Permissions.TreatPlanEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.TreatPlanPresenterEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.TreatPlanSign);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Chart Module
			node=SetNode(Permissions.ChartModule);
				node2=SetNode("Procedure");
					node3=SetNode(Permissions.ProcExistingEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ProcEditShowFee);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ProcDelete);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ProcedureNoteFull);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ProcedureNoteUser);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.GroupNoteEditSigned);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Completed Procedure");
						node3=SetNode(Permissions.ProcComplCreate);
						node2.Nodes.Add(node3);
						node3=SetNode(Permissions.ProcCompleteEdit);
						node2.Nodes.Add(node3);
						node3=SetNode(Permissions.ProcCompleteStatusEdit);
						node2.Nodes.Add(node3);
						node3=SetNode(Permissions.ProcCompleteNote);
						node2.Nodes.Add(node3);
						node3=SetNode(Permissions.ProcCompleteAddAdj);
						node2.Nodes.Add(node3);
						node3=SetNode(Permissions.ProcCompleteEditMisc);
						node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Rx");
					node3=SetNode(Permissions.RxCreate);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.RxEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.OrthoChartEditFull);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.OrthoChartEditUser);
				node.Nodes.Add(node2);
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					node2=SetNode(Permissions.PerioEdit);
					node.Nodes.Add(node2);
				}
				node2=SetNode("Anesthesia");
					node3=SetNode(Permissions.AnesthesiaIntakeMeds);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.AnesthesiaControlMeds);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Imaging Module
			node=SetNode(Permissions.ImagingModule);
				node2=SetNode(Permissions.ImageCreate);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.ImageDelete);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.ImageEdit);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.ImageExport);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Manage Module
			node=SetNode(Permissions.ManageModule);
				node2=SetNode(Permissions.Accounting);
					node3=SetNode(Permissions.AccountingCreate);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.AccountingEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.Billing);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.DepositSlips);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.Backup);
				node.Nodes.Add(node2);
				node2=SetNode("Time Card");
					node3=SetNode(Permissions.TimecardsEditAll);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.TimecardDeleteEntry);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.ProtectedLeaveAdjustmentEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Equipment");
					node3=SetNode(Permissions.EquipmentSetup);
					node2.Nodes.Add(node3);
					node3=SetNode(Permissions.EquipmentDelete);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			node=SetNode("Merge Tools");
				node2=SetNode(Permissions.InsCarrierCombine);
				node.Nodes.Add(node2);
				node2=SetNode(Permissions.InsPlanMerge);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			node=SetNode("Web Applications");
				node2=SetNode(Permissions.MobileWeb);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#region ODCloud Only Permissions
			if(ODBuild.IsWeb()) {
				node=SetNode("Cloud");
					node2=SetNode(Permissions.AllowLoginFromAnyLocation);
					node.Nodes.Add(node2);
				treePermissions.Nodes.Add(node);
			}
			#endregion
			#region HQ Only Permissions
			if(PrefC.IsODHQ) {
				node=SetNode("HQ Only");
					node2=SetNode(Permissions.CommlogPersistent);
					node.Nodes.Add(node2);
					node2=SetNode(Permissions.SalesTaxAdjEdit);
					node.Nodes.Add(node2);
					node2=SetNode(Permissions.HeadmasterSetup);
					node.Nodes.Add(node2);
					node2=SetNode(Permissions.FAQEdit);
					node.Nodes.Add(node2);
					node2=SetNode(Permissions.FeatureRequestEdit);
					node.Nodes.Add(node2);
				treePermissions.Nodes.Add(node);
			}
			#endregion
			treePermissions.ExpandAll();
		}

		///<summary>Fills the tree's checkboxes depending on whether or not the usergroups in _listUserGroupNums have the specific permission.</summaryZ>
		public void FillTreePerm() {
			GroupPermissions.RefreshCache();
			if(_listUserGroupNums.Count == 0) {
				treePermissions.Enabled=false;
			}
			else {
				treePermissions.Enabled=true;
			}
			treePermissions.BeginUpdate();
			for(int i = 0;i<treePermissions.Nodes.Count;i++) {
				FillNodes(treePermissions.Nodes[i],_listUserGroupNums);
			}
			treePermissions.EndUpdate();
		}

		///<summary>Only called from FillTreePermissionsInitial</summary>
		private TreeNode SetNode(string text) {
			TreeNode retVal = new TreeNode();
			retVal.Text=Lan.g(this,text);
			retVal.Tag=Permissions.None;
			retVal.ImageIndex=0;
			retVal.SelectedImageIndex=0;
			return retVal;
		}

		///<summary>This just keeps FillTreePermissionsInitial looking cleaner.</summary>
		private TreeNode SetNode(Permissions perm) {
			TreeNode retVal = new TreeNode();
			retVal.Text=GroupPermissions.GetDesc(perm);
			retVal.Tag=perm;
			retVal.ImageIndex=1;
			retVal.SelectedImageIndex=1;
			return retVal;
		}

		///<summary>A recursive function that sets the checkbox for a node.  Also sets the text for the node.</summary>
		private void FillNodes(TreeNode node,List<long> listUserGroupNums) {
			//first, any child nodes
			for(int i = 0;i<node.Nodes.Count;i++) {
				FillNodes(node.Nodes[i],listUserGroupNums);
			}
			//then this node
			if(node.ImageIndex==0) {
				return;
			}
			node.ImageIndex=1;
			node.Text=GroupPermissions.GetDesc((Permissions)node.Tag);
			//get all grouppermissions for the passed-in usergroups
			List<GroupPermission> listGroupPerms = GroupPermissions.GetForUserGroups(listUserGroupNums);
			List<GroupPermission> listBaseReportingPerms = listGroupPerms.Where(x => x.PermType == Permissions.Reports && x.FKey == 0).ToList();
			List<GroupPermission> listDisplayReportingPerms = listGroupPerms.Where(x => x.PermType == Permissions.Reports && x.FKey != 0).ToList();
			//group by permtype, preferring newerdays/newerdate that are further back in the past.
			listGroupPerms=listGroupPerms.GroupBy(x => x.PermType)
				.Select(x => x
					.OrderBy((GroupPermission y) => {
						if(y.NewerDays==0 && y.NewerDate == DateTime.MinValue) {
							return DateTime.MinValue;
						}
						if(y.NewerDays==0) {
							return y.NewerDate;
						}
						return DateTime.Today.AddDays(-y.NewerDays);
					}).FirstOrDefault())
				.ToList();
			//display the correct newerdays/newerdate that was found for each permission.
			for(int i = 0;i<listGroupPerms.Count;i++) {
				if(listUserGroupNums.Contains(listGroupPerms[i].UserGroupNum)
					&& listGroupPerms[i].PermType==(Permissions)node.Tag) 
				{
					node.ImageIndex=2;
					if(listGroupPerms[i].NewerDate.Year>1880) {
						node.Text+=" ("+Lan.g(this,"if date newer than")+" "+listGroupPerms[i].NewerDate.ToShortDateString()+")";
					}
					else if(listGroupPerms[i].NewerDays>0) {
						node.Text+=" ("+Lan.g(this,"if days newer than")+" "+listGroupPerms[i].NewerDays.ToString()+")";
					}
				}
			}
			//Special case for Reports permission.  
			//Get a list of all report permissions from usergroups that this user is associated to IF the usergroup has the "base" (FKey = 0) report permission.
			if((Permissions)node.Tag == Permissions.Reports) {
				List<GroupPermission> listReportPermsForUser = listDisplayReportingPerms.FindAll(x => listUserGroupNums.Contains(x.UserGroupNum)).ToList();
				listReportPermsForUser.RemoveAll(x => !listBaseReportingPerms.Select(y => y.UserGroupNum).Contains(x.UserGroupNum));
				int state = DisplayReports.GetReportState(listReportPermsForUser);
				if(state==1) {
					node.ImageIndex=2;//Checked
				}
				else if(state==0) {
					node.ImageIndex=3;//Partially Checked
				}
				else {
					node.ImageIndex=1;//Unchecked
				}
			}
		}

		public void CollapseAll(){
			treePermissions.CollapseAll();
		}

		public void ExpandAll(){
			treePermissions.ExpandAll();
		}

		///<summary>Gives the current usergroup all permissions. There should only be one usergroup selected when this is called.  Throws exceptions.</summary>
		public void SetAll() {
			if(_listUserGroupNums.Count!=1) {
				throw new Exception("SetAll may not be called when multiple usergroups are selected.");
			}
			GroupPermission perm;
			for(int i = 0;i<Enum.GetNames(typeof(Permissions)).Length;i++) {
				if(i==(int)Permissions.SecurityAdmin
					|| i==(int)Permissions.StartupMultiUserOld
					|| i==(int)Permissions.StartupSingleUserOld
					|| i==(int)Permissions.EhrKeyAdd) 
				{
					continue;
				}
				perm=GroupPermissions.GetPerm(_listUserGroupNums.First(),(Permissions)i);
				if(perm==null) {
					perm=new GroupPermission();
					perm.PermType=(Permissions)i;
					perm.UserGroupNum=_listUserGroupNums.First();
					try {
						GroupPermissions.Insert(perm);
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
					}
				}
			}
			//add all the report permissions as well
			List<DisplayReport> _listDisplayReportAll=DisplayReports.GetAll(false);
			foreach(DisplayReport report in _listDisplayReportAll) {
				if(GroupPermissions.HasPermission(_listUserGroupNums.First(),Permissions.Reports,report.DisplayReportNum)) {
					continue; //don't bother creating or adding the permission if the usergroup already has it.
				}
				perm=new GroupPermission();
				perm.NewerDate=DateTime.MinValue;
				perm.NewerDays=0;
				perm.PermType=Permissions.Reports;
				perm.UserGroupNum=_listUserGroupNums.First();
				perm.FKey=report.DisplayReportNum;
				try {
					GroupPermissions.Insert(perm);
				}
				catch (Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
			FillTreePerm();
		}

		///<summary>Sets the current usergroup and refills the tree.</summary>
		public void FillForUserGroup(long userGroupNum) {
			_listUserGroupNums = new List<long>() { userGroupNum };
			FillTreePerm();
		}

		///<summary>Sets the current usergroups and refills the tree.</summary>
		public void FillForUserGroup(List<long> listUserGroupNums) {
			_listUserGroupNums = listUserGroupNums;
			FillTreePerm();
		}

		///<summary>Toggles permissions based on the node the user has clicked. 
		///Will do nothing if in Read-Only mode or more than one usergroup is selected.</summary>
		private void treePermissions_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(ReadOnly || _listUserGroupNums.Count != 1) {
				return;
			}
			_clickedPermNode=treePermissions.GetNodeAt(e.X,e.Y);
			if(_clickedPermNode==null) {
				return;
			}
			//Do nothing if the user didn't click on a check box.
			if(_clickedPermNode.Parent==null) {//level 1
				//if(e.X<5 || e.X>17) {
				if(e.X<24 || e.X>35) {//These new ranges are needed because of the +/- boxes.  Tested at different zoom levels.
					return;
				}
			}
			else if(_clickedPermNode.Parent.Parent==null) {//level 2
				//if(e.X<24 || e.X>36) {
				if(e.X<43 || e.X>54) {
					return;
				}
			}
			else if(_clickedPermNode.Parent.Parent.Parent==null) {//level 3
				//if(e.X<43 || e.X>55) {
				if(e.X<62 || e.X>73) {
					return;
				}
			}
			List<Permissions> listLimitedPermissions=new List<Permissions> { Permissions.ProcCompleteNote, Permissions.ProcCompleteAddAdj,
					Permissions.ProcCompleteEditMisc
			};
			Permissions permEcEo=Permissions.ProcExistingEdit;
			//User clicked on a check box.  Do stuff.
			if(_clickedPermNode.ImageIndex==1) {//unchecked, so need to add a permission
				GroupPermission perm = new GroupPermission();
				perm.PermType=(Permissions)_clickedPermNode.Tag;
				perm.UserGroupNum=_listUserGroupNums.First();
				if(GroupPermissions.PermTakesDates(perm.PermType)) {
					perm.IsNew=true;
					//Call an event that bubbles back up to the calling Form. The event returns a dialog result so we know how to continue here.
					DialogResult result = GroupPermissionChecked?.Invoke(sender,new SecurityEventArgs(perm))??DialogResult.Cancel;
					if(result==DialogResult.Cancel) {
						treePermissions.EndUpdate();
						return;
					}
				}
				else if(perm.PermType==Permissions.Reports) {//Reports permission is being checked.  
					//Call an event that bubbles back up to the calling Form. The event returns a dialog result so we know how to continue here.
					DialogResult result = ReportPermissionChecked?.Invoke(sender,new SecurityEventArgs(perm))??DialogResult.Cancel;
					if(result==DialogResult.Cancel) {
						treePermissions.EndUpdate();
						return;
					}
				}
				else {
					try {
						GroupPermissions.Insert(perm);
						SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Permission '"+perm.PermType+"' granted to '"
							+UserGroups.GetGroup(perm.UserGroupNum).Description+"'");
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
						return;
					}
				}
				if(perm.PermType==permEcEo) {
					//Adding ProcExistingEdit, so add ProcComplNote, ProcComplAddAdj, and ProcComplEditMisc.
					foreach(Permissions permission in listLimitedPermissions) {
						//We used to have one "limited edit" permission, but this was split into three distinct permissions.  Previously, if a usergroup was granted
						//"full edit"(deprecated) or ProcExistingEdit, the group automatically inherited "limited edit".  Maintain this behavior for the three new
						//distinct permissions.
						GroupPermission permLimited=GroupPermissions.GetPerm(_listUserGroupNums.First(),permission);
						if(permLimited!=null) {
							continue;
						}
						GroupPermissions.RefreshCache();//refresh NewerDays/Date to add the same for limited permissions on a completed procedure
						perm=GroupPermissions.GetPerm(_listUserGroupNums.First(),perm.PermType);
						permLimited=new GroupPermission();
						permLimited.NewerDate=perm.NewerDate;
						permLimited.NewerDays=perm.NewerDays;
						permLimited.UserGroupNum=perm.UserGroupNum;
						permLimited.PermType=permission;
						try {
							GroupPermissions.Insert(permLimited);
							SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,Lans.g(this,"Permission ")+"'"+permLimited.PermType+"' "
								+Lans.g(this,"granted to")+" '"+UserGroups.GetGroup(perm.UserGroupNum).Description+"'");
						}
						catch(Exception ex) {
							MessageBox.Show(ex.Message);
							return;
						}
					}
				}
				else if(perm.PermType==Permissions.FeeSchedEdit) {
					//When giving a user group the full FeeSchedEdit permission, they get the AllowFeeEditWhileRecivingClaim by default
					GroupPermission permFeeSchedLimited=new GroupPermission();
					permFeeSchedLimited.PermType=Permissions.AllowFeeEditWhileReceivingClaim;
					permFeeSchedLimited.UserGroupNum=_listUserGroupNums.First();
					try {
						GroupPermissions.Insert(permFeeSchedLimited);
						SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,Lans.g(this,"Permission ")+"'"+permFeeSchedLimited.PermType+"' "
							+Lans.g(this,"granted to")+" '"+UserGroups.GetGroup(permFeeSchedLimited.UserGroupNum).Description+"'");
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
						return;
					}
				}
			}
			else if(_clickedPermNode.ImageIndex==2) {//checked, so need to delete the perm
				try {
					if((Permissions)_clickedPermNode.Tag==Permissions.AllowFeeEditWhileReceivingClaim && GroupPermissions.HasPermission(_listUserGroupNums.First(),Permissions.FeeSchedEdit,0)) {
						//As it stands now, the AllowFeeEditWhileReceivingClaim permission is automatically given if a a user has the full FeeSchedEdit permission.
						//Allowing users to uncheck this permission while stil having the full permission it would lead to confusion
						//Using MsgBox with autotranslate as the descriptions of the permissions should never change, and if they do we would them to be translated.
						MsgBox.Show(this,$"{GroupPermissions.GetDesc(Permissions.AllowFeeEditWhileReceivingClaim)} " +
							$"cannot be removed from a user who has {GroupPermissions.GetDesc(Permissions.FeeSchedEdit)}. Please remove this permission first.");
						return;
					}
					GroupPermissions.RemovePermission(_listUserGroupNums.First(),(Permissions)_clickedPermNode.Tag);
					SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Permission '"+_clickedPermNode.Tag+"' revoked from '"
						+UserGroups.GetGroup(_listUserGroupNums.First()).Description+"'");
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				if(ListTools.In(((Permissions)_clickedPermNode.Tag),listLimitedPermissions)) {
					//Deselecting one of the new limited permissions so deselect ProcExistingEdit permissions if present.
					if(GroupPermissions.HasPermission(_listUserGroupNums.First(),permEcEo,0)) {
						try {
							GroupPermissions.RemovePermission(_listUserGroupNums.First(),permEcEo);
							SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,Lans.g(this,"Permission")+" '"+permEcEo+"' "
								+Lans.g(this,"revoked from")+" '"+UserGroups.GetGroup(_listUserGroupNums.First()).Description+"'");
						}
						catch(Exception ex) {
							MessageBox.Show(ex.Message);
							return;
						}
					}
				}
			}
			else if(_clickedPermNode.ImageIndex==3) {//Partially checked (currently only applies to Reports permission)
				try {
					GroupPermissions.RemovePermission(_listUserGroupNums.First(),(Permissions)_clickedPermNode.Tag);
					SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Permission '"+_clickedPermNode.Tag+"' revoked from '"
						+UserGroups.GetGroup(_listUserGroupNums.First()).Description+"'");
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
			}
			FillTreePerm();
		}

		private void treePermissions_AfterSelect(object sender,System.Windows.Forms.TreeViewEventArgs e) {
			treePermissions.SelectedNode=null;
		}

		///<summary>Allows users to edit the date locks on specific permissions.
		///Does nothing if in ReadOnly mode or if more than one UserGroup is selected.</summary>
		private void treePermissions_DoubleClick(object sender,System.EventArgs e) {
			if(ReadOnly || _listUserGroupNums.Count!=1) {
				return;
			}
			if(_clickedPermNode==null) {
				return;
			}
			Permissions permType = (Permissions)_clickedPermNode.Tag;
			if(!GroupPermissions.PermTakesDates(permType)) {
				return;
			}
			GroupPermission perm = GroupPermissions.GetPerm(_listUserGroupNums.First(),(Permissions)_clickedPermNode.Tag);
			if(perm==null) {
				return;
			}
			//Call an event that bubbles back up to the calling Form. The event returns a dialog result so we know how to continue here.
			DialogResult result = GroupPermissionChecked?.Invoke(sender,new SecurityEventArgs(perm))??DialogResult.Cancel;
			if(result==DialogResult.Cancel) {
				return;
			}
			FillTreePerm();
		}

		private void treePermissions_MouseMove(object sender,MouseEventArgs e) {
			textXpos.Text=e.X.ToString();
		}

		private void UserControlSecurityTree_SizeChanged(object sender,EventArgs e) {
			if(LayoutManager!=null){
				treePermissions.ItemHeight=LayoutManager.Scale(15);
			}
		}

		
	}
}
