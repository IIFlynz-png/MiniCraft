using System.Diagnostics;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpForce = 5f;
    public float gravity = 9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private Camera playerCamera;
    private float xRotation = 0f;
    [Header("Portal Settings")]
    public LayerMask portalLayer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
        MovePlayer();
        JumpHandler();
        LookAround();
        CheckPortalInteraction();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calcola la direzione del movimento in base alla direzione della camera
        Vector3 move = playerCamera.transform.right * moveX + playerCamera.transform.forward * moveZ;
        move.y = 0f; // Blocca il movimento verticale per evitare che il player si muova in diagonale

        controller.Move(move * moveSpeed * Time.deltaTime);

    
        velocity.y -= gravity * Time.deltaTime;
    
        // Limita la velocità di caduta per evitare una caduta troppo veloce nel caso fosse in aria senza aver saltato
        float maxFallSpeed = -6f;
        if (velocity.y < maxFallSpeed)
        {
            velocity.y = maxFallSpeed;
        }

        controller.Move(velocity * Time.deltaTime);
    }   
    void JumpHandler()
    {
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpForce;
        }
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    void CheckPortalInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position,
                         playerCamera.transform.forward,
                         out hit,
                         portalLayer))
        {
            //UnityEngine.Debug.Log("Rilevato portale: " + hit.collider.name);
            Portal portal = hit.collider.GetComponent<Portal>();
            if (portal != null && Input.GetKeyDown(KeyCode.E))
            {
                portal.LoadScene();
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Portal portal = hit.gameObject.GetComponent<Portal>();
        if (portal != null)
        {
            portal.LoadScene(); 
        }
    }
}

