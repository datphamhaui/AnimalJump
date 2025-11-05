# Menu System Logic - Win/Lose/Pause

## 📋 Tổng Quan

Game có 3 menu chính cho các trạng thái game:
- 🏆 **WinMenu** - Khi hoàn thành level (đạt target score)
- 💀 **LoseMenu** - Khi game over (chết hoặc rơi)
- ⏸️ **PauseMenu** - Khi tạm dừng game

## 🔄 Logic Flow

### 1. Win Menu (Thắng Level)
```
GameManager.CheckWinCondition()
  ↓ [Score >= TargetScore]
GameManager.GameWin()
  ↓
- Calculate stars (based on _missCount)
- Save progress (LevelProgressManager.CompleteLevel())
- Unlock next level
- Trigger OnGameWin event → WinMenu receives stars
  ↓
MenuManager.SwitchMenu(MenuType.Win)
  ↓
WinMenu.SetEnable()
  ↓ Display:
- ⭐ Stars (3/2/1 based on misses)
- 💰 Coins (base × star multiplier)
- 📊 Level score
- 🎮 Buttons: Close, Home, Retry, Next
```

**Trigger Location:**
```csharp
// File: GameManager.cs
// Method: CheckWinCondition()
// Called from: SetScore() (mỗi khi score tăng)

if (_scoreManager.Score >= currentLevelData.targetScore)
{
    GameWin();
}
```

**Star Calculation:**
```csharp
// File: GameManager.cs
// Method: CalculateStars()

private int CalculateStars()
{
    if (_missCount == 0) return 3; // Perfect run
    if (_missCount == 1) return 2; // Good run
    return 1;                       // Completed
}
```

---

### 2. Lose Menu (Thua Game)
```
Player dies OR falls off
  ↓
PlayerBehaviour.OnPlayerDeath event
OR
PlayerRenderer.OnInvisible event
OR
Piece.OnGameOver event (miss landing)
  ↓
GameManager.GameEnd()
  ↓
MenuManager.SwitchMenu(MenuType.Lose)
  ↓
LoseMenu.SetEnable()
  ↓ Display:
- 😢 "Oh.." + "You lose.."
- 📊 Level score (current score)
- 💰 Coins = 0 (no reward)
- 🎮 Buttons: Close, Home, Retry
```

**Trigger Locations:**
```csharp
// File: GameManager.cs
// Subscribed events in OnEnable():

Piece.OnGameOver      += HandleMiss;      // Miss landing
PlayerBehaviour.OnPlayerDeath += GameEnd; // Player dies
_playerRenderer.OnInvisible   += GameEnd; // Fall off screen

// All lead to:
public void GameEnd()
{
    _menuController.SwitchMenu(MenuType.Lose);
}
```

---

### 3. Pause Menu (Tạm Dừng)
```
Player presses Pause button
  ↓
UI Button callback OR Input system
  ↓
MenuManager.OpenMenu(MenuType.Pause)
  ↓
PauseMenu.SetEnable()
  ↓
Time.timeScale = 0f (freeze game)
  ↓ Display:
- 🎮 Buttons: Resume, Restart, Home
```

**Trigger:**
```csharp
// Manually triggered from:
// - UI Button in gameplay UI
// - Keyboard input (ESC key)
// - Mobile touch button

// Example implementation:
void Update()
{
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        if (MenuManager.GetInstance().GetCurrentMenu == MenuType.Gameplay)
        {
            MenuManager.GetInstance().OpenMenu(MenuType.Pause);
        }
    }
}
```

**Time Control:**
```csharp
// File: PauseMenu.cs

public override void SetEnable()
{
    Time.timeScale = 0f; // Pause
}

public override void SetDisable()
{
    Time.timeScale = 1f; // Resume
}
```

---

## 🎯 Key Differences

| Feature | Win Menu | Lose Menu | Pause Menu |
|---------|----------|-----------|------------|
| **Trigger** | Auto (score check) | Auto (death/fall) | Manual (button/input) |
| **Stars** | ⭐⭐⭐ (0-3) | None | N/A |
| **Coins** | 💰 Yes (with multiplier) | 💰 0 | N/A |
| **Next Level** | ✅ Available | ❌ N/A | N/A |
| **Time.timeScale** | Normal | Normal | **0 (paused)** |
| **Progress Saved** | ✅ Yes | ❌ No | N/A |

