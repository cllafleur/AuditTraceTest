using System;
using System.Diagnostics;
using Microsoft.Diagnostics.Tracing;
using System.Linq;

namespace ConsoleApplication1 {
	class Program {
		static void Main(string[] args) {
			//EventSource eventS = new EventSource();
			TraceSource trace = new TraceSource("Test");

			var listener = new ConsoleEventListener();
			//listener.EnableEvents(TestSource.Log, EventLevel.Verbose);
			foreach (var item in EventSource.GetSources()) {
				listener.EnableEvents(item, EventLevel.LogAlways);
			}
			//listener.EnableEvents();

			Trace.TraceError("super");
			Trace.TraceInformation("super");
			Trace.TraceWarning("super");
			Trace.WriteLine("super");
			//Trace.Fail("super");

			trace.TraceEvent(TraceEventType.Verbose, 1, "test traceSource {0}", "!?");
			TestSource.Log.LogSuper("cool");


			Console.ReadLine();

		}
	}

	[EventSource(Name = "Test")]
	sealed class TestSource : EventSource {
		public static readonly TestSource Log = new TestSource();


		[Event(1, Message = "super {0}", Level = EventLevel.Critical)]
		public void LogSuper(string msg) {

			if (this.IsEnabled())
				this.WriteEvent(1, msg);
		}
	}

	class ConsoleEventListener : EventListener {
		TraceSource ts = new TraceSource("Test");
		protected override void OnEventWritten(EventWrittenEventArgs eventData) {
			//Console.WriteLine(eventData.Message, eventData.Payload.ToArray());
			ts.TraceEvent(ConvertLevel(eventData.Level), eventData.EventId, eventData.Message, eventData.Payload.ToArray());

		}

		private TraceEventType ConvertLevel(EventLevel level) {
			switch (level) {
				case EventLevel.Critical:
					return TraceEventType.Critical;

				case EventLevel.Error:
					return TraceEventType.Error;

				case EventLevel.Informational:
					return TraceEventType.Information;

				case EventLevel.Warning:
					return TraceEventType.Warning;

				case EventLevel.Verbose:
				case EventLevel.LogAlways:
				default:
					return TraceEventType.Verbose;
			}
		}
	}
}
