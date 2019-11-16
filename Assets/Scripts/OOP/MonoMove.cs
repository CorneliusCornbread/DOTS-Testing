using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

namespace MonoScripts
{
    [BurstCompile]
    public class MonoMove : MonoBehaviour
    {
        public Rigidbody rb;
        public const float gravity = 4;
        public const float speed = 16;
        public const float maxVelocityChange = 30;

        public bool IsGrounded { get; private set; } = false;

        NativeArray<RaycastHit> raycastHits;
        NativeArray<RaycastCommand> commands;
        JobHandle raycastJob;

        private void Start()
        {
            

            if (rb == null)
            {
                BurstDebug.LogWarning("Rigidbody is not set on object " + name);
                enabled = false;
            }
        }

        public void Update()
        {
            commands[0] = new RaycastCommand(transform.position, Vector3.down, 1.25f);

            raycastJob = RaycastCommand.ScheduleBatch(commands, raycastHits, 1);
        }

        public void LateUpdate()
        {
            raycastJob.Complete();

            if (raycastHits[0].transform != null)
            {
                IsGrounded = true;
            }

            else
            {
                IsGrounded = false;
            }
        }

        private void FixedUpdate()
        {
            #if UNITY_EDITOR
            if (MonoInputManager.instance == null)
            {
                BurstDebug.Log("Waiting for MonoInputManager");
                return;
            }
            #endif

            float2 input = MonoInputManager.instance.smoothedMove;
            float3 targetVelocity = new float3(input.x, 0, input.y);            

            // Calculate how fast we should be moving
            targetVelocity = transform.TransformDirection(targetVelocity); //change from local space to world space
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            float3 velocity = rb.velocity;
            float3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = -gravity * rb.mass / 10; //Apply gravity

            if (MonoInputManager.instance.jump && IsGrounded)
            {
                velocityChange.y = 0;
                rb.velocity = new Vector3(rb.velocity.x, 26, rb.velocity.z);
            }

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        public void OnDisable()
        {
            raycastHits.Dispose();
            commands.Dispose();
        }

        private void OnEnable()
        {
            raycastHits = new NativeArray<RaycastHit>(1, Allocator.Persistent);
            commands = new NativeArray<RaycastCommand>(1, Allocator.Persistent);
        }
    }
}
