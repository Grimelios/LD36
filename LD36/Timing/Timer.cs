using System;

namespace LD36.Timing
{
	internal class Timer
	{
		private static TimerCollection timerCollection = DIKernel.Get<TimerCollection>();

		public Timer(float duration, Action trigger, bool repeating, bool triggerImmediately = false)
		{
			Duration = duration;
			Repeating = repeating;
			Trigger = trigger;

			if (triggerImmediately)
			{
				trigger();
			}

			timerCollection.Timers.Add(this);
		}

		public float Elapsed { get; set; }
		public float Duration { get; set; }
		public float Progress { get; set; }
		public bool Repeating { get; }
		public bool Paused { get; set; }

		public Action Trigger { get; }

		public void Destroy()
		{
			timerCollection.Timers.Remove(this);
		}
	}
}
