using System.Collections;
using UnityEngine;

public class AtributosUnidade : MonoBehaviour
{
    //Comentario
    public string nome;    //Alfanumerico
    public int vidaMaxima; //Inteiro
    public int vidaAtual;  //Inteiro
    public int dano;       //Inteiro

    private SpriteRenderer render;
    private Color corOrignal;

    private void Start()
    {
        vidaAtual = vidaMaxima;
        render = GetComponent<SpriteRenderer>(); //capturao componente SpriteRender
        corOrignal = render.color;//Captura a cor orignal
    }

    //Função para aplicar dano a unidade
    public void ReceberDano(int danoRecebido)
    {
        //vidaAtual = vidaAtual - danoRecebido;
        vidaAtual -= danoRecebido;

        if (vidaAtual <= 0) 
        {
            vidaAtual = 0;
            Morrer();
        }
        //Inicializa a corotina EfeitoDano
        StartCoroutine(EfeitoDano());
    }

    public void Morrer()
    {
        //Altera a cor para cinza
        render.color = Color.gray;
    }

    IEnumerator EfeitoDano()
    {
        if(vidaAtual > 0)
        {
            render.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            render.color = corOrignal;
        }
    }
}