namespace CORK.Data.Characters
{
    /// <summary>
    /// Classifies a character's role and interactivity level in the game world.
    /// </summary>
    public enum CharacterType
    {
        /// <summary>
        /// An important character tied to story or quest progression.
        /// Has meaningful primary dialogue.
        /// </summary>
        Essential,

        /// <summary>
        /// An atmospheric background character.
        /// Outputs a random ambient line when interacted with.
        /// </summary>
        Random
    }
}
