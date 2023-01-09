using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class MovePersonagem : MonoBehaviour

{

    public GameObject particulaOvo;
    public GameObject particulaPenao;
    public GameObject particulaEstrela;
    public GameObject objetoParticulaFogo;

    public AudioClip somOvo;
    public AudioClip somPena;
    public AudioClip somEstrela;
    public AudioClip somHit;
    public AudioClip somWin;
    public AudioClip somLose;
    public AudioClip somApareceStar;
    public AudioClip somFelpudoVoa;

    CharacterController objetoCharController;
    float velocidade = 5.0f;
    float giro = 3.0f;
    float frente = 3.0f;
    float pulo = 5.0f;
    Vector3 vetorDirecao = new Vector3(0, 0, 0);
    Vector3 moveCameraFrente;
    Vector3 moveMove;
    Vector3 normalZeroPiso = new Vector3(0, 0, 0);
    Transform transformCamera;

    float contaPisca = 0;
    bool podePegarStar;

    public GameObject jogador;
    public Animation aniacao;
    [SerializeField] private int numerObjetos;

    void Start()
    {
        objetoCharController = GetComponent<CharacterController>();
        aniacao = jogador.GetComponent<Animation>();
        transformCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Movimentacao();
         aplicaRotacao();

         Animacao();
       
    }

    private void Animacao()
    {
        if (CrossPlatformInputManager.GetButton("Jump"))
        {
            if (objetoCharController.isGrounded == true)
            {
                vetorDirecao.y = pulo;
                jogador.GetComponent<Animation>().Play("Jump");
                GetComponent<AudioSource>().PlayOneShot(somFelpudoVoa, 0.7f);
            }
        }
        else
        {
           // if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
           if((CrossPlatformInputManager.GetAxis("Horizontal")!= 0.0f) || (CrossPlatformInputManager.GetAxis("Vertical") != 0.0f))
            {
                if (!aniacao.IsPlaying("Jump"))
                {
                    jogador.GetComponent<Animation>().Play("Walk");
                }
            }
            else
            {
                if (objetoCharController.isGrounded == true)
                {
                    jogador.GetComponent<Animation>().Play("IDLE");
                }
            }
        }
    }

    private void Movimentacao()
    {
        moveCameraFrente = Vector3.Scale(transformCamera.forward, new Vector3(1, 0, 1)).normalized;
        moveMove = CrossPlatformInputManager.GetAxis("Vertical") * moveCameraFrente + CrossPlatformInputManager.GetAxis("Horizontal") * transformCamera.right;

        vetorDirecao.y -= 3.0f * Time.deltaTime;
        objetoCharController.Move(vetorDirecao * Time.deltaTime);
        objetoCharController.Move(moveMove * velocidade * Time.deltaTime);

        if (moveMove.magnitude > 1f) moveMove.Normalize();
        moveMove = transform.InverseTransformDirection(moveMove);

        moveMove = Vector3.ProjectOnPlane(moveMove, normalZeroPiso);
        giro = Mathf.Atan2(moveMove.x, moveMove.y);
        frente = moveMove.z;

        objetoCharController.SimpleMove(Physics.gravity);
    }

    void aplicaRotacao()
    {
        float mudarSpeed = Mathf.Lerp(180, 360, frente);
        transform.Rotate(0, giro * mudarSpeed * Time.deltaTime, 0);
    }


    void Teste()
    {
       /* Vector3 forward = Input.GetAxis("Vertical") * transform.TransformDirection(Vector3.forward) * velocidade;
        transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * giro * Time.deltaTime, 0));

        objetoCharController.Move(forward * Time.deltaTime);
        objetoCharController.SimpleMove(Physics.gravity);

        if (Input.GetButton("Jump"))
        {
            if (objetoCharController.isGrounded == true)
            {
                vetorDirecao.y = pulo;
                jogador.GetComponent<Animation>().Play("Jump");
            }
        }
        else
        {
            if(Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                if (!aniacao.IsPlaying("Jump"))
                {
                    jogador.GetComponent<Animation>().Play("Walk");
                }
            }
            else
            {
                if(objetoCharController.isGrounded == true)
                {
                    jogador.GetComponent<Animation>().Play("IDLE");
                }
            }
        }
        vetorDirecao.y -= 3.0f * Time.deltaTime;
        objetoCharController.Move(vetorDirecao * Time.deltaTime);*/
    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "OVO")
        {
            Instantiate(particulaOvo, other.gameObject.transform.position, Quaternion.identity);
            other.gameObject.SetActive(false);
            numerObjetos++; verificaPickObjetos();
            GetComponent<AudioSource>().PlayOneShot(somOvo, 0.7f);
        }
        if (other.gameObject.tag == "PENA")
        {
            Instantiate(particulaPenao, other.gameObject.transform.position, Quaternion.identity);
            other.gameObject.SetActive(false);
            numerObjetos++; verificaPickObjetos();
            GetComponent<AudioSource>().PlayOneShot(somPena, 0.7f);
        }
        if (other.gameObject.tag == "ESTRELA")
        {
            if (podePegarStar)
            {
                Instantiate(particulaEstrela, other.gameObject.transform.position, Quaternion.identity);
                other.gameObject.SetActive(false);
                Invoke("carregaFase", 3);
                GetComponent<AudioSource>().PlayOneShot(somEstrela, 0.7f);
            }
        }

        if (other.gameObject.tag == "FOGUEIRA")
        {
            InvokeRepeating("mudaEstadoFulpudo", 0, 0.1f);
            objetoCharController.Move(transform.TransformDirection(Vector3.back) * 3);
            GetComponent<AudioSource>().PlayOneShot(somHit, 0.7f);

        }

        if (other.gameObject.tag == "BURACO")
        {
            Invoke("carregaFase", 1.5f);
            GetComponent<AudioSource>().PlayOneShot(somLose, 0.7f);

        }
    }
    void mudaEstadoFulpudo()
    {
        contaPisca++;
        jogador.SetActive(!jogador.activeInHierarchy);
        if (contaPisca > 7)
        {
            contaPisca = 0;jogador.SetActive(true); CancelInvoke();
        }

    }

    void verificaPickObjetos()
    {
        if(numerObjetos > 15)
        {
            podePegarStar = true;
            Destroy(objetoParticulaFogo);
            GetComponent<AudioSource>().PlayOneShot(somApareceStar, 0.7f);

        }
    }

    void carregaFase()
    {

        SceneManager.LoadScene(0);

    }
}
