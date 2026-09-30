using UnityEngine;

namespace FrozenMine
{
    // UI pequeña sin dependencias de fuentes ni paquetes adicionales.
    public sealed class DungeonHUD : MonoBehaviour
    {
        [SerializeField] private FirstPersonLook look;
        [SerializeField] private FirstPersonMotor motor;
        [SerializeField] private Transform player;
        [SerializeField] private MineInteractor interactor;
        [SerializeField] private bool showHints = true;
        private bool completed;
        private GUIStyle title, body, small;
        private readonly Color ice = new Color(0.57f, 0.85f, 0.94f);
        public void CompleteJourney() { completed = true; }
        public void ResetJourney() { completed = false; }

        private void OnGUI()
        {
            if (look == null) return;
            if (title == null)
            {
                title = new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold };
                body = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };
                small = new GUIStyle(GUI.skin.label) { fontSize = 12, wordWrap = true };
                title.normal.textColor = Color.white;
                body.normal.textColor = Color.white;
                small.normal.textColor = ice;
            }
            Matrix4x4 oldMatrix = GUI.matrix;
            Color oldColor = GUI.color;
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            float width = Screen.width / scale;
            float height = Screen.height / scale;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * scale);
            Panel(new Rect(24, 24, 300, 90));
            GUI.Label(new Rect(42, 33, 275, 20), "SECTOR 07 / MINA HELADA", small);
            GUI.Label(new Rect(42, 55, 275, 32), AreaName(), title);
            GUI.Label(new Rect(42, 87, 275, 20), completed ? "RECORRIDO COMPLETADO" : "Encuentra el mirador de la mina", small);

            if (look.InputCaptured)
            {
                GUI.color = new Color(0.85f, 0.96f, 1f, 0.65f);
                GUI.DrawTexture(new Rect(width / 2f - 2, height / 2f - 2, 4, 4), Texture2D.whiteTexture);
                GUI.color = Color.white;
                if (showHints)
                {
                    Panel(new Rect(24, height - 52, 810, 30));
                    GUI.Label(new Rect(38, height - 48, 790, 25), "WASD  Moverte     RATÓN  Mirar     SHIFT  Correr     F  Linterna     E  Activar vagoneta     ESC  Opciones     R  Inicio", small);
                }
                if (interactor != null && !string.IsNullOrEmpty(interactor.Prompt))
                {
                    Panel(new Rect(width / 2 - 160, height / 2 + 40, 320, 42));
                    GUI.Label(new Rect(width / 2 - 142, height / 2 + 48, 290, 30), interactor.Prompt, body);
                }
                if (completed)
                {
                    Panel(new Rect(width / 2 - 220, height - 130, 440, 65));
                    GUI.Label(new Rect(width / 2 - 204, height - 120, 416, 28), "Has llegado al mirador", title);
                    GUI.Label(new Rect(width / 2 - 204, height - 91, 416, 24), "Puedes seguir explorando o pulsar R para volver.", small);
                }
            }
            else
            {
                GUI.color = new Color(0.015f, 0.03f, 0.05f, 0.8f);
                GUI.DrawTexture(new Rect(0, 0, width, height), Texture2D.whiteTexture);
                GUI.color = Color.white;
                Rect rect = new Rect(width / 2 - 220, height / 2 - 175, 440, 350);
                Panel(rect);
                GUILayout.BeginArea(new Rect(rect.x + 28, rect.y + 24, 384, 310));
                GUILayout.Label("MINA HELADA", title);
                GUILayout.Label("Explora a tu ritmo", body);
                GUILayout.Space(18);
                GUILayout.Label("Sensibilidad del ratón", small);
                look.Sensitivity = GUILayout.HorizontalSlider(look.Sensitivity, 0.025f, 0.3f);
                GUILayout.Label("Campo de visión: " + Mathf.RoundToInt(look.FieldOfView) + "°", small);
                look.FieldOfView = GUILayout.HorizontalSlider(look.FieldOfView, 60f, 100f);
                GUILayout.Space(10);
                showHints = GUILayout.Toggle(showHints, "Mostrar controles");
                GUILayout.Space(14);
                if (GUILayout.Button("Continuar / capturar ratón", GUILayout.Height(36))) look.SetCapture(true);
                if (GUILayout.Button("Volver a la entrada", GUILayout.Height(30))) { if (motor != null) motor.Respawn(); look.SetCapture(true); }
                GUILayout.EndArea();
            }
            GUI.matrix = oldMatrix;
            GUI.color = oldColor;
        }
        private string AreaName()
        {
            if (player == null) return "Entrada";
            Vector3 p = player.position;
            if (p.z > 35f) return "El mirador";
            if (p.z > 27f) return "Galería superior";
            if (p.x < -5f) return "Cámara de hielo";
            if (p.x > 8f) return "Almacén";
            if (p.z > 15f) return "Pozo central";
            if (p.z > 7f) return "Galería de acceso";
            return "Entrada";
        }
        private static void Panel(Rect r)
        {
            GUI.color = new Color(0.025f, 0.055f, 0.08f, 0.9f);
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = new Color(0.3f, 0.72f, 0.82f, 0.9f);
            GUI.DrawTexture(new Rect(r.x, r.y, 3, r.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}
