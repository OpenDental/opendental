/*using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace OpenDental.Eclaims{
	/// <summary>
	/// Summary description for Rs232.
	/// </summary>
	public class Rs232{
		private IntPtr mhRS = new IntPtr(0);	  // Handle to Com Port
		private int miPort=1;//  Default is COM1	
		private int miTimeout=70;   // Timeout in ms
		private int miBaudRate=9600;
		private DataParity meParity=0;
		private DataStopBit meStopBit=0;
		private int miDataBit=8;
		private bool mbEnableEvents;
		private Thread moEvents;
		private OVERLAPPED muOvlE;
		private byte[] mabtRxBuf;//  Receive buffer
		private int miBufThreshold = 1;

		public Rs232()
		{
			
		}

		public int Port{
			get{return miPort;}
			set{miPort=value;}
		}

		public int BaudRate{
			get{return miBaudRate;}
			set{miBaudRate=value;}
		}

		public int Timeout{
			get{return miTimeout;}
			set{
				miTimeout = value==0 ? 500 : value;
				// If Port is open updates it on the fly
				pSetTimeout();
			}
		}

		public int DataBit{
			get{return miDataBit;}
			set{miDataBit=value;}
		}

		public DataStopBit StopBit{
			get{return meStopBit;}
			set{meStopBit=value;}
		}

		public int Parity{
			get{return meParity;}
			set{meParity=value;}
		}

		///<summary>Enables monitoring of incoming events.</summary>
		public void EnableEvents(){
			if(mhRS.ToInt32() <= 0){
				throw new ApplicationException("Please initialize and open port before using this method");
			}
			else{
				if(moEvents==null){
					mbEnableEvents = true;
					moEvents = new Thread(new ThreadStart(pEventsWatcher));
					moEvents.IsBackground = true;
					moEvents.Start();
				}
			}
		}


		#region Enums
		public enum DataParity{
			Parity_None=0,
      Parity_Odd,
			Parity_Even,
			Parity_Mark
		}
	
		public enum DataStopBit{
			StopBit_1=1,
			StopBit_2
		}

		[Flags]
		public enum PurgeBuffers{
			RXAbort = &H2,
			RXClear = &H8,
			TxAbort = &H1,
			TxClear = &H4
		}

		// Comm Masks
		[Flags]
		public enum EventMasks{
			RxChar = &H1,
			RXFlag = &H2,
			TxBufferEmpty = &H4,
			ClearToSend = &H8,
			DataSetReady = &H10,
			CarrierDetect = &H20,
			Break = &H40,
			StatusError = &H80,
			Ring = &H100
		}
		#endregion

		#region structs
		/// <summary>The COMMTIMEOUTS structure is used in the SetCommTimeouts and GetCommTimeouts functions to set and query the time-out parameters for a communications device. The parameters determine the behavior of ReadFile, WriteFile, ReadFileEx, and WriteFileEx operations on the device.</summary>
		[StructLayout( LayoutKind.Sequential )]
		private struct COMMTIMEOUTS{
			public int ReadIntervalTimeout;
			public int ReadTotalTimeoutMultiplier;
			public int ReadTotalTimeoutConstant;
			public int WriteTotalTimeoutMultiplier;
			public int WriteTotalTimeoutConstant;
		}

		[StructLayout(LayoutKind.Sequential, Pack=1)]
		private struct COMSTAT{
      int fBitFields;
      int cbInQue;
      int cbOutQue;
    }

		[StructLayout(LayoutKind.Sequential, Pack=1)] 
		private struct DCB{
			public int DCBlength;
			public int BaudRate;
			public int Bits1;
			public Int16 wReserved;
			public Int16 XonLim;
			public Int16 XoffLim;
			public Byte ByteSize;
			public Byte Parity;
			public Byte StopBits;
			public char XonChar;
			public char XoffChar;
			public char ErrorChar;
			public char EofChar;
			public char EvtChar;
			public Int16 wReserved2;
		}

		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct OVERLAPPED{
      public int Internal;
      public int InternalHigh;
      public int Offset;
      public int OffsetHigh;
      public IntPtr hEvent;
    }

		
		#endregion

		#region constants
			private const int WAIT_OBJECT_0 = 0;
			private const int INFINITE = &HFFFFFFFF;
		#endregion

		#region Win32Api

		[DllImport("kernel32.dll", SetLastError=true)]
		private static extern int SetCommTimeouts(IntPtr hFile, [In] ref COMMTIMEOUTS lpCommTimeouts);

		/// <summary>Creates or opens any of the following objects and returns a handle that can be used to access the object: Consoles, Communications resources, Directories (open only), Disk devices, Files, Mailslots, Pipes</summary>
		[DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
		private static extern IntPtr CreateFile(
			String lpFileName, 
			UInt32 dwDesiredAccess, 
			UInt32 dwShareMode,
			IntPtr lpSecurityAttributes, 
			UInt32 dwCreationDisposition, 
			UInt32 dwFlagsAndAttributes,
			IntPtr hTemplateFile
			);

		/// <summary>Retrieves information about a communications error and reports the current status of a communications device. The function is called when a communications error occurs, and it clears the device's error flag to enable additional input and output (I/O) operations.</summary>
		[DllImport("kernel32.dll")]
		private static extern int ClearCommError(
			IntPtr hFile, 
			out UInt32 lpErrors, 
			out COMMSTAT cs
			);

		/// <summary>Discards all characters from the output or input buffer of a specified communications resource. It can also terminate any pending read or write operations on the resource.</summary>
		[DllImport("kernel32.dll")]
		private static extern int PurgeComm(IntPtr hFile,int dwFlags);

		[DllImport("kernel32.dll", SetlastError=True)]
		private static extern int GetCommState(IntPtr hCommDev,DCB lpDCB);

		[DllImport("kernel32.dll")]
		private static extern int BuildCommDCB(string lpDef,ref DCB lpDCB);

		///<summary>Retrieves the current control settings for a specified communications device.</summary>
		[DllImport("kernel32.dll", SetlastError=True)]
		private static extern int SetCommState(IntPtr hCommDev,ref DCB lpDCB);

		/// <summary>Initializes the communications parameters for a specified communications device.</summary>
		[DllImport("kernel32.dll")]
		private static extern int SetupComm(IntPtr hFile,int dwInQueue, int dwOutQueue);
   
		[DllImport("kernel32.dll")]
		private static extern IntPtr CreateEvent(IntPtr lpEventAttributes,int bManualReset,
			int bInitialState,string lpName);
	
		[DllImport("kernel32.dll")] 
		private static extern int SetCommMask(IntPtr hFile,int lpEvtMask);

		[DllImport("kernel32.dll", SetlastError=True)]
		private static extern int WaitCommEvent(IntPtr hFile,EventMasks Mask,OVERLAPPED lpOverlap);

		[DllImport("kernel32.dll", SetlastError=True)]
		private static extern int WaitForSingleObject(IntPtr hHandle,int dwMilliseconds);

		///<summary>Reads data from a file, starting at the position indicated by the file pointer. After the read operation has been completed, the file pointer is adjusted by the number of bytes actually read, unless the file handle is created with the overlapped attribute. If the file handle is created for overlapped input and output (I/O), the application must adjust the position of the file pointer after the read operation. This function is designed for both synchronous and asynchronous operation. The ReadFileEx function is designed solely for asynchronous operation. It lets an application perform other processing during a file read operation.</summary>
		[DllImport("kernel32.dll", SetLastError=true)]
		private static extern int ReadFile(IntPtr hFile,[Out] Byte[] lpBuffer,int nNumberOfBytesToRead,
			out int nNumberOfBytesRead,OVERLAPPED lpOverlapped);

		[DllImport("kernel32.dll", SetlastError=true)]
		private static extern bool CloseHandle(IntPtr hObject);

		#endregion

		public event CommEventHandler CommEvent;

		public delegate void CommEventHandler(Rs232 source,EventMasks Mask);

		public void Open(){
			// Get Dcb block,Update with current data
			DCB uDcb;
			int iRc;
			// Set working mode
			meMode = Mode.Overlapped;
			int iMode= meMode==Mode.Overlapped ? FILE_FLAG_OVERLAPPED : 0;//Convert.ToInt32(IIf(meMode = Mode.Overlapped, FILE_FLAG_OVERLAPPED, 0));
			// Initializes Com Port
			if(miPort > 0){
				try{
					// Creates a COM Port stream handle 
					mhRS=CreateFile(@"\\.\COM"+miPort.ToString(),GENERIC_READ | GENERIC_WRITE,
						0,0,OPEN_EXISTING,iMode,0);
					if(mhRS.ToInt32() > 0){
						int lpErrCode;
						iRc=ClearCommError(mhRS,lpErrCode,new COMSTAT());// Clear all comunication errors
						iRc=PurgeComm(mhRS, PurgeBuffers.RXClear | PurgeBuffers.TxClear);//Clears buffers
						iRc = GetCommState(mhRS, uDcb);// Gets COM Settings
						string sParity= "NOEM";// Updates COM Settings
						sParity = sParity.Substring(meParity, 1);
						//Set DCB State
						string sDCBState=String.Format("baud={0} parity={1} data={2} stop={3}",
							miBaudRate, sParity, miDataBit, CInt(meStopBit));
						iRc = BuildCommDCB(sDCBState, uDcb);
						uDcb.Parity = (byte)(meParity);
						//Set Xon/Xoff State
						if(mbUseXonXoff)
							uDcb.Bits1 = 768;
						else
							uDcb.Bits1 = 0;
						iRc = SetCommState(mhRS, uDcb);
						if(iRc==0){
							string sErrTxt=new System.Runtime.InteropServices.ExternalException().Message;
							throw new CIOChannelException("Unable to set COM state " + sErrTxt);
						}
						// Setup Buffers (Rx,Tx)
						iRc = SetupComm(mhRS, miBufferSize, miBufferSize);
						// Set Timeouts
						pSetTimeout();
						//Enables events if required
						if(mbEnableEvents)
							EnableEvents();
					}
					else{
						// Raise Initialization problems
						string sErrTxt= new System.ApplicationException().Message;
						throw new CIOChannelException("Unable to open COM" + miPort.ToString() 
							+"\\r\\n"+sErrTxt);
					}
				}
				catch(Exception Ex){
					// Generic error
					throw new CIOChannelException(Ex.Message, Ex);
				}
			}//if(miPort > 0){
			else{
				// Port not defined, cannot open
				throw new ApplicationException("COM Port not defined. Use Port property to set it before invoking InitPort");
			}
		}

		private void pSetTimeout(){
      COMMTIMEOUTS uCtm;
      // Set ComTimeout
			if(mhRS.ToInt32() <= 0)
				return;
			else{
				// Changes setup on the fly
				uCtm.ReadIntervalTimeout = 0;
				uCtm.ReadTotalTimeoutMultiplier = 0;
				uCtm.ReadTotalTimeoutConstant = miTimeout;
				uCtm.WriteTotalTimeoutMultiplier = 10;
				uCtm.WriteTotalTimeoutConstant = 100;
				SetCommTimeouts(mhRS, ref uCtm);
			}
		}

		///<summary>Watches for all events raising events when they arrive to the port.</summary>
		private void pEventsWatcher(){
      // Events to watch
      EventMasks lMask= EventMasks.Break | EventMasks.CarrierDetect | EventMasks.ClearToSend 
				| EventMasks.DataSetReady | EventMasks.Ring | EventMasks.RxChar | EventMasks.RXFlag
				| EventMasks.StatusError;
      EventMasks lRetMask;
			int iBytesRead;
			int iTotBytes;
			int iErrMask;
			int iRc;
			ArrayList aBuf=new ArrayList();
      COMSTAT uComStat;
      //-----------------------------------
      // Creates Event
      muOvlE.hEvent = CreateEvent(null, 1, 0, null);
      if(muOvlE.hEvent.ToInt32()==0)
				throw new ApplicationException("Error creating event for overlapped reading");
      // Set mask
      SetCommMask(mhRS, lMask);
      // Looks for RxChar
      while(mbEnableEvents){
        WaitCommEvent(mhRS, lMask, muOvlE);
        switch(WaitForSingleObject(muOvlE.hEvent, INFINITE)){
					case WAIT_OBJECT_0:
						// Event (or abort) detected
		//fix: need to actually break out of while
            if(!mbEnableEvents) break;
            if((lMask & EventMasks.RxChar) > 0){
              // Read incoming data
              ClearCommError(mhRS, iErrMask, uComStat);
              if(iErrMask==0){
                Overlapped ovl=new Overlapped();
                mabtRxBuf=new byte[uComStat.cbInQue-1];
                if(ReadFile(mhRS, mabtRxBuf, uComStat.cbInQue, iBytesRead, ovl) > 0){
                  if(iBytesRead > 0){
                    // Some bytes read, fills temporary buffer
                    if(iTotBytes < miBufThreshold){
                      aBuf.AddRange(mabtRxBuf);
                      iTotBytes += iBytesRead;
										}
                    // Threshold reached?, raises event
                    if(iTotBytes >= miBufThreshold){
                      //Copies temp buffer into Rx buffer
                      mabtRxBuf=new byte[iTotBytes - 1];
                      aBuf.CopyTo(mabtRxBuf);
                      // Raises event
                      try{
												OnCommEventReceived(this, lMask);
											}
                      finally{
												iTotBytes = 0;
												aBuf.Clear();
											}
                    }
                  }//if(iBytesRead > 0){
								}//if(ReadFile
              }//if(iErrMask==0){
						}
            else{
              // Simply raises OnCommEventHandler event
							OnCommEventReceived(this, lMask);
            }
						break;
					default:
            string sErr= new ApplicationException().Message;
            throw new ApplicationException(sErr);
						break;
				}//switch
      }//while
      // Release Event Handle
      CloseHandle(muOvlE.hEvent);
      muOvlE.hEvent = IntPtr.Zero;
		}

		///<summary>Raises CommEvent</summary>
		protected void OnCommEventReceived(Rs232 source, EventMasks mask){
			//Dim del As CommEventHandler = Me.CommEventEvent
			CommEventHandler del=this.CommEvent;
			if(del != null){
				ISynchronizeInvoke SafeInvoker;
				try{
					SafeInvoker = (ISynchronizeInvoke)del.Target;
				}
				catch{

				}
				if(SafeInvoker != null){
					SafeInvoker.Invoke(del, new Object[] {source, mask});
				}
				else{
					del. Invoke((source, mask)
				}
			End If
		End Sub


	}

	#region Exceptions
	///<summary>Raised when NACK error found</summary>
	public class CIOChannelException : ApplicationException{
		public CIOChannelException(){

		}
		public CIOChannelException(string message) : base(message){

		}
		public CIOChannelException(string message,Exception inner) : base(message,inner){

		}
	}
}


*/




