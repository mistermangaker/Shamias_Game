using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{

    private const string ISMOVING = "IsMoving";
    private const string LOOKDIRX = "LookDirX";
    private const string LOOKDIRY = "LookDirY";
    private Animator _animator;
    private PlayerController _playerController;


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
    }
    void Update()
    {
        _animator.SetBool(ISMOVING, _playerController.moveDirection != Vector3.zero);
        _animator.SetFloat(LOOKDIRX, _playerController.lookDirection.x);
        _animator.SetFloat(LOOKDIRY, _playerController.lookDirection.z);
    }
}
