# 💰 Currency & Animal Ownership System

## 📋 Tổng quan

Hệ thống quản lý:
- **Currency (Coin)**: Lưu trữ và quản lý coin của người chơi
- **Animal Ownership**: Quản lý unlock/lock động vật, mua động vật bằng coin

## 🎮 Tính năng

### Currency System
- ✅ Lưu coin vào PlayerPrefs (persist qua các session)
- ✅ Add/Spend coin với validation
- ✅ Events khi coin thay đổi để UI tự động update
- ✅ Debug commands (Add 1000 coins, Reset coins, v.v.)

### Animal Ownership System
- ✅ Mặc định unlock **Kangaroo** (free)
- ✅ Các động vật khác cần coin để mở khóa:
  - 🐘 **Elephant**: 500 coins
  - 🦁 **Lion**: 1000 coins
  - 🐻 **Bear**: 1500 coins
- ✅ Chỉ cho phép select động vật đã unlock
- ✅ Mua động vật tự động trừ coin
- ✅ Lưu unlock status vào PlayerPrefs

### UI Components
- ✅ **CurrencyDisplay**: Hiển thị số coin (với animation)
- ✅ **AnimalShopButton**: Button mua/chọn động vật
- ✅ **AnimalSelector**: Cập nhật để hỗ trợ unlock system

### Win Menu Integration
- ✅ Khi thắng level, nhận coin reward
- ✅ Coin reward dựa trên số sao: 1⭐=1x, 2⭐=1.5x, 3⭐=2x

---

## 🛠️ Setup Guide

### 1️⃣ Setup CurrencyManager (Singleton)

1. Tạo GameObject mới trong scene: `_Managers/CurrencyManager`
2. Add component: `CurrencyManager.cs`
3. CurrencyManager sẽ tự động DontDestroyOnLoad

### 2️⃣ Setup AnimalOwnershipManager (Singleton)

1. Tạo GameObject mới trong scene: `_Managers/AnimalOwnershipManager`
2. Add component: `AnimalOwnershipManager.cs`
3. **QUAN TRỌNG**: Config Animal Database:
   - Mở Inspector của AnimalOwnershipManager
   - Trong `Animal Database`, set size = 4
   - Assign thông tin cho từng động vật:
     ```
     [0] Kangaroo: price=0, displayName="Kangaroo"
     [1] Elephant: price=500, displayName="Elephant"
     [2] Lion: price=1000, displayName="Lion"
     [3] Bear: price=1500, displayName="Bear"
     ```

### 3️⃣ Setup UI - CurrencyDisplay

**Thêm vào các scene cần hiển thị coin:**
- Main Menu
- Level Selection
- Animal Selection
- Win/Lose Menu

**Cách setup:**
1. Tạo UI Text (TMP) để hiển thị số coin
2. Add component: `CurrencyDisplay.cs`
3. Assign references:
   - `Coin Text`: TMP_Text hiển thị số
   - `Coin Icon`: Image icon coin (optional)
   - `Use Animation`: true (recommend)

### 4️⃣ Setup UI - AnimalSelector (Đã có, cần update)

**Trong scene Animal Selection:**
1. Mở Inspector của `AnimalSelector` GameObject
2. Thêm các field mới:
   - `Animal Name Text`: TMP_Text hiển thị tên động vật
   - `Status Text`: TMP_Text hiển thị "Locked/Unlocked"
   - `Price Text`: TMP_Text hiển thị giá
   - `Price Display`: GameObject chứa UI giá (ẩn khi unlocked)

**UI Structure đề xuất:**
```
AnimalSelector
├─ SimpleScrollSnap (existing)
├─ AnimalInfo Panel (NEW)
│  ├─ Name Text (TMP)
│  ├─ Status Text (TMP)
│  └─ Price Display (GameObject)
│     ├─ Coin Icon (Image)
│     └─ Price Text (TMP)
└─ Select Button (existing - text sẽ đổi thành "SELECT" hoặc "BUY")
```

### 5️⃣ Setup UI - AnimalShopButton (Optional - Alternative UI)

Nếu muốn dùng button riêng cho từng động vật thay vì scroll:

1. Tạo UI Button cho mỗi động vật
2. Add component: `AnimalShopButton.cs`
3. Assign references:
   - `Animal Type`: Chọn Kangaroo/Elephant/Lion/Bear
   - `Main Button`: Button component
   - `Name Text`, `Price Text`, v.v.
   - `Locked State`: GameObject hiển thị khi locked
   - `Unlocked State`: GameObject hiển thị khi unlocked
   - `Selected Highlight`: GameObject highlight khi được chọn

---

## 🎯 Cách sử dụng trong Code

### Lấy số coin hiện tại
```csharp
CurrencyManager currencyManager = CurrencyManager.GetInstance();
int currentCoins = currencyManager.Coins;
```

### Thêm coin
```csharp
currencyManager.AddCoins(100); // Thêm 100 coins
```

