using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI {
	///<summary>Analogous to the MS TabPage.</summary>
	[Designer(typeof(Design.TabPageDesigner))]
	//[Browsable(false)]//doesn't hide tabPage from Properties window as hoped
	public class TabPage:Panel{//INotifyPropertyChanged {
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>The rectangle for the tab at the top, not the tab page.</summary>
		public Rectangle rectangleTab;
		private string _text;

		//public event PropertyChangedEventHandler PropertyChanged;
		//https://social.msdn.microsoft.com/Forums/Windowsapps/en-US/8d9b3c2a-9aaa-44c5-be60-0a6070cb78d1/raise-propertychanged-on-observablecollection-when-items-property-is-changed?forum=wpdevelop

		//public new string Text;

		///<summary></summary>
		public TabPage() {
			DoubleBuffered=true;
			Text="tab";
		}

		///<summary>Specify text that will go on the tab.</summary>
		public TabPage(string text) {
			DoubleBuffered=true;
			Text=text;
		}

		protected override void OnLayout(LayoutEventArgs levent){
			if(DesignMode || LayoutManager.IsLayoutMS){
				//MS is responsible for layout while dragging
				base.OnLayout(levent);
				return;
			}
			//We are blocking MS from laying out our TabPage children.
			//This is because MS adds a PerformLayout() for some tabPages at the end of the auto-generated code.
			//I didn't have time to track down why, but this causes problems.  Specifically grids inside of tabs inside of splitters get wildly out of place.
			//I also could not find any way to prevent MS from adding the PerformLayout().
		}

		public override string ToString() {
			return "UI.TabPage2: "+Name;
		}

		///<summary>Warning. This is not supported.  Instead, put an autoscroll panel inside the tab page.</summary>
		[Category("OD")]
		[Description("Warning. This is not supported.  Instead, put an autoscroll panel inside the tab page.")]
		[DefaultValue(false)]
		public new bool AutoScroll{
			get{
				return base.AutoScroll;
			}
			set{
				base.AutoScroll=value;
			}
		}

		///<summary>Default is Empty (0,0,0,0)</summary>
		[Category("Appearance")]
		[Description("Default is Empty (0,0,0,0)")]
		[DefaultValue(typeof(Color),"")]//Color.Empty=0,0,0,0
		[EditorBrowsable(EditorBrowsableState.Always)]//in the collection editor
		public Color ColorTab{get;set;}=Color.Empty;

		///<summary>Not meaningful for this control, so we ignore it.</summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new DockStyle Dock{
			get;
			set;
		}

		///<summary>Not meaningful for this control, so we ignore it.</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Point Location{
			get=>base.Location;
			set=>value=base.Location;
		}

		//consider overriding the Locked property, which was added by designer

		[Category("Appearance")]
		[DefaultValue("")]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(true)]
		public new string Text { 
			get{
				return _text;
			}
			set{
				_text=value;
				if(Parent is null){
					return;
				}
				TabControl tabControl=Parent as TabControl;
				if(tabControl is null){
					return;
				}
				tabControl.LayoutTabs();
				tabControl.Invalidate();
			}
		}


	}
}
