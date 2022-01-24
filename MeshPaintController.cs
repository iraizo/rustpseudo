using UnityEngine;
using UnityEngine.UI;

public class MeshPaintController : MonoBehaviour, IClientComponent
{
	public Camera pickerCamera;

	public Texture2D brushTexture;

	public Vector2 brushScale = new Vector2(8f, 8f);

	public Color brushColor = Color.get_white();

	public float brushSpacing = 2f;

	public RawImage brushImage;

	public float brushPreviewScaleMultiplier = 1f;

	public bool applyDefaults;

	public Texture2D defaltBrushTexture;

	public float defaultBrushSize = 16f;

	public Color defaultBrushColor = Color.get_black();

	public float defaultBrushAlpha = 0.5f;

	public Toggle lastBrush;

	public Button UndoButton;

	public Button RedoButton;

	private Vector3 lastPosition;

	public MeshPaintController()
		: this()
	{
	}//IL_000b: Unknown result type (might be due to invalid IL or missing references)
	//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0016: Unknown result type (might be due to invalid IL or missing references)
	//IL_001b: Unknown result type (might be due to invalid IL or missing references)
	//IL_0042: Unknown result type (might be due to invalid IL or missing references)
	//IL_0047: Unknown result type (might be due to invalid IL or missing references)

}
