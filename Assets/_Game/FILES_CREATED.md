# 📦 CÁC FILE ĐÃ TẠO/SỬA ĐỔI

## ✅ FILES MỚI TẠO

### 1. Data/ScriptableObjects
- **LevelDataSO.cs** - ScriptableObject config cho mỗi level
  - Target score, tốc độ, độ khó, gap, safe zone...

### 2. Managers
- **LevelProgressManager.cs** - Quản lý tiến độ người chơi
  - Save/Load progress (PlayerPrefs)
  - Unlock levels, lưu stars
  - Singleton pattern

### 3. UI/Menus
- **WinMenu.cs** - Menu khi hoàn thành level
  - Hiển thị số sao, level, score
  - Next level, Retry, Home buttons
  
- **PauseMenu.cs** - Menu pause game
  - Time.timeScale = 0/1
  - Resume, Restart, Home buttons

### 4. UI Components
- **LevelButton.cs** - Component cho mỗi level button
  - Lock/Unlock display
  - Stars display
  - Current level indicator
  
- **LevelSelectionManager.cs** - Manager cho màn chọn level
  - Quản lý tất cả level buttons
  - Refresh display

### 5. Editor Tools
- **LevelProgressManagerEditor.cs** - Custom Inspector
  - Debug tools, unlock levels, test stars
  
- **LevelDataCreator.cs** - Tạo level data assets
  - Menu: Tools > Game > Create Level Data Assets

### 6. Documentation
- **README.md** - Tổng quan và quick start
- **SETUP_GUIDE.md** - Hướng dẫn setup chi tiết đầy đủ (tất cả trong 1 file)
- **FILES_CREATED.md** - File này (danh sách files và cơ chế)

---

## 🔧 FILES ĐÃ SỬA ĐỔI

### 1. GameManager.cs
**Thay đổi:**
- ✅ Thêm tracking số lần miss (đáp lệch mép)
- ✅ Thêm `GameWin()` method - xử lý khi thắng level
- ✅ Thêm `CalculateStars()` - tính số sao (0 miss = 3⭐, 1 miss = 2⭐, 2+ miss = 1⭐)
- ✅ Thêm `CheckWinCondition()` - kiểm tra đạt target score
- ✅ Thêm `ResetGameState()` - reset trạng thái khi bắt đầu level mới
- ✅ Thêm reference `LevelProgressManager`
- ✅ Thay đổi `Piece.OnGameOver` → `HandleMiss()` thay vì game end ngay
- ✅ Thêm event `OnGameWin`

### 2. LevelManager.cs
**Thay đổi:**
- ✅ Refactor hoàn toàn: từ "high score based" → "level data based"
- ✅ Load config từ `LevelDataSO` thay vì hard-code
- ✅ Thêm `GetCurrentLevelData()` - lấy data level hiện tại
- ✅ Thêm `GetPlatformGapRange()` - lấy gap từ level config
- ✅ Thêm `GetSafeLandingZoneRatio()` - lấy safe zone từ config
- ✅ Remove logic "level up trong game" (không còn dùng)

### 3. MenuType.cs
**Thay đổi:**
- ✅ Thêm `Win` enum - menu type mới

### 4. GameplayMenu.cs
**Thay đổi:**
- ✅ Thêm hiển thị level number (`_levelText`)
- ✅ Thêm hiển thị target score (`_targetScoreText`)
- ✅ Thêm pause button (`_pauseButton`)
- ✅ Thêm `UpdateLevelInfo()` - cập nhật UI level info

### 5. Piece.cs
**Thay đổi:**
- ✅ Load `_safeLandingZoneRatio` từ `LevelManager` trong `Start()`
- ✅ Safe zone giờ dynamic theo từng level

### 6. Platform.cs
**Thay đổi:**
- ✅ Load `_gap` range từ `LevelManager` trong `Start()`
- ✅ Gap giờ dynamic theo từng level
- ✅ Thêm reference `LevelManager`

---

## 🎯 CÁCH HOẠT ĐỘNG

### Flow chơi game:

