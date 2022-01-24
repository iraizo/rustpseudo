using ConVar;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
	public AudioMixer mixer;

	private void Update()
	{
		if (!((Object)(object)mixer == (Object)null))
		{
			mixer.SetFloat("MasterVol", LinearToDecibel(Audio.master));
			float num = default(float);
			mixer.GetFloat("MusicVol", ref num);
			if (!LevelManager.isLoaded || !MainCamera.isValid)
			{
				mixer.SetFloat("MusicVol", Mathf.Lerp(num, LinearToDecibel(Audio.musicvolumemenu), Time.get_deltaTime()));
			}
			else
			{
				mixer.SetFloat("MusicVol", Mathf.Lerp(num, LinearToDecibel(Audio.musicvolume), Time.get_deltaTime()));
			}
			mixer.SetFloat("WorldVol", LinearToDecibel(Audio.game));
			mixer.SetFloat("VoiceVol", LinearToDecibel(Audio.voices));
			mixer.SetFloat("InstrumentVol", LinearToDecibel(Audio.instruments));
			float num2 = LinearToDecibel(Audio.voiceProps) - 28.7f;
			mixer.SetFloat("VoicePropsVol", num2);
		}
	}

	private float LinearToDecibel(float linear)
	{
		if (linear > 0f)
		{
			return 20f * Mathf.Log10(linear);
		}
		return -144f;
	}

	public AudioSettings()
		: this()
	{
	}
}
