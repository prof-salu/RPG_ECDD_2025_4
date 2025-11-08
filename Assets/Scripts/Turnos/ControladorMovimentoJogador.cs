using UnityEngine;

public class ControladorMovimentoJogador : MonoBehaviour
{
    public float velocidadeMovimento = 5f;

    private Vector2 posicaoAlvo;
    private Camera cam;

    void Start()
    {
        // Pega a câmera principal
        cam = Camera.main;

        // Define a posição alvo inicial para a própria posição do jogador,
        // assim ele não se move quando o jogo começa.
        posicaoAlvo = transform.position;
    }

    void Update()
    {
        // 1. Detectar o clique do mouse
        // Input.GetMouseButtonDown(0) detecta o clique do botão esquerdo
        if (Input.GetMouseButtonDown(0))
        {
            // 2. Converter a posição do mouse (pixels da tela) 
            // para uma posição no mundo do jogo (world point)
            // É crucial que sua câmera principal tenha a tag "MainCamera"
            posicaoAlvo = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        // 3. Mover o jogador em direção ao alvo
        // Vector2.MoveTowards move suavemente de um ponto a outro
        // em uma velocidade constante.
        transform.position = Vector2.MoveTowards(
            transform.position, // Posição atual
            posicaoAlvo,        // Alvo para onde ir
            velocidadeMovimento * Time.deltaTime // Velocidade
        );
    }
}