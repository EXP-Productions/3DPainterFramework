using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.EventSystems;

namespace Painter3D
{
    /// <summary>
    /// Painter manager controls the painting 3D experience
    /// </summary>
    public class Painter3DManager : MonoBehaviour
    {
        #region Variables
        public delegate void InBoundsEvent( bool inbounds);
        public InBoundsEvent OnInBoundsEvent;

        public static Painter3DManager Instance;

        public Painter3DResourceManager m_ResourceManager;

        public Brush m_ActiveBrush;
        public int m_PaintingLayer = 8;
                
        bool m_UseRedo = true;
        
        public GameObject m_PCCam;

        public GameObject m_EventSystem;

        [Header("Serialization")]
        public int m_SaveIndex = 0;
        public int m_CurrentLoadedIndex = 0;
        public int m_MaxSaveIndex = 0;

        public KeyCode m_ModifierKey = KeyCode.LeftShift;

        #region Canvases
        // Canvas list and active canvas
        List<Canvas> m_AllCanvases = new List<Canvas>();
        int m_ActiveCanvasIndex = 0;
        Canvas m_ActiveCanvas;
        public Canvas ActiveCanvas { get { return m_ActiveCanvas; } set { m_ActiveCanvas = value; } }
        public List<Canvas> AllCanvases { get { return m_AllCanvases; } }
        Transform m_CanvasParent;
        public Transform CanvasParent { get { return m_CanvasParent; } }
        #endregion       

        #region Undo/Redo
        Stack<Stroke> m_ActiveStrokeStack = new Stack<Stroke>();
        Stack<Stroke> m_UndoStack = new Stack<Stroke>();
        #endregion

        // A collider that defines the area you can paint on
        public Collider m_CanvasCollider;

        public int m_UndoCount;
        public int m_ColourCount;
        public int m_StrokeCount;

        //debug
        public bool m_UseStrokeOffset = false;
        
        // Flag for accepting input, ends stroke of input is turned off
        public bool m_InputActive = true;
        public bool InputActive
        {
            get { return m_InputActive; }
            set
            {
                if (!value && m_InputActive)
                    m_ActiveBrush.EndStroke();

                m_InputActive = value;
            }
        }
        #endregion
        
        // Use this for initialization
        void Awake()
        {
            // Set the singleton instance
            Instance = this;
            
            // Create a canvas parent
            m_CanvasParent = new GameObject("_Canvas parent").transform;
            m_CanvasParent.SetParent(transform);
           

            // Set the onstroke complete event 
            m_ActiveBrush.OnStrokeCompleteEvent += ActiveBrush_OnStrokeCompleteEvent;
            
            // Create a new canvas
            CreateNewCanvas();

            m_MaxSaveIndex = PlayerPrefs.GetInt("m_MaxSaveIndex", 0);
            m_CurrentLoadedIndex = 0;
            m_MaxSaveIndex = m_SaveIndex;
        }
        
        
        // Update is called once per frame
        void Update()
        {
            #region Keyboard input
            // Clear all
            if (Input.GetKey(m_ModifierKey) && Input.GetKeyDown(KeyCode.C))
            {
                ClearAll();
            }

            // Quit application
            if (Input.GetKey(m_ModifierKey) && Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            #endregion
        }
        
        GameObject CreateNewCanvas()
        {
            Canvas newCanvas = new GameObject("_Canvas " + m_AllCanvases.Count).AddComponent<Canvas>();
            newCanvas.transform.SetParent(m_CanvasParent);
            m_AllCanvases.Add(newCanvas);

            ActiveCanvas = newCanvas;

            return newCanvas.gameObject;
        }

        void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("m_MaxSaveIndex", m_MaxSaveIndex);
        }

        #region Serialization
        public void LoadNext()
        {
            print("Load next  " + m_CurrentLoadedIndex + "    " + m_MaxSaveIndex);
            if (m_CurrentLoadedIndex >= m_MaxSaveIndex)
                m_CurrentLoadedIndex = 0;
            
            m_CurrentLoadedIndex++;
            
            Load(Application.streamingAssetsPath + "\\SplashSave " + m_CurrentLoadedIndex.ToString() + ".txt");
        }

        void Load(string filename)
        {
            try
            {   // Read JSON from text file
                string jsonPayload = System.IO.File.ReadAllText(filename);

                CanvasData canvasData = new CanvasData();
                canvasData = (CanvasData)JsonConvert.DeserializeObject(jsonPayload, canvasData.GetType());

                m_ActiveCanvas.LoadCanvas(canvasData);         
            }
            catch { }
        }

        public void SaveAndClear()
        {
            print("Save and clear");            
            Save(Application.streamingAssetsPath + "\\SplashSave " + m_SaveIndex.ToString() + ".txt");
            m_SaveIndex++;
            if (m_SaveIndex > m_MaxSaveIndex) m_MaxSaveIndex = m_SaveIndex;
            ClearAll();
        }

        public void Save(string filename)
        {
            // Create pay load, pass in canvas data and write
            string payload = "";
            payload = JsonConvert.SerializeObject(m_ActiveCanvas.GetCanvasData());
            System.IO.File.WriteAllText(filename, payload);
        }
        #endregion
         
        #region Undo Redo Clear
        private void ActiveBrush_OnStrokeCompleteEvent(Stroke stroke)
        {
            m_StrokeCount++;
            // Push the completed stroke to the active stroke stack
            m_ActiveStrokeStack.Push(stroke);
        }

        public void UndoLastStroke()
        {
            if (m_ActiveStrokeStack.Count > 0)
            {
                // pops the latest stroke from the stack
                Stroke stroke = m_ActiveStrokeStack.Pop();
                m_UndoCount++;

                if (m_UseRedo)
                { 
                    m_UndoStack.Push(stroke);  
                    // Stop stroke rendering
                    stroke.m_StrokeRenderer.SetRenderState(false);
                    m_ActiveCanvas.RemoveUndoStroke(stroke);
                }
                else
                {
                    m_ActiveCanvas.RemoveAndDestroyLastStroke();
                }
            }
        }

        public void Redo()
        {
            if (m_UndoStack.Count > 0)
            {
                Stroke stroke = m_UndoStack.Pop();
                m_ActiveStrokeStack.Push(stroke);

                m_ActiveCanvas.AddRedoStroke(stroke);

                // Start stroke rendering
                stroke.m_StrokeRenderer.SetRenderState(true);
            }
        }

        public void ClearRedo()
        {
            foreach (Stroke s in m_UndoStack)
                Destroy(s.gameObject);

            m_UndoStack.Clear();
        }

        public void ClearAll()
        {
            for (int i = 0; i < m_AllCanvases.Count; i++)
            {
                m_AllCanvases[i].Clear();
                m_ActiveStrokeStack.Clear();
                m_UndoStack.Clear();
            }
        }
        #endregion        
    }
}
