using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI {
	///<summary>Analogous to the MS SplitterPanel.</summary>
	[Designer(typeof(Design.SplitterPanelDesigner))]
	public class SplitterPanel:Panel {
		
		public SplitterPanel() { 
			DoubleBuffered=true;
		}

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

		protected override void OnPaint(PaintEventArgs e){
			base.OnPaint(e);
			if(Parent is null){
				return;
			}
			SplitContainer splitContainer=Parent as SplitContainer;
			if(splitContainer is null){
				return;
			}
			Graphics g = e.Graphics;
			using Pen pen = new Pen(splitContainer.ColorBorder);
			if(splitContainer.Orientation==Orientation.Vertical){
				if(this==splitContainer.Panel1){//Left
					g.DrawLine(pen,Width-1,Height-1,0,Height-1);//bottom
					g.DrawLine(pen,0,Height-1,0,0);//left
					g.DrawLine(pen,0,0,Width-1,0);//top
					if(splitContainer.Panel2Collapsed){
						g.DrawLine(pen,Width-1,0,Width-1,Height-1);//right
					}
				}
				if(this==splitContainer.Panel2){//Right
					g.DrawLine(pen,0,0,Width-1,0);//top
					g.DrawLine(pen,Width-1,0,Width-1,Height-1);//right
					g.DrawLine(pen,Width-1,Height-1,0,Height-1);//bottom
					if(splitContainer.Panel1Collapsed){
						g.DrawLine(pen,0,Height-1,0,0);//left
					}
				}
			}
			else{//horizontal
				if(this==splitContainer.Panel1){//Top
					g.DrawLine(pen,0,Height-1,0,0);//left
					g.DrawLine(pen,0,0,Width-1,0);//top
					g.DrawLine(pen,Width-1,0,Width-1,Height-1);//right
					if(splitContainer.Panel2Collapsed){
						g.DrawLine(pen,Width-1,Height-1,0,Height-1);//bottom
					}
				}
				if(this==splitContainer.Panel2){//Bottom
					g.DrawLine(pen,Width-1,0,Width-1,Height-1);//right
					g.DrawLine(pen,Width-1,Height-1,0,Height-1);//bottom
					g.DrawLine(pen,0,Height-1,0,0);//left
					if(splitContainer.Panel1Collapsed){
						g.DrawLine(pen,0,0,Width-1,0);//top
					}
				}
			}
		}


	}
}
