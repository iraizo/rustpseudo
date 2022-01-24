using UnityEngine;

public static class ParticleSystemEx
{
	public static void SetPlayingState(this ParticleSystem ps, bool play)
	{
		if (play && !ps.get_isPlaying())
		{
			ps.Play();
		}
		else if (!play && ps.get_isPlaying())
		{
			ps.Stop();
		}
	}

	public static void SetEmitterState(this ParticleSystem ps, bool enable)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		EmissionModule emission = ps.get_emission();
		if (enable != ((EmissionModule)(ref emission)).get_enabled())
		{
			EmissionModule emission2 = ps.get_emission();
			((EmissionModule)(ref emission2)).set_enabled(enable);
		}
	}
}
