using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Painter3D
{
    public class StrokeRenderer_RibbonMesh : StrokeRenderer
    {       
        // Renderer, filter and mesh for displaying mesh
       // MeshRenderer m_MeshRenderer;
        MeshFilter m_Filter;
        Mesh m_Mesh;

        #region Mesh variables
        // Mesh variables as lists and arrays
        List<Vector3> m_VertList = new List<Vector3>();
        List<int> m_IndiciesList = new List<int>();
        List<Vector3> m_NormalsList = new List<Vector3>();
        List<Vector2> m_UVList = new List<Vector2>();

        Vector3[] m_Verts;
        int[] m_Indicies;
        Vector3[] m_Normals;
        Vector2[] m_UVs;
        Color[] m_Colors;           
        #endregion

        public Vector3 m_LineDirection = Vector3.right;
        float m_MasterScaler = 1;

        public AnimationCurve m_ProfileCurve;

        public override Renderer Renderer
        {
            get
            {
                if (m_Renderer == null)
                {
                    m_Renderer = gameObject.GetComponent<MeshRenderer>();
                    if (m_Renderer == null)
                    {
                        m_Renderer = gameObject.AddComponent<MeshRenderer>();
                        m_Renderer.material = m_Mat;
                        m_Renderer.material.SetCol(m_Color);
                    }
                }

                return m_Renderer;
            }
        }

        public override Material Material { get { return m_Mat; } }

        void Awake()
        {
            //if (m_Renderer == null)
            {
                m_Renderer = gameObject.GetComponent<MeshRenderer>();
                //m_Renderer = m_MeshRenderer;
                m_Filter = gameObject.GetComponent<MeshFilter>();

                if (m_Renderer == null)
                {
                    m_Renderer = gameObject.AddComponent<MeshRenderer>();
                    m_Filter = gameObject.AddComponent<MeshFilter>();
                }
            }
        }

       
        public override void DrawStroke(bool forceRedraw)
        {
			base.DrawStroke(forceRedraw);

            if (forceRedraw) m_PopulatedMatrixIndex = 0;           
                
            GenerateMeshFromStrokeNodes(m_Stroke);
            
            SetColour(m_Color);
        }

        void Update()
        {
            if (_props == null)
                _props = new MaterialPropertyBlock();

            // test mirror matrix
           // DrawMesh(-1, 1, 1);
        }

        MaterialPropertyBlock _props;
        void DrawMesh(float sx, float sy, float sz)
        {
            var scale = new Vector3(sx, sy, sz);
            var matrix = Matrix4x4.Scale(scale) * transform.localToWorldMatrix;

            Graphics.DrawMesh( m_Mesh, matrix, m_Mat, 0, null,0, _props);
        }

        public override void SetMaterial(Material mat)
        {
            base.SetMaterial(mat);

            m_Renderer.material = mat;
            m_Renderer.material.SetCol(m_Color);
        }        

        public override void SetRenderState(bool active)
        {
            base.SetRenderState(active);
            m_Renderer.enabled = active;
        }

        #region Mesh Generation

        void GenerateMeshFromStrokeNodes(Stroke stroke)
        {
            

            if (m_Mesh != null)
                m_Mesh.Clear();
            else
                m_Mesh = new Mesh();

            if (m_PopulatedMatrixIndex == 0)
            {
                m_VertList = new List<Vector3>();
                m_IndiciesList = new List<int>();
                m_NormalsList = new List<Vector3>();
                m_UVList = new List<Vector2>();
            }
            
            List<StrokeNode> strokeNodes = m_Stroke.SpacedStrokeNodes;
            int segmentCount = stroke.SpacedStrokeNodes.Count;
            int vertCount = 2 * strokeNodes.Count;
            int vertIndex = 0;
            int intIndex = 0;

            Vector3 scale;
            float profileCurveVal;
            Vector3 offsetDirection;
            Vector3 normal;
            Vector3 pos;

           // UnityEngine.Profiling.Profiler.BeginSample("draw ribbon");
            // Starting form the last populated matrix index, add new segment data
            for (int i = m_PopulatedMatrixIndex; i < segmentCount; ++i)
            {
                float t = (float)i / (segmentCount - 1);    // 0.0 -> 1.0
                
                t = Mathf.Clamp01(t);
                
                scale = strokeNodes[i].ModifiedScale;

                // Get the vert points
                profileCurveVal = 1;

                if( segmentCount > 2)
                    profileCurveVal = m_ProfileCurve.Evaluate(t);

                pos = strokeNodes[i].ModifiedPos;

                offsetDirection = m_LineDirection * scale.y * profileCurveVal * .25f;
                offsetDirection = strokeNodes[i].ModifiedRot * offsetDirection;
                m_VertList.Add(pos + offsetDirection);
                m_VertList.Add(pos - offsetDirection);

                // Get normals
                normal = Vector3.forward;
                m_NormalsList.Add(normal);
                m_NormalsList.Add(normal);

                float u = t;

                // Get UVs
                m_UVList.Add(new Vector2(u, 0));
                m_UVList.Add(new Vector2(u, 1));

                if (i > 0)
                {
                    m_IndiciesList.Add(vertIndex);
                    m_IndiciesList.Add(vertIndex - 2);
                    m_IndiciesList.Add(vertIndex - 1);

                    m_IndiciesList.Add(vertIndex);
                    m_IndiciesList.Add(vertIndex - 1);
                    m_IndiciesList.Add(vertIndex + 1);

                    intIndex = m_IndiciesList.Count;
                }

                vertIndex = m_VertList.Count;
            }
         //   UnityEngine.Profiling.Profiler.EndSample();

         //   UnityEngine.Profiling.Profiler.BeginSample("set ribbon");
            m_Colors = new Color[m_VertList.Count];
            for (int i = 0; i < m_Colors.Length; i++)
            {
                m_Colors[i] = m_Color;
            }

            m_Verts = m_VertList.ToArray();
            m_UVs = m_UVList.ToArray();
            m_Indicies = m_IndiciesList.ToArray();
            
            SetMeshFromData();
          //  UnityEngine.Profiling.Profiler.EndSample();
        }

        void SetMeshFromData()
        {
            m_Mesh.vertices = m_Verts;
            m_Mesh.uv = m_UVs;
            m_Mesh.triangles = m_Indicies;
            m_Mesh.colors = m_Colors;
            
            m_Mesh.RecalculateBounds();
            m_Mesh.RecalculateNormals();
            
            m_Filter.mesh = m_Mesh;
            m_Renderer.material = m_Mat;
        }

        static void CalculateNormalsManaged(Vector3[] verts, Vector3[] normals, int[] tris, Mesh mesh)
        {
            for (int i = 0; i < tris.Length; i += 3)
            {
                int tri0 = tris[i];
                int tri1 = tris[i + 1];
                int tri2 = tris[i + 2];
                Vector3 vert0 = verts[tri0];
                Vector3 vert1 = verts[tri1];
                Vector3 vert2 = verts[tri2];
                // Vector3 normal = Vector3.Cross(vert1 - vert0, vert2 - vert0);
                Vector3 normal = new Vector3()
                {
                    x = vert0.y * vert1.z - vert0.y * vert2.z - vert1.y * vert0.z + vert1.y * vert2.z + vert2.y * vert0.z - vert2.y * vert1.z,
                    y = -vert0.x * vert1.z + vert0.x * vert2.z + vert1.x * vert0.z - vert1.x * vert2.z - vert2.x * vert0.z + vert2.x * vert1.z,
                    z = vert0.x * vert1.y - vert0.x * vert2.y - vert1.x * vert0.y + vert1.x * vert2.y + vert2.x * vert0.y - vert2.x * vert1.y
                };
                normals[tri0] += normal;
                normals[tri1] += normal;
                normals[tri2] += normal;
            }

            for (int i = 0; i < normals.Length; i++)
            {
                // normals [i] = Vector3.Normalize (normals [i]);
                Vector3 norm = normals[i];
                float invlength = 1.0f / (float)System.Math.Sqrt(norm.x * norm.x + norm.y * norm.y + norm.z * norm.z);
                normals[i].x = norm.x * invlength;
                normals[i].y = norm.y * invlength;
                normals[i].z = norm.z * invlength;
            }

            mesh.normals = normals;
        }
        #endregion     
    }
}
