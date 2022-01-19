using UnityEngine;

public class ConversationManager : MonoBehaviour
{
	public class Conversation : MonoBehaviour
	{
		public ConversationData data;

		public int currentSpeechNodeIndex;

		public IConversationProvider provider;

		public int GetSpeechNodeIndex(string name)
		{
			if ((Object)(object)data == (Object)null)
			{
				return -1;
			}
			return data.GetSpeechNodeIndex(name);
		}

		public Conversation()
			: this()
		{
		}
	}

	public ConversationManager()
		: this()
	{
	}
}
