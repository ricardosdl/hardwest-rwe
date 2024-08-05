using UnityEngine;

public class CFGIndicatorCamera : MonoBehaviour
{
	public static bool s_IndicatorsEnabled = true;

	private void OnPostRender()
	{
		if (s_IndicatorsEnabled)
		{
			CFGPointSprite.Render();
		}
	}
}
