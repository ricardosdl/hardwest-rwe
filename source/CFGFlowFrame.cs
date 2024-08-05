using UnityEngine;

public class CFGFlowFrame : CFGFlowObject
{
	public string m_Comment = "Comment";

	public Vector2 m_Size = new Vector2(768f, 512f);

	public int m_BorderWidth = 15;

	public bool m_DrawBox = true;

	public bool m_Filled;

	public Color m_BoarderColor = Color.black;

	public Color m_FillColor = new Color(1f, 1f, 1f, 0.0627451f);

	public override string GetDisplayName()
	{
		return m_Comment;
	}
}
