using Core;
using UnityEngine;

namespace BehaviorPattern.BehaviorTree;

[Node("Field Test")]
public class BTTask_FieldTest : BTTask
{
	public enum EnumTest
	{
		One,
		Two,
		Three
	}

	[Body("Text")]
	public string m_Text = "Message";

	[Body("Enabled")]
	public bool m_Bool = true;

	[Body("2nd")]
	public bool m_Bool2;

	[Body("Integer")]
	public int m_Int = 7;

	[Body("Floating")]
	public float m_Float = 0.33f;

	[Body("Enumeration")]
	public EnumTest m_Enum;

	[Body("Vector 2D")]
	public Vector2 m_Vector2;

	[Body("Vector 3D")]
	public Vector3 m_Vector3;

	[Body("Vector 4D")]
	public Vector4 m_Vector4;
}
