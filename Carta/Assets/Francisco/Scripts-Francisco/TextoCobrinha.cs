
using UnityEngine;
using TMPro;

public class TextoCobrinha : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TMP_Text texto;

    [Header("Animacao")]
    [SerializeField] private float velocidade = 3f;
    [SerializeField] private float altura = 15f;

    [Header("Distancia da Onda")]
    [SerializeField] private float comprimentoOnda = 4f;

    private TMP_MeshInfo[] verticesOriginais;

    void OnEnable()
    {
        PrepararTexto();
    }

    void PrepararTexto()
    {
        if (texto == null)
            texto = GetComponent<TMP_Text>();

        if (texto == null)
            return;

        texto.ForceMeshUpdate();

        verticesOriginais =
            texto.textInfo.CopyMeshInfoVertexData();
    }

    void LateUpdate()
    {
        if (texto == null || verticesOriginais == null)
            return;

        texto.ForceMeshUpdate();

        TMP_TextInfo textInfo = texto.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo character = textInfo.characterInfo[i];

            if (!character.isVisible)
                continue;

            int materialIndex = character.materialReferenceIndex;
            int vertexIndex = character.vertexIndex;

            Vector3[] vertices =
                textInfo.meshInfo[materialIndex].vertices;

            Vector3[] originais =
                verticesOriginais[materialIndex].vertices;

            // Calcula a altura da letra.
            float fase = (float)i / Mathf.Max(0.01f, comprimentoOnda);

            float movimento = Mathf.Sin(
                (fase * Mathf.PI * 2f) +
                (Time.time * velocidade)
            ) * altura;

            // Aplica o movimento nos quatro vertices da letra.
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] =
                    originais[vertexIndex + j] +
                    new Vector3(0, movimento, 0);
            }
        }

        texto.UpdateVertexData(
            TMP_VertexDataUpdateFlags.Vertices
        );
    }
}

