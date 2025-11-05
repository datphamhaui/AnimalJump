# Pause Button Setup Guide

## Quick Setup (3 bước đơn giản)

### 1️⃣ Tạo Pause Button UI
```
Canvas (Gameplay UI)
└── PauseButton
    ├── Image (icon pause: ||)
    └── Button component
```

### 2️⃣ Add Script
- Select **PauseButton** GameObject
- Add Component → **PauseButton.cs**
- Script sẽ tự động attach vào Button component

### 3️⃣ Done! ✅
- Click button → Mở Pause menu
- Press ESC → Mở Pause menu (tự động)

---

## Chi Tiết Setup

### UI Hierarchy Khuyên Dùng
```
Game Scene
└── Canvas (Gameplay UI)
    ├── ScoreText (top left)
    ├── PauseButton (top right) ← SETUP NÀY
    │   ├── Button component
    │   ├── Image (pause icon)
    │   └── PauseButton.cs ← Add script này
    └── Other gameplay UI...
```

### Vị Trí Button
- **Top Right**: Góc phải trên cùng (standard)
- **Top Left**: Góc trái trên (nếu score ở phải)
- **Size**: 50x50 đến 80x80 pixels

### Icon Gợi Ý
- ⏸️ Pause symbol: `||`
- ⚙️ Settings gear
- 📋 Menu icon (3 lines)

---

## Features

### ✨ Auto Handling
- **Click button**: Tự động mở PauseMenu
- **ESC key**: Tự động mở PauseMenu (desktop)
- **Button disable**: Tránh click spam
- **Auto re-enable**: Khi resume game

### 🔄 Flow
```
Click Pause Button
    ↓
PauseButton.OnPauseButtonClicked()
    ↓
MenuManager.OpenMenu(MenuType.Pause)
    ↓
PauseMenu.SetEnable()
    ↓
Time.timeScale = 0 (game freeze)

[User clicks Resume]
    ↓
PauseMenu.SetDisable()
    ↓
Time.timeScale = 1 (game resume)
```

---

## Code Reference

### PauseButton.cs (Auto-generated)
```csharp
// Đã tự động:
// ✅ Attach vào Button component
// ✅ Add click listener
// ✅ Call MenuManager.OpenMenu(MenuType.Pause)
// ✅ Hỗ trợ ESC key
// ✅ Disable/enable button tự động
```

### Nếu Muốn Custom
```csharp
// File: PauseButton.cs

// Tắt ESC key support:
// → Comment/xóa method Update()

// Thay đổi key khác:
if (Input.GetKeyDown(KeyCode.P)) // P key thay vì ESC
{
    OnPauseButtonClicked();
}

// Add sound effect:
private void OnPauseButtonClicked()
{
    SoundController.GetInstance().PlayAudio(AudioType.BUTTON_CLICK);
    MenuManager.GetInstance().OpenMenu(MenuType.Pause);
}
```

---

## Troubleshooting

### ❌ Button không hoạt động
- Check: Button component có được add không?
- Check: PauseButton.cs có attach không?
- Check: MenuManager có trong scene không?
- Check: PauseMenu đã register trong MenuManager chưa?

### ❌ ESC key không hoạt động
- Check: `MenuType.Gameplay` có đúng không?
- Check: Update() method có bị comment không?
- Check: Input System đang dùng (Old/New)?

### ❌ Button bị disable mãi
- Restart game
- Check: OnEnable() có được gọi không?

---

## Alternative: Không Dùng Script

Nếu không muốn dùng PauseButton.cs, có thể setup trực tiếp:

### Option 1: Via Inspector
```
1. Create empty GameObject: "GameplayUIController"
2. Add script với method:
   public void OnPauseButtonClick()
   {
       MenuManager.GetInstance().OpenMenu(MenuType.Pause);
   }
3. Button OnClick() → Drag GameObject → Select method
```

### Option 2: Via Code Inline
```csharp
// Trong GameManager hoặc UI Manager:
[SerializeField] private Button pauseButton;

void Start()
{
    pauseButton.onClick.AddListener(() => {
        MenuManager.GetInstance().OpenMenu(MenuType.Pause);
    });
}
```

---

## Khuyến Nghị ⭐

**Dùng PauseButton.cs** vì:
- ✅ Tự động handle everything
- ✅ Hỗ trợ ESC key built-in
- ✅ Clean, reusable component
- ✅ Debug logging included
- ✅ Error handling

**Setup nhanh**: Chỉ cần add component vào Button GameObject!
