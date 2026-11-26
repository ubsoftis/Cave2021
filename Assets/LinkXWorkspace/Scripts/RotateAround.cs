using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    //相机围绕物体旋转
    public Transform target;
    public float speed = 10;
    public float distance = 10;
    public float minDistance = 1;
    public float maxDistance = 20;
    public float xSpeed = 250;
    public float ySpeed = 120;
    public float yMinLimit = -20;
    public float yMaxLimit = 80;
    public float x = 0;
    public float y = 0;
    public float smoothTime = 2;
    public float zoomSpeed = 1;
    public float rotationDampening = 3;
    public float zoomDampening = 5;
    private float xDeg = 0;
    private float yDeg = 0;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;    
    private Vector3 position;
    private Vector3 targetOffset;
    private Vector3 desiredPosition;
    private Vector3 offset;
    private Vector3 targetPosition;
    private Vector3 targetDirection;
        
    private Vector3 direction;

        
}
