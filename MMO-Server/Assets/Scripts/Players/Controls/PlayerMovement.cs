using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player m_Player;
    [SerializeField] private CharacterController m_Controller;
    [SerializeField] private float m_Gravity;
    [SerializeField] private float m_JumpHeight;

    private float m_GravityAcceleration;
    private float m_Speed;
    private float m_JumpSpeed;
    private float m_YVel;

    //Received Inputs
    private Vector2 m_MovementInput;
    private float m_YRot;
    private bool m_Jump, m_RightClick, m_LeftClick;

    private float m_TurnSmoothVel;

    private void OnValidate()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        m_Controller = m_Controller == null ? GetComponent<CharacterController>() : m_Controller;
        m_Player = m_Player == null ? GetComponent<Player>() : m_Player;

        m_GravityAcceleration = m_Gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        m_Speed = Constants.PLAYER_MOVE_SPEED * Time.fixedDeltaTime;
        m_JumpSpeed = Mathf.Sqrt(m_JumpHeight * -2f * m_GravityAcceleration);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (m_RightClick)
        {
            float faceCamera = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_YRot, ref m_TurnSmoothVel, Constants.PLAYER_TURN_SPEED);
            transform.rotation = Quaternion.Euler(0, faceCamera, 0);
        }
        Vector3 dir = new Vector3(m_MovementInput.x, 0, m_MovementInput.y).normalized;
        Quaternion rot = Quaternion.Euler(transform.rotation.x, m_YRot, transform.rotation.z);
        dir *= m_Speed;
        if (dir.magnitude >= 0.01f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + m_YRot; // IF RIGHT CLICK DONT ROTATE
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_TurnSmoothVel, Constants.PLAYER_TURN_SPEED);
            if (!m_RightClick) transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            dir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            m_Controller.Move(dir.normalized * m_Speed);
        }
        var nearbyPlayers = PlayerManager.Instance.GetNearbyPlayers(transform.position);
        foreach (var player in nearbyPlayers)
        {
            IDLogger.Log($"{gameObject.name} -- Sending details to {player.Id}");
            SendMovement(player.Id);
        }
    }

    public void SetInput(Vector2 moveInput, float yRot, bool jump, bool rightClick, bool leftClick)
    {
        m_MovementInput = moveInput;
        m_YRot = yRot;
        m_Jump = jump;
        m_RightClick = rightClick;
        m_LeftClick = leftClick;
    }


    #region Messages

    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Sending ----------------------------------------//
    //-------------------------------------------------------------------------------------//
    private void SendMovement(ushort toId)
    {
        Vector2 movement = m_MovementInput;
        bool jump = m_Jump;
        bool rightClick = m_RightClick;
        bool leftClick = m_LeftClick;
        byte input = Utilities.BoolsToByte(new bool[7] { movement.x > 0, movement.x < 0, movement.y > 0, movement.y < 0, jump, rightClick, leftClick });
        Message msg = Message.Create(MessageSendMode.Unreliable, ServerToClientId.UpdatePosition);
        msg.AddUShort(m_Player.Id);
        msg.AddVector3Int(transform.position);
        msg.AddByte(input);
        NetworkManager.Instance.Server.Send(msg, toId);
    }
    #endregion
}
