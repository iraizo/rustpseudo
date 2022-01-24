using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class GranularAudioClip : MonoBehaviour
{
	public class Grain
	{
		private float[] sourceData;

		private int sourceDataLength;

		private int startSample;

		private int currentSample;

		private int attackTimeSamples;

		private int sustainTimeSamples;

		private int releaseTimeSamples;

		private float gain;

		private float gainPerSampleAttack;

		private float gainPerSampleRelease;

		private int attackEndSample;

		private int releaseStartSample;

		private int endSample;

		public bool finished => currentSample >= endSample;

		public void Init(float[] source, int start, int attack, int sustain, int release)
		{
			sourceData = source;
			sourceDataLength = sourceData.Length;
			startSample = start;
			currentSample = start;
			attackTimeSamples = attack;
			sustainTimeSamples = sustain;
			releaseTimeSamples = release;
			gainPerSampleAttack = 1f / (float)attackTimeSamples;
			gainPerSampleRelease = -1f / (float)releaseTimeSamples;
			attackEndSample = startSample + attackTimeSamples;
			releaseStartSample = attackEndSample + sustainTimeSamples;
			endSample = releaseStartSample + releaseTimeSamples;
			gain = 0f;
		}

		public float GetSample()
		{
			int num = currentSample % sourceDataLength;
			if (num < 0)
			{
				num += sourceDataLength;
			}
			float num2 = sourceData[num];
			if (currentSample <= attackEndSample)
			{
				gain += gainPerSampleAttack;
			}
			else if (currentSample >= releaseStartSample)
			{
				gain += gainPerSampleRelease;
			}
			currentSample++;
			return num2 * gain;
		}
	}

	public AudioClip sourceClip;

	private float[] sourceAudioData;

	private int sourceChannels = 1;

	public AudioClip granularClip;

	public int sampleRate = 44100;

	public float sourceTime = 0.5f;

	public float sourceTimeVariation = 0.1f;

	public float grainAttack = 0.1f;

	public float grainSustain = 0.1f;

	public float grainRelease = 0.1f;

	public float grainFrequency = 0.1f;

	public int grainAttackSamples;

	public int grainSustainSamples;

	public int grainReleaseSamples;

	public int grainFrequencySamples;

	public int samplesUntilNextGrain;

	public List<Grain> grains = new List<Grain>();

	private Random random = new Random();

	private bool inited;

	private void Update()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		if (!inited && (int)sourceClip.get_loadState() == 2)
		{
			sampleRate = sourceClip.get_frequency();
			sourceAudioData = new float[sourceClip.get_samples() * sourceClip.get_channels()];
			sourceClip.GetData(sourceAudioData, 0);
			InitAudioClip();
			AudioSource component = ((Component)this).GetComponent<AudioSource>();
			component.set_clip(granularClip);
			component.set_loop(true);
			component.Play();
			inited = true;
		}
		RefreshCachedData();
	}

	private void RefreshCachedData()
	{
		grainAttackSamples = Mathf.FloorToInt(grainAttack * (float)sampleRate * (float)sourceChannels);
		grainSustainSamples = Mathf.FloorToInt(grainSustain * (float)sampleRate * (float)sourceChannels);
		grainReleaseSamples = Mathf.FloorToInt(grainRelease * (float)sampleRate * (float)sourceChannels);
		grainFrequencySamples = Mathf.FloorToInt(grainFrequency * (float)sampleRate * (float)sourceChannels);
	}

	private void InitAudioClip()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		int num = 1;
		int num2 = 1;
		AudioSettings.GetDSPBufferSize(ref num, ref num2);
		granularClip = AudioClip.Create(((Object)sourceClip).get_name() + " (granular)", num, sourceClip.get_channels(), sampleRate, true, new PCMReaderCallback(OnAudioRead));
		sourceChannels = sourceClip.get_channels();
	}

	private void OnAudioRead(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (samplesUntilNextGrain <= 0)
			{
				SpawnGrain();
			}
			float num = 0f;
			for (int j = 0; j < grains.Count; j++)
			{
				num += grains[j].GetSample();
			}
			data[i] = num;
			samplesUntilNextGrain--;
		}
		CleanupFinishedGrains();
	}

	private void SpawnGrain()
	{
		if (grainFrequencySamples != 0)
		{
			float num = (float)(random.NextDouble() * (double)sourceTimeVariation * 2.0) - sourceTimeVariation;
			int start = Mathf.FloorToInt((sourceTime + num) * (float)sampleRate / (float)sourceChannels);
			Grain grain = Pool.Get<Grain>();
			grain.Init(sourceAudioData, start, grainAttackSamples, grainSustainSamples, grainReleaseSamples);
			grains.Add(grain);
			samplesUntilNextGrain = grainFrequencySamples;
		}
	}

	private void CleanupFinishedGrains()
	{
		for (int num = grains.Count - 1; num >= 0; num--)
		{
			Grain grain = grains[num];
			if (grain.finished)
			{
				Pool.Free<Grain>(ref grain);
				grains.RemoveAt(num);
			}
		}
	}

	public GranularAudioClip()
		: this()
	{
	}
}
