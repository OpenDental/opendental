using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Lan is short for language.  Used to translate text to another language.</summary>
	public class Lan{

		//strings-----------------------------------------------
		///<summary>Converts a string to the current language.</summary>
		public static string g(string classType,string text) {
			string retVal=Lans.ConvertString(classType,text);
			return retVal;
		}

		///<summary>Converts a string to the current language.</summary>
		public static string g(object sender,string text) {
			string classType=(sender==null ? "All": sender.GetType().Name);
			string retVal=Lans.ConvertString(classType,text);
			return retVal;
		}

		///<summary>C is for control. Translates the text of all menu items to another language.</summary>
		public static void C(Control sender,params Menu[] arrayMenus) {
			string classType=(sender==null ? "All": sender.GetType().Name);
			C(classType,arrayMenus);
		}

		///<summary>C is for control. Translates the text of all menu items to another language.</summary>
		public static void C(string classType,params Menu[] arrayMenus) {
			foreach(Menu mainMenu in arrayMenus) {
				foreach(MenuItem menuItem in mainMenu.MenuItems) {
					TranslateMenuItems(classType,menuItem);
				}
			}
		}

		///<summary>This is recursive</summary>
		private static void TranslateMenuItems(string classType,MenuItem menuItem) {
			//first translate any child menuItems
			foreach(MenuItem menuItemCur in menuItem.MenuItems) {
				TranslateMenuItems(classType,menuItemCur);
			}
			//then this menuitem
			menuItem.Text=Lans.ConvertString(classType,menuItem.Text);
		}

		///<summary>C is for control. Translates the text of all context menu strip items to another language.</summary>
		public static void C(Control sender,params ContextMenuStrip[] arrayMenus) {
			string classType=(sender==null ? "All": sender.GetType().Name);
			C(classType,arrayMenus);
		}

		///<summary>C is for control. Translates the text of all context menu strip items to another language.</summary>
		public static void C(string classType,params ContextMenuStrip[] arrayMenus) {
			foreach(ContextMenuStrip mainMenu in arrayMenus) {
				foreach(ToolStripMenuItem menuItem in mainMenu.Items) {
					TranslateToolStripMenuItems(classType,menuItem);
				}
			}
		}

		///<summary>This is recursive</summary>
		public static void TranslateToolStripMenuItems(string classType,ToolStripMenuItem toolStripMenuItem) {
			//first translate any drop down items
			//Use ToolStripItem for DropDownItems since they can be of different Types. E.g. ToolStripMenuItem, ToolStripSeparator, etc.
			foreach(ToolStripItem dropDownItem in toolStripMenuItem.DropDownItems) {
				if(dropDownItem is ToolStripMenuItem toolStripMenuItemSub) {
					TranslateToolStripMenuItems(classType,toolStripMenuItemSub);
				}
			}
			//then this tool strip menu item
			toolStripMenuItem.Text=Lans.ConvertString(classType,toolStripMenuItem.Text);
		}

		//controls-----------------------------------------------
		///<summary></summary>
		public static void C(string classType,params Control[] arrayControls) {
			C(classType,arrayControls,false);
		}

		///<summary></summary>
		public static void C(Control sender,params Control[] arrayControls) {
			C(sender,arrayControls,false);
		}

		///<summary></summary>
		public static void C(Control sender,Control[] arrayControls,bool isRecursive) {
			string classType=(sender==null ? "All": sender.GetType().Name);
			C(classType,arrayControls,isRecursive);
		}

		///<summary></summary>
		public static void C(string classType,Control[] arrayControls,bool isRecursive) {
			foreach(Control contr in arrayControls) {
				if(contr.GetType()==typeof(UI.GridOD)) {
					TranslateGrid(contr);
					continue;
				}
				else if(contr is TabControl tabControl) {
					C(classType,tabControl);
					continue;
				}
				else if(contr is UI.MenuOD menuOD) {
					menuOD.TranslateMenuItems(classType);
					continue;
				}
				contr.Text=Lans.ConvertString(classType,contr.Text);
				if(isRecursive) {
					Cchildren(classType,contr);
				}
			}
		}

		///<summary>This is recursive, but a little simpler than Fchildren.</summary>
		private static void Cchildren(string classType,Control parent) {
			foreach(Control contr in parent.Controls) {
				if(contr.HasChildren) {
					Cchildren(classType,contr);
				}
				contr.Text=Lans.ConvertString(classType,contr.Text);
			}
		}

		private static void TranslateGrid(Control contr) {
			if(contr.GetType()!=typeof(UI.GridOD)) {
				return;
			}
			UI.GridOD grid=((UI.GridOD)contr);
			grid.Title=Lans.ConvertString(grid.TranslationName,grid.Title);
			foreach(UI.GridColumn col in grid.ListGridColumns) {
				col.Heading=Lans.ConvertString(grid.TranslationName,col.Heading);
			}
			if(grid.ContextMenu!=null) {
				C(grid.TranslationName,grid.ContextMenu);
			}
			if(grid.ContextMenuStrip!=null) {
				C(grid.TranslationName,grid.ContextMenuStrip);
			}
		}

		//tab conrol-----------------------------------------------------------------------------------
		///<summary></summary>
		public static void C(Control sender,params TabControl[] arrayTabControls) {
			string classType=(sender==null ? "All": sender.GetType().Name);
			C(classType,arrayTabControls);
		}

		///<summary></summary>
		public static void C(string classType,params TabControl[] arrayTabControls) {
			foreach(TabControl tabControl in arrayTabControls) {
				foreach(TabPage tabPage in tabControl.TabPages) {
					tabPage.Text=Lans.ConvertString(classType,tabPage.Text);
				}
			}
		}

		//forms----------------------------------------------------------------------------------------
		///<summary>F is for Form. Translates the following controls on the entire form: title Text, labels, buttons, groupboxes, checkboxes, radiobuttons, ODGrid.  Can include a list of controls to exclude. Also puts all the correct controls into the All category (OK,Cancel,Close,Delete,etc).</summary>
		public static void F(Form sender) {
			F(sender,new Control[] { });
		}

		///<summary>F is for Form. Translates the following controls on the entire form: title Text, labels, buttons, groupboxes, checkboxes, radiobuttons, ODGrid.  Can include a list of controls to exclude. Also puts all the correct controls into the All category (OK,Cancel,Close,Delete,etc).</summary>
		public static void F(Form sender,params Control[] exclusions) {
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				return;
			}
			//if(CultureInfo.CurrentCulture.TextInfo.IsRightToLeft) {
			//	sender.RightToLeft=RightToLeft.Yes;//This started failing as we built each custom control without LTR support.
			//	sender.RightToLeftLayout=true;//This started failing when we switched to LayoutManager.
			//}
			//first translate the main title Text on the form:
			if(!Contains(exclusions,sender)) {
				sender.Text=Lans.ConvertString(sender.GetType().Name,sender.Text);
			}
			//then launch the recursive function for all child controls
			Fchildren(sender,sender,exclusions);
		}

		///<summary>Returns true if the contrToFind exists in the supplied contrArray. Used to check the exclusion list of F.</summary>
		private static bool Contains(Control[] contrArray,Control contrToFind) {
			for(int i=0;i<contrArray.Length;i++) {
				if(contrArray[i]==contrToFind) {
					return true;
				}
			}
			return false;
		}

		///<summary>Called from F and also recursively. Translates all children of the given control except those in the exclusions list.</summary>
		private static void Fchildren(Form sender,Control parent,params Control[] exclusions) {
			string senderTypeName=sender.GetType().Name;
			foreach(Control contr in parent.Controls) {
				Type contrType=contr.GetType();
				//Any controls with children of their own.  First so that we always translate children.
				if(contr.HasChildren) {
					Fchildren(sender,contr,exclusions);
				}
				//Process contr
				if(exclusions!=null && Contains(exclusions,contr)) {
					continue;//Do not translate contr because it is excluded.
				}
				//Test to see if the control supports the .Text property.
				//Every control will have a .Text property present but some controls will purposefully throw a NotSupportedException (e.g. WebBrowser).
				try {
					contr.Text=contr.Text;
				}
				catch(Exception) {
					continue;//We cannot translate this control so move on.
				}
				if(contrType==typeof(GroupBox)) {
					ShiftChildControls(contr);
					TranslateControl(senderTypeName,contr);
				}
				else if(contrType==typeof(UI.GridOD)) {
					TranslateGrid(contr);
				}
				else if(contr is UI.MenuOD menuOD) {
					menuOD.TranslateMenuItems(senderTypeName);
				}
				else if(contrType==typeof(Panel)) {
					ShiftChildControls(contr);
				}
				else if(contrType==typeof(TabControl)) {
					//Translate all tab pages on the tab control.
					C(senderTypeName,(TabControl)contr);
				}
				else {
					//Generically try to translate all orther controls not specifically mentioned above.
					TranslateControl(senderTypeName,contr);
				}
			}
		}

		private static void TranslateControl(string classType,Control contr) {
			if(contr.Text=="OK"
					|| contr.Text=="&OK"
					|| contr.Text=="Cancel"
					|| contr.Text=="&Cancel"
					|| contr.Text=="Close"
					|| contr.Text=="&Close"
					|| contr.Text=="Add"
					|| contr.Text=="&Add"
					|| contr.Text=="Delete"
					|| contr.Text=="&Delete"
					|| contr.Text=="Up"
					|| contr.Text=="&Up"
					|| contr.Text=="Down"
					|| contr.Text=="&Down"
					|| contr.Text=="Print"
					|| contr.Text=="&Print") 
			{
				//Maintain the same translation text for the most common text throughout the entire program.
				contr.Text=Lans.ConvertString("All",contr.Text);
			}
			else {
				contr.Text=Lans.ConvertString(classType,contr.Text);
			}
		}

		///<summary>Shift specific controls to the right for certain cultures (not USA).</summary>
		private static void ShiftChildControls(Control contr) {
			if(!CultureInfo.CurrentCulture.TextInfo.IsRightToLeft) {
				return;
			}
			foreach(Control child in contr.Controls) {
				child.Location=new Point(contr.Width-child.Width-child.Left,child.Top);
			}
		}

		public static string[] TranslateArray(string classType,string[] strings){
			string[] retVal=new string[strings.Length];
			for(int i=0;i<strings.Length;i++){
				retVal[i]=g(classType,strings[i]);
			}
			return retVal;
		}

	}
}













