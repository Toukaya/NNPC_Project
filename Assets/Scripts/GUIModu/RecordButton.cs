using System;
using Character.NPC;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VocaModu;

namespace GUIModu
{
    [RequireComponent(typeof(Text))]
    public class RecordButton : MonoBehaviour
    {
        private enum State {
            Waiting,  
            Processing,
            Recording
        }
        private Button _btn;
        private AudioClip _recording;
        private bool _isRecording;
        private State _currentState = State.Waiting;
        [SerializeField] [NotNull] private Text text;
        public UnityEvent onWait;
        public UnityEvent onRecord;
        
        private void Awake()
        {
            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(TaskOnClick);
            if (!text)
                text = GetComponentInChildren<Text>();
        }

        private void Start()
        {
            SetButtonText("Start Recording");
        }

        private void SetButtonText(string context)
        {
            text.text = context;
        }

        private void TaskOnClick()
        {
            switch (_currentState)
            {
                case State.Waiting:
                    StartRecording();
                    break;
                case State.Processing:
                    break;
                case State.Recording:
                    EndRecording();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartRecording()
        {
            _currentState = State.Processing;
            _isRecording = true;
            _btn.interactable = false;
            SetButtonText("Processing");
            onRecord?.Invoke();
        }

        public void SetEnable()
        {
            _btn.interactable = true;
            if (_isRecording)
            {
                _currentState = State.Recording;
                SetButtonText("Stop Recording");
            }
            else
            {
                _currentState = State.Waiting;
                SetButtonText("Start Recording");
            }
        }

        private void EndRecording()
        {
            _currentState = State.Processing;
            _isRecording = false;
            _btn.interactable = false;
            SetButtonText("Processing");
            onWait?.Invoke();
        }
    }
}