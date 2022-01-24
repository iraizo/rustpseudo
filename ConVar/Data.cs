using System.IO;
using UnityEngine;

namespace ConVar
{
	[Factory("data")]
	public class Data : ConsoleSystem
	{
		[ServerVar]
		[ClientVar]
		public static void export(Arg args)
		{
			string @string = args.GetString(0, "none");
			string text = Path.Combine(Application.get_persistentDataPath(), @string + ".raw");
			switch (@string)
			{
			case "splatmap":
				if (Object.op_Implicit((Object)(object)TerrainMeta.SplatMap))
				{
					RawWriter.Write(TerrainMeta.SplatMap.ToEnumerable(), text);
				}
				break;
			case "heightmap":
				if (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap))
				{
					RawWriter.Write(TerrainMeta.HeightMap.ToEnumerable(), text);
				}
				break;
			case "biomemap":
				if (Object.op_Implicit((Object)(object)TerrainMeta.BiomeMap))
				{
					RawWriter.Write(TerrainMeta.BiomeMap.ToEnumerable(), text);
				}
				break;
			case "topologymap":
				if (Object.op_Implicit((Object)(object)TerrainMeta.TopologyMap))
				{
					RawWriter.Write(TerrainMeta.TopologyMap.ToEnumerable(), text);
				}
				break;
			case "alphamap":
				if (Object.op_Implicit((Object)(object)TerrainMeta.AlphaMap))
				{
					RawWriter.Write(TerrainMeta.AlphaMap.ToEnumerable(), text);
				}
				break;
			case "watermap":
				if (Object.op_Implicit((Object)(object)TerrainMeta.WaterMap))
				{
					RawWriter.Write(TerrainMeta.WaterMap.ToEnumerable(), text);
				}
				break;
			default:
				args.ReplyWith("Unknown export source: " + @string);
				return;
			}
			args.ReplyWith("Export written to " + text);
		}

		public Data()
			: this()
		{
		}
	}
}
