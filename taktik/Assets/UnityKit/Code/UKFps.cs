using UnityEngine;
using System.Collections;

public static class UKFps {

	private static int frames = 0;
	private static float framesDeltaTimeSum = 0f;
	private static int resetCountAfterFrames = 30;
	private static int frameCountOfLastCheck = -1;

	public static float MinFps = 0f;
	public static float MaxFps = 0f;
	public static float AvgFps = 0f;

	public static float CalculateFps() {
		// called a second time this frame?
		if (frameCountOfLastCheck == Time.frameCount) {
			return AvgFps;
		}

		if (frames >= resetCountAfterFrames) {
			frames = 0;
			framesDeltaTimeSum = 0f;
			MinFps = float.MaxValue;
			MaxFps = float.MinValue;
		}

		frameCountOfLastCheck = Time.frameCount;
		frames += 1;
		framesDeltaTimeSum += Time.deltaTime;

		AvgFps = framesDeltaTimeSum > 0f ? (frames / framesDeltaTimeSum) : 0f;
		MinFps = Mathf.Min(MinFps, AvgFps);
		MaxFps = Mathf.Min(MaxFps, AvgFps);

		return AvgFps;
	}
}

