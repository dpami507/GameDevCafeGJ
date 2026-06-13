using UnityEngine;

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

    private void Start()
    {
        Events.ShakeCamera += Shake;
        Events.ZoomCamera += SetZoom;

        cam = GetComponent<Camera>();
        initZoomAmount = cam.orthographicSize;

        lastShook = Time.time;
    }
    private void OnDestroy()
    {
        Events.ShakeCamera -= Shake;
        Events.ZoomCamera -= SetZoom;
    }

    private void FixedUpdate()
    {
        if(target == null)
        {
            target = FindFirstObjectByType<PlayerMovement>()?.transform;
            return;
        }

        //Calculate the position of it should be
        Vector3 thisPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, thisPos, ref vel, smoothTime, maxSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, shakeLerpTime * Time.deltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, initZoomAmount, zoomLerpTime * Time.deltaTime);
    }
    public void Shake(float magnitude = 1)
    {
        int shakeDir = (Random.value > 0.5) ? 1 : -1;
        this.transform.rotation = Quaternion.Euler(0, 0, shakeAngleAmount * shakeDir * magnitude);

        lastShook = 0;
    }
    public void SetZoom(float amount)
    {
        cam.orthographicSize = initZoomAmount + (zoomAmount * amount);
    }
}
