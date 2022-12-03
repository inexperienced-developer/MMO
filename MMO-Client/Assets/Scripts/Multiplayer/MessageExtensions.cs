using InexperiencedDeveloper.Utils.Log;
using Riptide;
using UnityEngine;

public enum MessageId : ushort
{
    PlayerSpawn,
    PlayerPosition,
}

public static class MessageExtensions
{
    const float INT_DIVIDER = 1000f;
    const float SHORT_DIVIDER = 100f;

    #region float
    public static Message Add(this Message message, float value) => AddFloatInt(message, value);

    public static Message AddFloatInt(this Message msg, float value)
    {
        value *= INT_DIVIDER;
        return msg.AddInt((int)value);
    }

    public static float GetFloatInt(this Message msg)
    {
        float val = (float)msg.GetInt() / (float)INT_DIVIDER;
        return val;
    }
    #endregion

    #region Vector2
    /// <inheritdoc cref="AddVector2(Message, Vector2)"/>
    /// <remarks>This method is simply an alternative way of calling <see cref="AddVector2(Message, Vector2)"/>.</remarks>
    public static Message Add(this Message message, Vector2 value) => AddVector2Int(message, value);

    /// <summary>Adds a <see cref="Vector2"/> to the message.</summary>
    /// <param name="value">The <see cref="Vector2"/> to add.</param>
    /// <returns>The message that the <see cref="Vector2"/> was added to.</returns>
    public static Message AddVector2Int(this Message message, Vector2 value)
    {
        value.x *= INT_DIVIDER;
        value.y *= INT_DIVIDER;
        return message.AddInt((int)value.x).AddInt((int)value.y);
    }

    public static Message AddVector2Float(this Message message, Vector2 value)
    {
        return message.AddFloat(value.x).AddFloat(value.y);
    }

    public static Message AddVector2Short(this Message msg, Vector2 val)
    {
        val.x *= SHORT_DIVIDER;
        val.y *= SHORT_DIVIDER;
        return msg.AddShort((short)val.x).AddShort((short)val.y);
    }

    public static Message AddVector2Byte(this Message msg, Vector2 val, float maxVal = 1)
    {
        val.x *= 255;
        val.x /= maxVal;
        val.y *= 255;
        val.y /= maxVal;
        return msg.AddByte((byte)val.x).AddByte((byte)val.y);
    }

    public static Message AddVector2SByte(this Message msg, Vector2 val, float maxVal = 1)
    {
        if (val.x > 0)
            val.x *= 127;
        else
            val.x *= 128;
        val.x /= maxVal;
        if (val.y > 0)
            val.y *= 127;
        else
            val.y *= 128;
        val.y /= maxVal;
        return msg.AddSByte((sbyte)val.x).AddSByte((sbyte)val.y);
    }

    /// <summary>Retrieves a <see cref="Vector2"/> from the message.</summary>
    /// <returns>The <see cref="Vector2"/> that was retrieved.</returns>
    public static Vector2 GetVector2Int(this Message message)
    {
        int xInt = message.GetInt();
        int yInt = message.GetInt();
        float x = (float)xInt / INT_DIVIDER;
        float y = (float)yInt / INT_DIVIDER;
        return new Vector2(x, y);
    }

    public static Vector2 GetVector2Short(this Message msg)
    {
        short xShort = msg.GetShort();
        short yShort = msg.GetShort();
        float x = (float)xShort / SHORT_DIVIDER;
        float y = (float)yShort / SHORT_DIVIDER;
        return new Vector2(x, y);
    }

    public static Vector2 GetVector2Float(this Message message)
    {
        return new Vector2(message.GetFloat(), message.GetFloat());
    }

    public static Vector2 GetVector2Byte(this Message msg, float maxVal = 1)
    {
        byte xByte = msg.GetByte();
        byte yByte = msg.GetByte();
        float x = (float)(xByte * maxVal) / 255;
        float y = (float)(yByte * maxVal) / 255;
        return new Vector2(x, y);
    }

