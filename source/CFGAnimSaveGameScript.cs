using UnityEngine.EventSystems;

public class CFGAnimSaveGameScript : UIBehaviour
{
	public void OnAnimEnd()
	{
		base.transform.parent.gameObject.SetActive(value: false);
	}
}
