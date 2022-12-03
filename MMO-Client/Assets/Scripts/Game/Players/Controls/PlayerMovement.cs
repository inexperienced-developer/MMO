using Cinemachine;
using InexperiencedDeveloper.Core.Controls;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private InGamePlayer m_Player;
    private CharacterController m_Controller;
    private float m_Speed;
    private float m_TurnSmoothVel;

    private PlayerCameraController m_CameraController;

    private bool m_LeftClickHeld;
    private float m_CameraAdder;

    public float LookDir => m_CameraAdder;


    //Not local player
    private Vector2 m_Move;
    private bool m_Jump;
    private bool m_RightClick;
    private bool m_LeftClick;

    public void Init()
    {
        m_Player = GetComponent<InGamePlayer>();
        m_Controller = GetComponent<CharacterController>();
        m_Controller.center = Constants.CHARACTER_CONTROLLER_CAPSULE_CENTER;

        if (m_Player.IsLocal || m_Player.DEBUG)
        {
            m_CameraController = Instantiate(GameManager.Instance.LocalCameraPrefab, transform).GetComponent<PlayerCameraController>();
            m_CameraController.Init();
        }

        m_Speed = Constants.PLAYER_MOVE_SPEED;
        m_Speed *= Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        if (m_Player.IsLocal)
            Move();
        else
            NotLocalMove();
    }

    private void Move()
    {
        if (m_Player == null || m_Player.Controls == null) Init();
        Vector3 dir = m_Player.Controls.Movement.normalized;
        IDLogger.Log($"Dir: {dir}");
        if (m_Player.Controls.RightClick)
        {
            float faceCamera = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_CameraController.Camera.transform.eulerAngles.y, ref m_TurnSmoothVel, Constants.PLAYER_TURN_SPEED);
            transform.rotation = Quaternion.Euler(0, faceCamera, 0);
        }
        if (m_Player.Controls.LeftClick)
        {
            if (!m_LeftClickHeld)
            {
                m_CameraAdder = transform.eulerAngles.y;
                m_LeftClickHeld = true;
            }
        }
        else
        {
            m_CameraAdder = m_CameraController.Camera.transform.eulerAngles.y;
            if (m_LeftClickHeld) m_LeftClickHeld = false;
        }
        dir *= m_Speed;
        if (dir.magnitude >= 0.01f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + m_CameraAdder;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_TurnSmoothVel, Constants.PLAYER_TURN_SPEED);
            if (!m_Player.Controls.RightClick) transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            dir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            m_Controller.Move(dir.normalized * m_Speed);
        }
        m_Player.Anim.SetMove(new Vector2(m_Player.Controls.Movement.x, m_Player.Controls.Movement.z));
    }

    private void NotLocalMove()
    {
        if (m_Player == null) Init();
        Vector3 dir = m_Move.normalized;
        if (m_RightClick)
        {
            float faceCamera = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_CameraController.Camera.transform.eulerAngles.y, ref m_TurnSmoothVel, Constants.PLAYER_TURN_SPEED);
            transform.rotation = Quaternion.Euler(0, faceCamera, 0);
        }
        if (m_LeftClick)
        {
            if (!m_LeftClickHeld)
            {
                m_CameraAdder = transform.eulerAngles.y;
                m_LeftClickHeld = true;
            }
        }
        else
        {
            m_CameraAdder = m_CameraController.Camera.transform.eulerAngles.y;
            if (m_LeftClickHeld) m_LeftClickHeld = false;
        }
        dir *= m_Speed;
        if (dir.magnitude >= 0.01f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + m_CameraAdder;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_TurnSmoothVel, Constants.PLAYER_TURN_SPEED);
            if (m_RightClick) transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            dir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            m_Controller.Move(dir.normalized * m_Speed);
        }
        m_Player.Anim.SetMove(dir.normalized);
    }

    public void SetInputs(bool[] input)
    {
        float x = input[0] ? 1 : 0;
        x = input[1] ? -1 : x;
        float y = input[2] ? 1 : 0;
        y = input[3] ? -1 : y;
        if (x > 0 && y > 0)
        {
            x = 0.75f;
            y = 0.75f;
        }
        m_Move = new Vector2(x, y);
        m_Jump = input[4];
        m_RightClick = input[5];
        m_LeftClick = input[6];
    }


}
