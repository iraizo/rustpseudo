using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rust;
using UnityEngine;
using UnityEngine.Video;

public class MenuBackgroundVideo : SingletonComponent<MenuBackgroundVideo>
{
	private string[] videos;

	private int index;

	private bool errored;

	protected override void Awake()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		((SingletonComponent)this).Awake();
		LoadVideoList();
		NextVideo();
		((Component)this).GetComponent<VideoPlayer>().add_errorReceived(new ErrorEventHandler(OnVideoError));
	}

	private void OnVideoError(VideoPlayer source, string message)
	{
		errored = true;
	}

	public void LoadVideoList()
	{
		videos = Enumerable.ToArray<string>((IEnumerable<string>)Enumerable.OrderBy<string, Guid>(Enumerable.Where<string>(Directory.EnumerateFiles(Application.get_streamingAssetsPath() + "/MenuVideo/"), (Func<string, bool>)((string x) => x.EndsWith(".mp4") || x.EndsWith(".webm"))), (Func<string, Guid>)((string x) => Guid.NewGuid())));
	}

	public void Update()
	{
		if (Input.GetKeyDown((KeyCode)258))
		{
			LoadVideoList();
		}
		if (Input.GetKeyDown((KeyCode)257))
		{
			NextVideo();
		}
	}

	private void NextVideo()
	{
		if (Application.isQuitting)
		{
			return;
		}
		string text = videos[index++ % videos.Length];
		errored = false;
		if (Global.get_LaunchCountThisVersion() <= 3)
		{
			string text2 = Enumerable.FirstOrDefault<string>(Enumerable.Where<string>((IEnumerable<string>)videos, (Func<string, bool>)((string x) => x.EndsWith("whatsnew.mp4"))));
			if (!string.IsNullOrEmpty(text2))
			{
				text = text2;
			}
		}
		Debug.Log((object)("Playing Video " + text));
		VideoPlayer component = ((Component)this).GetComponent<VideoPlayer>();
		component.set_url(text);
		component.Play();
	}

	internal IEnumerator ReadyVideo()
	{
		if (!errored)
		{
			VideoPlayer player = ((Component)this).GetComponent<VideoPlayer>();
			while (!player.get_isPrepared() && !errored)
			{
				yield return null;
			}
		}
	}
}
