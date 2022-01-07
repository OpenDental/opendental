using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormEtrans834Import:FormODBase {

		private ODThread _odThread=null;
		private delegate void Update834Delegate();
		private string[] _arrayImportFilePaths;
		private int _colDateIndex;
		private int _colPatCountIndex;
		private int _colPlanCountIndex;
		private int _colErrorIndex;
		private X12object _x834selected=null;
		public FormEtrans834Preview FormE834P=null;

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
			if(folderBrowserImportPath.ShowDialog()==DialogResult.OK) {
				textImportPath.Text=folderBrowserImportPath.SelectedPath;
				FillGridInsPlanFiles();
			}
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
			if(gridInsPlanFiles.ListGridColumns.Count==0) {
				gridInsPlanFiles.ListGridColumns.Add(new UI.GridColumn("FileName",300,UI.GridSortingStrategy.StringCompare));
				_colDateIndex=gridInsPlanFiles.ListGridColumns.Count;
				gridInsPlanFiles.ListGridColumns.Add(new UI.GridColumn("Date",80,UI.GridSortingStrategy.StringCompare));
				_colPatCountIndex=gridInsPlanFiles.ListGridColumns.Count;
				gridInsPlanFiles.ListGridColumns.Add(new UI.GridColumn("PatCount",80,UI.GridSortingStrategy.AmountParse));
				_colPlanCountIndex=gridInsPlanFiles.ListGridColumns.Count;
				gridInsPlanFiles.ListGridColumns.Add(new UI.GridColumn("PlanCount",80,UI.GridSortingStrategy.AmountParse));
				_colErrorIndex=gridInsPlanFiles.ListGridColumns.Count;
				gridInsPlanFiles.ListGridColumns.Add(new UI.GridColumn("Errors",80,UI.GridSortingStrategy.StringCompare){ IsWidthDynamic=true });
			}			
			gridInsPlanFiles.ListGridRows.Clear();
			gridInsPlanFiles.EndUpdate();
			if(!Directory.Exists(textImportPath.Text)) {
				return;
			}
			gridInsPlanFiles.BeginUpdate();
			_arrayImportFilePaths=Directory.GetFiles(textImportPath.Text);
			for(int i=0;i<_arrayImportFilePaths.Length;i++) {
				UI.GridRow row=new UI.GridRow();
				gridInsPlanFiles.ListGridRows.Add(row);
				string filePath=_arrayImportFilePaths[i];
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
			if(_odThread!=null) {
				_odThread.QuitSync(0);
			}
			_odThread=new ODThread(WorkerParse834);
			_odThread.Start();
		}

		private void WorkerParse834(ODThread odThread) {
			Load834_Safe();
			odThread.QuitAsync();
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
			_x834selected=null;
			for(int i=0;i<gridInsPlanFiles.ListGridRows.Count;i++) {
				UI.GridRow row=gridInsPlanFiles.ListGridRows[i];
				if(i < previewLimitCount) {
					gridInsPlanFiles.BeginUpdate();
				}
				string filePath=(string)row.Tag;
				ShowStatus(Lan.g(this,"Parsing file")+" "+Path.GetFileName(filePath));
				string messageText=File.ReadAllText(filePath);
				X12object xobj=X12object.ToX12object(messageText);
				if(xobj==null) {
					row.Cells[_colErrorIndex].Text="Is not in X12 format.";
					continue;
				}
				try {
					if(!X834.Is834(xobj)) {
						row.Cells[_colErrorIndex].Text="Is in X12 format, but is not an 834 document.";
						continue;
					}
					xobj.FilePath=filePath;
					row.Cells[_colDateIndex].Text=xobj.DateInterchange.ToString();
					int memberCount=xobj.GetSegmentCountById("INS");
					int planCount=xobj.GetSegmentCountById("HD");
					row.Cells[_colPatCountIndex].Text=memberCount.ToString();
					row.Cells[_colPlanCountIndex].Text=planCount.ToString();
					row.Cells[_colErrorIndex].Text="";
					if(_x834selected==null || _x834selected.DateInterchange > xobj.DateInterchange) {
						selectedIndex=i;
						_x834selected=xobj;
					}
				}
				catch(ApplicationException aex) {
					row.Cells[_colErrorIndex].Text=aex.Message;
				}
				catch(Exception ex) {
					row.Cells[_colErrorIndex].Text=ex.ToString();
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
			if(_odThread!=null && !_odThread.HasQuit) {
				return;//Force the user to wait until the window has finished loading/refreshing, because we need to know which file to import.
			}
			if(!Directory.Exists(textImportPath.Text)) {
				MsgBox.Show(this,"Invalid import path.");
				return;
			}
			Prefs.UpdateString(PrefName.Ins834ImportPath,textImportPath.Text);
			if(_x834selected==null) {
				MsgBox.Show(this,"No files to import.");
				return;
			}
			FormE834P=new FormEtrans834Preview(new X834(_x834selected));
			FormE834P.Show();
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			if(_odThread!=null) {
				_odThread.QuitAsync();
			}
			DialogResult=DialogResult.Cancel;
			Close();
		}

	}
}