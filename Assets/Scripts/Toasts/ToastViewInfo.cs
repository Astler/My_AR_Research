using System;

namespace Toasts
{
    public struct ToastViewInfo
    {
        public string Text;
        public float Duration;

        public Action Showed;
        public Action Hidden;
    }
}