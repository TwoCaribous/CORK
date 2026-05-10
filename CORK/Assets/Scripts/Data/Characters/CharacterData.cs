using UnityEngine;

namespace CORK.Data.Characters
{
    /// <summary>
    /// Abstract base ScriptableObject for all characters and NPCs in the game.
    /// Use EssentialCharacterData or RandomCharacterData to create concrete instances.
    /// 
    /// Keeping a shared base allows RoomData to hold a single List&lt;CharacterData&gt;
    /// containing both types polymorphically.
    /// </summary>
    public abstract class CharacterData : ScriptableObject
    {
        [Tooltip("The display name of this character.")]
        public string characterName;

        [TextArea(2, 6)]
        [Tooltip("A brief description of this character shown when the player looks at them.")]
        public string description;

        [Tooltip("Classifies this character as Essential (story-critical) or Random (atmospheric).")]
        public CharacterType characterType;

        [Tooltip("Optional portrait sprite shown during interactions.")]
        public Sprite portrait;
    }
}