### Trừ coin (mua item)
```csharp
bool success = currencyManager.SpendCoins(500); // Trừ 500 coins
if (success)
{
    Debug.Log("Mua thành công!");
}
else
{
    Debug.Log("Không đủ coin!");
}
```

### Kiểm tra động vật đã unlock
```csharp
AnimalOwnershipManager ownershipManager = AnimalOwnershipManager.GetInstance();
bool isUnlocked = ownershipManager.IsAnimalUnlocked(AnimalType.Elephant);
```

### Mua động vật
```csharp
bool success = ownershipManager.PurchaseAnimal(AnimalType.Elephant);
if (success)
{
    Debug.Log("Mua Elephant thành công!");
}
```

### Chọn động vật (phải unlock trước)
```csharp
bool success = ownershipManager.SelectAnimal(AnimalType.Lion);
```

### Subscribe events
```csharp
private void OnEnable()
{
    CurrencyManager.OnCoinsChanged += OnCoinsChanged;
    AnimalOwnershipManager.OnAnimalUnlocked += OnAnimalUnlocked;
}

private void OnDisable()
{
    CurrencyManager.OnCoinsChanged -= OnCoinsChanged;
    AnimalOwnershipManager.OnAnimalUnlocked -= OnAnimalUnlocked;
}

private void OnCoinsChanged(int newAmount)
{
    Debug.Log($"Coin mới: {newAmount}");
}

private void OnAnimalUnlocked(AnimalType type)
{
    Debug.Log($"Unlock: {type.GetDisplayName()}");
}
```

---

## 🐛 Debug Commands

### CurrencyManager
- **Right-click script → Debug: Add 1000 Coins**
- **Right-click script → Debug: Add 10000 Coins**
- **Right-click script → Debug: Reset Coins**
- **Right-click script → Debug: Log Coins**

### AnimalOwnershipManager
- **Right-click script → Debug: Unlock All Animals**
- **Right-click script → Debug: Reset All Animals** (chỉ giữ Kangaroo)
- **Right-click script → Debug: Log Animal Status**

---

## 📊 Data Flow

```
Player wins level
    ↓
GameManager calculates stars
    ↓
WinMenu shows results
    ↓
CurrencyManager.AddCoins(reward)
    ↓
CurrencyManager.OnCoinsChanged event
    ↓
All CurrencyDisplay components auto-update
```

```
Player clicks "Buy Elephant"
    ↓
AnimalShopButton/AnimalSelector
    ↓
AnimalOwnershipManager.PurchaseAnimal()
    ↓
Check if unlocked? → Already unlocked!
Check enough coins? → Not enough!
    ↓
CurrencyManager.SpendCoins(500)
    ↓
AnimalOwnershipManager.UnlockAnimal()
    ↓
Save to PlayerPrefs
    ↓
Trigger events → UI auto-update
```

---

## ⚙️ PlayerPrefs Keys

- `PlayerCoins`: Số coin hiện tại
- `AnimalUnlock_Kangaroo`: 1=unlocked, 0=locked
- `AnimalUnlock_Elephant`: 1=unlocked, 0=locked
- `AnimalUnlock_Lion`: 1=unlocked, 0=locked
- `AnimalUnlock_Bear`: 1=unlocked, 0=locked
- `SelectedAnimal`: Panel index động vật đã chọn (1-4)

---

## 🎨 Giá động vật mặc định

| Động vật | Giá | Status mặc định |
|----------|-----|-----------------|
| 🦘 Kangaroo | 0 (Free) | ✅ Unlocked |
| 🐘 Elephant | 500 coins | 🔒 Locked |
| 🦁 Lion | 1000 coins | 🔒 Locked |
| 🐻 Bear | 1500 coins | 🔒 Locked |

---

## ✅ Testing Checklist

- [ ] CurrencyManager xuất hiện trong Hierarchy (DontDestroyOnLoad)
- [ ] AnimalOwnershipManager xuất hiện trong Hierarchy (DontDestroyOnLoad)
- [ ] Kangaroo tự động unlock khi chơi lần đầu
- [ ] CurrencyDisplay hiển thị đúng số coin
- [ ] Win level → nhận coin
- [ ] AnimalSelector hiển thị trạng thái lock/unlock
- [ ] Mua động vật → trừ coin → unlock
- [ ] Chỉ có thể select động vật đã unlock
- [ ] Coin được lưu khi thoát game và load lại

---

## 🚀 Next Steps (Optional Enhancements)

1. **Shop Scene riêng**: Tạo scene shop với grid các animal buttons
2. **Daily Rewards**: Thưởng coin hàng ngày
3. **Watch Ads for Coins**: Xem quảng cáo nhận coin
4. **Special Events**: Sale động vật giảm giá
5. **Coin Rewards**: Nhặt coin trong gameplay
6. **Achievement System**: Hoàn thành achievement nhận coin
7. **Not Enough Coins Popup**: Hiển thị popup khi không đủ coin

---

**🎉 Hệ thống đã sẵn sàng sử dụng!**
