# Const Dropdown Attribute

A Unity Editor extension that provides **searchable dropdowns** for constant values in Inspector.  
Perfect for large sets of `const string` or `enum-like` constants spread across classes.

---

## 🚀 Features
- Show `const` or `static readonly` fields from a given class in Inspector.
- Built-in **search box** with magnifying glass icon.
- Keeps Unity-style **label + field layout**.
- Supports **conditional switching** between two classes:
  - Based on a `bool` **field**
  - Based on a `bool` **property**
  - Based on a `bool` **method**
- Safe fallback when no condition is met.
- Easy to extend for more advanced logic (enums, multiple classes).

---

## 📦 Installation
1. Copy the url: https://github.com/VuongThanhAn/Const-Dropdown-Attribute.git paste in the Package Manager url
2. Done. The attribute is now ready to use.

---

## 🛠 Usage

### 1. Basic usage
```csharp
public static class ConstClassA
{
    public const string A1 = "Value A1";
    public const string A2 = "Value A2";
}

public class ExampleMono : MonoBehaviour
{
    [ConstDropValue(typeof(ConstClassA))]
    public string myValue;
}
```

### 2. Conditional switching (method / property / field)
```csharp
public static class ConstClassA
{
    public const string A1 = "Value A1";
    public const string A2 = "Value A2";
}

public static class ConstClassB
{
    public const string B1 = "Value B1";
    public const string B2 = "Value B2";
}

public class ExampleMono : MonoBehaviour
{
    [SerializeField] private bool useClassB; // 👈 simple boolean field

    // Switch between ConstClassA and ConstClassB based on useClassB
    [ConstDropValue(typeof(ConstClassA), typeof(ConstClassB), nameof(useClassB))]
    public string myValue;
}
```

### 3. Using a method condition
```csharp
public class ExampleMono : MonoBehaviour
{
    [SerializeField] private int gameMode;

    private bool CheckIfHardMode()
    {
        return gameMode == 2;
    }

    [ConstDropValue(typeof(ConstClassEasy), typeof(ConstClassHard), nameof(CheckIfHardMode))]
    public string myValue;
}
```
