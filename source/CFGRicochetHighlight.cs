using UnityEngine;

[ExecuteInEditMode]
public class CFGRicochetHighlight : MonoBehaviour
{
	public bool enableHighlight = true;

	private void OnEnable()
	{
		if (enableHighlight)
		{
			Shader.SetGlobalFloat("_HighlightPow", 1f);
		}
		else
		{
			Shader.SetGlobalFloat("_HighlightPow", 0f);
		}
	}
}
