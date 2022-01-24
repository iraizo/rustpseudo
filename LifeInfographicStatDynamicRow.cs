using Rust.UI;

public class LifeInfographicStatDynamicRow : LifeInfographicStat
{
	public RustText StatName;

	public void SetStatName(Phrase phrase)
	{
		StatName.SetPhrase(phrase);
	}
}
