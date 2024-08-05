public class CFGEmptyObject : CFGSerializableObject
{
	public override ESerializableType SerializableType => ESerializableType.Empty;

	public override bool NeedsSaving => true;

	public override bool OnSerialize(CFG_SG_Node ParentNode)
	{
		CFG_SG_Node cFG_SG_Node = OnBeginSerialization(ParentNode);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("Pos", base.transform.position.ToString());
		cFG_SG_Node.Attrib_Set("Rot", base.transform.rotation.ToString());
		return true;
	}

	public override bool OnDeserialize(CFG_SG_Node Node)
	{
		return true;
	}
}
