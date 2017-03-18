using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Painter3D
{
    public class Painter3DResourceManager : MonoBehaviour
    {
        public static Painter3DResourceManager Instance;
        public delegate void ActiveStrokeChanged(int index, Color col);
        public ActiveStrokeChanged OnActiveStrokeChanged;
        
        #region Stroke renderers
        // All the stroke renderers
        [SerializeField]
        StrokeRenderer[] m_AllStrokeRenderers;
        public StrokeRenderer[] AllStrokeRenderers { get { return m_AllStrokeRenderers; } }

        // The currently seelcted stroke renderer
        public int m_ActiveStokeRendererIndex = 0;
        public StrokeRenderer ActiveStrokeRenderer { get { return m_AllStrokeRenderers[m_ActiveStokeRendererIndex]; } }
        #endregion
        
        public Material[] m_AllMaterials;
        
        void Awake()
        {
            Instance = this;         
        }

        void Start()
        {
            List<Material> mats = new List<Material>();
       
            for (int i = 0; i< m_AllMaterials.Length; i++)
			{
                mats.Add(m_AllMaterials[i]);
            }

            for (int i = 0; i < m_AllStrokeRenderers.Length; i++)
            {
                if (!mats.Contains(m_AllStrokeRenderers[i].Material))
                    mats.Add(m_AllStrokeRenderers[i].Material);
            }

            m_AllMaterials = mats.ToArray();
        }
        
        public StrokeRenderer GetActiveStrokeRenderer()
        {
            return GetStrokeRenderer(m_ActiveStokeRendererIndex);
        }

        public StrokeRenderer GetStrokeRenderer(int index)
        {
            // Instantiate the renderer and return it
            StrokeRenderer renderer = Instantiate(m_AllStrokeRenderers[index]);
            return renderer;
        }

        public StrokeRenderer GetStrokeRenderer(string name)
        {
            for (int i = 0; i < AllStrokeRenderers.Length; i++)
            {
                if (AllStrokeRenderers[i].name == name)
                {
                    return GetStrokeRenderer(i);
                }
            }

            return GetStrokeRenderer(0);
        }

        public void SetActiveStroke(int index)
        {
            // Set the active stroke colour
            Color col = ActiveStrokeRenderer.m_Color;

            // Clamp the index
            m_ActiveStokeRendererIndex = m_ActiveStokeRendererIndex % m_AllStrokeRenderers.Length;

            // Set the index
            m_ActiveStokeRendererIndex = index;

            if(OnActiveStrokeChanged != null)
            {
                OnActiveStrokeChanged(index, col);
            }
        }

        public StrokeRenderer GetNextStrokeRenderer()
        {
            m_ActiveStokeRendererIndex++;
            if (m_ActiveStokeRendererIndex >= m_AllStrokeRenderers.Length) m_ActiveStokeRendererIndex = 0;

            // Set the active stroke colour
            Color col = ActiveStrokeRenderer.m_Color;

            if (OnActiveStrokeChanged != null)
            {
                OnActiveStrokeChanged(m_ActiveStokeRendererIndex, col);
            }

            return GetActiveStrokeRenderer();
        }

        public StrokeRenderer GetPrevStrokeRenderer()
        {
            m_ActiveStokeRendererIndex--;
            if (m_ActiveStokeRendererIndex < 0) m_ActiveStokeRendererIndex = m_AllStrokeRenderers.Length - 1;

            // Set the active stroke colour
            Color col = ActiveStrokeRenderer.m_Color;

            if (OnActiveStrokeChanged != null)
            {
                OnActiveStrokeChanged(m_ActiveStokeRendererIndex, col);
            }

            return GetActiveStrokeRenderer();
        }
    }
}
