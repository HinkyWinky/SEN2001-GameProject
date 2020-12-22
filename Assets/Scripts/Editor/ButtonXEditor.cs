using UnityEditor;
using Game.UI;
using UnityEditor.UI;

[CustomEditor(typeof(ButtonX))]
public class buttonXEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        ButtonX component = (ButtonX)target;

        base.OnInspectorGUI();

        component.hasText = EditorGUILayout.Toggle("Has Text", component.hasText);
    }
}
