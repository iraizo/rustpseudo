using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	public class UIPrimitiveBase : MaskableGraphic, ILayoutElement, ICanvasRaycastFilter
	{
		protected static Material s_ETC1DefaultUI;

		private List<Vector2> outputList = new List<Vector2>();

		[SerializeField]
		private Sprite m_Sprite;

		[NonSerialized]
		private Sprite m_OverrideSprite;

		internal float m_EventAlphaThreshold = 1f;

		[SerializeField]
		private ResolutionMode m_improveResolution;

		[SerializeField]
		protected float m_Resolution;

		[SerializeField]
		private bool m_useNativeSize;

		public Sprite sprite
		{
			get
			{
				return m_Sprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass(ref m_Sprite, value))
				{
					GeneratedUVs();
				}
				((Graphic)this).SetAllDirty();
			}
		}

		public Sprite overrideSprite
		{
			get
			{
				return activeSprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass(ref m_OverrideSprite, value))
				{
					GeneratedUVs();
				}
				((Graphic)this).SetAllDirty();
			}
		}

		protected Sprite activeSprite
		{
			get
			{
				if (!((Object)(object)m_OverrideSprite != (Object)null))
				{
					return sprite;
				}
				return m_OverrideSprite;
			}
		}

		public float eventAlphaThreshold
		{
			get
			{
				return m_EventAlphaThreshold;
			}
			set
			{
				m_EventAlphaThreshold = value;
			}
		}

		public ResolutionMode ImproveResolution
		{
			get
			{
				return m_improveResolution;
			}
			set
			{
				m_improveResolution = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public float Resoloution
		{
			get
			{
				return m_Resolution;
			}
			set
			{
				m_Resolution = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public bool UseNativeSize
		{
			get
			{
				return m_useNativeSize;
			}
			set
			{
				m_useNativeSize = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public static Material defaultETC1GraphicMaterial
		{
			get
			{
				if ((Object)(object)s_ETC1DefaultUI == (Object)null)
				{
					s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
				}
				return s_ETC1DefaultUI;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if ((Object)(object)activeSprite == (Object)null)
				{
					if ((Object)(object)((Graphic)this).get_material() != (Object)null && (Object)(object)((Graphic)this).get_material().get_mainTexture() != (Object)null)
					{
						return ((Graphic)this).get_material().get_mainTexture();
					}
					return (Texture)(object)Graphic.s_WhiteTexture;
				}
				return (Texture)(object)activeSprite.get_texture();
			}
		}

		public bool hasBorder
		{
			get
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)activeSprite != (Object)null)
				{
					Vector4 border = activeSprite.get_border();
					return ((Vector4)(ref border)).get_sqrMagnitude() > 0f;
				}
				return false;
			}
		}

		public float pixelsPerUnit
		{
			get
			{
				float num = 100f;
				if (Object.op_Implicit((Object)(object)activeSprite))
				{
					num = activeSprite.get_pixelsPerUnit();
				}
				float num2 = 100f;
				if (Object.op_Implicit((Object)(object)((Graphic)this).get_canvas()))
				{
					num2 = ((Graphic)this).get_canvas().get_referencePixelsPerUnit();
				}
				return num / num2;
			}
		}

		public override Material material
		{
			get
			{
				if ((Object)(object)((Graphic)this).m_Material != (Object)null)
				{
					return ((Graphic)this).m_Material;
				}
				if (Object.op_Implicit((Object)(object)activeSprite) && (Object)(object)activeSprite.get_associatedAlphaSplitTexture() != (Object)null)
				{
					return defaultETC1GraphicMaterial;
				}
				return ((Graphic)this).get_defaultMaterial();
			}
			set
			{
				((Graphic)this).set_material(value);
			}
		}

		public virtual float minWidth => 0f;

		public virtual float preferredWidth
		{
			get
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)overrideSprite == (Object)null)
				{
					return 0f;
				}
				Rect rect = overrideSprite.get_rect();
				return ((Rect)(ref rect)).get_size().x / pixelsPerUnit;
			}
		}

		public virtual float flexibleWidth => -1f;

		public virtual float minHeight => 0f;

		public virtual float preferredHeight
		{
			get
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)overrideSprite == (Object)null)
				{
					return 0f;
				}
				Rect rect = overrideSprite.get_rect();
				return ((Rect)(ref rect)).get_size().y / pixelsPerUnit;
			}
		}

		public virtual float flexibleHeight => -1f;

		public virtual int layoutPriority => 0;

		protected UIPrimitiveBase()
			: this()
		{
			((Graphic)this).set_useLegacyMeshGeneration(false);
		}

		protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			UIVertex[] array = (UIVertex[])(object)new UIVertex[4];
			for (int i = 0; i < vertices.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = Color32.op_Implicit(((Graphic)this).get_color());
				simpleVert.position = Vector2.op_Implicit(vertices[i]);
				simpleVert.uv0 = uvs[i];
				array[i] = simpleVert;
			}
			return array;
		}

		protected Vector2[] IncreaseResolution(Vector2[] input)
		{
			return IncreaseResolution(new List<Vector2>(input)).ToArray();
		}

		protected List<Vector2> IncreaseResolution(List<Vector2> input)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			outputList.Clear();
			switch (ImproveResolution)
			{
			case ResolutionMode.PerLine:
			{
				float num3 = 0f;
				float num = 0f;
				for (int j = 0; j < input.Count - 1; j++)
				{
					num3 += Vector2.Distance(input[j], input[j + 1]);
				}
				ResolutionToNativeSize(num3);
				num = num3 / m_Resolution;
				int num4 = 0;
				for (int k = 0; k < input.Count - 1; k++)
				{
					Vector2 val3 = input[k];
					outputList.Add(val3);
					Vector2 val4 = input[k + 1];
					float num5 = Vector2.Distance(val3, val4) / num;
					float num6 = 1f / num5;
					for (int l = 0; (float)l < num5; l++)
					{
						outputList.Add(Vector2.Lerp(val3, val4, (float)l * num6));
						num4++;
					}
					outputList.Add(val4);
				}
				break;
			}
			case ResolutionMode.PerSegment:
			{
				for (int i = 0; i < input.Count - 1; i++)
				{
					Vector2 val = input[i];
					outputList.Add(val);
					Vector2 val2 = input[i + 1];
					ResolutionToNativeSize(Vector2.Distance(val, val2));
					float num = 1f / m_Resolution;
					for (float num2 = 1f; num2 < m_Resolution; num2 += 1f)
					{
						outputList.Add(Vector2.Lerp(val, val2, num * num2));
					}
					outputList.Add(val2);
				}
				break;
			}
			}
			return outputList;
		}

		protected virtual void GeneratedUVs()
		{
		}

		protected virtual void ResolutionToNativeSize(float distance)
		{
		}

		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		public virtual void CalculateLayoutInputVertical()
		{
		}

		public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Expected O, but got Unknown
			if (m_EventAlphaThreshold >= 1f)
			{
				return true;
			}
			Sprite val = overrideSprite;
			if ((Object)(object)val == (Object)null)
			{
				return true;
			}
			Vector2 val2 = default(Vector2);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(((Graphic)this).get_rectTransform(), screenPoint, eventCamera, ref val2);
			Rect pixelAdjustedRect = ((Graphic)this).GetPixelAdjustedRect();
			val2.x += ((Graphic)this).get_rectTransform().get_pivot().x * ((Rect)(ref pixelAdjustedRect)).get_width();
			val2.y += ((Graphic)this).get_rectTransform().get_pivot().y * ((Rect)(ref pixelAdjustedRect)).get_height();
			val2 = MapCoordinate(val2, pixelAdjustedRect);
			Rect textureRect = val.get_textureRect();
			Vector2 val3 = default(Vector2);
			((Vector2)(ref val3))._002Ector(val2.x / ((Rect)(ref textureRect)).get_width(), val2.y / ((Rect)(ref textureRect)).get_height());
			float num = Mathf.Lerp(((Rect)(ref textureRect)).get_x(), ((Rect)(ref textureRect)).get_xMax(), val3.x) / (float)((Texture)val.get_texture()).get_width();
			float num2 = Mathf.Lerp(((Rect)(ref textureRect)).get_y(), ((Rect)(ref textureRect)).get_yMax(), val3.y) / (float)((Texture)val.get_texture()).get_height();
			try
			{
				return val.get_texture().GetPixelBilinear(num, num2).a >= m_EventAlphaThreshold;
			}
			catch (UnityException val4)
			{
				UnityException val5 = val4;
				Debug.LogError((object)("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + ((Exception)(object)val5).Message + " Also make sure to disable sprite packing for this sprite."), (Object)(object)this);
				return true;
			}
		}

		private Vector2 MapCoordinate(Vector2 local, Rect rect)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			sprite.get_rect();
			return new Vector2(local.x * ((Rect)(ref rect)).get_width(), local.y * ((Rect)(ref rect)).get_height());
		}

		private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i <= 1; i++)
			{
				float num = ((Vector4)(ref border)).get_Item(i) + ((Vector4)(ref border)).get_Item(i + 2);
				Vector2 size = ((Rect)(ref rect)).get_size();
				if (((Vector2)(ref size)).get_Item(i) < num && num != 0f)
				{
					size = ((Rect)(ref rect)).get_size();
					float num2 = ((Vector2)(ref size)).get_Item(i) / num;
					ref Vector4 reference = ref border;
					int num3 = i;
					((Vector4)(ref reference)).set_Item(num3, ((Vector4)(ref reference)).get_Item(num3) * num2);
					reference = ref border;
					num3 = i + 2;
					((Vector4)(ref reference)).set_Item(num3, ((Vector4)(ref reference)).get_Item(num3) * num2);
				}
			}
			return border;
		}

		protected override void OnEnable()
		{
			((MaskableGraphic)this).OnEnable();
			((Graphic)this).SetAllDirty();
		}
	}
}
