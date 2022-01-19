using System;
using System.Collections.Generic;
using Facepunch.Math;
using UnityEngine;

namespace Facepunch
{
	public static class Output
	{
		public struct Entry
		{
			public string Message;

			public string Stacktrace;

			public string Type;

			public int Time;
		}

		public static bool installed = false;

		public static List<Entry> HistoryOutput = new List<Entry>();

		public static event Action<string, string, LogType> OnMessage;

		public static void Install()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			if (!installed)
			{
				Application.add_logMessageReceived(new LogCallback(LogHandler));
				installed = true;
			}
		}

		internal static void LogHandler(string log, string stacktrace, LogType type)
		{
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			if (Output.OnMessage == null || log.StartsWith("Kinematic body only supports Speculative Continuous collision detection") || log.StartsWith("Skipped frame because GfxDevice") || log.StartsWith("Your current multi-scene setup has inconsistent Lighting") || log.Contains("HandleD3DDeviceLost") || log.Contains("ResetD3DDevice") || log.Contains("dev->Reset") || log.Contains("D3Dwindow device not lost anymore") || log.Contains("D3D device reset") || log.Contains("group < 0xfff") || log.Contains("Mesh can not have more than 65000 vert") || log.Contains("Trying to add (Layout Rebuilder for)") || log.Contains("Coroutine continue failure") || log.Contains("No texture data available to upload") || log.Contains("Trying to reload asset from disk that is not") || log.Contains("Unable to find shaders used for the terrain engine.") || log.Contains("Canvas element contains more than 65535 vertices") || log.Contains("RectTransform.set_anchorMin") || log.Contains("FMOD failed to initialize the output device") || log.Contains("Cannot create FMOD::Sound") || log.Contains("invalid utf-16 sequence") || log.Contains("missing surrogate tail") || log.Contains("Failed to create agent because it is not close enough to the Nav") || log.Contains("user-provided triangle mesh descriptor is invalid") || log.Contains("Releasing render texture that is set as"))
			{
				return;
			}
			TimeWarning val = TimeWarning.New("Facepunch.Output.LogHandler", 0);
			try
			{
				Output.OnMessage?.Invoke(log, stacktrace, type);
			}
			catch (Exception)
			{
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			HistoryOutput.Add(new Entry
			{
				Message = log,
				Stacktrace = stacktrace,
				Type = ((object)(LogType)(ref type)).ToString(),
				Time = Epoch.get_Current()
			});
			while (HistoryOutput.Count > 65536)
			{
				HistoryOutput.RemoveAt(0);
			}
		}
	}
}
