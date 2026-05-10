using UnityEngine;

namespace CORK.Data.Props
{
    /// <summary>
    /// A prop (object) that exists in a room. Props can be examined and,
    /// if canBeTaken is true, picked up by the player.
    /// </summary>
    [CreateAssetMenu(menuName = "CORK/Prop", fileName = "New Prop")]
    public class PropData : ScriptableObject
    {
        [Tooltip("The display name of this prop as it appears in the game.")]
        public string propName;

        [TextArea(2, 6)]
        [Tooltip("The text shown when the player examines this prop.")]
        public string description;

        [Tooltip("Whether the player can pick up this prop and add it to their inventory.")]
        public bool canBeTaken;

        [Tooltip("Optional sprite shown when this prop is examined. Leave empty for text-only props.")]
        public Sprite propImage;
    }
}
