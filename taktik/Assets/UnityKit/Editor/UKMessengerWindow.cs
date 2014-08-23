using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class UKMessengerWindow : EditorWindow 
{
    private List<Entry> events = new List<Entry>();
    private Vector2 scrollPos;

    private struct Entry
    {
        public string Keyword;
        public string Name;
        public string Parameters;
    }

	// Add menu named "My Window" to the Window menu
	[MenuItem ("UnityKit/Messenger")]
	static void Init () 
    {
		// Get existing open window or if none, make a new one:
        UKMessengerWindow window = (UKMessengerWindow)EditorWindow.GetWindow(typeof(UKMessengerWindow));
        window.Parse();
	}
	
	void OnGUI () 
    {
        if (GUILayout.Button("Parse"))
        {
            Parse();
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        foreach (var e in events)
        {
            EditorGUILayout.TextField(e.Keyword, string.Format("{0} {1}", e.Name, e.Parameters));
        }

        GUILayout.EndScrollView();
	}

    private void Parse()
    {
        events.Clear();

        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        foreach(var file in files)
        {
            events.AddRange(ParseFile(file));
        }

        events = events.OrderBy(it => it.Keyword + it.Name + it.Parameters).Distinct().ToList();
    }

    private IEnumerable<Entry> ParseFile(string file)
    {
        string text = File.ReadAllText(file);

        {
            Regex r = new Regex(@"UKMessenger.(AddListener|Broadcast)\s*([^(]*)\s*\(\s*""\s*([^""]+)\s*""");

            foreach (Match m in r.Matches(text))
            {
                yield return new Entry() { 
                    Keyword = m.Groups[1].ToString(),
                    Name = m.Groups[3].ToString(),
                    Parameters = m.Groups[2].ToString(),
                };
            }
        }
    }
}