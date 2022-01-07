using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpenDental.SmartCards  {
	class WindowsSmartCardManager : ISmartCardManager {
		#region P/Invokes
		[DllImport("winscard.dll", EntryPoint = "SCardEstablishContext", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern SmartCardErrors EstablishContext(ScopeOptions scope, IntPtr reserved1, IntPtr reserved2, ref IntPtr context);

		[DllImport("winscard.dll", EntryPoint = "SCardReleaseContext", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern SmartCardErrors ReleaseContext(IntPtr context);

		[DllImport("winscard.dll", EntryPoint = "SCardListReaders", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern SmartCardErrors ListReaders(IntPtr context, string groups, string readers, ref int size);

		[DllImport("winscard.dll", EntryPoint = "SCardGetStatusChange", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern SmartCardErrors GetStatusChange(IntPtr handle, int timeout, [In, Out] ReaderState[] states, int count);
		#endregion

		#region Fields and properties
		// A pointer to the unmanaged context;
		private IntPtr context = IntPtr.Zero;
		// The name of all readers found on the computer;
		private Collection<string> readers;
		ReaderState[] states;
		// A thread that watches for new smart cards
		private BackgroundWorker worker;

		public ReadOnlyCollection<string> Readers {
			get { return new ReadOnlyCollection<string>(readers); }
		}
		#endregion

		#region Constructor
		public WindowsSmartCardManager() {
			// Establish the Windows Smart Card context
			SmartCardErrors result = EstablishContext(ScopeOptions.User, IntPtr.Zero, IntPtr.Zero, ref context);
			CheckResult(result);

			// List all readers
			int size = 2048;
			readers = new Collection<string>();
			// Ask for the size of the buffer first.
			if(ListReaders(context, null, null, ref size) == SmartCardErrors.SCARD_S_SUCCESS) {
				// Allocate a string of the proper size.
				string names = new string(' ', size);
				if(ListReaders(context, null, names, ref size) == SmartCardErrors.SCARD_S_SUCCESS) {
					// The 'names' string will contain a multi-string of the,
					// reader names i.e. they are separated by 0x00 characters.
					string name = string.Empty;
					for(int i = 0; i < names.Length; i++) {
						if(names[i] == 0x00) {
							if(name.Length > 0) {
								//
								// We have the reader name.
								//
								readers.Add(name);
								name = string.Empty;
							}
						}
						else {
							// Append found character.
							name = name + new string(names[i], 1);
						}
					}
				}
			}
			states = new ReaderState[readers.Count];
			for(int i = 0; i < readers.Count; i++)
				states[i].Reader = readers[i];

            if (readers.Count > 0) {
                worker = new BackgroundWorker();
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += new DoWorkEventHandler(WaitChangeStatus);
                worker.RunWorkerAsync();
            }
		}
		#endregion

		#region Events
		public event SmartCardStateChangedEventHandler SmartCardChanged;
		#endregion

        #region Methods
        public void Dispose() {
					try{
            worker.CancelAsync();

            // Obtaina lock when we use the context pointer, which may be used (is used every 1s!) by
            // WaitChangeStatus.
            lock (this) {
                if (context != IntPtr.Zero) {
                    SmartCardErrors result = ReleaseContext(context);
                    CheckResult(result);
                    context = IntPtr.Zero;
                }
            }
					}
					catch{
						//This is to catch an error message that many users were getting on shutdown of the program
					}
		}

        private void WaitChangeStatus(object sender, DoWorkEventArgs e) {
			while(!e.Cancel) {
                SmartCardErrors result;

                // Obtain a lock when we use the context pointer, which may be modified in the Dispose() method.
                lock (this) {
                    if (context == IntPtr.Zero)
                        return;

                    // This thread will be executed every 1000ms. The thread also blocks for 1000ms, meaning that the application
                    // may keep on running for one extra second after the user has closed the Main Form.
                    result = GetStatusChange(context, 1000, states, readers.Count);
                }

                if (result == SmartCardErrors.SCARD_E_TIMEOUT) {
                    // Time out has passed, but there is no new info. Just go on with the loop
                    continue;
                }

				CheckResult(result);
				for(int i = 0; i < states.Length; i++) {
					// Check if the state changed from the last time
					if((states[i].EventState & CardState.Changed) == CardState.Changed) {
						// Check what changed.
						SmartCardState state = SmartCardState.None;
						if((states[i].EventState & CardState.Present) == CardState.Present &&
							(states[i].CurrentState & CardState.Present) != CardState.Present) {
							// The card was inserted.
							state = SmartCardState.Inserted;
						}
						else if((states[i].EventState & CardState.Empty) == CardState.Empty &&
							(states[i].CurrentState & CardState.Empty) != CardState.Empty) {
							// The card was ejected.
							state = SmartCardState.Ejected;
						}
						if(state != SmartCardState.None && states[i].CurrentState != CardState.Unaware) {
							// Raise an event to indicate the change, if we weren't previously unaware of the state
							// This prevents from the event being raised the first time.
							this.OnStateChanged(
								new SmartCardStateChangedEventArgs(
								states[i].Reader, state, states[i].rgbAtr));
						}
						// Update the current state 
						// for the next time they are checked.
						states[i].CurrentState = states[i].EventState;
					}
				}
			}
		}

		protected void OnStateChanged(SmartCardStateChangedEventArgs e) {
			if(SmartCardChanged != null)
				SmartCardChanged(this, e);
		}


		private void CheckResult(SmartCardErrors result) {
			//if(result != SmartCardErrors.SCARD_S_SUCCESS);
				//throw new Win32Exception((int)result);
		}
		#endregion

		#region Destructor
		~WindowsSmartCardManager() {
			Dispose();
		}
		#endregion
	}
}
