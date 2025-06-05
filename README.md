# Open Tag Custom Map Guide

This repository shows you how to create and export custom maps for **Open Tag** using the [[OpenTag-MapEditor](https://github.com/grayson-is-cool-i-guess/OpenTag-MapEditor)](https://github.com/grayson-is-cool-i-guess/OpenTag-MapEditor) Unity project and the provided `OpenExporter` scripts.

---

## Table of Contents

1. [[Prerequisites](#prerequisites)]
2. [[Getting Started](#getting-started)]
3. [[Creating Your Map](#creating-your-map)]
4. [[Exporting Your Map](#exporting-your-map)]
5. [[Optimization Levels](#optimization-levels)]
7. [[Troubleshooting](#troubleshooting)]
8. [[Contributing & License](#contributing--license)]

---

## Prerequisites

* **Unity** (2021.3.21f1)
* Basic familiarity with Unity scenes, GameObjects, and the Editor
* Clone (or fork) the Map Editor repo:

  ```bash
  git clone https://github.com/grayson-is-cool-i-guess/OpenTag-MapEditor.git
  cd OpenTag-MapEditor
  ```

---

## Getting Started

1. **Open the project**
   Launch Unity and open the `OpenTag-MapEditor` folder.

2. **Open the example scene**
   In **Assets/Scenes/** you’ll find:

   * `ExampleMap.unity`

   And open the ExampleMap scene.
---

## Creating Your Map

1. **Build your scene**

   * Put ALL your map objects under the "CustomMap" GameObject with the "OpenExporter" script on it.
   * BE CAREFUL! Do NOT move the CustomMap object, but move the objects under it. All objects that are not a child of the CustomMap object will NOT be included in your map.

3. **Configure export settings**
   In the **OpenExporter** component, set:

   * **Map Name**: A name for your map, that will be shown on the computer.
   * **Optimization Level**: choose from `NONE`, `LOW`, `MEDIUM`, or `HIGH`

---

## Exporting Your Map

1. **Open the custom editor**
   With your root still selected, you’ll see the **Export Project** button at the bottom of the Inspector.

2. **Click Export Project**
   This will:

   * Clone your map into a temporary `_export` copy.
   * Make ALL your objects Read/Write, so you don't have to do it yourself.
   * Apply the optimization.
   * Bundle it together into a ".jzlib" file.

3. **Locate your `.jzlib`**

   ```txt
   Assets/ExportedMaps/(Map Name).jzlib
   ```

---

## Optimization Levels

* **NONE**
  Doesn't clean anything, keeps it as is.
* **LOW**

  * Removes any missing-script components.
  * Deletes GameObjects with no components (aside from Transform) and no children.

* **MEDIUM**

  * All LOW optimizations.
  * Deletes any disabled MonoBehaviour components.

* **HIGH**

  * All MEDIUM optimizations.
  * Flattens single-child GameObject chains to reduce depth.

Choose higher levels to reduce filesize and load-time overhead, at the cost of removing unused or “empty” objects.

---

## Integrating into Open Tag

1. Submit your map
  Go into the [Discord](https://discord.gg/DVcxcPGq), and create a new post in "SUBMIT-MAPS" with the title of your map. READ THE RULES BEFORE POSTING!!! Then, wait until an administrator accepts the submission. After the first submission, ping Grayson (BandlabBandit) every time you make an update that you want to publish.

---

## Troubleshooting

* **“bundle build failed”**

  * Ensure you have a valid target platform selected under **Build Settings** → **Standalone**.
  * Check the Editor log for detailed error messages.

* **Meshes still not readable**

  * Manually select your model in the Project window.
  * In Model Import Settings, tick **Read/Write Enabled** and click **Apply**.

* **Editor button not showing**

  * Confirm `OpenExporterEditor.cs` resides in an `/Editor/` folder.
  * Reimport scripts or restart Unity.

---

## Contributing & License

Contributions welcome! Feel free to submit issues or pull requests to improve the map exporter, add features, or update documentation.






eagler, eagler, eaglerrrrr...