using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float Speed = 15;
    [SerializeField] float SpeedMultiplierWhenShift = 2;
    [SerializeField] float Responsiveness = 10;

    [Header("Using mouse to move")]
    [SerializeField] float xBorderMargin;
    [SerializeField] float yBorderMargin;

    [Header("Movement Bounds")]
    [SerializeField] Vector2 MaxBound;
    [SerializeField] Vector2 MinBound;

    [Header("Zoom")]
    [SerializeField] float MinZoom = 2;
    [SerializeField] float MaxZoom = 10;
    [SerializeField] float ZoomSpeed = 5000;

    Vector2 movement;
    float currentSpeed 
        => Speed * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? SpeedMultiplierWhenShift : 1);

    Camera mainCam;

    void Start()
    {
        mainCam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector2 inputMovement = TakeMovementInput().normalized * currentSpeed;
        movement = Vector2.Lerp(movement, inputMovement, Responsiveness * Time.deltaTime);
        transform.position += Time.deltaTime * new Vector3(movement.x, movement.y);
        ClampCameraPosition();

        mainCam.orthographicSize 
            = Mathf.Clamp(mainCam.orthographicSize + TakeZoomInput() * ZoomSpeed * Time.deltaTime, MinZoom, MaxZoom);
    }

    void ClampCameraPosition()
    {
        float x = Mathf.Clamp(mainCam.transform.position.x, MinBound.x, MaxBound.x);
        float y = Mathf.Clamp(mainCam.transform.position.y, MinBound.y, MaxBound.y);
        mainCam.transform.position = new Vector3(x, y, mainCam.transform.position.z);
    }

    Vector2 TakeMovementInput()
    {
        Vector2 mouse = NormalisedMousePos();

        float x = Input.GetAxisRaw("Horizontal");
        if (mouse.x > 1 - xBorderMargin) x += 1;
        else if (mouse.x < xBorderMargin) x -= 1;

        float y = Input.GetAxisRaw("Vertical");
        if (mouse.y > 1 - yBorderMargin) y += 1;
        else if (mouse.y < yBorderMargin) y -= 1;

        return new Vector2(Mathf.Clamp(x, -1, 1), Mathf.Clamp(y, -1, 1));
    }

    Vector2 NormalisedMousePos()
    {
        Vector3 pixelCoords = Input.mousePosition;
        return new Vector2(Mathf.Clamp01(pixelCoords.x / Screen.width), Mathf.Clamp01(pixelCoords.y / Screen.height));
    }

    float TakeZoomInput()
        => -Input.GetAxisRaw("Mouse ScrollWheel");
}
