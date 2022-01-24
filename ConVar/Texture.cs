using System.Text;
using UnityEngine;

namespace ConVar
{
	[Factory("texture")]
	public class Texture : ConsoleSystem
	{
		[ClientVar]
		public static int streamingBudgetOverride;

		[ClientVar(Saved = true, Help = "Enable/Disable texture streaming")]
		public static bool streaming
		{
			get
			{
				return QualitySettings.get_streamingMipmapsActive();
			}
			set
			{
				QualitySettings.set_streamingMipmapsActive(value);
			}
		}

		[ClientVar]
		public static void stats(Arg arg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"Supports streaming:               {SystemInfo.get_supportsMipStreaming()}");
			stringBuilder.AppendLine($"Streaming enabled:                {QualitySettings.get_streamingMipmapsActive()}");
			stringBuilder.AppendLine($"Immediately discard unused mips:  {Texture.get_streamingTextureDiscardUnusedMips()}");
			stringBuilder.AppendLine($"Max level of reduction:           {QualitySettings.get_streamingMipmapsMaxLevelReduction()}");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine($"currentTextureMemory:             {Texture.get_currentTextureMemory() / 1048576uL}MB (current estimated usage)");
			stringBuilder.AppendLine($"desiredTextureMemory:             {Texture.get_desiredTextureMemory() / 1048576uL}MB");
			stringBuilder.AppendLine($"nonStreamingTextureCount:         {Texture.get_nonStreamingTextureCount()}");
			stringBuilder.AppendLine($"nonStreamingTextureMemory:        {Texture.get_nonStreamingTextureMemory() / 1048576uL}MB");
			stringBuilder.AppendLine($"streamingTextureCount:            {Texture.get_streamingTextureCount()}");
			stringBuilder.AppendLine($"targetTextureMemory:              {Texture.get_targetTextureMemory() / 1048576uL}MB");
			stringBuilder.AppendLine($"totalTextureMemory:               {Texture.get_totalTextureMemory() / 1048576uL}MB (if everything was loaded at highest quality)");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine($"streamingMipmapUploadCount:       {Texture.get_streamingMipmapUploadCount()}");
			stringBuilder.AppendLine($"streamingTextureLoadingCount:     {Texture.get_streamingTextureLoadingCount()}");
			stringBuilder.AppendLine($"streamingTexturePendingLoadCount: {Texture.get_streamingTexturePendingLoadCount()}");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine($"TargetBudget:                     {QualitySettings.get_streamingMipmapsMemoryBudget()}MB");
			arg.ReplyWith(stringBuilder.ToString());
		}

		public Texture()
			: this()
		{
		}
	}
}
