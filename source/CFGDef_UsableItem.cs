public class CFGDef_UsableItem
{
	public enum eActionType
	{
		None,
		AddBuff,
		RemoveBuff,
		RemoveAllBuffs,
		HP_Mod,
		PAdd_MaxHealth,
		PAdd_Aim,
		PAdd_Defense,
		PAdd_Movement,
		PAdd_Sight,
		PAdd_MaxLuck,
		PAdd_Luck,
		PSet_MaxHealth,
		PSet_Aim,
		PSet_Defense,
		PSet_Movement,
		PSet_Sight,
		PSet_MaxLuck,
		PSet_Luck
	}

	[CFGTableField(ColumnName = "ItemID", DefaultValue = "")]
	public string ItemID = string.Empty;

	public int UseLimit = 1;

	[CFGTableField(ColumnName = "Cooldown", DefaultValue = -1)]
	public int Cooldown = -1;

	[CFGTableField(ColumnName = "SelectionTarget", DefaultValue = "")]
	public string SelectionTarget = string.Empty;

	[CFGTableField(ColumnName = "AOE_Type", DefaultValue = eAOE_Type.None)]
	public eAOE_Type AOE_Type;

	[CFGTableField(ColumnName = "Radius", DefaultValue = 0f)]
	public float Radius;

	[CFGTableField(ColumnName = "Range", DefaultValue = 50f)]
	public float Range = 50f;

	[CFGTableField(ColumnName = "UseLOS", DefaultValue = true)]
	public bool UseLOS = true;

	[CFGTableField(ColumnName = "Replenish", DefaultValue = true)]
	public bool Replenish = true;

	[CFGTableField(ColumnName = "causes_combat", DefaultValue = false)]
	public bool CausesCombat;

	[CFGTableField(ColumnName = "self_action", DefaultValue = eActionType.None)]
	public eActionType Self_Action;

	[CFGTableField(ColumnName = "friend_action", DefaultValue = eActionType.None)]
	public eActionType Friend_Action;

	[CFGTableField(ColumnName = "enemy_action", DefaultValue = eActionType.None)]
	public eActionType Enemy_Action;

	[CFGTableField(ColumnName = "other_action", DefaultValue = eActionType.None)]
	public eActionType Other_Action;

	[CFGTableField(ColumnName = "self_id", DefaultValue = "")]
	public string Self_Buff = string.Empty;

	[CFGTableField(ColumnName = "enemy_id", DefaultValue = "")]
	public string Enemy_Buff = string.Empty;

	[CFGTableField(ColumnName = "friend_id", DefaultValue = "")]
	public string Friend_Buff = string.Empty;

	[CFGTableField(ColumnName = "other_id", DefaultValue = "")]
	public string Other_Buff = string.Empty;

	[CFGTableField(ColumnName = "icon_hud", DefaultValue = 0)]
	public int Icon_Hud;

	[CFGTableField(ColumnName = "EndDelay", DefaultValue = 0f)]
	public float Delay;

	[CFGTableField(ColumnName = "Animation", DefaultValue = eAbilityAnimation.None)]
	public eAbilityAnimation Animation;

	[CFGTableField(ColumnName = "GrenadeName", DefaultValue = "")]
	public string GrenadeName = string.Empty;

	[CFGTableField(ColumnName = "CircleHelper", DefaultValue = eAOECircleHelper.Default)]
	public eAOECircleHelper AOECircleHelper;

	public eTargetableType SelectableTargets;

	public void FinalizeImport()
	{
		SelectableTargets = eTargetableType.None;
		if (SelectionTarget.Contains("e") || SelectionTarget.Contains("E"))
		{
			SelectableTargets |= eTargetableType.Enemy;
		}
		if (SelectionTarget.Contains("s") || SelectionTarget.Contains("S"))
		{
			SelectableTargets |= eTargetableType.Self;
		}
		if (SelectionTarget.Contains("f") || SelectionTarget.Contains("F"))
		{
			SelectableTargets |= eTargetableType.Friendly;
		}
		if (SelectionTarget.Contains("c") || SelectionTarget.Contains("C"))
		{
			SelectableTargets |= eTargetableType.Cell;
		}
		if (SelectionTarget.Contains("o") || SelectionTarget.Contains("O"))
		{
			SelectableTargets |= eTargetableType.Other;
		}
	}

	public bool ApplyOnTarget(CFGCharacter Self, CFGIAttackable Target)
	{
		if (Self == null || Target == null)
		{
			return false;
		}
		if (Target == Self && Self_Action != 0)
		{
			Self.ApplyBuffAction(Self_Action, Self_Buff, Self);
			return true;
		}
		CFGOwner owner = Self.Owner;
		CFGOwner owner2 = Target.GetOwner();
		if (Enemy_Action != 0 && owner != owner2 && (bool)owner && (bool)owner2)
		{
			Target.ApplyBuffAction(Enemy_Action, Enemy_Buff, Self);
			return true;
		}
		if (Friend_Action != 0 && owner == owner2 && (bool)owner && (bool)owner2)
		{
			Target.ApplyBuffAction(Friend_Action, Friend_Buff, Self);
			return true;
		}
		if (Other_Action != 0 && owner2 == null)
		{
			Target.ApplyBuffAction(Other_Action, Other_Buff, Self);
			return true;
		}
		return false;
	}

	public string GetActionForTarget(CFGIAttackable Target, CFGCharacter Self)
	{
		if (Target == null)
		{
			return eActionType.None.ToString();
		}
		CFGCharacter cFGCharacter = Target as CFGCharacter;
		if ((bool)cFGCharacter)
		{
			if (cFGCharacter == Self)
			{
				return "self " + Self_Action;
			}
			if (cFGCharacter.Owner == null)
			{
				return "other " + Other_Action;
			}
			if (cFGCharacter.Owner.IsPlayer)
			{
				return "friend " + Friend_Action;
			}
			return "enemy " + Enemy_Action;
		}
		return "other " + Other_Action;
	}
}
