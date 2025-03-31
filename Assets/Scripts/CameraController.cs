using UnityEngine;

public class CameraController : MonoBehaviour
{
       [Header("Movement Settings")]
    [SerializeField] private float edgeScrollSpeed = 15f;
    [SerializeField] private float scrollAreaThickness = 15f; // How many pixels from the edge of the screen
    
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 50f;
    
    [Header("Boundaries")]
    [SerializeField] private bool useBoundaries = true;
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;
    
    [Header("Controls")]
    [SerializeField] private bool enableEdgeScrolling = true;
    [SerializeField] private bool enableKeyboardControls = true;
    [SerializeField] private KeyCode keyUp = KeyCode.W;
    [SerializeField] private KeyCode keyDown = KeyCode.S;
    [SerializeField] private KeyCode keyLeft = KeyCode.A;
    [SerializeField] private KeyCode keyRight = KeyCode.D;
    
    private Transform cameraTransform;
    private Vector3 targetPosition;
    private float targetZoom;
    
    private void Awake()
    {
        cameraTransform = transform;
        targetPosition = cameraTransform.position;
        targetZoom = Camera.main.orthographicSize;
    }
    
    private void Update()
    {
        HandleMovementInput();
        HandleZoomInput();
        UpdateCameraPosition();
    }
    
    private void HandleMovementInput()
    {
        Vector3 movementDirection = Vector3.zero;
        
        // Edge scrolling
        if (enableEdgeScrolling)
        {
            Vector3 mousePosition = Input.mousePosition;
            
            // Check left edge
            if (mousePosition.x < scrollAreaThickness)
            {
                movementDirection.x -= 1;
            }
            
            // Check right edge
            if (mousePosition.x > Screen.width - scrollAreaThickness)
            {
                movementDirection.x += 1;
            }
            
            // Check bottom edge
            if (mousePosition.y < scrollAreaThickness)
            {
                movementDirection.z -= 1;
            }
            
            // Check top edge
            if (mousePosition.y > Screen.height - scrollAreaThickness)
            {
                movementDirection.z += 1;
            }
        }
        
        // Keyboard controls
        if (enableKeyboardControls)
        {
            if (Input.GetKey(keyUp))
            {
                movementDirection.z += 1;
            }
            
            if (Input.GetKey(keyDown))
            {
                movementDirection.z -= 1;
            }
            
            if (Input.GetKey(keyLeft))
            {
                movementDirection.x -= 1;
            }
            
            if (Input.GetKey(keyRight))
            {
                movementDirection.x += 1;
            }
        }
        
        // Normalize for consistent speed in all directions
        if (movementDirection.magnitude > 0)
        {
            movementDirection.Normalize();
        }
        
        // Calculate new position
        Vector3 movement = movementDirection * edgeScrollSpeed * Time.deltaTime;
        
        // Convert to world space movement (accounting for camera rotation)
        movement = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * movement;
        
        targetPosition += movement;
    }
    
    private void HandleZoomInput()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        
        if (Mathf.Abs(scrollInput) > 0)
        {
            targetZoom -= scrollInput * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }
    
    private void UpdateCameraPosition()
    {
        // Apply boundaries if enabled
        if (useBoundaries)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);
        }
        
        // Move camera smoothly to target position
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * 5f);
        
        // Apply zoom if using orthographic camera
        if (Camera.main.orthographic)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime * 5f);
        }
        else
        {
            // For perspective camera, adjust the y position instead
            Vector3 pos = cameraTransform.position;
            pos.y = Mathf.Lerp(pos.y, targetZoom, Time.deltaTime * 5f);
            cameraTransform.position = pos;
        }
    }
}
