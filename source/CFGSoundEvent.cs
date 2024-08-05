using UnityEngine;

public class CFGSoundEvent : MonoBehaviour
{
	public void OnPlaySound(GameObject go)
	{
		if (go != null)
		{
			CFGSoundDef.Play(go.GetComponent<CFGSoundDef>(), base.transform);
		}
	}
}
