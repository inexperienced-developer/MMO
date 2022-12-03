using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoTest : MonoBehaviour
{
    [Header("Bits To Byte Settings")]
    public bool a;
    public bool b;
    public bool c;

    [ContextMenu("Bits To Byte test")]
    public byte BitsToByteTest()
    {
        BitArray total = new BitArray(new byte[1]);
        total.Set(0, a);
        total.Set(1, b);
        total.Set(2, c);
        IDLogger.Log(Utilities.BitsToByte(total).ToString());
        return Utilities.BitsToByte(total);
    }

    [ContextMenu("Bits To Byte -> Byte to Bools")]
    public void BitsToByteByteToBools()
    {
        byte b = BitsToByteTest();
        bool[] bools = new bool[3];
        bools = Utilities.ByteToBools(b, 3);
        foreach(bool b2 in bools)
        {
            IDLogger.Log(b2.ToString());
        }
    }
}
