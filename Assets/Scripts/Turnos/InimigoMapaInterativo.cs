using UnityEngine;
using UnityEngine.UI; // Necessário para interagir com o Botão

public class InimigoMapaInterativo : MonoBehaviour
{
    [Header("Configuração")]
    public float distanciaInteracao = 3f; // A distância de 3 unidades

    [Header("Referências")]
    // Arraste o Gerenciador de Batalha
    public SistemaBatalha sistemaBatalha;

    // Arraste o Botão de "Lutar" da sua UI
    public GameObject botaoLutar;

    // Referência privada ao jogador
    private Transform alvoJogador;

    void Start()
    {
        // Encontra o jogador pela Tag "Player"
        GameObject jogadorObj = GameObject.FindGameObjectWithTag("Player");
        if (jogadorObj != null)
        {
            alvoJogador = jogadorObj.transform;
        }

        // Garante que o botão comece desligado
        if (botaoLutar != null)
        {
            botaoLutar.SetActive(false);

            // Adiciona a função OnBotaoLutarClick ao clique do botão
            botaoLutar.GetComponent<Button>().onClick.AddListener(OnBotaoLutarClick);
        }
    }

    void Update()
    {
        // Se não encontrou o jogador, ou se já estamos em batalha, não faz nada.
        if (alvoJogador == null || sistemaBatalha.estado != SistemaBatalha.EstadoBatalha.OCIOSO)
        {
            // Garante que o botão suma se a batalha começar
            if (botaoLutar.activeSelf)
                botaoLutar.SetActive(false);
            return;
        }

        // Calcula a distância até o jogador
        float distancia = Vector2.Distance(transform.position, alvoJogador.position);

        // Se o jogador está PERTO (dentro das 3 unidades)
        if (distancia <= distanciaInteracao)
        {
            // Mostra o botão (se ele já não estiver visível)
            if (!botaoLutar.activeSelf)
            {
                botaoLutar.SetActive(true);
            }
        }
        // Se o jogador está LONGE
        else
        {
            // Esconde o botão (se ele estiver visível)
            if (botaoLutar.activeSelf)
            {
                botaoLutar.SetActive(false);
            }
        }
    }

    // Esta função é chamada QUANDO o jogador clica no Botão de Lutar
    public void OnBotaoLutarClick()
    {
        // 1. Esconde o botão
        botaoLutar.SetActive(false);

        // 2. Chama o Sistema de Batalha
        //    (O SistemaBatalha.cs vai parar o movimento do jogador)
        sistemaBatalha.IniciarBatalha();

        // 3. Desativa este inimigo do mapa
        gameObject.SetActive(false);
    }

    // Opcional: Desenha o Gizmo para vermos a área no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaInteracao);
    }
}