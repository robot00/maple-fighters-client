﻿using Game.Common;

namespace Scripts.Gameplay.Player
{
    public interface ISpawnedCharacterDetails
    {
        CharacterSpawnDetailsParameters GetCharacterDetails();
    }
}