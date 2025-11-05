# 🎮 HƯỚNG DẪN SETUP HỆ THỐNG LEVEL - HOÀN CHỈNH

## 📋 TỔNG QUAN

### Tính năng đã implement:
- ✅ Level goal-based với target score
- ✅ Hệ thống tính sao (3 sao, 2 sao, 1 sao)
- ✅ Win/Lose system
- ✅ Pause game
- ✅ Level progression (unlock level tiếp theo)
- ✅ Save/Load progress
- ✅ Coin reward (dự bị cho tương lai)

### Scenes trong game:
1. **LoadingScene** - Màn loading ban đầu
2. **SelectAnimalScene** - Chọn động vật
3. **Level** - Chọn level để chơi ⚠️ (CẦN SETUP)
4. **Game** - Chơi game chính ⚠️ (CẦN SETUP)

### Flow game:
```
LoadingScene → SelectAnimalScene → Level → Game
                                      ↑        ↓
                                      └────────┘
                                   (Home/Next Level)
```

---

## 🚀 BƯỚC 1: TẠO LEVEL DATA (Unity Editor)

### Tự động tạo 10 level mẫu (KHUYẾN NGHỊ):
```
1. Unity Menu: Tools > Game > Create Level Data Assets
2. Sẽ tạo 10 files trong: Assets/_Game/Data/Levels/
3. Config mẫu: Level 1-10 với độ khó tăng dần
```

### Hoặc tạo thủ công:
```
1. Right click trong Project
2. Create > Game > Level Data
3. Config các thông số:
   - Level Number: 1, 2, 3...
   - Target Score: Điểm cần đạt
   - Coin Reward: Coin thưởng (TODO: implement sau)
   - Platform Speed: Tốc độ
   - Platform Gap Range: Khoảng cách pieces
   - Safe Landing Zone Ratio: 0.7 = 70% vùng an toàn
```

### ✅ Verify:
- [ ] Check folder `Assets/_Game/Data/Levels/` có 10 files
- [ ] Mở 1 file xem config có đúng không

---

## 🛠️ BƯỚC 2: SETUP CÁC SCENE

### 📦 Tổng quan các scene:
1. **LoadingScene** - Màn loading ban đầu
2. **SelectAnimalScene** - Màn chọn động vật
3. **Level** - Màn chọn level (Level Selection)
4. **Game** - Màn chơi game chính

---

### 🎬 Scene 1: LoadingScene
**Mục đích**: Màn loading ban đầu khi mở game

**Setup cần làm**: ✅ Không cần setup gì (giữ nguyên)

**Flow**: LoadingScene → SelectAnimalScene

---

### 🐾 Scene 2: SelectAnimalScene  
**Mục đích**: Chọn động vật trước khi vào chơi

**Setup cần làm**: ✅ Không cần setup gì (giữ nguyên)

**Flow**: SelectAnimalScene → Level (scene chọn level)

---

### 🎯 Scene 3: Level (Level Selection Scene)
**Mục đích**: Màn chọn level để chơi

**Setup cần làm**:

#### A. Tạo LevelProgressManager (QUAN TRỌNG!)
```
1. Tạo Empty GameObject: "LevelProgressManager"
2. Add component: LevelProgressManager
3. Kéo tất cả 10 Level Data vào mảng "All Levels" (theo thứ tự)
```

#### B. Setup Level Selection UI
```
1. Tạo LevelSelectionManager GameObject (nếu chưa có)
   - Add component: LevelSelectionManager

2. Tạo 10 Level Buttons (theo design hình bạn gửi)
   Mỗi button cần:
   - Add component: LevelButton
   - Set Level Number: 1, 2, 3, ... 10
   - Assign các field:
     * _button: Button component
     * _levelText: TextMeshPro hiển thị số level
     * _lockIcon: GameObject icon ổ khóa (active khi locked)
     * _unlockedContent: GameObject content khi unlock (stars, etc.)
     * _starIcons[0,1,2]: 3 GameObject star icons
     * _currentLevelIndicator: GameObject hiển thị level đang chơi (avatar)
     * _gameSceneName: "Game" (tên scene game)

3. Layout level buttons theo grid (như hình)
```

