using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Reporting;

// LinkX SDK 部署辅助：按《LinkX SDK 使用手册》在 SampleScene 中接入 Cave 运行所需对象并打包。
public static class CaveDeploy
{
    const string ScenePath = "Assets/Scenes/SampleScene.unity";
    const string TrackerPrefab = "Assets/LinkXWorkspace/Prefabs/TrackerManager.prefab";
    const string PlayerPrefab = "Assets/LinkXWorkspace/Prefabs/Player.prefab";
    const string EventSystemPrefab = "Assets/LinkXWorkspace/Prefabs/EventSystem.prefab";
    const string BuildPath = "Build/Windows/Cave.exe";

    [MenuItem("CaveDeploy/Setup Scene")]
    public static void SetupScene()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        // 场景自带的 Main Camera 与 Player 预制体中的相机重复（AudioListener 冲突），移除场景根节点上的相机
        for (int i = scene.GetRootGameObjects().Length - 1; i >= 0; i--)
        {
            GameObject root = scene.GetRootGameObjects()[i];
            if (root.name == "Main Camera")
            {
                Object.DestroyImmediate(root);
                Debug.Log("[CaveDeploy] removed scene root 'Main Camera'");
            }
        }

        InstantiateIfMissing(TrackerPrefab);
        InstantiateIfMissing(PlayerPrefab);
        InstantiateIfMissing(EventSystemPrefab);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[CaveDeploy] scene setup finished: " + ScenePath);
    }

    static void InstantiateIfMissing(string prefabPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("[CaveDeploy] prefab not found: " + prefabPath);
            return;
        }

        // 已存在则重建，保证使用预制体自身的位置/旋转/缩放（此前误改为原点会导致场景错位）
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name == prefab.name)
            {
                Object.DestroyImmediate(root);
                Debug.Log("[CaveDeploy] rebuilt existing: " + prefab.name);
                break;
            }
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Debug.Log("[CaveDeploy] instantiated: " + prefab.name + " at " + instance.transform.position);
    }

    [MenuItem("CaveDeploy/Build Windows x64")]
    public static void BuildWindows()
    {
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = BuildPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;
        Debug.Log(string.Format("[CaveDeploy] build {0}, size={1}, errors={2}, warnings={3}, output={4}",
            summary.result, summary.totalSize, summary.totalErrors, summary.totalWarnings, summary.outputPath));

        if (summary.result != BuildResult.Succeeded)
        {
            EditorApplication.Exit(1);
        }

        PatchPlayerMonoRuntime();
    }

    // 播放器默认打包的是原版 mono 运行时，SDK 的加密 DLL 需要编辑器里替换过的补丁版 mono 才能解密，
    // 因此构建完成后把编辑器目录下的 mono-2.0-bdwgc.dll 同步到播放器。
    static void PatchPlayerMonoRuntime()
    {
        string source = System.IO.Path.Combine(EditorApplication.applicationContentsPath,
            "MonoBleedingEdge/EmbedRuntime/mono-2.0-bdwgc.dll");
        string target = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(BuildPath),
            "MonoBleedingEdge/EmbedRuntime/mono-2.0-bdwgc.dll");

        if (!System.IO.File.Exists(source) || !System.IO.File.Exists(target))
        {
            Debug.LogError("[CaveDeploy] mono runtime not found, source=" + source + " target=" + target);
            return;
        }

        System.IO.File.Copy(source, target, true);
        Debug.Log("[CaveDeploy] patched player mono runtime: " + target);
    }

    // 批处理入口
    public static void SetupAndBuild()
    {
        try
        {
            SetupScene();
            BuildWindows();
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            EditorApplication.Exit(1);
        }
    }

    // ---------- 玩家移动与手柄射线 ----------

    [MenuItem("CaveDeploy/Setup Player Interaction")]
    public static void SetupPlayerInteraction()
    {
        WirePlayerPrefab();
        AssetDatabase.SaveAssets();
        Debug.Log("[CaveDeploy] player prefab wired: WASD move + handle ray");
    }

    static void WirePlayerPrefab()
    {
        GameObject contents = PrefabUtility.LoadPrefabContents(PlayerPrefab);
        try
        {
            // 1) 自由移动
            LinkXWorkspace.CaveFreeMove mover = contents.GetComponent<LinkXWorkspace.CaveFreeMove>();
            if (mover == null) mover = contents.AddComponent<LinkXWorkspace.CaveFreeMove>();

            Transform head = contents.transform.Find("Head");
            Transform cam = head != null ? head.Find("Main Camera") : null;
            mover.view = cam != null ? cam : head;

            // 2) 手柄射线：SDK 的 CaveForwardPointer 已存在，只差把绘制器接上
            Transform handle = contents.transform.Find("Handle");
            if (handle == null) handle = contents.transform.Find("Head");

            if (handle != null)
            {
                // 未接追踪时把射线抬到视高；VRPN 追踪接入后会覆盖本地位置
                handle.localPosition = new Vector3(0f, 1f, 0f);

                LineRenderer line = handle.gameObject.GetComponent<LineRenderer>();
                if (line == null) line = handle.gameObject.AddComponent<LineRenderer>();
                line.useWorldSpace = true;

                Component pointer = FindComponent(handle.gameObject, "CaveForwardPointer");
                Component drawer = FindComponent(handle.gameObject, "CavePointerDrawer_LineRenderer");

                if (drawer != null)
                {
                    Behaviour behaviour = drawer as Behaviour;
                    if (behaviour != null) behaviour.enabled = true;

                    SerializedObject so = new SerializedObject(drawer);
                    SetObjectReference(so, "m_CavePointer", pointer);
                    SetObjectReference(so, "lineRenderer", line);
                    so.ApplyModifiedPropertiesWithoutUndo();
                }

                if (pointer != null)
                {
                    SerializedObject so = new SerializedObject(pointer);
                    SerializedProperty maxDistance = so.FindProperty("m_MaxDistance");
                    if (maxDistance != null) maxDistance.floatValue = 60f;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            PrefabUtility.SaveAsPrefabAsset(contents, PlayerPrefab);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(contents);
        }
    }

    static Component FindComponent(GameObject go, string typeName)
    {
        Component[] all = go.GetComponents<Component>();
        for (int i = 0; i < all.Length; i++)
            if (all[i] != null && all[i].GetType().Name == typeName) return all[i];
        return null;
    }

    static void SetObjectReference(SerializedObject so, string fieldName, UnityEngine.Object value)
    {
        SerializedProperty property = so.FindProperty(fieldName);
        if (property != null) property.objectReferenceValue = value;
    }

    // 批处理入口：重建场景实例 -> 接入移动与射线 -> 重新打包
    public static void SetupInteractionAndBuild()
    {
        try
        {
            SetupScene();
            WirePlayerPrefab();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
            BuildWindows();
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            EditorApplication.Exit(1);
        }
    }
}
