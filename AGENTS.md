# 仓库指南

## 项目结构与模块组织
- `Assets/` 包含所有 Unity 内容。自定义编辑器工具在 `Assets/SkillEditor/Editor/Track/Scripts`，主场景是 `Assets/SkillEditor/SkillEditorScene.unity`。
- `Packages/manifest.json` 定义 Unity 包依赖。
- `ProjectSettings/` 保存 Unity 项目配置，包括所需的编辑器版本。
- `Library/`、`Temp/`、`Logs/` 和 `obj/` 由 Unity 或构建生成；不要编辑或提交它们。

## 构建、测试与开发命令
- 使用 `ProjectSettings/ProjectVersion.txt` 中的版本在 Unity Hub 打开项目（2021.3.17f1c1）。
- 本地运行：打开项目并点击编辑器中的 Play 按钮。
- 构建播放器：在 Unity 中 `File > Build Settings...`，选择目标后点击 Build。
- 以 batchmode 运行测试（按需调整 `Unity.exe` 路径）：
  `"C:\Program Files\Unity\Hub\Editor\2021.3.17f1c1\Editor\Unity.exe" -batchmode -projectPath . -runTests -testPlatform EditMode -logFile Logs/editmode.log -quit`

## 编码风格与命名规范
- C# 使用 4 空格缩进；类/方法用 PascalCase，局部变量/字段用 camelCase（见 `Assets/SkillEditor/Editor/Track/Scripts`）。
- 每个文件只保留一个 public 类型，文件名与类名一致。
- 编辑器工具使用 UIElements；编辑器代码放在 `Assets/SkillEditor/Editor/` 下。
- 保留 Unity `.meta` 文件；通过 Unity 编辑器重命名/移动资源以保持 GUID 稳定。

## 测试指南
- 使用 Unity Test Runner（`Window > General > Test Runner`）运行 EditMode 与 PlayMode 测试。
- 新测试放在 `Assets/Tests/EditMode` 或 `Assets/Tests/PlayMode`，文件命名为 `*Tests.cs`。
- 优先使用 NUnit 断言与 `UnityTest` 进行协程相关检查。

## 提交与 PR 指南
- 该仓库没有 Git 历史，因此不强制提交规范。如需提交，请使用清晰的祈使句摘要（例如 "Add skill track toolbar"）。
- PR 应包含简要描述、测试结果（或跳过测试的原因），以及编辑器 UI 变更的截图/GIF。

## 配置提示
- 未经团队同意不要升级 Unity 版本；期望版本记录在 `ProjectSettings/ProjectVersion.txt`。