#### C. Thêm nút Back về SelectAnimalScene (optional)

**Flow**: Level → Click level button → Game scene

---

### 🎮 Scene 4: Game (Gameplay Scene)
**Mục đích**: Màn chơi game chính

**Setup cần làm**:

#### A. Setup GameManager (đã có sẵn)
- GameManager đã tự động tích hợp level system

#### B. Setup UI

#### 1. GameplayMenu (In-game UI)
**Tìm Canvas/GameObject đã có**: GameplayMenu

**Thêm/Cập nhật**:
- TextMeshPro: `_levelText` - Hiển thị "Level X"
- TextMeshPro: `_targetScoreText` - Hiển thị "Target: XX"
- TextMeshPro: `_scoreText` - Hiển thị điểm (đã có)
- Button: `_pauseButton` - Nút pause game

**Assign vào component GameplayMenu** trong Inspector

---

#### 2. WinMenu (Win Screen) - MỚI TẠO
**Tạo Canvas/Panel mới**: "WinMenu"

**Cấu trúc UI**:
```
WinMenu (Canvas/Panel)
├─ Background (Image)
├─ LevelText (TextMeshPro) - "Level X"
├─ ScoreText (TextMeshPro) - "Score: XX"
├─ Stars Container
│  ├─ Star1 (Image) - _starIcons[0]
│  ├─ Star2 (Image) - _starIcons[1]
│  └─ Star3 (Image) - _starIcons[2]
└─ Buttons
   ├─ NextLevelButton - _nextLevelButton
   ├─ RetryButton - _retryButton
   └─ HomeButton - _homeButton
```

**Steps**:
1. Tạo GameObject "WinMenu" với Canvas hoặc Panel
2. Add component: **WinMenu** script
3. Add component: **Menu** base class (set Type = **Win**)
4. Tạo các UI elements như trên
5. Assign vào các field trong WinMenu component

---

#### 3. PauseMenu (Pause Screen) - MỚI TẠO
**Tạo Canvas/Panel mới**: "PauseMenu"

**Cấu trúc UI**:
```
PauseMenu (Panel)
├─ Background Overlay (Image - dark transparent)
├─ Pause Popup Panel
│  ├─ Title: "PAUSED"
│  └─ Buttons
│     ├─ ResumeButton - _resumeButton
│     ├─ RestartButton - _restartButton
│     └─ HomeButton - _homeButton
```

**Steps**:
1. Tạo GameObject "PauseMenu"
2. Add component: **PauseMenu** script
3. Add component: **Menu** base class (set Type = **Pause**)
4. Tạo các UI elements như trên
5. Assign vào các field trong PauseMenu component

---

#### 4. Đăng ký menus trong MenuManager
**Tìm GameObject**: MenuManager (đã có trong scene)

**Steps**:
```
1. Mở GameObject "MenuManager" trong Inspector
2. Trong danh sách "_menus":
   - Kéo WinMenu GameObject vào
   - Kéo PauseMenu GameObject vào
3. Check Type của mỗi menu:
   - WinMenu.Type = Win
   - PauseMenu.Type = Pause
   - GameplayMenu.Type = Gameplay (đã có)
```

---

### 📝 Summary Setup cho Scene Game:
- ✅ GameManager: Đã có sẵn, không cần sửa
- ✅ LevelManager: Đã có sẵn, không cần sửa  
- ✅ ScoreManager: Đã có sẵn, không cần sửa
- 🔧 GameplayMenu: Thêm level text, target text, pause button
- ➕ WinMenu: Tạo mới hoàn toàn
- ➕ PauseMenu: Tạo mới hoàn toàn
- 🔧 MenuManager: Đăng ký 2 menu mới (Win, Pause)

**Flow trong Game scene**: 
- Start → GameplayMenu
- Đạt target score → WinMenu
- Rơi xuống → ReviveMenu (đã có) → GameOverMenu (đã có)
- Click pause → PauseMenu

