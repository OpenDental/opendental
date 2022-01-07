using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace UnitTests{
	public partial class FormApptLayoutTests : Form{
		private List<ScenarioApptUnitTest> _listScenarios;
		private int _widthTotal=300;
		private int _heightTotal=300;
		private float _heightLine=12f;

		public FormApptLayoutTests(){
			InitializeComponent();
		}

		private void FormApptLayoutTests_Load(object sender, EventArgs e){
			_listScenarios=new List<ScenarioApptUnitTest>();
			//These tests won't be documented, so the numbering doesn't have to fit into our bigger numbering scheme
			_listScenarios.Add(Test1());
			_listScenarios.Add(Test2());
			_listScenarios.Add(Test3());
			_listScenarios.Add(Test4());
			_listScenarios.Add(Test5());
			_listScenarios.Add(Test6());
			_listScenarios.Add(Test7());
			_listScenarios.Add(Test8());
			_listScenarios.Add(Test9());
			_listScenarios.Add(Test10());
			_listScenarios.Add(Test11());
			_listScenarios.Add(Test12());
			_listScenarios.Add(Test13());
			_listScenarios.Add(Test14());
			_listScenarios.Add(Test15());
			_listScenarios.Add(Test16());
			_listScenarios.Add(Test17());
			_listScenarios.Add(Test18());
			
			RunTests();
			//only gets filled on load
			FillGrid();
			
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Result",55);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Description",500);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			GridCell cell;
			for(int i=0;i<_listScenarios.Count;i++){
				row=new GridRow();
				if(_listScenarios[i].ResultExpected==_listScenarios[i].ResultObtained){
					cell=new GridCell("PASSED");
					cell.ColorBackG=Color.LimeGreen;
					row.Cells.Add(cell);
				}
				else{
					cell=new GridCell("FAILED");
					cell.ColorBackG=Color.Red;
					row.Cells.Add(cell);
				}
				row.Cells.Add(_listScenarios[i].DescriptionShort);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void RunTests(){
			List<Operatory> listOperatories=new List<Operatory>();
			listOperatories.Add(new Operatory(){OperatoryNum=1 });
			for(int i=0;i<_listScenarios.Count;i++){
				List<ApptLayoutInfo> listApptLayoutInfo=ControlApptPanel.PlaceAppointments(
					_listScenarios[i].TableAppointments,listOperatories,_heightLine,false,10,7,1,_widthTotal,0,
					new DateTime(2001,1,1),new DateTime(2001,1,1));
				_listScenarios[i].ResultObtained=GetHash(listApptLayoutInfo);
				_listScenarios[i].BitmapResult=GetBitmap(listApptLayoutInfo);
			}
		}

		private ScenarioApptUnitTest Test1(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="1: 0/2 60min by 1/2 40min at same time";
			scenario.Details="0/2 60min by 1/2 40min at same time";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromHours(1),TimeSpan.FromHours(1));
			scenario.AddRow(TimeSpan.FromHours(1),TimeSpan.FromMinutes(40));
			scenario.ResultExpected="0,72,150,72;150,72,150,48";
			return scenario;
		}

		private ScenarioApptUnitTest Test2(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="2: 0/2 60min, 1/2 60min just below";
			scenario.Details="0/2 60min, 1/2 60min just below";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromHours(1),TimeSpan.FromHours(1));
			scenario.AddRow(TimeSpan.FromHours(2),TimeSpan.FromHours(1));
			scenario.ResultExpected="0,72,300,72;0,144,300,72";
			return scenario;
		}

		private ScenarioApptUnitTest Test3(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="3: Cascade of 5 wrap, 10 minutes apart";
			scenario.Details="Cascade of 5 wrap, 10 minutes apart";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(30),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(40),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(50),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(60),TimeSpan.FromMinutes(40));
			scenario.ResultExpected="0,24,75,48;75,36,75,48;150,48,75,48;225,60,75,48;0,72,75,48";
			return scenario;
		}

		private ScenarioApptUnitTest Test4(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="4: Cascade of 5 wrap, plus one overlap";
			scenario.Details="Cascade of 5 wrap, plus one overlap";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(30),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(40),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(50),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(60),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(80));
			scenario.ResultExpected="0,24,60,48;60,36,60,48;120,48,60,48;180,60,60,48;0,72,60,48;240,24,60,96";
			return scenario;
		}

		private ScenarioApptUnitTest Test5(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="5: Cascade of 5 wrap, double below";
			scenario.Details="Cascade of 5 wrap, double below";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(30),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(40),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(50),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(60),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(110),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(120),TimeSpan.FromMinutes(40));
			scenario.ResultExpected="0,24,75,48;75,36,75,48;150,48,75,48;225,60,75,48;0,72,75,48;0,132,150,48;150,144,150,48";
			return scenario;
		}

		private ScenarioApptUnitTest Test6(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="6: Cascade of 5 wrap, a single below, plus one overlap";
			scenario.Details="Cascade of 5 wrap, a single below, plus one overlap";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(30),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(40),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(50),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(60),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(110),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(140));
			scenario.ResultExpected="0,24,60,48;60,36,60,48;120,48,60,48;180,60,60,48;0,72,60,48;0,132,60,48;240,24,60,168";
			return scenario;
		}

		private ScenarioApptUnitTest Test7(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="7: Cascade of 5 wrap, double below, plus one overlap";
			scenario.Details="Cascade of 5 wrap, double below, plus one overlap";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(30),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(40),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(50),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(60),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(110),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(120),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(140));
			scenario.ResultExpected="0,24,60,48;60,36,60,48;120,48,60,48;180,60,60,48;0,72,60,48;0,132,60,48;60,144,60,48;240,24,60,168";
			return scenario;
		}

		private ScenarioApptUnitTest Test8(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="8: Cascade of 5 wrap, double below, partial overlap";
			scenario.Details="Cascade of 5 wrap, double below, partial overlap";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(30),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(40),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(50),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(60),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(110),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(120),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(90),TimeSpan.FromMinutes(100));
			scenario.ResultExpected="0,24,75,48;75,36,75,48;150,48,75,48;225,60,75,48;0,72,75,48;0,132,75,48;75,144,75,48;150,108,75,120";
			return scenario;
		}
		
		private ScenarioApptUnitTest Test9(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="9: Cascade of 5";
			scenario.Details="Cascade of 5";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(50));
			scenario.AddRow(TimeSpan.FromMinutes(30),TimeSpan.FromMinutes(50));
			scenario.AddRow(TimeSpan.FromMinutes(40),TimeSpan.FromMinutes(50));
			scenario.AddRow(TimeSpan.FromMinutes(50),TimeSpan.FromMinutes(50));
			scenario.AddRow(TimeSpan.FromMinutes(60),TimeSpan.FromMinutes(50));
			scenario.ResultExpected="0,24,60,60;60,36,60,60;120,48,60,60;180,60,60,60;240,72,60,60";
			return scenario;
		}

		private ScenarioApptUnitTest Test10(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="10: Cascade of 5 up wrap";
			scenario.Details="Cascade of 5 up wrap";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(90),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(80),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(60),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(50),TimeSpan.FromMinutes(40));
			scenario.ResultExpected="0,108,75,48;75,96,75,48;150,84,75,48;225,72,75,48;0,60,75,48";
			return scenario;
		}

		private ScenarioApptUnitTest Test11(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="11: One tall 0/2, 3 short 1/2";
			scenario.Details="One tall 0/2, 3 short 1/2";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(80));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(130),TimeSpan.FromMinutes(40));
			scenario.ResultExpected="0,84,150,96;150,84,150,36;150,120,150,36;150,156,150,48";
			return scenario;
		}

		private ScenarioApptUnitTest Test12(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="12: Starting with #11, add 1 lower 2/3";
			scenario.Details="Starting with #11, add 1 lower 2/3";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(80));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(130),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(140),TimeSpan.FromMinutes(30));
			scenario.ResultExpected="0,84,100,96;150,84,150,36;150,120,150,36;100,156,100,36;200,168,100,36";
			return scenario;
		}

		private ScenarioApptUnitTest Test13(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="13: Starting with #12, add 1 big 3/4";
			scenario.Details="Starting with #12, add 1 big 3/4";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(80));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(130),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(140),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(60),TimeSpan.FromMinutes(140));
			scenario.ResultExpected="0,84,75,96;75,84,75,36;75,120,75,36;75,156,75,36;150,168,75,36;225,72,75,168";
			return scenario;
		}

		private ScenarioApptUnitTest Test14(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="14: Starting with #12, add another high at 2/3";
			scenario.Details="Starting with #12, add another high at 2/3";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(80));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(130),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(140),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(20));
			scenario.ResultExpected="0,84,100,96;100,84,100,36;150,120,150,36;100,156,100,36;200,168,100,36;200,84,100,24";
			return scenario;
		}

		private ScenarioApptUnitTest Test15(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="15: Starting with #14, add another in the middle";
			scenario.Details="";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(80));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(130),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(140),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(20));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(20));
			scenario.ResultExpected="0,84,100,96;100,84,100,36;100,120,100,36;100,156,100,36;200,168,100,36;200,84,100,24;200,120,100,24";
			return scenario;
		}

		private ScenarioApptUnitTest Test16(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="16: Starting with #15, add another in the middle";
			scenario.Details="";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(80));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(130),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(140),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(20));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(20));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(20));
			scenario.ResultExpected="0,84,75,96;100,84,100,36;75,120,75,36;100,156,100,36;200,168,100,36;200,84,100,24;150,120,75,24;225,120,75,24";
			return scenario;
		}

		private ScenarioApptUnitTest Test17(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="17: Three short 0/2, one tall 1/2, add one low";
			scenario.Details="";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(130),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(100));
			scenario.AddRow(TimeSpan.FromMinutes(140),TimeSpan.FromMinutes(40));
			scenario.ResultExpected="0,84,100,36;0,120,100,36;0,156,100,48;100,84,100,120;200,168,100,48";
			return scenario;
		}
		private ScenarioApptUnitTest Test18(){
			ScenarioApptUnitTest scenario=new ScenarioApptUnitTest();
			scenario.DescriptionShort="18: Like 17, but with 5 appointments at left intead of 3";
			scenario.Details="";
			scenario.TableAppointments=GenerateTableAppts();
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(130),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(100),TimeSpan.FromMinutes(30));
			scenario.AddRow(TimeSpan.FromMinutes(130),TimeSpan.FromMinutes(40));
			scenario.AddRow(TimeSpan.FromMinutes(70),TimeSpan.FromMinutes(100));
			scenario.AddRow(TimeSpan.FromMinutes(140),TimeSpan.FromMinutes(40));
			scenario.ResultExpected="0,84,100,36;0,120,75,36;0,156,75,48;75,120,75,36;75,156,75,48;150,84,75,120;225,168,75,48";
			return scenario;
		}

		private DataTable GenerateTableAppts(){
			DataTable table=new DataTable();
			table.Columns.Add("AptDateTime");
			table.Columns.Add("Op");
			table.Columns.Add("Pattern");
			return table;
		}

		private string GetHash(List<ApptLayoutInfo> listApptLayoutInfo){
			//The hash will the the rectangle defs, all chained together.
			string str="";
			for(int i=0;i<listApptLayoutInfo.Count;i++){
				if(i>0){
					str+=";";
				}
				str+=listApptLayoutInfo[i].RectangleBounds.X.ToString()
					+","+listApptLayoutInfo[i].RectangleBounds.Y.ToString()
					+","+listApptLayoutInfo[i].RectangleBounds.Width.ToString()
					+","+listApptLayoutInfo[i].RectangleBounds.Height.ToString();
			}
			return str;
		}

		private Bitmap GetBitmap(List<ApptLayoutInfo> listApptLayoutInfo){
			Bitmap bitmap=new Bitmap(_widthTotal+1,_heightTotal+1);
			Graphics g=Graphics.FromImage(bitmap);
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.FillRectangle(Brushes.WhiteSmoke,0,0,_widthTotal+1,_heightTotal+1);
			for(int i=0;i<25;i++){
				g.DrawLine(Pens.LightGray,0,i*_heightLine,_widthTotal,i*_heightLine);//every 10 min
			}
			for(int i=0;i<5;i++){//every hour
				g.DrawLine(Pens.LightGray,0,i*_heightLine*6-1,_widthTotal,i*_heightLine*6-1);
				g.DrawLine(Pens.Black,0,i*_heightLine*6,_widthTotal,i*_heightLine*6);
			}
			for(int i=0;i<listApptLayoutInfo.Count;i++){
				g.FillRectangle(Brushes.White,
					listApptLayoutInfo[i].RectangleBounds.X,
					listApptLayoutInfo[i].RectangleBounds.Y,
					listApptLayoutInfo[i].RectangleBounds.Width,
					listApptLayoutInfo[i].RectangleBounds.Height);
				g.DrawRectangle(Pens.Black,
					listApptLayoutInfo[i].RectangleBounds.X,
					listApptLayoutInfo[i].RectangleBounds.Y,
					listApptLayoutInfo[i].RectangleBounds.Width,
					listApptLayoutInfo[i].RectangleBounds.Height);
				string strNum=i.ToString()+"-"+listApptLayoutInfo[i].OverlapPosition.ToString()+"/"+listApptLayoutInfo[i].OverlapSections.ToString();
				float widthString=g.MeasureString(strNum,Font).Width;
				g.DrawString(strNum,Font,Brushes.Black,
					listApptLayoutInfo[i].RectangleBounds.X+listApptLayoutInfo[i].RectangleBounds.Width/2f-widthString/2f,
					listApptLayoutInfo[i].RectangleBounds.Y+listApptLayoutInfo[i].RectangleBounds.Height/2f-6f);
			}
			return bitmap;
		}

		private void ButCopyToClipboard_Click(object sender, EventArgs e){
			Clipboard.SetText(textResult.Text);
		}

		private void GridMain_CellClick(object sender, ODGridClickEventArgs e){
			textDetails.Text=_listScenarios[e.Row].Details;
			pictureBox.BackgroundImage=_listScenarios[e.Row].BitmapResult;
			textResult.Text=_listScenarios[e.Row].ResultObtained;
		}

		private void ButTestRectSpeed_Click(object sender, EventArgs e){
			List<RectangleF> listRectangleFs=new List<RectangleF>();
			//the sizes won't affect the speed.  Speed is determined by simple math functions.
			//Lots of the rectangles are the same
			RectangleF rectangleF;
			for(int i=0;i<10000;i++){
				rectangleF=new RectangleF(10,10,100,100);
				listRectangleFs.Add(rectangleF);
			}
			for(int i=0;i<10000;i++){
				rectangleF=new RectangleF(30,30,100,100);
				listRectangleFs.Add(rectangleF);
			}
			DateTime dateTimeStart=DateTime.Now;
			int counter=0;//force the compiler to use the result, just so it doesn't get optimized out.
			for(int i=0;i<20000;i++){
				rectangleF=new RectangleF(20,20,100,100);
				bool intersects=Rectangle.Truncate(rectangleF).IntersectsWith(Rectangle.Truncate(listRectangleFs[i]));
				if(intersects){
					counter++;
				}
			}
			TimeSpan timeSpan=DateTime.Now-dateTimeStart;
			MessageBox.Show(timeSpan.ToString()+". Intersecting:"+counter.ToString());
		}
	}

	public class ScenarioApptUnitTest{
		public string DescriptionShort;
		public string Details;
		public DataTable TableAppointments;
		public string ResultExpected;
		public string ResultObtained;
		public Bitmap BitmapResult;

		public void AddRow(TimeSpan timeAppt, TimeSpan timeLength){
			DataRow dataRow=TableAppointments.NewRow();
			DateTime dateT=new DateTime(2001,1,1)+timeAppt;//date is always 1/1/2001
			dataRow["AptDateTime"]=POut.DateT(dateT,false);
			dataRow["Op"]=1;//always the same
			int increments=(int)timeLength.TotalMinutes/5;
			string pattern="";
			for(int i=0;i<increments;i++){
				pattern+="/";//all assistant time
			}
			dataRow["Pattern"]=pattern;
			TableAppointments.Rows.Add(dataRow);
		}

	
	}


}
