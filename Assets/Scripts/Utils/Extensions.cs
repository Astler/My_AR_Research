using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Utils
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this string self) => string.IsNullOrEmpty(self);
        
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

            T[] arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf(arr, src) + 1;
            return (arr.Length == j) ? arr[0] : arr[j];
        }
        
        public static void BindViewAndPresenter<TView, TPresenter>(this DiContainer container)
        {
            container.BindInterfacesAndSelfTo<TView>().FromComponentInHierarchy().AsSingle().NonLazy();
            container.BindInterfacesAndSelfTo<TPresenter>().FromNew().AsSingle().NonLazy();
        }

        public static void ActionWithThrottle(this Button button, Action action, int throttleMillis = 200)
        {
            UniRx.ObservableExtensions.Subscribe(button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(throttleMillis)),_ =>
            {
                action?.Invoke();
            }).AddTo(button);
        }

        public static void SetAlpha(this Image image, float alpha)
        {
            Color imageColor = image.color;
            image.color = new Color(imageColor.r, imageColor.g, imageColor.b, alpha);
        }
    }
}