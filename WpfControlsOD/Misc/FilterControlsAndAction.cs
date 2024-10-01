using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WpfControls.UI;

namespace OpenDental {
/*
Background:
In FormODBase, there's a section for FilterControlsAndAction.  Because it's in FormODBase, it's available to all derived forms.  The scenario where it's useful is when you have a group of controls at the top that you're using to filter results that are usually in a grid.  For example, dateStart, isHidden, name, etc.  You don't want the grid to refresh with each keystroke in the name field, and you also don't want to make the user click a Refresh button.  Instead, you can pass in all the controls that you want to "watch".  Then, each time they change, that triggers the refresh, but with less annoyance.  There's a time interval that it waits. For example, if it's set to 1 second, then it waits until typing has paused for 1 second before refreshing.  Since it runs the db query in a background thread, the UI will not lock up during the refresh so that they can keep typing.  The query should be able to safely abort if we need to terminate a previous refresh and quickly start again.

This new class:
We are moving to WPF and away from WinForms, so we need a new class that performs the same functionality.  There will also be some improvements from the old one:
-Separate class instead of embedded in FormODBase. This is a better design. Only available to the forms where you add this class.
-Any grid refresh that is in the action must be moved out.  No UI action in background threads.

How to use:
Add a private Field to your form: 
	FilterControlsAndAction _filterControlsAndAction;
Initialize:
			//This can be in ctr or load event, as long as it's after you've set any initial values for the filters
			_filterControlsAndAction=new FilterControlsAndAction();
			_filterControlsAndAction.AddControl(textName);
			_filterControlsAndAction.AddControl(checkIsHidden);
			_filterControlsAndAction.AddControl(dateStart);
			_filterControlsAndAction.FuncDb=RefreshFromDb;
			_filterControlsAndAction.SetInterval(700); //optional. Default is 1000
			_filterControlsAndAction.SetMinChars(4);//optional. If not set, then textboxes always wait the interval.
			_filterControlsAndAction.ActionComplete=FillGrid;
FuncDb:
		private object RefreshFromDb(){
			//This gets run on a background thread so that the UI will not lock up.
			//call db method
			//You will typically need some parameters, but they are not allowed.
			//So use class level fields or grab the values from UI objects.
			//No UI actions here.  Always forbidden in background threads.
			//You cannot change UI values, but you can safely get them like this:
			string txtProvNum="";
			Dispatcher.Invoke(()=>txtProvNum=textProvNum.Text);
			//If you think you need to set UI values in the action, there are other approaches you could use. For example, maybe a timer instead of this class.
			//Return table, List<>, etc. Whatever object you return is what will then get passed into ActionComplete.  Null is ok.
		}
ActionComplete:
		private void FillGrid(object objectData){
			//This gets run on the main UI thread.
			//You can use a wait cursor if you want, but usually not necessary.
			List<Provider> listProviders=(List<Provider>)objectData;//or whatever type you need
			gridMain.BeginUpdate();
			...Fill the grid like normal.
		}
There may also be situations where you will RefreshFromDb and FillGrid separately from the FilterControlsAndAction.
For example, when your form first loads.
So this is fine: FillGrid(RefreshFromDb()); 
but understand that RefreshFromDb will happen on the main thread in this case, which is totally fine.
*/
	public class FilterControlsAndAction {
		///<summary>This is the action that fires after the query is done. It is run on the main UI thread. Typically set it to FillGrid.  It's required, even if empty.</summary>
		public Action<object> ActionComplete;
		///<summary>This is the func where your db query will run in a separate thread. It should return an object encapsulating the new data such as a DataTable or a List.</summary>
		public Func<object> FuncDb;
		/// <summary>True by default. Set to false temporarily if you don't want to trigger FuncDb, usually if you're setting a text property in the load.</summary>
		public bool ShouldRefresh=true;
		///Default is 1. Tracks page number for grids with pages. This resets to 1 each time a control watched by FilterControlsAndAction is changed. Refreshing or filling the grid won't cause this to reset.
		public int PageNum=1;
		///<summary>This timer starts when user makes a change, and then it fires about a second later.</summary>
		private DispatcherTimer _dispatcherTimer;
		///<summary>To call Invoke, we need a control.  Any control will do. So we just stash any control here for that sole purpose.</summary>
		private FrameworkElement _frameworkElementForInvoke;
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private int _minChars=0;
		//These lists contain the controls that are able to have text in them. They are needed so we can compare their text to _minChars
		private List<TextBox> _listTextboxes=new List<TextBox>();
		private List<TextRich> _listTextRichs=new List<TextRich>();
		private List<TextVDate> _listTextVDates=new List<TextVDate>();
		private List<TextVDouble> _listTextVDouble=new List<TextVDouble>();
		private List<TextVInt> _listTextVInt=new List<TextVInt>();

		public FilterControlsAndAction(){
			_dispatcherTimer=new DispatcherTimer();
			_dispatcherTimer.Interval=TimeSpan.FromMilliseconds(1000);
			_dispatcherTimer.Tick+=_dispatcherTimer_Tick;
		}

