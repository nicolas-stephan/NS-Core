# Core

A foundational utility library providing essential building blocks for Unity projects. This package includes singletons, save system helpers, editor utilities, and common tools used across NS packages.

## Features

- **Singleton Patterns**: Multiple singleton implementations for MonoBehaviours and ScriptableObjects
- **Save System**: Type-safe PlayerPrefs wrapper with automatic persistence
- **Editor Utilities**: Visual Element extensions and property drawers for enhanced editor workflows
- **Code Generation**: Utilities for programmatic code writing in the editor
- **Attributes**: Custom property attributes like `[ReadOnly]` for inspector customization

## Core Utilities

### Singleton Patterns

Multiple singleton implementations to fit different use cases:

```csharp
// Persistent singleton (survives scene loads, auto-creates on access)
public class GameManager : PersistentSingleton<GameManager> { }

// Persistent singleton (survives scene loads, no auto-creation)
public class AudioManager : PersistentSingletonNoAutoInit<AudioManager> { }

// Scene-based singleton (destroyed on scene load)
public class LevelController : Singleton<LevelController> { }

// ScriptableObject singleton (Addressables-based)
public class GameConfig : ScriptableObjectSingleton<GameConfig> { }
```

**Key Features:**
- Automatic instance management and lifecycle handling
- Built-in duplicate prevention with warnings
- ScriptableObject singletons use Addressables for async/sync loading
- Editor-time asset creation and Addressables setup

### Save System

Type-safe wrappers around Unity's PlayerPrefs with automatic serialization:

```csharp
// Define saved variables
private SavedInt highScore = new("HighScore", 0);
private SavedBool soundEnabled = new("SoundEnabled", true);
private SavedString playerName = new("PlayerName", "Player");

// Use like regular variables
highScore.Value = 1000;
bool isSoundOn = soundEnabled.Value;

// Listen to changes
highScore.OnChanged += (newValue) => Debug.Log($"New high score: {newValue}");

// Delete saved data
highScore.Delete();
```

**Available Types:**
- `SavedInt`, `SavedFloat`, `SavedBool`, `SavedString`
- `SavedEnum<T>` for enum types
- `SavedColor` for Color values
- `SavedClass<T>` and `SavedStruct<T>` for JSON serialization

**Key Features:**
- Automatic persistence on value change
- Change event callbacks
- Virtual player ID support for multiplayer testing in editor
- JSON serialization for complex types

### Editor Utilities

#### EditorVisualElementExtensions

Utilities for loading UI Toolkit assets by name:

```csharp
// Load stylesheet by name
visualElement.LoadStylesheetFromName("MyStyleSheet");

// Build UI from VisualTreeAsset
EditorVisualElementExtensions.BuildFromName(out VisualElement ui, "MyUIDocument");

// Load any asset by type and name
var texture = EditorVisualElementExtensions.LoadAssetFromName<Texture2D>("Texture2D", "Icon");
```

#### SerializedPropertyExtensions

Extension methods for working with SerializedProperty in custom editors.

#### ReadOnly Attribute

Mark fields as read-only in the Inspector:

```csharp
[SerializeField, ReadOnly] private int calculatedValue;
```

### Code Generation

The `CodeWriter` utility provides a fluent API for generating C# code programmatically:

```csharp
var writer = new CodeWriter();
writer.WriteUsing("UnityEngine")
      .WriteNamespace("MyGame")
      .BeginClass("MyClass", "MonoBehaviour")
      .WriteLine("// Generated code")
      .EndClass();
```

### Assembly Utilities

Helper methods for working with assemblies and types:

```csharp
using NS.Core.Utils;

// Find types by attribute, interface, or base class
var allTypes = AssemblyUtils.GetAllTypesWithAttribute<MyAttribute>();
```

## Installation

### Install via Git URL

Add the following to your `manifest.json` in the `Packages` folder:

```json
{
  "dependencies": {
    "com.ns.core": "https://github.com/nicolas-stephan/Core.git?path=/Assets/Core"
  }
}
```

Or use Unity's Package Manager:
1. Open **Window > Package Manager**
2. Click the **+** button
3. Select **Add package from git URL**
4. Enter: `https://github.com/nicolas-stephan/Core.git?path=/Assets/Core`

### Dependencies

This package requires:
- **Unity 6000.2.13f1** or later
- **UniTask** (com.cysharp.unitask 2.5.10)
- **Addressables** (com.unity.addressables 2.7.4)

These dependencies are automatically resolved when installing the package.

## Usage Example

```csharp
using NS.Core;
using NS.Core.SavedVariables;
using NS.Core.Utils;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager> {
    private SavedInt score = new("PlayerScore", 0);
    private SavedBool firstLaunch = new("FirstLaunch", true);

    protected override void Awake() {
        base.Awake();

        if (firstLaunch.Value) {
            Debug.Log("Welcome to the game!");
            firstLaunch.Value = false;
        }

        score.OnChanged += OnScoreChanged;
    }

    public void AddScore(int points) {
        score.Value += points;
    }

    private void OnScoreChanged(int newScore) {
        Debug.Log($"Score updated: {newScore}");
    }
}
```

## CI/CD Pipeline Setup – Required Secrets

This project uses GameCI (https://game.ci) in GitHub Actions.
To allow Unity to activate and run tests in CI, you must configure the following GitHub Secrets.

Go to GitHub → Repository → Settings → Secrets and variables → Actions → New repository secret

1. UNITY_EMAIL
2. UNITY_PASSWORD
3. UNITY_LICENSE (https://game.ci/docs/github/activation)

## License

MIT License

Copyright (c) 2026 Nicolas Stephan

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


## Contributing

Contributions are welcome! Please submit issues and pull requests to the [GitHub repository](https://github.com/nicolas-stephan/Core).
