public class UIDialog : ListComponent<UIDialog>
{
	public SoundDefinition openSoundDef;

	public SoundDefinition closeSoundDef;

	public static bool isOpen => ListComponent<UIDialog>.InstanceList.get_Count() > 0;
}
