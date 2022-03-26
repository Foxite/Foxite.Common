using System;
using System.Threading;

namespace Foxite.Common {
	/// <summary>
	/// A lock object that guarantees that locks will be acquired in the same order they were requested.
	/// </summary>
	/// <remarks>
	/// https://stackoverflow.com/a/961904
	/// </remarks>
	public sealed class QueuedLock {
		private readonly object m_InnerLock;
		private volatile int m_TicketsCount = 0;
		private volatile int m_TicketToRide = 1;

		public QueuedLock() {
			m_InnerLock = new object();
		}

		private void Enter() {
			int myTicket = Interlocked.Increment(ref m_TicketsCount);
			Monitor.Enter(m_InnerLock);
			while (true) {
				if (myTicket == m_TicketToRide) {
					return;
				} else {
					Monitor.Wait(m_InnerLock);
				}
			}
		}

		private void Exit() {
			Interlocked.Increment(ref m_TicketToRide);
			Monitor.PulseAll(m_InnerLock);
			Monitor.Exit(m_InnerLock);
		}

		private sealed class DisposableLock : IDisposable {
			private QueuedLock m_Lock;
			private bool m_Entered;

			public DisposableLock(QueuedLock @lock) {
				m_Lock = @lock;
				m_Lock.Enter();
				m_Entered = true;
			}

			public void Dispose() {
				if (m_Entered) {
					try {
						m_Lock.Exit();
					} finally {
						m_Entered = false;
					}
				} else {
					throw new InvalidOperationException("Tried to exit a lock that wasn't entered");
				}
			}
		}
	
		/// <summary>
		/// Locks the QueuedLock and returns an object that will exit the lock when disposed.
		/// </summary>
		public IDisposable Lock() {
			return new DisposableLock(this);
		}		
	}
}
