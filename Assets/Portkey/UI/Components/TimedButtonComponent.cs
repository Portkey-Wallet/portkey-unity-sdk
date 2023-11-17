using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class TimedButtonComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Button button;
        [SerializeField] private float maxInterval = 60.9f;

        private float _timer = 0.0f;
        
        public delegate void OnClickHandler();
        public event OnClickHandler OnClick;

        public bool Interactable
        {
            get => button.interactable;
            set => button.interactable = value;
        }
        
        private string Text { get; set; }

        private void Start()
        {
            button.onClick.AddListener(OnClickButton);
            Text = description.text;
        }
        
        private void OnClickButton()
        {
            OnClick?.Invoke();
        }

        private void BecomesClickable()
        {
            description.text = $"{Text}";
            Interactable = true;
        }
        
        public void ActivateTimeOut()
        {
            _timer = maxInterval;
            Interactable = false;
        }
        
        private void Update()
        {
            if (_timer <= 0.0f)
            {
                BecomesClickable();
                return;
            }
            
            _timer -= Time.deltaTime;
            
            if (_timer <= 0.0f)
            {
                _timer = 0.0f;
            }

            description.text = $"{Text} after {(int)_timer}s";
        }
    }
}