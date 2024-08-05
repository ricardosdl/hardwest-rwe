public class CFGPlayerOwner : CFGOwner
{
	public delegate void OnReceivedItemDelegate(string ItemID, int Count);

	public delegate void OnActionActivated(ETurnAction action);

	public delegate void OnConfirmClicked(ETurnAction action);

	[CFGFlowCode(Category = "Owner", Title = "On Player Received Item")]
	public OnReceivedItemDelegate m_OnReceivedItemCallback;

	[CFGFlowCode(Category = "Owner", Title = "On Action Activated")]
	public OnActionActivated m_OnActionActivatedCallback;

	[CFGFlowCode(Category = "Owner", Title = "On Confirm Clicked")]
	public OnConfirmClicked m_OnConfirmClicked;

	public override bool IsPlayer => true;

	protected override void OnEnable()
	{
		base.OnEnable();
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.RegisterPlayerOwner(this);
	}
}
