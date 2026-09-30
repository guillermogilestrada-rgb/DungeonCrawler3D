#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using FrozenMine;

public static class DungeonSceneTools
{
    [MenuItem("Dungeon/Abrir mina helada")]
    private static void OpenMine()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene("Assets/Scenes/Dungeon_FrozenMine.unity");
    }

    [MenuItem("Dungeon/Comprobar escena abierta")]
    private static void ValidateScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        int issues = 0;
        int prefabCount = 0;
        int cameras = 0;
        int listeners = 0;
        FirstPersonMotor player = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject) > 0)
                { Debug.LogError("Script ausente: " + t.name, t); issues++; }
                if (PrefabUtility.IsAnyPrefabInstanceRoot(t.gameObject)) prefabCount++;
                MeshFilter filter = t.GetComponent<MeshFilter>();
                if (filter != null && filter.sharedMesh == null)
                { Debug.LogError("Malla ausente: " + t.name, t); issues++; }
                Renderer renderer = t.GetComponent<Renderer>();
                if (renderer != null)
                    foreach (Material material in renderer.sharedMaterials)
                        if (material == null || material.shader == null || !material.shader.isSupported)
                        { Debug.LogError("Material o shader inválido: " + t.name, t); issues++; }
                if (t.gameObject.activeInHierarchy)
                {
                    Camera camera = t.GetComponent<Camera>();
                    if (camera != null && camera.enabled) cameras++;
                    AudioListener listener = t.GetComponent<AudioListener>();
                    if (listener != null && listener.enabled) listeners++;
                    if (t.TryGetComponent(out FirstPersonMotor motor)) player = motor;
                }
            }
        }
        if (cameras != 1) { Debug.LogError("Se esperaba una cámara activa; hay " + cameras); issues++; }
        if (listeners != 1) { Debug.LogError("Se esperaba un AudioListener activo; hay " + listeners); issues++; }
        if (player == null) { Debug.LogError("Falta FirstPersonMotor en la escena abierta."); issues++; }
        else
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc == null || !cc.enabled) { Debug.LogError("Falta activar CharacterController.", player); issues++; }
            if (player.GetComponent<Rigidbody>() != null)
            { Debug.LogError("El nuevo jugador no debe combinar Rigidbody con CharacterController.", player); issues++; }
            Physics.SyncTransforms();
            if (!Physics.Raycast(player.transform.position + Vector3.up * .1f, Vector3.down, 1f, ~0, QueryTriggerInteraction.Ignore))
            { Debug.LogWarning("Comprueba el suelo bajo el jugador.", player); issues++; }
        }
        Debug.Log("Revisión de " + scene.name + ": " + issues + " incidencias; " + prefabCount + " instancias de prefab. " +
                  "Falta comprobar el movimiento en Play: esta revisión no sustituye esa prueba.");
    }
}
#endif
