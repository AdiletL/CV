# Changelog

## [0.11.1] 2024-01-15

### Added
* **Context Menu**
  * Added general MonoBehaviour context menu to open specific Components from GameObjects via Inspector Graph

### Changed
* **Graph**
  * Changed Graph logic when inspecting MonoBehaviours to only show visible references

## [0.11.0] 2023-11-05

### Added
* **Graph**
  * Displaying a reference counter next to the connection lines when multiple references to the same object exist
* **Settings**
  * Added option to disable or enable all Filters at once
  * Added option to show or hide the reference counter in the graph
* **Inspector Windows**
  * Added Asset icon next to the name of the asset in the title bar
* **Main Window**
  * Added "Displaying" label to indicate the logic being used that construct the graph

### Changed
* **Graph**
  * Adjusted connection lines position to make it easier to identify them
* **Inspector Windows**
  * Modified quick stats to be more explicit
  * Modified quick stats tooltip to have more details
  * Changed where the project settings are saved, from `Assets` to `ProjectSettings/GiantParticle`
* **Settings**
  * Added more details to settings

### Fixed
* Fixed issues when updating the graph either manually or automatically when changing references

## [0.10.1-b] 2023-04-16

### Fixed
* Fixed issue where trackpads cannot move the graph due to the lack of middle button. Now there are two (2) options to move graph that follow [shortcut conventions](https://docs.unity3d.com/Manual/SceneViewNavigation.html):
    * Using mouse middle button
    * [macOS] Using combination of mouse left button + Option Key + Command key
    * [Windows | Linux] Using combination of mouse left button + Alt Key + Control key
* Removed attribute to create UI Document Catalogs from general public. This is only for internal use.

## [0.10.0-b] 2023-04-08

### Added
* **Settings**
    * Added option to change reference arrows color in settings
    * Added Settings for default inspector window size based on view mode
    * Made settings panel scrollable

### Changed
* Excluded references to same asset from the visualization (Example: `Texture2D` references from `Sprite`)
* **Project**
    * Organized files into a more understandable structure
    * Renamed namespaces for consistency
    * Split `UIDocumentCatalog` into separate catalogs based on context:
        * `InspectorWindowUIDocumentCatalog` for Floating Inspector Window related UI
        * `MainWindowUIDocumentCatalog` for Main Inspector Graph Window related UI
        * `SettingsUIDocumentCatalog` for Settings related UI
        * Refactored InspectorGraph window toolbar into own class `InspectorGraphToolbar`
* **UI**
    * Changed way to move around the graph from using scrollbars to drag with middle button

### Fixed
* Changed saved last inspected object by path to by GUID
* Added Workaround to properly display Animation Previews
* Fixed missing connection lines when toggling `view > expand` on filters
* Optimized rendering by hiding Inspector windows that are not visible within the viewport

## [0.9.0-b] 2023-03-01

### Added
- Project settings section for Inspector Graph
  - Max Inspector Window Limit
  - Max Preview Mode Limit
  - Filters by type
- Consolidated UI Toolkit UIDocuments in a single catalog file

### Changed
- Reorganized toolbar menu
  - Moved Reset button from toolbar to View menu
  - Added Filters submenu
  - Added Edit menu to open Project Settings
  - Added Help menu with useful links
- Reorganized UXML and USS files under Assets subfolder

### Fixed
- Fixed performance issue when displaying assets like SpriteAtlas that can reference multiple assets ([Issue #2](https://github.com/giantparticlegames/InspectorGraph/issues/2))
- Fixed issue where the connection lines between windows were re-added every time a refresh was called
- Fixed Unity warning about UI Toolkit referenced assets being moved due to absolute path declaration

### Removed
- Removed hard-coded `Show > Scripts` and `Show > Prefab References` menu items (Replaced with Filters)



## [0.8.0-b] 2022-12-17

### Added
- Initial Beta implementation of the tool
