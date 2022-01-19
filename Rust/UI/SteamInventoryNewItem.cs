using System.Threading.Tasks;
using UnityEngine;

namespace Rust.UI
{
	public class SteamInventoryNewItem : MonoBehaviour
	{
		public async Task Open(IPlayerItem item)
		{
			((Component)this).get_gameObject().SetActive(true);
			((Component)this).GetComponentInChildren<SteamInventoryItem>().Setup(item);
			while (Object.op_Implicit((Object)(object)this) && ((Component)this).get_gameObject().get_activeSelf())
			{
				await Task.Delay(100);
			}
		}

		public SteamInventoryNewItem()
			: this()
		{
		}
	}
}
