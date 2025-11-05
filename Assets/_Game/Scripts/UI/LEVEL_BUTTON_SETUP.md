# 🎯 LEVEL BUTTON SETUP GUIDE

## 📋 UI Structure

Dựa trên hình, mỗi Level Button có cấu trúc sau:

```
Level Button (GameObject + Button + LevelButton script)
├─ Radial Shine (GameObject) - Hiệu ứng sáng quanh button khi unlock
├─ Shadow (GameObject) - Bóng của button
├─ Lines (GameObject) - Hiệu ứng đường tia khi unlock
├─ UnLock (GameObject) - Container UI khi level đã mở
├─ Lock (GameObject) - Icon khóa khi level bị khóa
├─ Text (TMP) (TextMeshPro) - Số level (1, 2, 3...)
└─ Stars (GameObject) - Container chứa 3 sao
    ├─ Star 1 (GameObject/Image)
    ├─ Star 2 (GameObject/Image)
    └─ Star 3 (GameObject/Image)
```

---

## 🛠️ SETUP TRONG UNITY

### Bước 1: Tạo Level Button GameObject

1. Duplicate button level hiện có HOẶC tạo mới:
   - Right-click trong Hierarchy
   - UI > Button - TextMeshPro
   - Rename: "Level Button"

2. Add component: **LevelButton** script

---

### Bước 2: Assign Fields trong Inspector

#### **Level Info Section:**
```
Level Number: 1 (hoặc 2, 3, 4... tùy button)
Button: Assign Button component của chính GameObject này
```

#### **Text Section:**
```
Level Text: Kéo TextMeshPro "Text (TMP)" vào đây
```

#### **Lock/Unlock GameObjects Section:**
```
Lock: Kéo GameObject "Lock" (icon khóa) vào
UnLock: Kéo GameObject "UnLock" vào
```

#### **Visual Effects (Optional) Section:**
```
Radial Shine: Kéo GameObject "Radial Shine" vào
Lines: Kéo GameObject "Lines" vào
```

#### **Stars Section:**
```
Stars Container: Kéo GameObject "Stars" vào
Star Images (Size = 3):
  Element 0: Kéo Image component của Star 1 vào
  Element 1: Kéo Image component của Star 2 vào  
  Element 2: Kéo Image component của Star 3 vào
```

#### **Star Sprites Section:**
```
Yellow Star Sprite: Kéo sprite sao vàng (khi đạt được sao)
Gray Star Sprite: Kéo sprite sao xám (khi chưa đạt được sao)
```

#### **Settings Section:**
```
Game Scene Name: "Game" (tên scene chơi game)
```

---

## 🎨 CƠ CHẾ HOẠT ĐỘNG

### Khi Level LOCKED (khóa):
```
✅ Active:
- Lock (icon khóa)
- Shadow
- Text (TMP) - hiển thị số level

❌ Inactive:
- UnLock
- Radial Shine
- Lines
- Stars (container)
- Button.interactable = false
```

### Khi Level UNLOCKED (đã mở):
```
✅ Active:
- UnLock
- Radial Shine (hiệu ứng sáng)
- Lines (hiệu ứng tia)
- Shadow
- Text (TMP) - hiển thị số level
- Stars (container)
- Stars icons (theo số sao đạt được)
- Button.interactable = true

❌ Inactive:
- Lock (ẩn icon khóa)
```

### Stars Display Logic:
```
0 sao: Star 1,2,3 = Gray sprite
1 sao: Star 1 = Yellow, Star 2,3 = Gray
2 sao: Star 1,2 = Yellow, Star 3 = Gray  
3 sao: Star 1,2,3 = Yellow sprite

Tất cả star images luôn active, chỉ đổi sprite
```

---

## 📝 CHECKLIST SETUP

### Mỗi Level Button cần:
- [ ] GameObject có Button component
- [ ] Add LevelButton script
- [ ] Set Level Number (1-10)
- [ ] Assign Button reference
- [ ] Assign Text (TMP)
- [ ] Assign Lock GameObject
- [ ] Assign UnLock GameObject
- [ ] Assign Radial Shine (optional)
- [ ] Assign Lines (optional)
- [ ] Assign Stars container
- [ ] Assign 3 Star Images (Image components)
- [ ] Assign Yellow Star Sprite
- [ ] Assign Gray Star Sprite
- [ ] Game Scene Name = "Game"

### Verify:
- [ ] Lock active khi chưa unlock
- [ ] UnLock active khi đã unlock
- [ ] Stars luôn hiển thị với gray/yellow sprites
- [ ] Click button → Load Game scene
- [ ] Visual effects (Radial Shine, Lines) chỉ hiện khi unlock

---

## 🎨 SPRITES REQUIREMENTS

### Star Sprites cần chuẩn bị:
```
1. Yellow Star Sprite:
   - Màu vàng, sáng
   - Dùng khi player đạt được sao
   - Size khuyến nghị: 64x64 hoặc 128x128

2. Gray Star Sprite:  
   - Màu xám, tối
   - Dùng khi player chưa đạt được sao
   - Cùng size với Yellow Star
   - Có thể là version desaturated của yellow star
```

