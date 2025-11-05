# 💰 Currency & Animal Ownership System - Quick Reference

## 📁 Files Created

### Managers (Core System)
1. **`CurrencyManager.cs`** - Quản lý coin (Singleton, DontDestroyOnLoad)
2. **`AnimalOwnershipManager.cs`** - Quản lý unlock/lock động vật (Singleton)

### Data
3. **`AnimalData.cs`** - Data class và extension methods cho AnimalType

### UI Components
4. **`CurrencyDisplay.cs`** - Hiển thị số coin (auto-update với animation)
5. **`AnimalShopButton.cs`** - Button mua/chọn động vật
6. **`AnimalSelector.cs`** - ✏️ Updated để support ownership system

### Win Menu Integration
7. **`WinMenu.cs`** - ✏️ Updated để cộng coin khi thắng level

### Debug Tools
8. **`CurrencyDebugPanel.cs`** - Panel debug để test hệ thống

### Documentation
9. **`CURRENCY_SYSTEM_README.md`** - Hướng dẫn chi tiết setup và sử dụng

---

## 🎯 Key Features

✅ **Currency System**
- Lưu coin vào PlayerPrefs
- Add/Spend với validation
- Events để UI tự động update
- Debug commands (Context Menu)

✅ **Animal Ownership**
- Kangaroo unlock mặc định (free)
- Elephant: 500 coins
- Lion: 1000 coins  
- Bear: 1500 coins
- Chỉ select được động vật đã unlock
- Mua tự động trừ coin

✅ **UI Integration**
- CurrencyDisplay với animation
- AnimalSelector support buy/unlock
- Win menu tự động thưởng coin

---

## 🚀 Quick Setup (3 Steps)

### 1. Tạo Managers (trong Loading/Main scene)
```
Hierarchy → Create Empty "CurrencyManager" → Add CurrencyManager.cs
Hierarchy → Create Empty "AnimalOwnershipManager" → Add AnimalOwnershipManager.cs
```

### 2. Config Animal Database
```
Select AnimalOwnershipManager → Inspector → Animal Database (size=4)
[0] Kangaroo: price=0
[1] Elephant: price=500
[2] Lion: price=1000
[3] Bear: price=1500
```

### 3. Add UI Components
```
Thêm CurrencyDisplay vào các UI cần hiển thị coin
Assign TMP_Text reference
```

---

## 💡 Usage Examples

```csharp
// Lấy coin hiện tại
int coins = CurrencyManager.GetInstance().Coins;

// Thêm coin
CurrencyManager.GetInstance().AddCoins(100);

// Kiểm tra unlock
bool isUnlocked = AnimalOwnershipManager.GetInstance()
    .IsAnimalUnlocked(AnimalType.Elephant);

// Mua động vật
bool success = AnimalOwnershipManager.GetInstance()
    .PurchaseAnimal(AnimalType.Lion);
```

---

## 🐛 Debug (Context Menu)

**CurrencyManager:**
- Right-click → Debug: Add 1000 Coins
- Right-click → Debug: Reset Coins

**AnimalOwnershipManager:**
- Right-click → Debug: Unlock All Animals
- Right-click → Debug: Reset All Animals
- Right-click → Debug: Log Animal Status

---

## ✅ Testing

1. Play game → Check Kangaroo auto-unlocked
2. Win level → Check coin reward received
3. Go to Animal Selection → Check locked animals show price
4. Try to buy animal → Check coin deducted
5. Exit and restart → Check coin & unlock status persisted

---

**📖 For detailed documentation, see `CURRENCY_SYSTEM_README.md`**
