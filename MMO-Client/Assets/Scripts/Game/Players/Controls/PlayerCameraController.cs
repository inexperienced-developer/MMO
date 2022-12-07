using Cinemachine;
using InexperiencedDeveloper.Core.Controls;
using InexperiencedDeveloper.Utils;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    private Player m_Player;
    private PlayerControls m_Controls;
    private Camera m_Camera;
    public Camera Camera => m_Camera;
    private CinemachineFreeLook m_FreeLook;

    private float m_XAxisSpeed, m_YAxisSpeed;

    public void Init()
    {
        m_Camera = GetComponentInChildren<Camera>();
        m_FreeLook = GetComponentInChildren<CinemachineFreeLook>();
        m_Player = GetComponentInParent<Player>();
        m_Controls = m_Player.GetComponent<PlayerControls>();
        Transform parentTransform = m_Player.transform;
        m_FreeLook.Follow = parentTransform;
        m_FreeLook.LookAt = parentTransform;
        m_XAxisSpeed = m_FreeLook.m_XAxis.m_MaxSpeed;
        m_YAxisSpeed = m_FreeLook.m_YAxis.m_MaxSpeed;
    }

    private void Update()
    {
        if(m_FreeLook != null)
        {
            if (m_Controls.LeftClick || m_Controls.RightClickHeld)
            {
                m_FreeLook.m_XAxis.m_MaxSpeed = m_XAxisSpeed;
                m_FreeLook.m_YAxis.m_MaxSpeed = m_YAxisSpeed;
            }
            else
            {
                m_FreeLook.m_XAxis.m_MaxSpeed = 0;
                m_FreeLook.m_YAxis.m_MaxSpeed = 0;
            }
        }

    }
}
