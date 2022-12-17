using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [Header("cameraPlayer")]
    public Camera cam;
    [SerializeField] private Transform pointOfView;
    [SerializeField] public Transform GunPoint;
    public Transform recoil;
    private Vector2 mouseInput;
    private float HorizontalRotateStore;
    private float VerticalRotateStore;

    [Header("BasicMovement")]
    Vector3 direction;
    Vector3 movement;

    [SerializeField] private float walkedSpeed;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravityMod = 2.5f;
    [SerializeField] private float runSpeed;
    [SerializeField] private float actualSpeed;
    [SerializeField]private CharacterController controller;

    [Header("Ground Direction")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private float radio;
    [SerializeField] private float distance;
    [SerializeField] private Vector3 offset;
    [SerializeField] private LayerMask lm;
    #endregion

    #region Unity Functions;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
    }
    private void Update()
    {
        Rotation();
        Movement();
    }
    private void LateUpdate()
    {
        cam.transform.position = recoil.position;
        cam.transform.rotation = recoil.rotation;

        GunPoint.transform.position = recoil.position;
        GunPoint.transform.rotation = recoil.rotation;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + offset, radio);
        if (Physics.SphereCast(transform.position + offset, radio, Vector3.down, out RaycastHit hit, distance, lm))
        {
            Gizmos.color = Color.green;
            Vector3 endPoint = ((transform.position + offset) + (Vector3.down * distance));
            Gizmos.DrawWireSphere(endPoint, radio);
            Gizmos.DrawLine(transform.position + offset, endPoint);

            Gizmos.DrawSphere(hit.point, 0.1f);
        }
        else
        {
            Gizmos.color = Color.red;
            Vector3 endPoint = ((transform.position + offset) + (Vector3.down * distance));
            Gizmos.DrawWireSphere(endPoint, radio);
            Gizmos.DrawLine(transform.position + offset, endPoint);
        }
    }
    #endregion

    #region Custom Functions
    private void Rotation()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        HorizontalRotateStore += mouseInput.x;
        VerticalRotateStore -= mouseInput.y;

        VerticalRotateStore = Mathf.Clamp(VerticalRotateStore,-60f,60);

        transform.rotation = Quaternion.Euler(0f,HorizontalRotateStore,0f);
        pointOfView.localRotation = Quaternion.Euler(VerticalRotateStore, 0f, 0f);
    }
    private void Movement()
    {
        direction = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));

        float velY = movement.y; 
        movement = ((transform.forward * direction.z) + (transform.right * direction.x)).normalized;
        movement.y = velY;


        if (Input.GetButton("Fire3"))
        {
            actualSpeed = runSpeed;
        }
        else
        {
            actualSpeed = walkedSpeed;
        }

        if (IsGrounded() && Mathf.Abs(movement.y) >= jumpForce)
        {
            movement.y = 0;
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            movement.y = jumpForce * Time.deltaTime;
        }

        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;             
        controller.Move(movement * (actualSpeed * Time.deltaTime));
    }

    private bool IsGrounded()
    {
        isGrounded = false;
        if (Physics.SphereCast(transform.position + offset, radio, Vector3.down, out RaycastHit hit, distance, lm))
        {
            isGrounded = true;
        }
        return isGrounded;
    }
    #endregion
}
