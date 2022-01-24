using System;
using System.Globalization;
using UnityEngine;

namespace ConVar
{
	[Factory("weather")]
	public class Weather : ConsoleSystem
	{
		[ServerVar]
		public static float wetness_rain = 0.4f;

		[ServerVar]
		public static float wetness_snow = 0.2f;

		[ReplicatedVar(Default = "1")]
		public static float clear_chance
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return 1f;
				}
				return SingletonComponent<Climate>.Instance.Weather.ClearChance;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.Weather.ClearChance = Mathf.Clamp01(value);
				}
			}
		}

		[ReplicatedVar(Default = "0")]
		public static float dust_chance
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.DustChance;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.Weather.DustChance = Mathf.Clamp01(value);
				}
			}
		}

		[ReplicatedVar(Default = "0")]
		public static float fog_chance
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.FogChance;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.Weather.FogChance = Mathf.Clamp01(value);
				}
			}
		}

		[ReplicatedVar(Default = "0")]
		public static float overcast_chance
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.OvercastChance;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.Weather.OvercastChance = Mathf.Clamp01(value);
				}
			}
		}

		[ReplicatedVar(Default = "0")]
		public static float storm_chance
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.StormChance;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.Weather.StormChance = Mathf.Clamp01(value);
				}
			}
		}

		[ReplicatedVar(Default = "0")]
		public static float rain_chance
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.RainChance;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.Weather.RainChance = Mathf.Clamp01(value);
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float rain
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Rain;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Rain = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float wind
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Wind;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Wind = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float thunder
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Thunder;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Thunder = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float rainbow
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Rainbow;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Rainbow = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float fog
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Fogginess;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Fogginess = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_rayleigh
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.RayleighMultiplier;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.RayleighMultiplier = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_mie
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.MieMultiplier;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.MieMultiplier = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_brightness
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Brightness;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Brightness = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_contrast
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Contrast;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Contrast = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_directionality
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Directionality;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Directionality = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float cloud_size
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Size;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Size = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float cloud_opacity
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Opacity;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Opacity = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float cloud_coverage
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coverage;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coverage = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float cloud_sharpness
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Sharpness;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Sharpness = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float cloud_coloring
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coloring;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coloring = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float cloud_attenuation
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Attenuation;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Attenuation = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float cloud_saturation
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Saturation;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Saturation = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float cloud_scattering
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Scattering;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Scattering = value;
				}
			}
		}

		[ReplicatedVar(Default = "-1")]
		public static float cloud_brightness
		{
			get
			{
				if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Brightness;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
				{
					SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Brightness = value;
				}
			}
		}

		[ClientVar]
		[ServerVar]
		public static void load(Arg args)
		{
			if (!Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
			{
				return;
			}
			string name = args.GetString(0, "");
			if (string.IsNullOrEmpty(name))
			{
				args.ReplyWith("Weather preset name invalid.");
				return;
			}
			WeatherPreset weatherPreset = Array.Find(SingletonComponent<Climate>.Instance.WeatherPresets, (WeatherPreset x) => StringEx.Contains(((Object)x).get_name(), name, CompareOptions.IgnoreCase));
			if ((Object)(object)weatherPreset == (Object)null)
			{
				args.ReplyWith("Weather preset not found: " + name);
				return;
			}
			SingletonComponent<Climate>.Instance.WeatherOverrides.Set(weatherPreset);
			if (args.get_IsServerside())
			{
				ServerMgr.SendReplicatedVars("weather.");
			}
		}

		[ClientVar]
		[ServerVar]
		public static void reset(Arg args)
		{
			if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
			{
				SingletonComponent<Climate>.Instance.WeatherOverrides.Reset();
				if (args.get_IsServerside())
				{
					ServerMgr.SendReplicatedVars("weather.");
				}
			}
		}

		[ClientVar]
		[ServerVar]
		public static void report(Arg args)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			if (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance))
			{
				TextTable val = new TextTable();
				val.AddColumn(((Object)SingletonComponent<Climate>.Instance.WeatherStatePrevious).get_name());
				val.AddColumn("|");
				val.AddColumn(((Object)SingletonComponent<Climate>.Instance.WeatherStateTarget).get_name());
				val.AddColumn("|");
				val.AddColumn(((Object)SingletonComponent<Climate>.Instance.WeatherStateNext).get_name());
				int num = Mathf.RoundToInt(SingletonComponent<Climate>.Instance.WeatherStateBlend * 100f);
				if (num < 100)
				{
					val.AddRow(new string[5]
					{
						"fading out (" + (100 - num) + "%)",
						"|",
						"fading in (" + num + "%)",
						"|",
						"up next"
					});
				}
				else
				{
					val.AddRow(new string[5] { "previous", "|", "current", "|", "up next" });
				}
				args.ReplyWith(((object)val).ToString() + Environment.NewLine + ((object)SingletonComponent<Climate>.Instance.WeatherState).ToString());
			}
		}

		public Weather()
			: this()
		{
		}
	}
}