### Import Settings:
```
- Sprite Mode: Single
- Pixels Per Unit: 100 (hoặc phù hợp với UI scale)
- Filter Mode: Bilinear
- Format: RGBA 32 bit (cho chất lượng tốt)
```

---

## 🛠️ SETUP SPRITES

### Bước 1: Import Sprites
```
1. Drag 2 star sprites vào Project
2. Set Texture Type = Sprite (2D and UI)
3. Apply settings
```

### Bước 2: Assign vào LevelButton
```
1. Select Level Button GameObject
2. LevelButton component > Star Sprites section:
   - Yellow Star Sprite: Kéo yellow star sprite
   - Gray Star Sprite: Kéo gray star sprite
```

### Bước 3: Setup Star Images
```
Đảm bảo mỗi star trong Stars container:
1. Có Image component (không phải raw image)
2. Source Image có thể để trống (script sẽ set)
3. Preserve Aspect = true (khuyến nghị)
```

---

## 🔧 TẠO 10 LEVEL BUTTONS

### Cách 1: Duplicate & Update
```
1. Setup Level Button đầu tiên hoàn chỉnh
2. Duplicate 9 lần (Ctrl+D)
3. Rename: Level Button 1, 2, 3... 10
4. Mỗi button: Chỉ cần đổi "Level Number" field (1-10)
5. Arrange trong Grid Layout
```

### Cách 2: Prefab (Khuyến nghị)
```
1. Setup Level Button đầu tiên
2. Drag vào Project → Tạo Prefab
3. Instantiate Prefab 10 lần
4. Unpack prefab (right-click > Unpack)
5. Đổi Level Number cho mỗi instance
```

---

## 🧪 TESTING

### Test Level 1 (Auto unlock):
```
1. Play scene
2. Level 1 button:
   - Lock: Inactive
   - UnLock: Active
   - Radial Shine: Active
   - Lines: Active
   - Stars: Active với gray sprites (0 sao ban đầu)
   - Button clickable
3. Click level 1 → Load Game scene
```

### Test Level 2+ (Locked):
```
1. Play scene
2. Level 2 button:
   - Lock: Active (icon khóa hiện)
   - UnLock: Inactive
   - Radial Shine: Inactive
   - Lines: Inactive
   - Stars: Inactive (không hiện stars container)
   - Button NOT clickable
3. Click → Không load scene
```

### Test Stars Sprites:
```
1. Select LevelProgressManager trong scene
2. Inspector > Debug Tools > "Unlock Level 1-5"
3. Select LevelButton (Level 1)
4. Right-click component > "Debug: Set 3 Stars"
5. Verify: 
   - Star 1, 2, 3 đều hiển thị yellow sprite
6. LevelProgressManager > Save Level Stars = 1
7. Verify:
   - Star 1 = yellow sprite
   - Star 2, 3 = gray sprite
```

---

## 🎨 VISUAL HIERARCHY (Layer Order)

Đảm bảo render order đúng:
```
1. Shadow (dưới cùng)
2. Radial Shine (hiệu ứng nền)
3. UnLock / Lock (nút chính)
4. Lines (hiệu ứng tia)
5. Text (TMP) (số level ở trên)
6. Stars (trên cùng)
```

---

## 💡 TIPS

### UI Scale & Position:
- Dùng RectTransform để position buttons
- Khuyến nghị: Grid Layout Group để tự động arrange
- Scale uniform cho tất cả buttons

### Performance:
- Disable Raycast Target cho các Image không cần click (Shadow, Lines, Stars)
- Chỉ Button component cần Raycast Target = true

### Animation (Optional):
- Thêm Animator cho hover/click effects
- Scale tween khi unlock level mới
- Particle effects khi đạt 3 sao

---

## 🐛 TROUBLESHOOTING

### Button không click được:
→ Check Button.interactable = true
→ Check level đã unlock chưa?

### Stars không hiển thị:
→ Check Stars container active?
→ Check Star Images assigned đúng (Image components, không phải GameObjects)?
→ Check sprites assigned: Yellow Star + Gray Star?
→ Check Console có warning không?

### Stars không đổi màu:
→ Verify sprites khác nhau (yellow vs gray)
→ Check Image.sprite được set đúng?
→ Test với different số sao (0, 1, 2, 3)

### Missing sprites warnings:
→ Assign Yellow Star Sprite và Gray Star Sprite
→ Check sprites import settings (Texture Type = Sprite)

### Image component not found:
→ Star Images phải là Image components, không phải GameObjects
→ Mỗi star trong Stars container cần có Image component

### Lock/UnLock không đổi:
→ Verify GameObject names đúng: "Lock" và "UnLock"
→ Check assigned trong Inspector

### Visual effects không hiện:
→ Check Radial Shine, Lines assigned?
→ Check level đã unlock?

---

Good luck! 🎮✨
