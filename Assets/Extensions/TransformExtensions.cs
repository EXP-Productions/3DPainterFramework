using UnityEngine;
using System.Collections;

public static class TransformExtensions
{
    // Find furthest in array
    // Make heirarchy world 0
    // Follow XY, XZ, YZ


    [ContextMenu("Reset pivot")]
    public static void SetPosToFirstChild( this Transform t )
    {
        Vector3 child0Pos = t.GetChild(0).transform.position;
        Vector3 distance = child0Pos - t.position;

        t.position = child0Pos;

        for (int i = 0; i < t.childCount; i++)
        {
            t.GetChild(i).transform.position -= distance;
        }
    }

	public static void SetWorldX( this Transform t, float x )
	{
		Vector3 pos = t.position;
		pos.x = x;
		t.position = pos;
	}
	
	public static void SetWorldY( this Transform t, float y )
	{
		Vector3 pos = t.position;
		pos.y = y;
		t.position = pos;
	}
	
	public static void SetWorldZ( this Transform t, float z )
	{
		Vector3 pos = t.position;
		pos.z = z;
		t.position = pos;
	}
	
	public static void SetLocalX( this Transform t, float x )
	{
		Vector3 pos = t.localPosition;
		pos.x = x;
		t.localPosition = pos;
	}
	
	public static void SetLocalY( this Transform t, float y )
	{
		Vector3 pos = t.localPosition;
		pos.y = y;
		t.localPosition = pos;        
	}
	
	public static void SetLocalZ( this Transform t, float z )
	{
		Vector3 pos = t.localPosition;
		pos.z = z;
		
		
		t.localPosition = pos;
	}
	
	public static void SetLocalRotX( this Transform t, float x )
	{
		Vector3 rot = t.localRotation.eulerAngles;
		rot.x = x;
		
		Quaternion rotation = Quaternion.Slerp( t.localRotation, Quaternion.Euler( rot ), 1 );
				
		t.localRotation = rotation;
	}
	
	public static void SetLocalRotY( this Transform t, float y )
	{
		Vector3 rot = t.localRotation.eulerAngles;
		rot.y = y;
		
		Quaternion rotation = Quaternion.Slerp( t.localRotation, Quaternion.Euler( rot ), 1 );
		t.localRotation = rotation;
	}
	
	public static void SetLocalRotZ( this Transform t, float z )
	{
		Vector3 rot = t.localRotation.eulerAngles;
		rot.z = z;
		
		Quaternion rotation = Quaternion.Slerp( t.localRotation, Quaternion.Euler( rot ), 1 );
		t.localRotation = rotation;
	}

	public static void SetRotX( this Transform t, float x )
	{
		Vector3 rot = t.rotation.eulerAngles;
		rot.x = x;
		
		Quaternion rotation = Quaternion.Slerp( t.rotation, Quaternion.Euler( rot ), 1 );
		
		
		
		t.rotation = rotation;
	}
	
	public static void SetRotY( this Transform t, float y )
	{
		Vector3 rot = t.rotation.eulerAngles;
		rot.y = y;
		
		Quaternion rotation = Quaternion.Slerp( t.rotation, Quaternion.Euler( rot ), 1 );
		t.rotation = rotation;
	}
	
	public static void SetRotZ( this Transform t, float z )
	{
		Vector3 rot = t.rotation.eulerAngles;
		rot.z = z;
		
		Quaternion rotation = Quaternion.Slerp( t.rotation, Quaternion.Euler( rot ), 1 );
		t.rotation = rotation;
	}
	
	public static void SetScaleX( this Transform t, float x )
	{
		Vector3 scale = t.localScale;
		scale.x = x;
		t.localScale = scale;
	}
	
	public static void SetScaleY( this Transform t, float y )
	{
		Vector3 scale = t.localScale;
		scale.y = y;
		t.localScale = scale;
	}
	
	public static void SetScaleZ( this Transform t, float z )
	{
		Vector3 scale = t.localScale;
		scale.z = z;
		t.localScale = scale;
	}
	
	public static void CopyTransform( this Transform t, Transform transformToCopy )
	{
		t.position = 	transformToCopy.position;
		t.rotation = 	transformToCopy.rotation;
		t.localScale = 	transformToCopy.localScale;
	}
	
	public static void DestroyAllChildren( this Transform t )
	{
		for( int i = 0; i < t.childCount; i++ )
		{
			UnityEngine.GameObject.Destroy( t.GetChild( i ).gameObject );
		}
	}	
	
	public static Vector3 PositionBetween( this Transform t, Transform t2, float normalizedPosBetween )
	{
		Vector3 pos = Vector3.zero;
		Vector3 placementVector = t2.position - t.position;
		pos = t.position + ( placementVector * normalizedPosBetween );
			
		return pos;
	}

	public static void ParentAndZero( this Transform t, Transform parent )
	{
		t.parent = parent;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;
	}

	public static void ParentAndZero( this Transform t, Transform parent, bool zeroPos, bool zeroRot, bool zeroScale )
	{
		t.parent = parent;

		if( zeroPos )
			t.localPosition = Vector3.zero;

		if( zeroRot )
			t.localRotation = Quaternion.identity;

		if( zeroScale )
			t.localScale = Vector3.one;
	}

	
}
