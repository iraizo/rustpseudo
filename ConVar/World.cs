using System;
using System.IO;
using UnityEngine;

namespace ConVar
{
	[Factory("world")]
	public class World : ConsoleSystem
	{
		[ServerVar]
		[ClientVar]
		public static bool cache = true;

		[ClientVar]
		public static bool streaming = true;

		[ClientVar]
		public static void monuments(Arg arg)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			if (!Object.op_Implicit((Object)(object)TerrainMeta.Path))
			{
				return;
			}
			TextTable val = new TextTable();
			val.AddColumn("type");
			val.AddColumn("name");
			val.AddColumn("pos");
			foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
			{
				string[] obj = new string[3]
				{
					monument.Type.ToString(),
					((Object)monument).get_name(),
					null
				};
				Vector3 position = ((Component)monument).get_transform().get_position();
				obj[2] = ((object)(Vector3)(ref position)).ToString();
				val.AddRow(obj);
			}
			arg.ReplyWith(((object)val).ToString());
		}

		[ServerVar(Clientside = true, Help = "Renders a high resolution PNG of the current map")]
		public static void rendermap(Arg arg)
		{
			float @float = arg.GetFloat(0, 1f);
			int imageWidth;
			int imageHeight;
			Color background;
			byte[] array = MapImageRenderer.Render(out imageWidth, out imageHeight, out background, @float, lossy: false);
			if (array == null)
			{
				arg.ReplyWith("Failed to render the map (is a map loaded now?)");
				return;
			}
			string fullPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, $"map_{global::World.Size}_{global::World.Seed}.png"));
			File.WriteAllBytes(fullPath, array);
			arg.ReplyWith("Saved map render to: " + fullPath);
		}

		[ServerVar(Clientside = true, Help = "Renders a PNG of the current map's tunnel network")]
		public static void rendertunnels(Arg arg)
		{
			RenderMapLayerToFile(arg, "tunnels", MapLayer.TrainTunnels);
		}

		[ServerVar(Clientside = true, Help = "Renders a PNG of the current map's underwater labs, for a specific floor")]
		public static void renderlabs(Arg arg)
		{
			int underwaterLabFloorCount = MapLayerRenderer.GetOrCreate().GetUnderwaterLabFloorCount();
			int @int = arg.GetInt(0, 0);
			if (@int < 0 || @int >= underwaterLabFloorCount)
			{
				arg.ReplyWith($"Floor number must be between 0 and {underwaterLabFloorCount}");
			}
			else
			{
				RenderMapLayerToFile(arg, $"labs_{@int}", (MapLayer)(1 + @int));
			}
		}

		private static void RenderMapLayerToFile(Arg arg, string name, MapLayer layer)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				MapLayerRenderer orCreate = MapLayerRenderer.GetOrCreate();
				orCreate.Render(layer);
				string fullPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, $"{name}_{global::World.Size}_{global::World.Seed}.png"));
				RenderTexture targetTexture = orCreate.renderCamera.get_targetTexture();
				Texture2D val = new Texture2D(((Texture)targetTexture).get_width(), ((Texture)targetTexture).get_height());
				RenderTexture active = RenderTexture.get_active();
				try
				{
					RenderTexture.set_active(targetTexture);
					val.ReadPixels(new Rect(0f, 0f, (float)((Texture)targetTexture).get_width(), (float)((Texture)targetTexture).get_height()), 0, 0);
					val.Apply();
					File.WriteAllBytes(fullPath, ImageConversion.EncodeToPNG(val));
				}
				finally
				{
					RenderTexture.set_active(active);
					Object.DestroyImmediate((Object)(object)val);
				}
				arg.ReplyWith("Saved " + name + " render to: " + fullPath);
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)ex);
				arg.ReplyWith("Failed to render " + name);
			}
		}

		public World()
			: this()
		{
		}
	}
}
