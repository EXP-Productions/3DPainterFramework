using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StrokeNode
{
    Space m_Space = Space.Self;

    #region Original transform values
    public Vector3 m_OriginalPos;
    public Vector3  OriginalPos { get { return m_OriginalPos; } set { m_OriginalPos = value; } }

    private Quaternion m_OriginalRot;
    public Quaternion  OriginalRot { get { return m_OriginalRot; } }

    private Vector3 m_OriginalScale;
    public Vector3  OriginalScale { get { return m_OriginalScale; } }

    public Vector3 m_AdjustedScale;
    public Vector3 AdjustedScale { get { return m_AdjustedScale; } set { m_AdjustedScale = value; } }
    #endregion

    #region Jitter transform values
    private Vector3 m_JitterPos = Vector3.zero;
    public Vector3 JitterPos { get { return m_JitterPos; } set { m_JitterPos = value; } }

    private Quaternion m_JitterRot = Quaternion.identity;
    public Quaternion JitterRot { get { return m_JitterRot; } set { m_JitterRot = value; } }

    public Vector3 m_JitterScale = Vector3.zero;
    public Vector3 JitterScale { get { return m_JitterScale; } set { m_JitterScale = value; } }
    #endregion

    #region Offset transform values
    private Vector3 m_OffsetPos = Vector3.zero;
    public Vector3 OffsetPos { get { return m_OffsetPos; } set { m_OffsetPos = value; } }

    private Quaternion m_OffsetRot = Quaternion.identity;
    public Quaternion OffsetRot { get { return m_OffsetRot; } }

    private Vector3 m_OffsetScale = Vector3.zero;
    public Vector3 OffsetScale { get { return m_OffsetScale; } }
    #endregion
    
    #region Modified transform values
    private Vector3 m_ModifiedPos;
    public Vector3 ModifiedPos
    {
        get
        {
            if (m_Space == Space.World)
            {
                return m_OriginalPos + m_OffsetPos + m_JitterPos;
            }
            else
            {
                return m_OriginalPos + (ModifiedRot * (m_OffsetPos + m_JitterPos));
            }           
        }
    }
     
    private Quaternion m_ModifiedRot;
    public Quaternion ModifiedRot { get { return m_OriginalRot * m_OffsetRot * m_JitterRot; } }

    private Vector3 m_ModifiedScale;
    public Vector3 ModifiedScale { get { return AdjustedScale + m_OffsetScale + JitterScale; } }
    #endregion

    #region Constructor methods
    public StrokeNode(Transform transform)
    {        
        m_OriginalPos = transform.position;
        m_OriginalRot = transform.rotation;
        m_OriginalScale = transform.localScale;

        ResetModifiedValues();
    }

    public StrokeNode(Transform transform, Transform strokeTransform)
    {
        Transform preParent = transform.parent;
        transform.SetParent(strokeTransform);
        
        m_OriginalPos = transform.localPosition;
        m_OriginalRot = transform.localRotation;
        m_OriginalScale = transform.localScale;

        transform.SetParent(preParent);

        ResetModifiedValues();
    }

    public StrokeNode(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        m_OriginalPos = pos;
        m_OriginalRot = rot;
        m_OriginalScale = scale;

        ResetModifiedValues();
    }
    #endregion

    #region Helper methods
    void ResetModifiedValues()
    {
        m_ModifiedPos = m_OriginalPos;
        m_ModifiedRot = m_OriginalRot;
        m_ModifiedScale = m_OriginalScale;
    }

    public void SetOriginalRotation(Quaternion r)
    {
        m_OriginalRot = r;
    }

    public void SetTransform(Transform t, bool local)
    {
        if (local)
        {
            t.localPosition = ModifiedPos;
            t.localRotation = ModifiedRot;
            t.localScale = ModifiedScale;
        }
        else
        {
            t.position = ModifiedPos;
            t.rotation = ModifiedRot;
            t.localScale = ModifiedScale;
        }
    }

    public Vector3 GetNormal()
    {
        return ModifiedRot * Vector3.right;
    }

    #endregion
}
