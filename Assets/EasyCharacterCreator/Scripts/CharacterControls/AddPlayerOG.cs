using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AddPlayerOG
{
    public class CreatePlayerOG : MonoBehaviour
    {
       [MenuItem("GameObject/Easy Character Creator/Create Player/Male/Orthographic", false, 10)]
       [MenuItem("Easy Character Creator/Create Player/Male/Orthographic", false, 10)]
        static void Create()
        {
            GameObject player = new GameObject("Player");
            GameObject character = Instantiate((GameObject)Resources.Load("CharacterPrefabs/CharacterMale")); // Instantiate the 3D model
            player.tag = "Player";
            character.transform.parent = player.transform;
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.slopeLimit = 90f;
            cc.skinWidth = 0.0001f;
            cc.center = new Vector3(0f, 0.9f, 0f);
            cc.radius = 0.2f;
            cc.height = 1.9f;
            player.AddComponent<PlayerMovementOG>();
            Animator animator = character.GetComponent<Animator>();
            animator.applyRootMotion = false;
            animator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Locomotion");
            if(Camera.main.GetComponent<CameraControllerOrthoGraphic>() == null || Camera.main.GetComponent<CameraController>() != null)
            {
                Camera.main.gameObject.AddComponent<CameraControllerOrthoGraphic>();
                CameraController cco = Camera.main.GetComponent<CameraController>();
                DestroyImmediate(cco);
            }
            Camera.main.GetComponent<CameraControllerOrthoGraphic>().followTarget = player.transform;
            Camera.main.GetComponent<CameraControllerOrthoGraphic>().distance = 10f;
            //Camera.main.GetComponent<CameraControllerOrthoGraphic>().framingOffset = new Vector2(0,1.5f);
        }
    }

    public class CreatePlayerOGFM : MonoBehaviour
    {
       [MenuItem("GameObject/Easy Character Creator/Create Player/Female/Orthographic", false, 10)]
       [MenuItem("Easy Character Creator/Create Player/Female/Orthographic", false, 10)]
        static void Create()
        {
            GameObject player = new GameObject("Player");
            GameObject character = Instantiate((GameObject)Resources.Load("CharacterPrefabs/CharacterFemale")); // Instantiate the 3D model
            player.tag = "Player";
            character.transform.parent = player.transform;
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.slopeLimit = 90f;
            cc.skinWidth = 0.0001f;
            cc.center = new Vector3(0f, 0.9f, 0f);
            cc.radius = 0.2f;
            cc.height = 1.9f;
            player.AddComponent<PlayerMovementOG>();
            Animator animator = character.GetComponent<Animator>();
            animator.applyRootMotion = false;
            animator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/LocomotionFemale");
            if(Camera.main.GetComponent<CameraControllerOrthoGraphic>() == null || Camera.main.GetComponent<CameraController>() != null)
            {
                Camera.main.gameObject.AddComponent<CameraControllerOrthoGraphic>();
                CameraController cco = Camera.main.GetComponent<CameraController>();
                DestroyImmediate(cco);
            }
            Camera.main.GetComponent<CameraControllerOrthoGraphic>().followTarget = player.transform;
            Camera.main.GetComponent<CameraControllerOrthoGraphic>().distance = 10f;
            //Camera.main.GetComponent<CameraControllerOrthoGraphic>().framingOffset = new Vector2(0,1.5f);
        }
    }

    public class PlayerMovementOG : MonoBehaviour
    {
        CharacterController characterController;
        CameraControllerOrthoGraphic cameraController;
        public float runSpeed = 6f;
        public float walkSpeed = 3f;
        public float rotationSpeed = 500f;
        public float jumpForce = 5f;

        private float moveSpeed;
        private Quaternion targetRotation;
        private float ySpeed;
        private Animator animator;

        private bool isFalling;

        private float fallStart;

        void Start()
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
            cameraController = Camera.main.GetComponent<CameraControllerOrthoGraphic>();
            characterController = transform.GetComponent<CharacterController>();
        }

        void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

            var moveInput = (new Vector3(h, 0, v)).normalized;

            var moveDir = cameraController.PlanarRotation * moveInput;

            

            if (characterController.isGrounded)
            {
                animator.SetBool("IsGrounded", true);
                animator.SetBool("IsFalling", false);
                animator.SetBool("IsJumping", false);
                isFalling = false;

                if (moveAmount > 0)
                {
                    animator.SetBool("IsMoving", true);
                    targetRotation = Quaternion.LookRotation(moveInput);

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        moveAmount = 0.2f;
                    }
                    else
                    {
                        moveSpeed = runSpeed;
                    }

                    if (moveAmount < 0.7f)
                    {
                        moveSpeed = walkSpeed;
                    }
                }
                else
                {
                    animator.SetBool("IsMoving", false);
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    animator.SetBool("IsJumping", true);
                    ySpeed = jumpForce;
                }
                
            }
            else
            {
                animator.SetBool("IsGrounded", false);
                
                ySpeed += Physics.gravity.y * Time.deltaTime;
                
                if(!isFalling)
                {
                    fallStart = Time.time;
                    isFalling = true;
                }
                
                if(Time.time - fallStart > 0.2f)
                {
                    animator.SetBool("IsFalling", true);
                }
            }

            var velocity = moveDir * moveSpeed;
            velocity.y = ySpeed;

            characterController.Move(velocity * Time.deltaTime);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            animator.SetFloat("moveAmount", moveAmount, 0.2f, Time.deltaTime);
        }
    }
}
