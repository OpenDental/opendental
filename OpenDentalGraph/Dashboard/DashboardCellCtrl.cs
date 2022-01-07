using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDentalGraph {	
	public partial class DashboardCellCtrl:UserControl {
		#region Defined Events
		public event EventHandler DeleteColumnButtonMouseEnter;
		public event EventHandler DeleteColumnButtonMouseLeave;
		public event EventHandler DeleteRowButtonMouseEnter;
		public event EventHandler DeleteRowButtonMouseLeave;
		public event EventHandler DeleteColumnButtonClick;
		public event EventHandler DeleteRowButtonClick;
		public event EventHandler DeleteCellButtonClick;
		#endregion

		#region Private Data
		private DashboardDockContainer _dockedControlHolder;
		private bool _isHightlighted=false;
		private bool _isEditMode=false;
		private bool _hasUnsavedChanges=false;
		#endregion

		#region Properties
		private EventHandler _onEditClick {
			get {
				return _dockedControlHolder==null||_dockedControlHolder.OnEditClick==null ? null : _dockedControlHolder.OnEditClick;
			}
		}
		private EventHandler _onEditCancel {
			get {
				return _dockedControlHolder==null||_dockedControlHolder.OnEditCancel==null ? new EventHandler((s,e) => { }) : _dockedControlHolder.OnEditCancel;
			}
		}
		private EventHandler _onEditOk {
			get {
				return _dockedControlHolder==null||_dockedControlHolder.OnEditOk==null ? new EventHandler((s,e) => { }) : _dockedControlHolder.OnEditOk;
			}
		}
		private EventHandler _onDropComplete {
			get {
				return _dockedControlHolder==null||_dockedControlHolder.OnDropComplete==null ? null : _dockedControlHolder.OnDropComplete;
			}
		}
		private EventHandler _onRefreshCache {
			get {
				return _dockedControlHolder==null||_dockedControlHolder.OnRefreshCache==null ? new EventHandler((s,e) => { }) : _dockedControlHolder.OnRefreshCache;
			}
		}
		public Control DockedControl {
			get {
				if(_dockedControlHolder==null) {
					return null;
				}
				return _dockedControlHolder.Contr;
			}
		}
		public OpenDentBusiness.TableBase DockedControlTag {
			get {
				if(_dockedControlHolder==null) {
					return null;
				}
				return _dockedControlHolder.DbItem;
			}
		}
		private DashboardDockContainer _dockedControl {
			get {
				return _dockedControlHolder;
			}
			set {
				if(value==null) {
					_dockedControlHolder=null;
					butDrag.Enabled=false;
					butDeleteCell.Enabled=false;
					butEdit.Enabled=false;
					return;
				}
				if(value.Contr==this) {
					return;
				}
				if(_dockedControlHolder!=null) {
					MessageBox.Show("This cell already has contains a graph. You must move or delete this graph before dragging a new graph onto the cell.");
					return;
				}
				_dockedControlHolder=value;
				DockedControl.Dock=DockStyle.Fill;
				DockedControl.AllowDrop=true;
				this.Controls.Remove(pictureBox);
				this.Controls.Add(DockedControl);
				DockedControl.SendToBack();				
				butDrag.Enabled=true;
				butDeleteCell.Enabled=true;
				butEdit.Enabled=_onEditClick!=null;
				if(_onDropComplete!=null) {
					_onDropComplete(this,new EventArgs());					
				}
			}
		}
		public bool IsHighlighted {
			get {
				return _isHightlighted;
			}
			set {
				if(IsHighlighted==value) {
					return;
				}
				_isHightlighted=value;
				this.Refresh();
			}
		}
		public bool HasDockedControl {
			get {
				return DockedControl!=null;
			}
		}
		public bool IsEditMode {
			get { return _isEditMode; }
			set { _isEditMode=value; }
		}
		///<summary>Setter only works to set to false. Setting to true does nothing. This is handled internally by the control itself.
		///Getter returns true if new 1) dockable control is dropped or 2) Ok is clicked after editing a cell.</summary>
		public bool HasUnsavedChanges {
			get { return _hasUnsavedChanges; }
			set {
				if(value) { //Only allow turn off (not on).
					return;
				}
				_hasUnsavedChanges=false;
			}
		}
		#endregion

		public DashboardCellCtrl() : this(null) {
		}

		public DashboardCellCtrl(DashboardDockContainer controlHolder) {
			InitializeComponent();
			_dockedControl=controlHolder;
		}

		#region Public Methods
		public Control RemoveDockedControl() {
			if(DockedControl==null) {
				return null;
			}
			Control ret=DockedControl;
			this.Controls.Remove(DockedControl);
			this.Controls.Add(pictureBox);
			pictureBox.SendToBack();
			return ret;
		}
		#endregion

		#region Drag/Drop
		private void butDrag_MouseDown(object sender,MouseEventArgs e) {
			if(DockedControl==null) {
				return;
			}
			DockedControl.DoDragDrop(_dockedControlHolder,DragDropEffects.All);
		}

		private void DashboardCell_DragDrop(object sender,DragEventArgs e) {			
			this.BackColor=SystemColors.Control;
			_dockedControl=GetDroppedControl(e);
			_hasUnsavedChanges=true;
		}

		private void DashboardCell_DragEnter(object sender,DragEventArgs e) {
			if(!CanDrop(e)) {
				return;
			}
			e.Effect=DragDropEffects.All;
			this.BackColor=Color.Red;
		}

		private void DashboardCell_DragLeave(object sender,EventArgs e) {
			this.BackColor=SystemColors.Control;
		}

		public bool CanDrop(DragEventArgs e) {
			return IsEditMode && GetDroppedControl(e)!=null;
		}

		public DashboardDockContainer GetDroppedControl(DragEventArgs e) {
			if(DockedControl!=null) {
				return null;
			}
			if(!e.Data.GetDataPresent(typeof(DashboardDockContainer))) {
				return null;
			}
			DashboardDockContainer holder=(DashboardDockContainer)e.Data.GetData(typeof(DashboardDockContainer));
			if(holder==null||holder.Contr==DockedControl) {
				return null;
			}
			return holder;
		}
		#endregion

		#region Handle Events
		private void butRefresh_Click(object sender,EventArgs e) {
			_onRefreshCache(this,e);
		}

		private void butPrintPreview_Click(object sender,EventArgs e) {
			if(_dockedControlHolder!=null &&_dockedControlHolder.Printer!=null) {
				_dockedControlHolder.Printer.PrintPreview();
			}
		}
		
		private void butEdit_Click(object sender,EventArgs e) {
			if(DockedControl==null) {
				return;
			}
			DashboardDockContainer holder=_dockedControl;
			if(_onEditClick!=null) {
				_onEditClick(holder.Contr,new EventArgs());
			}
			EventHandler onEditOk=_onEditOk;
			EventHandler onEditCancel=_onEditCancel;
			using FormDashboardEditCell f=new FormDashboardEditCell(holder.Contr,IsEditMode);
			if(f.ShowDialog()==DialogResult.OK) {
				_hasUnsavedChanges=true;
				onEditOk(holder.Contr,new EventArgs());
			}
			else {
				onEditCancel(holder.Contr,new EventArgs());
			}
			_dockedControl=holder;
		}

		private void butDeleteCell_Click(object sender,EventArgs e) {
			if(DeleteCellButtonClick!=null) {
				DeleteCellButtonClick(this,new EventArgs());
			}
		}

		private void butDeleteColumn_Click(object sender,EventArgs e) {
			if(DeleteColumnButtonClick!=null) {
				DeleteColumnButtonClick(this,new EventArgs());
			}
		}

		private void butDeleteRow_Click(object sender,EventArgs e) {
			if(DeleteRowButtonClick!=null) {
				DeleteRowButtonClick(this,new EventArgs());
			}
		}

		private void butDeleteColumn_MouseEnter(object sender,EventArgs e) {
			if(DeleteColumnButtonMouseEnter!=null) {
				DeleteColumnButtonMouseEnter(this,new EventArgs());
			}
			DashboardCell_MouseEnterLeave(sender,e);
		}

		private void butDeleteColumn_MouseLeave(object sender,EventArgs e) {
			if(DeleteColumnButtonMouseLeave!=null) {
				DeleteColumnButtonMouseLeave(this,new EventArgs());
			}
			DashboardCell_MouseEnterLeave(sender,e);
		}

		private void butDeleteRow_MouseEnter(object sender,EventArgs e) {
			if(DeleteRowButtonMouseEnter!=null) {
				DeleteRowButtonMouseEnter(this,new EventArgs());
			}
			DashboardCell_MouseEnterLeave(sender,e);
		}

		private void butDeleteRow_MouseLeave(object sender,EventArgs e) {
			if(DeleteRowButtonMouseLeave!=null) {
				DeleteRowButtonMouseLeave(this,new EventArgs());
			}
			DashboardCell_MouseEnterLeave(sender,e);
		}

		private void butDeleteCell_MouseLeave(object sender,EventArgs e) {
			IsHighlighted=false;
			DashboardCell_MouseEnterLeave(sender,e);
		}

		private void butDeleteCell_MouseEnter(object sender,EventArgs e) {
			IsHighlighted=true;
			DashboardCell_MouseEnterLeave(sender,e);
		}
	
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved(e);
			if(e.Control!=DockedControl) {
				return;
			}
			this.Controls.Add(pictureBox);
			pictureBox.SendToBack();
			_dockedControl=null;			
		}

		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			List<Control> controls=new List<Control>() { e.Control };
			for(int i = 0;i<controls.Count;i++) {
				controls[i].MouseEnter+=DashboardCell_MouseEnterLeave;
				controls[i].MouseLeave+=DashboardCell_MouseEnterLeave;
				controls[i].MouseMove+=DashboardCell_MouseEnterLeave;
				controls[i].MouseDown+=DashboardCell_MouseDown;
				controls.AddRange(controls[i].Controls.Cast<Control>());
			}
		}

		private void DashboardCell_MouseDown(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right && IsEditMode) {
				if(_dockedControlHolder==null) {
					deleteCellContentsToolStripMenuItem.Enabled=false;
					refreshToolStripMenuItem.Enabled=false;
					printToolStripMenuItem.Enabled=false;
					editToolStripMenuItem.Enabled=false;
				}
				contextMenuStripRight.Show(System.Windows.Forms.Cursor.Position);
			}
		}

		private void DashboardCell_MouseEnterLeave(object sender,EventArgs e) {
			Rectangle rcThisToScreen=new Rectangle(new Point(0,0),this.Size);
			Point ptCursorScreen=this.PointToClient(System.Windows.Forms.Cursor.Position);
			if(rcThisToScreen.Contains(ptCursorScreen)) {
				panelEditCell.Visible=IsEditMode;
				panelPrint.Visible=true;
			}
			else {
				panelEditCell.Visible=panelPrint.Visible=false;
			}
		}

		private void DashboardCell_Paint(object sender,PaintEventArgs e) {
			if(!IsHighlighted) {
				return;
			}
			int thickness = 4;
			int halfThickness = thickness/2;
			using(Pen p = new Pen(Color.Red,thickness)) {
				e.Graphics.DrawRectangle(p,new Rectangle(halfThickness,halfThickness,this.ClientSize.Width-thickness,this.ClientSize.Height-thickness));
			}
		}
		
		private void timer_Tick(object sender,EventArgs e) {
			//panelButtons doesn't always get hidden when it should so hide it when necessary.
			if(!panelEditCell.Visible && !panelPrint.Visible) {
				return;
			}
			DashboardCell_MouseEnterLeave(sender,e);
		}
		#endregion
	}
}
