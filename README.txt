# TDViewer
`com.square_enix.android_googleplay.PTD`の3Dモデルを読み込みUnity Editorに表示します。
現在ソースコード準備中。
必要なもの：

* Unity 2019.4以上(アプリとバージョンを揃えたUnity 2018のほうが良いのかもしれない)
* ゲームのデータ(`/sdcard/Android/data/com.square_enix.android_googleplay.PTD/files/prim`)
* Newtonsoft Json.NET(同梱 13.0.1 .NET Standard 2.0)
* [UnityChanToonShader](https://github.com/unity3d-jp/UnityChanToonShaderVer2_Project/releases/tag/legacy-2.0.8) (オプション)

詳細は[Docs/index.html](Docs/index.html)を見てください。

## 変更履歴
* v0.2.1 GitHub公開用に整理。
* v0.2 オリジナルのシェーダーに対応(OpenGLES2時のみ)。武器暫定対応。propロードのみ対応。
