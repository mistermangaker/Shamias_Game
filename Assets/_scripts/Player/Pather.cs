using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Pather : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerMovementOrders currentPlayerOrders;
    public bool canMove { get; private set; } = true;
    private bool PatherHasDestinationSet => currentPlayerOrders.destination != Vector3.zero;
    public UnityAction<PlayerMovementOrders> OnPatherReachedDestination;

    public bool PatherAtLocation
    {
        get
        {
            if (currentPlayerOrders.destination == Vector3.zero) return true;
            else
            {
                return Vector3.Distance(transform.position, currentPlayerOrders.destination) < 2f;
            }
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
 

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (PatherHasDestinationSet)
        {
            if (PatherAtLocation)
            {
                OnPatherReachedDestination?.Invoke(currentPlayerOrders);
                ClearPatherMoveOrders();
            }
            else
            {
                Vector3 dir = GetDirectionTo(currentPlayerOrders.destination);
                MoveTowards(dir);
            }
        }
       
    }

    private Vector3 GetDirectionTo(Vector3 destination)
    {
        Vector3 temp = (destination - transform.position).normalized;
        temp.y = 0;
        return temp;
    }
    private void MoveTowards(Vector3 direction)
    {
        if (canMove) rb.linearVelocity = direction * PlayerInternalState.Instance.PlayerMoveSpeed;
        else rb.angularVelocity = Vector3.zero;
    }
    public void SetPatherMoveOrders(PlayerMovementOrders orders)
    {
        currentPlayerOrders = orders;
    }

    public void ClearPatherMoveOrders()
    {
        currentPlayerOrders = new();
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
    }
}
