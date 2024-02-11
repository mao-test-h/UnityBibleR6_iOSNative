using PluginExamples.Introduction.Plugins.iOS;
using UnityEngine;
using UnityEngine.UI;

namespace PluginExamples.Introduction
{
    internal sealed class IntroductionExample : MonoBehaviour
    {
        [SerializeField] private Button helloWorldButton;
        [SerializeField] private Button paramSampleIntButton;
        [SerializeField] private Button paramSampleStringButton;

        [SerializeField] private Button createInstanceButton;
        [SerializeField] private Button callInstanceMethodButton;
        [SerializeField] private Button releaseInstanceButton;

        [SerializeField] private Button callbackSampleButton;

        private INativeProxy _nativeProxy;

        private void Start()
        {
#if UNITY_IOS && !UNITY_EDITOR
            _nativeProxy = new NativeProxyImpl();
#else
            _nativeProxy = new NativeProxyDummy();
#endif

            helloWorldButton.onClick.AddListener(() => { _nativeProxy.PrintHelloWorld(); });

            paramSampleIntButton.onClick.AddListener(() =>
            {
                var ret = _nativeProxy.ParamSample(1);
                Debug.Log($"[Unity]: {ret}");
            });

            paramSampleStringButton.onClick.AddListener(() =>
            {
                var ret = _nativeProxy.ParamSample("UnityMessage");
                Debug.Log($"[Unity]: {ret}");
            });

            createInstanceButton.onClick.AddListener(() => { _nativeProxy.CreateInstance(); });
            callInstanceMethodButton.onClick.AddListener(() => { _nativeProxy.CallInstanceMethod(); });
            releaseInstanceButton.onClick.AddListener(() => { _nativeProxy.ReleaseInstance(); });

            callbackSampleButton.onClick.AddListener(() => { _nativeProxy.CallbackSample(); });
        }
    }
}