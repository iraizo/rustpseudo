using System;
using UnityEngine;

namespace ConVar
{
	[Factory("audio")]
	public class Audio : ConsoleSystem
	{
		[ClientVar(Help = "Volume", Saved = true)]
		public static float master = 1f;

		[ClientVar(Help = "Volume", Saved = true)]
		public static float musicvolume = 1f;

		[ClientVar(Help = "Volume", Saved = true)]
		public static float musicvolumemenu = 1f;

		[ClientVar(Help = "Volume", Saved = true)]
		public static float game = 1f;

		[ClientVar(Help = "Volume", Saved = true)]
		public static float voices = 1f;

		[ClientVar(Help = "Volume", Saved = true)]
		public static float instruments = 1f;

		[ClientVar(Help = "Volume", Saved = true)]
		public static float voiceProps = 1f;

		[ClientVar(Help = "Ambience System")]
		public static bool ambience = true;

		[ClientVar(Help = "Max ms per frame to spend updating sounds")]
		public static float framebudget = 0.3f;

		[ClientVar]
		public static float minupdatefraction = 0.1f;

		[ClientVar(Help = "Use more advanced sound occlusion", Saved = true)]
		public static bool advancedocclusion = false;

		[ClientVar(Help = "Use higher quality sound fades on some sounds")]
		public static bool hqsoundfade = false;

		[ClientVar(Saved = false)]
		public static bool debugVoiceLimiting = false;

		[ClientVar(Help = "Volume", Saved = true)]
		public static int speakers
		{
			get
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Expected I4, but got Unknown
				return (int)AudioSettings.get_speakerMode();
			}
			set
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				value = Mathf.Clamp(value, 2, 7);
				if (!Application.get_isEditor())
				{
					AudioConfiguration configuration = AudioSettings.GetConfiguration();
					configuration.speakerMode = (AudioSpeakerMode)value;
					TimeWarning val = TimeWarning.New("Audio Settings Reset", 250);
					try
					{
						AudioSettings.Reset(configuration);
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
			}
		}

		[ClientVar]
		public static void printSounds(Arg arg)
		{
		}

		[ClientVar(ClientAdmin = true, Help = "print active engine sound info")]
		public static void printEngineSounds(Arg arg)
		{
		}

		public Audio()
			: this()
		{
		}
	}
}