```
1. Màn Level Selection
   ↓
2. Click level button → LevelButton.OnLevelButtonClicked()
   ↓
3. LevelProgressManager.SetCurrentLevel(X)
   ↓
4. SceneManager.LoadScene("GameScene")
   ↓
5. LevelManager.LoadCurrentLevel() → Load LevelDataSO
   ↓
6. GameManager.Start() → Show GameplayMenu
   ↓
7. Player chơi → Đạt điểm → CheckWinCondition()
   ↓
8. Nếu Score >= TargetScore → GameWin()
   ↓
9. CalculateStars() → CompleteLevel() → Show WinMenu
   ↓
10. Next Level / Retry / Home
```

### Flow tính sao:

```
GameManager tracking:
- Mỗi lần đáp lệch mép → _missCount++
- Khi win → CalculateStars():
  - 0 miss = 3 sao ⭐⭐⭐
  - 1 miss = 2 sao ⭐⭐
  - 2+ miss = 1 sao ⭐
```

### Flow pause:

```
Pause Button Click
   ↓
MenuManager.OpenMenu(MenuType.Pause)
   ↓
PauseMenu.SetEnable() → Time.timeScale = 0
   ↓
Resume Button
   ↓
MenuManager.CloseMenu()
   ↓
PauseMenu.SetDisable() → Time.timeScale = 1
```

---

## 📊 DATA STRUCTURE

### PlayerPrefs Keys:
```
"LevelUnlock_1" = 1/0 (unlocked/locked)
"LevelUnlock_2" = 1/0
...
"LevelStars_1" = 0-3 (số sao)
"LevelStars_2" = 0-3
...
"CurrentLevel" = số level đang chọn
"BestScore" = điểm cao nhất (giữ nguyên từ cũ)
```

### LevelDataSO Fields:
```
- levelNumber: int
- levelName: string
- targetScore: int ⭐ (điểm cần đạt)
- platformSpeed: float (tốc độ)
- platformGapRange: Vector2 (gap min-max)
- safeLandingZoneRatio: float (0-1)
- hasObstacles: bool
- speedIncreaseRate: float
```

---

## 🎨 UI COMPONENTS CẦN SETUP

### GameplayMenu:
```
- TextMeshPro: Level X
- TextMeshPro: Target: XX
- TextMeshPro: Score
- Button: Pause
```

### WinMenu:
```
- TextMeshPro: Level X
- TextMeshPro: Score: XX
- 3x GameObject: Star icons
- Button: Next Level
- Button: Retry
- Button: Home
```

### PauseMenu:
```
- Button: Resume
- Button: Restart
- Button: Home
```

### LevelButton (cho Level Selection):
```
- TextMeshPro: Level number
- GameObject: Lock icon
- GameObject: Unlocked content
- 3x GameObject: Star icons
- GameObject: Current level indicator
- Button: Self
```

---

## 🧪 TESTING CHECKLIST

- [ ] Tạo 10 Level Data (Tools > Game > Create Level Data Assets)
- [ ] Setup LevelProgressManager trong scene
- [ ] Assign Level Data vào mảng All Levels
- [ ] Tạo UI cho WinMenu
- [ ] Tạo UI cho PauseMenu
- [ ] Đăng ký menus trong MenuManager
- [ ] Test chơi Level 1, đạt target score → Win
- [ ] Test miss 0, 1, 2+ lần → Check stars
- [ ] Test pause/resume
- [ ] Test next level unlock
- [ ] Test level selection screen

---

## 🔧 TROUBLESHOOTING

### "LevelManager not found"
→ Có LevelManager component trong scene chưa?

### "LevelDataSO is null"
→ Check LevelProgressManager có assign Level Data chưa?

### Stars không đúng
→ Check logic CalculateStars() trong GameManager

### Pause không hoạt động
→ Check Time.timeScale và PauseMenu có trong MenuManager chưa?

### Level không unlock
→ Level 1 auto unlock, các level khác unlock sau khi win level trước

---

## 📞 SUPPORT

- Check Console logs (có nhiều debug log chi tiết)
- Dùng Debug Tools trong LevelProgressManager Inspector
- Right-click LevelButton → Debug: Unlock This Level
- Menu: Tools > Game > Open Level Data Folder

---

Hoàn thành! 🎉
