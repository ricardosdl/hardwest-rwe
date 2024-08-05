using UnityEngine;

public class CFGWorldDetailsLevel : MonoBehaviour
{
	public EWorldDetailsLevel m_MinLevel = EWorldDetailsLevel.Low;

	private void Start()
	{
		base.gameObject.SetActive((int)m_MinLevel >= CFGOptions.Graphics.WorldDetails);
	}
}
