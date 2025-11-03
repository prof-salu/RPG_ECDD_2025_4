using System.Collections;
using TMPro;
using UnityEngine;

public class SistemaBatalha : MonoBehaviour
{
    //Enum -> define os possiveis estados de uma batalha
    public enum EstadoBatalha { INICIO, TURNO_JOGADOR, TURNO_INIMIGO, VITORIA, DERROTA}

    public EstadoBatalha estado;

    //Referencias do jogo
    public GameObject jogador;
    public GameObject inimigo;

    private AtributosUnidade atributosJogador;
    private AtributosUnidade atributosInimigo;

    public Canvas canvasPrincipal;
    public GameObject textoDano;

    private void Start()
    {
        estado = EstadoBatalha.INICIO;
        ConfigurarBatalha();
    }

    void ConfigurarBatalha()
    {
        atributosJogador = jogador.GetComponent<AtributosUnidade>();
        atributosInimigo = inimigo.GetComponent<AtributosUnidade>();

        estado = EstadoBatalha.TURNO_JOGADOR;
    }

    public void BotaoAtaque01()
    {
        if (estado != EstadoBatalha.TURNO_JOGADOR)
        {
            //Ignora o pressionar do botão
            return;
        }

        StartCoroutine(TurnoJogador());
    }

    IEnumerator TurnoJogador()
    {
        yield return StartCoroutine(ExecutarSequenciaDeAtaque(atributosJogador, atributosInimigo));

        //Verifica se o inimigo foi derrotado
        if (atributosInimigo.vidaAtual <= 0)
        {
            //Altera o estado para vitoria do JOGADOR
            estado = EstadoBatalha.VITORIA;
        }
        else
        {
            //Altera o estado para TURNO do INIMIGO (ações do inimigo)
            estado = EstadoBatalha.TURNO_INIMIGO;
            //Inicia a rotina do inimigo
            StartCoroutine(TurnoInimigo());
        }
    }

    IEnumerator TurnoInimigo()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ExecutarSequenciaDeAtaque(atributosInimigo, atributosJogador));

        if(atributosJogador.vidaAtual <= 0)
        {
            estado = EstadoBatalha.DERROTA;
        }
        else
        {
            estado = EstadoBatalha.TURNO_JOGADOR;
        }
    }

    IEnumerator ExecutarSequenciaDeAtaque(AtributosUnidade atacante, AtributosUnidade alvo)
    {
        //Guarda a posição inicial dos personagens
        Vector3 posicaoOriginal = atacante.transform.position;
        Vector3 posicaoAlvo = alvo.transform.position;

        //Animação de salto
        //Vector3.Distance --> Calcula a distancia entre 2 gameobjects
        while (Vector3.Distance(atacante.transform.position, posicaoAlvo) > 1.5f)
        {
            //Vecotr3.MoveTowards --> Move um gameobject na direção do alvo, a uma velocidade fixa;
            atacante.transform.position = Vector3.MoveTowards(atacante.transform.position, 
                                                              posicaoAlvo,
                                                              20f * Time.deltaTime);
            yield return null;
        }

        //Executa o ataque
        alvo.ReceberDano(atacante.dano);
        MostrarTextoDano(atacante.dano, alvo.transform);

        //Animação de retorno a posição original
        while(Vector3.Distance(atacante.transform.position, posicaoOriginal) > 0.01f)
        {
            atacante.transform.position = Vector3.MoveTowards(atacante.transform.position,
                                                              posicaoOriginal,
                                                              20f * Time.deltaTime);
            yield return null;
        }
        
        atacante.transform.position = posicaoOriginal;
    }

    void MostrarTextoDano(int dano, Transform alvo)
    {
        //Cria um novo TEXTODANO e o posiciona no canvas principal
        GameObject texto = Instantiate(textoDano, canvasPrincipal.transform);

        texto.GetComponent<TextMeshProUGUI>().text = dano.ToString() + " de dano";
        texto.transform.position = Camera.main.WorldToScreenPoint(alvo.position);
    }

}
