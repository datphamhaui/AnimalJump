# 🏗️ Currency & Animal Ownership System Architecture

## 📊 System Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                    CURRENCY & OWNERSHIP SYSTEM                   │
└─────────────────────────────────────────────────────────────────┘
                                 │
                ┌────────────────┴────────────────┐
                │                                 │
        ┌───────▼────────┐               ┌───────▼────────┐
        │ CurrencyManager │               │ AnimalOwnership│
        │   (Singleton)   │               │    Manager     │
        │                 │               │   (Singleton)  │
        └────────┬────────┘               └────────┬───────┘
                 │                                 │
                 │ Events:                         │ Events:
                 │ - OnCoinsChanged                │ - OnAnimalUnlocked
                 │ - OnCoinsAdded                  │ - OnAnimalSelected
                 │ - OnCoinsSpent                  │
                 │                                 │
      ┌──────────┴──────────┐           ┌─────────┴──────────┐
      │                     │           │                    │
┌─────▼─────┐        ┌──────▼─────┐   ┌▼────────┐   ┌───────▼────────┐
│ Currency  │        │  WinMenu   │   │ Animal  │   │ AnimalShopButton│
│  Display  │        │ (Add Coins)│   │ Selector│   │                │
└───────────┘        └────────────┘   └─────────┘   └────────────────┘
```

---

## 🔄 Data Flow Diagrams

### 💰 Coin Flow When Winning Level

```
┌──────────────┐
│  Player Wins │
│    Level     │
└──────┬───────┘
       │
       ▼
┌──────────────────────┐
│    GameManager       │
│ Calculate Stars:     │
│ 0 miss = 3⭐         │
│ 1 miss = 2⭐         │
│ 2+ miss = 1⭐        │
└──────┬───────────────┘
       │
       ▼
┌──────────────────────┐
│      WinMenu         │
│ Calculate Rewards:   │
│ 1⭐ = 1x coins       │
│ 2⭐ = 1.5x coins     │
│ 3⭐ = 2x coins       │
└──────┬───────────────┘
       │
       ▼
┌──────────────────────┐
│  CurrencyManager     │
│  AddCoins(amount)    │
│  Save to PlayerPrefs │
└──────┬───────────────┘
       │
       ▼
┌──────────────────────┐
│ OnCoinsChanged Event │
└──────┬───────────────┘
       │
       ▼
┌──────────────────────┐
│  CurrencyDisplay     │
│  (All instances)     │
│  Auto-update with    │
│  animation           │
└──────────────────────┘
```

---

### 🦘 Animal Purchase Flow

```
┌──────────────┐
│ Player Clicks│
│ "BUY Animal" │
└──────┬───────┘
       │
       ▼
┌──────────────────────────┐
│ AnimalOwnershipManager   │
│ PurchaseAnimal(type)     │
└──────┬───────────────────┘
       │
       ├─────► Check Already Unlocked? ──► YES ──► Return false
       │
       ▼ NO
       │
       ├─────► Get Price (from database or default)
       │
       ▼
┌──────────────────────────┐
│   CurrencyManager        │
│   HasEnoughCoins(price)? │
└──────┬───────────────────┘
       │
       ├─────► NO ──► Show "Not enough coins" ──► Return false
       │
       ▼ YES
       │
┌──────────────────────────┐
│   CurrencyManager        │
│   SpendCoins(price)      │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│ AnimalOwnershipManager   │
│ UnlockAnimal(type)       │
│ Save to PlayerPrefs      │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│  Events Triggered:       │
│  - OnAnimalUnlocked      │
│  - OnCoinsChanged        │
└──────┬───────────────────┘
       │
       ├────────┬────────┐
       │        │        │
       ▼        ▼        ▼
   ┌─────┐  ┌─────┐  ┌─────┐
   │ UI  │  │ UI  │  │ UI  │
   │Auto │  │Auto │  │Auto │
   │Update│ │Update│ │Update│
   └─────┘  └─────┘  └─────┘
```

---

### 🎯 Animal Selection Flow

```
┌──────────────────┐
│ Player Opens     │
│ Animal Selector  │
└────────┬─────────┘
         │
         ▼
┌────────────────────────┐
│ For Each Animal:       │
│ Check IsUnlocked()     │
└────────┬───────────────┘
         │
    ┌────┴─────┐
    │          │
    ▼          ▼
┌────────┐  ┌────────┐
│Unlocked│  │ Locked │
└───┬────┘  └───┬────┘
    │           │
    │           ▼
    │      ┌──────────────┐
    │      │ Show Price   │
    │      │ Check if     │
    │      │ enough coins │
    │      └──────────────┘
    │
    ▼
