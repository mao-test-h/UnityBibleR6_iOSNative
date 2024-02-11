# Unityプログラミング・バイブル R6冬号 「iOS 向けのネイティブ プラグイン開発入門」 サンプルプログラム

これは、株式会社ボーンデジタル社 発行・発売の「Unityバイブル R6冬号」の「iOS 向けのネイティブ プラグイン開発入門」セクションのサンプルプロジェクトです。  

## サンプルプロジェクトについて  

サンプルプロジェクトには、シーンデータとサンプルプログラムが含まれています。  
(以降、iOS 向けのネイティブプラグインを「プラグイン」と省略して記載します)  

## フォルダ／ファイルの説明

### `Assets/PluginExamples/Introduction  `

サンプルプロジェクトの「シーン」及び「UI 上からプラグインを呼び出すためのプログラム」が入っています。  

- `IntroductionExample.unity`
    - シーン
- `IntroductionExample.cs`
    - UI上からプラグインを呼び出すためのプログラム  

### `Assets/PluginExamples/Introduction/Plugins/iOS`

サンプルプロジェクトのプラグインが含まれています。

- `NativePlugin.swift`
    - Swift で実装したプラグイン  
- `NativeProxy.cs`
    - 上記「 `NativePlugin.swift` 」を C# から呼び出すためのプログラム  

## サンプルデータのライセンスについて  

`LICENSE` ファイルを御覧ください。  
