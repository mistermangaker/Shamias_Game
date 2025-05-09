using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.InputSystem.InputAction;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraLookATTarget;
    public Vector3 moveDirection { get; private set; }
    public Vector3 lookDirection { get; private set; }

    [SerializeField] private float lookAtDistance = 3f;
    [SerializeField] private float moveSpeed =5f;

    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection*moveSpeed ;
        transform.localScale = new Vector3(Mathf.Sign(lookDirection.x), 1f, 1f);
        cameraLookATTarget.transform.position = transform.position + (moveDirection * lookAtDistance);
    }

    public void Move(CallbackContext context)
    {
        Vector2 temp = context.ReadValue<Vector2>();
        moveDirection = new Vector3(temp.x,0,temp.y);
        if (context.performed)
        {
            lookDirection = moveDirection;
        }
        
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(cameraLookATTarget.position, 0.5f);
    }
}
