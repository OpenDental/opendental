using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPreferences:FormODBase {

		#region Fields - Private
		//Main Window
		private UserControlMainWindow userControlMainWindow=new UserControlMainWindow();
		private UserControlMainWindowMisc userControlMainWindowMisc=new UserControlMainWindowMisc();
		//Appointment 
		private UserControlApptGeneral userControlApptGeneral=new UserControlApptGeneral();
		private UserControlApptAppearance userControlApptAppearance=new UserControlApptAppearance();
		//Family
		private UserControlFamilyGeneral userControlFamilyGeneral=new UserControlFamilyGeneral();
		private UserControlFamilyInsurance userControlFamilyInsurance=new UserControlFamilyInsurance();
		//Account
		private UserControlAccountGeneral userControlAccountGeneral=new UserControlAccountGeneral();
		private UserControlAccountAdjustments userControlAccountAdjustments=new UserControlAccountAdjustments();
		private UserControlAccountClaimSend userControlAccountClaimSend=new UserControlAccountClaimSend();
		private UserControlAccountClaimReceive userControlAccountClaimReceive=new UserControlAccountClaimReceive();
		private UserControlAccountPayments userControlAccountPayments=new UserControlAccountPayments();
		private UserControlAccountRecAndRepCharges userControlAccountRecAndRepCharges=new UserControlAccountRecAndRepCharges();
		//Treat Plan
		private UserControlTreatPlanGeneral userControlTreatPlanGeneral=new UserControlTreatPlanGeneral();
		private UserControlTreatPlanFreqLimit userControlTreatPlanFreqLimit=new UserControlTreatPlanFreqLimit();
		//Chart
		private UserControlChartGeneral userControlChartGeneral=new UserControlChartGeneral();
		private UserControlChartProcedures userControlChartProcedures=new UserControlChartProcedures();
		//Imaging
		private UserControlImagingGeneral userControlImagingGeneral=new UserControlImagingGeneral();
		//Manage
		private UserControlManageGeneral userControlManageGeneral=new UserControlManageGeneral();
		private UserControlManageBillingStatements userControlManageBillingStatements=new UserControlManageBillingStatements();
		//Ortho
		private UserControlOrtho userControlOrtho=new UserControlOrtho();
		//Enterprise
		private UserControlEnterpriseGeneral userControlEnterpriseGeneral=new UserControlEnterpriseGeneral();
		private UserControlEnterpriseAccount userControlEnterpriseAccount=new UserControlEnterpriseAccount();
		private UserControlEnterpriseAppts userControlEnterpriseAppts=new UserControlEnterpriseAppts();
		private UserControlEnterpriseFamily userControlEnterpriseFamily=new UserControlEnterpriseFamily();
		private UserControlEnterpriseManage userControlEnterpriseManage=new UserControlEnterpriseManage();
		private UserControlEnterpriseReports userControlEnterpriseReports=new UserControlEnterpriseReports();
		//Server Connections 
		private UserControlServerConnections userControlServerConnections=new UserControlServerConnections();
		//Experimental
		private UserControlExperimentalPrefs userControlExperimentalPrefs=new UserControlExperimentalPrefs();
		///<summary>This is the info icon.</summary>
		private UI.PanelOD panelInfo;
		// <summary>This list is only used in debug mode for some automation of db entries.</summary>
		//private List<PrefInfo2> _listPrefInfos=new List<PrefInfo2>();
		private List<PrefInf> _listPrefInfs=new List<PrefInf>();
		///<summary>A reference to this list is passed to each panel that needs it. This just syncs about 15 prefs that are present in multiple panels.</summary>
		private List<PrefValSync> _listPrefValSyncs;
		///<summary>Only tracks the 15 synced prefs.</summary>
		private bool _changed;
		private FormWebBrowserPrefs formWebBrowserPrefs;
		#endregion Fields - Private

		#region Fields - Public
		public int SelectedNode=0;
		#endregion Fields - Public
		
		#region Constructor
		public FormPreferences() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		#endregion Constructor

		#region Methods - Event Handlers
		/*This section should not be deleted, since I use it sometimes for maintenance.
		private void butGenerateXML_Click(object sender,EventArgs e) {
			//this is just temporary
			List<PrefInf> listPrefInfs=new List<PrefInf>();
			for(int i=0;i<_listPrefInfos.Count;i++){
				PrefInf prefInf=new PrefInf();
				prefInf.PrefName=_listPrefInfos[i].PrefName;
				prefInf.Category=_listPrefInfos[i].Category;
				prefInf.ControlName=_listPrefInfos[i].ControlName;
				prefInf.ControlText=_listPrefInfos[i].ControlText;
				prefInf.Details=_listPrefInfos[i].Details;
				listPrefInfs.Add(prefInf);
			}
			XmlSerializer xmlSerializer=new XmlSerializer(listPrefInfs.GetType());
			using StringWriter stringWriter=new StringWriter();
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			using XmlWriter xmlWriter=XmlWriter.Create(stringWriter,xmlWriterSettings);
			xmlSerializer.Serialize(xmlWriter,listPrefInfs);
			xmlWriter.Close();
			string str=stringWriter.ToString();
			string fileName=@"E:\Documents\GIT REPOS\Versioned\OpenDental\OpenDental\Resources\PrefInfos.xml";
			File.WriteAllText(fileName,str);
		}*/

		private void FormModulePrefs_Load(object sender,EventArgs e) {
			FillUserControls();
			if(ODBuild.IsDebug())	{
				LoadUserControls();
			}
			else {
				try {//try/catch used to prevent setup form from partially loading and filling controls.  Causes UEs, Example: TimeCardOvertimeFirstDayOfWeek set to -1 because UI control not filled properly.
					LoadUserControls();
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"An error has occurred while attempting to load preferences.  Run database maintenance and try again."),ex);
					DialogResult=DialogResult.Abort;
					return;
				}
			}
			if(SelectedNode==0) {
				treeMain.SelectedNode=treeMain.Nodes[0];
			}
			else {
				treeMain.SelectedNode=treeMain.Nodes[SelectedNode];
			}
			if(ODBuild.IsDebug() && Environment.MachineName.ToLower().In("jordanhome","jordancryo")){
				//_listPrefInfos=Prefs.GetAllPrefInfos();
			}
			panelInfo=new UI.PanelOD();
			panelInfo.Name="panelInfo";
			panelInfo.Size=new Size(LayoutManager.Scale(16),LayoutManager.Scale(16));
			panelInfo.Visible=false;
			GraphicsPath graphicsPath=new GraphicsPath();
			graphicsPath.AddEllipse(0,0,LayoutManager.ScaleF(14.5f),LayoutManager.ScaleF(14.5f));
			Region region=new Region(graphicsPath);
			panelInfo.Region=region;//This makes it transparent outside the circle, but there's no antialiasing
			panelInfo.Paint+=PanelInfo_Paint;
			LayoutManager.Add(panelInfo,this);
			string strXml=Properties.Resources.PrefInfos;
			MemoryStream memoryStream=new MemoryStream();
			StreamWriter streamWriter=new StreamWriter(memoryStream);
			streamWriter.Write(strXml);
			streamWriter.Flush();
			memoryStream.Position=0;
			using StreamReader streamReader = new StreamReader(memoryStream,Encoding.UTF8,true);
			XmlSerializer xmlSerializer=new XmlSerializer(_listPrefInfs.GetType());
			_listPrefInfs=(List<PrefInf>)xmlSerializer.Deserialize(streamReader);
			List<Control> listControlsAll=GetListControlsFlat(this);
			for(int i=0;i<listControlsAll.Count;i++){
				//we only care about labels and checkboxes, but we add all of them to handle the mouse leaving the ones we care about
				listControlsAll[i].MouseMove+=FormPreferences_MouseMove;
				listControlsAll[i].MouseDown+=FormPreferences_MouseDown;
				if(listControlsAll[i] is UI.CheckBox checkBox){
					//We need to make checkboxes with hoverinfo not have the text be clickable or checks will accidentally change when user clicks to see hover info.
					PrefInf prefInf=GetPrefInf(checkBox);
					if(prefInf!=null && !prefInf.Details.IsNullOrEmpty()){
						checkBox.IsTextClickable=false;
					}
				}
			}
			Left=Left-200;//to make room on the right for the popup info boxes.
			Plugins.HookAddCode(this,"FormModuleSetup.FormModuleSetup_Load_end");
		}

		private void FormPreferences_MouseMove(object sender,MouseEventArgs e){
			Control control=(Control)sender;
			//we only care about labels, checkboxes, and the rare groupbox
			if(!control.GetType().In(typeof(Label),typeof(UI.CheckBox),typeof(UI.GroupBox))){
				panelInfo.Visible=false;
				return;
			}
			if(control is UI.CheckBox checkBox){
				if(!checkBox.IsOnActualText(e.Location)){
					panelInfo.Visible=false;
					return;
				}
			}
			if(control is UI.GroupBox groupBox){
				if(!groupBox.IsOnActualText(e.Location)){
					panelInfo.Visible=false;
					return;
				}
				//panelInfo.Visible=true;//just for testing
				//panelInfo.BringToFront();
				//Point pointScreen2=control.PointToScreen(e.Location);
				//Point pointWindow2=PanelClient.PointToClient(pointScreen2);
				//Point pointShow2=new Point(pointWindow2.X+LayoutManager.Scale(10),pointWindow2.Y-LayoutManager.Scale(10));
				//LayoutManager.MoveLocation(panelInfo,pointShow2);
				//return;
			}
			//Variations of this code are used from time to time to automatically add/edit db items.
			/*
			if(ODBuild.IsDebug() && Environment.MachineName.ToLower().In("jordanhome","jordancryo")){
				Control parent=control.Parent;
				while(true){
					if(parent is null){
						return;
					}
					Type type=parent.GetType();
					if(type.IsSubclassOf(typeof(UserControl))){
						break;
					}
					parent=parent.Parent;
				}
				if(!parent.Name.StartsWith("UserControl")){
					return;
				}
				string category=parent.Name.Substring(11);
				PrefInfo2 prefInfo=_listPrefInfos.Find(x=>x.ControlName==control.Name && x.Category==category);
				if(prefInfo!=null){
					return;
				}
				//this is a new control that has not yet been added to db
				prefInfo=new PrefInfo2();
				prefInfo.Category=category;
				prefInfo.ControlName=control.Name;
				prefInfo.ControlText=control.Text;
				Prefs.PrefInfoInsert(prefInfo);
				_listPrefInfos.Add(prefInfo);
			}*/
			PrefInf prefInf=GetPrefInf(control);
			if(prefInf is null || prefInf.Details.IsNullOrEmpty()){
				panelInfo.Visible=false;
				return;
			}
			panelInfo.Visible=true;
			panelInfo.BringToFront();
			Point pointScreen=control.PointToScreen(e.Location);
			Point pointWindow=PanelClient.PointToClient(pointScreen);
			Point pointShow=new Point(pointWindow.X+LayoutManager.Scale(10),pointWindow.Y-LayoutManager.Scale(10));
			LayoutManager.MoveLocation(panelInfo,pointShow);
		}

		private void FormPreferences_MouseDown(object sender,MouseEventArgs e){
			if(!panelInfo.Visible){
				return;//This keeps it from popping up when clicking on a the box portion of a checkbox
			}
			Control control=(Control)sender;
			if(!control.GetType().In(typeof(Label),typeof(UI.CheckBox),typeof(UI.GroupBox))){
				return;
			}
			PrefInf prefInf=GetPrefInf(control);
			if(prefInf is null){
				return;
			}
			if(prefInf.Details.IsNullOrEmpty()){//empty in db, but comes in as null from xml
				return;
			}
			panelInfo.Visible=false;
			string html="<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head>"
				+"<body style='font-family:sans-serif; font-size:"+(int)LayoutManager.ScaleFontODZoom(11)+"px'>"
				+"<p><b>"+control.Text+"</b></p>"
				+"<p>"+prefInf.Details+"</p>";
			if(prefInf.PrefName!=""){
				html+="<p style=\"background-color:rgb(230,230,234)\">Internal PrefName: "+prefInf.PrefName+"</p>";
			}
			html+="</body><html>";
			if(formWebBrowserPrefs!=null && formWebBrowserPrefs.Visible){
				formWebBrowserPrefs.Close();
			}
			formWebBrowserPrefs=new FormWebBrowserPrefs();
			formWebBrowserPrefs.HtmlContent=html;
			Point pointWindowR=panelMain.PointToScreen(new Point(LayoutManager.Scale(476),0));//we only care about x (# of pixels moved to the right from left of panelMain)
			Point pointControlUR=control.PointToScreen(new Point(0,panelMain.Top-LayoutManager.Scale(50)));//we only care about y (# of pixels moved upwards from top of control)
			Point pointShow=new Point(pointWindowR.X,pointControlUR.Y);
			formWebBrowserPrefs.PointStart=pointShow;
			if(prefInf.WidthWindow>0 && prefInf.HeightWindow>0){
				formWebBrowserPrefs.SizeWindow=new Size(prefInf.WidthWindow,prefInf.HeightWindow);
			}
			formWebBrowserPrefs.Show(this);
		}

		///<summary>Returns null if there was no pref info for this control.</summary>
		private PrefInf GetPrefInf(Control control){
			//first look for controlName
			Control parent=control.Parent;
			while(true){
				if(parent is null){
					return null;
				}
				Type type=parent.GetType();
				if(type.IsSubclassOf(typeof(UserControl))){
					break;
				}
				parent=parent.Parent;
			}
			if(!parent.Name.StartsWith("UserControl")){
				return null;
			}
			string category=parent.Name.Substring(11);
			PrefInf prefInf=_listPrefInfs.Find(x=>x.ControlName==control.Name && x.Category==category);
			if(prefInf==null){
				//Then match by text
				prefInf=_listPrefInfs.Find(x=>x.ControlText==control.Text.Trim());
				if(prefInf is null){
					return null;
				}
			}
			return prefInf;
		}

		private void PanelInfo_Paint(object sender,PaintEventArgs e) {
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			g.Clear(panelInfo.BackColor);
			//This will be basically centered inside of a round control, with an extra pixel around the edges for antialiasing.
			g.FillEllipse(Brushes.Blue,1.5f,1.5f,LayoutManager.ScaleF(12),LayoutManager.ScaleF(12));
			using Font font=new Font("Times New Roman",LayoutManager.ScaleFontODZoom(10f),FontStyle.Bold);
			//I need the i to be exactly centered, so DrawString won't cut it.
			using GraphicsPath graphicsPath=new GraphicsPath();
			float fontSize=LayoutManager.ScaleF(12.5f);			
			graphicsPath.AddString("i",FontFamily.GenericSerif,(int)FontStyle.Bold,fontSize,Point.Empty,StringFormat.GenericTypographic);
			RectangleF rectangleFBounds=graphicsPath.GetBounds();
			g.TranslateTransform(1.5f+LayoutManager.ScaleF(6),1.5f+LayoutManager.ScaleF(6));//to center of ellipse
			g.TranslateTransform(-rectangleFBounds.Width/2f-rectangleFBounds.X,-rectangleFBounds.Height/2f-rectangleFBounds.Y);//to UL of text
			g.FillPath(Brushes.White,graphicsPath);
			g.ResetTransform();
		}

		private void textSearch_TextChanged(object sender,EventArgs e) {
			//loop through the different categories
			List<TreeNode> listTreeNodes=GetListNodesFlat();
			for(int i=0;i<listTreeNodes.Count;i++) {
				bool isCategoryYellow=false;
				if(textSearch.Text!="" && listTreeNodes[i].Text.ToLower().Contains(textSearch.Text.ToLower())){
					isCategoryYellow=true;
				}
				UserControl userControl=(UserControl)listTreeNodes[i].Tag;
				List<Control> listControls=GetListControlsFlat(userControl);
				//labels
				for(int c=0;c<listControls.Count;c++){
					if(listControls[c] is Label label){
						if(listControls[c].Visible==false){
							continue;
						}
						if(textSearch.Text==""){
							label.BackColor=Color.White;
							continue;
						}
						PrefInf prefInf=GetPrefInf(label);
						if(label.Text.ToLower().Contains(textSearch.Text.ToLower())
							|| (prefInf!=null && !prefInf.Details.IsNullOrEmpty() && prefInf.Details.ToLower().Contains(textSearch.Text.ToLower())))
						{
							isCategoryYellow=true;
							label.BackColor=Color.FromArgb(255, 255, 192);
						}
						else{
							label.BackColor=Color.White;
						}
					}
				}
				//checkboxes
				for(int c=0;c<listControls.Count;c++){
					if(listControls[c] is UI.CheckBox checkBox){
						if(listControls[c].Visible==false){
							continue;
						}
						if(textSearch.Text==""){
							checkBox.BackColor=Color.White;
							continue;
						}
						PrefInf prefInf=GetPrefInf(checkBox);
						if(checkBox.Text.ToLower().Contains(textSearch.Text.ToLower()) 
							|| (prefInf!=null && !prefInf.Details.IsNullOrEmpty() && prefInf.Details.ToLower().Contains(textSearch.Text.ToLower())))
						{
							isCategoryYellow=true;
							checkBox.BackColor=Color.FromArgb(255, 255, 192);
						}
						else{
							checkBox.BackColor=Color.White;
						}
					}
				}
				//groupboxes
				for(int c=0;c<listControls.Count;c++){
					if(listControls[c] is UI.GroupBox groupBox){
						if(listControls[c].Visible==false){
							continue;
						}
						if(textSearch.Text==""){
							groupBox.ColorBackLabel=Color.Empty;
							continue;
						}
						if(groupBox.Text.ToLower().Contains(textSearch.Text.ToLower())){
							isCategoryYellow=true;
							groupBox.ColorBackLabel=Color.FromArgb(255, 255, 192);
						}
						else{
							groupBox.ColorBackLabel=Color.Empty;
						}
					}
				}
				if(isCategoryYellow){
					listTreeNodes[i].BackColor=Color.FromArgb(255, 255, 192);
				}
				else{
					listTreeNodes[i].BackColor=Color.Empty;
				}
			}
		}

		///<summary>Gets a flat list of all tree nodes</summary>
		private List<TreeNode> GetListNodesFlat(){
			List<TreeNode> listTreeNodes=new List<TreeNode>();
			for(int i=0;i<treeMain.Nodes.Count;i++){
				listTreeNodes.Add(treeMain.Nodes[i]);
				listTreeNodes.AddRange(GetChildrenFlat(treeMain.Nodes[i]));
			}
			return listTreeNodes;
		}

		///<summary>Recursive. Returns empty list when no children.</summary>
		private List<TreeNode> GetChildrenFlat(TreeNode treeNode){
			List<TreeNode> listTreeNodes=new List<TreeNode>();
			for(int i=0;i<treeNode.Nodes.Count;i++){
				listTreeNodes.Add(treeNode.Nodes[i]);//all direct children
				listTreeNodes.AddRange(GetChildrenFlat(treeNode.Nodes[i]));//and recursively, their children
			}
			return listTreeNodes;
		}
		
		///<summary>Gets a flat list of this control and all of its child controls. Recursive.</summary>
		private List<Control> GetListControlsFlat(Control control){
			List<Control> listControls=new List<Control>();
			listControls.Add(control);//self
			for(int i=0;i<control.Controls.Count;i++){
				listControls.AddRange(GetListControlsFlat(control.Controls[i]));//all children, recursively
			}
			return listControls;
		}

		private void treeMain_AfterSelect(object sender,TreeViewEventArgs e) {
			UserControl userControlSelected=treeMain.SelectedNode.Tag as UserControl;
			if(userControlSelected==null) {
				return;
			}
			userControlSelected.BringToFront();
		}

		private void treeMain_BeforeCollapse(object sender,TreeViewCancelEventArgs e) {
			e.Cancel=true;//Never allow the tree to collapse
		}

		private void butSave_Click(object sender,EventArgs e) {
			//validation is done within each save.
			//One save to db might succeed, and then a subsequent save can fail to validate. That's ok.
			if(!userControlMainWindow.SaveMainWindow()
				||!userControlMainWindowMisc.SaveMainWindowMisc()
				||!userControlApptGeneral.SaveApptGeneral()
				|| !userControlApptAppearance.SaveApptAppearance()
				|| !userControlFamilyGeneral.SaveFamilyGeneral()
				|| !userControlFamilyInsurance.SaveFamilyInsurance()
				|| !userControlAccountAdjustments.SaveAccountAdjustments()
				|| !userControlAccountGeneral.SaveAccountGeneral()
				|| !userControlAccountClaimSend.SaveAccountClaimSend()
				|| !userControlAccountClaimReceive.SaveAccountClaimReceive()
				|| !userControlAccountPayments.SaveAccountPayments()
				|| !userControlAccountRecAndRepCharges.SaveAccountRecAndRepCharges()
				|| !userControlTreatPlanGeneral.SaveTreatPlanGeneral()
				|| !userControlTreatPlanFreqLimit.SaveTreatPlanFreqLimit()
				|| !userControlChartGeneral.SaveChartGeneral()
				|| !userControlChartProcedures.SaveChartProcedures()
				|| !userControlImagingGeneral.SaveImagingGeneral()
				|| !userControlManageGeneral.SaveManageGeneral()
				|| !userControlManageBillingStatements.SaveManageBillingStatements()
				|| !userControlOrtho.SaveOrtho()
				|| !userControlEnterpriseGeneral.SaveEnterpriseGeneral()
				|| !userControlEnterpriseAccount.SaveEnterpriseAccount()
				|| !userControlEnterpriseAppts.SaveEnterpriseAppts()
				|| !userControlEnterpriseFamily.SaveEnterpriseFamily()
				|| !userControlEnterpriseManage.SaveEnterpriseManage()
				|| !userControlEnterpriseReports.SaveEnterpriseReports()
				|| !userControlServerConnections.SaveServerConnections()
				|| !userControlExperimentalPrefs.SaveExperimentalPrefs()
				|| !SaveSyncedPrefs())
			{
				return;
			}
//todo: add DataValid.SetInvalid(InvalidType.Prefs); here when we remove FormClosing
			//Special case. If server connection settings were updated, clear the dictionary of connections so it reinitializes all connections the next time it is accessed.
			if(userControlServerConnections.DoClearConnectionDictionary) {
				DataValid.SetInvalid(InvalidType.ConnectionStoreClear);
				SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"Read-Only Server settings have been changed.");
			}
			//Special case. If image module needs to be swapped, force close the program.
			if(userControlImagingGeneral.DoSwapImagingModule) {
				Cursor=Cursors.WaitCursor;
				FormOpenDental.S_ProcessKillCommand();
			}
			DialogResult=DialogResult.OK;
		}

		private void FormPreferences_FormClosing(object sender,FormClosingEventArgs e) {
			//todo: this entire method needs to go away.
			//Changed should be a local variable within the various Save methods.
			if(userControlMainWindow.Changed
				|| userControlMainWindowMisc.Changed
				|| userControlApptGeneral.Changed
				|| userControlApptAppearance.Changed
				|| userControlFamilyGeneral.Changed
				|| userControlFamilyInsurance.Changed
				|| userControlAccountAdjustments.Changed
				|| userControlAccountGeneral.Changed
				|| userControlAccountClaimSend.Changed
				|| userControlAccountClaimReceive.Changed
				|| userControlAccountPayments.Changed
				|| userControlAccountRecAndRepCharges.Changed
				|| userControlTreatPlanGeneral.Changed
				|| userControlTreatPlanFreqLimit.Changed
				|| userControlChartGeneral.Changed
				|| userControlChartProcedures.Changed
				|| userControlManageGeneral.Changed
				|| userControlManageBillingStatements.Changed
				|| userControlOrtho.Changed
				|| userControlEnterpriseGeneral.Changed
				|| userControlEnterpriseAccount.Changed
				|| userControlEnterpriseAppts.Changed
				|| userControlEnterpriseFamily.Changed
				|| userControlEnterpriseManage.Changed
				|| userControlEnterpriseReports.Changed
				|| userControlServerConnections.Changed
				|| userControlExperimentalPrefs.Changed
				|| userControlImagingGeneral.Changed)
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		///<summary>Create one control per menu item using the LayoutManager.</summary>
		private void FillUserControls() {
			List<UserControl> listUserControls=new List<UserControl>();
			listUserControls.Add(userControlMainWindow);
			listUserControls.Add(userControlMainWindowMisc);
			listUserControls.Add(userControlApptGeneral);
			listUserControls.Add(userControlApptAppearance);	
			listUserControls.Add(userControlFamilyGeneral);
			listUserControls.Add(userControlFamilyInsurance);
			listUserControls.Add(userControlAccountGeneral);
			listUserControls.Add(userControlAccountAdjustments);
			listUserControls.Add(userControlAccountClaimSend);
			listUserControls.Add(userControlAccountClaimReceive);
			listUserControls.Add(userControlAccountPayments);
			listUserControls.Add(userControlAccountRecAndRepCharges);
			listUserControls.Add(userControlTreatPlanGeneral);
			listUserControls.Add(userControlTreatPlanFreqLimit);
			listUserControls.Add(userControlChartGeneral);
			listUserControls.Add(userControlChartProcedures);
			listUserControls.Add(userControlImagingGeneral);
			listUserControls.Add(userControlManageGeneral);
			listUserControls.Add(userControlManageBillingStatements);
			listUserControls.Add(userControlOrtho);
			listUserControls.Add(userControlEnterpriseGeneral);
			listUserControls.Add(userControlEnterpriseAccount);
			listUserControls.Add(userControlEnterpriseAppts);
			listUserControls.Add(userControlEnterpriseFamily);
			listUserControls.Add(userControlEnterpriseManage);
			listUserControls.Add(userControlEnterpriseReports);
			listUserControls.Add(userControlServerConnections);
			listUserControls.Add(userControlExperimentalPrefs);
			for(int i=0;i<listUserControls.Count;i++) {
				listUserControls[i].Dock=DockStyle.Fill;
				LayoutManager.AddUnscaled(listUserControls[i],panelMain);
			}
			LayoutManager.LayoutControlBoundsAndFonts(panelMain);
			FillTree();
		}

		///<summary>Fills the MenuTree, make sure that the Tag is the exact same name as the control.</summary>
		private void FillTree() {
			TreeNode treeNodeParent;
			TreeNode treeNodeChild;
			//MainWindow
			treeNodeParent=new TreeNode("Main Window - General");
			treeNodeParent.Tag=userControlMainWindow;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Miscellaneous");
			treeNodeChild.Tag=userControlMainWindowMisc;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Appointment
			treeNodeParent=new TreeNode("Appointment - General");
			treeNodeParent.Tag=userControlApptGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Appearance");
			treeNodeChild.Tag=userControlApptAppearance;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Family
			treeNodeParent=new TreeNode("Family - General");
			treeNodeParent.Tag=userControlFamilyGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Insurance");
			treeNodeChild.Tag=userControlFamilyInsurance;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Account
			treeNodeParent=new TreeNode("Account - General");
			treeNodeParent.Tag=userControlAccountGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Adjustments");
			treeNodeChild.Tag=userControlAccountAdjustments;
			treeNodeParent.Nodes.Add(treeNodeChild);
			treeNodeChild=new TreeNode("Claim - Send");
			treeNodeChild.Tag=userControlAccountClaimSend;
			treeNodeParent.Nodes.Add(treeNodeChild);
			treeNodeChild=new TreeNode("Claim - Receive");
			treeNodeChild.Tag=userControlAccountClaimReceive;
			treeNodeParent.Nodes.Add(treeNodeChild);
			treeNodeChild=new TreeNode("Payments");
			treeNodeChild.Tag=userControlAccountPayments;
			treeNodeParent.Nodes.Add(treeNodeChild);
			treeNodeChild=new TreeNode("Recurring and Repeating Charges");
			treeNodeChild.Tag=userControlAccountRecAndRepCharges;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Treat Plan
			treeNodeParent=new TreeNode("Treat Plan - General");
			treeNodeParent.Tag=userControlTreatPlanGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Frequency Limitations");
			treeNodeChild.Tag=userControlTreatPlanFreqLimit;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Chart
			treeNodeParent=new TreeNode("Chart - General");
			treeNodeParent.Tag=userControlChartGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Procedures");
			treeNodeChild.Tag=userControlChartProcedures;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Imaging
			treeNodeParent=new TreeNode("Imaging - General");
			treeNodeParent.Tag=userControlImagingGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			//Manage
			treeNodeParent=new TreeNode("Manage - General");
			treeNodeParent.Tag=userControlManageGeneral;
			treeMain.Nodes.Add(treeNodeParent);
			treeNodeChild=new TreeNode("Billing Statements");
			treeNodeChild.Tag=userControlManageBillingStatements;
			treeNodeParent.Nodes.Add(treeNodeChild);
			//Ortho
			treeNodeParent=new TreeNode("Ortho");
			treeNodeParent.Tag=userControlOrtho;
			treeMain.Nodes.Add(treeNodeParent);
			//Enterprise
			if(PrefC.GetBool(PrefName.ShowFeatureEnterprise)) {//This pref must be true for Enterprise to be present in tree
				treeNodeParent=new TreeNode("Enterprise - General");
				treeNodeParent.Tag=userControlEnterpriseGeneral;
				treeMain.Nodes.Add(treeNodeParent);
				treeNodeChild=new TreeNode("Account");
				treeNodeChild.Tag=userControlEnterpriseAccount;
				treeNodeParent.Nodes.Add(treeNodeChild);
				treeNodeChild=new TreeNode("Appts");
				treeNodeChild.Tag=userControlEnterpriseAppts;
				treeNodeParent.Nodes.Add(treeNodeChild);
				treeNodeChild=new TreeNode("Family");
				treeNodeChild.Tag=userControlEnterpriseFamily;
				treeNodeParent.Nodes.Add(treeNodeChild);
				treeNodeChild=new TreeNode("Manage");
				treeNodeChild.Tag=userControlEnterpriseManage;
				treeNodeParent.Nodes.Add(treeNodeChild);
				treeNodeChild=new TreeNode("Reports");
				treeNodeChild.Tag=userControlEnterpriseReports;
				treeNodeParent.Nodes.Add(treeNodeChild);
			}
			//Server Connections 
			treeNodeParent=new TreeNode("Server Connections - General");
			treeNodeParent.Tag=userControlServerConnections;
			treeMain.Nodes.Add(treeNodeParent);
			//Experimental
			treeNodeParent=new TreeNode("Experimental");
			treeNodeParent.Tag=userControlExperimentalPrefs;
			treeMain.Nodes.Add(treeNodeParent);
			treeMain.ExpandAll();
		}

		///<summary>Load all of the preferences of each control.</summary>
		private void LoadUserControls() {
			//Build the sync list
			_listPrefValSyncs=new List<PrefValSync>();
			PrefValSync prefValSync;
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.ProcessSigsIntervalInSecs;
			prefValSync.PrefVal=PrefC.GetString(PrefName.ProcessSigsIntervalInSecs);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.SignalInactiveMinutes;
			prefValSync.PrefVal=PrefC.GetString(PrefName.SignalInactiveMinutes);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.PatientPhoneUsePhonenumberTable;
			prefValSync.PrefVal=PrefC.GetString(PrefName.PatientPhoneUsePhonenumberTable);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.PatientSelectFilterRestrictedClinics;
			prefValSync.PrefVal=PrefC.GetString(PrefName.PatientSelectFilterRestrictedClinics);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.EnterpriseAllowRefreshWhileTyping;
			prefValSync.PrefVal=PrefC.GetString(PrefName.EnterpriseAllowRefreshWhileTyping);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.AgingServiceTimeDue;
			prefValSync.PrefVal=PrefC.GetString(PrefName.AgingServiceTimeDue);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.PaymentClinicSetting;
			prefValSync.PrefVal=PrefC.GetString(PrefName.PaymentClinicSetting);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.PaymentsPromptForPayType;
			prefValSync.PrefVal=PrefC.GetString(PrefName.PaymentsPromptForPayType);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.ClaimIdPrefix;
			prefValSync.PrefVal=PrefC.GetString(PrefName.ClaimIdPrefix);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.PayPlansVersion;
			prefValSync.PrefVal=PrefC.GetString(PrefName.PayPlansVersion);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.BillingElectBatchMax;
			prefValSync.PrefVal=PrefC.GetString(PrefName.BillingElectBatchMax);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.BillingShowSendProgress;
			prefValSync.PrefVal=PrefC.GetString(PrefName.BillingShowSendProgress);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.ApptsRequireProc;
			prefValSync.PrefVal=PrefC.GetString(PrefName.ApptsRequireProc);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.ApptSecondaryProviderConsiderOpOnly;
			prefValSync.PrefVal=PrefC.GetString(PrefName.ApptSecondaryProviderConsiderOpOnly);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.ClaimSnapshotRunTime;
			prefValSync.PrefVal=PrefC.GetString(PrefName.ClaimSnapshotRunTime);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.ClaimSnapshotTriggerType;
			prefValSync.PrefVal=PrefC.GetString(PrefName.ClaimSnapshotTriggerType);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.ShowFeatureSuperfamilies;
			prefValSync.PrefVal=PrefC.GetString(PrefName.ShowFeatureSuperfamilies);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.CloneCreateSuperFamily;
			prefValSync.PrefVal=PrefC.GetString(PrefName.CloneCreateSuperFamily);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.ReportingServerCompName;
			prefValSync.PrefVal=PrefC.GetString(PrefName.ReportingServerCompName);
			_listPrefValSyncs.Add(prefValSync);
			prefValSync=new PrefValSync();
			prefValSync.PrefName_=PrefName.ReportingServerURI;
			prefValSync.PrefVal=PrefC.GetString(PrefName.ReportingServerURI);
			_listPrefValSyncs.Add(prefValSync);
			//Pass the sync list to each control that needs it
			userControlMainWindow.ListPrefValSyncs=_listPrefValSyncs;
			userControlMainWindowMisc.ListPrefValSyncs=_listPrefValSyncs;
			userControlApptGeneral.ListPrefValSyncs=_listPrefValSyncs;
			userControlApptAppearance.ListPrefValSyncs=_listPrefValSyncs;
			userControlFamilyGeneral.ListPrefValSyncs=_listPrefValSyncs;
			userControlAccountGeneral.ListPrefValSyncs=_listPrefValSyncs;
			userControlAccountClaimSend.ListPrefValSyncs=_listPrefValSyncs;
			userControlAccountClaimReceive.ListPrefValSyncs=_listPrefValSyncs;
			userControlAccountPayments.ListPrefValSyncs=_listPrefValSyncs;
			userControlManageBillingStatements.ListPrefValSyncs=_listPrefValSyncs;
			userControlEnterpriseGeneral.ListPrefValSyncs=_listPrefValSyncs;
			userControlEnterpriseAccount.ListPrefValSyncs=_listPrefValSyncs;
			userControlEnterpriseAppts.ListPrefValSyncs=_listPrefValSyncs;
			userControlEnterpriseFamily.ListPrefValSyncs=_listPrefValSyncs;
			userControlEnterpriseReports.ListPrefValSyncs=_listPrefValSyncs;
			//Add events
			userControlMainWindow.SyncChanged+=UserControl_SyncChanged;
			userControlMainWindowMisc.SyncChanged+=UserControl_SyncChanged;
			userControlApptGeneral.SyncChanged+=UserControl_SyncChanged;
			userControlApptAppearance.SyncChanged+=UserControl_SyncChanged;
			userControlFamilyGeneral.SyncChanged+=UserControl_SyncChanged;
			userControlAccountGeneral.SyncChanged+=UserControl_SyncChanged;
			userControlAccountClaimSend.SyncChanged+=UserControl_SyncChanged;
			userControlAccountClaimReceive.SyncChanged+=UserControl_SyncChanged;
			userControlAccountPayments.SyncChanged+=UserControl_SyncChanged;
			userControlManageBillingStatements.SyncChanged+=UserControl_SyncChanged;
			userControlEnterpriseGeneral.SyncChanged+=UserControl_SyncChanged;
			userControlEnterpriseAccount.SyncChanged+=UserControl_SyncChanged;
			userControlEnterpriseAppts.SyncChanged+=UserControl_SyncChanged;
			userControlEnterpriseFamily.SyncChanged+=UserControl_SyncChanged;
			userControlEnterpriseReports.SyncChanged+=UserControl_SyncChanged;
			//Fill all the usercontrols.
			userControlMainWindow.FillWindowMain();
			userControlMainWindowMisc.FillMainWindowMisc();
			userControlApptGeneral.FillApptGeneral();
			userControlApptAppearance.FillApptAppearance();
			userControlFamilyGeneral.FillFamilyGeneral();
			userControlFamilyInsurance.FillFamilyInsurance();
			userControlAccountAdjustments.FillAccountAdjustments();
			userControlAccountGeneral.FillAccountGeneral();
			userControlAccountClaimSend.FillAccountClaimSend();
			userControlAccountClaimReceive.FillAccountClaimReceive();
			userControlAccountPayments.FillAccountPayments();
			userControlAccountRecAndRepCharges.FillAccountRecAndRepCharges();
			userControlTreatPlanGeneral.FillTreatPlanGeneral();
			userControlTreatPlanFreqLimit.FillTreatPlanFreqLimit();
			userControlChartGeneral.FillChartGeneral();
			userControlChartProcedures.FillChartProcedures();
			userControlImagingGeneral.FillImagingGeneral();
			userControlManageGeneral.FillManageGeneral();
			userControlManageBillingStatements.FillManageBillingStatements();
			userControlOrtho.FillOrtho();
			userControlEnterpriseGeneral.FillEnterpriseGeneral();
			userControlEnterpriseAccount.FillEnterpriseAccount();
			userControlEnterpriseAppts.FillEnterpriseAppts();
			userControlEnterpriseFamily.FillEnterpriseFamily();
			userControlEnterpriseManage.FillEnterpriseManage();
			userControlEnterpriseReports.FillEnterpriseReports();
			userControlServerConnections.FillServerConnections();
			userControlExperimentalPrefs.FillExperimentalPrefs();
			FillSynced();
		}

		private void UserControl_SyncChanged(object sender,EventArgs e) {
			FillSynced();
		}

		private void FillSynced(){
			userControlMainWindow.FillSynced();
			userControlMainWindowMisc.FillSynced();
			userControlApptGeneral.FillSynced();
			userControlApptAppearance.FillSynced();
			userControlFamilyGeneral.FillSynced();
			userControlAccountGeneral.FillSynced();
			userControlAccountClaimSend.FillSynced();
			userControlAccountPayments.FillSynced();
			userControlManageBillingStatements.FillSynced();
			userControlEnterpriseGeneral.FillSynced();
			userControlEnterpriseAccount.FillSynced();
			userControlEnterpriseAppts.FillSynced();
			userControlEnterpriseFamily.FillSynced();
			userControlEnterpriseReports.FillSynced();
		}

		private bool SaveSyncedPrefs(){
			PrefValSync prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.ProcessSigsIntervalInSecs);
			_changed|=Prefs.UpdateString(PrefName.ProcessSigsIntervalInSecs,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.SignalInactiveMinutes);
			_changed|=Prefs.UpdateString(PrefName.SignalInactiveMinutes,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.PatientPhoneUsePhonenumberTable);
			_changed|=Prefs.UpdateString(PrefName.PatientPhoneUsePhonenumberTable,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.PatientSelectFilterRestrictedClinics);
			_changed|=Prefs.UpdateString(PrefName.PatientSelectFilterRestrictedClinics,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.EnterpriseAllowRefreshWhileTyping);
			_changed|=Prefs.UpdateString(PrefName.EnterpriseAllowRefreshWhileTyping,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.AgingServiceTimeDue);
			_changed|=Prefs.UpdateString(PrefName.AgingServiceTimeDue,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.PaymentClinicSetting);
			_changed|=Prefs.UpdateString(PrefName.PaymentClinicSetting,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.PaymentsPromptForPayType);
			_changed|=Prefs.UpdateString(PrefName.PaymentsPromptForPayType,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimIdPrefix);
			_changed|=Prefs.UpdateString(PrefName.ClaimIdPrefix,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.PayPlansVersion);
			_changed|=Prefs.UpdateString(PrefName.PayPlansVersion,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.BillingElectBatchMax);
			_changed|=Prefs.UpdateString(PrefName.BillingElectBatchMax,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.BillingShowSendProgress);
			_changed|=Prefs.UpdateString(PrefName.BillingShowSendProgress,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.ApptsRequireProc);
			_changed|=Prefs.UpdateString(PrefName.ApptsRequireProc,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.ApptSecondaryProviderConsiderOpOnly);
			_changed|=Prefs.UpdateString(PrefName.ApptSecondaryProviderConsiderOpOnly,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotRunTime);
			_changed|=Prefs.UpdateString(PrefName.ClaimSnapshotRunTime,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimSnapshotTriggerType);
			_changed|=Prefs.UpdateString(PrefName.ClaimSnapshotTriggerType,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.ShowFeatureSuperfamilies);
			_changed|=Prefs.UpdateString(PrefName.ShowFeatureSuperfamilies,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.CloneCreateSuperFamily);
			_changed|=Prefs.UpdateString(PrefName.CloneCreateSuperFamily,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.ReportingServerCompName);
			_changed|=Prefs.UpdateString(PrefName.ReportingServerCompName,prefValSync.PrefVal);
			prefValSync=_listPrefValSyncs.Find(x=>x.PrefName_==PrefName.ReportingServerURI);
			_changed|=Prefs.UpdateString(PrefName.ReportingServerURI,prefValSync.PrefVal);
			return true;//todo: all the similar methods return true like this.
			//But that's wrong. See notes in FormClosing and butSave_Click.
		}
		#endregion Methods - Private

		#region Methods - Other
		protected override string GetHelpOverride() {
			return treeMain.SelectedNode.Name;
		}
		#endregion Methods - Other
	}
}

public class PrefValSync{
	public PrefName PrefName_;
	public string PrefVal;
}