---

---

## 🎯 BƯỚC 3: FLOW GIỮA CÁC SCENE

### Luồng chơi game chuẩn:

```
🎬 LoadingScene
    ↓ (Auto load sau vài giây)
🐾 SelectAnimalScene (Chọn động vật)
    ↓ (Click "Play" hoặc "Next")
🎯 Level (Chọn level)
    ↓ (Click level button - LevelButton component xử lý)
🎮 Game (Chơi game)
    ├─ Win → WinMenu
    │   ├─ Next Level → Game (level tiếp)
    │   ├─ Retry → Game (cùng level)
    │   └─ Home → Level (về chọn level)
    │
    └─ Lose → ReviveMenu
        ├─ Revive → Continue
        └─ GameOver → GameOverMenu
            ├─ Restart → Game
            └─ Home → Level
```

### Scene transitions quan trọng:

#### Từ SelectAnimalScene → Level:
```csharp
// Trong SelectAnimalScene, button "Next/Play"
SceneManager.LoadScene("Level");
```

#### Từ Level → Game (tự động xử lý bởi LevelButton):
```csharp
// LevelButton.OnLevelButtonClicked() tự động:
// 1. Check level unlock
// 2. Set current level
// 3. Load scene "Game"
LevelProgressManager.GetInstance().SetCurrentLevel(levelNumber);
SceneManager.LoadScene("Game");
```

#### Từ Game → Level (về chọn level):
```csharp
// Trong WinMenu hoặc GameOverMenu button "Home"
SceneManager.LoadScene("Level");
```

---

## ⚙️ CƠ CHẾ HOẠT ĐỘNG

### 1. Tính sao (Stars Calculation)
```
- Không trượt lần nào (miss = 0): ⭐⭐⭐ 3 sao
- Trượt 1 lần (miss = 1): ⭐⭐ 2 sao  
- Trượt 2+ lần (miss >= 2): ⭐ 1 sao
```

**Trượt = Đáp lệch mép platform (ngoài vùng safe zone)**

### 2. Win Condition
- Đạt đủ điểm >= Target Score của level
- Hiển thị WinMenu với số sao
- Auto unlock level tiếp theo
- Lưu tiến độ vào PlayerPrefs

### 3. Lose Condition
- Đáp trượt quá xa mép → Rơi xuống
- Player rơi xuống ngoài màn hình
- Hiển thị ReviveMenu (1 lần) hoặc GameOverMenu

### 4. Pause System
- Nhấn nút Pause → `Time.timeScale = 0`
- Game dừng hoàn toàn
- Resume → `Time.timeScale = 1`

---

## 🧪 DEBUG & TESTING

### 1. Debug Tools trong Inspector
Chọn GameObject "LevelProgressManager", sẽ thấy buttons:
- **📊 Log Progress**: Xem tiến độ hiện tại
- **🔄 Reset All Progress**: Reset toàn bộ
- **Unlock Level 1-5**: Unlock nhanh
- **⭐ Test Stars**: Test lưu sao

### 2. Test trong game
```csharp
// Check level unlock
bool unlocked = LevelProgressManager.GetInstance().IsLevelUnlocked(2);

// Get stars
int stars = LevelProgressManager.GetInstance().GetLevelStars(1);

// Unlock level
LevelProgressManager.GetInstance().UnlockLevel(5);

// Reset progress
LevelProgressManager.GetInstance().ResetAllProgress();
```

---

## 📝 NOTES

### Về ScoreManager
- Hiện tại ScoreManager lưu "BestScore" toàn game
- Có thể giữ nguyên để làm "Total Score" mode
- Level system hoạt động độc lập

### Về LevelManager
- LevelManager cũ đã được refactor để load từ LevelDataSO
- Không còn dùng high score để tính level
- Tốc độ, độ khó được lấy từ config

### Audio
- Win sound: Hiện tại dùng GAMEOVER sound
- Cần add sound mới: AudioType.WIN
- Update trong GameManager.GameWin()

---

## 🎨 UI DESIGN TIPS

