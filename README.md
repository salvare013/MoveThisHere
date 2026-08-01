# MoveThisHere - 搬运点

**《缺氧》(Oxygen Not Included)** 模组，添加一个名为"搬运点 (Hauling Point)"的 1×1 建筑，专门用于集中搬运**气体和液体**。

> **English**: A small *Oxygen Not Included* mod that adds a 1×1 "Hauling Point" building for relocating liquids and gases to a specific spot.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](./LICENSE)

- [更新日志](./CHANGELOG.md)
- [贡献指南](./CONTRIBUTING.md)
- [许可证](./LICENSE)

---

## 功能

### 📦 只搬运气体和液体

复制人会主动将液体和气体（瓶装）运送到搬运点，方便集中处理。

### ⚙️ 右键菜单选项

| 选项 | 说明 |
|------|------|
| **🔄 自动装瓶** | 控制复制人是否将液体/气体装瓶后运送到搬运点 |
| **🗑️ 自动清空** | 存储量 ≥99% 时自动拆除搬运点，清空所有物品 |
| **💧 自动倾倒** | 拆除时液体/气体是直接倾倒到世界中，还是以瓶装形式掉落 |
| **⚖️ kg / g 切换** | 切换容量滑块单位为千克（精确到 0.01 kg）或克（整数） |

### 🎛️ 容量自定义滑块

- 每个搬运点可独立设置容量
- 默认最大容量 **20,000 kg（20吨）**
- 超过 100 kg 时自动取整
- 支持 **g / kg 单位一键切换**（右键菜单），千克模式下显示 2 位小数，克模式下为整数

### 🏗️ 建筑属性

| 属性 | 值 |
|------|-----|
| 大小 | 1×1 |
| 放置 | 任意位置，**可建在其他建筑之上** |
| 建造消耗 | **无**（强制使用钻石材质但不消耗资源） |
| 耐久 | 无敌、不可洪水、不可过热、不可修复、不可消毒 |
| 装饰度 | -10 |
| 图层 | `Canvases`（渲染在普通建筑前面，重叠时优先被点击） |

### 🔧 拆除

- 点击"拆除"按钮**立即拆除**（无需复制人执行）
- 不掉落任何建筑材料
- 根据自动倾倒设置处理内部液体/气体

---

## 安装

1. 下载最新 Release 中的 `MoveThisHere.dll`
2. 放入 `OxygenNotIncluded/mods/Local/` 目录
3. 在游戏中启用模组

## 编译

使用 Visual Studio 或 `dotnet build` 编译：

```
dotnet build MoveThisHere.csproj -c Release
```

需要引用游戏目录下的 `Assembly-CSharp.dll` 和 `0Harmony.dll`。

## 本地化

支持多语言：
- `locales/en.po` - 英文
- `locales/zh.po` - 中文

新增或修改 UI 文本时，请同时更新 `STRINGS.cs`、`locales/en.po` 和 `locales/zh.po`。详见[贡献指南](./CONTRIBUTING.md)。

## 贡献

欢迎提交 issue 和 PR。请先阅读[贡献指南](./CONTRIBUTING.md)。

## 许可证

本项目使用 [MIT 许可证](./LICENSE)。
