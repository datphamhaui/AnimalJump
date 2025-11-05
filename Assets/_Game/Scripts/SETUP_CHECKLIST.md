# ✅ Setup Checklist - Currency & Animal Ownership System

## 📋 Pre-Setup Checklist

- [ ] Đã backup project
- [ ] Đã đọc `CURRENCY_QUICK_REFERENCE.md`
- [ ] Unity đang mở project AnimalJump
- [ ] Không có compile errors

---

## 🎯 STEP 1: Create Manager GameObjects

### 1.1 CurrencyManager
- [ ] Mở scene **Loading** hoặc scene đầu tiên của game
- [ ] Tạo Empty GameObject: `Create Empty` → Rename thành `CurrencyManager`
- [ ] Add Component: `CurrencyManager.cs`
- [ ] Verify trong Inspector:
  - [ ] Script attached thành công
  - [ ] Không có missing references

### 1.2 AnimalOwnershipManager
- [ ] Trong cùng scene, tạo Empty GameObject: `Create Empty` → Rename thành `AnimalOwnershipManager`
- [ ] Add Component: `AnimalOwnershipManager.cs`
- [ ] **QUAN TRỌNG:** Config Animal Database trong Inspector:
  - [ ] Set **Animal Database → Size = 4**
  - [ ] Element 0:
    - [ ] Animal Type: `Kangaroo`
    - [ ] Display Name: `"Kangaroo"`
    - [ ] Price: `0`
  - [ ] Element 1:
    - [ ] Animal Type: `Elephant`
    - [ ] Display Name: `"Elephant"`
    - [ ] Price: `500`
  - [ ] Element 2:
    - [ ] Animal Type: `Lion`
    - [ ] Display Name: `"Lion"`
    - [ ] Price: `1000`
  - [ ] Element 3:
    - [ ] Animal Type: `Bear`
    - [ ] Display Name: `"Bear"`
    - [ ] Price: `1500`

### 1.3 Verify Managers Setup
- [ ] Play scene
- [ ] Check Console: Không có errors
- [ ] Check Hierarchy khi đang Play:
  - [ ] `CurrencyManager` tồn tại trong **DontDestroyOnLoad**
  - [ ] `AnimalOwnershipManager` tồn tại trong **DontDestroyOnLoad**
- [ ] Stop playing

---

## 🎨 STEP 2: Setup UI - Currency Display

### 2.1 Main Menu Scene
- [ ] Mở scene **Main Menu** / **Level Selection**
- [ ] Tìm hoặc tạo UI Canvas
- [ ] Tạo coin display:
  - [ ] Tạo Panel: `Right-click Canvas → UI → Panel` → Rename `CoinPanel`
  - [ ] Position ở góc trên phải (recommend)
  - [ ] Trong CoinPanel:
    - [ ] Add Image (Icon): `UI → Image` → Assign coin sprite
    - [ ] Add Text: `UI → Text - TextMeshPro` → Rename `CoinText`
- [ ] Add Component vào CoinPanel: `CurrencyDisplay.cs`
- [ ] Assign references:
  - [ ] Coin Text: Drag `CoinText` vào
  - [ ] Coin Icon: Drag Image vào (optional)
  - [ ] Use Animation: `✓` (checked)
  - [ ] Animation Duration: `0.5`

### 2.2 Duplicate to Other Scenes
- [ ] Copy `CoinPanel` (Ctrl+C)
- [ ] Mở **Animal Selection Scene**
  - [ ] Paste vào Canvas (Ctrl+V)
  - [ ] Position phù hợp
- [ ] Lặp lại cho:
  - [ ] Win Menu
  - [ ] Lose Menu
  - [ ] Gameplay UI (optional)

---

## 🦘 STEP 3: Update Animal Selection UI

### 3.1 Find AnimalSelector GameObject
- [ ] Mở scene **Animal Selection**
- [ ] Tìm GameObject có `AnimalSelector.cs` component

### 3.2 Create Animal Info Panel
- [ ] Tạo Panel mới trong Canvas: `UI → Panel` → Rename `AnimalInfoPanel`
- [ ] Position bên dưới hoặc bên cạnh animal display
- [ ] Trong AnimalInfoPanel:
  - [ ] Add Text (TMP): `AnimalNameText` (size lớn, bold)
  - [ ] Add Text (TMP): `StatusText` (màu xanh/đỏ)
  - [ ] Create Panel: `PricePanel`
    - [ ] Add Image: Coin icon
    - [ ] Add Text (TMP): `PriceText`

### 3.3 Update AnimalSelector Component
- [ ] Select GameObject có `AnimalSelector.cs`
- [ ] Trong Inspector, assign các field mới:
  - [ ] Animal Name Text: Drag `AnimalNameText`
  - [ ] Status Text: Drag `StatusText`
  - [ ] Price Text: Drag `PriceText`
  - [ ] Price Display: Drag `PricePanel`

### 3.4 Update Select Button
- [ ] Tìm `Select Button` trong scene
- [ ] Check Button component text sẽ đổi dynamic:
  - Unlocked: "SELECT"
  - Locked: "BUY (XXX COINS)"

---

## 🧪 STEP 4: Testing

### 4.1 First Run Test
- [ ] Delete PlayerPrefs: `Edit → Clear All PlayerPrefs` (nếu có)
- [ ] Play game từ Loading scene
- [ ] Check Console:
  - [ ] `[CurrencyManager] Loaded coins: 0`
  - [ ] `[AnimalOwnershipManager] Kangaroo unlocked by default`
- [ ] Check UI:
  - [ ] Coin display shows `0`
- [ ] Stop playing

