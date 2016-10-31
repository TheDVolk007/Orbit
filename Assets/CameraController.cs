using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed = 3f;

    float cameraDistanceMax = 50f;
    float cameraDistanceMin = 5f;
    float cameraDistance = 10f;
    float scrollSpeed = 5f;

    void Start()
    {
    }

    void Update()
    {
        var cameraComponent = transform.GetComponent<Camera>();
        
        cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
        transform.position = new Vector3(transform.position.x, transform.position.y, -cameraDistance);

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
