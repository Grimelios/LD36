using System.Collections.Generic;
using LD36.Interfaces;

namespace LD36.Timing
{
	internal class TimerCollection : IDynamic
	{
		public TimerCollection()
		{
			Timers = new List<Timer>();
		}

		public List<Timer> Timers { get; }

		public void Update(float dt)
		{
			float elapsed = dt * 1000;

			for (int i = Timers.Count - 1; i >= 0; i--)
			{
				Timer timer = Timers[i];

				if (!timer.Paused)
				{
					timer.Elapsed += elapsed;

					if (timer.Elapsed >= timer.Duration)
					{
						timer.Trigger();

						if (timer.Repeating)
						{
							timer.Elapsed -= timer.Duration;
						}
						else
						{
							Timers.RemoveAt(i);
						}
					}

					timer.Progress = timer.Elapsed / timer.Duration;
				}
			}
		}
	}
}
