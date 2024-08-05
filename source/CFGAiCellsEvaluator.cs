using System.Collections.Generic;
using UnityEngine;

public class CFGAiCellsEvaluator
{
	private static CFGCharacter s_Character;

	private static CFGAiPreset s_AiPreset;

	public static CFGCell EvaluateCellsForCombat(HashSet<CFGCell> cells, CFGCharacter character)
	{
		s_Character = character;
		s_AiPreset = (character.Owner as CFGAiOwner).GetAiTeamPreset(character.AiTeam);
		if (s_AiPreset == null)
		{
			return null;
		}
		CFGCell cFGCell = null;
		foreach (CFGCell cell in cells)
		{
			if ((cell == character.CurrentCell || (cell.CanStandOnThisTile(can_stand_now: true) && cell.StairsType != CFGCell.EStairsType.Slope)) && EvaluateCellForCombat(cell) && (cFGCell == null || cell.m_AiValue > cFGCell.m_AiValue))
			{
				cFGCell = cell;
			}
		}
		return cFGCell;
	}

	private static bool EvaluateCellForCombat(CFGCell cell)
	{
		float num = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_RectionShootInfo.m_Range + 1f;
		CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
		foreach (CFGCharacter character in playerOwner.Characters)
		{
			if ((float)CFGCellMap.Distance(cell, character.CurrentCell) <= num)
			{
				return false;
			}
		}
		cell.m_AiValue = 0;
		if (s_Character.VisibleEnemies.Count == 0)
		{
			if (s_AiPreset.m_MovementMode != CFGAiPreset.EMovementMode.Hiding)
			{
				int a = 0;
				a = Mathf.Max(a, Mathf.Abs(cell.GetBorderCover(EDirection.NORTH).GetAimMod()));
				a = Mathf.Max(a, Mathf.Abs(cell.GetBorderCover(EDirection.EAST).GetAimMod()));
				a = Mathf.Max(a, Mathf.Abs(cell.GetBorderCover(EDirection.SOUTH).GetAimMod()));
				a = Mathf.Max(a, Mathf.Abs(cell.GetBorderCover(EDirection.WEST).GetAimMod()));
				cell.m_AiValue = a;
			}
			return true;
		}
		switch (s_AiPreset.m_MovementMode)
		{
		case CFGAiPreset.EMovementMode.Defensive:
		{
			bool flag2 = false;
			int num11 = 0;
			foreach (CFGCharacter visibleEnemy in s_Character.VisibleEnemies)
			{
				if (!flag2 && CFGCellMap.GetLineOfSightAutoSideSteps(s_Character, visibleEnemy, cell, visibleEnemy.CurrentCell, s_Character.BuffedSight) == ELOXHitType.None)
				{
					flag2 = true;
				}
				int chanceToHit4 = visibleEnemy.GetChanceToHit(s_Character, null, null, cell, ETurnAction.Shoot);
				int num12 = visibleEnemy.CalcDamage(s_Character, ETurnAction.Shoot, null, bIgnoreCovers: false, cell);
				num11 += chanceToHit4 * num12;
			}
			if (flag2)
			{
				cell.m_AiValue = Mathf.Max(10000 - num11, 0);
			}
			break;
		}
		case CFGAiPreset.EMovementMode.Offensive:
		{
			bool flag = false;
			int num7 = 0;
			foreach (CFGCharacter visibleEnemy2 in s_Character.VisibleEnemies)
			{
				if (!flag && CFGCellMap.GetLineOfSightAutoSideSteps(s_Character, visibleEnemy2, cell, visibleEnemy2.CurrentCell, s_Character.BuffedSight) == ELOXHitType.None)
				{
					flag = true;
				}
				int chanceToHit3 = s_Character.GetChanceToHit(visibleEnemy2, null, cell, visibleEnemy2.CurrentCell, ETurnAction.Shoot);
				int num8 = s_Character.CalcDamage(visibleEnemy2, ETurnAction.Shoot, cell);
				int num9 = Mathf.Abs(CFGCharacter.GetTargetCover(visibleEnemy2.CurrentCell, cell).GetAimMod());
				int num10 = chanceToHit3 * num8 + num9;
				if (num10 > num7)
				{
					num7 = num10;
				}
			}
			cell.m_AiValue = num7;
			if (flag)
			{
				cell.m_AiValue += 200;
			}
			break;
		}
		case CFGAiPreset.EMovementMode.Hiding:
			foreach (CFGCharacter character2 in playerOwner.Characters)
			{
				if (CFGCellMap.GetLineOfSightAutoSideSteps(s_Character, character2, cell, character2.CurrentCell, s_Character.BuffedSight) != 0)
				{
					cell.m_AiValue += 10;
				}
			}
			break;
		case CFGAiPreset.EMovementMode.Custom:
		{
			int num2 = 1000;
			for (int i = 0; i < s_Character.VisibleEnemies.Count; i++)
			{
				CFGCharacter cFGCharacter = s_Character.VisibleEnemies[i];
				if (!(cFGCharacter == null))
				{
					int num3 = CFGCellMap.Distance(cell, cFGCharacter.CurrentCell);
					num2 -= num3 * s_AiPreset.m_PressingRate;
					int num4 = CFGCellMap.Distance(cell, s_Character.CurrentCell);
					num2 += num4 * s_AiPreset.m_MobilityRate;
					int chanceToHit = cFGCharacter.GetChanceToHit(s_Character, null, null, cell, ETurnAction.Shoot);
					num2 -= chanceToHit * s_AiPreset.m_HidingRate;
					int chanceToHit2 = s_Character.GetChanceToHit(cFGCharacter, null, cell, cFGCharacter.CurrentCell, ETurnAction.Shoot);
					num2 += chanceToHit2 * s_AiPreset.m_ScoutingRate;
					if (chanceToHit > 0)
					{
						int num5 = cFGCharacter.CalcDamage(s_Character, ETurnAction.Shoot, null, bIgnoreCovers: false, cell);
						num2 -= num5 * s_AiPreset.m_DefenseRate;
					}
					if (chanceToHit2 > 0 && s_Character.ActionPoints == 2)
					{
						int num6 = s_Character.CalcDamage(cFGCharacter, ETurnAction.Shoot, cell);
						num2 += num6 * s_AiPreset.m_OffenseRate;
					}
				}
			}
			if (num2 > 0)
			{
				cell.m_AiValue = num2;
			}
			break;
		}
		}
		return true;
	}
}
