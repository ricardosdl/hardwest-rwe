using System.Runtime.InteropServices;
using UnityEngine;

public interface CFGIAttackable
{
	CFGCell CurrentCell { get; }

	bool IsInShadow { get; }

	bool IsCorpseLooted { get; }

	bool IsDodging { get; }

	int BuffedDefense { get; }

	int Hp { get; }

	int MaxHp { get; }

	bool IsInvulnerable { get; set; }

	bool IsAlive { get; }

	string NameId { get; }

	int Luck { get; set; }

	int MaxLuck { get; }

	Vector3 Position { get; }

	Quaternion Rotation { get; }

	bool TakeDamage(int dmg, CFGCharacter damage_giver, bool bSilent, [Optional] Vector3 recoilDir);

	ECoverType GetCoverState();

	int GetCoverMult();

	int GetAimBonus();

	Transform GetDamagePivot();

	CFGOwner GetOwner();

	void ApplyBuffAction(CFGDef_UsableItem.eActionType Action, string Param, CFGCharacter caster);
}
