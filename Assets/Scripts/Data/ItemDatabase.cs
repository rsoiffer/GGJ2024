using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using AssetProcessing;
#endif

namespace Data
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item Database", order = 0)]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private ItemData[] database;

        [CanBeNull] private Dictionary<string, ItemData> _table;

        public ItemData Get(string id)
        {
            if (_table != null) return _table[id];

            _table = new Dictionary<string, ItemData>();
            foreach (var move in database) _table.Add(move.Id, move);
            return _table[id];
        }

        public IEnumerable<ItemData> GetAll()
        {
            return database;
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Build ItemDatabase")]
        public static void BuildDatabase()
        {
            AssetProcessor.IterateOverSelectedAssets(path =>
            {
                var moveDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabase>(path);
                if (moveDatabase == null) return;
                var pbsDatabase = PbsDatabase.LoadDatabase("items");
                moveDatabase.database =
                    pbsDatabase.Entries.Values.Select(pbsEntry => new ItemData(pbsEntry)).ToArray();
                EditorUtility.SetDirty(moveDatabase);
                AssetDatabase.SaveAssetIfDirty(moveDatabase);
            });
        }
#endif
    }

    [Serializable]
    public class ItemData
    {
        public string Id;
        public string Name;
        public Sprite Sprite;

#if UNITY_EDITOR
        public ItemData(PbsEntry pbsEntry)
        {
            try
            {
                Id = pbsEntry.GetId();
                Name = pbsEntry.Lookup("Name");
                Sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/DataPack/Graphics/Items/{Id}.png");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while parsing move {pbsEntry.GetId()}");
                Debug.LogException(e);
            }
        }
#endif
    }
}