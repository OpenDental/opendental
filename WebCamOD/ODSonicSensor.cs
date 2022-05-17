using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CodeBase;

namespace ProximityOD {

	///<summary>Auto scans serial ports for presence of a Maxbotics Sonic Sensor. Robust, will recover from many errors. 
	///Simply wire up to events ProximityChanged and RangeChanged to use.</summary>
	public class ODSonicSensor : IDisposable {
		///<summary>When range reading in inches changes to be less than range proximal, this event will fire with a bool indicating proximity.</summary>
		public event Action<bool> ProximityChanged = null;
		///<summary>When range reading in inches changes, this event will fire with the new range as a parameter.</summary>
		public event Action<int> RangeChanged = null;
		private SerialPort _port;
		///<summary>Background thread used to take reading from the serial port and save to _range and _isProximal properties.</summary>
		private Thread _threadRead;

		///<summary>When true, signals the _threadRead to quit gracfully on next loop.</summary>
		public bool ThreadExit {get;set;}

		private int _range;
		public int Range {
			get {
				return _range;
			}
			private set {
				if(Range!=value) {
					_range=value;
					RangeChanged?.Invoke(value);
				}
			}
		}

		private bool _isProximal;
		public bool IsProximal {
			get { return _isProximal; }
			private set {
				if(IsProximal!=value) { 
					_isProximal=value;
					ProximityChanged?.Invoke(value);
				}
			}
		}

		public ODSonicSensor() {
			_threadRead = new Thread(new ThreadStart(PerpetualRead));
			_threadRead.Name="SonicSensorThread";
			_threadRead.IsBackground=true;//IMPORTANT!! Without this, the background thread can prevent the process from exiting fully.
			_threadRead.Start();
		}

		private void PerpetualRead() {
			int errorCount = 0;
			while(!ThreadExit) {
				try {
					while(_port==null && !ThreadExit) {
						AutoAcquirePort();
						Thread.Sleep(100);
					}
					while(!_port.IsOpen && !ThreadExit) {
						_port.Open();
						Thread.Sleep(100);
					}
					byte[] buffer = new byte[512];
					_port.BaseStream.Read(buffer,0,buffer.Length);
					ProcessSensorData(buffer);
					_port.BaseStream.Flush();
					Thread.Sleep(250);//refill buffer
					errorCount=0;
				}
				catch(Exception ex) {
					ex.DoNothing();
					if(errorCount++>20) {
						try {
							_port.Dispose();
						}
						catch(Exception exc) {
							exc.DoNothing();
						}
						finally {
							errorCount=0;
							_port=null;
						}
					}
					Thread.Sleep(100);
				}
			}//end while
			Dispose();
		}

		private void ProcessSensorData(byte[] received) {
			string s = Encoding.UTF8.GetString(received);
			string[] readings = s.Split(new[] { "\r" },StringSplitOptions.RemoveEmptyEntries);
			string bestReading = readings.LastOrDefault(x => Regex.IsMatch(x,"R\\d{3} P\\d"));//Readings come in SUPER fast. Also, they might be partial. I.E. "6 P1\r" instead of "R016 P1\r"
			if(string.IsNullOrEmpty(bestReading)) {
				return;
			}
			int rangeNew = int.Parse(bestReading.Substring(1,3));
			Range=rangeNew;//Triggers Event
			IsProximal=bestReading[6]=='1';//Triggers Event; 2.5 second acquire time, 1.5 second release time.
		}

		private bool AutoAcquirePort() {
			List<string> portNames = SerialPort.GetPortNames().ToList();
			int maxTryCount = 20;
			int waitMs = 100;
			int tryCount = 0;
			while(_port==null && tryCount++<=maxTryCount) {
				using(SerialPort port = new SerialPort(portNames[tryCount%portNames.Count],57600,Parity.None,8,StopBits.One) { Handshake=Handshake.None,ReadTimeout=500 }) {
					if(tryCount>maxTryCount-portNames.Count) {
						//LAST TRY PER PORT.
						Thread.Sleep(100);
						waitMs=500;
					}
					port.Open();
					Thread.Sleep(waitMs);
					byte[] buffer = new byte[512];
					try {
						port.BaseStream.Read(buffer,0,buffer.Length);
						string s = Encoding.UTF8.GetString(buffer);
						string[] readings = s.Split(new[] { "\r" },StringSplitOptions.RemoveEmptyEntries);
						string bestReading = readings.LastOrDefault(x => Regex.IsMatch(x,"R\\d{3} P\\d"));//Readings come in SUPER fast. Also, they might be partial. I.E. "6 P1\r" instead of "R016 P1\r"
						if(string.IsNullOrEmpty(bestReading)) {
							continue;
						}
						//Cannot set _port=port inside this using block.
						_port=new SerialPort(portNames[tryCount%portNames.Count],57600,Parity.None,8,StopBits.One) { Handshake=Handshake.None,ReadTimeout=500 };
						return true;
					}
					catch(Exception) { }
					finally {
						port.Close();
						Thread.Sleep(100);
					}
				}//end using
			}//end while
			return false;
		}//end AutoAcquirePort

		public void Dispose() {
			try { _port.Close(); } catch(Exception) { }
			try { _port.Dispose(); } catch(Exception) { }
			_port=null;
			try { _threadRead.Abort(); } catch(Exception) { }
			_threadRead=null;
		}

	}
}
