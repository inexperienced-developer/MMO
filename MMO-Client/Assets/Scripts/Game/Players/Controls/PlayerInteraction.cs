using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private InGamePlayer m_Player;

    public void Init()
    {
        m_Player = GetComponent<InGamePlayer>();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(m_Player.Controls.MousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Constants.MAX_MOUSE_RAYCAST_DISTANCE))
        {
            IDLogger.Log($"Raycast hit {hit.collider.gameObject.name}");
            //If selectable object, select
            if (m_Player.Controls.LeftClick) return;
            //If interactable, interact
            if (m_Player.Controls.RightClickPressed)
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                interactable?.Interact(m_Player);
            }
        }
    }
}
