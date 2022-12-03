using InexperiencedDeveloper.Core.Controls;
using InexperiencedDeveloper.Utils.Log;
using UnityEngine;

public class InGamePlayer : Player
{
    public Inventory Inventory { get; private set; }
    public Gear Gear { get; private set; }
    public int GeoArea = -1;

    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerAnimation Anim { get; protected set; }

    protected Vector3 lastPos;

    public override void Init(ushort id, string email = "")
    {
        base.Init(id, email);
        if (IsLocal)
        {
            if(Controls == null)
            {
                Controls = GetComponent<PlayerControls>() == null ? gameObject.AddComponent<PlayerControls>() : GetComponent<PlayerControls>();

            }

            Controls.Init();
        }
        if (PlayerMovement == null)
        {
            PlayerMovement = GetComponent<PlayerMovement>() == null ? gameObject.AddComponent<PlayerMovement>() : GetComponent<PlayerMovement>();
            PlayerMovement.Init();
        }
        if (Anim == null)
        {
            Anim = GetComponent<PlayerAnimation>() == null ? gameObject.AddComponent<PlayerAnimation>() : GetComponent<PlayerAnimation>();
            Anim.Init();
        }
    }

    public void ReceiveMovement(Vector3 pos, bool[] inputs)
    {
        if(!(Id == NetworkManager.Instance.Client.Id))
        {
            PlayerMovement.SetInputs(inputs);
        }
        if (Vector3.Distance(pos, transform.position) > 1)
        {
            transform.position = pos;
        }
    }

    //protected void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //protected void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if(scene.buildIndex == GeoArea)
    //    {
    //        LevelManager.Instance.MoveGameObjectToScene
    //    }
    //}

}
