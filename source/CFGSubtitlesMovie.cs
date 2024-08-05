public class CFGSubtitlesMovie : CFGPanel
{
	public CFGTextExtension m_SubtitlesText;

	protected override void Start()
	{
		base.Start();
		CheckField(m_SubtitlesText, "SubtitlesText");
	}

	public void SetText(string text)
	{
		if ((bool)m_SubtitlesText)
		{
			m_SubtitlesText.text = text;
		}
	}
}