		#region Methods - public
		public void AddControl(FrameworkElement frameworkElement){
			_frameworkElementForInvoke=frameworkElement;//doesn't matter which one, so just overwrite
			Type type=frameworkElement.GetType();
			string mynamespace=type.Namespace;
			if(mynamespace!="WpfControls.UI"){
				throw new Exception("Only controls in WpfControls.UI are allowed.");
			}
			if(frameworkElement is CheckBox checkBox){
				checkBox.Click+=Control_ChangedImmediate;
				return;
			}
			if(frameworkElement is ComboBox comboBox){
				comboBox.SelectionChangeCommitted+=Control_ChangedImmediate;
				return;
			}
			if(frameworkElement is ComboClinic comboClinic){
				comboClinic.SelectionChangeCommitted+=Control_ChangedImmediate;
				return;
			}
			if(frameworkElement is ListBox listBox){
				//if we make any changes here, we should also consider the same code in FormODBase
				listBox.MouseUp+=Control_ChangedImmediate;
				return;
			}
			if(frameworkElement is RadioButton radioButton){
				//this was not done in FormODBase
				radioButton.MouseUp+=Control_ChangedImmediate;
				return;
			}
			if(frameworkElement is TextBox textBox){
				textBox.TextChanged+=Control_Changed;
				_listTextboxes.Add(textBox);
				return;
			}
			if(frameworkElement is TextRich textRich){
				_listTextRichs.Add(textRich);
				textRich.TextChanged+=Control_Changed;
				return;
			}
			if(frameworkElement is TextVDate textVDate){
				_listTextVDates.Add(textVDate);
				textVDate.TextChanged+=Control_Changed;
				return;
			}
			if(frameworkElement is TextVDouble textVDouble){
				_listTextVDouble.Add(textVDouble);
				textVDouble.TextChanged+=Control_Changed;
				return;
			}
			if(frameworkElement is TextVInt textVInt){
				_listTextVInt.Add(textVInt);
				textVInt.TextChanged+=Control_Changed;
				return;
			}
			throw new Exception("That control type is not yet supported.");
		}

		/// <summary>Counts the total amount of text in each UI element to check against _minChars</summary>
		private int CountText(){
			int count=0;
			for(int i=0;i<_listTextboxes.Count;i++){
				count+=_listTextboxes[i].Text.Length;
			}
			for(int i=0;i<_listTextRichs.Count;i++){
				count+=_listTextRichs[i].Text.Length;
			}
			for(int i=0;i<_listTextVDates.Count;i++){
				count+=_listTextVDates[i].Text.Length;
			}
			for(int i=0;i<_listTextVDouble.Count;i++){
				count+=_listTextVDouble[i].Text.Length;
			}
			for(int i=0;i<_listTextVInt.Count;i++){
				count+=_listTextVInt[i].Text.Length;
			}
			return count;
		}

		///<summary>Default 1 second. The number of milliseconds to wait after the last user input on one of the specified filter controls to wait before calling ActionFilter.</summary>
		public void SetInterval(int intervalMs){
			_dispatcherTimer.Interval=TimeSpan.FromMilliseconds(intervalMs);
		}

		///<summary>Allowed 0 to 10. If this is not set (0), then textboxes always wait the interval. If this is set, then it's compared to the total sum of all characters entered into all textboxes. If the number of characters entered so far is >= minChars, then the textbox will refresh immediately instead of waiting the interval. This is used in FrmPatientSelect(...) along with the pref PatientSelectSearchMinChars.</summary>
		public void SetMinChars(int minChars){
			if(minChars>10){
				throw new Exception("MinChars cannot be greater than 10.");
			}
			if(minChars<0){
				throw new Exception("MinChars cannot be less than 0.");
			}
			_minChars=minChars;
		}
		#endregion Methods - public

		#region Methods - private
		///<summary></summary>
		private void LaunchThread(){
			if(FuncDb is null){
				return;
			}
			if(ActionComplete is null){
				return;
			}
			PageNum=1;
			_cancellationTokenSource.Cancel();//cancel previous thread
			_cancellationTokenSource=new CancellationTokenSource();
			Task.Run(()=>DoWork(_cancellationTokenSource.Token),_cancellationTokenSource.Token);
		}

		private void DoWork(CancellationToken cancellationToken){
			object objectData=null;
			try{
				objectData=FuncDb();
			}
			finally{
				if(!cancellationToken.IsCancellationRequested){
					_frameworkElementForInvoke.Dispatcher.Invoke(ActionComplete,objectData);
				}
			}
		}
		#endregion Methods - private

		#region Methods - private event handlers
		///<summary></summary>
		private void Control_Changed(object sender,EventArgs e) {
			if(!ShouldRefresh){
				return;
			}
			//Check the count of the textboxes to see if we should launch immediately or not
			if(CountText() >= _minChars){
				LaunchThread();//immediate
				return;
			}
			if(!_dispatcherTimer.IsEnabled){//not running
				_dispatcherTimer.Start();
				return;
			}
			//already running, but has not hit its limit yet
			//Restart it
			_dispatcherTimer.Stop();
			_dispatcherTimer.Start();
		}

		///<summary></summary>
		private void Control_ChangedImmediate(object sender,EventArgs e) {
			_dispatcherTimer.Stop();//previous timer does not need to finish since we are running immediately.
			LaunchThread();
		}

		private void _dispatcherTimer_Tick(object sender,EventArgs e) {
			_dispatcherTimer.Stop();//so it only runs once
			LaunchThread();
		}
		#endregion Methods - private event handlers
	}
}
