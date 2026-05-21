using RicochetBrigade;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RicochetBrigadeEditor
{
    public static class GreyboxSceneBuilder
    {
        private const string SceneFolder = "Assets/Scenes";
        private const string ScenePath = "Assets/Scenes/GreyboxArena.unity";

        [MenuItem("Ricochet Brigade/Build Greybox Scene")]
        [MenuItem("Tools/Ricochet Brigade/Build Greybox Scene")]
        [MenuItem("GameObject/Ricochet Brigade/Build Greybox Scene", false, 10)]
        public static void BuildGreyboxScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            Camera camera = CreateCamera();
            GameObject root = new GameObject("Ricochet Brigade V2 Blockout");

            GameObject layoutObject = new GameObject("V2 Arena Layout");
            layoutObject.transform.SetParent(root.transform, false);
            V2ArenaLayout layout = layoutObject.AddComponent<V2ArenaLayout>();
            layout.BuildArena();

            GameObject launcherObject = new GameObject("V2 Slingshot Spell");
            launcherObject.transform.SetParent(root.transform, false);
            V2SlingshotSpell launcher = launcherObject.AddComponent<V2SlingshotSpell>();
            launcher.layout = layout;

            GameObject controllerObject = new GameObject("V2 Game Controller");
            controllerObject.transform.SetParent(root.transform, false);
            V2GameController controller = controllerObject.AddComponent<V2GameController>();
            controller.layout = layout;
            controller.slingshot = launcher;

            Selection.activeGameObject = root;
            EnsureSceneFolder();
            EditorSceneManager.SaveScene(camera.gameObject.scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("Ricochet Brigade v2 blockout scene created at " + ScenePath + ". Press Play to test card placement, hero ults, and slingshot spell.");
        }

        private static Camera CreateCamera()
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 8.0f;
            camera.backgroundColor = new Color(0.07f, 0.08f, 0.1f);
            camera.transform.position = new Vector3(0f, 0f, -10f);
            return camera;
        }

        private static void EnsureSceneFolder()
        {
            if (!AssetDatabase.IsValidFolder(SceneFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
        }
    }
}
