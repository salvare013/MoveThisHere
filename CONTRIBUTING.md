# 贡献指南

感谢你对 MoveThisHere 感兴趣！本指南帮助你快速上手开发和提交改动。

本项目最初基于 [DoctorFeelGoodMD/OxygenNotIncluded-Mods](https://github.com/DoctorFeelGoodMD/OxygenNotIncluded-Mods)（MIT 许可证）修改而来，感谢原作者的开源工作。

## 开发环境

- **操作系统**：Windows（因为游戏和引用路径默认按 Windows 目录结构）
- **.NET Framework**：4.8
- **IDE**：Visual Studio 或 Visual Studio Code + C# Dev Kit
- **游戏**：拥有《缺氧》(Oxygen Not Included) 本体

## 构建项目

```bash
dotnet build MoveThisHere.csproj -c Release
```

### 关于游戏 DLL 引用

`MoveThisHere.csproj` 中硬编码了游戏 DLL 路径：

```xml
D:\SteamLibrary\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\...
```

如果你的游戏安装在其他位置，请修改 `.csproj` 中的 `HintPath`。

需要引用的 DLL：
- `0Harmony.dll`
- `Assembly-CSharp.dll`
- `Assembly-CSharp-firstpass.dll`
- `UnityEngine.dll`
- `UnityEngine.CoreModule.dll`

## 测试方式

本项目没有单元测试，所有验证通过在游戏中加载模组完成：

1. 编译生成 `bin/Release/MoveThisHere.dll`
2. 将 DLL 连同 `anim/`、`locales/` 等资源放入 `OxygenNotIncluded/mods/Local/MoveThisHere/`
3. 在游戏模组菜单中启用并进入存档
4. 放置搬运点，测试容量滑块、右键菜单、拆除、存档读档等行为

## 本地化工作流程

新增或修改 UI 文本时：

1. 在 `STRINGS.cs` 中添加/修改 `LocString` 常量
2. 在 `locales/en.po` 和 `locales/zh.po` 中添加对应的 `msgctxt`/`msgid`/`msgstr` 条目
3. 确保 `msgid` 与 `STRINGS.cs` 中的源字符串**完全一致**（包括标点和空格），否则翻译不会生效
4. 编译后在游戏中切换语言测试

## 提交信息

沿用仓库现有风格即可，例如：

- `feat: 新增 xxx 功能`
- `fix: 修复 xxx 问题`
- `docs: 更新 README`
- `refactor: 重构 xxx`

中英混合均可。

## 提交 PR 前请检查

- `dotnet build MoveThisHere.csproj -c Release` 无错误、无警告
- 新 UI 字符串已同步到 `en.po` 和 `zh.po`
- 已在游戏中实际测试

## 行为准则

保持友善、尊重不同语言背景的贡献者。
