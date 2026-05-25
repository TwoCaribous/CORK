using System;
using UnityEngine;
using CORK.Data.Props;
using CORK.Data.Dialogue;

namespace CORK.Data.Characters
{
    /// <summary>
    /// Pairs a prop with the dialogue an EssentialCharacter speaks when given that prop.
    /// Lives inline inside EssentialCharacterData.itemInteractions — no separate asset required.
    ///
    /// The item is consumed when the interaction fires (removed from inventory).
    /// </summary>
    [Serializable]
    public class ItemInteraction
    {
        [Tooltip("The prop the player must give to trigger this interaction.")]
        public PropData expectedItem;

        [Tooltip("The dialogue the character speaks when handed this item.")]
        public DialogueEntry response;
    }
}
