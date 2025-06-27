# Generate Unity Meta Files

## Quick Steps

1. **Open Unity Hub** and create a new 3D project (Unity 2020.3+)

2. **Copy this SDK** into your Unity project:
   - Option A: Copy the entire `decisionbox-unity-sdk` folder to `Assets/` or `Packages/`
   - Option B: In Unity, go to Window > Package Manager > + > Add package from disk > Select package.json

3. **Unity will automatically generate all .meta files**

4. **Copy meta files back** to this repository:
   ```bash
   # From your Unity project directory
   cd Assets/decisionbox-unity-sdk  # or Packages/decisionbox-unity-sdk
   
   # Copy all meta files back to the SDK repository
   find . -name "*.meta" -exec cp --parents {} /path/to/decisionbox-unity-sdk/ \;
   ```

5. **Verify and commit**:
   ```bash
   # In the SDK repository
   git add "*.meta"
   git status  # Should show all new .meta files
   git commit -m "Add Unity meta files for package importing"
   git push
   ```

## Alternative: Using Unity in batch mode

```bash
# Create meta files using Unity CLI (if Unity is in PATH)
Unity -batchmode -quit -projectPath /tmp/TempUnityProject -importPackage /path/to/decisionbox-unity-sdk
```

## What files need meta files?

- ✅ Every `.cs` file
- ✅ Every `.asmdef` file  
- ✅ Every folder
- ✅ `package.json`
- ✅ `README.md`
- ✅ `LICENSE.md`
- ✅ `CHANGELOG.md`
- ❌ `.gitignore` (Unity ignores dotfiles)
- ❌ `.meta` files themselves

Total files needing meta files: ~50+