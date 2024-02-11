using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace PluginExamples.Introduction.Plugins.iOS
{
    internal interface INativeProxy
    {
        /// <summary>
        /// ネイティブで `Hello World` を出力するサンプル
        /// </summary>
        void PrintHelloWorld();


        /// <summary>
        /// 引数と戻り値の受け渡しを行うサンプル (整数)
        /// </summary>
        /// <param name="value">ネイティブに渡す値</param>
        /// <returns>ネイティブからの戻り値</returns>
        Int32 ParamSample(Int32 value);

        /// <summary>
        /// 引数と戻り値の受け渡しを行うサンプル (文字列)
        /// </summary>
        /// <param name="message">ネイティブに渡す値</param>
        /// <returns>ネイティブからの戻り値</returns>
        string ParamSample(string message);


        /// <summary>
        /// インスタンスメソッドを呼び出すサンプル: インスタンス化
        /// </summary>
        void CreateInstance();

        /// <summary>
        /// インスタンスメソッドを呼び出すサンプル: メソッドの呼び出し
        /// </summary>
        void CallInstanceMethod();

        /// <summary>
        /// インスタンスメソッドを呼び出すサンプル: インスタンスの解放
        /// </summary>
        void ReleaseInstance();


        /// <summary>
        /// Swift から C# のメソッドを呼び出すサンプル
        /// </summary>
        void CallbackSample();
    }

    internal class NativeProxyImpl : INativeProxy
    {
        public void PrintHelloWorld()
        {
            NativeMethod();

            [DllImport("__Internal", EntryPoint = "printHelloWorld")]
            static extern void NativeMethod();
        }


        public Int32 ParamSample(Int32 value)
        {
            return NativeMethod(value);

            [DllImport("__Internal", EntryPoint = "paramSampleInt")]
            static extern Int32 NativeMethod(Int32 value);
        }

        public string ParamSample(string message)
        {
            return NativeMethod(message);

            [DllImport("__Internal", EntryPoint = "paramSampleString")]
            static extern string NativeMethod(string message);
        }


        private IntPtr _instance = IntPtr.Zero;

        public void CreateInstance()
        {
            if (_instance != IntPtr.Zero)
            {
                Debug.Log("[Unity]: Instance already created");
                return;
            }

            // Swift 側のインスタンスを生成
            // NOTE: インスタンスのポインタを戻り値として受け取ることで C# 側で管理する
            _instance = NativeMethod();
            Debug.Log("[Unity]: Instance created");

            [DllImport("__Internal", EntryPoint = "createInstance")]
            static extern IntPtr NativeMethod();
        }

        public void CallInstanceMethod()
        {
            if (_instance == IntPtr.Zero)
            {
                Debug.Log("[Unity]: Instance not created");
                return;
            }

            // インスタンスメソッドの呼び出し
            // NOTE: 呼び出したいインスタンスのポインタだけ渡し、処理自体はネイティブ側で行う
            NativeMethod(_instance);

            [DllImport("__Internal", EntryPoint = "callInstanceMethod")]
            static extern void NativeMethod(IntPtr instance);
        }

        public void ReleaseInstance()
        {
            if (_instance == IntPtr.Zero)
            {
                Debug.Log("[Unity]: Instance not created");
                return;
            }

            // 必要が無くなったら解放
            NativeMethod(_instance);
            _instance = IntPtr.Zero;
            Debug.Log("[Unity]: Instance released");

            [DllImport("__Internal", EntryPoint = "releaseInstance")]
            static extern void NativeMethod(IntPtr instance);
        }


        // ネイティブに渡すメソッドと同じフォーマットのデリゲート
        private delegate void SampleCallback(string message);

        public void CallbackSample()
        {
            // ネイティブに `OnCallback` を渡して呼び出してもらう
            NativeMethod(OnCallback);

            // iOS (正確に言うと IL2CPP) の場合には Static Method に対し、`MonoPInvokeCallbackAttribute` を付ける必要がある
            [MonoPInvokeCallback(typeof(SampleCallback))]
            static void OnCallback(string message)
            {
                // このメソッドがネイティブから呼び出される
                Debug.Log("[Unity]: " + message);
            }

            [DllImport("__Internal", EntryPoint = "callbackSample")]
            static extern void NativeMethod([MarshalAs(UnmanagedType.FunctionPtr)] SampleCallback sampleCallback);
        }
    }

    internal class NativeProxyDummy : INativeProxy
    {
        public void PrintHelloWorld()
        {
        }

        public int ParamSample(int value)
        {
            return 0;
        }

        public string ParamSample(string message)
        {
            return "";
        }

        public void CreateInstance()
        {
        }

        public void CallInstanceMethod()
        {
        }

        public void ReleaseInstance()
        {
        }

        public void CallbackSample()
        {
        }
    }
}