using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using CORK.Data;
using CORK.Data.Rooms;
using CORK.Data.Characters;
using CORK.Data.Props;
using CORK.Data.Inventory;

public class GameController : MonoBehaviour
{
    public Text displayText;
    public InputAction[] inputActions;
    [SerializeField] Image roomImageDisplay;
    [SerializeField] Material roomImageGrayscaleMaterial;
    [SerializeField] bool roomImagesAlwaysGrayscale = true;
    public PlayerInventoryData playerInventory;
    public GameFlags gameFlags;

    [HideInInspector] public RoomNavigation roomNavigation;
    [HideInInspector] public List<string> interactionDescriptionsInRoom = new List<string>();
    [HideInInspector] public HashSet<PropData> droppedProps = new HashSet<PropData>();

    PropPickupSystem propPickupSystem;
    List<string> actionLog = new List<string>();

    // ── World state snapshot structs ─────────────────────────────────────────────
    struct PropSnapshot
    {
        public bool hasBeenDiscovered;
        public bool canBeTaken;
    }

    struct ContainerSnapshot
    {
        public bool isOpen;
        public bool isLocked;
        public List<PropData> containedProps;
    }

    struct RoomSnapshot
    {
        public List<PropData>      props;
        public List<CharacterData> characters;
    }

    struct ConnectionSnapshot
    {
        public bool hasBeenVisited;
        public bool isLocked;
        public bool isHidden;
    }

    struct CharacterSnapshot
    {
        public bool hasBeenMet;
    }
    // ────────────────────────────────────────────────────────────────────────────

    Dictionary<PropData,         PropSnapshot>       snapshotProps       = new Dictionary<PropData,         PropSnapshot>();
    Dictionary<PropContainerData, ContainerSnapshot> snapshotContainers  = new Dictionary<PropContainerData, ContainerSnapshot>();
    Dictionary<RoomData,          RoomSnapshot>      snapshotRooms       = new Dictionary<RoomData,          RoomSnapshot>();
    Dictionary<RoomConnection,    ConnectionSnapshot> snapshotConnections = new Dictionary<RoomConnection,    ConnectionSnapshot>();
    Dictionary<CharacterData,     CharacterSnapshot>  snapshotCharacters  = new Dictionary<CharacterData,     CharacterSnapshot>();
    List<PropData> snapshotPlayerInventoryItems = new List<PropData>();

    Material defaultRoomImageMaterial;

    void Awake()
    {
        roomNavigation = GetComponent<RoomNavigation>();
        propPickupSystem = GetComponent<PropPickupSystem>();
        if (roomImageDisplay != null)
            defaultRoomImageMaterial = roomImageDisplay.material;
        CaptureWorldState();
    }

    void ApplyRoomImageMaterial(bool useGrayscale)
    {
        if (roomImageDisplay == null)
            return;

        if (useGrayscale && roomImageGrayscaleMaterial != null)
            roomImageDisplay.material = roomImageGrayscaleMaterial;
        else if (defaultRoomImageMaterial != null)
            roomImageDisplay.material = defaultRoomImageMaterial;
        else
            roomImageDisplay.material = null;
    }

    void CaptureWorldState()
    {
        foreach (PropData prop in Resources.FindObjectsOfTypeAll<PropData>())
        {
            snapshotProps[prop] = new PropSnapshot
            {
                hasBeenDiscovered = prop.hasBeenDiscovered,
                canBeTaken        = prop.canBeTaken
            };

            if (prop is PropContainerData container)
            {
                snapshotContainers[container] = new ContainerSnapshot
                {
                    isOpen         = container.isOpen,
                    isLocked       = container.isLocked,
                    containedProps = new List<PropData>(container.containedProps)
                };
            }
        }

        foreach (RoomData room in Resources.FindObjectsOfTypeAll<RoomData>())
        {
            snapshotRooms[room] = new RoomSnapshot
            {
                props      = new List<PropData>(room.props),
                characters = new List<CharacterData>(room.characters)
            };

            foreach (RoomConnection conn in room.connections)
            {
                snapshotConnections[conn] = new ConnectionSnapshot
                {
                    hasBeenVisited = conn.hasBeenVisited,
                    isLocked       = conn.isLocked,
                    isHidden       = conn.isHidden
                };
            }
        }

        foreach (CharacterData character in Resources.FindObjectsOfTypeAll<CharacterData>())
        {
            snapshotCharacters[character] = new CharacterSnapshot
            {
                hasBeenMet = character.hasBeenMet
            };
        }

        if (playerInventory != null)
            snapshotPlayerInventoryItems = new List<PropData>(playerInventory.items);
    }

