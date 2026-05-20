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
            GameObject root = new GameObject("Ricochet Brigade Greybox");

            GameObject gridObject = new GameObject("Arena Grid");
            gridObject.transform.SetParent(root.transform, false);
            ArenaGrid grid = gridObject.AddComponent<ArenaGrid>();
            grid.columns = BrigadeTuning.Columns;
            grid.rows = BrigadeTuning.Rows;
            grid.cellSize = BrigadeTuning.CellSize;

            GameObject launcherObject = new GameObject("Slingshot Launcher");
            launcherObject.transform.SetParent(root.transform, false);
            SlingshotLauncher launcher = launcherObject.AddComponent<SlingshotLauncher>();
            launcher.grid = grid;

            GameObject controllerObject = new GameObject("Game Controller");
            controllerObject.transform.SetParent(root.transform, false);
            GameController controller = controllerObject.AddComponent<GameController>();
            controller.grid = grid;
            controller.launcher = launcher;

            Selection.activeGameObject = root;
            EnsureSceneFolder();
            EditorSceneManager.SaveScene(camera.gameObject.scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("Ricochet Brigade greybox scene created at " + ScenePath + ". Press Play to test flick -> merge -> settle -> defend.");
        }

        private static Camera CreateCamera()
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 7.4f;
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
