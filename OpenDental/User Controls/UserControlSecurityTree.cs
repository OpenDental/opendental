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
		///<summary></summary>
		public bool _isCEMT;
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
		[Description("Occurs when the Definition Picker window would be shown. Open the Definition Picker Form and return its DialogResult.")]
		public event SecurityTreeEventHandler AdjustmentTypeDenyPermissionChecked = null;

		[Category("OD")]
		[Description("Set to true to disallow users from interacting with the control. The tree will still display correctly.")]
		public bool ReadOnly {
			get;set;
		}

		public UserControlSecurityTree() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
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
					node3=SetNode(EnumPermType.GraphicsEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ChooseDatabase);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.PrinterSetup);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
				#region Setup
				node2=SetNode(EnumPermType.Setup);
					#region EHR
					node3=SetNode("Chart - EHR");
						node4=SetNode(EnumPermType.EhrEmergencyAccess);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.EhrMeasureEventEdit);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					#endregion
					node3=SetNode("Advanced Setup");
						node4=SetNode(EnumPermType.ReplicationSetup);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.ShowFeatures);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.AutoNoteQuickNoteEdit);
					node2.Nodes.Add(node3);
					if(ODBuild.IsWeb()) {
						node3=SetNode(EnumPermType.CloseOtherSessions);
						node2.Nodes.Add(node3);
					}
					node3=SetNode("Definitions");
						node4=SetNode(EnumPermType.DefEdit);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					#region Dental School
					node3=SetNode("Dental School");
						node4=SetNode(EnumPermType.AdminDentalInstructors);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.AdminDentalStudents);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.AdminDentalEvaluations);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					#endregion
					node3=SetNode(EnumPermType.Schedules);
					node2.Nodes.Add(node3);
					node3=SetNode("Security");
						node4=SetNode(EnumPermType.SecurityAdmin);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.AddNewUser);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.ManageHighSecurityProgProperties);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.UpdateInstall);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
				#region Lists
				node2=SetNode("Lists");
					node3=SetNode(EnumPermType.ProcCodeEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.FeeSchedEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.AllowFeeEditWhileReceivingClaim);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ProviderFeeEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.MedicationDefEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.AllergyDefEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ProblemDefEdit);
					node2.Nodes.Add(node3);
					node3=SetNode("Providers");
						node4=SetNode(EnumPermType.ProviderAdd);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.ProviderEdit);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.ProviderAlphabetize);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					#region Clinics
					node3=SetNode("Clinics");
						node4=SetNode(EnumPermType.ClinicEdit);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.UnrestrictedSearch);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					#endregion
					#region Referrals
					node3=SetNode("Referrals");
						node4=SetNode(EnumPermType.ReferralAdd);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.ReferralEdit);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.RefAttachAdd);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.RefAttachDelete);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					#endregion
				node.Nodes.Add(node2);
				#endregion
				#region Reports
				node2=SetNode(EnumPermType.Reports);
					node3=SetNode(EnumPermType.ReportProdIncAllProviders);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ReportDailyAllProviders);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.GraphicalReportSetup);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.GraphicalReports);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.UserQuery);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.UserQueryAdmin);
					node2.Nodes.Add(node3);
					if(!ODBuild.IsWeb()) {
						node3=SetNode(EnumPermType.CommandQuery);
						node2.Nodes.Add(node3);
					}
					node3=SetNode(EnumPermType.NewClaimsProcNotBilled);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
				#region Tools
				node2=SetNode("Tools");
					node3=SetNode("Misc Tools");
						node4=SetNode(EnumPermType.MedicationMerge);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.PatientMerge);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.ProviderMerge);
						node3.Nodes.Add(node4);
						node4=SetNode(EnumPermType.ReferralMerge);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.Advertising);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.AuditTrail);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.CertificationEmployee);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.CertificationSetup);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.RepeatChargeTool);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.SetupWizard);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.WikiAdmin);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.WikiListSetup);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.WebFormAccess);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.Zoom);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
				#region eServices
				node2=SetNode("eServices");
					node3=SetNode(EnumPermType.EServicesSetup);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
				#region Help
				node2=SetNode("Help");
					node3=SetNode(EnumPermType.QueryMonitor);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				#endregion
			treePermissions.Nodes.Add(node);
			#endregion
			#region Main Toolbar
			node=SetNode("Main Toolbar");
				node2=SetNode(EnumPermType.CommlogCreate);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.CommlogEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.EmailSend);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.TextMessageView);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.TextMessageSend);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.WebMailSend);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.SheetEdit);
				node.Nodes.Add(node2);
					node3=SetNode(EnumPermType.SheetDelete);
					node2.Nodes.Add(node3);
				node2=SetNode(EnumPermType.TaskEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.TaskNoteEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.TaskDelete);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.TaskListCreate);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.PopupEdit);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Appts Module
			node=SetNode(EnumPermType.AppointmentsModule);
				node2=SetNode(EnumPermType.AppointmentCreate);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.AppointmentMove);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.AppointmentResize);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.AppointmentEdit);
				node.Nodes.Add(node2);
					node3=SetNode(EnumPermType.AppointmentDelete);
					node2.Nodes.Add(node3);
				node2=SetNode(EnumPermType.AppointmentCompleteEdit);
				node.Nodes.Add(node2);
					node3=SetNode(EnumPermType.AppointmentCompleteDelete);
					node2.Nodes.Add(node3);
				node2=SetNode(EnumPermType.ViewAppointmentAuditTrail);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.EcwAppointmentRevise);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.InsPlanVerifyList);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.ApptConfirmStatusEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.Blockouts);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Family Module
			node=SetNode(EnumPermType.FamilyModule);
				node2=SetNode(EnumPermType.InsPlanEdit);
				node.Nodes.Add(node2);
					node3=SetNode(EnumPermType.InsPlanPickListExisting);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.InsuranceVerification);
					node2.Nodes.Add(node3);
				node2=SetNode(EnumPermType.InsPlanChangeAssign);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.InsPlanChangeSubsc);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.InsPlanOrthoEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.CarrierCreate);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.CarrierEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.PatientBillingEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.PatPriProvEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.PatientApptRestrict);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.ArchivedPatientSelect);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.ArchivedPatientEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.PatientSSNView);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.PatientDOBView);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.PatientEdit);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Account Module
			node=SetNode(EnumPermType.AccountModule);
				node2=SetNode("Claim");
					node3=SetNode(EnumPermType.ClaimSend);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ClaimSentEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ClaimDelete);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ClaimHistoryEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ClaimView);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ClaimProcClaimAttachedProvEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ClaimProcReceivedEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.UpdateCustomTracking);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.PreAuthSentEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.AccountProcsQuickAdd);
				node.Nodes.Add(node2);
				node2=SetNode("Insurance Payment");
					node3=SetNode(EnumPermType.InsPayCreate);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.InsPayEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.InsWriteOffEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Payment");
					node3=SetNode(EnumPermType.PaymentCreate);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.PaymentEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.SplitCreatePastLockDate);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Payment Plan");
					node3=SetNode(EnumPermType.PayPlanEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.PayPlanChargeDateEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Adjustment");
					node3=SetNode(EnumPermType.AdjustmentCreate);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.AdjustmentEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.AdjustmentEditZero);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.AdjustmentTypeDeny);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Statement");
					node3=SetNode(EnumPermType.StatementCSV);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Treat Plan Module
			node=SetNode(EnumPermType.TPModule);
				node2=SetNode(EnumPermType.TreatPlanEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.TreatPlanPresenterEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.TreatPlanSign);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Chart Module
			node=SetNode(EnumPermType.ChartModule);
				node2=SetNode("Procedure");
					node3=SetNode(EnumPermType.ProcExistingEdit);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ProcEditShowFee);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ProcDelete);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ProcedureNoteFull);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ProcedureNoteUser);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.GroupNoteEditSigned);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Completed Procedure");
						node3=SetNode(EnumPermType.ProcComplCreate);
						node2.Nodes.Add(node3);
						node3=SetNode(EnumPermType.ProcCompleteEdit);
						node2.Nodes.Add(node3);
						node3=SetNode(EnumPermType.ProcCompleteStatusEdit);
						node2.Nodes.Add(node3);
						node3=SetNode(EnumPermType.ProcCompleteNote);
						node2.Nodes.Add(node3);
						node3=SetNode(EnumPermType.ProcCompleteAddAdj);
						node2.Nodes.Add(node3);
						node3=SetNode(EnumPermType.ProcCompleteEditMisc);
						node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Rx");
					node3=SetNode(EnumPermType.RxCreate);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.RxEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.OrthoChartEditFull);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.OrthoChartEditUser);
				node.Nodes.Add(node2);
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					node2=SetNode(EnumPermType.PerioEdit);
					node.Nodes.Add(node2);
				}
				node2=SetNode("Anesthesia");
					node3=SetNode(EnumPermType.AnesthesiaIntakeMeds);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.AnesthesiaControlMeds);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.PatMedicationListEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.PatAllergyListEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.PatProblemListEdit);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Imaging Module
			node=SetNode(EnumPermType.ImagingModule);
				node2=SetNode(EnumPermType.ImageCreate);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.ImageDelete);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.ImageEdit);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.ImageExport);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			#region Manage Module
			node=SetNode(EnumPermType.ManageModule);
				node2=SetNode(EnumPermType.Accounting);
					node3=SetNode(EnumPermType.AccountingCreate);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.AccountingEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.Billing);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.DepositSlips);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.Backup);
				node.Nodes.Add(node2);
				node2=SetNode("Time Card");
					node3=SetNode(EnumPermType.TimecardsEditAll);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.TimecardDeleteEntry);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.ProtectedLeaveAdjustmentEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
				node2=SetNode("Supply Inventory");
					node3=SetNode("Equipment");
						node4=SetNode(EnumPermType.EquipmentSetup);
						node3.Nodes.Add(node4);			
						node4=SetNode(EnumPermType.EquipmentDelete);
						node3.Nodes.Add(node4);
					node2.Nodes.Add(node3);
					node3=SetNode(EnumPermType.SupplierEdit);
					node2.Nodes.Add(node3);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#endregion
			node=SetNode("Merge Tools");
				node2=SetNode(EnumPermType.InsCarrierCombine);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.InsPlanMerge);
				node.Nodes.Add(node2);
				node2=SetNode(EnumPermType.RxMerge);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			node=SetNode("Web Applications");
				node2=SetNode(EnumPermType.MobileWeb);
				node.Nodes.Add(node2);
			treePermissions.Nodes.Add(node);
			#region ODCloud Only Permissions
			if(ODBuild.IsWeb()) {
				node=SetNode("Cloud");
					node2=SetNode(EnumPermType.AllowLoginFromAnyLocation);
					node.Nodes.Add(node2);
				treePermissions.Nodes.Add(node);
			}
			#endregion
			#region HQ Only Permissions
			if(PrefC.IsODHQ) {
				node=SetNode("HQ Only");
					node2=SetNode(EnumPermType.CommlogPersistent);
					node.Nodes.Add(node2);
					node2=SetNode(EnumPermType.SalesTaxAdjEdit);
					node.Nodes.Add(node2);
					node2=SetNode(EnumPermType.HeadmasterSetup);
					node.Nodes.Add(node2);
					node2=SetNode(EnumPermType.FAQEdit);
					node.Nodes.Add(node2);
					node2=SetNode(EnumPermType.FeatureRequestEdit);
					node.Nodes.Add(node2);
					node2=SetNode(EnumPermType.EditReadOnlyTasks);
					node.Nodes.Add(node2);
					node2=SetNode(EnumPermType.ApiAccountEdit);
					node.Nodes.Add(node2);
					node2=SetNode(EnumPermType.TextingAccountEdit);
					node.Nodes.Add(node2);
					node2=SetNode(EnumPermType.PreferenceEditBroadcastMonitor);
					node.Nodes.Add(node2);
					node2=SetNode(EnumPermType.CloudCustomerEdit);
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
			retVal.Tag=EnumPermType.None;
			retVal.ImageIndex=0;
			retVal.SelectedImageIndex=0;
			return retVal;
		}

		///<summary>This just keeps FillTreePermissionsInitial looking cleaner.</summary>
		private TreeNode SetNode(EnumPermType perm) {
			TreeNode retVal = new TreeNode();
			retVal.Text=GroupPermissions.GetDesc(perm);
			retVal.Tag=perm;
			retVal.ImageIndex=1;
			retVal.SelectedImageIndex=1;
			return retVal;
		}

		///<summary>Returns an integer associated to the TreeNode.ImageIndex for the passed in PermType. Determined by the amount of FKeys.</summary>
		private int GetNodeStatusForPerm(EnumPermType permission,List<GroupPermission> listGroupPerms,List<long> listUserGroupNums) {
			List<GroupPermission> listAllPermsForGroup=listGroupPerms.FindAll(x => x.PermType==permission && listUserGroupNums.Contains(x.UserGroupNum) && x.FKey==0);
			List<GroupPermission> listIndividualPermsForGroup=listGroupPerms.FindAll(x => x.PermType==permission && listUserGroupNums.Contains(x.UserGroupNum) && x.FKey!=0);
			//If user group(s) have all FKey of 0, they have all permissions.
			if(listIndividualPermsForGroup.Count==0 && listAllPermsForGroup.Count==listUserGroupNums.Count) {
				return 2;//Check
			}
			//If user group(s) have any individual permissions or not all groups have FKey of 0, they have some permissions.
			else if(listIndividualPermsForGroup.Count > 0 || listAllPermsForGroup.Count > 0) {
				return 3;//Square
			}
			//Otherwise, user group(s) have no permissions.
			else {
				return 1;//Empty
			}
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
			node.Text=GroupPermissions.GetDesc((EnumPermType)node.Tag);
			//get all grouppermissions for the passed-in usergroups
			List<GroupPermission> listGroupPerms = GroupPermissions.GetForUserGroups(listUserGroupNums);
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
					&& listGroupPerms[i].PermType==(EnumPermType)node.Tag) 
				{
					node.ImageIndex=2;
					if(listGroupPerms[i].NewerDate.Year>1880) {
						node.Text+=" ("+Lan.g(this,"if date newer than")+" "+listGroupPerms[i].NewerDate.ToShortDateString()+")";
					}
					else if(listGroupPerms[i].NewerDays>0) {
						node.Text+=" ("+Lan.g(this,"if days newer than")+" "+listGroupPerms[i].NewerDays.ToString()+")";
					}
					if((EnumPermType)node.Tag==EnumPermType.AdjustmentTypeDeny) {
						List<GroupPermission> listGroupPermAdjustmentTypesDenied=GroupPermissions.GetAdjustmentTypeDenyPermsForUserGroup(listGroupPerms[i].UserGroupNum);
						string countAdjustmentTypesDeniedStr=listGroupPermAdjustmentTypesDenied.Count.ToString();
						if(listGroupPermAdjustmentTypesDenied.Any(x => x.FKey==0)) {//All
							countAdjustmentTypesDeniedStr="All";
						}
						node.Text+=" ("+countAdjustmentTypesDeniedStr+" "+Lan.g(this,"adjustment types denied)");
					}
				}
			}
			//Special cases that need to check the FKeys to determine the node status.
			List<EnumPermType> listPermissionsSpecial=new List<EnumPermType>() {
				EnumPermType.AdjustmentTypeDeny,
				EnumPermType.Reports
			};
			if(listPermissionsSpecial.Contains((EnumPermType)node.Tag)) {
				node.ImageIndex=GetNodeStatusForPerm((EnumPermType)node.Tag,listGroupPerms,listUserGroupNums);
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
			long userGroupNum=_listUserGroupNums.First();
			GroupPermission perm;
			for(int i = 0;i<Enum.GetNames(typeof(EnumPermType)).Length;i++) {
				EnumPermType permType=(EnumPermType)i;
				if(permType==EnumPermType.SecurityAdmin
					|| permType==EnumPermType.StartupMultiUserOld
					|| permType==EnumPermType.StartupSingleUserOld
					|| permType==EnumPermType.EhrKeyAdd) 
				{
					continue;
				}
				if(GroupPermissions.DoesPermissionTreatZeroFKeyAsAll(permType)) {
					//Give this user group the all permission (only a singular zero FKey row).
					GroupPermissions.GiveUserGroupPermissionAll(userGroupNum,permType);
					continue;
				}
				//Check to see if the user group already has the permission and only insert one if it isn't present.
				perm=GroupPermissions.GetPerm(userGroupNum,permType);
				if(perm==null) {
					perm=new GroupPermission();
					perm.PermType=permType;
					perm.UserGroupNum=userGroupNum;
					try {
						GroupPermissions.Insert(perm);
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
					}
				}
			}
			FillTreePerm();
		}

		public void SetNone() {
			if(_listUserGroupNums.Count!=1) {
				throw new Exception("SetNone may not be called when multiple usergroups are selected.");
			}
			long userGroupNum=_listUserGroupNums.First();
			GroupPermission perm;
			for(int i = 0;i<Enum.GetNames(typeof(EnumPermType)).Length;i++) {
				EnumPermType permType=(EnumPermType)i;
				if(permType==EnumPermType.SecurityAdmin
					|| permType==EnumPermType.StartupMultiUserOld
					|| permType==EnumPermType.StartupSingleUserOld
					|| permType==EnumPermType.EhrKeyAdd) 
				{
					continue;
				}
				GroupPermissions.DeleteForPermTypeAndUserGroup(permType,userGroupNum);
				//AdjustmentTypeDeny is a permission that denies access to a usergroup when they have this permission, unlike other permissions which grant access when given to a usergroup.
				//When a user clicks 'Set None', they do not want the user group to have any permissions, they want to restrict all access. For AdjustmentTypeDeny permission, this means they
				//do NOT want the user group to have access to any adjustment type. So we need to delete all existing adjustment type deny permissions for this user group, which we do above. 
				//And we want to create a 0 FKey AdjustmentTypeDeny perm because that will indicate the user group does not have access to any adjusment type.
				if(permType==EnumPermType.AdjustmentTypeDeny) {
					//Insert a new permission with a zero FKey.
					GroupPermission groupPermission=new GroupPermission();
					groupPermission.NewerDate=DateTime.MinValue;
					groupPermission.NewerDays=0;
					groupPermission.PermType=permType;
					groupPermission.UserGroupNum=userGroupNum;
					groupPermission.FKey=0;
					GroupPermissions.Insert(groupPermission);
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

		///<summary>Only call this if the PermType has non-zero FKeys and needs to open a form to provide individual permission selection (i.e. Reports, AdjustmentTypeDeny, etc). Otherwise, let treePermissions_MouseDown() take care of the toggling.</summary>
		private void CheckFKeyPermissions(GroupPermission perm,SecurityTreeEventHandler securityTreeEventHandler,object sender,SecurityEventArgs securityEventArgs) {
			//Call an event that bubbles back up to the calling Form. The event returns a dialog result so we know how to continue here.
			DialogResult result=securityTreeEventHandler?.Invoke(sender,securityEventArgs)??DialogResult.Cancel;
			if(result==DialogResult.OK) {
				SecurityLogs.MakeLogEntry(EnumPermType.SecurityAdmin,0,"Permission '"+_clickedPermNode.Tag+"' changes made for '"+UserGroups.GetGroup(perm.UserGroupNum).Description+"'");
			}
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
			List<EnumPermType> listLimitedPermissions=new List<EnumPermType> { 
				EnumPermType.ProcCompleteNote, 
				EnumPermType.ProcCompleteAddAdj,
				EnumPermType.ProcCompleteEditMisc
			};
			GroupPermission perm = new GroupPermission();
			perm.PermType=(EnumPermType)_clickedPermNode.Tag;
			perm.UserGroupNum=_listUserGroupNums.First();
			EnumPermType permEcEo=EnumPermType.ProcExistingEdit;
			SecurityEventArgs securityEventArgs=new SecurityEventArgs(perm);
			//User clicked on a check box.  Do stuff.
			//Reports permission does not toggle and is instead determined by whether the user group has permissions.
			if(perm.PermType==EnumPermType.Reports) {//Reports permission is being checked.
				CheckFKeyPermissions(perm,ReportPermissionChecked,sender,securityEventArgs);
				return;
			}
			//CEMT mode AdjustmentTypeDeny permission toggles between all or no permissions selected. Non-CEMT mode does not toggle and instead opens a Definition Picker window.
			else if(perm.PermType==EnumPermType.AdjustmentTypeDeny) { //AdjustmentTypeDeny permission is being checked
				CheckFKeyPermissions(perm,AdjustmentTypeDenyPermissionChecked,sender,securityEventArgs);
				return;
			}
			if(_clickedPermNode.ImageIndex==1) {//unchecked, so need to add a permission
				if(GroupPermissions.PermTakesDates(perm.PermType)) {
					perm.IsNew=true;
					//Call an event that bubbles back up to the calling Form. The event returns a dialog result so we know how to continue here.
					DialogResult result = GroupPermissionChecked?.Invoke(sender,new SecurityEventArgs(perm))??DialogResult.Cancel;
					if(result==DialogResult.Cancel) {
						treePermissions.EndUpdate();
						return;
					}
				}
				else {
					try {
						GroupPermissions.Insert(perm);
						SecurityLogs.MakeLogEntry(EnumPermType.SecurityAdmin,0,"Permission '"+perm.PermType+"' granted to '"
							+UserGroups.GetGroup(perm.UserGroupNum).Description+"'");
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
						return;
					}
				}
				if(perm.PermType==permEcEo) {
					//Adding ProcExistingEdit, so add ProcComplNote, ProcComplAddAdj, and ProcComplEditMisc.
					foreach(EnumPermType permission in listLimitedPermissions) {
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
							SecurityLogs.MakeLogEntry(EnumPermType.SecurityAdmin,0,Lans.g(this,"Permission ")+"'"+permLimited.PermType+"' "
								+Lans.g(this,"granted to")+" '"+UserGroups.GetGroup(perm.UserGroupNum).Description+"'");
						}
						catch(Exception ex) {
							MessageBox.Show(ex.Message);
							return;
						}
					}
				}
				else if(perm.PermType==EnumPermType.FeeSchedEdit) {
					//When giving a user group the full FeeSchedEdit permission, they get the AllowFeeEditWhileRecivingClaim by default
					GroupPermission permFeeSchedLimited=new GroupPermission();
					permFeeSchedLimited.PermType=EnumPermType.AllowFeeEditWhileReceivingClaim;
					permFeeSchedLimited.UserGroupNum=_listUserGroupNums.First();
					try {
						GroupPermissions.Insert(permFeeSchedLimited);
						SecurityLogs.MakeLogEntry(EnumPermType.SecurityAdmin,0,Lans.g(this,"Permission ")+"'"+permFeeSchedLimited.PermType+"' "
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
					if((EnumPermType)_clickedPermNode.Tag==EnumPermType.AllowFeeEditWhileReceivingClaim && GroupPermissions.HasPermission(_listUserGroupNums.First(),EnumPermType.FeeSchedEdit,0)) {
						//As it stands now, the AllowFeeEditWhileReceivingClaim permission is automatically given if a a user has the full FeeSchedEdit permission.
						//Allowing users to uncheck this permission while stil having the full permission it would lead to confusion
						//Using MsgBox with autotranslate as the descriptions of the permissions should never change, and if they do we would them to be translated.
						MsgBox.Show(this,$"{GroupPermissions.GetDesc(EnumPermType.AllowFeeEditWhileReceivingClaim)} " +
							$"cannot be removed from a user who has {GroupPermissions.GetDesc(EnumPermType.FeeSchedEdit)}. Please remove this permission first.");
						return;
					}
					GroupPermissions.RemovePermission(_listUserGroupNums.First(),(EnumPermType)_clickedPermNode.Tag);
					SecurityLogs.MakeLogEntry(EnumPermType.SecurityAdmin,0,"Permission '"+_clickedPermNode.Tag+"' revoked from '"
						+UserGroups.GetGroup(_listUserGroupNums.First()).Description+"'");
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				if(listLimitedPermissions.Contains(((EnumPermType)_clickedPermNode.Tag))) {
					//Deselecting one of the new limited permissions so deselect ProcExistingEdit permissions if present.
					if(GroupPermissions.HasPermission(_listUserGroupNums.First(),permEcEo,0)) {
						try {
							GroupPermissions.RemovePermission(_listUserGroupNums.First(),permEcEo);
							SecurityLogs.MakeLogEntry(EnumPermType.SecurityAdmin,0,Lans.g(this,"Permission")+" '"+permEcEo+"' "
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
					GroupPermissions.RemovePermission(_listUserGroupNums.First(),(EnumPermType)_clickedPermNode.Tag);
					SecurityLogs.MakeLogEntry(EnumPermType.SecurityAdmin,0,"Permission '"+_clickedPermNode.Tag+"' revoked from '"
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

		///<summary>Allows users to edit the date locks on specific permissions or select adjustment types for AdjustmentTypeDeny permission.
		///Does nothing if in ReadOnly mode or if more than one UserGroup is selected.</summary>
		private void treePermissions_DoubleClick(object sender,System.EventArgs e) {
			if(ReadOnly || _listUserGroupNums.Count!=1) {
				return;
			}
			if(_clickedPermNode==null) {
				return;
			}
			EnumPermType permType = (EnumPermType)_clickedPermNode.Tag;
			//If perm doesn't take a date, or the perm isn't AdjustmentTypeDeny, then the double click should not do anything so we just return.
			if(!GroupPermissions.PermTakesDates(permType) && permType!=EnumPermType.AdjustmentTypeDeny) {
				return;
			}
			GroupPermission perm = GroupPermissions.GetPerm(_listUserGroupNums.First(),(EnumPermType)_clickedPermNode.Tag);
			if(perm==null) {
				return;
			}
			DialogResult result;
			if(perm.PermType==EnumPermType.AdjustmentTypeDeny) {
				//Call an event that bubbles back up to the calling Form. The event returns a dialog result so we know how to continue here.
				result=AdjustmentTypeDenyPermissionChecked?.Invoke(sender,new SecurityEventArgs(perm))??DialogResult.Cancel;
			}
			else{
				//Call an event that bubbles back up to the calling Form. The event returns a dialog result so we know how to continue here.
				result=GroupPermissionChecked?.Invoke(sender,new SecurityEventArgs(perm))??DialogResult.Cancel;
			}
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
