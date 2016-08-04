using System;
using System.Threading;

namespace JukeWeb.Foundry.Utilities
{
	public abstract class ThreadWorker : IDisposable
	{
		static readonly Random random = new Random((int)DateTime.UtcNow.Ticks); // one seed for all threads
		Thread thread;

		enum EWaitEventType
		{
			Work = 0, // event to run the operation now
			ChangeSleepTime = 1, // event to change the sleep time (without running the operation)
			Exit = 2    // event to exit the maintenance thread
		}

		readonly WaitHandle[] events;    // events holder
		DateTime? lastExecution = null;
		TimeSpan timeBetweenExecutions = new TimeSpan(1, 0, 0, 0);  // once a day
		readonly string name = String.Empty;  // worker thread name
		TimeSpan? sleepTime = null;
		DateTime? nextExecutionTime = null;
		int errorCounter = 0;

		protected ThreadWorker(string name)
		{
			this.name = name;
			events = new WaitHandle[] { new AutoResetEvent(false), new AutoResetEvent(false), new AutoResetEvent(false) };
		}

		protected ThreadWorker(string name, TimeSpan timeBetweenExecutions)
			: this(name)
		{
			this.timeBetweenExecutions = timeBetweenExecutions;
		}

		public DateTime? LastExecute
		{
			get { return lastExecution; }
		}

		public TimeSpan TimeBetweenExecutions
		{
			get { return timeBetweenExecutions; }
			set { timeBetweenExecutions = value; }
		}

		public string Name
		{
			get { return name; }
		}

		public DateTime? NextExecutionTime
		{
			get { return nextExecutionTime; }
		}

		public int ErrorCounter
		{
			get { return errorCounter; }
		}

		public void StartWorkerThread()
		{
			if (thread == null)
				thread = new Thread(StartThread);
			thread.Start();
		}

		public void StopExecution()
		{
			((EventWaitHandle)events[(int)EWaitEventType.Exit]).Set();
		}

		public void RunOperationNow()
		{
			((EventWaitHandle)events[(int)EWaitEventType.Work]).Set();
		}

		public void RunOperationIn(TimeSpan timeToSleep)
		{
			sleepTime = timeToSleep;
			((EventWaitHandle)events[(int)EWaitEventType.ChangeSleepTime]).Set();
		}

		public void RunOperationIn(DateTime dateTimeInUTC)
		{
			DateTime now = DateTime.UtcNow;
			if (dateTimeInUTC > now)
				sleepTime = dateTimeInUTC - now;
			else
				sleepTime = TimeSpan.Zero;

			((EventWaitHandle)events[(int)EWaitEventType.ChangeSleepTime]).Set();
		}

		private void StartThread() 
		{
			sleepTime = GetFirstSleepTime();
			nextExecutionTime = DateTime.UtcNow + sleepTime;
			int waitIndex;

			while ((waitIndex = WaitHandle.WaitAny(events, (TimeSpan)sleepTime, false)) != (int)EWaitEventType.Exit)
			{
				lock (this)
				{
					if (waitIndex == WaitHandle.WaitTimeout ||
						(EWaitEventType)waitIndex == EWaitEventType.Work)
					{
						try
						{
							//Log.DebugFormat("{0} start operation.", name);
							TimeSpan? nextSleepTime = SpecificOperation();
							//Log.DebugFormat("{0} finished operation.", name);

							lastExecution = DateTime.UtcNow;

							if (nextSleepTime != null)
								sleepTime = nextSleepTime;
							else
								sleepTime = timeBetweenExecutions;
						}
						catch(Exception ex)
						{
							//Log.Error(String.Format("{0} thread has an error.", name), ex);
							errorCounter++;
							//if (errorCounter >= 3)
							///	break; // this will close the thread
                            //throw new Exception(ex.Message);
						//	sleepTime = new TimeSpan(0, 0, 1, 0); // try again in one minute
						}
					}
					nextExecutionTime = DateTime.UtcNow + sleepTime;
				}
			}
		}

		// can override next sleep time by returning it (return null if not)
		protected abstract TimeSpan? SpecificOperation();

		virtual protected TimeSpan GetFirstSleepTime()
		{
			// first time - sleep a random time in order to avoid all threads running at the same time.
			return new TimeSpan(0, 0, random.Next(1, 10), random.Next(0, 60)); // between 1 to 10 minutes
		}

		#region IDisposable

		public void Dispose()
		{
			StopExecution();
			if (thread != null)
			{
				thread.Join(500);
				thread.Abort();
				thread = null;
			}
			Thread.Sleep(100);
		}

		#endregion
	}
}
