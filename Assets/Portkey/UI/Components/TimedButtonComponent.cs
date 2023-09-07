using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class TimedButtonComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Button button;
        [SerializeField] private float maxInterval = 60.0f;

        private float _timer = 0.0f;
        
        public delegate void OnClickHandler();
        public event OnClickHandler OnClick;
        public string Text { get; set; }

        private void Start()
        {
            button.onClick.AddListener(OnClickButton);
            Text = description.text;
        }
        
        private void OnClickButton()
        {
            OnClick?.Invoke();
            _timer = maxInterval;
            button.interactable = false;
        }
        
        public void Activate()
        {
            description.text = $"{Text}";
            button.interactable = true;
        }
        
        private void Update()
        {
            if (_timer <= 0.0f)
            {
                Activate();
                return;
            }
            
            _timer -= Time.deltaTime;
            
            if (_timer <= 0.0f)
            {
                _timer = 0.0f;
            }

            description.text = $"{Text} in ({(int)_timer})";
        }
    }
}