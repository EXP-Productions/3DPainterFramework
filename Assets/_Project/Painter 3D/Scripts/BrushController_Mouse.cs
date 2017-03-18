using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Painter3D
{
    public class BrushController_Mouse : MonoBehaviour
    {
        public Camera m_Cam;        
        public Brush m_Brush;
        public Transform m_BrushTip;
        public bool m_UpdateFacing = false;
        float m_Depth = 3;
        public float m_Smoothing = 2;

        void Start()
        {
            m_BrushTip = transform;
        }

        // Update is called once per frame
        void Update()
        {          
            m_Brush.m_InputOverUI = EventSystem.current.IsPointerOverGameObject();

            UpdateBrushTipTransformFromMouse();

            if (Input.GetMouseButtonDown(0))
            {
                m_Brush.BeginStroke(transform);
            }
            else if (Input.GetMouseButton(0) && m_Brush.Painting)
            {
                m_Brush.UpdateStroke();
            }
            else if (Input.GetMouseButtonUp(0) && m_Brush.Painting)
            {
                m_Brush.EndStroke();
            }


            // Move canvas
            if (Input.GetMouseButtonDown(1))
            {
                m_UpdateFacing = false;
                Painter3DManager.Instance.ActiveCanvas.BeginMoveCanvas(m_BrushTip);
            }
            else if(Input.GetMouseButton(1))
            {
                

            }
            else if (Input.GetMouseButtonUp(1))
            {
                m_UpdateFacing = true;
                Painter3DManager.Instance.ActiveCanvas.EndCanvasMove();
            }

            if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0 )
            {
                Painter3DManager.Instance.ActiveCanvas.Scale(Input.GetAxis("Mouse ScrollWheel"));
            }
        }

        void UpdateBrushTipTransformFromMouse()
        {
            Vector3 targetPos = m_Cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_Depth));

            if (m_Smoothing != 0)
                targetPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * m_Smoothing);

            // Update the rotation of the brush tip
            if (m_UpdateFacing && Vector3.Distance(targetPos, m_BrushTip.position) > .01f)
            {
                UpdateTipAngleOnXY(m_BrushTip.transform.position, targetPos);
            }

            transform.position = targetPos;
        }
        
        void UpdateTipAngleOnXY(Vector3 currentPos, Vector3 targetPos)
        {
            var newRotation = Quaternion.LookRotation(currentPos - targetPos, Vector3.forward);
            newRotation.x = 0.0f;
            newRotation.y = 0.0f;
            m_BrushTip.rotation = Quaternion.Slerp(m_BrushTip.transform.rotation, newRotation, 1);
        }
    }
}
