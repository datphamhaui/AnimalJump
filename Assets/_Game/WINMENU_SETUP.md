# WinMenu Setup Guide

## Overview
WinMenu đã được cập nhật để phù hợp với giao diện "You win!" mới với các tính năng:

- ✨ **Celebration text** ("Yay!")
- 🏆 **Level score display** 
- 💰 **Coin rewards** với multiplier theo số sao
- ⭐ **Star display** bằng sprite system (yellow/gray)
- 🎮 **4 buttons**: Close (X), Home (Purple), Retry (Blue), Next (Green)

## Required UI Elements

### Text Components (TMP_Text)
1. **_celebrationText** - "Yay!" text 
2. **_winTitleText** - "You win!" title
3. **_levelScoreText** - "Level # score:" label
4. **_scoreValueText** - Score number (formatted with commas)
5. **_coinValueText** - Coin reward amount

### Stars Display (Image[])
6. **_starImages[3]** - Array of 3 Image components
7. **_yellowStarSprite** - Active star sprite 
8. **_grayStarSprite** - Inactive star sprite

### Buttons
9. **_closeButton** - X button (close menu)
10. **_homeButton** - Purple home button 
11. **_retryButton** - Blue retry button
12. **_nextLevelButton** - Green next level button

## Setup Steps in Unity

### 1. Create WinMenu UI Structure
```
WinMenu Canvas
├── Background Panel
├── Stars Container (3 Star Images)
├── Celebration Text ("Yay!")
├── Win Title Text ("You win!")
├── Level Score Text ("Level # score:")
├── Score Value Text (formatted number)
├── Coin Icon & Value Text
├── Close Button (X)
└── Bottom Buttons Panel
    ├── Home Button (Purple)
    ├── Retry Button (Blue) 
    └── Next Button (Green)
```

### 2. Assign Components
- Drag all Text components to corresponding fields
- Drag 3 Star Image components to _starImages array
- Import and assign yellow/gray star sprites
- Assign all 4 buttons to their fields

### 3. Star Sprites Requirements
- **Yellow Star**: Active/earned star (bright, glowing)
- **Gray Star**: Inactive/unearned star (dimmed, outline)

### 4. Button Icons & Colors
- **Close (X)**: Simple X icon, neutral color
- **Home**: House icon, purple background
- **Retry**: Refresh/replay icon, blue background  
- **Next**: Arrow/play icon, green background

## Features

### 🪙 Dynamic Coin Rewards
- **Base reward**: From LevelDataSO.coinReward (config-based)
- **Star multiplier**: 1★=1x, 2★=1.5x, 3★=2x
- **Calculation**: `baseCoinReward * starMultiplier`
- **Fallback**: 100 coins per star if no level data found

### Coin Calculation Example:
```csharp
// Level data: coinReward = 500
// Player gets 3 stars
// Final coins = 500 * 2.0 = 1000 coins

// Player gets 2 stars  
// Final coins = 500 * 1.5 = 750 coins
```

### ⭐ Smart Star Display
- Uses sprite swapping instead of show/hide
- Always shows all 3 stars (earned = yellow, unearned = gray)
- OnValidate() warnings for missing sprites

### 🎮 Button Behavior
- **Close/Home**: Return to Level selection scene
- **Retry**: Reload current level
- **Next**: Advance to next level (hidden if last level)
- Safe button disable during transitions

## Code Integration

### Event System
```csharp
// WinMenu subscribes to GameManager win event
GameManager.OnGameWin += OnGameWin;

// Receives star count and calculates coin reward
private void OnGameWin(int stars)
{
    _earnedStars = stars;
    CalculateCoinReward(); // Based on level data + star multiplier
}
```

### Level Data Integration
```csharp
// Gets current level and its data
int currentLevel = _levelProgressManager.GetCurrentLevel();
LevelDataSO currentLevelData = _levelProgressManager.GetLevelData(currentLevel);

// Gets coin reward from level config
int baseCoinReward = currentLevelData.coinReward;

// Applies star multiplier
_earnedCoins = Mathf.RoundToInt(baseCoinReward * starMultiplier);
```

## Testing Checklist

- [ ] All text fields display correctly
- [ ] Stars show proper sprites (yellow for earned, gray for unearned)
- [ ] Coin calculation works with star multipliers
- [ ] All 4 buttons function properly
- [ ] Next button hides on final level
- [ ] Scene transitions work smoothly
- [ ] Score formatting shows comma separators
- [ ] OnValidate warnings in Inspector work

## Notes

- Menu automatically calculates and displays rewards when game is won
- Coin rewards are stored for future currency system integration
- Star sprites should be imported and assigned in Inspector
- Button colors and icons should match the reference design
- All text should use TextMeshPro for better quality