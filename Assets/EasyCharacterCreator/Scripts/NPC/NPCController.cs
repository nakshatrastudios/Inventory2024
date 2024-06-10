using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    CharacterController characterController;
    public float moveSpeed = 3f;
    public float rotationSpeed = 500f;
    public float jumpForce = 5f;

    private Quaternion targetRotation;
    private float ySpeed;
    private Animator animator;

    private bool isFalling;
    private bool isJumping;

    private float fallStart;

    public List<Transform> waypoints;
    private int currentWaypoint = 0;

    private NavMeshAgent agent;

    private Vector3 jumpDirection;
    private float jumpStartTime;

    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        characterController = transform.GetComponent<CharacterController>();
    }

    void Update()
    {
        if (agent.isOnNavMesh)
        {
            

            

            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false);
            isFalling = false;
            isJumping = false;

            if (currentWaypoint < waypoints.Count)
            {
                animator.SetBool("IsMoving", true);
                agent.SetDestination(waypoints[currentWaypoint].position);

                if (agent.isOnOffMeshLink)
                {
                    if (agent.currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeJumpAcross)
                    {
                        animator.SetBool("IsJumping", true);
                        agent.CompleteOffMeshLink();
                        return;
                    }
                }

                if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.1f)
                {
                    currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
                }

                if (!isJumping)
                {
                    NavMeshHit hit;
                    NavMesh.SamplePosition(waypoints[currentWaypoint].position, out hit, 10f, NavMesh.AllAreas);
                    Vector3 nextPos = hit.position;
                    float heightDiff = nextPos.y - transform.position.y;

                    if (heightDiff > 1f)
                    {
                        isJumping = true;
                        jumpDirection = (nextPos - transform.position).normalized;
                        jumpDirection.y = Mathf.Sqrt(heightDiff * -2f * Physics.gravity.y);
                        jumpStartTime = Time.time;
                    }
                }
                else
                {
                    float elapsedTime = Time.time - jumpStartTime;
                    Vector3 gravity = Physics.gravity * elapsedTime;
                    Vector3 move = jumpDirection * elapsedTime + gravity;
                    characterController.Move(move * Time.deltaTime);

                    if (characterController.isGrounded)
                    {
                        isJumping = false;
                    }
                }
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }
        else
        {
            animator.SetBool("IsGrounded", false);

            if (!isFalling)
            {
                fallStart = Time.time;
                isFalling = true;
            }

            if (Time.time - fallStart > 0.2f)
            {
                animator.SetBool("IsFalling", true);
            }
        }

        animator.SetFloat("moveAmount", 0.2f, 0.2f, Time.deltaTime);
    }
}
