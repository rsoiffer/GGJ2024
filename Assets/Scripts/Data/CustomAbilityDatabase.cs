#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Data
{
    public class PokemonDatabaseEditor : EditorWindow
    {
        [SerializeField] private PokemonDatabase pokeDatabase;
        [SerializeField] private int pokeNum;
        [SerializeField] private Ability[] abilities;
        [SerializeField] private bool done;

        private void OnEnable()
        {
            if (pokeDatabase == null) return;
            pokeNum = 0;
            while (pokeDatabase.GetAll()[pokeNum].CustomAbilities != null &&
                   pokeDatabase.GetAll()[pokeNum].CustomAbilities!.Length >= 2) pokeNum++;
        }

        private void OnGUI()
        {
            using var scriptableObject = new SerializedObject(this);
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(scriptableObject.FindProperty("pokeDatabase"), true);
            scriptableObject.ApplyModifiedProperties();

            if (pokeDatabase == null) return;
            EditorGUILayout.LabelField(pokeDatabase.GetAll()[pokeNum].Name);
            EditorGUILayout.PropertyField(scriptableObject.FindProperty("abilities"), true);
            EditorGUILayout.PropertyField(scriptableObject.FindProperty("done"), true);
            scriptableObject.ApplyModifiedProperties();

            if (done)
            {
                pokeDatabase.GetAll()[pokeNum].CustomAbilities = abilities;
                EditorUtility.SetDirty(pokeDatabase);
                pokeNum += 1;
                done = false;
            }
        }

        [MenuItem("Tools/Pokemon Database Editor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(PokemonDatabaseEditor));
        }
    }
}
#endif