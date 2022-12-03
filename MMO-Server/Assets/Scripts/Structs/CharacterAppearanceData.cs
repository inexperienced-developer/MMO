using InexperiencedDeveloper.Utils;
using System;

namespace InexperiencedDeveloper.MMO.Data
{
    [Serializable]
    public struct CharacterAppearanceData
    {
        public string Name;
        public byte Level;
        public ushort TotalLevel;

        //Appearance
        public byte SkinColor, HairColor, HairStyle, FacialHairStyle, EyebrowStyle, EyeColor;
        public bool BootsOn, ShirtOn, PantsOn;

        public CharacterAppearanceData(string name, byte level, ushort totalLevel,
            byte skinColor, byte hairColor, byte hairStyle, byte facialHairStyle, byte eyebrowStyle, byte eyeColor,
            byte appearanceByte)
        {
            Name = name;
            Level = level;
            TotalLevel = totalLevel;

            SkinColor = skinColor;
            HairColor = hairColor;
            HairStyle = hairStyle;
            FacialHairStyle = facialHairStyle;
            EyebrowStyle = eyebrowStyle;
            EyeColor = eyeColor;
            bool[] appearanceBools = Utilities.ByteToBools(appearanceByte, 3);
            BootsOn = appearanceBools[2];
            ShirtOn = appearanceBools[1];
            PantsOn = appearanceBools[0];
        }

    }
}
