using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.Utility
{
	[RequireComponent(typeof(Toggle))]
	internal class ForceWeather : MonoBehaviour
	{
		private Toggle component;

		public bool Rain;

		public bool Fog;

		public bool Wind;

		public bool Clouds;

		public void OnEnable()
		{
			component = ((Component)this).GetComponent<Toggle>();
		}

		public void Update()
		{
			if (!((Object)(object)SingletonComponent<Climate>.Instance == (Object)null))
			{
				if (Rain)
				{
					SingletonComponent<Climate>.Instance.Overrides.Rain = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Rain, (float)(component.get_isOn() ? 1 : 0), Time.get_deltaTime() / 2f);
				}
				if (Fog)
				{
					SingletonComponent<Climate>.Instance.Overrides.Fog = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Fog, (float)(component.get_isOn() ? 1 : 0), Time.get_deltaTime() / 2f);
				}
				if (Wind)
				{
					SingletonComponent<Climate>.Instance.Overrides.Wind = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Wind, (float)(component.get_isOn() ? 1 : 0), Time.get_deltaTime() / 2f);
				}
				if (Clouds)
				{
					SingletonComponent<Climate>.Instance.Overrides.Clouds = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Clouds, (float)(component.get_isOn() ? 1 : 0), Time.get_deltaTime() / 2f);
				}
			}
		}

		public ForceWeather()
			: this()
		{
		}
	}
}
