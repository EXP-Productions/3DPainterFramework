using UnityEngine;
using System.Collections;

public static class MaterialExtensions
{	
	public static void SetCol( this Material mat, Color col )
	{
		if (mat.HasProperty(		"_TintColor"))
			mat.SetColor(			"_TintColor", 	col);
		else if (mat.HasProperty(	"_Color"))
			mat.SetColor(			"_Color", 		col);
		else if (mat.HasProperty(	"_MainColor"))
			mat.SetColor( 			"_MainColor", 	col);
	}

	public static void SetColWithoutAlpha( this Material mat, Color col )
	{
		Color matCol = mat.GetMatCol();
		col.a = matCol.a;

		if (mat.HasProperty(		"_TintColor"))
			mat.SetColor(			"_TintColor", 	col);
		else if (mat.HasProperty(	"_Color"))
			mat.SetColor(			"_Color", 		col);
		else if (mat.HasProperty(	"_MainColor"))
			mat.SetColor( 			"_MainColor", 	col);
	}

	public static void SetAlpha( this Material mat, float alpha )
	{
		if (mat.HasProperty(		"_TintColor"))
		{
			Color col = mat.GetColor("_TintColor");
			col.a = alpha;
			mat.SetColor(			"_TintColor", 	col);
		}
		else if (mat.HasProperty(	"_MainColor"))
		{
			Color col = mat.GetColor("_MainColor");
			col.a = alpha;
			mat.SetColor(			"_MainColor", 	col);
		}
		else if (mat.HasProperty(	"_Color"))
		{
			Color col = mat.GetColor("_Color");
			col.a = alpha;
			mat.SetColor(			"_Color", 	col);
		}
		else
		{
			Debug.Log( "Can't find colour property" );
		}
	}

	public static void SetAlpha( this Material mat, Color baseCol, float alpha )
	{
		if (mat.HasProperty(		"_TintColor"))
		{
			Color col = baseCol;
			col.a = alpha;
			mat.SetColor(			"_TintColor", 	col);
		}
	}

	public static void LerpAlpha( this Material mat, Color baseCol, float alpha, float smoothing )
	{
		if (mat.HasProperty(		"_TintColor"))
		{
			Color col = mat.GetColor("_TintColor");
			col.r = baseCol.r;
			col.g = baseCol.g;
			col.b = baseCol.b;
			col.a = Mathf.Lerp( col.a, alpha, Time.deltaTime * smoothing );
			mat.SetColor(			"_TintColor", 	col);
		}
	}


	public static Color GetMatCol( this Material mat )
	{
		if (mat.HasProperty("_TintColor"))
			return mat.GetColor("_TintColor");
		else if(mat.HasProperty("_Color"))
			return mat.GetColor("_Color");
		else if(mat.HasProperty("_MainColor"))
			return mat.GetColor("_MainColor");
		
		return Color.white;
	}

	public static Material CloneMaterial( this Material mat, bool destroyOnLoad )
	{
		Material newMat = (Material)Material.Instantiate( mat );
		
		if (!destroyOnLoad)
			UnityEngine.Object.DontDestroyOnLoad( newMat );
		
		return newMat;
	}
}