---

## 📍 Menu Manager System

### Registration
```csharp
// File: MenuManager.cs
// All menus must be registered in Inspector

[SerializeField] private List<Menu> _menus;
// Add: WinMenu, LoseMenu, PauseMenu components
```

### Switch Logic
```csharp
public void SwitchMenu(MenuType type)
{
    CloseMenu();  // Disable current menu
    OpenMenu(type); // Enable new menu
}
```

### Menu Stack
- **Stack-based**: Menus pushed/popped for back navigation
- **Current tracking**: `_currentMenu` tracks active menu
- **Validation**: Warns if menu not registered

---

## 🔧 Setup Requirements

### 1. GameManager Setup
✅ Already configured:
- Win condition check in `SetScore()`
- Lose condition via events subscription
- Menu switching calls

### 2. MenuManager Setup
In Unity Inspector:
- Add **WinMenu** component → Canvas
- Add **LoseMenu** component → Canvas
- Add **PauseMenu** component → Canvas
- Drag all to `_menus` list in MenuManager

### 3. UI Canvas Hierarchy
```
UI Canvas
├── WinMenu (LoseMenu.cs) ← MenuType.Win
├── LoseMenu (LoseMenu.cs) ← MenuType.Lose
└── PauseMenu (PauseMenu.cs) ← MenuType.Pause
```

---

## 🐛 Debug Tips

### Check Current Menu
```csharp
MenuType current = MenuManager.GetInstance().GetCurrentMenu;
Debug.Log($"Current menu: {current}");
```

### Force Show Menu (Testing)
```csharp
// In Unity Console or debug script:
MenuManager.GetInstance().SwitchMenu(MenuType.Win);
MenuManager.GetInstance().SwitchMenu(MenuType.Lose);
MenuManager.GetInstance().OpenMenu(MenuType.Pause);
```

### Log Menu Transitions
```csharp
// File: Menu.cs (base class)
public override void SetEnable()
{
    Debug.Log($"[{Type}] Menu opened");
}

public override void SetDisable()
{
    Debug.Log($"[{Type}] Menu closed");
}
```

---

## ⚠️ Important Notes

1. **Win vs Lose Priority**: 
   - `if (_isGameWon || _isGameOver) return;` prevents double-triggering
   - Win check happens BEFORE death can trigger lose

2. **Pause Time Scale**:
   - MUST reset `Time.timeScale = 1f` before loading scenes
   - Otherwise next scene will be frozen!

3. **Menu Registration**:
   - All menus MUST be in MenuManager's `_menus` list
   - Otherwise `MenuExist()` check fails

4. **Event Cleanup**:
   - Always unsubscribe events in `OnDisable()`
   - Prevents memory leaks and duplicate calls

---

## 📊 Event Flow Diagram

```
Game Start
    ↓
MenuManager.Start() → OpenMenu(MenuType.Main)
    ↓
Player starts game → MenuType.Gameplay
    ↓
    ├─→ Score >= Target? ─→ YES ─→ MenuType.Win ─→ Save progress
    │                                                Show stars/coins
    │
    └─→ Player dies? ─────→ YES ─→ MenuType.Lose ─→ No rewards
                                                     Show score only
    
[At any time during Gameplay]
    ↓
Player presses Pause → MenuType.Pause → Time.timeScale = 0
    ↓
Resume button → MenuType.Gameplay → Time.timeScale = 1
```

---

## 🎮 Button Actions Summary

### Win Menu
- **Close (X)**: → Level selection scene
- **Home**: → Level selection scene
- **Retry**: → Reload current level (keep current level number)
- **Next**: → Advance to next level (increment level number)

### Lose Menu
- **Close (X)**: → Level selection scene
- **Home**: → Level selection scene
- **Retry**: → Reload current level (same level number)

### Pause Menu
- **Resume**: → Close pause menu (Time.timeScale = 1)
- **Restart**: → Reload current level
- **Home**: → Level selection scene

All scene transitions use `LevelLoader.ReloadLevelAsync()` for smooth loading.
