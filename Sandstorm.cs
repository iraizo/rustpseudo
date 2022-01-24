using UnityEngine;

public class Sandstorm : MonoBehaviour
{
	public ParticleSystem m_psSandStorm;

	public float m_flSpeed;

	public float m_flSwirl;

	public float m_flEmissionRate;

	private void Start()
	{
	}

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).get_transform().RotateAround(((Component)this).get_transform().get_position(), Vector3.get_up(), Time.get_deltaTime() * m_flSwirl);
		Vector3 eulerAngles = ((Component)this).get_transform().get_eulerAngles();
		eulerAngles.x = -7f + Mathf.Sin(Time.get_time() * 2.5f) * 7f;
		((Component)this).get_transform().set_eulerAngles(eulerAngles);
		if ((Object)(object)m_psSandStorm != (Object)null)
		{
			m_psSandStorm.set_startSpeed(m_flSpeed);
			ParticleSystem psSandStorm = m_psSandStorm;
			psSandStorm.set_startSpeed(psSandStorm.get_startSpeed() + Mathf.Sin(Time.get_time() * 0.4f) * (m_flSpeed * 0.75f));
			m_psSandStorm.set_emissionRate(m_flEmissionRate + Mathf.Sin(Time.get_time() * 1f) * (m_flEmissionRate * 0.3f));
		}
	}

	public Sandstorm()
		: this()
	{
	}
}
