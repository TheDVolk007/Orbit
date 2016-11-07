using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed = 3f;

    public float CameraDistanceMax = 50f;
    public float CameraDistanceMin = 3f;
    public float CameraSize = 10f;
    public float ScrollSpeed = 5f;

    void Start()
    {
    }

    void Update()
    {
        var cameraComponent = transform.GetComponent<Camera>();
        
        CameraSize -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
        CameraSize = Mathf.Clamp(CameraSize, CameraDistanceMin, CameraDistanceMax);
        cameraComponent.orthographicSize = CameraSize;

        Speed = CameraSize * 3f / 10f;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-Speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, -Speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0, Speed * Time.deltaTime, 0));
        }

        if(Input.GetKey(KeyCode.C))
        {
            cameraComponent.orthographicSize = 5f;
            transform.position = new Vector3(0, 0, -10);
        }
    }
}
