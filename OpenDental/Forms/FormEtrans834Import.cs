using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormEtrans834Import:FormODBase {

		private ODThread _thread=null;
		private delegate void Update834Delegate();
		private string[] _stringArrayFilePaths;
		private int _idxDateCol;
		private int _idxPatCountCol;
		private int _idxPlanCountCol;
		private int _idxErrorCol;
		private X12object _x12object=null;
		public FormEtrans834Preview FormEtrans834PreviewCur=null;

		public FormEtrans834Import() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEtrans834Import_Load(object sender,EventArgs e) {
			textImportPath.Text=PrefC.GetString(PrefName.Ins834ImportPath);
			if(ODBuild.IsWeb()) {
				//Not implemented yet for OD Cloud
				textImportPath.Text="";
				textImportPath.Enabled=false;
			}
			FillGridInsPlanFiles();
		}

		private void butImportPathPick_Click(object sender,EventArgs e) {
			if(folderBrowserImportPath.ShowDialog()!=DialogResult.OK) {
				return;
			}
			textImportPath.Text=folderBrowserImportPath.SelectedPath;
			FillGridInsPlanFiles();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGridInsPlanFiles();
		}

		///<summary>Shows current status to user in the progress label.  Useful for when processing for a few seconds or more.</summary>
		private void ShowStatus(string message) {
			labelProgress.Text=message;
			Application.DoEvents();
		}

		private void FillGridInsPlanFiles() {
			gridInsPlanFiles.BeginUpdate();
			if(gridInsPlanFiles.Columns.Count==0) {
				gridInsPlanFiles.Columns.Add(new UI.GridColumn("FileName",300,UI.GridSortingStrategy.StringCompare));
				_idxDateCol=gridInsPlanFiles.Columns.Count;
				gridInsPlanFiles.Columns.Add(new UI.GridColumn("Date",80,UI.GridSortingStrategy.StringCompare));
				_idxPatCountCol=gridInsPlanFiles.Columns.Count;
				gridInsPlanFiles.Columns.Add(new UI.GridColumn("PatCount",80,UI.GridSortingStrategy.AmountParse));
				_idxPlanCountCol=gridInsPlanFiles.Columns.Count;
				gridInsPlanFiles.Columns.Add(new UI.GridColumn("PlanCount",80,UI.GridSortingStrategy.AmountParse));
				_idxErrorCol=gridInsPlanFiles.Columns.Count;
				UI.GridColumn col=new UI.GridColumn("Errors",80,UI.GridSortingStrategy.StringCompare);
				col.IsWidthDynamic=true;
				gridInsPlanFiles.Columns.Add(col);
			}			
			gridInsPlanFiles.ListGridRows.Clear();
			gridInsPlanFiles.EndUpdate();
			if(!Directory.Exists(textImportPath.Text)) {
				return;
			}
			gridInsPlanFiles.BeginUpdate();
			_stringArrayFilePaths=Directory.GetFiles(textImportPath.Text);
			for(int i=0;i<_stringArrayFilePaths.Length;i++) {
				UI.GridRow row=new UI.GridRow();
				gridInsPlanFiles.ListGridRows.Add(row);
				string filePath=_stringArrayFilePaths[i];
				row.Tag=filePath;				
				string fileName=Path.GetFileName(filePath);
				row.Cells.Add(fileName);//FileName
				row.Cells.Add("");//Date - This value will be filled in when WorkerParse834() runs below.
				row.Cells.Add("");//PatCount - This value will be filled in when WorkerParse834() runs below.
				row.Cells.Add("");//PlanCount - This value will be filled in when WorkerParse834() runs below.
				row.Cells.Add("Loading file...");//Errors - This value will be filled in when WorkerParse834() runs below.
			}
			gridInsPlanFiles.EndUpdate();
			Application.DoEvents();
			if(_thread!=null) {
				_thread.QuitSync(0);
			}
			_thread=new ODThread(WorkerParse834);
			_thread.Start();
		}

		private void WorkerParse834(ODThread thread) {
			Load834_Safe();
			thread.QuitAsync();
		}

		///<summary>Call this from external thread. Invokes to main thread to avoid cross-thread collision.</summary>
		private void Load834_Safe() {
			try {
				this.BeginInvoke(new Update834Delegate(Load834_Unsafe),new object[] { });
			}
			//most likely because form is no longer available to invoke to
			catch { }			
		}

		private void Load834_Unsafe() {
			Cursor=Cursors.WaitCursor;
			ShowStatus("Loading...");
			Application.DoEvents();
			const int previewLimitCount=40;
			int selectedIndex=-1;
			_x12object=null;
			for(int i=0;i<gridInsPlanFiles.ListGridRows.Count;i++) {
				UI.GridRow row=gridInsPlanFiles.ListGridRows[i];
				if(i < previewLimitCount) {
					gridInsPlanFiles.BeginUpdate();
				}
				string filePath=(string)row.Tag;
				ShowStatus(Lan.g(this,"Parsing file")+" "+Path.GetFileName(filePath));
				string messageText=File.ReadAllText(filePath);
				X12object x12object=X12object.ToX12object(messageText);
				if(x12object==null) {
					row.Cells[_idxErrorCol].Text="Is not in X12 format.";
					continue;
				}
				if(!X834.Is834(x12object)) {
					row.Cells[_idxErrorCol].Text="Is in X12 format, but is not an 834 document.";
					continue;
				}
				x12object.FilePath=filePath;
				row.Cells[_idxDateCol].Text=x12object.DateInterchange.ToString();
				int memberCount=x12object.GetSegmentCountById("INS");
				int planCount=x12object.GetSegmentCountById("HD");
				row.Cells[_idxPatCountCol].Text=memberCount.ToString();
				row.Cells[_idxPlanCountCol].Text=planCount.ToString();
				row.Cells[_idxErrorCol].Text="";
				if(_x12object==null || _x12object.DateInterchange > x12object.DateInterchange) {
					selectedIndex=i;
					_x12object=x12object;
				}
				if(i < previewLimitCount) {
					gridInsPlanFiles.EndUpdate();//Also invalidates grid.  Update required in case there was large error text.
					Application.DoEvents();
				}
			}
			//These 834 files are large and take a lot of memory when parsed into objects.
			//Run garbage collection to prevent OD from taking up too much memory at one time.
			GC.Collect();
			gridInsPlanFiles.BeginUpdate();
			if(selectedIndex >= 0) {
				gridInsPlanFiles.ListGridRows[selectedIndex].ColorBackG=Color.LightYellow;
			}
			gridInsPlanFiles.EndUpdate();//Also invalidates grid.  Update required in case there was large error text.
			ShowStatus("");
			Cursor=Cursors.Default;
			Application.DoEvents();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_thread!=null && !_thread.HasQuit) {
				return;//Force the user to wait until the window has finished loading/refreshing, because we need to know which file to import.
			}
			if(!Directory.Exists(textImportPath.Text)) {
				MsgBox.Show(this,"Invalid import path.");
				return;
			}
			Prefs.UpdateString(PrefName.Ins834ImportPath,textImportPath.Text);
			if(_x12object==null) {
				MsgBox.Show(this,"No files to import.");
				return;
			}
			FormEtrans834PreviewCur=new FormEtrans834Preview(new X834(_x12object));
			FormEtrans834PreviewCur.Show();
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			if(_thread!=null) {
				_thread.QuitAsync();
			}
			DialogResult=DialogResult.Cancel;
			Close();
		}

	}
}