### 4.2 Debug Commands Test
- [ ] Play game
- [ ] Select `CurrencyManager` trong Hierarchy (DontDestroyOnLoad)
- [ ] Right-click script → `Debug: Add 1000 Coins`
- [ ] Check:
  - [ ] Console: `[CurrencyManager] +1000 coins → Total: 1000`
  - [ ] UI auto-updates to `1000`
- [ ] Select `AnimalOwnershipManager`
- [ ] Right-click script → `Debug: Log Animal Status`
- [ ] Check Console shows:
  ```
  Kangaroo: ✅ Unlocked
  Elephant: 🔒 Locked (500 coins)
  Lion: 🔒 Locked (1000 coins)
  Bear: 🔒 Locked (1500 coins)
  ```
- [ ] Stop playing

### 4.3 Animal Selection Test
- [ ] Play game → Navigate to Animal Selection
- [ ] Check UI:
  - [ ] Kangaroo: Shows "SELECT" button, status "Unlocked"
  - [ ] Other animals: Shows "BUY (XXX COINS)", status "Locked"
  - [ ] Price text màu trắng (nếu đủ coin) hoặc đỏ (không đủ)
- [ ] Try to select Kangaroo:
  - [ ] Click SELECT
  - [ ] Should load game scene successfully
- [ ] Go back to Animal Selection
- [ ] Try to select locked animal (Elephant):
  - [ ] If not enough coins: Nothing happens (need to add "not enough coins" popup later)
  - [ ] If enough coins: Buy successfully → Status changes to "Unlocked"

### 4.4 Win Level Test
- [ ] Play game → Win a level
- [ ] Check Win Menu:
  - [ ] Stars displayed correctly
  - [ ] Coin reward calculated correctly
  - [ ] Console: `[CurrencyManager] +XXX coins`
- [ ] Click Home → Check coin persisted
- [ ] Close game → Restart → Check coin still there

### 4.5 Purchase Test
- [ ] Use Debug: Add 1000 Coins
- [ ] Go to Animal Selection
- [ ] Buy Elephant (500 coins):
  - [ ] Click BUY button
  - [ ] Check Console: Purchase successful
  - [ ] Check coin: 1000 - 500 = 500
  - [ ] Button changes to "SELECT"
  - [ ] Status changes to "Unlocked"
- [ ] Try to buy again:
  - [ ] Should show "already unlocked" in console
- [ ] Buy Lion (1000 coins):
  - [ ] Should fail (only have 500)
  - [ ] Check console warning

### 4.6 Persistence Test
- [ ] Play game
- [ ] Buy some animals
- [ ] Stop playing
- [ ] Play again
- [ ] Check:
  - [ ] Coins persisted
  - [ ] Unlocked animals still unlocked
  - [ ] Selected animal still selected

---

## 🎉 STEP 5: Final Verification

### 5.1 Complete Feature Test
- [ ] Start fresh (Clear PlayerPrefs)
- [ ] Complete this flow:
  1. [ ] Start game → Kangaroo unlocked, 0 coins
  2. [ ] Win Level 1 → Get coins (e.g., 200 coins with 3 stars)
  3. [ ] Go to Animal Selection → See locked animals
  4. [ ] Buy Elephant (500 coins) → Fail (not enough)
  5. [ ] Win more levels → Get more coins
  6. [ ] Buy Elephant → Success
  7. [ ] Select Elephant → Play with Elephant
  8. [ ] Exit game
  9. [ ] Restart → Coins and unlocks persisted

### 5.2 UI Verification
- [ ] All CurrencyDisplay components update simultaneously
- [ ] Coin animation plays smoothly
- [ ] Animal button states correct (locked/unlocked/selected)
- [ ] Price text color changes based on affordability
- [ ] No UI overlapping or visual glitches

### 5.3 Console Log Check
- [ ] No errors in Console
- [ ] No warnings (except expected ones)
- [ ] All debug logs are clear and helpful

---

## 🐛 Troubleshooting

### If Kangaroo not unlocked:
1. [ ] Check AnimalOwnershipManager in scene
2. [ ] Check Awake() calls InitializeDefaultUnlocks()
3. [ ] Check PlayerPrefs: `AnimalUnlock_Kangaroo` should be 1

### If coins not saving:
1. [ ] Check CurrencyManager in scene
2. [ ] Check DontDestroyOnLoad works
3. [ ] Check SaveCoins() is called after changes

### If UI not updating:
1. [ ] Check CurrencyDisplay subscribed to OnCoinsChanged
2. [ ] Check OnEnable/OnDisable subscriptions
3. [ ] Check manager instances exist (GetInstance() returns non-null)

### If cannot buy animal:
1. [ ] Check coin amount
2. [ ] Check animal not already unlocked
3. [ ] Check price in Animal Database
4. [ ] Check CurrencyManager.SpendCoins() return value

---

## 📚 Next Steps (Optional Enhancements)

- [ ] Add "Not Enough Coins" popup
- [ ] Add purchase success animation/effect
- [ ] Add coin earn animation when winning
- [ ] Add daily reward system
- [ ] Add watch ads for coins feature
- [ ] Create separate Shop scene
- [ ] Add animal preview/stats
- [ ] Add special sale events

---

## ✅ Completion Checklist

- [ ] All managers setup correctly
- [ ] UI displays coins properly
- [ ] Animal selection works with ownership
- [ ] Win level rewards coins
- [ ] Can buy animals with coins
- [ ] All data persists across sessions
- [ ] No errors in Console
- [ ] Tested all features thoroughly

---

**🎊 Congratulations! Currency & Animal Ownership System is ready to use!**

**Need help?** See:
- `CURRENCY_QUICK_REFERENCE.md` for quick usage
- `CURRENCY_SYSTEM_README.md` for detailed docs
- `SYSTEM_ARCHITECTURE.md` for system design
