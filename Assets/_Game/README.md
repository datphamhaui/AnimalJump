# 🎮 CuteZooJump - Level System

## 📋 Tổng Quan

Hệ thống level goal-based đã được implement đầy đủ cho game CuteZooJump với các tính năng:

- ✅ **Level Management**: Config từng level qua ScriptableObject
- ✅ **Star System**: 3 mức sao dựa trên performance (0 miss = 3⭐, 1 miss = 2⭐, 2+ miss = 1⭐)
- ✅ **Win/Lose System**: Win khi đạt target score, lose khi rơi xuống
- ✅ **Pause System**: Pause/Resume game với Time.timeScale
- ✅ **Progress Saving**: Lưu unlock status và stars vào PlayerPrefs
- ✅ **Level Progression**: Auto unlock level tiếp theo khi hoàn thành
- ✅ **Editor Tools**: Debug tools và auto-create level data

---

## 🚀 Quick Start (TL;DR)

### 1. Tạo Level Data
```
Unity Menu: Tools > Game > Create Level Data Assets
```

### 2. Setup Level Scene (Scene chọn level)
- Tạo GameObject "LevelProgressManager"
- Add component: LevelProgressManager
- Assign 10 Level Data files

### 3. Setup Game Scene (Scene chơi game)
- Tạo WinMenu (stars, buttons)
- Tạo PauseMenu (3 buttons)
- Update GameplayMenu (pause button, level info)
- Đăng ký 2 menus mới vào MenuManager

### 4. Test
- Level scene: Click level 1 → Game scene
- Game: Đạt target → Win với stars
- Pause/Resume test

**→ Xem chi tiết trong [SETUP_GUIDE.md](SETUP_GUIDE.md)**

---

## 📚 Documentation

- **[SETUP_GUIDE.md](SETUP_GUIDE.md)** ⭐ - Hướng dẫn setup chi tiết đầy đủ (BẮT ĐẦU TỪ ĐÂY)
- **[FILES_CREATED.md](FILES_CREATED.md)** - Danh sách file đã tạo/sửa và cơ chế hoạt động

---

## 🏗️ Architecture

### Core Components

```
LevelProgressManager (Singleton)
├─ Manages player progress
├─ Save/Load unlock & stars
└─ Current level tracking

LevelManager
├─ Load LevelDataSO config
├─ Apply settings to game
└─ Provide level info

GameManager
├─ Win/Lose logic
├─ Stars calculation
├─ Miss tracking
└─ Game flow control
```

### Data Flow

```
Level Selection → Set Current Level
    ↓
Load Game Scene → Load Level Config
    ↓
Play Game → Track Score & Misses
    ↓
Win → Calculate Stars → Save Progress
    ↓
Unlock Next Level → Return to Selection
```

---

## 🎯 Features

### Level Configuration (LevelDataSO)
- Target score
- Platform speed
- Gap range
- Safe landing zone ratio
- Obstacles toggle
- Speed increase rate

### Star System
- **3 Stars** ⭐⭐⭐ - Perfect! (0 misses)
- **2 Stars** ⭐⭐ - Good! (1 miss)
- **1 Star** ⭐ - Completed! (2+ misses)

**Miss** = Đáp lệch mép platform (ngoài vùng safe zone)

### Pause System
- Time.timeScale = 0 khi pause
- Time.timeScale = 1 khi resume
- UI với Resume, Restart, Home buttons

---

## 🛠️ Editor Tools

### Level Data Creator
```
Menu: Tools > Game > Create Level Data Assets
```
Auto tạo 10 level với config tăng dần độ khó

### Level Progress Manager Inspector
- 📊 Log Progress - Xem tiến độ hiện tại
- 🔄 Reset All Progress - Reset về đầu
- Unlock Level 1-5, 6-10 - Quick unlock
- ⭐ Test Stars - Test lưu sao

### Level Button Context Menu
- Right-click component → "Debug: Unlock This Level"
- Right-click component → "Debug: Set 3 Stars"

---

## 🧪 Testing

### Debug Commands
```csharp
// Check unlock
bool unlocked = LevelProgressManager.GetInstance().IsLevelUnlocked(2);

// Get stars
int stars = LevelProgressManager.GetInstance().GetLevelStars(1);

// Unlock level
LevelProgressManager.GetInstance().UnlockLevel(5);

// Set stars
LevelProgressManager.GetInstance().SaveLevelStars(1, 3);

// Reset all
LevelProgressManager.GetInstance().ResetAllProgress();

// Log progress
LevelProgressManager.GetInstance().LogProgress();
```

---

## 🎨 UI Components

### GameplayMenu
- Level text: "Level X"
- Target text: "Target: XX"
- Score text: "XX"
- Pause button

### WinMenu
- Level text
- Score text
- 3 Star icons (active/inactive)
- Next Level button (hidden nếu hết level)
- Retry button
- Home button

### PauseMenu
- Resume button
- Restart button
- Home button

### LevelButton (Level Selection)
- Level number text
- Lock icon (shown when locked)
- Unlocked content (stars, etc.)
- 3 Star icons
- Current level indicator (avatar/highlight)

---

## 📊 Data Storage (PlayerPrefs)

```
"LevelUnlock_1" → 1/0
"LevelUnlock_2" → 1/0
...
"LevelStars_1" → 0-3
"LevelStars_2" → 0-3
...
"CurrentLevel" → 1-10
"BestScore" → highest score (legacy)
```

---

## 🔧 Troubleshooting

### "LevelManager not found"
→ Add LevelManager component vào scene

### "LevelDataSO is null"
→ Assign Level Data vào LevelProgressManager

### Stars không đúng
→ Check miss tracking trong GameManager

### Pause không work
→ Check PauseMenu đã đăng ký trong MenuManager chưa

### Level không unlock
→ Level 1 auto unlock, các level khác cần hoàn thành level trước

---

## 📞 Support

- Check Console logs (nhiều debug info)
- Dùng Debug Tools trong Inspector
- Đọc SETUP_GUIDE.md
- Xem FILES_CREATED.md để biết file nào đã thay đổi

---

## 🎉 Status

**✅ READY TO USE**

Chỉ cần setup UI trong Unity Editor là có thể chơi được!

---

Made with ❤️ for CuteZooJump