    public static Vector2 GetVector2SByte(this Message msg, float maxVal = 1)
    {
        sbyte xByte = msg.GetSByte();
        sbyte yByte = msg.GetSByte();
        float x = (float)(xByte * maxVal);
        float y = (float)(yByte * maxVal);
        if (x > 0)
            x /= 127;
        else
            x /= 128;
        if (x > 0)
            y /= 127;
        else
            y /= 128;
        return new Vector2(x, y);

    }

    #endregion

    #region Vector3
    /// <inheritdoc cref="AddVector3(Message, Vector3)"/>
    /// <remarks>This method is simply an alternative way of calling <see cref="AddVector3(Message, Vector3)"/>.</remarks>
    public static Message Add(this Message message, Vector3 value) => AddVector3Int(message, value);

    /// <summary>Adds a <see cref="Vector3"/> to the message.</summary>
    /// <param name="value">The <see cref="Vector3"/> to add.</param>
    /// <returns>The message that the <see cref="Vector3"/> was added to.</returns>
    public static Message AddVector3Int(this Message message, Vector3 value)
    {
        value.x *= INT_DIVIDER;
        value.y *= INT_DIVIDER;
        value.z *= INT_DIVIDER;
        return message.AddInt((int)value.x).AddInt((int)value.y).AddInt((int)value.z);
    }

    public static Message AddVector3Float(this Message message, Vector3 value)
    {
        return message.AddFloat(value.x).AddFloat(value.y).AddFloat(value.z);
    }

    /// <summary>Retrieves a <see cref="Vector3"/> from the message.</summary>
    /// <returns>The <see cref="Vector3"/> that was retrieved.</returns>
    public static Vector3 GetVector3Int(this Message message)
    {
        int xInt = message.GetInt();
        int yInt = message.GetInt();
        int zInt = message.GetInt();
        float x = (float)xInt / INT_DIVIDER;
        float y = (float)yInt / INT_DIVIDER;
        float z = (float)zInt / INT_DIVIDER;
        return new Vector3(x, y, z);
    }

    public static Vector3 GetVector3Float(this Message message)
    {
        return new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());
    }
    #endregion

    #region Quaternion
    /// <inheritdoc cref="AddQuaternion(Message, Quaternion)"/>
    /// <remarks>This method is simply an alternative way of calling <see cref="AddQuaternion(Message, Quaternion)"/>.</remarks>
    public static Message Add(this Message message, Quaternion value) => AddQuaternionInt(message, value);

    /// <summary>Adds a <see cref="Quaternion"/> to the message.</summary>
    /// <param name="value">The <see cref="Quaternion"/> to add.</param>
    /// <returns>The message that the <see cref="Quaternion"/> was added to.</returns>
    public static Message AddQuaternionInt(this Message message, Quaternion value)
    {
        value.w *= INT_DIVIDER;
        value.x *= INT_DIVIDER;
        value.y *= INT_DIVIDER;
        value.z *= INT_DIVIDER;
        return message.AddInt((int)value.x).AddInt((int)value.y).AddInt((int)value.z).AddInt((int)value.w);
    }
    public static Message AddQuaternionFloat(this Message message, Quaternion value)
    {
        return message.AddFloat(value.x).AddFloat(value.y).AddFloat(value.z).AddFloat(value.w);
    }

    /// <summary>Retrieves a <see cref="Quaternion"/> from the message.</summary>
    /// <returns>The <see cref="Quaternion"/> that was retrieved.</returns>
    public static Quaternion GetQuaternionInt(this Message message)
    {
        int xInt = message.GetInt();
        int yInt = message.GetInt();
        int zInt = message.GetInt();
        int wInt = message.GetInt();
        float x = (float)xInt / INT_DIVIDER;
        float y = (float)yInt / INT_DIVIDER;
        float z = (float)zInt / INT_DIVIDER;
        float w = (float)wInt / INT_DIVIDER;
        return new Quaternion(x,y,z,w);
    }

    public static Quaternion GetQuaternionFloat(this Message message)
    {
        return new Quaternion(message.GetFloat(), message.GetFloat(), message.GetFloat(), message.GetFloat());
    }
    #endregion
}
