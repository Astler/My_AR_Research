using UnityEngine;
using Zenject;

namespace Assets
{
    [CreateAssetMenu(fileName = "GameAssetsSO", menuName = "SO/GameAssets/GameAssetsSO")]
    public class GameAssetsSO : ScriptableObjectInstaller<GameAssetsSO>
    {
        [SerializeField] private EditorAssets editorAssets;
        [SerializeField] private ScreenAssets screenAssets;
        [SerializeField] private AssetsScriptableObject assetsScriptableObject;

        public override void InstallBindings()
        {
            Container.BindInstance(editorAssets);
            Container.BindInstance(screenAssets);
            Container.BindInstance(assetsScriptableObject);
        }
    }
}