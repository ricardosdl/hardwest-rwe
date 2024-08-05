using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CFGCharacterTrigger : MonoBehaviour
{
	public delegate void OnTriggerDelegate(CFGCharacterTrigger trigger, CFGCharacter character);

	[CFGFlowCode(Category = "Trigger", Title = "On Character Trigger Enter")]
	public OnTriggerDelegate m_OnTriggerEnterCallback;

	[CFGFlowCode(Category = "Trigger", Title = "On Character Trigger Exit")]
	public OnTriggerDelegate m_OnTriggerExitCallback;

	private void Start()
	{
		if (!GetComponent<Collider>() || !GetComponent<Collider>().isTrigger)
		{
			Debug.Log("Something wrong with collider!", this);
		}
	}

	private void OnTriggerEnter(Collider trigger)
	{
		CFGCharacter component = trigger.GetComponent<CFGCharacter>();
		if (component != null && m_OnTriggerEnterCallback != null)
		{
			m_OnTriggerEnterCallback(this, component);
		}
	}

	private void OnTriggerExit(Collider trigger)
	{
		CFGCharacter component = trigger.GetComponent<CFGCharacter>();
		if (component != null && m_OnTriggerExitCallback != null)
		{
			m_OnTriggerExitCallback(this, component);
		}
	}
}
