using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using PDMPScript;

namespace OpenDental.UI.Design{
	public class TabPageDesigner:ParentControlDesigner{

		public TabPageDesigner(){
			EnableDragDrop(true);
		}

		protected override bool GetHitTest(Point pointScreen){
			Point point=Control.PointToClient(pointScreen);
			TabPage tabPage=Control as TabPage;
			if(tabPage.VerticalScroll.Visible && point.X>tabPage.Width-18){//no scaling since we're in design
				return true;//so user can scroll
			}
			if(tabPage.HorizontalScroll.Visible && point.Y>tabPage.Height-18){
				return true;//so user can scroll
			}
			return false;//mouse should not be handled by control. This allows us to drag rectangles.
		}

		public override SelectionRules SelectionRules {
			get{
				return SelectionRules.Locked;//This prevents user from moving the tabPage
			}
		}

		//public override DesignerVerbCollection Verbs{
			//this is an old feature that was replaced by DesignerActions
			//get{
				//this is a failed attempt to remove the context menu at the upper right (aka Smart Tag)
				//DesignerVerbCollection designerVerbCollection=new DesignerVerbCollection();
				//DesignerVerb designerVerb=new DesignerVerb("TestCommand",TestEventHandler);
				//designerVerbCollection.Add(designerVerb);
				//return designerVerbCollection;
				//return null;
			//}
		//}

		//public override DesignerActionListCollection ActionLists{
			//get{
				//return null;
				//this is another failed attempt to remove the context menu at the upper right (aka Smart Tag)
				//DesignerActionListCollection designerActionListCollection=new DesignerActionListCollection();
				//DesignerActionList designerActionList
				//return designerActionListCollection;
			//}
		//}

		public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType) {
			GlyphCollection glyphCollectionBase=base.GetGlyphs(selectionType);
			GlyphCollection glyphCollection=new GlyphCollection();
			for(int i=0;i<glyphCollectionBase.Count;i++){
				//Behavior behavior=glyphCollectionBase[i].Behavior;
				//behavior.
				if(i==0){//The first one seems to be the Smart Tag that I don't want. No obvious way to test.
					//Hopefully, this will be consistent.
					//The other glyph that we want to keep is the one that shows that this tabPage is selected.
					continue;
				}
				glyphCollection.Add(glyphCollectionBase[i]);
			}
			return glyphCollection;
		}

		//private void TestEventHandler(object sender, EventArgs e){

		//}

	}
}
