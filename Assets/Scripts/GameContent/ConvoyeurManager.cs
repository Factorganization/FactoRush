using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace GameContent
{
    public class ConveyorPlacement3D : MonoBehaviour
    {
        public GameObject conveyorPrefab;   // The conveyor prefab
        public float gridUnitSize = 0.5f;   // Size of the invisible grid
        public Camera isoCamera;            // Reference to the isometric camera
        private Vector3 startDragPosition;
        private bool isDragging = false;
        private List<GameObject> previewConveyors = new List<GameObject>(); // List for temporary previews
        public float conveyorYPosition = 0f; // Fixed Y position for conveyors

        private Vector3 lastSnappedPosition; // To keep track of the last snapped position
        private List<Vector3> conveyorPositions = new List<Vector3>(); // To store the positions of placed conveyors

        private void Awake()
        {
            EnhancedTouchSupport.Enable();
        }

        private void Update()
        {
            if (Touch.activeTouches.Count <= 0)
                return;
        
            var touch = Touch.activeTouches[0]; 

            var ray = isoCamera.ScreenPointToRay(touch.screenPosition);
            RaycastHit hit;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (Physics.Raycast(ray, out hit))
                    {
                        startDragPosition = hit.point;
                        startDragPosition.y = conveyorYPosition;
                        isDragging = true;
                        conveyorPositions.Clear();
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            var currentTouchPosition = hit.point;
                            currentTouchPosition.y = conveyorYPosition;

                            var snappedPosition = SnapToGrid(currentTouchPosition);
                            UpdateConveyorPreview(snappedPosition);
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    if (isDragging)
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            PlaceConveyors();
                        }
                        isDragging = false;
                    }
                    break;
            
                case TouchPhase.None:
                case TouchPhase.Canceled:
                case TouchPhase.Stationary:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateConveyorPreview(Vector3 snappedPosition)
        {
            foreach (var conveyor in previewConveyors)
            {
                Destroy(conveyor);
            }
            previewConveyors.Clear();

            if (lastSnappedPosition != snappedPosition)
            {
                conveyorPositions.Add(snappedPosition);
                lastSnappedPosition = snappedPosition;
            }

            foreach (var position in conveyorPositions)
            {
                var previewConveyor = Instantiate(conveyorPrefab, position, Quaternion.identity);
                previewConveyor.transform.localScale = new Vector3(1, 1, 1);
                previewConveyors.Add(previewConveyor);
            }
        }

        private void PlaceConveyors()
        {
            foreach (var position in conveyorPositions)
            {
                Instantiate(conveyorPrefab, position, Quaternion.identity);
            }
            conveyorPositions.Clear();
        }

        private Vector3 SnapToGrid(Vector3 targetPosition)
        {
            var snappedX = Mathf.Round(targetPosition.x / gridUnitSize) * gridUnitSize;
            var snappedZ = Mathf.Round(targetPosition.z / gridUnitSize) * gridUnitSize;

            var snappedPosition = new Vector3(snappedX, conveyorYPosition, snappedZ);

            if (conveyorPositions.Count <= 0)
                return snappedPosition;
        
            var lastPosition = conveyorPositions[^1];
            if (Mathf.Abs(snappedX - lastPosition.x) < gridUnitSize && Mathf.Abs(snappedZ - lastPosition.z) < gridUnitSize)
            {
                return lastPosition; 
            }

            return snappedPosition;
        }
    }
}