### Level Selection Screen (như hình)
```
Mỗi level button cần:
- Level number (1, 2, 3...)
- Lock/Unlock state (icon ổ khóa)
- Stars display (0-3 stars)
- Current player indicator (avatar)

Code mẫu:
foreach (var levelButton in levelButtons)
{
    int levelNum = levelButton.levelNumber;
    bool unlocked = LevelProgressManager.GetInstance().IsLevelUnlocked(levelNum);
    int stars = LevelProgressManager.GetInstance().GetLevelStars(levelNum);
    
    levelButton.SetUnlocked(unlocked);
    levelButton.SetStars(stars);
}
```

---

## 🐛 TROUBLESHOOTING

### Lỗi "LevelManager not found"
→ Đảm bảo có LevelManager component trong scene

### Stars không lưu
→ Check LevelProgressManager có reference đúng Level Data không

### Pause không hoạt động
→ Check PauseMenu đã được add vào MenuManager chưa

### Level không unlock
→ Level 1 tự động unlock lúc start
→ Level khác unlock sau khi hoàn thành level trước

---

## ✅ CHECKLIST SETUP THEO SCENE

### 📦 Chuẩn bị (Unity Editor)
- [ ] Tạo Level Data (10 files): `Tools > Game > Create Level Data Assets`
- [ ] Verify: Check folder `Assets/_Game/Data/Levels/` có 10 files

---

### 🎬 Scene: LoadingScene
- [ ] ✅ Giữ nguyên, không cần setup gì

---

### 🐾 Scene: SelectAnimalScene  
- [ ] ✅ Giữ nguyên, không cần setup gì
- [ ] (Optional) Kiểm tra button "Next/Play" có load scene "Level"

---

### 🎯 Scene: Level (Level Selection)
- [ ] Tạo GameObject "LevelProgressManager"
- [ ] Add component: LevelProgressManager
- [ ] Kéo 10 Level Data vào mảng "All Levels" (theo thứ tự)
- [ ] Tạo GameObject "LevelSelectionManager" (optional)
- [ ] Tạo 10 Level Buttons với component LevelButton
- [ ] Mỗi LevelButton assign:
  - [ ] Level Number (1-10)
  - [ ] Button reference
  - [ ] Level text
  - [ ] Lock icon
  - [ ] Unlocked content
  - [ ] 3 Star icons
  - [ ] Current level indicator
  - [ ] Game Scene Name = "Game"
- [ ] Test: Click level 1 → Load scene Game

---

### 🎮 Scene: Game (Gameplay)
#### Setup UI:
- [ ] **GameplayMenu**: Thêm và assign
  - [ ] _levelText (TextMeshPro)
  - [ ] _targetScoreText (TextMeshPro)
  - [ ] _pauseButton (Button)
  
- [ ] **WinMenu**: Tạo mới
  - [ ] Tạo Canvas/Panel "WinMenu"
  - [ ] Add component: WinMenu
  - [ ] Add component: Menu (Type = Win)
  - [ ] Tạo UI: level text, score text, 3 stars, 3 buttons
  - [ ] Assign tất cả vào WinMenu component
  
- [ ] **PauseMenu**: Tạo mới
  - [ ] Tạo Panel "PauseMenu"
  - [ ] Add component: PauseMenu
  - [ ] Add component: Menu (Type = Pause)
  - [ ] Tạo UI: 3 buttons (Resume, Restart, Home)
  - [ ] Assign vào PauseMenu component
  
- [ ] **MenuManager**: Đăng ký menus
  - [ ] Mở GameObject "MenuManager"
  - [ ] Add WinMenu vào danh sách "_menus"
  - [ ] Add PauseMenu vào danh sách "_menus"
  - [ ] Verify Types: Win, Pause

#### Test Game Scene:
- [ ] Play scene → Level info hiển thị đúng
- [ ] Chơi đạt target score → WinMenu hiện
- [ ] Check stars: 0 miss = 3⭐, 1 miss = 2⭐, 2+ miss = 1⭐
- [ ] Click Next Level → Level 2 unlock
- [ ] Click Pause → Game dừng (Time.timeScale = 0)
- [ ] Click Resume → Game chạy tiếp
- [ ] Rơi xuống → ReviveMenu → GameOverMenu

