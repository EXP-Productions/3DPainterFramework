using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Painter3D
{
    public class StrokeRenderer_LineRenderer : StrokeRenderer
    {
        public LineRenderer m_LineRenderer;

        public override void DrawStroke(bool forceRedraw)
        {
            base.DrawStroke(forceRedraw);

            // Set line renderer number of positions
            m_LineRenderer.numPositions = m_Stroke.RawNodeCount;

            //float size = m_Stroke.RawMatracies[0].GetScale().x.ScaleFrom01(m_ScaleRange.x, m_ScaleRange.y);

            m_LineRenderer.startWidth = AdjustedScale;
            m_LineRenderer.endWidth = AdjustedScale;

            for (int i = 0; i < m_Stroke.RawNodeCount; i++)
            {
                m_LineRenderer.SetPosition(i, m_Stroke.GetPositionAt(i));                
            }
        }

        public override void SetMaterial(Material mat)
        {
            base.SetMaterial(mat);

            m_LineRenderer.material = mat;
        }

        public override void SetColour(Color col)
        {
            base.SetColour(col);

            m_LineRenderer.startColor = col;
            m_LineRenderer.endColor = col;
        }

        public override void SetRenderState(bool active)
        {
            m_LineRenderer.enabled = active;
        }
    }
}
