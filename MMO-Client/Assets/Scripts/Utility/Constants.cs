using UnityEngine;

public static class Constants
{
    #region Characters

    public const float PLAYER_MOVE_SPEED = 3;
    public const float PLAYER_TURN_SPEED = 0.016f;
    public const float PLAYER_INTERACT_DISTANCE = 1f;
    public static Vector3 CHARACTER_CONTROLLER_CAPSULE_CENTER = Vector3.up;


    //ANIMATION PARAMETER NAMES
    public const string ANIM_RIGHT = "right";
    public const string ANIM_FWD = "forward";
    public const string ANIM_B_INTERACTING = "interacting";
    public const string ANIM_T_TREE = "chopTree";
    public const string ANIM_T_MINING = "mining";

    #endregion

    #region Items

    //ITEMS
    public const string ITEM_ID_FORMAT = "000000";

    #endregion

    #region General

    public const float MAX_MOUSE_RAYCAST_DISTANCE = 30f;

    #endregion
}