    void Start()
    {
        if (displayText != null)
            displayText.supportRichText = true;
        LogStringWithReturn(GetCommandHelp());
        DisplayRoomText();
        DisplayLoggedText();
    }

    public string GetCommandHelp()
    {
        return
            "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
            "  CORK — Commands\n" +
            "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
            "  look                        - describe the room and anyone in it\n" +
            "  search                      - look for items in the room\n" +
            "  move                        - list exits\n" +
            "  move [exit]                 - travel to an exit (e.g. 'move glass door')\n" +
            "  inspect [thing]             - examine something closely\n" +
            "  talk [person]               - talk to someone nearby\n" +
            "  take [thing]                - pick something up\n" +
            "  open [thing]                - open a container\n" +
            "  give [item] to [person]     - hand something to someone\n" +
            "  use [item] on [thing]       - use an item on something\n" +
            "  drop [item]                 - drop something from your inventory\n" +
            "  inventory                   - list what you're carrying\n" +
            "  inventory [item]            - examine a carried item\n" +
            "  help                        - show this list\n" +
            "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━";
    }

    public void DisplayLoggedText()
    {
        string logAsText = string.Join("\n", actionLog.ToArray());
        displayText.text = logAsText;
    }

    public void DisplayRoomText()
    {
        ClearCollectionsForNewRoom();
        UpdateRoomImage();
        UnpackRoom();

        string roomText = FormatRoomName(roomNavigation.currentRoom.roomName) + "\n" + IndentText(roomNavigation.currentRoom.description);

        foreach (CharacterData character in roomNavigation.currentRoom.characters)
        {
            if (character == null) continue;
            if (character.hasBeenMet)
                roomText += "\n\n" + IndentText($"<color=#FFA500>{character.characterName}</color> - {character.description}");
            else if (!string.IsNullOrEmpty(character.description))
                roomText += "\n\n" + IndentText(character.description);
        }

        if (interactionDescriptionsInRoom.Count > 0)
            roomText += "\n\n" + string.Join("\n\n", interactionDescriptionsInRoom.ConvertAll(s => IndentText(s)).ToArray());

        LogRawStringWithReturn(roomText);
    }

    string FormatRoomName(string roomName)
    {
        int underlineLength = Mathf.Max(roomName.Length + 6, 16);
        return roomName + "\n" + new string('-', underlineLength);
    }

    string IndentText(string text, int depth = 1)
    {
        string indent = new string(' ', 4 * depth);
        if (displayText == null)
            return indent + text.Replace("\n", "\n" + indent);

        float availableWidth = Mathf.Max(displayText.GetPixelAdjustedRect().width, displayText.rectTransform.rect.width) - 12f;
        if (availableWidth <= 0f)
            return indent + text.Replace("\n", "\n" + indent);

        return WrapText(text, availableWidth, indent);
    }

    string WrapText(string text, float lineWidth, string indent)
    {
        TextGenerationSettings settings = displayText.GetGenerationSettings(new Vector2(lineWidth, 0f));
        TextGenerator generator = displayText.cachedTextGeneratorForLayout;
        StringBuilder formatted = new StringBuilder();

        string[] paragraphs = text.Split('\n');
        for (int p = 0; p < paragraphs.Length; p++)
        {
            if (p > 0)
                formatted.Append('\n');

            string paragraph = paragraphs[p].Trim();
            if (paragraph.Length == 0)
            {
                formatted.Append(indent);
                continue;
            }

            string line = indent;
            string[] words = paragraph.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (string.IsNullOrEmpty(word))
                    continue;

                string candidate = line == indent ? line + word : line + " " + word;
                float candidateWidth = generator.GetPreferredWidth(candidate, settings);

                if (candidateWidth > lineWidth && line != indent)
                {
                    formatted.AppendLine(line);
                    line = indent + word;
                }
                else
                {
                    line = candidate;
                }
            }

            formatted.Append(line);
        }

