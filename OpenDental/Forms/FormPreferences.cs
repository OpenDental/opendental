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
		//Appointment 
		private UserControlApptGeneral userControlApptGeneral=new UserControlApptGeneral();
		private UserControlApptAppearance userControlApptAppearance=new UserControlApptAppearance();
		//Family
		private UserControlFamilyGeneral userControlFamilyGeneral=new UserControlFamilyGeneral();
		private UserControlFamilyInsurance userControlFamilyInsurance=new UserControlFamilyInsurance();
		//Account
		private UserControlAccountGeneral userControlAccountGeneral=new UserControlAccountGeneral();
		private UserControlAccountAdjustments userControlAccountAdjustments=new UserControlAccountAdjustments();
		private UserControlAccountInsurance userControlAccountInsurance=new UserControlAccountInsurance();
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
		//Server Connections 
		private UserControlServerConnections userControlServerConnections=new UserControlServerConnections();
		//Experimental
		private UserControlExperimentalPrefs userControlExperimentalPrefs=new UserControlExperimentalPrefs();
		///<summary>This is the info icon.</summary>
		private UI.PanelOD panelInfo;
		// <summary>This list is only used in debug mode for some automation of db entries.</summary>
		//private List<PrefInfo2> _listPrefInfos=new List<PrefInfo2>();
		private List<PrefInf> _listPrefInfs=new List<PrefInf>();
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
			Point pointWindowR=panelMain.PointToScreen(new Point(LayoutManager.Scale(476),0));//we only care about x
			Point pointControlUR=control.PointToScreen(new Point(0,control.Top-LayoutManager.Scale(50)));//we only care about y
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
						if(textSearch.Text==""){
							label.BackColor=Color.White;
							continue;
						}
						if(label.Text.ToLower().Contains(textSearch.Text.ToLower())){
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
						if(textSearch.Text==""){
							checkBox.BackColor=Color.White;
							continue;
						}
						if(checkBox.Text.ToLower().Contains(textSearch.Text.ToLower())){
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

		private void butOK_Click(object sender,EventArgs e) {
			//validation is done within each save.
			//One save to db might succeed, and then a subsequent save can fail to validate.  That's ok.
			if(!userControlApptGeneral.SaveApptGeneral()
				|| !userControlApptAppearance.SaveApptAppearance()
				|| !userControlFamilyGeneral.SaveFamilyGeneral()
				|| !userControlFamilyInsurance.SaveFamilyInsurance()
				|| !userControlAccountAdjustments.SaveAccountAdjustments()
				|| !userControlAccountGeneral.SaveAccountGeneral()
				|| !userControlAccountInsurance.SaveAccountInsurance()
				|| !userControlAccountPayments.SaveAccountPayments()
				|| !userControlAccountRecAndRepCharges.SaveAccountRecAndRepCharges()
				|| !userControlTreatPlanGeneral.SaveTreatPlanGeneral()
				|| !userControlTreatPlanFreqLimit.SaveTreatPlanFreqLimit()
				|| !userControlChartGeneral.SaveChartGeneral()
				|| !userControlChartProcedures.SaveChartProcedures()
				|| !userControlImagingGeneral.SaveImagingGeneral()
				|| !userControlManageGeneral.SaveManageGeneral()
				|| !userControlManageBillingStatements.SaveManageBillingStatements()
				|| !userControlServerConnections.SaveServerConnections()
				|| !userControlExperimentalPrefs.SaveExperimentalPrefs())
			{
				return;
			}
			//Special case. If server connection settings were updated, clear the dictionary of connections so it reinitializes all connections the next time it is accessed.
			if(userControlServerConnections.DoClearConnectionDictionary) {
				ConnectionStoreBase.ClearConnectionDictionary();
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Read-Only Server settings have been changed.");
			}
			//Special case. If image module needs to be swapped, force close the program.
			if(userControlImagingGeneral.DoSwapImagingModule) {
				Cursor=Cursors.WaitCursor;
				FormOpenDental.S_ProcessKillCommand();
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPreferences_FormClosing(object sender,FormClosingEventArgs e) {
			if(userControlApptGeneral.Changed
				|| userControlApptAppearance.Changed
				|| userControlFamilyGeneral.Changed
				|| userControlFamilyInsurance.Changed
				|| userControlAccountAdjustments.Changed
				|| userControlAccountGeneral.Changed
				|| userControlAccountInsurance.Changed
				|| userControlAccountPayments.Changed
				|| userControlAccountRecAndRepCharges.Changed
				|| userControlTreatPlanGeneral.Changed
				|| userControlTreatPlanFreqLimit.Changed
				|| userControlChartGeneral.Changed
				|| userControlChartProcedures.Changed
				|| userControlManageGeneral.Changed
				|| userControlManageBillingStatements.Changed
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
			listUserControls.Add(userControlApptGeneral);
			listUserControls.Add(userControlApptAppearance);	
			listUserControls.Add(userControlFamilyGeneral);
			listUserControls.Add(userControlFamilyInsurance);
			listUserControls.Add(userControlAccountGeneral);
			listUserControls.Add(userControlAccountAdjustments);
			listUserControls.Add(userControlAccountInsurance);
			listUserControls.Add(userControlAccountPayments);
			listUserControls.Add(userControlAccountRecAndRepCharges);
			listUserControls.Add(userControlTreatPlanGeneral);
			listUserControls.Add(userControlTreatPlanFreqLimit);
			listUserControls.Add(userControlChartGeneral);
			listUserControls.Add(userControlChartProcedures);
			listUserControls.Add(userControlImagingGeneral);
			listUserControls.Add(userControlManageGeneral);
			listUserControls.Add(userControlManageBillingStatements);
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
			treeNodeChild=new TreeNode("Insurance");
			treeNodeChild.Tag=userControlAccountInsurance;
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
			userControlApptGeneral.FillApptGeneral();
			userControlApptAppearance.FillApptAppearance();
			userControlFamilyGeneral.FillFamilyGeneral();
			userControlFamilyInsurance.FillFamilyInsurance();
			userControlAccountAdjustments.FillAccountAdjustments();
			userControlAccountGeneral.FillAccountGeneral();
			userControlAccountInsurance.FillAccountInsurance();
			userControlAccountPayments.FillAccountPayments();
			userControlAccountRecAndRepCharges.FillAccountRecAndRepCharges();
			userControlTreatPlanGeneral.FillTreatPlanGeneral();
			userControlTreatPlanFreqLimit.FillTreatPlanFreqLimit();
			userControlChartGeneral.FillChartGeneral();
			userControlChartProcedures.FillChartProcedures();
			userControlImagingGeneral.FillImagingGeneral();
			userControlManageGeneral.FillManageGeneral();
			userControlManageBillingStatements.FillManageBillingStatements();
			userControlServerConnections.FillServerConnections();
			userControlExperimentalPrefs.FillExperimentalPrefs();
		}
		#endregion Methods - Private

		#region Methods - Other
		protected override string GetHelpOverride() {
			return treeMain.SelectedNode.Name;
		}
		#endregion Methods - Other
	}
}