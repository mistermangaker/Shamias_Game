using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(Animator))]
public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private Animator animator;

    private const string LOOKDIRECTIONY = "LookDirectionY";
    private const string LOOKDIRECTIONX = "LookDirectionX";
    private const string ISMOVING = "IsMoving";

    private void Update()
    {
        animator.SetFloat(LOOKDIRECTIONY, controller.LookDirection.y);
        animator.SetFloat(LOOKDIRECTIONX, controller.LookDirection.x);
        animator.SetBool(ISMOVING, controller.MoveDirection != Vector3.zero);
    }
}
