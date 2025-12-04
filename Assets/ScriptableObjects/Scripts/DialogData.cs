using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName ="Dialog")]
public class DialogData : ScriptableObject
{
    public string dialogName;
    [TextArea(3, 3)]
    public string text;
}
