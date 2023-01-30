using System;
using TMPro;
using UnityEngine;

namespace Screens.Views
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class InfoTextView: MonoBehaviour
    {
        [SerializeField] private Color baseTextColor = Color.white;
        [SerializeField] private Color hintTextColor = new(0.44f, 0.72f, 1f);
        [SerializeField] private Color titleTextColor = Color.white;
        
        private TextMeshProUGUI _text;

        public void SetText(InfoTextViewInfo viewInfo)
        {
            _text.text = viewInfo.Text;
            _text.color = viewInfo.TextType switch
            {
                InfoTextType.Base => baseTextColor,
                InfoTextType.Hint => hintTextColor,
                InfoTextType.Title => titleTextColor,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }
    }
}