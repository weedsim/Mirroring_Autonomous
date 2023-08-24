using Fusion;
using Opsive.Shared.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    Vector2 cameraMove;

    [Header("Camera Controller Settings")]
    public GameObject _camera;
    public GameObject _camRotateOrigin;
    public float _rotationX = 0;
    float _threshold = 0.01f;

    public float rotationSpeed = 15.0f;
    public float viewUpDownRotationSpeed = 50.0f;

    void Update()
    {
        if (cameraMove.sqrMagnitude > _threshold)
        {
            transform.Rotate(0, cameraMove.x * rotationSpeed * Time.deltaTime, 0);

            _rotationX -= (cameraMove.y * viewUpDownRotationSpeed * Time.deltaTime);
            _rotationX = Mathf.Clamp(_rotationX, -20, 15);

            float radian = DegreeToRadian(-_rotationX);
            _camera.transform.position = _camRotateOrigin.transform.position + transform.forward * 4.5f * Mathf.Cos(radian) * Mathf.Sin(DegreeToRadian(70));
            _camera.transform.position += transform.up * 4.5f * Mathf.Sin(radian);
            _camera.transform.position += transform.right * 4.5f * Mathf.Cos(radian) * Mathf.Cos(DegreeToRadian(70));

            _camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        }
    }

    public void SetCameraMoveVector(Vector2 viewInput)
    {
        cameraMove = viewInput;
    }

    protected float DegreeToRadian(float degree) { return Mathf.PI * degree / 180.0f; }
}
