using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensibilidade = 2.0f; //Controla a sensibilidade do mouse

    private float mouseX = 0.0f, mouseY = 0.0f; //Variáveis que controla a rotação do mouse

    enum controlOption { mouse };
    [SerializeField]
    controlOption control;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false; //Oculta o cursor do mouse
    }

    // Update is called once per frame
    void Update()
    {
        if (control == controlOption.mouse)
        {
            if (Input.GetMouseButton(1))
            {
                mouseX += Input.GetAxis("Mouse X") * sensibilidade; // Incrementa o valor do eixo X e multiplica pela sensibilidade
                mouseY -= Input.GetAxis("Mouse Y") * sensibilidade; // Incrementa o valor do eixo Y e multiplica pela sensibilidade. (Obs. usamos o - para inverter os valores)

                gameObject.transform.eulerAngles = new Vector3(mouseY, mouseX, 0); //Executa a rotação da câmera de acordo com os eixos
            }
        }
    }
}
