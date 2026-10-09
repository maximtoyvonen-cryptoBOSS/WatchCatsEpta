#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;

public class TestSceneBuilder
{
    [MenuItem("Tools/WatchCats/Generate Test Sandbox Scene")]
    public static void GenerateScene()
    {
        // Create new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        newScene.name = "TestSandbox";

        // Create Environment
        GameObject environment = new GameObject("Environment");

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.SetParent(environment.transform);
        floor.transform.localScale = new Vector3(3f, 1f, 3f); // 30x30 roughly
        GameObjectUtility.SetStaticEditorFlags(floor, StaticEditorFlags.NavigationStatic);

        // Add some obstacles
        for (int i = 0; i < 5; i++)
        {
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.name = $"Obstacle_{i}";
            obstacle.transform.SetParent(environment.transform);
            obstacle.transform.position = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));
            obstacle.transform.localScale = new Vector3(2f, 2f, 2f);
            obstacle.layer = LayerMask.NameToLayer("Default"); // Ensure it blocks vision if needed
            GameObjectUtility.SetStaticEditorFlags(obstacle, StaticEditorFlags.NavigationStatic);
        }

        // Create Player
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.layer = LayerMask.NameToLayer("Default");
        player.transform.position = new Vector3(0, 1, 0);

        player.AddComponent<CharacterController>();
        player.AddComponent<PlayerController>();

        var interaction = player.AddComponent<PlayerInteraction>();
        var energy = player.AddComponent<HackingEnergy>();
        var hackManager = player.AddComponent<HackManager>();
        var hackScanner = player.AddComponent<HackScanner>();

        // Setup Main Camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.name = "Main Camera";
            mainCam.tag = "MainCamera";
            var tpCam = mainCam.gameObject.AddComponent<ThirdPersonCamera>();

            // We need to use SerializedObject to set private fields if there are no public setters,
            // but the prompt allows us to setup serialized links. Let's use reflection or SerializedObject.
            SerializedObject camSo = new SerializedObject(tpCam);
            camSo.FindProperty("target").objectReferenceValue = player.transform;
            camSo.ApplyModifiedProperties();
        }

        // Connect HackManager with ThirdPersonCamera
        SerializedObject hmSo = new SerializedObject(hackManager);
        hmSo.FindProperty("playerCamera").objectReferenceValue = mainCam.GetComponent<ThirdPersonCamera>();
        hmSo.FindProperty("energyManager").objectReferenceValue = energy;
        hmSo.FindProperty("playerController").objectReferenceValue = player.GetComponent<PlayerController>();
        hmSo.ApplyModifiedProperties();

        // Create AI Guard
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemy.name = "Guard";
        enemy.layer = LayerMask.NameToLayer("Default");
        enemy.transform.position = new Vector3(5, 1, 5);
        enemy.GetComponent<Renderer>().sharedMaterial.color = Color.red;

        var navAgent = enemy.AddComponent<NavMeshAgent>();
        var guardController = enemy.AddComponent<GuardController>();
        var fov = enemy.AddComponent<FieldOfView>();

        GameObject takedownZone = new GameObject("TakedownZone");
        takedownZone.transform.SetParent(enemy.transform);
        takedownZone.transform.localPosition = new Vector3(0, 0, -1); // Behind guard
        var tkCol = takedownZone.AddComponent<BoxCollider>();
        tkCol.isTrigger = true;
        takedownZone.AddComponent<GuardTakedown>();

        // Waypoints
        GameObject wp1 = new GameObject("Waypoint_1");
        wp1.transform.position = new Vector3(5, 0, 5);
        GameObject wp2 = new GameObject("Waypoint_2");
        wp2.transform.position = new Vector3(10, 0, 10);

        SerializedObject gcSo = new SerializedObject(guardController);
        SerializedProperty patrolArray = gcSo.FindProperty("patrolPoints");
        patrolArray.arraySize = 2;
        patrolArray.GetArrayElementAtIndex(0).objectReferenceValue = wp1.transform;
        patrolArray.GetArrayElementAtIndex(1).objectReferenceValue = wp2.transform;
        gcSo.ApplyModifiedProperties();

        // Hackable Objects
        GameObject cctv = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cctv.name = "CCTVCamera";
        cctv.transform.position = new Vector3(-5, 4, 5);
        var cctvNode = cctv.AddComponent<CCTVCameraNode>();
        GameObject camObj = new GameObject("Camera");
        camObj.transform.SetParent(cctv.transform);
        camObj.AddComponent<Camera>().enabled = false;
        camObj.AddComponent<AudioListener>().enabled = false;
        cctvNode.GetComponent<Camera>().enabled = false; // AddComponent logic adds to root

        GameObject overload = GameObject.CreatePrimitive(PrimitiveType.Cube);
        overload.name = "PowerDistributor";
        overload.transform.position = new Vector3(-8, 1, -8);
        overload.AddComponent<OverloadDistributor>();

        GameObject terminal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        terminal.name = "DataTerminal";
        terminal.transform.position = new Vector3(8, 1, -8);
        terminal.AddComponent<DataTerminal>();

        // Extraction Zone
        GameObject extraction = new GameObject("ExtractionZone");
        extraction.transform.position = new Vector3(0, 0, 12);
        var extCol = extraction.AddComponent<BoxCollider>();
        extCol.isTrigger = true;
        extCol.size = new Vector3(4, 2, 4);
        extraction.AddComponent<ExtractionZone>();

        // Managers
        GameObject managers = new GameObject("_Managers");
        managers.AddComponent<MissionManager>();
        managers.AddComponent<SaveSystem>();
        managers.AddComponent<YandexGamesManager>();
        // NoiseManager is static, doesn't need to be attached

        // Basic UI Setup
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject phoneUI = new GameObject("PhoneUI");
        phoneUI.transform.SetParent(canvasObj.transform);
        var phoneManager = phoneUI.AddComponent<PhoneUIManager>();

        GameObject phonePanel = new GameObject("PhonePanel");
        phonePanel.transform.SetParent(phoneUI.transform);
        phonePanel.SetActive(false);

        SerializedObject phoneSo = new SerializedObject(phoneManager);
        phoneSo.FindProperty("phonePanel").objectReferenceValue = phonePanel;
        phoneSo.ApplyModifiedProperties();

        // Optional: Bake NavMesh
        // NavMeshBuilder.BuildNavMesh(); // Requires UnityEditor.AI

        Debug.Log("WatchCats Sandbox Scene generated successfully!");
    }
}
#endif
