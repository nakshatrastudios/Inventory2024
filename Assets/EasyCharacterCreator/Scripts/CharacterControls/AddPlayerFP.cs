using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AddPlayerFP
{
    public class CreatePlayerFP : MonoBehaviour
    {
       [MenuItem("GameObject/Easy Character Creator/Create Player/Male/First-person", false, 10)]
       [MenuItem("Easy Character Creator/Create Player/Male/First-person", false, 10)]
        static void Create()
        {
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.slopeLimit = 90f;
            cc.skinWidth = 0.0001f;
            cc.center = new Vector3(0f, 0.9f, 0f);
            cc.radius = 0.2f;
            cc.height = 1.9f;
            player.AddComponent<PlayerMovementFP>();
            if(Camera.main.GetComponent<CameraController>() == null || Camera.main.GetComponent<CameraControllerOrthoGraphic>() != null)
            {
                Camera.main.gameObject.AddComponent<CameraController>();
                CameraControllerOrthoGraphic cco = Camera.main.gameObject.GetComponent<CameraControllerOrthoGraphic>();
                DestroyImmediate(cco);
            }
            Camera.main.GetComponent<CameraController>().followTarget = player.transform;
            Camera.main.GetComponent<CameraController>().distance = 0f;
            Camera.main.GetComponent<CameraController>().framingOffset = new Vector2(0,1.5f);
        }
    }

    public class PlayerMovementFP : MonoBehaviour
    {
        private float moveSpeed;
        [SerializeField] float walkSpeed = 3f;
        [SerializeField] float runSpeed = 6f;
        [SerializeField] float rotationSpeed = 500f;
        [SerializeField] float jumpForce = 5f;
        Quaternion targetRotation;
        CameraController cameraController;
        CharacterController characterController;
        float ySpeed;
        
        private void Awake()
        {
            cameraController = Camera.main.GetComponent<CameraController>();
            characterController = GetComponent<CharacterController>();
        }
        
        private void Update () 
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

            var moveInput = (new Vector3(h, 0, v)).normalized;

            var moveDir = cameraController.PlanarRotation * moveInput;

            if(characterController.isGrounded)
            {
                if(moveAmount > 0)
                {
                    targetRotation = Quaternion.LookRotation(moveDir);
                    if(Input.GetKey(KeyCode.LeftShift))
                    {
                        moveAmount = 0.2f;
                    }
                    else
                    {
                        moveSpeed = runSpeed;
                    }
                    if(moveAmount < 0.7f)
                    {
                        moveSpeed = walkSpeed;
                    }
                }
                
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    ySpeed = jumpForce;
                }
            }
            else
            {
                ySpeed += Physics.gravity.y * Time.deltaTime;
            }
            var velocity = moveDir * moveSpeed;
            velocity.y = ySpeed;

            characterController.Move(velocity * Time.deltaTime);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        }
    }
}
