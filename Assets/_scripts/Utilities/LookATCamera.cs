using UnityEngine;

public class LookATCamera : MonoBehaviour
{
    private enum Mode
    {
        lookAT,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted
    }
    [SerializeField] private Mode mode =Mode.CameraForward;

    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.lookAT:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }

    }
}
