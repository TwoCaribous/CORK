using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Dialogue;

namespace CORK.Data.Characters
{
    /// <summary>
    /// An atmospheric background character that outputs a random ambient line
    /// when the player interacts with them. Not tied to story progression.
    /// </summary>
    [CreateAssetMenu(menuName = "CORK/Character/Random", fileName = "New Random Character")]
    public class RandomCharacterData : CharacterData
    {
        [Tooltip("Pool of ambient lines this character can randomly say when interacted with.")]
        public List<DialogueLine> ambientLines = new List<DialogueLine>();

        /// <summary>
        /// Returns a randomly selected DialogueLine from the ambient pool.
        /// Returns null if the pool is empty.
        /// </summary>
        public DialogueLine GetRandomAmbientLine()
        {
            if (ambientLines == null || ambientLines.Count == 0)
                return null;

            int index = Random.Range(0, ambientLines.Count);
            return ambientLines[index];
        }
    }
}
