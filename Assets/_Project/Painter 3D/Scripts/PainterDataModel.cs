using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Painter3D
{
    /// <summary>
    /// Holds data for the stroke to be seriliazed and deserialized
    /// </summary>
    [System.Serializable]
    public class CanvasData
    {
        public StrokeData[] m_StrokesData;

        public void Initialise(Canvas c)
        {
			List<StrokeData> strokeData = new List<StrokeData>();
	
			for (int i = 0; i < c.Strokes.Count; i++) {
				if (c.Strokes [i] != null) {
					StrokeData s = new StrokeData ();
					if (s.Initialise (c.Strokes [i])) {
						strokeData.Add (s);
					}
				}
			}
			m_StrokesData = strokeData.ToArray ();
        }
    }


    /// <summary>
    /// Holds data for the stroke to be seriliazed and deserialized
    /// </summary>
    [System.Serializable]
    public class StrokeData
    {
        public TransformData m_TransformData;
        public string m_Scale;
        public string m_RendererName;
        public string m_Color;
        public TransformData[] m_RawStrokeNodeData;

        public bool Initialise(Stroke s)
        {
			try
			{
                m_TransformData = new TransformData(s.transform);

                // Save scale
                m_Scale = s.m_Scale.ToString();

                // Save renderer
                m_RendererName = s.m_StrokeRenderer.name;
                m_RendererName = m_RendererName.Replace("(Clone)", "");

                Color col = s.m_StrokeRenderer.m_Color;
                Vector3 colVec = new Vector4(col.r, col.g, col.b);
                m_Color = colVec.ToString("G2");                

                // Serialize stroke nodes
                m_RawStrokeNodeData = new TransformData[s.RawNodeCount];
                for (int i = 0; i < s.RawNodeCount; i++)
                {
                    TransformData nData = new TransformData(s.RawStrokeNodes[i]);
                    m_RawStrokeNodeData[i] = nData;
                }
			}
			catch{
				return false;
			}
			return true;
        }
    }

    /// <summary>
    /// Holds data for the stroke nodes to be seriliazed and deserialized
    /// </summary>
    [System.Serializable]
    public class TransformData
    {
        public string m_PosData;
        public string m_RotData;
        public string m_ScaleData;

        public TransformData( Transform t)
        {
            m_PosData = t.localPosition.ToString("G3");
            m_RotData = t.localRotation.eulerAngles.ToString("G3");
            m_ScaleData = t.localScale.ToString("G3");
        }

        public TransformData( StrokeNode s )
        {
            m_PosData = s.OriginalPos.ToString("G3");
            m_RotData = s.OriginalRot.eulerAngles.ToString("G3");
            m_ScaleData = s.OriginalScale.ToString("G3");
        }

        public void Initialise(Transform t)
        {
            m_PosData = t.localPosition.ToString("G3");
            m_RotData = t.localRotation.eulerAngles.ToString("G3");
            m_ScaleData = t.localScale.ToString("G3");
        }

        public void SetTransfrom( Transform t )
        {
            t.localPosition = VectorExtensions.Parse(m_PosData);
            t.localRotation = Quaternion.Euler(VectorExtensions.Parse(m_RotData));
            t.localScale = VectorExtensions.Parse(m_ScaleData);
        }
    }    
}
