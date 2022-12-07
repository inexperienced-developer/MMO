using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InexperiencedDeveloper.Core.Controls
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerControls : MonoBehaviour
    {
        private Player m_Player;
        public PlayerInputActions PlayerActions { get; private set; }
        public PlayerMovement PlayerMovement { get; private set; }
        public Vector3 Movement { get; private set; }
        public Vector2 Look { get; private set; }
        public float CameraPitchAngle { get; private set; }
        public float CameraYawAngle { get; private set; }
        public float TargetPitchAngle { get; private set; }
        public float TargetYawAngle { get; private set; }
        public bool Jump { get; private set; }
        public bool LeftClick { get; private set; }
        public bool RightClickHeld { get; private set; }
        public bool RightClickPressed { get; private set; }
        public Vector2 MousePos { get; private set; }

        public Vector3 WalkDir { get; private set; }
        public float WalkSpeed { get; private set; }
        private Vector3 walkLocalDir;
        private Vector3 lastWalkDir;
        public float UnsmoothedWalkSpeed;

        private List<float> mouseInputsX = new List<float>();
        private List<float> mouseInputsY = new List<float>();

        public void Init()
        {
            m_Player = GetComponent<Player>();
            if (!m_Player.IsLocal)
            {
                Destroy(this);
                return;
            }
            PlayerActions = new PlayerInputActions();
            PlayerMovement = GetComponent<PlayerMovement>();
            PlayerActions.Enable();
        }

        private void OnDisable()
        {
            PlayerActions.Disable();
        }

        private void Update()
        {
            if (m_Player.IsLocal)
            {
                ReadInput();
            }
            if (m_Player.DEBUG && !m_Player.IsLocal)
                ReadInput();
            HandleInput();
        }

        public void ReceiveInputs(Vector3 movement, Vector2 lookDir, bool jump)
        {
            Movement = movement;
            CameraPitchAngle = lookDir.x;
            CameraYawAngle = lookDir.y;
            Jump = jump;
        }

        private void ReadInput()
        {
            Movement = CalcKeyWalk;
            //Add conditionals for mouse/controller
            CameraYawAngle += Smoothing.SmoothValue(mouseInputsX, m_CalcKeyLook.x);
            if(CameraYawAngle > 360)
            {
                CameraYawAngle -= 360;
            }
            else if(CameraYawAngle < 0)
            {
                CameraYawAngle += 360;
            }
            CameraPitchAngle -= Smoothing.SmoothValue(mouseInputsY, m_CalcKeyLook.y);
            CameraPitchAngle = Mathf.Clamp(CameraPitchAngle, -80f, 80f);
            Look = m_CalcKeyLook;
            Jump = m_GetJump;
            RightClickHeld = m_GetRightClick;
            RightClickPressed = m_GetRightClickPressed;
            LeftClick = m_GetLeftClick;
            MousePos = m_MousePos;
        }

        private void HandleInput()
        {
            walkLocalDir = Movement;
            TargetPitchAngle = Mathf.MoveTowards(TargetPitchAngle, CameraPitchAngle, 180f * Time.fixedDeltaTime / 0.1f);
            TargetYawAngle = CameraYawAngle;
            Quaternion rot = Quaternion.Euler(0, CameraYawAngle, 0);
            Vector3 dir = rot * walkLocalDir;
            UnsmoothedWalkSpeed = dir.magnitude;
            dir = new Vector3(FilterAxisAcceleration(lastWalkDir.x, dir.x), 0f, FilterAxisAcceleration(lastWalkDir.z, dir.z));
            WalkSpeed = dir.magnitude;
            if (WalkSpeed > 0f)
            {
                WalkDir = dir;
            }
            lastWalkDir = dir;
        }

        private float FilterAxisAcceleration(float currVal, float desiredVal)
        {
            float timeStep = Time.fixedDeltaTime;
            float min = 0.2f;
            if (currVal * desiredVal <= 0)
                currVal = 0;
            if (Mathf.Abs(currVal) > Mathf.Abs(desiredVal))
                currVal = desiredVal;
            if (Mathf.Abs(currVal) < min)
                timeStep = Mathf.Max(timeStep, min - Mathf.Abs(currVal));
            if (Mathf.Abs(currVal) > 0.8f)
                timeStep /= 3f;
            return Mathf.MoveTowards(currVal, desiredVal, timeStep);
        }

        public Vector3 CalcKeyWalk
        {
            get
            {
                Vector2 movement = PlayerActions.Player.Movement.ReadValue<Vector2>();
                return new Vector3(movement.x, 0, movement.y);
            }
        }

        private Vector2 m_CalcKeyLook => PlayerActions.Player.Look.ReadValue<Vector2>();
        private Vector2 m_MousePos => PlayerActions.Player.MousePosition.ReadValue<Vector2>();
        private bool m_GetJump => PlayerActions.Player.Jump.IsPressed();
        private bool m_GetRightClick => PlayerActions.Player.RightClick.IsPressed();
        private bool m_GetRightClickPressed => PlayerActions.Player.RightClick.WasPressedThisFrame();
        private bool m_GetLeftClick => PlayerActions.Player.LeftClick.IsPressed();

    }
}