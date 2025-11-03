using UnityEngine;
using System.Collections;

public class AtributosUnidadeEmTempoReal : MonoBehaviour
{
    public string nome;
    public int vidaMaxima;
    public int vidaAtual;
    public int dano;

    private SpriteRenderer render;
    private Color corOriginal; 

    private void Start()
    {
        vidaAtual = vidaMaxima;
        render = GetComponent<SpriteRenderer>();
        corOriginal = render.color;
    }

    //Função para aplicar dano a unidade
    public void ReceberDano(int danoRecebido)
    {
        // Se já estiver morto, não faz nada
        if (vidaAtual <= 0) return;

        vidaAtual -= danoRecebido;

        // Inicia o "flash" de dano imediatamente
        StartCoroutine(EfeitoDano());

        if (vidaAtual <= 0)
        {
            vidaAtual = 0;
            Morrer();
        }
    }

    public void Morrer()
    {
        // Feedback visual: Muda a cor para cinza para indicar a morte
        render.color = Color.gray;

        // Simplificação de "Game Feel": Desativa o colisor
        // para que o jogador não "tropece" no corpo morto
        GetComponent<Collider2D>().enabled = false;

        // Isto impede que o inimigo morto continue a tentar atacar
        ControladorInimigo ia = GetComponent<ControladorInimigo>();
        if (ia != null)
        {
            ia.enabled = false;
        }

        // Desativa o controle, se for o jogador
        ControladorPersonagem controle = GetComponent<ControladorPersonagem>();
        if (controle != null)
        {
            controle.enabled = false;
        }

    }

    IEnumerator EfeitoDano()
    {
        // O flash vermelho agora vai tocar
        render.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        // Se o inimigo não morreu, volta ao normal
        if (vidaAtual > 0)
        {
            render.color = corOriginal;
        }
    }
}

