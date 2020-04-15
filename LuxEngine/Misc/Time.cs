using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public static class Time
    {
		/// <summary>
		/// Total amount of seconds the game has been running,
		/// Not for physics calculations.
		/// </summary>
		public static double TotalGameTime;

		/// <summary>
        /// Duration of a tick in seconds.
		/// Unscaled version of DeltaTime. Not affected by TimeScale
		/// </summary>
		public static readonly float Timestep = 1f / HardCodedConfig.TICKS_PER_SECOND;

		/// <summary>
		/// Delta time from the previous frame to the current, scaled by TimeScale.
        /// Measured in seconds.
		/// </summary>
		public static float DeltaTime
        {
            get
            {
				return Timestep * TimeScale;
            }
        }

		/// <summary>
		/// Secondary deltaTime for use when you need to scale two different deltas simultaneously
		/// Measured in seconds.
		/// </summary>
		public static double AltDeltaTime
        {
            get
            {
				return Timestep * AltTimeScale;
            }
        }

		/// <summary>
		/// Time scale of DeltaTime
		/// </summary>
		public static float TimeScale = 1f;

		/// <summary>
		/// Time scale of AltDeltaTime
		/// </summary>
		public static float AltTimeScale = 1f;

		/// <summary>
		/// Total number of ticks that have passed
		/// </summary>
		public static uint TicksCount = 0;

		/// <summary>
        /// Total seconds spent in simulation, including the current tick.
		/// </summary>
		public static float SimulationTotalTime
        {
            get
            {
				return TicksCount * Timestep;
            }
        }

        /// <summary>
        /// A number that represents a fraction of a tick.
        /// Used for interpolation.
        /// </summary>
		public static double Alpha
		{
            get
            {
				return Accumulator / Timestep;
            }
		}

        /// <summary>
        /// How many frames per seconds the game is drawing
        /// </summary>
		public static int FPS = 0;

		private static int _fpsCounter = 0;
		private static double _fpsSecondsCounter = 0;

		/// <summary>
		/// Accumulates time every update/frame, spends that time every tick
		/// </summary>
		internal static double Accumulator = 0;


        internal static void Update(double totalSeconds)
		{
			// Add time since last update to accumulator
			Accumulator += (float)(totalSeconds - TotalGameTime);

			// Update total time
			TotalGameTime = totalSeconds;
		}

		internal static void Tick()
        {
            // Consume a timestep from the time accumulator
			Accumulator -= Timestep;
			TicksCount++;
        }

        internal static void Draw(double deltaTime)
        {
			_fpsCounter++;
			_fpsSecondsCounter += deltaTime;

			if (_fpsSecondsCounter >= 1)
            {
				FPS = _fpsCounter;
				_fpsCounter = 0;
				_fpsSecondsCounter -= 1;
			}
		}
	}
}
