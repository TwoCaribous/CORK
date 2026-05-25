# CORK — World Building Guide

### For Designers & Writers

---

## Table of Contents

**Getting Started**

- [Overview](#overview)
- [The Asset Menu](#the-asset-menu)
- [Naming Conventions](#naming-conventions-recommended)
- [Things That Reset Between Play Sessions](#things-that-reset-between-play-sessions)

**Building the World**

- [Rooms](#rooms)
- [Props (Objects)](#props-objects)
- [Characters](#characters)
- [Dialogue](#dialogue)

**Story Systems**

- [Game Flags](#game-flags)
- [Player Inventory](#player-inventory)

**Player Reference**

- [Player Commands Reference](#player-commands-reference)

**Walkthroughs**

- [Step-by-Step: Adding a New Room](#step-by-step-adding-a-new-room)
- [Step-by-Step: Adding a New Prop](#step-by-step-adding-a-new-prop)
- [Step-by-Step: Adding a New Story Character](#step-by-step-adding-a-new-story-character)

**For Developers**

- [Code Architecture Overview](#code-architecture-overview)
- [Adding a New Command](#adding-a-new-command)

---

## Overview

CORK is a text adventure game built in Unity. Everything in the game world — rooms, objects, characters, dialogue — is stored as a **ScriptableObject (SO)** asset in the Project window. No coding is required to build the world. You create assets, fill in fields, and wire them together using drag-and-drop in the Inspector.

All state (which items have been picked up, which containers are open, which characters have been spoken to, etc.) resets cleanly every time you stop Play Mode. You can iterate freely.

---

## The Asset Menu

Right-click anywhere in the Project window → **Create** to find all CORK asset types:

| Menu Path                    | What it creates                             |
| ---------------------------- | ------------------------------------------- |
| CORK > Room                  | A room/location                             |
| CORK > Prop                  | A simple object in a room                   |
| CORK > Prop Container        | An object that holds other objects          |
| CORK > Character > Essential | A story character with conditional dialogue |
| CORK > Character > Random    | An atmospheric background character         |
| CORK > Dialogue Entry        | A sequence of dialogue lines                |
| CORK > Game Flags            | The world flag tracker (one per project)    |

---

## Naming Conventions (Recommended)

| Asset Type          | Prefix       | Example                   |
| ------------------- | ------------ | ------------------------- |
| Room                | `Room_`      | `Room_SecretaryOffice`    |
| Prop                | `Prop_`      | `Prop_FireExtinguisher`   |
| Prop Container      | `Container_` | `Container_FilingCabinet` |
| Essential Character | `Char_`      | `Char_DeanHartwell`       |
| Random Character    | `NPC_`       | `NPC_StudentInHallway`    |
| Dialogue Entry      | `Dlg_`       | `Dlg_Dean_FirstMeeting`   |

---

## Things That Reset Between Play Sessions

The following all reset automatically every time you stop Play Mode:

- Which props the player has taken (rooms restock)
- Which containers are open or locked
- The player's inventory
- Which characters have been met
- Which connections have been visited (display names restore)
- All active game flags
- Dropped props

This means you can **freely iterate and test** without manually cleaning up state.

---

## Rooms

**Asset:** `CORK > Room`

Rooms are the locations the player moves between. Each room is its own asset.

### Fields

| Field            | What it does                                                                                                                      |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| **Room Name**    | The name displayed when the player enters or types `look`. Also used as the navigation key after the player has visited the room. |
| **Description**  | The long-form atmospheric text shown when the player looks around. Write this richly — it is the main scene-setting text.         |
| **Floor Number** | Which floor of the building the room is on. For organizational reference.                                                         |
| **Room Image**   | A Sprite shown in the image panel while the player is in this room. Leave empty for no image.                                     |
| **Props**        | All objects present in this room. Drag `PropData` or `PropContainerData` assets into this list.                                   |
| **Characters**   | All NPCs present in this room. Drag `Essential` or `Random` character assets here.                                                |
| **Connections**  | The exits from this room. Each entry is a `RoomConnection` (see below).                                                           |

> **Tip:** The `description` does NOT list props or characters — those are discovered by the player using `search` and `look`. Write the description as pure atmosphere.

### Room Connections (Exits)

Each entry in the **Connections** list is an inline connection block. You don't create a separate asset — just click the `+` on the list.

| Field                | What it does                                                                                                                                                                                                                                                       |
| -------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Connected Room**   | Drag the destination `RoomData` asset here.                                                                                                                                                                                                                        |
| **Direction**        | A label like `north`, `east`, `up`, `south`. Shown in exit listings.                                                                                                                                                                                               |
| **Door Description** | Optional flavour text describing the passage (e.g. `a heavy fire door`, `a narrow ventilation shaft`).                                                                                                                                                             |
| **Display Name**     | The name shown for this exit **before** the player has visited the destination. Once visited, the actual Room Name is shown instead. Use this to preserve mystery (e.g. `Unmarked Door` instead of `Dean's Office`). Leave empty to use the Room Name immediately. |
| **Is Locked**        | Check this to block passage until unlocked.                                                                                                                                                                                                                        |
| **Locked Message**   | Text shown when the player tries to use a locked exit. Defaults to a generic message if left empty.                                                                                                                                                                |
| **Required Key**     | Drag a `PropData` asset here. The player must `use [key] on [exit]` to unlock it. The key is **not** consumed — the door simply becomes unlocked. Leave empty if no key is needed.                                                                                 |
| **Is Hidden**        | Hidden exits do not appear in `move` listings. Use this for secret passages that need to be revealed by another mechanic first.                                                                                                                                    |
| **Has Been Visited** | Set automatically at runtime when the player passes through. Do not set this manually.                                                                                                                                                                             |

> **Important:** Connections are one-directional. If Room A connects to Room B, Room B does **not** automatically connect back to Room A. You must add a connection on both rooms if you want two-way travel.

---

## Props (Objects)

**Asset:** `CORK > Prop`

Props are objects the player can find, examine, pick up, carry, use, and drop.

### Fields

| Field                 | What it does                                                                                                                                                       |
| --------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Prop Name**         | The name the player types to interact with this prop. Keep it short and natural (e.g. `fire extinguisher`, `sticky note`).                                         |
| **Description**       | The text shown when the player types `inspect [prop]`. Write this in detail — it is the player's close-up view of the object.                                      |
| **Can Be Taken**      | If checked, the player can `take` this prop and carry it in their inventory. Uncheck for scenery that cannot be picked up.                                         |
| **Take Message**      | Custom text shown when the player successfully picks up this prop. Leave empty for the default.                                                                    |
| **Cant Take Message** | Custom text shown when the player tries to take this prop but can't. Leave empty for a default refusal.                                                            |
| **Prop Image**        | A Sprite shown in the image panel when the player `inspect`s this prop. The room image restores when the player `look`s or moves. Leave empty for text-only props. |

> **Has Been Discovered** is a runtime flag. It is set automatically the first time the player inspects the prop. On first discovery, the game displays `"You'll remember this as: [Prop Name]."` so the player knows what to type for future commands.

### Prop Containers

**Asset:** `CORK > Prop Container`

A Prop Container is a prop that also holds other props inside it (e.g. a desk, a locker, a cardboard box). It has all the same fields as a regular prop, plus:

| Field               | What it does                                                                                          |
| ------------------- | ----------------------------------------------------------------------------------------------------- |
| **Is Open**         | Whether the container starts open. Usually leave unchecked.                                           |
| **Is Locked**       | Whether the container starts locked.                                                                  |
| **Locked Message**  | Text shown when the player tries to open it while locked.                                             |
| **Contained Props** | The props inside this container. Drag other `PropData` assets here.                                   |
| **Required Key**    | Drag a `PropData` here. The player must `use [key] on [container]` to unlock it. Key is not consumed. |

> **Important:** A Prop Container is placed in a room's **Props** list like any other prop. The player uses `search` to discover it, `inspect` to examine it, `open` to open it, and `take` to pick up items inside.

---

## Characters

### Essential Characters

**Asset:** `CORK > Character > Essential`

Essential characters are story-critical NPCs. They respond conditionally based on what the player is carrying or what flags have been set.

| Field                     | What it does                                                                                                       |
| ------------------------- | ------------------------------------------------------------------------------------------------------------------ |
| **Character Name**        | The name displayed in room descriptions and dialogue.                                                              |
| **Description**           | Shown when the player `look`s at the room (before meeting them) or after meeting them.                             |
| **Portrait**              | Optional sprite for this character.                                                                                |
| **Conditional Dialogues** | A priority-ordered list of condition + dialogue pairs. Evaluated top-to-bottom; the first passing entry is played. |
| **Item Interactions**     | Responses triggered when the player `give`s this character a specific item. The item is consumed.                  |
| **Primary Dialogue**      | The fallback conversation used when no conditional dialogue passes. This is what the character says by default.    |

#### How Conditional Dialogue Works

Each entry in **Conditional Dialogues** has:

- A **condition** that must be met for this dialogue to play
- A **Dialogue Entry** asset that plays if the condition passes

Conditions are evaluated **top to bottom**. The first one that passes wins. The **Primary Dialogue** is the final fallback if nothing passes.

**Condition Types:**

| Condition Type         | When it passes                                                   |
| ---------------------- | ---------------------------------------------------------------- |
| **None**               | Always passes. Use this to slot an unconditional entry mid-list. |
| **Has Item**           | The player is currently carrying a specific prop.                |
| **Does Not Have Item** | The player is NOT carrying a specific prop.                      |
| **Has Flag**           | A named world flag is currently active.                          |
| **Does Not Have Flag** | A named world flag is NOT currently active.                      |

**Example setup for a character named "Secretary":**

1. Entry 1 — Condition: `Has Item` → `Stolen Keycard` → plays dialogue: _"Wait... where did you get that?"_
2. Entry 2 — Condition: `Has Flag` → `"metDean"` → plays dialogue: _"You've spoken to the Dean already? Hmm."_
3. Primary Dialogue → _"Can I help you? You look lost."_

#### Item Interactions (Give Command)

Each entry in **Item Interactions** has:

- **Expected Item** — the prop the player must `give` to trigger this
- **Response** — a `Dialogue Entry` the character speaks when given that item

The item is **removed from the player's inventory** when the interaction fires. If the character has no matching item interaction, they say they're not interested.

### Random Characters

**Asset:** `CORK > Character > Random`

Random characters are atmospheric background NPCs with no story relevance. When the player `talk`s to them, a random line from their pool is selected.

| Field              | What it does                                                                                       |
| ------------------ | -------------------------------------------------------------------------------------------------- |
| **Character Name** | Display name.                                                                                      |
| **Description**    | Shown when looking at the room.                                                                    |
| **Portrait**       | Optional sprite.                                                                                   |
| **Ambient Lines**  | A pool of `DialogueLine` entries. One is chosen at random when the player talks to this character. |

---

## Dialogue

### Dialogue Entry

**Asset:** `CORK > Dialogue Entry`

A Dialogue Entry is a sequence of lines displayed one after another when a conversation plays. Assign these to character conditional slots or item interactions.

| Field     | What it does                                                                              |
| --------- | ----------------------------------------------------------------------------------------- |
| **Lines** | An ordered list of `DialogueLine` entries. Each line has a **Speaker Name** and **Text**. |

> Create one Dialogue Entry asset per distinct conversation. Name them clearly (e.g. `DlgSecretary_HasKeycard`, `DlgDean_FirstMeeting`).

### Dialogue Line (inline — no separate asset)

A `DialogueLine` lives inside a Dialogue Entry. It has:

- **Speaker Name** — who is speaking this line (e.g. `Secretary`, `Dean Hartwell`)
- **Text** — what they say

---

## Game Flags

**Asset:** `CORK > Game Flags`

Game Flags are named boolean switches that represent persistent world state. Think of them as story checkpoints: `"metDean"`, `"alarmPulled"`, `"canvasHacked"`. Any part of the game can check whether a flag is active and react accordingly.

There is **one** Game Flags asset for the whole project. Create it once and assign it to the **Game Flags** slot on the `GameController` in the scene.

### How Flags Are Used Without Code (Designer Side)

Flags are referenced by plain strings in **Dialogue Conditions** on Essential Characters. No code required — you just type the flag name.

- Set **Condition Type** to `Has Flag` and type the flag name (e.g. `metDean`) in **Required Flag**.
- The character will use that dialogue entry only after the flag has been activated.
- Use `Does Not Have Flag` for the opposite — dialogue that plays _before_ a story beat happens.

### How Flags Are Set (Code Side)

Flags are activated from within a command's `RespondToInput` method. You have three methods available on `controller.gameFlags`:

```csharp
// Activate a flag
controller.gameFlags.SetFlag("metDean");

// Check if a flag is active
bool alreadyMet = controller.gameFlags.HasFlag("metDean");

// All flags clear automatically on Play Mode stop — no manual reset needed
```

Flags are most commonly set inside a custom command (see **Adding a New Command** below), but you can also set them inside an existing command like `Use` or `Talk` when a specific story moment is triggered.

### Complete End-to-End Flag Example

**Scenario:** The player speaks to the Dean. After that conversation, the Secretary has new dialogue.

**Step 1 — Set the flag when the player talks to the Dean.**
In `Talk.cs` (or a custom command), after the Dean's dialogue fires, add:

```csharp
if (character.characterName == "Dean Hartwell")
    controller.gameFlags.SetFlag("metDean");
```

**Step 2 — Author the Secretary's conditional dialogue in the Inspector.**

- Open `Char_Secretary`.
- In **Conditional Dialogues**, add an entry:
  - Condition Type: `Has Flag`
  - Required Flag: `metDean`
  - Dialogue: _drag `Dlg_Secretary_PostDean`_
- Leave **Primary Dialogue** as the default conversation.

Now, before the Dean is visited, the Secretary uses primary dialogue. After the player talks to the Dean (flag is set), the Secretary's next conversation plays the post-Dean entry instead.

### Flag Naming Conventions

Use camelCase, be descriptive, and prefix with a context hint:

| Example Flag            | Meaning                                 |
| ----------------------- | --------------------------------------- |
| `metDean`               | Player has spoken to the Dean           |
| `alarmPulled`           | The fire alarm has been activated       |
| `canvasUnlocked`        | The student canvas account was accessed |
| `keycard_secretarySeen` | Secretary has noticed the keycard       |

> All flags **clear automatically** when Play Mode ends. You never need to manually reset them between test runs.

---

## Player Inventory

The player's inventory is managed automatically. There is nothing to set up per-character or per-room for inventory to work — it is driven by:

- `take [prop]` — adds a prop to inventory (only if `Can Be Taken` is checked)
- `drop [prop]` — removes from inventory, places prop on room floor (always takeable again)
- `give [item] to [character]` — removes from inventory, triggers character's item interaction
- `use [item] on [thing]` — checks item against door keys and container keys
- `inventory` — lists carried items
- `inventory [item]` — shows the description of a carried item

---

## Player Commands Reference

| Command                   | What it does                                                                                                                                    |
| ------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| `look`                    | Describes the current room and lists characters present.                                                                                        |
| `search`                  | Lists all props in the room (by description). Marks them as discovered.                                                                         |
| `move`                    | Lists all visible exits from the current room.                                                                                                  |
| `move [exit]`             | Travels to an exit. Use the direction (`north`), display name, or room name.                                                                    |
| `inspect [thing]`         | Examines a prop closely. Shows the prop's image if it has one. On first inspect, registers its name so you can reference it in future commands. |
| `talk [person]`           | Talks to a character. Triggers their current dialogue based on conditions.                                                                      |
| `take [thing]`            | Picks up a prop and adds it to inventory.                                                                                                       |
| `open [thing]`            | Opens a container. Lists its contents if unlocked.                                                                                              |
| `give [item] to [person]` | Hands a carried item to a character. Triggers their item interaction response.                                                                  |
| `use [item] on [thing]`   | Uses an item on a door, container, or character. Unlocks if the item matches the required key.                                                  |
| `drop [item]`             | Drops a carried item onto the room floor. It can be taken again by anyone.                                                                      |
| `inventory`               | Lists everything the player is carrying.                                                                                                        |
| `inventory [item]`        | Shows the description of a specific carried item.                                                                                               |
| `help`                    | Displays the full command list in-game.                                                                                                         |

---

## Step-by-Step: Adding a New Room

1. Right-click in Project → **Create > CORK > Room**. Name the asset (e.g. `Room_LibraryStacks`).
2. Fill in **Room Name**, **Description**, and optionally assign a **Room Image** sprite.
3. Under **Connections**, click `+` and set **Connected Room** to an existing room. Fill in **Direction** and optionally **Door Description** and **Display Name**.
4. Go to the _other_ room's asset and add a matching connection back if you want two-way travel.
5. Drag the new room asset into the scene's **Room Navigation** component (or the appropriate starting room's connection list).

## Step-by-Step: Adding a New Prop

1. Right-click in Project → **Create > CORK > Prop**. Name the asset (e.g. `Prop_StickyNote`).
2. Fill in **Prop Name** (what the player types), **Description** (what they read on inspect).
3. Check **Can Be Taken** if the player should be able to pick it up.
4. Open the destination **Room** asset and drag this prop into its **Props** list.

## Step-by-Step: Adding a New Story Character

1. Right-click → **Create > CORK > Character > Essential**. Name it (e.g. `Char_Secretary`).
2. Fill in **Character Name** and **Description**.
3. Create one or more **Dialogue Entry** assets (right-click → **Create > CORK > Dialogue Entry**). Add lines to each.
4. On the character asset, add entries to **Conditional Dialogues**. Set the condition and assign the Dialogue Entry.
5. Assign a **Primary Dialogue** as the fallback.
6. Open the destination **Room** asset and drag this character into its **Characters** list.

---

## Code Architecture Overview

This section is for anyone who wants to write code that hooks into the existing systems. You do not need to understand this to build the world — it is only relevant if you are adding new commands or new mechanics.

### The Hub: `GameController`

`GameController` is a MonoBehaviour on the main scene GameObject. It is the central point everything routes through. When writing any command, you receive a reference to it. The most useful things you can access from it:

| Property / Method                        | What it gives you                                                                                         |
| ---------------------------------------- | --------------------------------------------------------------------------------------------------------- |
| `controller.roomNavigation.currentRoom`  | The `RoomData` asset for the room the player is currently in                                              |
| `controller.playerInventory`             | The player's inventory — use `.HasItem()`, `.AddItem()`, `.RemoveItem()`                                  |
| `controller.gameFlags`                   | The world flag tracker — use `.HasFlag()`, `.SetFlag()`                                                   |
| `controller.droppedProps`                | The set of props that were dropped by the player this session                                             |
| `controller.LogStringWithReturn(string)` | Queues a line of text for display (does not display immediately)                                          |
| `controller.DisplayLoggedText()`         | Flushes all queued text to the screen — call this at the end of a command if you need an immediate update |
| `controller.DisplayRoomText()`           | Re-describes the current room and updates the room image                                                  |
| `controller.UpdateRoomImage()`           | Updates the image panel to show the current room's image                                                  |
| `controller.ShowPropImage(Sprite)`       | Shows a specific sprite in the image panel (falls back to room image if null)                             |
| `controller.TryTakeFromRoom(PropData)`   | Attempts to move a prop from the room into the player's inventory                                         |

### How Input Works

When the player types something and presses Enter:

1. `TextInput` receives the raw string, lowercases it, and splits it on spaces into a `string[]`.
2. It loops through `GameController.inputActions[]` looking for an `InputAction` whose `keyWord` matches the first word.
3. When a match is found, it calls `inputAction.RespondToInput(controller, separatedInputWords)`.
4. After the command returns, `TextInput` calls `controller.DisplayLoggedText()` to flush everything to the screen.

This means inside a command you call `LogStringWithReturn()` as many times as you want and the player sees all of it at once when the method returns. You do not need to call `DisplayLoggedText()` yourself unless you need an update mid-command (rare).

### The InputAction Pattern

Every command is a `ScriptableObject` that extends `InputAction`. This means commands are **assets in the Project window**, not MonoBehaviours on GameObjects. The architecture is:

```
InputAction (abstract ScriptableObject)
  └─ keyWord          — the word the player types to trigger this command
  └─ RespondToInput() — override this to implement the command
```

Commands are registered by dragging their assets into the **Input Actions** array on `GameController`.

### The ScriptableObject State System

All game data lives on ScriptableObjects. Because SOs persist in memory, any changes made at runtime (items removed from rooms, containers opened, flags set) would normally survive between Play sessions. To prevent this, `GameController` takes a full **snapshot** of every mutable SO field the moment Play Mode starts (`Awake`), and **restores** the entire snapshot the moment Play Mode ends (`OnDestroy`). You do not need to manage this — but if you add a new mutable field to an existing SO type, you should add it to the snapshot structs in `GameController.cs`.

---

## Adding a New Command

Adding a command requires creating one `.cs` file and one asset. Here is the complete process.

### Step 1 — Create the Script

Create a new C# file in `Assets/Scripts/`. The class must extend `InputAction` and be decorated with `[CreateAssetMenu]`.

```csharp
using UnityEngine;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/MyCommand")]
public class MyCommand : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        // separatedInputWords[0] is the keyword (e.g. "mycommand")
        // separatedInputWords[1], [2], etc. are the words that followed it

        // Always check length before accessing indices beyond [0]
        if (separatedInputWords.Length < 2)
        {
            controller.LogStringWithReturn("Usage: mycommand [thing]");
            return;
        }

        string target = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);

        // Do something with target...
        controller.LogStringWithReturn("You do something to " + target + ".");
    }
}
```

### Step 2 — Create the Asset

1. Right-click in Project → **Create > TextAdventure > InputActions > MyCommand**.
2. Name the asset (e.g. `MyCommand`).
3. In the Inspector, set the **Key Word** field to the exact word the player will type (lowercase, e.g. `mycommand`).

### Step 3 — Register the Command

1. Select the **GameController** GameObject in the scene Hierarchy.
2. In the Inspector, find the **Input Actions** array.
3. Increase the size by 1 and drag your new asset into the empty slot.

That's all. The command is live.

### Practical Example: A `flush` Command That Sets a Flag

Scenario: the player types `flush toilet` and the game sets a flag `toiletFlushed`.

```csharp
[CreateAssetMenu(menuName = "TextAdventure/InputActions/Flush")]
public class Flush : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (separatedInputWords.Length < 2)
        {
            controller.LogStringWithReturn("Flush what?");
            return;
        }

        string target = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);

        // Check if there's a toilet prop in the current room
        bool foundToilet = false;
        foreach (var prop in controller.roomNavigation.currentRoom.props)
        {
            if (prop != null && prop.propName.ToLower() == target)
            {
                foundToilet = true;
                break;
            }
        }

        if (!foundToilet)
        {
            controller.LogStringWithReturn("You don't see a " + target + " here.");
            return;
        }

        if (controller.gameFlags.HasFlag("toiletFlushed"))
        {
            controller.LogStringWithReturn("It's already been flushed.");
            return;
        }

        controller.gameFlags.SetFlag("toiletFlushed");
        controller.LogStringWithReturn("You flush the toilet. Something rattles inside the tank.");
    }
}
```

Create the asset, set `keyWord` to `flush`, add it to `GameController.inputActions[]`. Now `flush toilet` works in-game and gates any dialogue conditions using `Has Flag → toiletFlushed`.

### Tips for Command Writing

- **Always validate `separatedInputWords.Length`** before accessing any index beyond `[0]`.
- **Use `string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1)`** to reconstruct multi-word targets (e.g. `fire extinguisher` from `use fire extinguisher on door`).
- **String comparisons should use `StringComparison.OrdinalIgnoreCase`** — input is already lowercased by `TextInput`, but prop names in assets might have mixed case.
- **Do not call `DisplayLoggedText()` yourself** unless you have a specific reason — `TextInput` calls it automatically after every command returns.
- **Do not hold references to rooms or props in the command asset itself** — command assets are shared SOs and should be stateless. All state lives on `GameController` and the data SOs.
