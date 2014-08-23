using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AudioDb.Music))]
[CustomPropertyDrawer(typeof(AudioDb.Sound))]
public class SoundDrawer : PropertyDrawer
{
    const int height = 80;

    private static GameObject player = null;

    public override float GetPropertyHeight(SerializedProperty prop,
                                             GUIContent label)
    {
        return height;
    }

    public class Grid
    {
        // cells
        public int CellsX;
        public int CellsY;
        // whole pixel area
        public Rect Area;
        // internal pixel border
        public int Border;

        public Grid(Rect area, int cellsX, int cellsY, int border)

        {
            Area = area;
            CellsX = cellsX;
            CellsY = cellsY;
            Border = border;
        }

        public Rect GetCellsArea(int x, int y, int w, int h)
        {
            int cellW = Mathf.FloorToInt(Area.width / CellsX);
            int cellH = Mathf.FloorToInt(Area.height / CellsY);

            Rect r = new Rect(Area.x + x * cellW, Area.y + y * cellH, w * cellW, h * cellH);
            r.x += Border / 2;
            r.y += Border / 2;
            r.width -= Border / 2;
            r.height -= Border / 2;

            return r;
        }
    }

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty name = prop.FindPropertyRelative("Name");
        SerializedProperty clip = prop.FindPropertyRelative("Clip");
        SerializedProperty volume = prop.FindPropertyRelative("Volume");

        if (name == null || clip == null || volume == null) return;

        Grid g = new Grid(pos, 2, 4, 5);

        EditorGUI.PropertyField(g.GetCellsArea(0, 0, 2, 1), name, GUIContent.none);

        EditorGUI.PropertyField(g.GetCellsArea(0, 1, 1, 1), clip, GUIContent.none);

        EditorGUI.Slider(g.GetCellsArea(1, 1, 1, 1), volume, 0f, 1f, GUIContent.none);
            
        var audioClip = clip.objectReferenceValue as AudioClip;

        if (GUI.Button(g.GetCellsArea(0, 2, 2, 1), "Play"))
        {

            if (player != null) {
                GameObject.DestroyImmediate(player);
                player = null;
            }

            if (player == null)
            {
                player = new GameObject();
                player.hideFlags = HideFlags.HideAndDontSave;
                var s = player.AddComponent<AudioSource>();
                s.clip = audioClip;
                s.volume = volume.floatValue;
                s.Play();
            }
        }

        // update volume on running clip
        if (player != null && player.audio != null && player.audio.clip == audioClip)
        {
            player.audio.volume = volume.floatValue;
        }
    }
}