        return formatted.ToString();
    }

    public void UpdateRoomImage()
    {
        if (roomImageDisplay == null) return;

        if (roomNavigation.currentRoom.roomImage != null)
        {
            roomImageDisplay.sprite = roomNavigation.currentRoom.roomImage;
            roomImageDisplay.color = Color.white;
            ApplyRoomImageMaterial(roomImagesAlwaysGrayscale);
        }
        else
        {
            roomImageDisplay.sprite = null;
            roomImageDisplay.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            ApplyRoomImageMaterial(false);
        }
    }

    /// <summary>
    /// Shows a prop's image if it has one; falls back to the current room image.
    /// Call from Inspect so the panel reflects what the player is looking at.
    /// </summary>
    public void ShowPropImage(Sprite propImage)
    {
        if (roomImageDisplay == null) return;
        Sprite toShow = propImage != null ? propImage : roomNavigation.currentRoom.roomImage;
        if (toShow != null)
        {
            roomImageDisplay.sprite = toShow;
            roomImageDisplay.color = Color.white;
            bool useGrayscale = toShow == roomNavigation.currentRoom.roomImage && roomImagesAlwaysGrayscale;
            ApplyRoomImageMaterial(useGrayscale);
        }
        else
        {
            roomImageDisplay.sprite = null;
            roomImageDisplay.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            ApplyRoomImageMaterial(false);
        }
    }

    void UnpackRoom()
    {
        roomNavigation.UnpackExitsInRoom();
    }

    void ClearCollectionsForNewRoom()
    {
        interactionDescriptionsInRoom.Clear();
        roomNavigation.ClearExits();
    }

    public void LogStringWithReturn(string stringToAdd)
    {
        actionLog.Add(IndentText(stringToAdd));
    }

    public void LogRawStringWithReturn(string stringToAdd)
    {
        actionLog.Add(stringToAdd);
    }

    public bool TryTakeFromRoom(PropData prop)
    {
        return propPickupSystem.TryPickUp(prop, roomNavigation.currentRoom);
    }

    void Update()
    {
    }

    void OnDestroy()
    {
        RestoreWorldState();
    }

    void RestoreWorldState()
    {
        foreach (var kvp in snapshotProps)
        {
            kvp.Key.hasBeenDiscovered = kvp.Value.hasBeenDiscovered;
            kvp.Key.canBeTaken        = kvp.Value.canBeTaken;
        }

        foreach (var kvp in snapshotContainers)
        {
            kvp.Key.isOpen         = kvp.Value.isOpen;
            kvp.Key.isLocked       = kvp.Value.isLocked;
            kvp.Key.containedProps = new List<PropData>(kvp.Value.containedProps);
        }

        foreach (var kvp in snapshotRooms)
        {
            kvp.Key.props      = new List<PropData>(kvp.Value.props);
            kvp.Key.characters = new List<CharacterData>(kvp.Value.characters);
        }

        foreach (var kvp in snapshotConnections)
        {
            kvp.Key.hasBeenVisited = kvp.Value.hasBeenVisited;
            kvp.Key.isLocked       = kvp.Value.isLocked;
            kvp.Key.isHidden       = kvp.Value.isHidden;
        }

        foreach (var kvp in snapshotCharacters)
            kvp.Key.hasBeenMet = kvp.Value.hasBeenMet;

        if (playerInventory != null)
            playerInventory.items = new List<PropData>(snapshotPlayerInventoryItems);

        if (gameFlags != null)
            gameFlags.ClearAll();

        droppedProps.Clear();
    }
}
