using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class DashToothChartLegend:PictureBox,IDashWidgetField {
		public const int DefaultWidth=600;
		public const int DefaultHeight=14;
		private SheetField _sheetField;
		private List<Def> _listDefs;
		public LayoutManagerForms LayoutManager;

		public DashToothChartLegend() {
			InitializeComponent();
		}

		public void PassLayoutManager(LayoutManagerForms layoutManager){
			LayoutManager=layoutManager;
		}

		///<summary></summary>
		public void SetData(PatientDashboardDataEventArgs data,SheetField sheetField) {
			if(sheetField==null) {
				return;
			}
			_sheetField=sheetField;
		}

		public void RefreshData(Patient pat,SheetField sheetField) {
			_sheetField=sheetField;
			_listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors,true);
		}

		public void RefreshView() {
			Image img=new Bitmap(_sheetField.Width,_sheetField.Height);
			Graphics g=Graphics.FromImage(img);
			OpenDentBusiness.SheetPrinting.DrawToothChartLegend(0,0,_sheetField.Width,0,_listDefs,g,null);
			if(Image!=null) {
				Image.Dispose();
			}
			Image=img;
			g.Dispose();
		}
	}
}
