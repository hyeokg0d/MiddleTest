#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class TopDownSceneBuilder
{
    const string PrefabDir = "Assets/Prefabs/TopDown";
    const string BulletPath = PrefabDir + "/Bullet.prefab";
    const string EnemyPath = PrefabDir + "/Enemy.prefab";

    [MenuItem("Tools/TopDown Shooter/Build Scene In Editor")]
    public static void Build()
    {
        EnsureFolder(PrefabDir);

        var bulletPrefab = CreateOrLoadBulletPrefab();
        var enemyPrefab = CreateOrLoadEnemyPrefab();

        var ground = Object.FindObjectOfType<GroundTag>();
        if (ground == null)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Plane);
            g.name = "Ground";
            g.transform.position = Vector3.zero;
            g.transform.localScale = new Vector3(4f, 1f, 4f);
            g.AddComponent<GroundTag>();
        }

        var player = Object.FindObjectOfType<PlayerTopDownController>();
        Transform firePoint;
        if (player == null)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Player";
            go.transform.position = new Vector3(0f, 1f, 0f);
            var rb = go.GetComponent<Rigidbody>();
            if (rb == null)
                rb = go.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            var col = go.GetComponent<CapsuleCollider>();
            if (col != null)
                col.height = 2f;

            player = go.AddComponent<PlayerTopDownController>();

            var fp = new GameObject("FirePoint");
            fp.transform.SetParent(go.transform, false);
            fp.transform.localPosition = new Vector3(0f, 0f, 0.75f);
            firePoint = fp.transform;
        }
        else
        {
            firePoint = player.transform.Find("FirePoint");
            if (firePoint == null)
            {
                var fp = new GameObject("FirePoint");
                fp.transform.SetParent(player.transform, false);
                fp.transform.localPosition = new Vector3(0f, 0f, 0.75f);
                firePoint = fp.transform;
            }
        }

        var cam = Camera.main;
        if (cam != null)
        {
            var follow = cam.GetComponent<TopDownCameraFollow>();
            if (follow == null)
                follow = cam.gameObject.AddComponent<TopDownCameraFollow>();
            follow.SetTarget(player.transform);
        }

        player.Configure(firePoint, bulletPrefab, cam, player.transform.position.y);

        var spawner = Object.FindObjectOfType<EnemySpawner>();
        if (spawner == null)
        {
            var sgo = new GameObject("EnemySpawner");
            spawner = sgo.AddComponent<EnemySpawner>();
        }
        spawner.Setup(enemyPrefab, player.transform, 2f, 22f, 0.6f);

        EnsureHudAndGameOver(player);

        GameScore.ResetScore();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        Debug.Log("TopDown: 씬 구성 완료. Play로 실행하세요. WASD 이동, 마우스 조준, Fire1(마우스 왼쪽) 발사.");
    }

    static void EnsureHudAndGameOver(PlayerTopDownController player)
    {
        var hud = Object.FindObjectOfType<ScoreHud>();
        GameObject canvasGo;
        if (hud != null)
            canvasGo = hud.gameObject;
        else
        {
            canvasGo = new GameObject("HUD");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        Text text = null;
        var scoreTextTr = canvasGo.transform.Find("ScoreText");
        if (scoreTextTr != null)
            text = scoreTextTr.GetComponent<Text>();
        if (text == null)
        {
            var textGo = new GameObject("ScoreText");
            textGo.transform.SetParent(canvasGo.transform, false);
            text = textGo.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 28;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;
            var rt = text.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(16f, -16f);
            rt.sizeDelta = new Vector2(400f, 48f);
        }

        if (hud == null)
            hud = canvasGo.AddComponent<ScoreHud>();
        hud.Bind(text);

        var gameOver = canvasGo.GetComponent<GameOverUIController>();
        if (gameOver == null)
            gameOver = canvasGo.AddComponent<GameOverUIController>();

        var panel = canvasGo.transform.Find("GameOverPanel");
        GameObject panelObj;
        if (panel == null)
        {
            panelObj = new GameObject("GameOverPanel");
            panelObj.transform.SetParent(canvasGo.transform, false);
            var panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.72f);
            var prt = panelObj.GetComponent<RectTransform>();
            prt.anchorMin = Vector2.zero;
            prt.anchorMax = Vector2.one;
            prt.offsetMin = Vector2.zero;
            prt.offsetMax = Vector2.zero;

            var titleObj = new GameObject("GameOverText");
            titleObj.transform.SetParent(panelObj.transform, false);
            var title = titleObj.AddComponent<Text>();
            title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            title.fontSize = 52;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = Color.white;
            title.text = "GAME OVER";
            var trt = titleObj.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0.5f, 0.62f);
            trt.anchorMax = new Vector2(0.5f, 0.62f);
            trt.sizeDelta = new Vector2(640f, 120f);
            trt.anchoredPosition = Vector2.zero;
        }
        else
        {
            panelObj = panel.gameObject;
        }

        var restartTr = panelObj.transform.Find("RestartButton");
        Button restart;
        if (restartTr == null)
        {
            var btnObj = new GameObject("RestartButton");
            btnObj.transform.SetParent(panelObj.transform, false);
            var btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(1f, 1f, 1f, 0.95f);
            restart = btnObj.AddComponent<Button>();
            var brt = btnObj.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.5f, 0.42f);
            brt.anchorMax = new Vector2(0.5f, 0.42f);
            brt.sizeDelta = new Vector2(240f, 72f);
            brt.anchoredPosition = Vector2.zero;

            var labelObj = new GameObject("Text");
            labelObj.transform.SetParent(btnObj.transform, false);
            var label = labelObj.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.fontSize = 30;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.black;
            label.text = "다시하기";
            var lrt = labelObj.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
        }
        else
        {
            restart = restartTr.GetComponent<Button>();
        }

        gameOver.Bind(panelObj, restart);
        player.ConfigureGameOver(gameOver);
    }

    static GameObject CreateOrLoadBulletPrefab()
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(BulletPath);
        if (existing != null)
            return existing;

        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Bullet";
        go.transform.localScale = Vector3.one * 0.35f;
        Object.DestroyImmediate(go.GetComponent<Collider>());
        var sc = go.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        go.AddComponent<BulletProjectile>();

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, BulletPath);
        Object.DestroyImmediate(go);
        return prefab;
    }

    static GameObject CreateOrLoadEnemyPrefab()
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyPath);
        if (existing != null)
            return existing;

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Enemy";
        go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        var rend = go.GetComponent<Renderer>();
        if (rend != null && rend.sharedMaterial != null)
        {
            var mat = new Material(rend.sharedMaterial);
            mat.color = new Color(0.85f, 0.2f, 0.2f);
            rend.sharedMaterial = mat;
        }

        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var box = go.GetComponent<BoxCollider>();
        if (box != null)
            box.isTrigger = false;

        go.AddComponent<EnemyHealth>();
        go.AddComponent<EnemyChasePlayer>();

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, EnemyPath);
        Object.DestroyImmediate(go);
        return prefab;
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;
        var parts = path.Split('/');
        string cur = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = cur + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(cur, parts[i]);
            cur = next;
        }
    }
}
#endif
