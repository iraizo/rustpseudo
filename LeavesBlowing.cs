using UnityEngine;

public class LeavesBlowing : MonoBehaviour
{
	public ParticleSystem m_psLeaves;

	public float m_flSwirl;

	public float m_flSpeed;

	public float m_flEmissionRate;

	private void Start()
	{
	}

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).get_transform().RotateAround(((Component)this).get_transform().get_position(), Vector3.get_up(), Time.get_deltaTime() * m_flSwirl);
		if ((Object)(object)m_psLeaves != (Object)null)
		{
			m_psLeaves.set_startSpeed(m_flSpeed);
			ParticleSystem psLeaves = m_psLeaves;
			psLeaves.set_startSpeed(psLeaves.get_startSpeed() + Mathf.Sin(Time.get_time() * 0.4f) * (m_flSpeed * 0.75f));
			m_psLeaves.set_emissionRate(m_flEmissionRate + Mathf.Sin(Time.get_time() * 1f) * (m_flEmissionRate * 0.3f));
		}
	}

	public LeavesBlowing()
		: this()
	{
	}
}
