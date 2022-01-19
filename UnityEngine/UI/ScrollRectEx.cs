using System;
using System.Runtime.CompilerServices;
using Rust;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Scroll Rect Ex", 37)]
	[SelectionBase]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class ScrollRectEx : UIBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutGroup, ILayoutController
	{
		public enum MovementType
		{
			Unrestricted,
			Elastic,
			Clamped
		}

		public enum ScrollbarVisibility
		{
			Permanent,
			AutoHide,
			AutoHideAndExpandViewport
		}

		[Serializable]
		public class ScrollRectEvent : UnityEvent<Vector2>
		{
		}

		public InputButton scrollButton;

		public InputButton altScrollButton;

		[SerializeField]
		private RectTransform m_Content;

		[SerializeField]
		private bool m_Horizontal = true;

		[SerializeField]
		private bool m_Vertical = true;

		[SerializeField]
		private MovementType m_MovementType = MovementType.Elastic;

		[SerializeField]
		private float m_Elasticity = 0.1f;

		[SerializeField]
		private bool m_Inertia = true;

		[SerializeField]
		private float m_DecelerationRate = 0.135f;

		[SerializeField]
		private float m_ScrollSensitivity = 1f;

		[SerializeField]
		private RectTransform m_Viewport;

		[SerializeField]
		private Scrollbar m_HorizontalScrollbar;

		[SerializeField]
		private Scrollbar m_VerticalScrollbar;

		[SerializeField]
		private ScrollbarVisibility m_HorizontalScrollbarVisibility;

		[SerializeField]
		private ScrollbarVisibility m_VerticalScrollbarVisibility;

		[SerializeField]
		private float m_HorizontalScrollbarSpacing;

		[SerializeField]
		private float m_VerticalScrollbarSpacing;

		[SerializeField]
		private ScrollRectEvent m_OnValueChanged = new ScrollRectEvent();

		private Vector2 m_PointerStartLocalCursor = Vector2.get_zero();

		private Vector2 m_ContentStartPosition = Vector2.get_zero();

		private RectTransform m_ViewRect;

		private Bounds m_ContentBounds;

		private Bounds m_ViewBounds;

		private Vector2 m_Velocity;

		private bool m_Dragging;

		private Vector2 m_PrevPosition = Vector2.get_zero();

		private Bounds m_PrevContentBounds;

		private Bounds m_PrevViewBounds;

		[NonSerialized]
		private bool m_HasRebuiltLayout;

		private bool m_HSliderExpand;

		private bool m_VSliderExpand;

		private float m_HSliderHeight;

		private float m_VSliderWidth;

		[NonSerialized]
		private RectTransform m_Rect;

		private RectTransform m_HorizontalScrollbarRect;

		private RectTransform m_VerticalScrollbarRect;

		private DrivenRectTransformTracker m_Tracker;

		private readonly Vector3[] m_Corners = (Vector3[])(object)new Vector3[4];

		public RectTransform content
		{
			get
			{
				return m_Content;
			}
			set
			{
				m_Content = value;
			}
		}

		public bool horizontal
		{
			get
			{
				return m_Horizontal;
			}
			set
			{
				m_Horizontal = value;
			}
		}

		public bool vertical
		{
			get
			{
				return m_Vertical;
			}
			set
			{
				m_Vertical = value;
			}
		}

		public MovementType movementType
		{
			get
			{
				return m_MovementType;
			}
			set
			{
				m_MovementType = value;
			}
		}

		public float elasticity
		{
			get
			{
				return m_Elasticity;
			}
			set
			{
				m_Elasticity = value;
			}
		}

		public bool inertia
		{
			get
			{
				return m_Inertia;
			}
			set
			{
				m_Inertia = value;
			}
		}

		public float decelerationRate
		{
			get
			{
				return m_DecelerationRate;
			}
			set
			{
				m_DecelerationRate = value;
			}
		}

		public float scrollSensitivity
		{
			get
			{
				return m_ScrollSensitivity;
			}
			set
			{
				m_ScrollSensitivity = value;
			}
		}

		public RectTransform viewport
		{
			get
			{
				return m_Viewport;
			}
			set
			{
				m_Viewport = value;
				SetDirtyCaching();
			}
		}

		public Scrollbar horizontalScrollbar
		{
			get
			{
				return m_HorizontalScrollbar;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)m_HorizontalScrollbar))
				{
					((UnityEvent<float>)(object)m_HorizontalScrollbar.get_onValueChanged()).RemoveListener((UnityAction<float>)SetHorizontalNormalizedPosition);
				}
				m_HorizontalScrollbar = value;
				if (Object.op_Implicit((Object)(object)m_HorizontalScrollbar))
				{
					((UnityEvent<float>)(object)m_HorizontalScrollbar.get_onValueChanged()).AddListener((UnityAction<float>)SetHorizontalNormalizedPosition);
				}
				SetDirtyCaching();
			}
		}

		public Scrollbar verticalScrollbar
		{
			get
			{
				return m_VerticalScrollbar;
			}
			set
			{
				if (Object.op_Implicit((Object)(object)m_VerticalScrollbar))
				{
					((UnityEvent<float>)(object)m_VerticalScrollbar.get_onValueChanged()).RemoveListener((UnityAction<float>)SetVerticalNormalizedPosition);
				}
				m_VerticalScrollbar = value;
				if (Object.op_Implicit((Object)(object)m_VerticalScrollbar))
				{
					((UnityEvent<float>)(object)m_VerticalScrollbar.get_onValueChanged()).AddListener((UnityAction<float>)SetVerticalNormalizedPosition);
				}
				SetDirtyCaching();
			}
		}

		public ScrollbarVisibility horizontalScrollbarVisibility
		{
			get
			{
				return m_HorizontalScrollbarVisibility;
			}
			set
			{
				m_HorizontalScrollbarVisibility = value;
				SetDirtyCaching();
			}
		}

		public ScrollbarVisibility verticalScrollbarVisibility
		{
			get
			{
				return m_VerticalScrollbarVisibility;
			}
			set
			{
				m_VerticalScrollbarVisibility = value;
				SetDirtyCaching();
			}
		}

		public float horizontalScrollbarSpacing
		{
			get
			{
				return m_HorizontalScrollbarSpacing;
			}
			set
			{
				m_HorizontalScrollbarSpacing = value;
				SetDirty();
			}
		}

		public float verticalScrollbarSpacing
		{
			get
			{
				return m_VerticalScrollbarSpacing;
			}
			set
			{
				m_VerticalScrollbarSpacing = value;
				SetDirty();
			}
		}

		public ScrollRectEvent onValueChanged
		{
			get
			{
				return m_OnValueChanged;
			}
			set
			{
				m_OnValueChanged = value;
			}
		}

		protected RectTransform viewRect
		{
			get
			{
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Expected O, but got Unknown
				if ((Object)(object)m_ViewRect == (Object)null)
				{
					m_ViewRect = m_Viewport;
				}
				if ((Object)(object)m_ViewRect == (Object)null)
				{
					m_ViewRect = (RectTransform)((Component)this).get_transform();
				}
				return m_ViewRect;
			}
		}

		public Vector2 velocity
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return m_Velocity;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				m_Velocity = value;
			}
		}

		private RectTransform rectTransform
		{
			get
			{
				if ((Object)(object)m_Rect == (Object)null)
				{
					m_Rect = ((Component)this).GetComponent<RectTransform>();
				}
				return m_Rect;
			}
		}

		public Vector2 normalizedPosition
		{
			get
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition);
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				SetNormalizedPosition(value.x, 0);
				SetNormalizedPosition(value.y, 1);
			}
		}

		public float horizontalNormalizedPosition
		{
			get
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				UpdateBounds();
				if (((Bounds)(ref m_ContentBounds)).get_size().x <= ((Bounds)(ref m_ViewBounds)).get_size().x)
				{
					return (((Bounds)(ref m_ViewBounds)).get_min().x > ((Bounds)(ref m_ContentBounds)).get_min().x) ? 1 : 0;
				}
				return (((Bounds)(ref m_ViewBounds)).get_min().x - ((Bounds)(ref m_ContentBounds)).get_min().x) / (((Bounds)(ref m_ContentBounds)).get_size().x - ((Bounds)(ref m_ViewBounds)).get_size().x);
			}
			set
			{
				SetNormalizedPosition(value, 0);
			}
		}

		public float verticalNormalizedPosition
		{
			get
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				UpdateBounds();
				if (((Bounds)(ref m_ContentBounds)).get_size().y <= ((Bounds)(ref m_ViewBounds)).get_size().y)
				{
					return (((Bounds)(ref m_ViewBounds)).get_min().y > ((Bounds)(ref m_ContentBounds)).get_min().y) ? 1 : 0;
				}
				return (((Bounds)(ref m_ViewBounds)).get_min().y - ((Bounds)(ref m_ContentBounds)).get_min().y) / (((Bounds)(ref m_ContentBounds)).get_size().y - ((Bounds)(ref m_ViewBounds)).get_size().y);
			}
			set
			{
				SetNormalizedPosition(value, 1);
			}
		}

		private bool hScrollingNeeded
		{
			get
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				if (Application.get_isPlaying())
				{
					return ((Bounds)(ref m_ContentBounds)).get_size().x > ((Bounds)(ref m_ViewBounds)).get_size().x + 0.01f;
				}
				return true;
			}
		}

		private bool vScrollingNeeded
		{
			get
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				if (Application.get_isPlaying())
				{
					return ((Bounds)(ref m_ContentBounds)).get_size().y > ((Bounds)(ref m_ViewBounds)).get_size().y + 0.01f;
				}
				return true;
			}
		}

		protected ScrollRectEx()
			: this()
		{
		}//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)


		public virtual void Rebuild(CanvasUpdate executing)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if ((int)executing == 0)
			{
				UpdateCachedData();
			}
			if ((int)executing == 2)
			{
				UpdateBounds();
				UpdateScrollbars(Vector2.get_zero());
				UpdatePrevData();
				m_HasRebuiltLayout = true;
			}
		}

		private void UpdateCachedData()
		{
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = ((Component)this).get_transform();
			m_HorizontalScrollbarRect = (RectTransform)(((Object)(object)m_HorizontalScrollbar == (Object)null) ? null : /*isinst with value type is only supported in some contexts*/);
			m_VerticalScrollbarRect = (RectTransform)(((Object)(object)m_VerticalScrollbar == (Object)null) ? null : /*isinst with value type is only supported in some contexts*/);
			bool num = (Object)(object)((Transform)viewRect).get_parent() == (Object)(object)transform;
			bool flag = !Object.op_Implicit((Object)(object)m_HorizontalScrollbarRect) || (Object)(object)((Transform)m_HorizontalScrollbarRect).get_parent() == (Object)(object)transform;
			bool flag2 = !Object.op_Implicit((Object)(object)m_VerticalScrollbarRect) || (Object)(object)((Transform)m_VerticalScrollbarRect).get_parent() == (Object)(object)transform;
			bool flag3 = num && flag && flag2;
			m_HSliderExpand = flag3 && Object.op_Implicit((Object)(object)m_HorizontalScrollbarRect) && horizontalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
			m_VSliderExpand = flag3 && Object.op_Implicit((Object)(object)m_VerticalScrollbarRect) && verticalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
			Rect rect;
			float hSliderHeight;
			if (!((Object)(object)m_HorizontalScrollbarRect == (Object)null))
			{
				rect = m_HorizontalScrollbarRect.get_rect();
				hSliderHeight = ((Rect)(ref rect)).get_height();
			}
			else
			{
				hSliderHeight = 0f;
			}
			m_HSliderHeight = hSliderHeight;
			float vSliderWidth;
			if (!((Object)(object)m_VerticalScrollbarRect == (Object)null))
			{
				rect = m_VerticalScrollbarRect.get_rect();
				vSliderWidth = ((Rect)(ref rect)).get_width();
			}
			else
			{
				vSliderWidth = 0f;
			}
			m_VSliderWidth = vSliderWidth;
		}

		protected override void OnEnable()
		{
			((UIBehaviour)this).OnEnable();
			if (Object.op_Implicit((Object)(object)m_HorizontalScrollbar))
			{
				((UnityEvent<float>)(object)m_HorizontalScrollbar.get_onValueChanged()).AddListener((UnityAction<float>)SetHorizontalNormalizedPosition);
			}
			if (Object.op_Implicit((Object)(object)m_VerticalScrollbar))
			{
				((UnityEvent<float>)(object)m_VerticalScrollbar.get_onValueChanged()).AddListener((UnityAction<float>)SetVerticalNormalizedPosition);
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild((ICanvasElement)(object)this);
		}

		protected override void OnDisable()
		{
			if (!Application.isQuitting)
			{
				CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild((ICanvasElement)(object)this);
				if (Object.op_Implicit((Object)(object)m_HorizontalScrollbar))
				{
					((UnityEvent<float>)(object)m_HorizontalScrollbar.get_onValueChanged()).RemoveListener((UnityAction<float>)SetHorizontalNormalizedPosition);
				}
				if (Object.op_Implicit((Object)(object)m_VerticalScrollbar))
				{
					((UnityEvent<float>)(object)m_VerticalScrollbar.get_onValueChanged()).RemoveListener((UnityAction<float>)SetVerticalNormalizedPosition);
				}
				m_HasRebuiltLayout = false;
				((DrivenRectTransformTracker)(ref m_Tracker)).Clear();
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
				((UIBehaviour)this).OnDisable();
			}
		}

		public override bool IsActive()
		{
			if (((UIBehaviour)this).IsActive())
			{
				return (Object)(object)m_Content != (Object)null;
			}
			return false;
		}

		private void EnsureLayoutHasRebuilt()
		{
			if (!m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
			{
				Canvas.ForceUpdateCanvases();
			}
		}

		public virtual void StopMovement()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			m_Velocity = Vector2.get_zero();
		}

		public virtual void OnScroll(PointerEventData data)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			if (!((UIBehaviour)this).IsActive())
			{
				return;
			}
			EnsureLayoutHasRebuilt();
			UpdateBounds();
			Vector2 scrollDelta = data.get_scrollDelta();
			scrollDelta.y *= -1f;
			if (vertical && !horizontal)
			{
				if (Mathf.Abs(scrollDelta.x) > Mathf.Abs(scrollDelta.y))
				{
					scrollDelta.y = scrollDelta.x;
				}
				scrollDelta.x = 0f;
			}
			if (horizontal && !vertical)
			{
				if (Mathf.Abs(scrollDelta.y) > Mathf.Abs(scrollDelta.x))
				{
					scrollDelta.x = scrollDelta.y;
				}
				scrollDelta.y = 0f;
			}
			Vector2 anchoredPosition = m_Content.get_anchoredPosition();
			anchoredPosition += scrollDelta * m_ScrollSensitivity;
			if (m_MovementType == MovementType.Clamped)
			{
				anchoredPosition += CalculateOffset(anchoredPosition - m_Content.get_anchoredPosition());
			}
			SetContentAnchoredPosition(anchoredPosition);
			UpdateBounds();
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (eventData.get_button() == scrollButton || eventData.get_button() == altScrollButton)
			{
				m_Velocity = Vector2.get_zero();
			}
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			if ((eventData.get_button() == scrollButton || eventData.get_button() == altScrollButton) && ((UIBehaviour)this).IsActive())
			{
				UpdateBounds();
				m_PointerStartLocalCursor = Vector2.get_zero();
				RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.get_position(), eventData.get_pressEventCamera(), ref m_PointerStartLocalCursor);
				m_ContentStartPosition = m_Content.get_anchoredPosition();
				m_Dragging = true;
			}
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (eventData.get_button() == scrollButton || eventData.get_button() == altScrollButton)
			{
				m_Dragging = false;
			}
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = default(Vector2);
			if ((eventData.get_button() != scrollButton && eventData.get_button() != altScrollButton) || !((UIBehaviour)this).IsActive() || !RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.get_position(), eventData.get_pressEventCamera(), ref val))
			{
				return;
			}
			UpdateBounds();
			Vector2 val2 = val - m_PointerStartLocalCursor;
			Vector2 val3 = m_ContentStartPosition + val2;
			Vector2 val4 = CalculateOffset(val3 - m_Content.get_anchoredPosition());
			val3 += val4;
			if (m_MovementType == MovementType.Elastic)
			{
				if (val4.x != 0f)
				{
					val3.x -= RubberDelta(val4.x, ((Bounds)(ref m_ViewBounds)).get_size().x);
				}
				if (val4.y != 0f)
				{
					val3.y -= RubberDelta(val4.y, ((Bounds)(ref m_ViewBounds)).get_size().y);
				}
			}
			SetContentAnchoredPosition(val3);
		}

		protected virtual void SetContentAnchoredPosition(Vector2 position)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			if (!m_Horizontal)
			{
				position.x = m_Content.get_anchoredPosition().x;
			}
			if (!m_Vertical)
			{
				position.y = m_Content.get_anchoredPosition().y;
			}
			if (position != m_Content.get_anchoredPosition())
			{
				m_Content.set_anchoredPosition(position);
				UpdateBounds();
			}
		}

		protected virtual void LateUpdate()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			if (!Object.op_Implicit((Object)(object)m_Content))
			{
				return;
			}
			EnsureLayoutHasRebuilt();
			UpdateScrollbarVisibility();
			UpdateBounds();
			float unscaledDeltaTime = Time.get_unscaledDeltaTime();
			Vector2 val = CalculateOffset(Vector2.get_zero());
			if (!m_Dragging && (val != Vector2.get_zero() || m_Velocity != Vector2.get_zero()))
			{
				Vector2 val2 = m_Content.get_anchoredPosition();
				for (int i = 0; i < 2; i++)
				{
					if (m_MovementType == MovementType.Elastic && ((Vector2)(ref val)).get_Item(i) != 0f)
					{
						float num = ((Vector2)(ref m_Velocity)).get_Item(i);
						int num2 = i;
						Vector2 anchoredPosition = m_Content.get_anchoredPosition();
						float num3 = ((Vector2)(ref anchoredPosition)).get_Item(i);
						anchoredPosition = m_Content.get_anchoredPosition();
						((Vector2)(ref val2)).set_Item(num2, Mathf.SmoothDamp(num3, ((Vector2)(ref anchoredPosition)).get_Item(i) + ((Vector2)(ref val)).get_Item(i), ref num, m_Elasticity, float.PositiveInfinity, unscaledDeltaTime));
						((Vector2)(ref m_Velocity)).set_Item(i, num);
					}
					else if (m_Inertia)
					{
						ref Vector2 reference = ref m_Velocity;
						int num4 = i;
						((Vector2)(ref reference)).set_Item(num4, ((Vector2)(ref reference)).get_Item(num4) * Mathf.Pow(m_DecelerationRate, unscaledDeltaTime));
						if (Mathf.Abs(((Vector2)(ref m_Velocity)).get_Item(i)) < 1f)
						{
							((Vector2)(ref m_Velocity)).set_Item(i, 0f);
						}
						reference = ref val2;
						num4 = i;
						((Vector2)(ref reference)).set_Item(num4, ((Vector2)(ref reference)).get_Item(num4) + ((Vector2)(ref m_Velocity)).get_Item(i) * unscaledDeltaTime);
					}
					else
					{
						((Vector2)(ref m_Velocity)).set_Item(i, 0f);
					}
				}
				if (m_Velocity != Vector2.get_zero())
				{
					if (m_MovementType == MovementType.Clamped)
					{
						val = CalculateOffset(val2 - m_Content.get_anchoredPosition());
						val2 += val;
					}
					SetContentAnchoredPosition(val2);
				}
			}
			if (m_Dragging && m_Inertia)
			{
				Vector3 val3 = Vector2.op_Implicit((m_Content.get_anchoredPosition() - m_PrevPosition) / unscaledDeltaTime);
				m_Velocity = Vector2.op_Implicit(Vector3.Lerp(Vector2.op_Implicit(m_Velocity), val3, unscaledDeltaTime * 10f));
			}
			if (m_ViewBounds != m_PrevViewBounds || m_ContentBounds != m_PrevContentBounds || m_Content.get_anchoredPosition() != m_PrevPosition)
			{
				UpdateScrollbars(val);
				((UnityEvent<Vector2>)m_OnValueChanged).Invoke(normalizedPosition);
				UpdatePrevData();
			}
		}

		private void UpdatePrevData()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_Content == (Object)null)
			{
				m_PrevPosition = Vector2.get_zero();
			}
			else
			{
				m_PrevPosition = m_Content.get_anchoredPosition();
			}
			m_PrevViewBounds = m_ViewBounds;
			m_PrevContentBounds = m_ContentBounds;
		}

		private void UpdateScrollbars(Vector2 offset)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)m_HorizontalScrollbar))
			{
				if (((Bounds)(ref m_ContentBounds)).get_size().x > 0f)
				{
					m_HorizontalScrollbar.set_size(Mathf.Clamp01((((Bounds)(ref m_ViewBounds)).get_size().x - Mathf.Abs(offset.x)) / ((Bounds)(ref m_ContentBounds)).get_size().x));
				}
				else
				{
					m_HorizontalScrollbar.set_size(1f);
				}
				m_HorizontalScrollbar.set_value(horizontalNormalizedPosition);
			}
			if (Object.op_Implicit((Object)(object)m_VerticalScrollbar))
			{
				if (((Bounds)(ref m_ContentBounds)).get_size().y > 0f)
				{
					m_VerticalScrollbar.set_size(Mathf.Clamp01((((Bounds)(ref m_ViewBounds)).get_size().y - Mathf.Abs(offset.y)) / ((Bounds)(ref m_ContentBounds)).get_size().y));
				}
				else
				{
					m_VerticalScrollbar.set_size(1f);
				}
				m_VerticalScrollbar.set_value(verticalNormalizedPosition);
			}
		}

		private void SetHorizontalNormalizedPosition(float value)
		{
			SetNormalizedPosition(value, 0);
		}

		private void SetVerticalNormalizedPosition(float value)
		{
			SetNormalizedPosition(value, 1);
		}

		private void SetNormalizedPosition(float value, int axis)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			EnsureLayoutHasRebuilt();
			UpdateBounds();
			Vector3 val = ((Bounds)(ref m_ContentBounds)).get_size();
			float num = ((Vector3)(ref val)).get_Item(axis);
			val = ((Bounds)(ref m_ViewBounds)).get_size();
			float num2 = num - ((Vector3)(ref val)).get_Item(axis);
			val = ((Bounds)(ref m_ViewBounds)).get_min();
			float num3 = ((Vector3)(ref val)).get_Item(axis) - value * num2;
			val = ((Transform)m_Content).get_localPosition();
			float num4 = ((Vector3)(ref val)).get_Item(axis) + num3;
			val = ((Bounds)(ref m_ContentBounds)).get_min();
			float num5 = num4 - ((Vector3)(ref val)).get_Item(axis);
			Vector3 localPosition = ((Transform)m_Content).get_localPosition();
			if (Mathf.Abs(((Vector3)(ref localPosition)).get_Item(axis) - num5) > 0.01f)
			{
				((Vector3)(ref localPosition)).set_Item(axis, num5);
				((Transform)m_Content).set_localPosition(localPosition);
				((Vector2)(ref m_Velocity)).set_Item(axis, 0f);
				UpdateBounds();
			}
		}

		private static float RubberDelta(float overStretching, float viewSize)
		{
			return (1f - 1f / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1f)) * viewSize * Mathf.Sign(overStretching);
		}

		protected override void OnRectTransformDimensionsChange()
		{
			SetDirty();
		}

		public virtual void SetLayoutHorizontal()
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			((DrivenRectTransformTracker)(ref m_Tracker)).Clear();
			Rect rect;
			if (m_HSliderExpand || m_VSliderExpand)
			{
				((DrivenRectTransformTracker)(ref m_Tracker)).Add((Object)(object)this, viewRect, (DrivenTransformProperties)16134);
				viewRect.set_anchorMin(Vector2.get_zero());
				viewRect.set_anchorMax(Vector2.get_one());
				viewRect.set_sizeDelta(Vector2.get_zero());
				viewRect.set_anchoredPosition(Vector2.get_zero());
				LayoutRebuilder.ForceRebuildLayoutImmediate(content);
				rect = viewRect.get_rect();
				Vector3 val = Vector2.op_Implicit(((Rect)(ref rect)).get_center());
				rect = viewRect.get_rect();
				m_ViewBounds = new Bounds(val, Vector2.op_Implicit(((Rect)(ref rect)).get_size()));
				m_ContentBounds = GetBounds();
			}
			if (m_VSliderExpand && vScrollingNeeded)
			{
				viewRect.set_sizeDelta(new Vector2(0f - (m_VSliderWidth + m_VerticalScrollbarSpacing), viewRect.get_sizeDelta().y));
				LayoutRebuilder.ForceRebuildLayoutImmediate(content);
				rect = viewRect.get_rect();
				Vector3 val2 = Vector2.op_Implicit(((Rect)(ref rect)).get_center());
				rect = viewRect.get_rect();
				m_ViewBounds = new Bounds(val2, Vector2.op_Implicit(((Rect)(ref rect)).get_size()));
				m_ContentBounds = GetBounds();
			}
			if (m_HSliderExpand && hScrollingNeeded)
			{
				viewRect.set_sizeDelta(new Vector2(viewRect.get_sizeDelta().x, 0f - (m_HSliderHeight + m_HorizontalScrollbarSpacing)));
				rect = viewRect.get_rect();
				Vector3 val3 = Vector2.op_Implicit(((Rect)(ref rect)).get_center());
				rect = viewRect.get_rect();
				m_ViewBounds = new Bounds(val3, Vector2.op_Implicit(((Rect)(ref rect)).get_size()));
				m_ContentBounds = GetBounds();
			}
			if (m_VSliderExpand && vScrollingNeeded && viewRect.get_sizeDelta().x == 0f && viewRect.get_sizeDelta().y < 0f)
			{
				viewRect.set_sizeDelta(new Vector2(0f - (m_VSliderWidth + m_VerticalScrollbarSpacing), viewRect.get_sizeDelta().y));
			}
		}

		public virtual void SetLayoutVertical()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			UpdateScrollbarLayout();
			Rect rect = viewRect.get_rect();
			Vector3 val = Vector2.op_Implicit(((Rect)(ref rect)).get_center());
			rect = viewRect.get_rect();
			m_ViewBounds = new Bounds(val, Vector2.op_Implicit(((Rect)(ref rect)).get_size()));
			m_ContentBounds = GetBounds();
		}

		private void UpdateScrollbarVisibility()
		{
			if (Object.op_Implicit((Object)(object)m_VerticalScrollbar) && m_VerticalScrollbarVisibility != 0 && ((Component)m_VerticalScrollbar).get_gameObject().get_activeSelf() != vScrollingNeeded)
			{
				((Component)m_VerticalScrollbar).get_gameObject().SetActive(vScrollingNeeded);
			}
			if (Object.op_Implicit((Object)(object)m_HorizontalScrollbar) && m_HorizontalScrollbarVisibility != 0 && ((Component)m_HorizontalScrollbar).get_gameObject().get_activeSelf() != hScrollingNeeded)
			{
				((Component)m_HorizontalScrollbar).get_gameObject().SetActive(hScrollingNeeded);
			}
		}

		private void UpdateScrollbarLayout()
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			if (m_VSliderExpand && Object.op_Implicit((Object)(object)m_HorizontalScrollbar))
			{
				((DrivenRectTransformTracker)(ref m_Tracker)).Add((Object)(object)this, m_HorizontalScrollbarRect, (DrivenTransformProperties)5378);
				m_HorizontalScrollbarRect.set_anchorMin(new Vector2(0f, m_HorizontalScrollbarRect.get_anchorMin().y));
				m_HorizontalScrollbarRect.set_anchorMax(new Vector2(1f, m_HorizontalScrollbarRect.get_anchorMax().y));
				m_HorizontalScrollbarRect.set_anchoredPosition(new Vector2(0f, m_HorizontalScrollbarRect.get_anchoredPosition().y));
				if (vScrollingNeeded)
				{
					m_HorizontalScrollbarRect.set_sizeDelta(new Vector2(0f - (m_VSliderWidth + m_VerticalScrollbarSpacing), m_HorizontalScrollbarRect.get_sizeDelta().y));
				}
				else
				{
					m_HorizontalScrollbarRect.set_sizeDelta(new Vector2(0f, m_HorizontalScrollbarRect.get_sizeDelta().y));
				}
			}
			if (m_HSliderExpand && Object.op_Implicit((Object)(object)m_VerticalScrollbar))
			{
				((DrivenRectTransformTracker)(ref m_Tracker)).Add((Object)(object)this, m_VerticalScrollbarRect, (DrivenTransformProperties)10756);
				m_VerticalScrollbarRect.set_anchorMin(new Vector2(m_VerticalScrollbarRect.get_anchorMin().x, 0f));
				m_VerticalScrollbarRect.set_anchorMax(new Vector2(m_VerticalScrollbarRect.get_anchorMax().x, 1f));
				m_VerticalScrollbarRect.set_anchoredPosition(new Vector2(m_VerticalScrollbarRect.get_anchoredPosition().x, 0f));
				if (hScrollingNeeded)
				{
					m_VerticalScrollbarRect.set_sizeDelta(new Vector2(m_VerticalScrollbarRect.get_sizeDelta().x, 0f - (m_HSliderHeight + m_HorizontalScrollbarSpacing)));
				}
				else
				{
					m_VerticalScrollbarRect.set_sizeDelta(new Vector2(m_VerticalScrollbarRect.get_sizeDelta().x, 0f));
				}
			}
		}

		private void UpdateBounds()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			Rect rect = viewRect.get_rect();
			Vector3 val = Vector2.op_Implicit(((Rect)(ref rect)).get_center());
			rect = viewRect.get_rect();
			m_ViewBounds = new Bounds(val, Vector2.op_Implicit(((Rect)(ref rect)).get_size()));
			m_ContentBounds = GetBounds();
			if (!((Object)(object)m_Content == (Object)null))
			{
				Vector3 size = ((Bounds)(ref m_ContentBounds)).get_size();
				Vector3 center = ((Bounds)(ref m_ContentBounds)).get_center();
				Vector3 val2 = ((Bounds)(ref m_ViewBounds)).get_size() - size;
				if (val2.x > 0f)
				{
					center.x -= val2.x * (m_Content.get_pivot().x - 0.5f);
					size.x = ((Bounds)(ref m_ViewBounds)).get_size().x;
				}
				if (val2.y > 0f)
				{
					center.y -= val2.y * (m_Content.get_pivot().y - 0.5f);
					size.y = ((Bounds)(ref m_ViewBounds)).get_size().y;
				}
				((Bounds)(ref m_ContentBounds)).set_size(size);
				((Bounds)(ref m_ContentBounds)).set_center(center);
			}
		}

		private Bounds GetBounds()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_Content == (Object)null)
			{
				return default(Bounds);
			}
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(float.MinValue, float.MinValue, float.MinValue);
			Matrix4x4 worldToLocalMatrix = ((Transform)viewRect).get_worldToLocalMatrix();
			m_Content.GetWorldCorners(m_Corners);
			for (int i = 0; i < 4; i++)
			{
				Vector3 val3 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint3x4(m_Corners[i]);
				val = Vector3.Min(val3, val);
				val2 = Vector3.Max(val3, val2);
			}
			Bounds result = default(Bounds);
			((Bounds)(ref result))._002Ector(val, Vector3.get_zero());
			((Bounds)(ref result)).Encapsulate(val2);
			return result;
		}

		private Vector2 CalculateOffset(Vector2 delta)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			Vector2 zero = Vector2.get_zero();
			if (m_MovementType == MovementType.Unrestricted)
			{
				return zero;
			}
			Vector2 val = Vector2.op_Implicit(((Bounds)(ref m_ContentBounds)).get_min());
			Vector2 val2 = Vector2.op_Implicit(((Bounds)(ref m_ContentBounds)).get_max());
			if (m_Horizontal)
			{
				val.x += delta.x;
				val2.x += delta.x;
				if (val.x > ((Bounds)(ref m_ViewBounds)).get_min().x)
				{
					zero.x = ((Bounds)(ref m_ViewBounds)).get_min().x - val.x;
				}
				else if (val2.x < ((Bounds)(ref m_ViewBounds)).get_max().x)
				{
					zero.x = ((Bounds)(ref m_ViewBounds)).get_max().x - val2.x;
				}
			}
			if (m_Vertical)
			{
				val.y += delta.y;
				val2.y += delta.y;
				if (val2.y < ((Bounds)(ref m_ViewBounds)).get_max().y)
				{
					zero.y = ((Bounds)(ref m_ViewBounds)).get_max().y - val2.y;
				}
				else if (val.y > ((Bounds)(ref m_ViewBounds)).get_min().y)
				{
					zero.y = ((Bounds)(ref m_ViewBounds)).get_min().y - val.y;
				}
			}
			return zero;
		}

		protected void SetDirty()
		{
			if (((UIBehaviour)this).IsActive())
			{
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			}
		}

		protected void SetDirtyCaching()
		{
			if (((UIBehaviour)this).IsActive())
			{
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild((ICanvasElement)(object)this);
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			}
		}

		public void CenterOnPosition(Vector2 pos)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = ((Component)this).get_transform();
			RectTransform val = (RectTransform)(object)((transform is RectTransform) ? transform : null);
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))._002Ector(((Transform)content).get_localScale().x, ((Transform)content).get_localScale().y);
			pos.x *= val2.x;
			pos.y *= val2.y;
			Rect rect = content.get_rect();
			float num = ((Rect)(ref rect)).get_width() * val2.x;
			rect = val.get_rect();
			float num2 = num - ((Rect)(ref rect)).get_width();
			rect = content.get_rect();
			float num3 = ((Rect)(ref rect)).get_height() * val2.y;
			rect = val.get_rect();
			Vector2 val3 = default(Vector2);
			((Vector2)(ref val3))._002Ector(num2, num3 - ((Rect)(ref rect)).get_height());
			pos.x = pos.x / val3.x + content.get_pivot().x;
			pos.y = pos.y / val3.y + content.get_pivot().y;
			if (movementType != 0)
			{
				pos.x = Mathf.Clamp(pos.x, 0f, 1f);
				pos.y = Mathf.Clamp(pos.y, 0f, 1f);
			}
			normalizedPosition = pos;
		}

		public void LayoutComplete()
		{
		}

		public void GraphicUpdateComplete()
		{
		}

		[SpecialName]
		Transform ICanvasElement.get_transform()
		{
			return ((Component)this).get_transform();
		}
	}
}
