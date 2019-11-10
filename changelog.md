# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased - 2019-01-01
- [Commits](https://github.com/unity-game-framework/ugf-code-generate/compare/0.0.0...0.0.0)
- [Milestone](https://github.com/unity-game-framework/ugf-code-generate/milestone/0?closed=1)

### Added
- Nothing.

### Changed
- Nothing.

### Deprecated
- Nothing.

### Removed
- Nothing.

### Fixed
- Nothing.

### Security
- Nothing.

## 4.0.0-preview - 2019-11-10
- [Commits](https://github.com/unity-game-framework/ugf-code-generate/compare/3.1.0...4.0.0-preview)
- [Milestone](https://github.com/unity-game-framework/ugf-code-generate/milestone/10?closed=1)

### Added
- Package dependencies:
    - `com.ugf.editortools`: `0.2.0-preview`.
- `CodeGenerateContainerField.HasInitializer` to determines whether field has initializer.

### Changed
- Update to Unity 2019.3.

### Deprecated
- `CodeGenerateContainerEditorUtility`: `GetFields`, `GetProperties`, `IsValidField` and `IsValidProperty` has been deprecated, use validation instead.

### Removed
- Package dependencies:
    - `com.ugf.types`: `2.2.0`.

## 3.1.0 - 2019-09-14
- [Commits](https://github.com/unity-game-framework/ugf-code-generate/compare/3.0.3...3.1.0)
- [Milestone](https://github.com/unity-game-framework/ugf-code-generate/milestone/9?closed=1)

### Added
- `TryConstructTypeSymbol` support for constructing type symbol from array type.
- `TryConstructArrayTypeSymbol` to construct type symbol from the array element type and rank.

### Deprecated
- `TryConstructTypeSymbol` overload with `INamedTypeSymbol` as result.

## 3.0.3 - 2019-09-14
- [Commits](https://github.com/unity-game-framework/ugf-code-generate/compare/3.0.2...3.0.3)
- [Milestone](https://github.com/unity-game-framework/ugf-code-generate/milestone/8?closed=1)

### Changed
- `CodeGenerateContainerExternalAssetImporter` to use `EditorJsonUtility` for serializing info as Json.

## 3.0.2 - 2019-08-18
- [Commits](https://github.com/unity-game-framework/ugf-code-generate/compare/3.0.1...3.0.2)
- [Milestone](https://github.com/unity-game-framework/ugf-code-generate/milestone/7?closed=1)

### Fixed
- `CodeGenerateContainerExternalInfoBase.TryGetTargetType`: throws exception when type name not specified.

## 3.0.1 - 2019-08-07
- [Commits](https://github.com/unity-game-framework/ugf-code-generate/compare/3.0.0...3.0.1)
- [Milestone](https://github.com/unity-game-framework/ugf-code-generate/milestone/6?closed=1)

### Fixed
- `CodeGenerateContainerExternalAssetImporterEditor` incorrectly applies changes.

## 3.0.0 - 2019-08-01
- [Commits](https://github.com/unity-game-framework/ugf-code-generate/compare/2.1.0...3.0.0)
- [Milestone](https://github.com/unity-game-framework/ugf-code-generate/milestone/5?closed=1)

### Added
- Package short description.

### Changed
- Update to Unity 2019.2.
- Package dependencies:
    - `com.ugf.code.analysis`: from `2.0.0` to `3.0.0`.

## 2.1.0 - 2019-01-01
- [Commits](https://github.com/unity-game-framework/ugf-code-generate/compare/2.0.0...2.1.0)
- [Milestone](https://github.com/unity-game-framework/ugf-code-generate/milestone/4?closed=1)

### Added
- `ConstructTypeSymbol` method that throws exception if the symbol can not be constructed. (#10)

## 2.0.0 - 2019-05-21
- [Commits](https://github.com/unity-game-framework/ugf-code-generate/compare/1.1.0...2.0.0)
- [Milestone](https://github.com/unity-game-framework/ugf-code-generate/milestone/3?closed=1)

### Changed
- Package dependencies:
    - `com.ugf.code.analysis`: from `1.0.0` to `2.0.0`.

## 1.1.0 - 2019-05-20
- [Commits](https://github.com/unity-game-framework/ugf-code-generate/compare/1.0.0...1.1.0)
- [Milestone](https://github.com/unity-game-framework/ugf-code-generate/milestone/2?closed=1)

### Added
- Package dependencies:
    - `com.ugf.types`: `2.2.0`.
- Container External to generate container from external type. (#3)
- `CodeGenerateContainerValidation` to determine what types can be used to generate containers.
- `CodeGenerateContainerEditorUtility.CreateUnit` and `CodeGenerateContainerEditorUtility.Create` overloads with validation.
- `TryGetAnyTypeByMetadataName` to get any found type symbol by the metadata name. (Not support generics)
- `TryConstructTypeSymbol` to construct type symbol from the specified type. (Support generics) (#5)
- `TryConstructGenericTypeSymbol` to construct type symbol from the specified generic definition and arguments.
- `TryGetGenericNameSyntax` to get generic name syntax from the specified type syntax, if possible.
- `CodeGenerateContainerExternalEditorUtility.CreateUnit` to create compilation unit from the specified container external info.
- `ICodeGenerateContainerExternalInfo.TryGetMember<T>` to get member info with cast to the specified type.
- `CodeGenerateEditorUtility.GetPathForGeneratedScript` to get path for scripts that contains generated code.

### Deprecated
- `CodeGenerateContainerEditorUtility.CreateUnit` and `CodeGenerateContainerEditorUtility.Create` has been deprecated, use overloads with validation instead.
- `TryGetTypeByMetadataName` has been deprecated, use `TryGetAnyTypeByMetadataName` to get type symbol from the metadata name or `TryConstructTypeSymbol` to get type symbol from the `Type`.
- `TryGetGenericTypeByMetadataName` has been deprecated, use `TryConstructGenericTypeSymbol` instead.

## 1.0.0 - 2019-04-29
- [Commits](https://github.com/unity-game-framework/ugf-code-generate/compare/34b7eb2...1.0.0)
- [Milestone](https://github.com/unity-game-framework/ugf-code-generate/milestone/1?closed=1)

### Added
- This is a initial release.

---
> Unity Game Framework | Copyright 2019
