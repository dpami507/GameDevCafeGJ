using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class PlayerCamFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;

    [Header("Movement Variables")]
    [SerializeField] float smoothTime;
    [SerializeField] float maxSpeed;

    [Header("Shake Variables")]
    [SerializeField] int shakeAngleAmount;
    [SerializeField] float shakeLerpTime;
    [SerializeField] float shakeCooldown;
    float lastShook;

    [Header("Zoom Variables")]
    [SerializeField] float zoomLerpTime;
    [SerializeField] float zoomAmount;
    float initZoomAmount;

    Vector3 vel = Vector3.zero;
    Camera cam;

    InputAction playerAction;

    private void Start()
    {
        Events.ShakeCamera += Shake;
        Events.ZoomCamera += SetZoom;

        cam = GetComponent<Camera>();
        initZoomAmount = cam.orthographicSize;

        playerAction = InputSystem.actions.FindAction("Jump");
        lastShook = Time.time;
    }

    private void FixedUpdate()
    {
        //Calculate the position of it should be
        Vector3 thisPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, thisPos, ref vel, smoothTime, maxSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, shakeLerpTime * Time.deltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, initZoomAmount, zoomLerpTime * Time.deltaTime);
    }
    private void Update()
    {
        lastShook += Time.deltaTime;
        if(playerAction.IsPressed())
        {
            if (lastShook > shakeCooldown) return;

            Shake(1f);
        }
    }
    public void Shake(float magnitude = 1)
    {
        int shakeDir = (Random.value > 0.5) ? 1 : -1;
        transform.rotation = Quaternion.Euler(0, 0, shakeAngleAmount * shakeDir * magnitude);

        lastShook = 0;
    }
    public void SetZoom(float amount)
    {
        cam.orthographicSize = initZoomAmount + (zoomAmount * amount);
    }
}
