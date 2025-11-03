using UnityEngine;
using System.Collections; // Necessário para Corrotinas

// Atributos [RequireComponent] removidos para simplificar

public class ControladorInimigo : MonoBehaviour // Nome da classe alterado
{
    // Header agrupa variáveis no Inspector da Unity
    [Header("Atributos de Movimento")]
    public float velocidadeMovimento = 2.5f;

    [Header("Atributos de Combate")]
    public float alcanceAtaque = 1.5f;        // Distância para parar e atacar
    public float tempoSinalizacao = 0.7f;     // Tempo em amarelo antes do golpe
    public float tempoRecargaAtaque = 2f;     // Cooldown entre ataques
    public Color corSinalizacao = Color.yellow; // Cor do "telegraph"

    // Componentes e Referências (privados, não aparecem no Inspector)
    private Transform alvo;
    private Rigidbody2D rb;
    private AtributosUnidadeEmTempoReal meusAtributos;
    private AtributosUnidadeEmTempoReal atributosAlvo;
    private SpriteRenderer render;
    private Color corOriginal;

    // Variáveis de Controle de Estado
    private bool podeAtacar = true;     // Controla a recarga (cooldown)
    private bool estaAtacando = false;  // "Trava" para não perseguir enquanto ataca

    void Start()
    {
        // Pega os componentes no início do jogo
        rb = GetComponent<Rigidbody2D>();
        meusAtributos = GetComponent<AtributosUnidadeEmTempoReal>();
        render = GetComponent<SpriteRenderer>();
        corOriginal = render.color;

        // Encontra o jogador (alvo) automaticamente
        ControladorPersonagem jogador = FindFirstObjectByType<ControladorPersonagem>();
        if (jogador != null)
        {
            alvo = jogador.transform;
            atributosAlvo = alvo.GetComponent<AtributosUnidadeEmTempoReal>();
        }
    }

    void Update()
    {
        // --- LÓGICA DE DECISÃO (Quando atacar) ---
        // Update é ótimo para checagens rápidas e decisões

        // Se não tem alvo ou já está atacando, não faz nada
        if (alvo == null || estaAtacando)
        {
            return;
        }

        // Calcula a distância até o jogador
        float distancia = Vector2.Distance(transform.position, alvo.position);

        // Se está no alcance E pode atacar
        if (distancia <= alcanceAtaque && podeAtacar)
        {
            StartCoroutine(Atacar());
        }
    }

    void FixedUpdate()
    {
        // --- LÓGICA DE MOVIMENTO (Física) ---
        // FixedUpdate é o local correto para manipular o Rigidbody

        // Se está atacando, ou não tem alvo, para de se mover.
        if (alvo == null || estaAtacando)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distancia = Vector2.Distance(transform.position, alvo.position);

        // Se está FORA do alcance, persegue.
        if (distancia > alcanceAtaque)
        {
            // Calcula a direção normalizada (vetor de tamanho 1)
            Vector2 direcao = (alvo.position - transform.position).normalized;
            // Define a velocidade do Rigidbody
            rb.linearVelocity = direcao * velocidadeMovimento;
        }
        else // Se está DENTRO do alcance, para.
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Corrotina para a sequência de ataque (com sinalização)
    IEnumerator Atacar()
    {
        // 1. INICIA O ESTADO DE ATAQUE
        podeAtacar = false;
        estaAtacando = true; // Trava o movimento no FixedUpdate
        rb.linearVelocity = Vector2.zero; // Garante que parou

        // 2. SINALIZAÇÃO (Telegraphing)
        render.color = corSinalizacao;
        yield return new WaitForSeconds(tempoSinalizacao); // Espera

        // 3. EXECUÇÃO DO ATAQUE (Com "Comprometimento")
        // O ataque termina, mas o DANO só é aplicado se o jogador ainda
        // estiver no alcance, como solicitado.
        float distanciaParaAtaque = Vector2.Distance(transform.position, alvo.position);
        if (distanciaParaAtaque <= alcanceAtaque && atributosAlvo.vidaAtual > 0)
        {
            // Causa dano no jogador
            atributosAlvo.ReceberDano(meusAtributos.dano);
        }
        // Se o jogador saiu do alcance: o ataque "erra" (não causa dano).

        // 4. RESFRIAMENTO (Cooldown)
        render.color = corOriginal; // Volta à cor normal
        yield return new WaitForSeconds(tempoRecargaAtaque);

        // 5. RESET DO ESTADO
        estaAtacando = false; // Destrava o movimento
        podeAtacar = true;    // Permite um novo ataque
    }

    // Desenha o Gizmo do alcance de ataque no Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // Define a cor do Gizmo
        // Desenha uma esfera de arame na posição do inimigo com o raio do ataque
        Gizmos.DrawWireSphere(transform.position, alcanceAtaque);
    }
}
