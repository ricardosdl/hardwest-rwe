using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CFGPathSimplifier
{
	[CompilerGenerated]
	private sealed class _003CCalculateSeeReactions_003Ec__AnonStorey56
	{
		internal MoveAction pomMoveAction;

		internal bool _003C_003Em__77(MoveAction m)
		{
			return m.CellEP == pomMoveAction.CellEP && m.Source == pomMoveAction.Source;
		}
	}

	public List<Vector3> m_Positions = new List<Vector3>();

	private CFGCharacter m_Char;

	public List<MoveAction> m_Reactions;

	public bool HasReactions
	{
		get
		{
			if (m_Reactions == null || m_Reactions.Count == 0)
			{
				return false;
			}
			return true;
		}
	}

	private bool IsCharacterInSuspiciousRangeOf(CFGCharacter PlayerChar, CFGCharacter Enemy, float fReqDot)
	{
		if (PlayerChar == null || Enemy == null || PlayerChar.CurrentCell == null || Enemy.CurrentCell == null || !Enemy.IsAlive)
		{
			return false;
		}
		float num = PlayerChar.CharacterData.TotalHeat;
		Vector3 vector = PlayerChar.Position;
		if (m_Positions != null && m_Positions.Count > 0)
		{
			vector = m_Positions[m_Positions.Count - 1];
		}
		float num2 = Vector3.Distance(PlayerChar.Position, Enemy.Position);
		if (num2 > num)
		{
			return false;
		}
		Vector3 rhs = -(Enemy.Position - PlayerChar.Position).normalized;
		float num3 = Vector3.Dot(Enemy.transform.forward, rhs);
		if (num3 < fReqDot)
		{
			return false;
		}
		num2 = Vector3.Distance(vector, Enemy.Position);
		rhs = -(Enemy.Position - vector).normalized;
		num3 = Vector3.Dot(Enemy.transform.forward, rhs);
		if (num2 < num && num3 > fReqDot)
		{
			return false;
		}
		return true;
	}

	public void CalculateSeeReactions(CFGCharacter Character, bool CheckOnly)
	{
		if (Character == null || m_Positions == null || m_Positions.Count == 0 || Character.Owner == null || !Character.Owner.IsPlayer || !CFGSingleton<CFGGame>.Instance.IsInGame() || CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner == null || !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			m_Reactions = null;
			return;
		}
		m_Reactions = new List<MoveAction>();
		if (m_Reactions == null)
		{
			return;
		}
		List<CFGCharacter> list = new List<CFGCharacter>();
		float num = Character.CharacterData.TotalHeat;
		float num2 = 0.7f;
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage != null)
		{
			float num3 = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage.DetectionAngle * 0.5f;
			num2 = Mathf.Cos(num3 * ((float)Math.PI / 180f));
		}
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.Characters)
		{
			if (character.CharacterData != null && character.AIState != EAIState.InCombat && character.AIState != EAIState.Subdued && !IsCharacterInSuspiciousRangeOf(m_Char, character, num2))
			{
				list.Add(character);
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		for (int i = 0; i < m_Positions.Count - 1; i++)
		{
			Vector2 vector = new Vector2(m_Positions[i].x, m_Positions[i].z);
			Vector2 vector2 = new Vector2(m_Positions[i + 1].x, m_Positions[i + 1].z);
			if (Mathf.Abs(vector.x - vector2.x) < 0.1f && Mathf.Abs(vector.y - vector2.y) < 0.1f)
			{
				continue;
			}
			Vector3 normalized = (m_Positions[i + 1] - m_Positions[i]).normalized;
			float magnitude = (m_Positions[i + 1] - m_Positions[i]).magnitude;
			int num4 = 0;
			while (true)
			{
				_003CCalculateSeeReactions_003Ec__AnonStorey56 _003CCalculateSeeReactions_003Ec__AnonStorey = new _003CCalculateSeeReactions_003Ec__AnonStorey56();
				if (num4 >= list.Count)
				{
					break;
				}
				CFGCharacter cFGCharacter = list[num4];
				int num5 = 0;
				Vector3 zero = Vector3.zero;
				float num6 = Vector3.Distance(m_Positions[i], cFGCharacter.Position);
				float num7 = 0f;
				if (num6 < num)
				{
					num7 = 0f;
					num5 = 1;
				}
				else
				{
					float num8 = CFGMath.IntersectRaySphere(m_Positions[i], normalized, cFGCharacter.Position, num);
					if (num8 >= 0f && num8 <= magnitude)
					{
						num7 = num8;
						num5 = 1;
					}
					else
					{
						num7 = 0f;
						num5 = 1;
					}
				}
				if (num5 == 0)
				{
					num4++;
					continue;
				}
				Vector3 forward = cFGCharacter.transform.forward;
				num5 = 0;
				float num9 = num7;
				CFGCell cFGCell = null;
				for (; num9 < magnitude; num9 += 0.3f)
				{
					Vector3 vector3 = m_Positions[i] + normalized * num9;
					Vector3 vector4 = vector3 - cFGCharacter.Position;
					if (!(vector4.magnitude > 0.1f))
					{
						continue;
					}
					float num10 = Vector3.Distance(vector3, cFGCharacter.Position);
					float num11 = Vector3.Dot(forward, vector4.normalized);
					if (!(num11 > num2) || !(num10 <= num))
					{
						continue;
					}
					zero = vector3;
					cFGCell = CFGCellMap.GetCell(zero);
					if (CFGCellMap.GetLineOf(cFGCharacter.CurrentCell, cFGCell, 10000, 16, CFGCellMap.m_bLOS_UseSideStepsForStartPoint, bUseEndSideSteps: false) == ELOXHitType.None)
					{
						MoveAction newMoveAction = new MoveAction(cFGCell.EP, cFGCharacter, EMOVEACTION.SUSPICIOUS_CHECK, 0, 0, zero);
						num5 = 1;
						if (!m_Reactions.Any((MoveAction m) => m.CellEP == newMoveAction.CellEP && m.Source == newMoveAction.Source))
						{
							m_Reactions.Add(new MoveAction(cFGCell.EP, cFGCharacter, EMOVEACTION.SUSPICIOUS_CHECK, 0, 0, zero));
						}
					}
				}
				if (0 == 0)
				{
					num4++;
					continue;
				}
				list.RemoveAt(num4);
				_003CCalculateSeeReactions_003Ec__AnonStorey.pomMoveAction = new MoveAction(cFGCell.EP, cFGCharacter, EMOVEACTION.SUSPICIOUS_CHECK, 0, 0, zero);
				if (!m_Reactions.Any(_003CCalculateSeeReactions_003Ec__AnonStorey._003C_003Em__77))
				{
					m_Reactions.Add(new MoveAction(cFGCell.EP, cFGCharacter, EMOVEACTION.SUSPICIOUS_CHECK, 0, 0, zero));
				}
				if (num4 < list.Count)
				{
					continue;
				}
				break;
			}
		}
	}

	public void CalculateReactions(CFGCharacter Character, bool CheckOnly)
	{
		if (Character == null || m_Positions == null || m_Positions.Count == 0 || Character.Owner == null || !Character.Owner.IsPlayer || !CFGSingleton<CFGGame>.Instance.IsInGame() || CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner == null || CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			m_Reactions = null;
			return;
		}
		m_Reactions = new List<MoveAction>();
		if (m_Reactions == null)
		{
			return;
		}
		List<CFGCharacter> list = new List<CFGCharacter>();
		float num = 10f;
		bool flag = true;
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_RectionShootInfo != null)
		{
			num = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_RectionShootInfo.m_Range;
			flag = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_RectionShootInfo.m_InstantRotation;
		}
		Vector3 vector = m_Positions[m_Positions.Count - 1];
		Vector3 worldPosition = Character.CurrentCell.WorldPosition;
		foreach (CFGCharacter visibleEnemy in Character.VisibleEnemies)
		{
			if (visibleEnemy.CanDoReactionShot)
			{
				Vector3 worldPosition2 = visibleEnemy.CurrentCell.WorldPosition;
				if (!(Vector3.Distance(worldPosition, worldPosition2) <= num + 0.2f) && CFGCellMap.GetLineOfSightAutoSideSteps(Character, visibleEnemy) == ELOXHitType.None)
				{
					list.Add(visibleEnemy);
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		int num2 = Character.Hp;
		for (int i = 0; i < m_Positions.Count - 1; i++)
		{
			Vector2 vector2 = new Vector2(m_Positions[i].x, m_Positions[i].z);
			Vector2 vector3 = new Vector2(m_Positions[i + 1].x, m_Positions[i + 1].z);
			if (Mathf.Abs(vector2.x - vector3.x) < 0.1f && Mathf.Abs(vector2.y - vector3.y) < 0.1f)
			{
				continue;
			}
			CFGCell cell = CFGCellMap.GetCell(m_Positions[i]);
			CFGCell cell2 = CFGCellMap.GetCell(m_Positions[i + 1]);
			bool flag2 = false;
			bool flag3 = false;
			if ((bool)cell && cell.StairsType == CFGCell.EStairsType.Slope)
			{
				flag2 = true;
			}
			if ((bool)cell2 && cell2.StairsType == CFGCell.EStairsType.Slope)
			{
				flag3 = true;
			}
			if (flag2 && flag3)
			{
				continue;
			}
			Vector3 normalized = (m_Positions[i + 1] - m_Positions[i]).normalized;
			float magnitude = (m_Positions[i + 1] - m_Positions[i]).magnitude;
			int num3 = 0;
			while (num3 < list.Count)
			{
				CFGCharacter cFGCharacter = list[num3];
				int num4 = 0;
				Vector3 vector4 = Vector3.zero;
				float num5 = Vector3.Distance(m_Positions[i], cFGCharacter.Position);
				if (num5 < num && CanCharDoReactionShotFromPos(cFGCharacter.CurrentCell, m_Positions[i]))
				{
					vector4 = m_Positions[i];
					num4 = 1;
				}
				if (num4 == 0)
				{
					float num6 = CFGMath.IntersectRaySphere(m_Positions[i], normalized, cFGCharacter.Position, num);
					if (num6 >= 0f && num6 <= magnitude)
					{
						vector4 = m_Positions[i] + normalized * num6;
						if (CanCharDoReactionShotFromPos(cFGCharacter.CurrentCell, vector4))
						{
							num4 = 1;
						}
					}
				}
				if (num4 == 0 && i == m_Positions.Count - 2)
				{
					num5 = Vector3.Distance(m_Positions[i + 1], cFGCharacter.Position);
					if (num5 < num && CanCharDoReactionShotFromPos(cFGCharacter.CurrentCell, m_Positions[i + 1]))
					{
						vector4 = m_Positions[i + 1];
						num4 = 1;
					}
				}
				if (num4 == 0)
				{
					num3++;
					continue;
				}
				CFGCell cell3 = CFGCellMap.GetCell(vector4);
				if (cell3 == null)
				{
					num3++;
					continue;
				}
				list.RemoveAt(num3);
				if ((flag || i < 2) && !CheckOnly)
				{
					cFGCharacter.RotateToward(vector4);
				}
				int num7 = cFGCharacter.CalcDamage(Character, ETurnAction.Shoot, null, bIgnoreCovers: true);
				num2 -= num7;
				int iV = 100;
				m_Reactions.Add(new MoveAction(cell3.EP, cFGCharacter, EMOVEACTION.SHOOT_AT, iV, num7, vector4));
				if (num2 < 1 && !CheckOnly)
				{
					return;
				}
				if (num3 < list.Count)
				{
					continue;
				}
				break;
			}
		}
	}

	private bool CanCharDoReactionShotFromPos(CFGCell CharCell, Vector3 Pos)
	{
		CFGCell cell = CFGCellMap.GetCell(Pos);
		if (cell == null)
		{
			return false;
		}
		if (CFGCellMap.GetLineOf(CharCell, cell, 10000, 16, bUseStartSideSteps: true, bUseEndSideSteps: true) != 0)
		{
			return false;
		}
		return true;
	}

	public void MakeCopy(LinkedList<CFGCell> SourcePath)
	{
		m_Positions.Clear();
		if (SourcePath == null)
		{
			return;
		}
		LinkedListNode<CFGCell> linkedListNode = SourcePath.First;
		if (linkedListNode != null)
		{
			while (linkedListNode != null)
			{
				m_Positions.Add(linkedListNode.Value.WorldPosition);
				linkedListNode = linkedListNode.Next;
			}
		}
	}

	public void Caclculate(LinkedList<CFGCell> SourcePath)
	{
		m_Positions.Clear();
		if (SourcePath == null)
		{
			return;
		}
		LinkedListNode<CFGCell> linkedListNode = SourcePath.First;
		if (linkedListNode == null)
		{
			return;
		}
		m_Char = linkedListNode.Value.CurrentCharacter;
		LinkedListNode<CFGCell> linkedListNode2 = SourcePath.Last;
		if (linkedListNode2 == null)
		{
			return;
		}
		m_Positions.Add(linkedListNode.Value.WorldPositionForMovement);
		if (SourcePath.Count == 2)
		{
			m_Positions.Add(linkedListNode2.Value.WorldPositionForMovement);
			return;
		}
		bool flag = false;
		LinkedListNode<CFGCell> linkedListNode3 = linkedListNode;
		while (linkedListNode != SourcePath.Last)
		{
			if (linkedListNode.Value.StairsType != 0 || linkedListNode2.Value.StairsType != 0)
			{
				linkedListNode2 = linkedListNode.Next;
				linkedListNode3 = linkedListNode2;
			}
			if (linkedListNode2 == linkedListNode || linkedListNode2 == linkedListNode.Next)
			{
				m_Positions.Add(linkedListNode.Next.Value.WorldPositionForMovement);
				if (linkedListNode3 != null)
				{
					CutRoad(m_Positions.Count - 1, linkedListNode, linkedListNode3, linkedListNode.Next);
				}
				linkedListNode = linkedListNode.Next;
				linkedListNode2 = SourcePath.Last;
				continue;
			}
			if (linkedListNode.Value.Floor == linkedListNode2.Value.Floor && CanMove(linkedListNode.Value, linkedListNode2.Value))
			{
				m_Positions.Add(linkedListNode2.Value.WorldPositionForMovement);
				if (m_Positions.Count > 2 && !flag)
				{
					flag = CutRoad(m_Positions.Count - 1, linkedListNode, linkedListNode3, linkedListNode2);
					if (flag)
					{
						linkedListNode3 = linkedListNode2;
					}
				}
				else if (linkedListNode != linkedListNode2)
				{
					flag = false;
				}
				linkedListNode3 = linkedListNode2;
				if (linkedListNode2.Next == null)
				{
					break;
				}
				linkedListNode = linkedListNode2;
				linkedListNode2 = SourcePath.Last;
				continue;
			}
			if (linkedListNode.Value.Floor != linkedListNode2.Value.Floor)
			{
				while (linkedListNode.Value.Floor != linkedListNode2.Value.Floor && linkedListNode2 != linkedListNode)
				{
					linkedListNode2 = linkedListNode2.Previous;
				}
			}
			if (linkedListNode2 != linkedListNode)
			{
				linkedListNode2 = linkedListNode2.Previous;
			}
		}
	}

	private bool CutRoad(int Idx, LinkedListNode<CFGCell> ToMove, LinkedListNode<CFGCell> Start, LinkedListNode<CFGCell> End)
	{
		if (m_Positions.Count < 3 || ToMove == null || Start == null || End == null)
		{
			return false;
		}
		if (ToMove.Value.Floor != Start.Value.Floor || ToMove.Value.Floor != End.Value.Floor)
		{
			return false;
		}
		if (ToMove.Value.StairsType != 0 || Start.Value.StairsType != 0)
		{
			return false;
		}
		if (Start == End || Start == ToMove || End == ToMove)
		{
			return false;
		}
		LinkedListNode<CFGCell> linkedListNode = ToMove;
		CFGCell value = Start.Value;
		CFGCell value2 = linkedListNode.Value;
		CFGCell value3 = End.Value;
		float num = Vector3.Distance(value.WorldPositionForMovement, value2.WorldPositionForMovement);
		num += Vector3.Distance(value2.WorldPositionForMovement, value3.WorldPositionForMovement);
		float num2 = 100000000f;
		CFGCell cFGCell = null;
		linkedListNode = ToMove.Previous;
		while (linkedListNode != Start && CanMove(linkedListNode.Value, value) && CanMove(linkedListNode.Value, value3))
		{
			float num3 = Vector3.Distance(value3.WorldPositionForMovement, linkedListNode.Value.WorldPositionForMovement);
			num3 += Vector3.Distance(value.WorldPositionForMovement, linkedListNode.Value.WorldPositionForMovement);
			if (num3 > num2)
			{
				break;
			}
			num2 = num3;
			cFGCell = linkedListNode.Value;
			linkedListNode = linkedListNode.Previous;
		}
		linkedListNode = ToMove.Next;
		while (linkedListNode != End && CanMove(linkedListNode.Value, value) && CanMove(linkedListNode.Value, value3))
		{
			float num4 = Vector3.Distance(value3.WorldPositionForMovement, linkedListNode.Value.WorldPositionForMovement);
			num4 += Vector3.Distance(value.WorldPositionForMovement, linkedListNode.Value.WorldPositionForMovement);
			if (num4 > num2)
			{
				break;
			}
			num2 = num4;
			cFGCell = linkedListNode.Value;
			linkedListNode = linkedListNode.Next;
		}
		if (cFGCell == null)
		{
			LinkedListNode<CFGCell> previous = ToMove.Previous;
			num2 = Vector3.Distance(Start.Value.WorldPositionForMovement, ToMove.Value.WorldPositionForMovement);
			num2 += Vector3.Distance(End.Value.WorldPositionForMovement, ToMove.Value.WorldPositionForMovement);
			CFGCell cFGCell2 = null;
			CFGCell cFGCell3 = null;
			while (previous != Start && previous != null)
			{
				LinkedListNode<CFGCell> next = ToMove.Next;
				while (next != End && next != null)
				{
					if (CanMove(previous.Value, next.Value))
					{
						float num5 = Vector3.Distance(previous.Value.WorldPositionForMovement, next.Value.WorldPositionForMovement);
						num5 += Vector3.Distance(previous.Value.WorldPositionForMovement, Start.Value.WorldPositionForMovement);
						num5 += Vector3.Distance(next.Value.WorldPositionForMovement, End.Value.WorldPositionForMovement);
						if (num5 < num2)
						{
							num2 = num5;
							cFGCell2 = previous.Value;
							cFGCell3 = next.Value;
						}
					}
					next = next.Next;
				}
				previous = previous.Previous;
			}
			if ((bool)cFGCell2 && (bool)cFGCell3)
			{
				m_Positions.RemoveAt(Idx - 1);
				m_Positions.Insert(Idx - 1, cFGCell3.WorldPositionForMovement);
				m_Positions.Insert(Idx - 1, cFGCell2.WorldPositionForMovement);
				return true;
			}
		}
		if (cFGCell != null)
		{
			m_Positions[Idx - 1] = cFGCell.WorldPositionForMovement;
			return true;
		}
		return false;
	}

	private bool CanMove(CFGCell Start, CFGCell End)
	{
		return CFGCellMap.CanMoveInStraightLine(Start, End, m_Char);
	}

	public void MakeSmooth()
	{
		if (m_Positions != null && m_Positions.Count >= 3)
		{
			List<Vector3> newl = new List<Vector3>(m_Positions);
			m_Positions.Clear();
			SmoothSegment(0, 0, 1, 2, ref newl);
			for (int i = 1; i < newl.Count - 2; i++)
			{
				SmoothSegment(i - 1, i, i + 1, i + 2, ref newl);
			}
			SmoothSegment(newl.Count - 3, newl.Count - 2, newl.Count - 1, newl.Count - 1, ref newl);
			m_Positions.Add(newl[newl.Count - 1]);
		}
	}

	private void SmoothSegment(int p0, int p1, int p2, int p3, ref List<Vector3> newl)
	{
		Vector3 p4 = ((p0 != p1) ? newl[p0] : (newl[p1] - (newl[p2] - newl[p1])));
		Vector3 p5 = ((p2 != p3) ? newl[p3] : (newl[p2] + (newl[p2] - newl[p1])));
		CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0f);
		if (p0 != p1)
		{
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.03f);
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.06f);
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.1f);
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.13f);
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.16f);
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.2f);
		}
		CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.28f);
		CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.35f);
		CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.5f);
		CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.65f);
		CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.72f);
		if (p2 != p3)
		{
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.8f);
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.83f);
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.86f);
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.9f);
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.93f);
			CatmullRomVec3(p4, newl[p1], newl[p2], p5, 0.96f);
		}
	}

	private void CatmullRomVec3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 normalized = (p2 - p1).normalized;
		Vector3 normalized2 = (p3 - p2).normalized;
		p0 = p1;
		p3 = p2;
		float num = Vector3.Distance(p1, p2);
		float a = 1.4f;
		if (Vector3.Dot(normalized, normalized2) < 0.2f)
		{
			a = 0.35f;
		}
		float b = num * 0.4f;
		p1 += ((!(normalized.sqrMagnitude > 0f)) ? Vector3.zero : (normalized * Mathf.Min(a, b)));
		p2 -= ((!(normalized2.sqrMagnitude > 0f)) ? Vector3.zero : (normalized2 * Mathf.Min(a, b)));
		CFGMath.Bezier(out var result, p0, p1, p2, p3, t);
		m_Positions.Add(result);
	}
}
