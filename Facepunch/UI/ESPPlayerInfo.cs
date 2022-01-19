using TMPro;
using UnityEngine;

namespace Facepunch.UI
{
	public class ESPPlayerInfo : MonoBehaviour
	{
		public Vector3 WorldOffset;

		public TextMeshProUGUI Text;

		public TextMeshProUGUI Image;

		public TextMeshProUGUI Loading;

		public CanvasGroup group;

		public Gradient gradientNormal;

		public Gradient gradientTeam;

		public Color TeamColor;

		public Color AllyColor = Color.get_blue();

		public Color EnemyColor;

		public QueryVis visCheck;

		public BasePlayer Entity { get; set; }

		public ESPPlayerInfo()
			: this()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)

	}
}
