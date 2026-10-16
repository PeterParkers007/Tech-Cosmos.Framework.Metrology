# 度量衡

> **包名**：`com.techcosmos.metrology`  
> **版本**：**1.0.0**  
> **Unity**：2022.3 LTS 或更高

把编辑器里的长度换成项目自己的单位。一个项目可以同时有多种单位，比如「码」和「体型」。每种单位是一个类，加上一份只属于它的比例资源。

引擎里的坐标保持原样。这个包不保存位置，也不认识技能、单位或界面。

## 比例

一份比例是两个数：

- **编辑器长度**：引擎里的长度
- **定义单位**：这种单位下的长度

两边都要大于 0。`1` 和 `100` 的意思是：1 个编辑器长度 = 100 个这种单位。

```
定义单位 = 编辑器长度 × (定义单位 / 编辑器长度)
编辑器长度 = 定义单位 × (编辑器长度 / 定义单位)
```

改这两个数只改资源。已经生成的类不用改，也不用为了这个数重新编译。

## 安装

在使用方项目的 `Packages/manifest.json` 里加上：

```json
{
  "dependencies": {
    "com.techcosmos.metrology": "file:../Tech-Cosmos.Framework.Metrology"
  }
}
```

`file:` 后面的路径相对项目的 `Packages` 目录，按仓库实际位置改。

仓库创建好之后也可以用 Git 地址：

```json
"com.techcosmos.metrology": "https://github.com/PeterParkers007/Tech-Cosmos.Framework.Metrology.git"
```

本包没有其它包依赖。EditMode 测试需要 Unity Test Framework。

## 新建一种单位

菜单：`Tech-Cosmos → 度量衡`。

点 **新建单位**，填写：

| 字段 | 含义 |
| --- | --- |
| 单位名 | 生成类的类名，也是运行时查找的名字。要用能做类名的名字，例如 `码`、`BodySize` |
| 编辑器长度 | 比例的引擎一侧，初始可填 `1` |
| 定义单位 | 比例的项目一侧，初始可填 `1` |

创建会在**使用方项目**里写出三样东西：

```
Assets/TechCosmos.Metrology/Units/码.asset
Assets/TechCosmos.Metrology/Generated/码.cs
Assets/Resources/TechCosmos.Metrology/MeasureCatalog.asset
```

`码.asset` 上是全局比例。`码.cs` 只负责换算，里面没有比例数字。`MeasureCatalog` 把项目里的单位资源收在一起，播放和打包时靠它加载。这些文件属于使用方项目，不要放回本包。

新建一种单位会生成一个新类，这一次需要编译。之后只改比例，不再编译。

## 主控面板

`Tech-Cosmos → 度量衡` 会扫到项目里全部单位资源。每种单位的两个数都在这里改，改的就是那份资源本身。

| 按钮 | 作用 |
| --- | --- |
| 新建单位 | 写类、写比例资源，并放进运行时目录 |
| 重新扫描 | 重新列出项目里的单位资源，不改目录 |
| 写入运行时目录 | 把当前扫到的单位写入 `MeasureCatalog` |
| 定位 | 在 Project 里高亮这份资源 |
| 生成类 | 资源还在、类文件丢了的时候补回类 |
| 删除 | 删掉这种单位的资源和类 |

比例改完，保存项目即可。播放时改在资源上的数，停下来仍然保留。

同名有多份资源时，面板会标出来。那种名字在运行时不能换算，其它名字不受影响。删到只剩一份，再点写入运行时目录。

## 代码里怎么用

```csharp
using UnityEngine;

public static class RangeExample
{
    public static float WorldRange(float rangeInYards)
    {
        return 码.ToEditor(rangeInYards);
    }

    public static float YardsBetween(Vector3 a, Vector3 b)
    {
        return 码.ToDefined(Vector3.Distance(a, b));
    }
}
```

比例是 1 个编辑器长度 = 100 码时：

- `码.ToEditor(800f)` 得到 `8`
- `码.ToDefined(8f)` 得到 `800`

第二种单位是另一个类，调用方式一样：

```csharp
float editorRadius = 体型.ToEditor(3f);
```

技能范围、界面上的距离，继续存定义单位里的数。拿去和碰撞、坐标比较时再换成编辑器长度。这样主控面板里一改比例，下一次换算就用新的数。

## 运行时怎么找到比例

生成的类调用 `MeasureUnits.Require("码")`，从 `Resources/TechCosmos.Metrology/MeasureCatalog` 取出对应资源，再读上面的两个数。

单位资源如果只存在于项目里、还没写入这份目录，播放和打包时会找不到。新建单位会自动写入。从外面复制进来的资源，打开主控面板点 **写入运行时目录**。

## 包里有什么

```
Tech-Cosmos.Framework.Metrology/
├── package.json
├── Runtime/
│   ├── MeasureScale.cs        两个方向的换算
│   ├── MeasureUnitId.cs       单位名能不能当类名
│   ├── MeasureUnitSO.cs       一种单位的比例资源
│   ├── MeasureCatalog.cs      运行时目录
│   └── MeasureUnits.cs        按名字找到比例
├── Editor/
│   ├── MetrologyWindow.cs     主控面板
│   └── MeasureUnitGenerator.cs
└── Tests/
    └── MeasureScaleTests.cs
```

## 直接换算

已经拿到两个比例数字时，可以不经过单位类：

```csharp
float defined = MeasureScale.ToDefined(editorLength, editorUnits, definedUnits);
float editor = MeasureScale.ToEditor(definedValue, editorUnits, definedUnits);
```

比例不是正数时，换算会抛 `InvalidOperationException`。传入的长度可以是负数，结果保持负号。

## 测试

`Tests/MeasureScaleTests.cs` 是 EditMode 测试，覆盖比例换算、单位名和重名。在 Unity Test Runner 里跑。
