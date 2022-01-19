using UnityEngine;

[ExecuteInEditMode]
public class WaterInteraction : MonoBehaviour
{
	[SerializeField]
	private Texture2D texture;

	[Range(0f, 1f)]
	public float Displacement = 1f;

	[Range(0f, 1f)]
	public float Disturbance = 0.5f;

	private Transform cachedTransform;

	public Texture2D Texture
	{
		get
		{
			return texture;
		}
		set
		{
			texture = value;
			CheckRegister();
		}
	}

	public WaterDynamics.Image Image { get; private set; }

	public Vector2 Position { get; private set; } = Vector2.get_zero();


	public Vector2 Scale { get; private set; } = Vector2.get_one();


	public float Rotation { get; private set; }

	protected void OnEnable()
	{
		CheckRegister();
		UpdateTransform();
	}

	protected void OnDisable()
	{
		Unregister();
	}

	public void CheckRegister()
	{
		if (!((Behaviour)this).get_enabled() || (Object)(object)texture == (Object)null)
		{
			Unregister();
		}
		else if (Image == null || (Object)(object)Image.texture != (Object)(object)texture)
		{
			Register();
		}
	}

	private void UpdateImage()
	{
		Image = new WaterDynamics.Image(texture);
	}

	private void Register()
	{
		UpdateImage();
		WaterDynamics.RegisterInteraction(this);
	}

	private void Unregister()
	{
		if (Image != null)
		{
			WaterDynamics.UnregisterInteraction(this);
			Image = null;
		}
	}

	public void UpdateTransform()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		cachedTransform = (((Object)(object)cachedTransform != (Object)null) ? cachedTransform : ((Component)this).get_transform());
		if (cachedTransform.get_hasChanged())
		{
			Vector3 position = cachedTransform.get_position();
			Vector3 lossyScale = cachedTransform.get_lossyScale();
			Position = new Vector2(position.x, position.z);
			Scale = new Vector2(lossyScale.x, lossyScale.z);
			Quaternion rotation = cachedTransform.get_rotation();
			Rotation = ((Quaternion)(ref rotation)).get_eulerAngles().y;
			cachedTransform.set_hasChanged(false);
		}
	}

	public WaterInteraction()
		: this()
	{
	}//IL_0017: Unknown result type (might be due to invalid IL or missing references)
	//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0022: Unknown result type (might be due to invalid IL or missing references)
	//IL_0027: Unknown result type (might be due to invalid IL or missing references)

}
