using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Painter3D
{
    /// <summary>
    /// Canvas hold strokes that brushes draw to it
    /// </summary>
    public class Canvas : MonoBehaviour
    {
        #region Variables
        // The strokes that the canvas contains
        public List<Stroke> m_Strokes = new List<Stroke>();
        public List<Stroke> Strokes { get { return m_Strokes; } }

        // How far strokes are offset to avoid z fighting if using a 2D painting plane. Not needed in 3D
        float m_StrokeOffset = -.002f;

        // Canvas trasform properties
        bool m_MovingCanvas = false;
        TransformData m_PrevCanvasPos;
        Transform m_CanvasParent;

        #endregion
        
        void Start()
        {
            m_CanvasParent = transform.parent;
        }

        // Move canvas
        public void BeginMoveCanvas( Transform t )
        {
            // Set moving canvas to true
            m_MovingCanvas = true;

            // store original pos
            m_PrevCanvasPos = new TransformData(transform);

            transform.SetParent(t);
        }        

        public void EndCanvasMove()
        {
            // Set moving canvas to true
            m_MovingCanvas = false;

            transform.SetParent(m_CanvasParent);
        }

        public void Scale( float amount )
        {
            transform.localScale += Vector3.one * amount;
        }

        #region Add, remove, destroy, clear
        // Add stroke and parent to this transform
        public void AddStroke(Stroke s)
        {
            //print("Adding stroke to canvas. Stroke count: " + m_Strokes.Count);

            // Name stroke with index
            s.name = "Stroke " + m_Strokes.Count;

            // Add stroke to the list and set parent to canvas
            m_Strokes.Add(s);
            s.transform.SetParent(transform);

            // Turn this on if you want to keep brush stroks consistent to canvas scale rather than world
            //s.transform.localScale = Vector3.one;

            Painter3DManager.Instance.ClearRedo();

            // Offset stroke if using the stroke offset
            if (Painter3DManager.Instance.m_UseStrokeOffset)
                s.transform.position = new Vector3(0, 0, m_StrokeOffset * m_Strokes.Count);
            else
                s.transform.localPosition = Vector3.zero;
        }

        public void RemoveUndoStroke(Stroke s)
        {
            m_Strokes.Remove(s);
        }

        public void AddRedoStroke(Stroke s)
        {
            m_Strokes.Add(s);
        }

        public void RemoveAndDestroyStroke( int index )
        {
            Stroke s = m_Strokes[index];
            m_Strokes.RemoveAt(index);
            Destroy(s.gameObject);
        }

        public void RemoveAndDestroyLastStroke()
        {
            RemoveAndDestroyStroke(m_Strokes.Count - 1);
        }

        public void Clear()
        {
            m_Strokes.RemoveAll(item => item == null);

            for (int i = 0; i < m_Strokes.Count; i++)
            {
                if( m_Strokes[i].gameObject != null )
                    Destroy(m_Strokes[i].gameObject);
            }

            m_Strokes.Clear();
        }
        #endregion

        #region Serialization
        public CanvasData GetCanvasData()
        {           
            CanvasData cd = new CanvasData();
            cd.Initialise(this);
            return cd;
        }
        
        public void LoadCanvas(CanvasData c)
        {
            // Clear canvas before load
            Clear();

            print("Loading canvas with stroke count of: " + c.m_StrokesData.Length );

            for (int i = 0; i < c.m_StrokesData.Length; i++)
            {
                // Get the stroke data
                StrokeData sData = c.m_StrokesData[i];

                // Find an instantiate stroke renderer
                StrokeRenderer sRend = Painter3DResourceManager.Instance.GetStrokeRenderer(sData.m_RendererName);

                Color col = VectorExtensions.ParseVec4(sData.m_Color);
                // Create new stroke
                Stroke s = Stroke.GetNewStroke(sRend, Painter3DManager.Instance.m_PaintingLayer, float.Parse(sData.m_Scale), col);

                // Set stroke transform
                sData.m_TransformData.SetTransfrom(s.transform);
                             

                /*
                // Create new stroke
                Stroke s = new GameObject("Stroke").AddComponent<Stroke>();

                // Set stroke transform
                sData.m_TransformData.SetTransfrom(s.transform);

                // Find an instantiate stroke renderer
                StrokeRenderer sRend = Painter3DResourceManager.Instance.GetStrokeRenderer(sData.m_RendererName);

                // Initialize the stroke
                s.Init(sRend, Painter3DManager.Instance.m_PaintingLayer, float.Parse(sData.m_Scale));
                */

                // Add stroke to canvas
                AddStroke(s);

                // Load stroke data
                s.LoadFromData(sData);
            }
        }
        

        public void LoadCanvasOverTime(CanvasData c, float duration)
        {
            StartCoroutine(LoadCanvasOverTimeRoutine(c, duration));
        }

        IEnumerator LoadCanvasOverTimeRoutine(CanvasData c, float duration)
        {
            // Clear canvas before load
            Clear();

            float frameDuration = duration / c.m_StrokesData.Length;

            for (int i = 0; i < c.m_StrokesData.Length; i++)
            {
                // Get the stroke data
                StrokeData sData = c.m_StrokesData[i];

                // Find an instantiate stroke renderer
                StrokeRenderer sRend = Painter3DResourceManager.Instance.GetStrokeRenderer(sData.m_RendererName);
                sRend = Instantiate(sRend);

                Color col = VectorExtensions.ParseVec4(sData.m_Color);

                // Create new stroke
                Stroke s = Stroke.GetNewStroke(sRend, Painter3DManager.Instance.m_PaintingLayer, float.Parse(sData.m_Scale), col);

                // Add stroke to canvas
                AddStroke(s);

                // Set stroke transform
                sData.m_TransformData.SetTransfrom(s.transform);

                // Load stroke data
                //s.LoadFromData(sData);

                // TESTING - loading over time
                s.LoadFromData(sData, frameDuration);

                // wait for end of frame
                yield return new WaitForSeconds(frameDuration);
            }
        }
        #endregion
    }
}