┌──────────────────┐
│ Allow Selection  │
│ Show "SELECT"    │
└──────┬───────────┘
       │
       ▼ Player clicks SELECT
       │
┌──────────────────────────┐
│ AnimalOwnershipManager   │
│ SelectAnimal(type)       │
│ Save to PlayerPrefs      │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────┐
│ Load Game Scene  │
│ with selected    │
│ animal           │
└──────────────────┘
```

---

## 💾 Data Persistence (PlayerPrefs)

```
┌─────────────────────────────────────┐
│          PlayerPrefs Keys           │
├─────────────────────────────────────┤
│                                     │
│  Currency:                          │
│  ├─ "PlayerCoins": int              │
│                                     │
│  Animal Ownership:                  │
│  ├─ "AnimalUnlock_Kangaroo": 0/1    │
│  ├─ "AnimalUnlock_Elephant": 0/1    │
│  ├─ "AnimalUnlock_Lion": 0/1        │
│  └─ "AnimalUnlock_Bear": 0/1        │
│                                     │
│  Selection:                         │
│  └─ "SelectedAnimal": 1-4 (int)     │
│                                     │
└─────────────────────────────────────┘
```

---

## 🎨 UI Component Hierarchy

### CurrencyDisplay Component
```
GameObject
├─ CurrencyDisplay.cs
│  ├─ Coin Text (TMP_Text) ← Assign
│  ├─ Coin Icon (Image) ← Optional
│  ├─ Use Animation: true
│  └─ Animation Duration: 0.5s
```

### AnimalShopButton Component
```
Animal Button GameObject
├─ Button (Component)
├─ AnimalShopButton.cs
│  ├─ Animal Type: Kangaroo/Elephant/Lion/Bear
│  ├─ Main Button ← Assign
│  ├─ Name Text (TMP) ← Assign
│  ├─ Price Text (TMP) ← Assign
│  ├─ Locked State (GameObject) ← Assign
│  ├─ Unlocked State (GameObject) ← Assign
│  └─ Selected Highlight (GameObject) ← Assign
```

### AnimalSelector Component (Updated)
```
AnimalSelector GameObject
├─ SimpleScrollSnap (Component)
├─ AnimalSelector.cs
│  ├─ Simple Scroll Snap ← Assign
│  ├─ Select Button ← Assign
│  ├─ Animal Name Text (TMP) ← NEW
│  ├─ Status Text (TMP) ← NEW
│  ├─ Price Text (TMP) ← NEW
│  └─ Price Display (GameObject) ← NEW
```

---

## 🔧 Extension Methods Available

```csharp
// From AnimalData.cs - AnimalTypeExtensions

// Get display name
string name = AnimalType.Elephant.GetDisplayName(); 
// → "Elephant"

// Get default price
int price = AnimalType.Lion.GetDefaultPrice(); 
// → 1000

// Get description
string desc = AnimalType.Bear.GetDescription(); 
// → "Powerful and brave."

// Convert panel index to AnimalType
AnimalType type = AnimalTypeExtensions.FromPanelIndex(2); 
// → AnimalType.Elephant

// Convert AnimalType to panel index
int index = AnimalType.Kangaroo.ToPanelIndex(); 
// → 1
```

---

## 🎯 Default Configuration

| Animal | Panel Index | Enum Value | Price | Default Status |
|--------|-------------|------------|-------|----------------|
| 🦘 Kangaroo | 1 | 0 | 0 coins (FREE) | ✅ Unlocked |
| 🐘 Elephant | 2 | 1 | 500 coins | 🔒 Locked |
| 🦁 Lion | 3 | 2 | 1000 coins | 🔒 Locked |
| 🐻 Bear | 4 | 3 | 1500 coins | 🔒 Locked |

---

## ⚡ Performance Notes

- **Singleton Pattern**: Both managers persist across scenes (DontDestroyOnLoad)
- **Events**: All UI components auto-update via events, no polling needed
- **PlayerPrefs**: Saved immediately after changes
- **UI Animation**: Uses LeanTween for smooth coin display animations

---

## 🐛 Common Issues & Solutions

### Issue: Kangaroo not unlocked by default
**Solution:** Check AnimalOwnershipManager.Awake() calls InitializeDefaultUnlocks()

### Issue: Coin not persisting after restart
**Solution:** Check CurrencyManager is in scene and DontDestroyOnLoad works

### Issue: Cannot buy animal
**Solution:** 
1. Check if enough coins
2. Check if already unlocked
3. Check CurrencyManager & AnimalOwnershipManager are in scene

### Issue: UI not updating
**Solution:** 
1. Check UI component subscribes to events in OnEnable()
2. Check unsubscribes in OnDisable()
3. Verify manager instances exist

---

**For implementation details, see `CURRENCY_SYSTEM_README.md`**
