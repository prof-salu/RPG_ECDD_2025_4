using UnityEngine;

public class ControladorPersonagem : MonoBehaviour
{
    //Variaveis de movimento
    public float velocidadeMovimento = 5f;
    private Rigidbody2D rb;
    private Vector2 direcaoMovimento;

    //Atributos do personagem
    private AtributosUnidadeEmTempoReal meusAtributos;

    //Variaveis de ataque
    public Transform pontoDeAtaque;
    public float alcanceAtaque = 0.5f;
    public LayerMask camadaInimigo;
    public float tempoRecargaAtaque = 1f; //1 segundo de recarga
    private float tempoUltimoAtaque = 0f; //Quando o jogador realizou o ultimo ataque
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        meusAtributos = GetComponent<AtributosUnidadeEmTempoReal>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODA CAPTURA DE DADOS DEVE SER FEITA DENTRO DO UPDATE 

        //Pega os inputs do teclado [setas ou WASD]
        direcaoMovimento.x = Input.GetAxisRaw("Horizontal");
        direcaoMovimento.y = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButtonDown(0) && Time.time >= tempoRecargaAtaque + tempoUltimoAtaque)
        {
            Atacar();
            tempoUltimoAtaque = Time.time;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direcaoMovimento.normalized * velocidadeMovimento;
    }

    private void Atacar()
    {
        //Debug.Log("Atacando");

        //Detectar os inimigos
        Collider2D[] inimigosAtingidos = Physics2D.OverlapCircleAll(pontoDeAtaque.position,
                                                                    alcanceAtaque,
                                                                    camadaInimigo);
        //Aplicar o Dano
        foreach(Collider2D inimigo in inimigosAtingidos)
        {
            AtributosUnidadeEmTempoReal atributosInimigos =
                                            inimigo.GetComponent<AtributosUnidadeEmTempoReal>();

            if(atributosInimigos.vidaAtual > 0)
            {
                atributosInimigos.ReceberDano(meusAtributos.dano);
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        //Desenha o Gizmo do alcance de ataque no Editor da Unity apenas
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pontoDeAtaque.position, alcanceAtaque);
    }
}
