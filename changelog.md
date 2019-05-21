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
- Package dependencies:
    - `com.ugf.code.analysis`: from `1.0.0` to `2.0.0`.

### Deprecated
- Nothing.

### Removed
- Nothing.

### Fixed
- Nothing.

### Security
- Nothing.

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
