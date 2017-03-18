using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Painter3D
{
    /// <summary>
    /// TODO
    /// - Add 3D rotation?
    /// </summary>
    public class StrokeRenderer_ParticleSystem : StrokeRenderer
    {
        [Header("Particle System")]
        public ParticleSystem m_PSys;
               
        ParticleSystem.EmitParams m_EmitParams;
        public bool m_3DRotation = false;

        public bool m_Trails = false;
        public float m_TrailLife = 1;

        float m_ParticleLife = 1000000;

        public override Renderer Renderer
        {
            get
            {
               if( m_Renderer == null )
                {
                    m_Renderer = m_PSys.GetComponent<Renderer>();
                }

                return m_Renderer;
            }
        }

        public override Material Material { get { return Renderer.sharedMaterial; } }

        void Awake()
        {
            m_Renderer = m_PSys.GetComponent<Renderer>();
        }
        
        public override void Initialize(Stroke s, int layer, Color col)
        {
            base.Initialize(s, layer, col);

            m_EmitParams = new ParticleSystem.EmitParams();
            m_EmitParams.startLifetime = 1000000;
            m_EmitParams.startColor = m_Color;
            m_PSys.scalingMode = ParticleSystemScalingMode.Hierarchy;

            var trails = m_PSys.trails;
            trails.enabled = m_Trails;
            trails.lifetimeMultiplier = m_TrailLife / m_EmitParams.startLifetime;
            trails.minVertexDistance = .01f;
            trails.ratio = 1;

            m_PSys.gameObject.layer = layer;
        }
       
        public override void DrawStroke(bool forceRedraw)
        {            
            // If there is no stroke then return
            if (m_Stroke == null || m_Stroke.SpacedStrokeNodes.Count < 2) return;

            base.DrawStroke(forceRedraw);
            
          
            if (m_PopulatedMatrixIndex != m_Stroke.SpacedStrokeNodes.Count)
            {                  
                m_PSys.Clear();
                m_PopulatedMatrixIndex = 0;

                List<StrokeNode> nodes = m_Stroke.SpacedStrokeNodes;
                for (int i = m_PopulatedMatrixIndex; i < nodes.Count; i++)
                {
                    EmitAtStrokeNode(nodes[i], i);
                }

                m_PopulatedMatrixIndex = nodes.Count;
            }
            else if (forceRedraw)
            {
                UpdatePartilcePositions();
            }
                                
        }

        ParticleSystem.Particle[] m_Particles;
        void UpdatePartilcePositions()
        {
            m_Particles = new ParticleSystem.Particle[m_PSys.particleCount];
            // GetParticles is allocation free because we reuse the m_Particles buffer between updates
            int numParticlesAlive = m_PSys.GetParticles(m_Particles);

            for (int i = 0; i < numParticlesAlive; i++)
            {
                m_Particles[i].position = m_Stroke.SpacedStrokeNodes[i].ModifiedPos;
                
            }

            m_PSys.SetParticles(m_Particles, numParticlesAlive);
        }

        public override void SetMaterial(Material mat)
        {
            base.SetMaterial(mat);

            m_Renderer.material = mat;
        }

        public override void SetColour(Color col)
        {
            base.SetColour(col);

            ParticleSystem.Particle[] m_Particles;
            m_Particles = new ParticleSystem.Particle[m_PSys.particleCount];
            // GetParticles is allocation free because we reuse the m_Particles buffer between updates
            int numParticlesAlive = m_PSys.GetParticles(m_Particles);

            for (int i = 0; i < numParticlesAlive; i++)
            {
                m_Particles[i].startColor = m_Color;
                m_Particles[i].color = m_Color;
            }

            m_PSys.SetParticles(m_Particles, numParticlesAlive);
        }
        

        public void EmitAtStrokeNode(StrokeNode node, int randomSeed)
        {
            m_EmitParams.position = node.ModifiedPos;

            // Rotation 
            if (m_3DRotation)
                m_EmitParams.rotation3D = node.ModifiedRot.eulerAngles;
            else
                m_EmitParams.rotation = -node.ModifiedRot.eulerAngles.z;

            // Scale
            float scale = node.ModifiedScale.x;
            m_EmitParams.startSize = scale;

            m_PSys.Emit(m_EmitParams, 1);
        }


        public override void SetRenderState(bool active)
        {
            base.SetRenderState(active);
            m_Renderer.enabled = active;
        }

        // Make surew that the particles repopulate once they are turned off and back on
        void OnEnable()
        {
            DrawStroke(true);
        }

        void OnDrawGizmos()
        {
            if (m_Debug)
            {
                m_Particles = new ParticleSystem.Particle[m_PSys.particleCount];
                // GetParticles is allocation free because we reuse the m_Particles buffer between updates
                int numParticlesAlive = m_PSys.GetParticles(m_Particles);

                for (int i = 0; i < numParticlesAlive; i++)
                {
                    Gizmos.DrawSphere(m_Particles[i].position, m_Particles[i].GetCurrentSize(m_PSys) / 4f);
                }
            }
        }
    }
}
