import Foundation

//  CChar = Int8
public typealias CCharPtr = UnsafePointer<CChar>?

// MARK: - Hello World

// ネイティブで `Hello World` を出力するサンプル
// NOTE: `@_cdecl` を用いて C の関数として公開 (パラメータには公開する際の関数名を渡す)
@_cdecl("printHelloWorld")
public func printHelloWorld() {
    // このメソッドが C# から P/Invoke 経由で呼び出される
    print("[Swift]: Hello World")
}

// MARK: - 引数と戻り値の受け渡しを行うサンプル

// 引数と戻り値の受け渡しを行うサンプル (整数)
// NOTE: C# とネイティブ側で型は合わせること (例えば Swift の Int は環境によって 32bit / 64bit が変わるので明示的にサイズを指定する)
@_cdecl("paramSampleInt")
public func paramSampleInt(_ value: Int32) -> Int32 {
    print("[Swift]: \(value)")
    return 2
}

// 引数と戻り値の受け渡しを行うサンプル (文字列)
// NOTE: `typealies` で型のエイリアスを指定
@_cdecl("paramSampleString")
public func paramSampleString(_ strPtr: CCharPtr) -> CCharPtr {
    
    // CCharPtr を String に変換
    if let strPtr = strPtr {
        let message = String(cString: strPtr)
        print("[Swift]: \(message)")
    }
    
    // String を CCharPtr に変換
    let message = "Swift Message"   // 戻り値として送りたい文字列
    
    // UnsafePointer<CChar> に変換し、戻り値分のメモリを確保してから strcpy でコピーして返す
    // NOTE: 確保したメモリは マーシャリング時に自動で解放される
    let utfText: CCharPtr = (message as NSString).utf8String;
    let pointer: UnsafeMutablePointer<Int8> = UnsafeMutablePointer<Int8>.allocate(capacity: (8 * message.count) + 1);
    
    // strcpy で返るのは UnsafeMutablePointer<T> であり、CCharPtr と型が異なるので変換して返す
    return UnsafePointer(strcpy(pointer, utfText))
}

// MARK: - インスタンスメソッドの呼び出し

// 今回のサンプルでインスタンス化するクラス
class SampleClass {
    func SampleMethod() {
        print("[Swift]: Call SampleMethod")
    }
}

// インスタンスの生成
@_cdecl("createInstance")
public func createInstance() -> UnsafeMutableRawPointer? {
    // インスタンス化
    let instance = SampleClass()
    
    // `Unmanaged` を利用することで参照型を ARC (自動参照カウント) の管理下から外すことが可能
    // NOTE: 以下の処理は自前で参照カウンタをインクリメント(retain)しつつ、インスタンスに対応する `Unmanaged` 型を取得している
    let unmanaged = Unmanaged<SampleClass>.passRetained(instance)
    
    // `Unmanaged.toOpaque` でインスタンスのポインタを取得して C# に返す
    return unmanaged.toOpaque()
}

// インスタンスメソッドの呼び出し
@_cdecl("callInstanceMethod")
public func callInstanceMethod(_ instancePtr: UnsafeRawPointer) {
    // C# から渡ってきたポインタは `Unmanaged<T>.fromOpaque` に渡すことで `Unmanaged` 型に変換可能
    // 変換した `Unmanaged` 型からは `takeUnretainedValue` を呼び出すことで参照カウンタをインクリメントせずにインスタンスを取得することが出来る
    let instance = Unmanaged<SampleClass>.fromOpaque(instancePtr).takeUnretainedValue()
    
    // インスタンスメソッドを呼び出す
    instance.SampleMethod()
}

// インスタンスの解放
@_cdecl("releaseInstance")
public func releaseInstance(_ instancePtr: UnsafeRawPointer) {
    // `createInstance()` 時にインクリメント(retain)した参照カウンタをデクリメント(release)することで解放
    let unmanaged = Unmanaged<SampleClass>.fromOpaque(instancePtr)
    unmanaged.release()
}

// MARK: - コールバックの呼び出し

// C# 側にある `SampleCallback` とフォーマットを合わせておく
public typealias SampleCallback = @convention(c) (CCharPtr) -> Void

// C# から渡されたコールバックを呼び出す
// NOTE: C# からは `OnCallback` と言うローカルメソッドが関数ポインタとして渡ってくる
@_cdecl("callbackSample")
func callbackSample(_ sampleCallback: SampleCallback) {
    // String を CCharPtr に変換
    let message = "Swift Callback Message"   // 戻り値として送りたい文字列
    
    // 戻り値として文字列を返すケースと違い、コールバックで渡す際には Mono マーシャリングが自動で解放してくれるとかは無いので、メモリは確保せずに変換した値をそのまま渡している
    let utfText: CCharPtr = (message as NSString).utf8String;
    
    // C# のメソッドを呼び出す
    // NOTE: ここでは直接呼び出しているが、例えば変数などに保持することで非同期的に呼び出すことも可能
    sampleCallback(utfText)
}
