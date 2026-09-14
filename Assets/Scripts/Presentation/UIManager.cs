using Rotatris.Data;
using UnityEngine;

namespace Rotatris
{
    /// <summary>
    /// HUD built from TextMesh and SpriteRenderer objects.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private TextMesh scoreText, bestText, holdLabel, nextLabel;

        private GameObject statusPanel;
        private TextMesh statusTitleText;
        private TextMesh statusBodyText;

        public void Init(Camera targetCamera)
        {
            transform.SetParent(targetCamera.transform);
            transform.localPosition = new Vector3(0f, 0f, 10f);
            transform.localRotation = Quaternion.identity;

            scoreText = MakeLabel("ScoreText", new Vector2(0f, 4.2f), 0.6f, TextAnchor.MiddleCenter, TextAlignment.Center);
            bestText = MakeLabel("BestText", new Vector2(0f, 3.5f), 0.5f, TextAnchor.MiddleCenter, TextAlignment.Center);

            holdLabel = MakeLabel("HoldLabel", new Vector2(-4.5f, 3.5f), 0.5f, TextAnchor.MiddleLeft, TextAlignment.Left);
            nextLabel = MakeLabel("NextLabel", new Vector2(4.5f, 3.5f), 0.5f, TextAnchor.MiddleRight, TextAlignment.Right);
            nextLabel.text = "NEXT";

            BuildStatusPanel();
        }

        private void BuildStatusPanel()
        {
            statusPanel = new GameObject("StatusPanel");
            statusPanel.transform.SetParent(transform);

            statusPanel.transform.localPosition = new Vector3(0f, -530f, -0.5f);

            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(statusPanel.transform);
            bgGO.transform.localPosition = Vector3.zero;
            bgGO.transform.localScale = new Vector3(7.5f, 4f, 1f);

            var spriteRenderer = bgGO.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateQuadSprite();
            spriteRenderer.color = new Color(0.02f, 0.03f, 0.05f, 0.92f);
            spriteRenderer.sortingOrder = 10;

            statusTitleText = MakeLabel("TitleText", new Vector2(0f, 1f), 0.8f, TextAnchor.MiddleCenter, TextAlignment.Center);
            statusTitleText.transform.SetParent(statusPanel.transform);
            statusTitleText.color = new Color(1f, 0.35f, 0.35f);
            statusTitleText.GetComponent<MeshRenderer>().sortingOrder = 11;

            statusBodyText = MakeLabel("BodyText", new Vector2(0f, -0.4f), 0.5f, TextAnchor.MiddleCenter, TextAlignment.Center);
            statusBodyText.transform.SetParent(statusPanel.transform);
            statusBodyText.color = Color.white;
            statusBodyText.GetComponent<MeshRenderer>().sortingOrder = 11;

            statusPanel.SetActive(false);
        }

        public void SetScore(int score) => scoreText.text = $"Score: {score}";
        public void SetBest(int best) => bestText.text = $"Best: {best}";
        public void SetHold(PieceType? hold) => holdLabel.text = hold.HasValue ? $"Hold [{hold.Value}]" : "Hold [ ]";

        public void ShowStatus(string title, string body = "")
        {
            statusTitleText.text = title;
            statusBodyText.text = body;
            statusPanel.SetActive(true);
        }

        public void HideStatus() => statusPanel.SetActive(false);

        private TextMesh MakeLabel(string name, Vector2 pos, float size, TextAnchor anchor, TextAlignment alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            go.transform.localPosition = new(pos.x, pos.y, 0f);

            var tm = go.AddComponent<TextMesh>();
            tm.fontSize = 48;
            tm.characterSize = size * 0.2f;
            tm.color = Color.white;
            tm.anchor = anchor;
            tm.alignment = alignment;

            var mr = go.GetComponent<MeshRenderer>();
            mr.sortingOrder = 5;
            return tm;
        }

        private Sprite CreateQuadSprite()
        {
            Texture2D tex = new(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }
    }
}