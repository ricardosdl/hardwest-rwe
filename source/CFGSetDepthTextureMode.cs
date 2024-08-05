using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CFGSetDepthTextureMode : MonoBehaviour
{
	public enum DepthModes
	{
		None,
		Depth,
		DepthNormals
	}

	public DepthModes CameraDepthMode;

	private DepthTextureMode pickedDepthMode;

	private void OnEnable()
	{
		switch (CameraDepthMode)
		{
		case DepthModes.None:
			pickedDepthMode = DepthTextureMode.None;
			break;
		case DepthModes.Depth:
			pickedDepthMode = DepthTextureMode.Depth;
			break;
		case DepthModes.DepthNormals:
			pickedDepthMode = DepthTextureMode.DepthNormals;
			break;
		}
		SetDepthMode();
	}

	private void SetDepthMode()
	{
		Camera component = GetComponent<Camera>();
		component.depthTextureMode = pickedDepthMode;
	}
}
