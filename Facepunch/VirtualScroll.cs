using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Facepunch
{
	public class VirtualScroll : MonoBehaviour
	{
		public interface IDataSource
		{
			int GetItemCount();

			void SetItemData(int i, GameObject obj);
		}

		public int ItemHeight = 40;

		public int ItemSpacing = 10;

		public RectOffset Padding;

		[Tooltip("Optional, we'll try to GetComponent IDataSource from this object on awake")]
		public GameObject DataSourceObject;

		public GameObject SourceObject;

		public ScrollRect ScrollRect;

		private IDataSource dataSource;

		private Dictionary<int, GameObject> ActivePool = new Dictionary<int, GameObject>();

		private Stack<GameObject> InactivePool = new Stack<GameObject>();

		private int BlockHeight => ItemHeight + ItemSpacing;

		public void Awake()
		{
			((UnityEvent<Vector2>)(object)ScrollRect.get_onValueChanged()).AddListener((UnityAction<Vector2>)OnScrollChanged);
			if ((Object)(object)DataSourceObject != (Object)null)
			{
				SetDataSource(DataSourceObject.GetComponent<IDataSource>());
			}
		}

		public void OnDestroy()
		{
			((UnityEvent<Vector2>)(object)ScrollRect.get_onValueChanged()).RemoveListener((UnityAction<Vector2>)OnScrollChanged);
		}

		private void OnScrollChanged(Vector2 pos)
		{
			Rebuild();
		}

		public void SetDataSource(IDataSource source)
		{
			if (dataSource != source)
			{
				dataSource = source;
				FullRebuild();
			}
		}

		public void FullRebuild()
		{
			int[] array = ActivePool.Keys.ToArray();
			foreach (int key in array)
			{
				Recycle(key);
			}
			Rebuild();
		}

		public void DataChanged()
		{
			foreach (KeyValuePair<int, GameObject> item in ActivePool)
			{
				dataSource.SetItemData(item.Key, item.Value);
			}
			Rebuild();
		}

		public void Rebuild()
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			if (dataSource == null)
			{
				return;
			}
			int itemCount = dataSource.GetItemCount();
			Transform child = ((Transform)ScrollRect.get_viewport()).GetChild(0);
			Transform obj = ((child is RectTransform) ? child : null);
			((RectTransform)obj).SetSizeWithCurrentAnchors((Axis)1, (float)(BlockHeight * itemCount - ItemSpacing + Padding.get_top() + Padding.get_bottom()));
			Rect rect = ScrollRect.get_viewport().get_rect();
			int num = Mathf.Max(2, Mathf.CeilToInt(((Rect)(ref rect)).get_height() / (float)BlockHeight));
			int num2 = Mathf.FloorToInt((((RectTransform)obj).get_anchoredPosition().y - (float)Padding.get_top()) / (float)BlockHeight);
			int num3 = num2 + num;
			RecycleOutOfRange(num2, num3);
			for (int i = num2; i <= num3; i++)
			{
				if (i >= 0 && i < itemCount)
				{
					BuildItem(i);
				}
			}
		}

		private void RecycleOutOfRange(int startVisible, float endVisible)
		{
			int[] array = (from x in ActivePool.Keys
				where x < startVisible || (float)x > endVisible
				select (x)).ToArray();
			foreach (int key in array)
			{
				Recycle(key);
			}
		}

		private void Recycle(int key)
		{
			GameObject val = ActivePool[key];
			val.SetActive(false);
			ActivePool.Remove(key);
			InactivePool.Push(val);
		}

		private void BuildItem(int i)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			if (i >= 0 && !ActivePool.ContainsKey(i))
			{
				GameObject item = GetItem();
				item.SetActive(true);
				dataSource.SetItemData(i, item);
				Transform transform = item.get_transform();
				Transform obj = ((transform is RectTransform) ? transform : null);
				((RectTransform)obj).set_anchorMin(new Vector2(0f, 1f));
				((RectTransform)obj).set_anchorMax(new Vector2(1f, 1f));
				((RectTransform)obj).set_pivot(new Vector2(0.5f, 1f));
				((RectTransform)obj).set_offsetMin(new Vector2(0f, 0f));
				((RectTransform)obj).set_offsetMax(new Vector2(0f, (float)ItemHeight));
				((RectTransform)obj).set_sizeDelta(new Vector2((float)((Padding.get_left() + Padding.get_right()) * -1), (float)ItemHeight));
				((RectTransform)obj).set_anchoredPosition(new Vector2((float)(Padding.get_left() - Padding.get_right()) * 0.5f, (float)(-1 * (i * BlockHeight + Padding.get_top()))));
				ActivePool[i] = item;
			}
		}

		private GameObject GetItem()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			if (InactivePool.Count == 0)
			{
				GameObject val = Object.Instantiate<GameObject>(SourceObject);
				val.get_transform().SetParent(((Transform)ScrollRect.get_viewport()).GetChild(0), false);
				val.get_transform().set_localScale(Vector3.get_one());
				val.SetActive(false);
				InactivePool.Push(val);
			}
			return InactivePool.Pop();
		}

		public VirtualScroll()
			: this()
		{
		}
	}
}
