using UnityEngine;

public class CommentComponent : MonoBehaviour, IEditorComponent
{
	[TextArea]
	public string comment;

	public CommentComponent()
		: this()
	{
	}
}
