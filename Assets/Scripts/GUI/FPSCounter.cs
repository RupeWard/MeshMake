using UnityEngine;
using System.Collections.Generic;

public class FPSCounter : MonoBehaviour
{
	public UnityEngine.UI.Text fpsText;

	private Queue< float > intervals_ = new Queue< float >();
	public int maxIntervals = 100;
	public int minIntervals = 50;
	public float accum = 0f;
	public int displayInterval = 40;
	private int sinceDisplay = 0;

	private bool active_ = true;
	public void ToggleActive()
	{
		active_ = !active_;
		if ( !active_ )
		{
			intervals_.Clear();
			if (fpsText != null)
			{
				fpsText.text = "FPS";
			}

		}
	}

	public System.Action <float> SendFPS;

	void Update () 
	{
		if ( active_ )
		{
			intervals_.Enqueue (Time.deltaTime);
			accum += Time.deltaTime;
			
			while (intervals_.Count > maxIntervals)
			{
				float f = intervals_.Dequeue();
				accum -= f;
			}
			if (intervals_.Count >= minIntervals)
			{
				sinceDisplay--;
				if (sinceDisplay < 0)
				{
					sinceDisplay = displayInterval;
					float meanDeltaTime = ( accum/intervals_.Count );
					float fps = 1f/meanDeltaTime;
					if (SendFPS != null)
					{
						SendFPS(fps);
					}
					if (fpsText != null)
					{
						fpsText.text = fps.ToString("F1");
					}
				}
			}
		}

	}
}
