using System;
using System.Collections.Generic;
using System.Linq;
using AssetProcessing;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

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
    }

    [Serializable]
    public class ItemData
    {
        public string Id;
        public string Name;
        public Sprite Sprite;

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
    }
}