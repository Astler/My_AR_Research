﻿using TMPro;
using UniRx;
using UnityEngine;

namespace Plugins.Honeti.I18N.Scripts
{
    public class I18NText : MonoBehaviour
    {
        private string _key = "";
        private TMP_Text _text;
        private bool _initialized = false;
        private bool _isValidKey = false;
        private TMP_FontAsset _defaultFont;
        private float _defaultLineSpacing;
        private float _defaultFontSize;
        private TextAlignmentOptions _defaultAlignment;

        [SerializeField]
        private bool _dontOverwrite = false;

        public string[] _params;

        public bool isFromCode;
        void Start()
        {
            if (!_initialized)
                _init();

            updateTranslation();
        }

        void OnDestroy()
        {
            if (_initialized)
            {
                I18N.OnLanguageChanged -= _onLanguageChanged;
                I18N.OnFontChanged -= _onFontChanged;
            }
        }

        /// <summary>
        /// Change text in Text component.
        /// </summary>
        private void _updateTranslation()
        {
            if (_text)
            {
                if (!_isValidKey)
                {
                    _key = _text.text;

                    if (_key.StartsWith("^"))
                    {
                        _isValidKey = true;
                    }
                }

                _text.text = I18N.instance.GetValue(_key, _params);
            }
        }

        /// <summary>
        /// Update translation text.
        /// </summary>
        /// <param name="invalidateKey">Force to invalidate current translation key</param>
        public void updateTranslation(bool invalidateKey = false)
        {
            if (invalidateKey)
            {
                _isValidKey = false;
            }

            _updateTranslation();
        }

        /// <summary>
        /// Init component.
        /// </summary>
        private void _init()
        {
            _text = GetComponent<TMP_Text>();
            _defaultFont = _text.font;
            _defaultLineSpacing = _text.lineSpacing;
            _defaultFontSize = _text.fontSize;
            _defaultAlignment = _text.alignment;
            _key = _text.text;
            _initialized = true;

            if (I18N.instance.useCustomFonts)
            {
                _changeFont(I18N.instance.customFont);
            }

            I18N.OnLanguageChanged += _onLanguageChanged;
            I18N.OnFontChanged += _onFontChanged;
            _text.ObserveEveryValueChanged(t => t.text).Subscribe(delegate(string s)
            {
                if (s.StartsWith("^"))
                {
                    updateTranslation(true);
                }
            }).AddTo(this);

            if (!_key.StartsWith("^") && !isFromCode)
            {
                Debug.LogWarning(string.Format("{0}: Translation key was not found! Found {1}", this, _key));
                _isValidKey = false;
            }
            else
            {
                _isValidKey = true;
            }

            if (!_text)
            {
                Debug.LogWarning(string.Format("{0}: Text component was not found!", this));
            }
        }

        private void _onLanguageChanged(LanguageCode newLang)
        {
            _updateTranslation();
        }

        private void _onFontChanged(I18NFonts newFont)
        {
            _changeFont(newFont);
        }

        private void _changeFont(I18NFonts f)
        {
            if (_dontOverwrite)
            {
                return;
            }

            if (f != null)
            {
                if (f.font)
                {
                    _text.font = f.font;
                }
                else
                {
                    _text.font = _defaultFont;
                }
                if (f.customLineSpacing)
                {
                    _text.lineSpacing = f.lineSpacing;
                }
                if (f.customFontSizeOffset)
                {
                    _text.fontSize = (int)(_defaultFontSize + (_defaultFontSize * f.fontSizeOffsetPercent /100));
                }
                if (f.customAlignment)
                {
                    _text.alignment = f.alignment;
                }
            }
            else
            {
                _text.font = _defaultFont;
                _text.lineSpacing = _defaultLineSpacing;
                _text.fontSize = _defaultFontSize;
                _text.alignment = _defaultAlignment;
            }
        }

        public void InitWithKey(string key)
        {
            _key = key;
            
        }
    }
}