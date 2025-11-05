# LoseMenu Setup Guide

## Overview
LoseMenu hiển thị khi người chơi game over (thua level) với giao diện tương tự WinMenu nhưng có theme buồn và không có reward.

## Required UI Elements

### Text Components (TMP_Text)
1. **_sadText** - "Oh.." speech bubble text
2. **_loseTitleText** - "You lose.." title (pink banner)
3. **_levelScoreText** - "Level # score:" label
4. **_scoreValueText** - Score number (formatted with commas)
5. **_coinValueText** - Coin amount (always shows "0")

### Buttons
6. **_closeButton** - X button (top right)
7. **_homeButton** - Purple home button
8. **_retryButton** - Blue retry button

## UI Structure Reference

Based on the "You lose.." screen:
```
LoseMenu Canvas
├── Background Panel (rounded, cyan border)
├── Skull Decorations (3 skulls on top)
├── Sad Emoji + "Oh.." Speech Bubble
├── Pink Banner → "You lose.." Text
├── Level Score Section
│   ├── "Level # score:" label
│   ├── X icon + Score value (pink, large)
│   └── Skull decoration
├── Coin Section
│   ├── Coin icon (dimmed/gray)
│   └── "0" text
├── Close Button (X, top right)
└── Bottom Buttons
    ├── Home Button (Purple, house icon)
    └── Retry Button (Blue, refresh icon)
```

## Key Differences from WinMenu

| Feature | WinMenu | LoseMenu |
|---------|---------|----------|
| **Theme** | 🎉 Happy, colorful | 😢 Sad, skulls |
| **Title** | "You win!" | "You lose.." |
| **Stars** | ⭐ Shows 1-3 stars | ❌ No stars |
| **Coins** | 💰 Reward amount | 💰 Always 0 |
| **Next Button** | ✅ Yes (if available) | ❌ No |
| **Buttons** | 4 (Close/Home/Retry/Next) | 3 (Close/Home/Retry) |

## Setup Steps

### 1. Create LoseMenu in Unity
- Create new Canvas GameObject
- Add `LoseMenu.cs` component
- Set `MenuType` to `Lose` in Inspector

### 2. Build UI Hierarchy
Follow the structure above with:
- Skull decorations on top
- Sad emoji with speech bubble
- Pink banner for title
- Score display with X icon
- Dimmed coin icon showing 0
- Close button (X)
- Bottom row: Home + Retry buttons

### 3. Assign Components
Drag all UI elements to script fields:
- 5 Text components
- 3 Buttons

### 4. Register in MenuManager
- Open MenuManager GameObject
- Add LoseMenu to `_menus` list
- Ensure it's registered at runtime

## Features

### 💔 No Rewards
- No stars displayed
- Coins always show "0"
- No progress saved (level not completed)
- Next level button not shown

### 🎮 Button Behavior
- **Close (X)**: Return to level selection
- **Home**: Return to level selection  
- **Retry**: Reload current level to try again

### 📊 Display Info
- Shows current level number
- Shows score achieved (even if failed)
- Formatted score with comma separators

## Trigger Logic

LoseMenu is automatically shown when:

```csharp
// 1. Player dies
PlayerBehaviour.OnPlayerDeath event
    ↓
GameManager.GameEnd()
    ↓
MenuManager.SwitchMenu(MenuType.Lose)

// 2. Player falls off
PlayerRenderer.OnInvisible event
    ↓
GameManager.GameEnd()
    ↓
MenuManager.SwitchMenu(MenuType.Lose)

// 3. Player misses landing
Piece.OnGameOver event
    ↓
GameManager.HandleMiss() → GameEnd()
    ↓
MenuManager.SwitchMenu(MenuType.Lose)
```

## Code Integration

### Event Subscription (Already Done)
```csharp
// File: GameManager.cs - OnEnable()
Piece.OnGameOver      += HandleMiss;
PlayerBehaviour.OnPlayerDeath += GameEnd;
_playerRenderer.OnInvisible   += GameEnd;
```

### Menu Switch (Already Done)
```csharp
// File: GameManager.cs - GameEnd()
public void GameEnd()
{
    if (_isGameOver || _isGameWon) return;
    _isGameOver = true;
    
    _menuController.SwitchMenu(MenuType.Lose);
}
```

## Testing Checklist

- [ ] LoseMenu appears when player dies
- [ ] All text displays correctly
- [ ] Coin always shows "0"
- [ ] Score formatted with commas
- [ ] Close button returns to level selection
- [ ] Home button returns to level selection
- [ ] Retry button reloads current level
- [ ] No stars or next button shown
- [ ] UI matches reference design (skulls, sad theme)

## Design Notes

### Visual Theme
- 💀 **Skulls**: 3 skulls on top (spooky decoration)
- 😢 **Sad emoji**: Shows disappointment
- 🗨️ **"Oh.." bubble**: Sad exclamation
- 🎀 **Pink banner**: Same style as WinMenu but "You lose.."
- ❌ **X icon**: Next to score (indicates failure)
- 🪙 **Dimmed coin**: Gray/inactive appearance with "0"

### Color Scheme
- Background: Same cyan rounded panel
- Banner: Same pink as WinMenu
- Buttons: Purple (Home), Blue (Retry)
- Overall darker/dimmer tone compared to WinMenu

## Notes

- LoseMenu does NOT save any progress
- Does NOT unlock next level
- Does NOT give coin rewards
- Player must retry to complete the level
- Can return to level selection to choose different level
