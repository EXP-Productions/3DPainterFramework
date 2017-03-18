using UnityEngine;
using System.Collections;

public static class GameObjectExtensions
{
	public static void SetParentAndZero( this GameObject gO, GameObject parent )
	{
		gO.transform.parent = parent.transform;
		
		gO.transform.localPosition = Vector3.zero;
		gO.transform.localRotation = Quaternion.identity;
		gO.transform.localScale = Vector3.one;
	}
	
	public static void SetParentAndZero( this GameObject gO, Transform parent )
	{
		gO.transform.parent = parent;
		
		gO.transform.localPosition = Vector3.zero;
		gO.transform.localRotation = Quaternion.identity;
		gO.transform.localScale = Vector3.one;
	}

	public static void SetAllRendererColors( this GameObject gO, Color col, bool SetChildren )
	{
		//
	}
}
