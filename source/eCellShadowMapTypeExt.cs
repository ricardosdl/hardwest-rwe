public static class eCellShadowMapTypeExt
{
	public static int GetMultiplier(this eCellShadowMapType level)
	{
		return level switch
		{
			eCellShadowMapType.ShadowMap_x2 => 2, 
			eCellShadowMapType.ShadowMap_x4 => 4, 
			eCellShadowMapType.ShadowMap_x8 => 8, 
			eCellShadowMapType.ShadowMap_x16 => 16, 
			_ => 1, 
		};
	}
}
