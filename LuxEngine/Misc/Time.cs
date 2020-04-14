using System;
namespace LuxEngine
{
    public static class Time
    {
		/// <summary>
		/// Total time the game has been running
		/// </summary>
		public static float TotalTime;

		/// <summary>
		/// Delta time from the previous frame to the current, scaled by TimeScale
		/// </summary>
		public static float DeltaTime;

		/// <summary>
		/// Unscaled version of DeltaTime. Not affected by TimeScale
		/// </summary>
		public static float UnscaledDeltaTime;

		/// <summary>
		/// Secondary deltaTime for use when you need to scale two different deltas simultaneously
		/// </summary>
		public static float AltDeltaTime;

		/// <summary>
		/// time scale of deltaTime
		/// </summary>
		public static float TimeScale = 1f;

		/// <summary>
		/// time scale of altDeltaTime
		/// </summary>
		public static float AltTimeScale = 1f;

		/// <summary>
		/// total number of frames that have passed
		/// </summary>
		public static uint FrameCount;


        internal static void Update(float dt)
		{
			TotalTime += dt;
			DeltaTime = dt * TimeScale;
			AltDeltaTime = dt * AltTimeScale;
			UnscaledDeltaTime = dt;
			FrameCount++;
		}
	}
}
