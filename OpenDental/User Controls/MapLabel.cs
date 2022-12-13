using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class MapLabel:DraggableControl {

		#region Member not available in designer.

		public MapArea MapAreaCur=new MapArea();

		#endregion Member not available in designer.

		#region Properties

		private bool _allowEdit=false;
		[Category("Behavior")]
		[Description("Double-click will open editor")]
		public bool AllowEdit {
			get {
				return _allowEdit;
			}
			set {
				_allowEdit=value;
			}
		}

		#endregion
		
		#region Events

		public event EventHandler MapAreaDisplayLabelChanged;

		#endregion

		public MapLabel() {
			InitializeComponent();
		}

		public MapLabel(MapArea displayLabel,Font font,Color foreColor,Color backColor,Point location,int pixelsPerFoot,bool allowDragging,bool allowEdit) :this() {
			displayLabel.ItemType=MapItemType.Label;
			MapAreaCur=displayLabel;
			Font=font;
			ForeColor=foreColor;
			BackColor=backColor;
			Location=location;
			Size=GetDrawingSize(this,pixelsPerFoot);
			AllowDragging=allowDragging;
			AllowEdit=allowEdit;
			Name=MapAreaCur.MapAreaNum.ToString();
		}

		///<summary>Make the control just tall enough to fit the font and the lesser of the user defined width vs the actual width.</summary>
		public static Size GetDrawingSize(MapLabel displayLabel,int pixelsPerFoot) {
			Size controlSize=MapAreaPanel.GetScreenSize(displayLabel.MapAreaCur.Width,displayLabel.MapAreaCur.Height,pixelsPerFoot);
			Size textSize=TextRenderer.MeasureText(displayLabel.MapAreaCur.Description,displayLabel.Font);
			return new Size(Math.Min(controlSize.Width,textSize.Width),textSize.Height);
		}

		private void MapAreaDisplayLabelControl_Paint(object sender,PaintEventArgs e) {
			e.Graphics.TextRenderingHint=TextRenderingHint.AntiAlias;
			using Brush brushBack=new SolidBrush(BackColor);
			using Brush brushFore=new SolidBrush(ForeColor);
			try {
				e.Graphics.FillRectangle(brushBack,this.ClientRectangle);
				StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
				stringFormat.Alignment=StringAlignment.Near;
				stringFormat.LineAlignment=StringAlignment.Center;
				Rectangle rectangle=this.ClientRectangle;
				e.Graphics.DrawString(MapAreaCur.Description,Font,brushFore,rectangle,stringFormat);
			}
			catch { }
		}

		private void MapAreaDisplayLabelControl_DoubleClick(object sender,System.EventArgs e) {
			if(!AllowEdit) {
				return;
			}
			//edit this display label
			using FormMapAreaEdit FormEP=new FormMapAreaEdit();
			FormEP.MapAreaItem=this.MapAreaCur;
			if(FormEP.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			if(MapAreaDisplayLabelChanged!=null) { //let anyone interested know that this display label was edited
				MapAreaDisplayLabelChanged(this,new EventArgs());
			}
		}
	}
}
