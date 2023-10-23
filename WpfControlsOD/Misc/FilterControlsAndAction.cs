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
			_filterControlsAndAction.ActionComplete=FillGrid;
FuncDb:
		private object RefreshFromDb(){
			//This gets run on a background thread so that the UI will not lock up.
			//call db method
			//No UI actions here.  Always forbidden in background threads.
			//You can get UI values, but not change them.
			//If you think you need to set UI values in the action, there are other approaches you could use. For example, maybe a timer instead of this class.
			//Return table, List<>, etc. Whatever object you return is what will then get passed into ActionComplete.  Null is ok.
		}
ActionComplete:
		private void FillGrid(object objectData){
			//This gets run on the main UI thread.
			//You can use a wait cursor if you want, but usually not necessary.
			//Fill the grid like normal.
		}
*/
	public class FilterControlsAndAction {
		///<summary>This is the action that fires after the query is done. It is run on the main UI thread. Typically set it to FillGrid.  It's required, even if empty.</summary>
		public Action<object> ActionComplete;
		///<summary>This is the func where your db query will run in a separate thread. It should return an object encapsulating the new data such as a DataTable or a List.</summary>
		public Func<object> FuncDb;
		///<summary>This timer starts when user makes a change, and then it fires about a second later.</summary>
		private DispatcherTimer _dispatcherTimer;
		///<summary>To call Invoke, we need a control.  Any control will do. So we just stash any control here for that sole purpose.</summary>
		private FrameworkElement _frameworkElementForInvoke;
		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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
			if(frameworkElement is ListBox listBox){
				listBox.SelectionChangeCommitted+=Control_ChangedImmediate;
				return;
			}
			if(frameworkElement is TextBox textBox){
				textBox.TextChanged+=Control_Changed;
				return;
			}
			if(frameworkElement is TextRich textRich){
				textRich.TextChanged+=Control_Changed;
				return;
			}
			if(frameworkElement is TextVDate textVDate){
				textVDate.TextChanged+=Control_Changed;
				return;
			}
			if(frameworkElement is TextVDouble textVDouble){
				textVDouble.TextChanged+=Control_Changed;
				return;
			}
			if(frameworkElement is TextVInt textVInt){
				textVInt.TextChanged+=Control_Changed;
				return;
			}
			throw new Exception("That control type is not yet supported.");
		}

		///<summary>Default 1 second. The number of milliseconds to wait after the last user input on one of the specified filter controls to wait before calling ActionFilter.</summary>
		public void SetInterval(int intervalMs){
			_dispatcherTimer.Interval=TimeSpan.FromMilliseconds(intervalMs);
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
			cancellationTokenSource.Cancel();//cancel previous thread
			cancellationTokenSource=new CancellationTokenSource();
			Task.Run(()=>DoWork(cancellationTokenSource.Token),cancellationTokenSource.Token);
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
