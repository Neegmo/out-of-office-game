using UnityEngine;

public class CharacterController : MonoBehaviour
{
      // References
    [SerializeField] private LayerMask groundLayer; // Set this in the inspector to your ground/plane layer
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameObject destinationMarker; // Optional visual marker for click destination
    
    // Movement variables
    private Vector3 targetPosition;
    private bool isMoving = false;
    
    // Optional NavMeshAgent reference (for pathfinding)
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    
    // Optional Animator reference
    private Animator animator;
    
    private void Awake()
    {
        // Initialize target position to current position
        targetPosition = transform.position;
        
        // Get NavMeshAgent component if using pathfinding
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.angularSpeed = rotationSpeed * 100f;
        }
        
        // Get animator if available
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            HandleMouseClick();
        }
        
        // Handle movement if not using NavMeshAgent
        if (isMoving && navMeshAgent == null)
        {
            MoveToTarget();
        }
        
        // Update animation if using Animator
        UpdateAnimation();
    }
    
    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Check if ray hits the ground layer
        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            // Set new target position
            targetPosition = hit.point;
            isMoving = true;
            
            // Show visual marker at click point if available
            if (destinationMarker != null)
            {
                destinationMarker.transform.position = targetPosition;
                destinationMarker.SetActive(true);
            }
            
            // If using NavMeshAgent, set destination
            if (navMeshAgent != null)
            {
                navMeshAgent.SetDestination(targetPosition);
            }
        }
    }
    
    private void MoveToTarget()
    {
        // Calculate direction and distance to target
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Keep movement on the horizontal plane
        float distance = direction.magnitude;
        
        // If we're close enough to the target, stop moving
        if (distance < 0.1f)
        {
            isMoving = false;
            if (destinationMarker != null)
            {
                destinationMarker.SetActive(false);
            }
            return;
        }
        
        // Normalize direction and move
        direction.Normalize();
        
        // Rotate towards movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        // Move character
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
    
    private void UpdateAnimation()
    {
        if (animator != null)
        {
            // Calculate if we're moving with or without NavMeshAgent
            bool moving = navMeshAgent != null ? navMeshAgent.velocity.magnitude > 0.1f : isMoving;
            
            // Set animation parameter
            animator.SetBool("IsMoving", moving);
        }
    }
}
