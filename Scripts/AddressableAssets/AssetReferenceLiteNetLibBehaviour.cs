using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LiteNetLibManager
{
    [System.Serializable]
    public class AssetReferenceLiteNetLibBehaviour<TBehaviour> : AssetReferenceLiteNetLibIdentity
        where TBehaviour : LiteNetLibBehaviour
    {
#if UNITY_EDITOR
        public AssetReferenceLiteNetLibBehaviour(LiteNetLibBehaviour behaviour) : base(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(behaviour)))
        {
            if (behaviour != null && behaviour.TryGetComponent(out LiteNetLibIdentity identity))
            {
                hashAssetId = identity.HashAssetId;
                Debug.Log($"[AssetReferenceLiteNetLibBehaviour] Set `hashAssetId` to `{hashAssetId}`, name: {behaviour.name}");
            }
            else
            {
                hashAssetId = 0;
                Debug.LogWarning($"[AssetReferenceLiteNetLibBehaviour] Cannot find behaviour, so set `hashAssetId` to `0`");
            }
        }

        public override bool SetEditorAsset(Object value)
        {
            if (!base.SetEditorAsset(value))
            {
                return false;
            }

            if ((value is GameObject gameObject) && gameObject.TryGetComponent(out LiteNetLibIdentity identity))
            {
                hashAssetId = identity.GetComponent<LiteNetLibIdentity>().HashAssetId;
                Debug.Log($"[AssetReferenceLiteNetLibBehaviour] Set `hashAssetId` to `{hashAssetId}` when set editor asset: `{value.name}`");
                return true;
            }
            else
            {
                hashAssetId = 0;
                Debug.LogWarning($"[AssetReferenceLiteNetLibBehaviour] Cannot find behaviour or not proper object's type, so set `hashAssetId` to `0`");
                return false;
            }
        }
#endif

        public new AsyncOperationHandle<TBehaviour> InstantiateAsync(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Addressables.ResourceManager.CreateChainOperation(Addressables.InstantiateAsync(RuntimeKey, position, rotation, parent, false), GameObjectReady);
        }

        public new AsyncOperationHandle<TBehaviour> InstantiateAsync(Transform parent = null, bool instantiateInWorldSpace = false)
        {
            return Addressables.ResourceManager.CreateChainOperation(Addressables.InstantiateAsync(RuntimeKey, parent, instantiateInWorldSpace, false), GameObjectReady);
        }

        public new AsyncOperationHandle<TBehaviour> LoadAssetAsync()
        {
            return Addressables.ResourceManager.CreateChainOperation(base.LoadAssetAsync<GameObject>(), GameObjectReady);
        }

        static AsyncOperationHandle<TBehaviour> GameObjectReady(AsyncOperationHandle<GameObject> arg)
        {
            var comp = arg.Result.GetComponent<TBehaviour>();
            return Addressables.ResourceManager.CreateCompletedOperation(comp, string.Empty);
        }

        public override bool ValidateAsset(Object obj)
        {
            return ValidateAsset<TBehaviour>(obj);
        }

        public override bool ValidateAsset(string path)
        {
            return ValidateAsset<TBehaviour>(path);
        }
    }


    [System.Serializable]
    public class AssetReferenceLiteNetLibBehaviour : AssetReferenceLiteNetLibBehaviour<LiteNetLibBehaviour>
    {
        public AssetReferenceLiteNetLibBehaviour(LiteNetLibBehaviour behaviour) : base(behaviour)
        {
        }
    }
}