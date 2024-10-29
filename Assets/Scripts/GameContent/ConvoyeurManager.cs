using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

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

    void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    void Update()
    {
        // Check if there are any touch inputs
        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0]; 

            // Create a ray from the touch position
            Ray ray = isoCamera.ScreenPointToRay(touch.screenPosition);
            RaycastHit hit;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Initialize the drag start point
                    if (Physics.Raycast(ray, out hit))
                    {
                        startDragPosition = hit.point;
                        startDragPosition.y = conveyorYPosition; // Set Y to the fixed conveyor Y position
                        isDragging = true;
                        conveyorPositions.Clear(); // Clear previous positions
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        // Update conveyor previews
                        if (Physics.Raycast(ray, out hit))
                        {
                            Vector3 currentTouchPosition = hit.point;
                            currentTouchPosition.y = conveyorYPosition; // Set Y to the fixed conveyor Y position

                            // Get the snapped position and create preview conveyors
                            Vector3 snappedPosition = SnapToGrid(currentTouchPosition);
                            UpdateConveyorPreview(snappedPosition);
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    if (isDragging)
                    {
                        // Final placement of conveyors
                        if (Physics.Raycast(ray, out hit))
                        {
                            Vector3 endPosition = hit.point;
                            endPosition.y = conveyorYPosition; // Set Y to the fixed conveyor Y position
                            PlaceConveyors();
                        }
                        isDragging = false;
                    }
                    break;
            }
        }
    }

    void UpdateConveyorPreview(Vector3 snappedPosition)
    {
        // Clear previous preview conveyors
        foreach (GameObject conveyor in previewConveyors)
        {
            Destroy(conveyor);
        }
        previewConveyors.Clear();

        // Add the new snapped position if it's different from the last one
        if (lastSnappedPosition != snappedPosition)
        {
            conveyorPositions.Add(snappedPosition); // Store the position
            lastSnappedPosition = snappedPosition; // Update the last snapped position
        }

        // Create preview conveyors for each position stored
        foreach (Vector3 position in conveyorPositions)
        {
            GameObject previewConveyor = Instantiate(conveyorPrefab, position, Quaternion.identity);
            previewConveyor.transform.localScale = new Vector3(1, 1, 1); // Adjust scale if necessary
            previewConveyors.Add(previewConveyor);
        }
    }

    void PlaceConveyors()
    {
        // Finalize the placement of conveyors based on stored positions
        foreach (Vector3 position in conveyorPositions)
        {
            Instantiate(conveyorPrefab, position, Quaternion.identity);
        }
        // Clear the positions after placement
        conveyorPositions.Clear();
    }

    Vector3 SnapToGrid(Vector3 targetPosition)
    {
        // Snap to the nearest grid point
        float snappedX = Mathf.Round(targetPosition.x / gridUnitSize) * gridUnitSize;
        float snappedZ = Mathf.Round(targetPosition.z / gridUnitSize) * gridUnitSize;

        // Create the new snapped position
        Vector3 snappedPosition = new Vector3(snappedX, conveyorYPosition, snappedZ);

        // Ensure the new position is either in line with the last one or is a 90-degree turn
        if (conveyorPositions.Count > 0)
        {
            Vector3 lastPosition = conveyorPositions[conveyorPositions.Count - 1];
            if (Mathf.Abs(snappedX - lastPosition.x) < gridUnitSize && Mathf.Abs(snappedZ - lastPosition.z) < gridUnitSize)
            {
                // Close enough to the last position, do not add a new conveyor
                return lastPosition; 
            }
        }

        return snappedPosition;
    }
}
