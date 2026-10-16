/*
 * Descripcion: Efecto visual simple para marcar los objetos interactuables.
 * Dibuja un halo en el piso que late y unos destellos que suben y titilan.
 * No necesita sprites: la textura se genera por codigo.
 * Se apaga cuando el minijuego del Interactuable ya fue completado.
 */

using UnityEngine;

public class BrilloInteractuable : MonoBehaviour
{
    [SerializeField] private Color color = new Color(1f, 0.85f, 0.3f, 1f);
    [SerializeField] private float tamanoHalo = 1.5f;
    [SerializeField] private float velocidadLatido = 2.5f;
    [SerializeField] private int cantidadDestellos = 4;
    [SerializeField] private int ordenCapa = 5;

    private static Sprite spriteSuave;

    private Interactuable interactuable;
    private GameObject brillo;
    private SpriteRenderer halo;
    private Transform[] destellos;
    private SpriteRenderer[] rendererDestellos;
    private float[] faseDestellos;

    private void Start()
    {
        interactuable = GetComponent<Interactuable>();

        brillo = new GameObject("Brillo");
        brillo.transform.SetParent(transform, false);

        halo = CrearSprite("Halo", ordenCapa);

        destellos = new Transform[cantidadDestellos];
        rendererDestellos = new SpriteRenderer[cantidadDestellos];
        faseDestellos = new float[cantidadDestellos];
        for (int i = 0; i < cantidadDestellos; i++)
        {
            rendererDestellos[i] = CrearSprite("Destello", ordenCapa + 1);
            destellos[i] = rendererDestellos[i].transform;
            faseDestellos[i] = (float)i / cantidadDestellos;
            ReubicarDestello(i);
        }
    }

    private void Update()
    {
        bool visible = interactuable == null || !interactuable.Completado;
        if (brillo.activeSelf != visible) brillo.SetActive(visible);
        if (!visible) return;

        float latido = (Mathf.Sin(Time.time * velocidadLatido) + 1f) / 2f;
        float escala = tamanoHalo * (0.9f + 0.15f * latido);
        halo.transform.localScale = new Vector3(escala, escala * 0.5f, 1f);
        halo.color = ConAlfa(0.35f + 0.3f * latido);

        for (int i = 0; i < destellos.Length; i++)
        {
            faseDestellos[i] += Time.deltaTime * 0.6f;
            if (faseDestellos[i] >= 1f)
            {
                faseDestellos[i] -= 1f;
                ReubicarDestello(i);
            }

            float t = faseDestellos[i];
            destellos[i].localPosition += Vector3.up * Time.deltaTime * 0.4f;
            destellos[i].localScale = Vector3.one * 0.18f * Mathf.Sin(t * Mathf.PI);
            rendererDestellos[i].color = ConAlfa(Mathf.Sin(t * Mathf.PI));
        }
    }

    private void ReubicarDestello(int i)
    {
        Vector2 punto = Random.insideUnitCircle * tamanoHalo * 0.4f;
        destellos[i].localPosition = new Vector3(punto.x, punto.y * 0.5f, 0f);
    }

    private SpriteRenderer CrearSprite(string nombre, int orden)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(brillo.transform, false);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = ObtenerSpriteSuave();
        sr.sortingOrder = orden;
        sr.color = color;
        return sr;
    }

    private Color ConAlfa(float alfa)
    {
        Color c = color;
        c.a = color.a * alfa;
        return c;
    }

    private static Sprite ObtenerSpriteSuave()
    {
        if (spriteSuave != null) return spriteSuave;

        const int lado = 64;
        Texture2D tex = new Texture2D(lado, lado, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        Vector2 centro = new Vector2(lado - 1, lado - 1) / 2f;
        for (int y = 0; y < lado; y++)
        {
            for (int x = 0; x < lado; x++)
            {
                float distancia = Vector2.Distance(new Vector2(x, y), centro) / (lado / 2f);
                float alfa = Mathf.Clamp01(1f - distancia);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alfa * alfa));
            }
        }
        tex.Apply();

        spriteSuave = Sprite.Create(tex, new Rect(0, 0, lado, lado), new Vector2(0.5f, 0.5f), lado);
        return spriteSuave;
    }
}
