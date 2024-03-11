using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GUIModu
{
    public class DialogSystem : MonoBehaviour
    {
        public GameObject dialogBox;
        
        public Text dialogText;
        
        [SerializeField] private Color textColor;

        [SerializeField] private float closeTime = 3f;
        
        private CancellationTokenSource _cts;

        public void Active(string context, string charName = "", Color color =  default)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            ShowDialogAsync(charName + ": " + context, color).Forget();
        }

        private async UniTask ShowDialogAsync(string text, Color color)
        {
            await UniTask.SwitchToMainThread();
            dialogText.color = color == default ? textColor : color;
            dialogBox.SetActive(true);
       
            // 添加打字效果
            dialogText.text = "";
            bool isCancel;
            foreach (var ch in text)
            {
                dialogText.text += ch;
                isCancel = await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: _cts.Token).SuppressCancellationThrow();
                if (isCancel) return;
            }
            
            isCancel = await UniTask.Delay(TimeSpan.FromSeconds(closeTime), cancellationToken: _cts.Token).SuppressCancellationThrow();
            if (isCancel) return;
            
            CloseDialog();
        }

        private void CloseDialog()
        {
            dialogBox.SetActive(false);
        }
        
        private static DialogSystem _instance = null;
        
        public static bool Exists() {
            return _instance != null;
        }

        public static DialogSystem Instance() {
            if (!Exists ()) {
                throw new Exception ("DialogSystem could not find the DialogSystem object. Please ensure you have added the DialogSystem Prefab to your scene.");
            }
            return _instance;
        }

        private void Awake() {
            if (_instance == null) {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

            CloseDialog();
        }

        private void OnDestroy() {
            _instance = null;
        }
    }
}