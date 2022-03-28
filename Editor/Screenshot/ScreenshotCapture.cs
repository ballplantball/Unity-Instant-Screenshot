using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Screenshot : EditorWindow
{
	int resWidth = Screen.width; 
	int resHeight = Screen.height;
	int scale = 1;

	public Camera Cam;

	string path = "";

	bool isTransparent = false;

	[MenuItem("Tool/Screenshot")]
	public static void ShowWindow()
	{
		//Show "Screenshot" Window
		EditorWindow editorWindow = EditorWindow.GetWindow(typeof(Screenshot));
		editorWindow.autoRepaintOnSceneChange = true;
		editorWindow.Show();
		editorWindow.title = "Screenshot";
	}

	float lastTime;

	void OnGUI()
	{
		EditorGUILayout.LabelField ("Resolution", EditorStyles.boldLabel);
		resWidth = EditorGUILayout.IntField ("Width", resWidth);
		resHeight = EditorGUILayout.IntField ("Height", resHeight);

		EditorGUILayout.Space();

		scale = EditorGUILayout.IntSlider ("Scale", scale, 1, 10);

		EditorGUILayout.HelpBox("The default resoluition is your game preview, you can edit Width and Height do you want",MessageType.None);
		
		EditorGUILayout.Space();		
		
		GUILayout.Label ("Save Path", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.TextField(path,GUILayout.ExpandWidth(false));
		if(GUILayout.Button("Browse",GUILayout.ExpandWidth(false)))
			path = EditorUtility.SaveFolderPanel("Path to Save Images",path,Application.dataPath);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.HelpBox("Choose the folder in which to save the screenshot",MessageType.None);
		EditorGUILayout.Space();

		GUILayout.Label ("Select Camera", EditorStyles.boldLabel);

		Cam = EditorGUILayout.ObjectField(Cam, typeof(Camera), true,null) as Camera;

		if(Cam == null)
		{
			Cam = Camera.main;
		}

		isTransparent = EditorGUILayout.Toggle("Transparent Background", isTransparent);


		EditorGUILayout.HelpBox("Choose the camera of which need to capture",MessageType.None);

		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField ("Default Options", EditorStyles.boldLabel);

		if(GUILayout.Button("Preview size"))
		{
			resHeight = (int)Handles.GetMainGameViewSize().y;
			resWidth = (int)Handles.GetMainGameViewSize().x;		
		}
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("res 5.5"))
		{
			resHeight = 1242;
			resWidth = 2208;
			scale = 1;
		}
		if (GUILayout.Button("res 6.5"))
		{
			resHeight = 1242;
			resWidth = 2688;
			scale = 1;
		}
		if (GUILayout.Button("iPad"))
		{
			resHeight = 2048;
			resWidth = 2732;
			scale = 1;
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Portrait", GUILayout.MinHeight(40)))
		{
			if(resWidth > resHeight)
            {
				int _width = resWidth;
				resWidth = resHeight;
				resHeight = _width;
            }	
		}

		if (GUILayout.Button("Landscape",GUILayout.MinHeight(40)))
		{
			if (resHeight > resWidth)
			{
				int _height = resHeight;
				resHeight = resWidth;
				resWidth = _height;
			}
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();
		
		if(GUILayout.Button("Take Screenshot",GUILayout.MinHeight(60)))
		{
			if(path == "")
			{
				path = EditorUtility.SaveFolderPanel("Path to Save Images",path,Application.dataPath);				
				TakeHiResShot();
			}
			else
			{
				TakeHiResShot();
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();

		if(GUILayout.Button("Open Last Screenshot",GUILayout.MaxWidth(160),GUILayout.MinHeight(40)))
		{
			if(lastScreenshot != "")
			{
				Application.OpenURL("file://" + lastScreenshot);
				Debug.Log("Opening File " + lastScreenshot);
			}
		}

		if(GUILayout.Button("Open Folder",GUILayout.MaxWidth(100),GUILayout.MinHeight(40)))		
			Application.OpenURL("file://" + path);

		EditorGUILayout.EndHorizontal();

		if (takeHiResShot) 
		{
			int resWidthN = resWidth*scale;
			int resHeightN = resHeight*scale;
			RenderTexture tex = new RenderTexture(resWidthN, resHeightN, 24);
			Cam.targetTexture = tex;

			TextureFormat tFormat;
			if(isTransparent)
				tFormat = TextureFormat.ARGB32;
			else
				tFormat = TextureFormat.RGB24;

			Texture2D screenShot = new Texture2D(resWidthN, resHeightN, tFormat,false);
			Cam.Render();
			RenderTexture.active = tex;
			screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
			Cam.targetTexture = null;
			RenderTexture.active = null; 
			byte[] bytes = screenShot.EncodeToPNG();
			string filename = ScreenShotName(resWidthN, resHeightN);
			
			System.IO.File.WriteAllBytes(filename, bytes);
			Debug.Log(string.Format("Took screenshot to: {0}", filename));			
			takeHiResShot = false;
		}
	}
	
	private bool takeHiResShot = false;
	public string lastScreenshot = "";	
		
	public string ScreenShotName(int width, int height) {

		string strPath="";

		strPath = string.Format("{0}/screen_{1}x{2}_{3}.png", path, width, height, System.DateTime.Now.ToString("HH-mm-ss"));
		lastScreenshot = strPath;
		
		return strPath;
	}

	public void TakeHiResShot() {
		takeHiResShot = true;
	}
}