---

## 🔄 BƯỚC 4: TEST HOÀN CHỈNH

### Test Flow Scenes:
- [ ] LoadingScene → SelectAnimalScene (auto)
- [ ] SelectAnimalScene → Level (click Next/Play)
- [ ] Level: Click level 1 → Game scene load
- [ ] Game: Chơi đạt target score → WinMenu
- [ ] Win: Click Next Level → Level 2 unlock
- [ ] Win: Click Home → Level scene
- [ ] Level: Level 2 đã unlock, có stars

### Test Gameplay:
- [ ] Level info hiển thị (Level X, Target: XX)
- [ ] Score tăng khi đáp đúng
- [ ] Đạt target → WinMenu với stars
- [ ] Stars: 0 miss = 3⭐, 1 miss = 2⭐, 2+ miss = 1⭐
- [ ] Pause button → Game dừng
- [ ] Resume → Game chạy tiếp
- [ ] Rơi xuống → ReviveMenu → GameOverMenu

---

## 🎬 SCENE NAMES REFERENCE

### Official Scene Names (Case-sensitive!):
```
✅ Correct:
1. "LoadingScene"
2. "SelectAnimalScene"
3. "Level"
4. "Game"

❌ Wrong:
- "level", "game" (lowercase)
- "SelectMapScene" (old name)
- "GameScene" (wrong name)
```

### Scene Transitions:
| From | To | Trigger | Auto/Manual |
|------|-----|---------|-------------|
| LoadingScene | SelectAnimalScene | Auto | Auto |
| SelectAnimalScene | Level | Button | Manual |
| Level | Game | Level Button | LevelButton.cs |
| Game | Level | Home buttons | WinMenu/PauseMenu/GameOverMenu |
| Game | Game | Restart/Next | WinMenu/GameOverMenu |

### Build Settings:
Make sure all 4 scenes are in Build Settings:
```
File > Build Settings > Scenes In Build
0. LoadingScene
1. SelectAnimalScene
2. Level
3. Game
```

---

## 🎨 UI DESIGN TIPS

### Level Selection Screen (Scene: Level)
```
Mỗi level button cần:
- Level number (1, 2, 3...)
- Lock/Unlock state (icon ổ khóa)
- Stars display (0-3 stars)
- Current player indicator (avatar)
```

### Win Menu Layout:
```
WinMenu
├─ Level text: "Level X"
├─ Score text: "Score: XX"
├─ Stars (3 icons)
│  ├─ Star 1 (active nếu đạt)
│  ├─ Star 2 (active nếu đạt)
│  └─ Star 3 (active nếu đạt)
└─ Buttons
   ├─ Next Level (ẩn nếu hết level)
   ├─ Retry
   └─ Home
```

---

## 🧪 DEBUG TOOLS

### Unity Inspector Tools:
```
Select "LevelProgressManager" GameObject → Inspector:
- 📊 Log Progress: Xem tiến độ hiện tại
- 🔄 Reset All Progress: Reset toàn bộ
- Unlock Level 1-5: Quick unlock
- ⭐ Test Stars: Test lưu sao
```

### Code Debug:
```csharp
// Check level unlock
bool unlocked = LevelProgressManager.GetInstance().IsLevelUnlocked(2);

// Get stars
int stars = LevelProgressManager.GetInstance().GetLevelStars(1);

// Unlock level
LevelProgressManager.GetInstance().UnlockLevel(5);

// Reset progress
LevelProgressManager.GetInstance().ResetAllProgress();

// Log all progress
LevelProgressManager.GetInstance().LogProgress();
```

### LevelButton Context Menu:
```
Right-click LevelButton component:
- Debug: Unlock This Level
- Debug: Set 3 Stars
```

---

## 🐛 TROUBLESHOOTING

