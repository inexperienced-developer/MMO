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
            bool bootsOn, bool shirtOn, bool pantsOn)
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
            BootsOn = bootsOn;
            ShirtOn = shirtOn;
            PantsOn = pantsOn;
        }

    }
}
