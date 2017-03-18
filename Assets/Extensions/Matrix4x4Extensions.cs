using UnityEngine;
using System.Collections;

public static class Matrix4x4Extensions
{
    /// <summary>
    /// Extract translation from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Translation offset.
    /// </returns>
    public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 translate;
        translate.x = matrix.m03;
        translate.y = matrix.m13;
        translate.z = matrix.m23;
        return translate;
    }


    /// <summary>
    /// Extract rotation quaternion from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Quaternion representation of rotation transform.
    /// </returns>
    public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        //if (forward.x == 0)
        //    return Quaternion.identity;
       // else
            return Quaternion.LookRotation(forward, upwards);
    }

    /// <summary>
    /// Extract scale from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Scale vector.
    /// </returns>
    public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }

    /// <summary>
    /// Extract position, rotation and scale from TRS matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <param name="localPosition">Output position.</param>
    /// <param name="localRotation">Output rotation.</param>
    /// <param name="localScale">Output scale.</param>
    public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale)
    {
        localPosition = ExtractTranslationFromMatrix(ref matrix);
        localRotation = ExtractRotationFromMatrix(ref matrix);
        localScale = ExtractScaleFromMatrix(ref matrix);
    }

    /// <summary>
    /// Set transform component from TRS matrix.
    /// </summary>
    /// <param name="transform">Transform component.</param>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix)
    {
        transform.position = ExtractTranslationFromMatrix(ref matrix);
        transform.localRotation = ExtractRotationFromMatrix(ref matrix);
        transform.localScale = ExtractScaleFromMatrix(ref matrix);
    }


    // EXTRAS!

    /// <summary>
    /// Identity quaternion.
    /// </summary>
    /// <remarks>
    /// <para>It is faster to access this variation than <c>Quaternion.identity</c>.</para>
    /// </remarks>
    public static readonly Quaternion IdentityQuaternion = Quaternion.identity;
    /// <summary>
    /// Identity matrix.
    /// </summary>
    /// <remarks>
    /// <para>It is faster to access this variation than <c>Matrix4x4.identity</c>.</para>
    /// </remarks>
    public static readonly Matrix4x4 IdentityMatrix = Matrix4x4.identity;

    /// <summary>
    /// Get translation matrix.
    /// </summary>
    /// <param name="offset">Translation offset.</param>
    /// <returns>
    /// The translation transform matrix.
    /// </returns>
    public static Matrix4x4 TranslationMatrix(Vector3 offset)
    {
        Matrix4x4 matrix = IdentityMatrix;
        matrix.m03 = offset.x;
        matrix.m13 = offset.y;
        matrix.m23 = offset.z;
        return matrix;
    }

    public static Matrix4x4 GetMirror(this Matrix4x4 m, float x, float y, float z)
    {
        var scale = new Vector3(x, y, z);
        var matrix = Matrix4x4.Scale(scale) * m;
        return matrix;
    }

    public static Matrix4x4 GetMatrixWithScale(this Matrix4x4 baseMatrix, float x, float y, float z)
    {
        var scale = new Vector3(x, y, z);
        return Matrix4x4.Scale(scale) * baseMatrix;
    }

    public static Matrix4x4 MatrixFromTransform( Transform t )
    {
        return Matrix4x4.TRS(t.position, t.rotation, t.localScale);
    }

    public static Matrix4x4 MatrixFromTRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        return Matrix4x4.TRS(pos, rot, scale);
    }

    /*
    public static Vector3 GetPosition(this Matrix4x4 matrix)
    {
        var x = matrix.m03;
        var y = matrix.m13;
        var z = matrix.m23;

        return new Vector3(x, y, z);
    }

    public static Vector3 GetScale(this Matrix4x4 m)
    {
        var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
        var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
        var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);

        return new Vector3(x, y, z);
    }
    

    public static Quaternion GetRotation( this Matrix4x4 m)
    {
        Vector3 forward;
        forward.x = m.m02;
        forward.y = m.m12;
        forward.z = m.m22;

        Vector3 upwards;
        upwards.x = m.m01;
        upwards.y = m.m11;
        upwards.z = m.m21;
        
        return Quaternion.LookRotation(forward, upwards);
    }
    */
    public static Matrix4x4 LerpTRSMatricies(Matrix4x4 m1, Matrix4x4 m2, float lerp)
    {
        Vector3 pos = Vector3.Lerp(m1.GetPosition(), m2.GetPosition(), lerp);
        Quaternion rot = Quaternion.Slerp(m1.GetRotation(), m2.GetRotation(), lerp);
        Vector3 scale = Vector3.Lerp(m1.GetScale(), m2.GetScale(), lerp);

        return Matrix4x4.TRS(pos, rot, scale);
    }

    public static Matrix4x4 TranslateTRSMatrix(Matrix4x4 m1, Vector3 translation )
    {
        Vector3 pos = m1.GetPosition() + translation;
        Quaternion rot = m1.GetRotation();
        Vector3 scale = m1.GetScale();

        return Matrix4x4.TRS(pos, rot, scale);
    }


    public static Quaternion GetRotation(this Matrix4x4 matrix)
    {
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + matrix.m00 + matrix.m11 + matrix.m22)) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + matrix.m00 - matrix.m11 - matrix.m22)) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - matrix.m00 + matrix.m11 - matrix.m22)) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - matrix.m00 - matrix.m11 + matrix.m22)) / 2;
        q.x = _copysign(q.x, matrix.m21 - matrix.m12);
        q.y = _copysign(q.y, matrix.m02 - matrix.m20);
        q.z = _copysign(q.z, matrix.m10 - matrix.m01);
        return q;
    }

    public static Vector3 GetPosition(this Matrix4x4 matrix)
    {
        var x = matrix.m03;
        var y = matrix.m13;
        var z = matrix.m23;

        return new Vector3(x, y, z);
    }

    public static Vector3 GetScale(this Matrix4x4 m)
    {
        var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
        var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
        var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);

        return new Vector3(x, y, z);
    }

    private static float _copysign(float sizeval, float signval)
    {
        return Mathf.Sign(signval) == 1 ? Mathf.Abs(sizeval) : -Mathf.Abs(sizeval);
    }
}