### "LevelManager not found"
→ Có LevelManager component trong Game scene chưa?
→ Đảm bảo LevelProgressManager đã setup trong Level scene

### "LevelDataSO is null"
→ Check LevelProgressManager có assign 10 Level Data chưa?
→ Verify folder `Assets/_Game/Data/Levels/` có 10 files

### Stars không đúng
→ Check miss tracking: Console log số lần miss
→ Verify logic: 0 miss = 3⭐, 1 miss = 2⭐, 2+ miss = 1⭐

### Pause không hoạt động
→ Check PauseMenu Type = Pause
→ Check PauseMenu đã add vào MenuManager chưa?
→ Check GameplayMenu có pause button assigned?

### Win menu không hiện
→ Check WinMenu Type = Win
→ Check WinMenu đã add vào MenuManager "_menus" list?
→ Check Console có error không

### Level không unlock
→ Level 1 tự động unlock lúc start
→ Level khác unlock sau khi win level trước
→ Dùng debug tool để unlock thủ công

### Scene not found
→ Check scene name: "Level" và "Game" (case-sensitive!)
→ Check Build Settings có tất cả 4 scenes?
→ Scene names: LoadingScene, SelectAnimalScene, Level, Game

### Time.timeScale stuck = 0
→ Game bị pause, check PauseMenu
→ Restart Unity Editor
→ Manual set: `Time.timeScale = 1f;`

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
"CurrentLevel" = 1-10 (level đang chọn)
"BestScore" = highest score (legacy)
```

### LevelDataSO Config:
```
- levelNumber: int (1-10)
- levelName: string ("Level 1")
- targetScore: int (điểm cần đạt)
- coinReward: int (coin thưởng - TODO)
- platformSpeed: float (tốc độ)
- platformGapRange: Vector2 (gap min-max)
- safeLandingZoneRatio: float (0-1)
- hasObstacles: bool
- speedIncreaseRate: float
```

---

## 📝 NOTES

### ScoreManager
- Lưu "BestScore" toàn game (legacy)
- Level system hoạt động độc lập

### LevelManager
- Đã refactor: load từ LevelDataSO
- Không dùng high score để tính level nữa

### Audio
- Win sound: Hiện dùng GAMEOVER sound
- TODO: Add AudioType.WIN mới

### Currency System (TODO)
- Coin reward đã có trong LevelDataSO
- Chờ implement CurrencyManager
- Code sẵn sàng trong GameManager.GameWin()

---

## 🎯 PRIORITY CHECKLIST

### Must Have (Để game chạy):
- [x] Tạo Level Data (10 files)
- [ ] Setup Level scene: LevelProgressManager + Level Buttons
- [ ] Setup Game scene: WinMenu + PauseMenu
- [ ] Test: Flow hoàn chỉnh 4 scenes

### Nice to Have (Polish):
- [ ] Animations cho menus
- [ ] Sound effects (win, star, level up)
- [ ] Particle effects khi win
- [ ] Level selection UI đẹp hơn
- [ ] Transitions giữa scenes

---

## 📞 SUPPORT & CONTACT

### Debug Resources:
1. Console logs (nhiều debug info chi tiết)
2. LevelProgressManager Inspector (debug buttons)
3. Unity Menu: Tools > Game > ...

### Documentation:
- **SETUP_GUIDE.md** (file này) - Hướng dẫn đầy đủ
- **FILES_CREATED.md** - Danh sách files đã tạo/sửa
- **README.md** - Overview tổng quan

### If stuck:
1. Check Console for errors
2. Verify scene names (case-sensitive!)
3. Check Build Settings có đủ scenes
4. Use Debug Tools trong Inspector
5. Reset progress và test lại

---

## � KẾT LUẬN

Hệ thống level đã hoàn chỉnh với:
- ✅ 10 level có thể config
- ✅ Star system 3 mức
- ✅ Win/Lose conditions
- ✅ Pause system
- ✅ Progress saving
- ✅ 4 scenes flow

**Chỉ cần setup UI trong Unity Editor là game chạy được!**

---

Good luck! �🎮✨

*Last updated: November 4, 2025*
