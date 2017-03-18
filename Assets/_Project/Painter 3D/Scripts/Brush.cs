using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.EventSystems;

namespace Painter3D
{
    /// <summary>
    /// Brushes draw strokes (a collection of 4x4 matracies) to a canvas
    /// </summary>
    public class Brush : MonoBehaviour
    {
        public enum PaintMode
        {
            TwoDimensional,
            ThreeDimensional,
        }

        [SerializeField]
        private PaintMode m_PaintMode = PaintMode.ThreeDimensional;

        // Stroke complete event
        public delegate void OnStrokeComplete(Stroke stroke);
        public event OnStrokeComplete OnStrokeCompleteEvent;

        // Reference to the paint manager
        public Painter3DManager m_PaintManager;

        // The canvas that the brush is drawing too
        public Canvas ActiveCanvas { get { return m_PaintManager.ActiveCanvas; } }

        #region Brush Tip
        // The transform the the brush guides
        private Transform m_BrushTip;       

        // The size of the brush tip
        float m_BrushSize = .25f;
        public float BrushSize {
            get { return m_BrushSize; }
            set { m_BrushSize = Mathf.Max(value, .01f); if(m_BrushTip != null) m_BrushTip.localScale = Vector3.one * value * .5f; }
        }
        #endregion

        public bool m_InputOverUI = false;

        // The previous node position
        Vector3 m_PrevNodePos;

        // The currently being drawn
        Stroke m_ActiveStroke;
        public Stroke ActiveStroke { get { return m_ActiveStroke; } }

        // The amount of spacing between the nodes in cm
        public float m_MinNodeSpacing = .01f;

        // Brush Colour
        Color m_Colour = Color.black;

        // Objects that show where the brush will draw, what size and colour
        public MeshRenderer[] m_BrushReticleObjects;

        [SerializeField]
        // Is the brush currently painting
        bool m_Painting = false;
        public bool Painting { get { return m_Painting; } }

        #region 2D painting variable
        // Offsets brush tip per stroke. Usefeul for painting on a 2d plane so that the strokes get layered and don't zfight
        bool m_UseOffsetPerStroke = false;
        public float m_DepthOffsetPerStroke = -.006f;
        #endregion
        
        // Use this for initialization
        void Awake()
        {
            m_PaintManager = FindObjectOfType<Painter3DManager>();           
        }

        void Start()
        {
            BrushSize = m_BrushSize;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_PaintManager.InputActive)
            {
               
            }
            else
            {
                if (m_Painting) EndStroke();
            }
        }

        void SetCol(Color col)
        {
            m_Colour = col;

            for (int i = 0; i < m_BrushReticleObjects.Length; i++)
            {
                m_BrushReticleObjects[i].material.SetCol(col);
            }
        }
              
        #region Brush begin update and end
        public Stroke BeginStroke(Transform brushTip)
        {
            m_BrushTip = brushTip;

            // Check if mouse is over UI           
            if (m_InputOverUI) // EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Tried to begin stroke while over UI");
                return null;
            }            

            // Set painting flag
            m_Painting = true;
            
            // create stroke
            //m_ActiveStroke = new GameObject("Stroke").AddComponent<Stroke>();
            //m_ActiveStroke.Init(Instantiate(Painter3DResourceManager.Instance.ActiveStrokeRenderer), m_PaintManager.m_PaintingLayer, m_BrushSize);

            //Test
            m_ActiveStroke = Stroke.GetNewStroke(Instantiate(Painter3DResourceManager.Instance.ActiveStrokeRenderer), m_PaintManager.m_PaintingLayer, m_BrushSize, m_Colour);

            // Add stroke to canvas
            m_PaintManager.ActiveCanvas.AddStroke(m_ActiveStroke);

            // Begin stroke
            m_ActiveStroke.BeginStroke(brushTip);
            
            // Set prev node pos
            m_PrevNodePos = brushTip.position;
            
            return m_ActiveStroke;
        }
               
        public void UpdateStroke()
        {
            Profiler.BeginSample("Update stroke");
            if (m_Painting)
            {
                if (Vector3.Distance(m_BrushTip.position, m_PrevNodePos) > m_MinNodeSpacing)
                {
                    m_ActiveStroke.UpdateStroke(m_BrushTip);
                    m_PrevNodePos = m_BrushTip.position;
                }
            }
            Profiler.EndSample();
        }

        public void EndStroke()
        {
            if (m_Painting)
            {
                m_ActiveStroke.EndStroke(m_BrushTip);
                m_Painting = false;

                if(OnStrokeCompleteEvent!=null)OnStrokeCompleteEvent(m_ActiveStroke);

                /*
                // Debug get mirror
                m_ActiveStroke = m_ActiveStroke.GetMirrorStroke(true, false, false);
                m_PaintManager.ActiveCanvas.AddStroke(m_ActiveStroke);
                if (OnStrokeCompleteEvent != null) OnStrokeCompleteEvent(m_ActiveStroke);
                */
            }
        }
        #endregion

        private void OnDrawGizmos()
        {
           // m_BrushTip.transform
        }
    }
}
