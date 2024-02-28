using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class Client : MonoBehaviour
{
    Socket clientSocket;
    byte[] buffer = new byte[1024];
    // Start is called before the first frame update
    void Start()
    {
        //Iniciamos o socket do client aqui
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //Conectamos ao localhost
        clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, 12012));

        Debug.Log("Connected to server");
        //Iniciamos o recebimento de mensagens
        clientSocket.BeginReceive(buffer,0,buffer.Length,SocketFlags.None,new AsyncCallback(ReceberMensagem),null);
    }
    private void ReceberMensagem(IAsyncResult ar)
    {
        //(Socket)ar.AsyncState indica que nós queremos o socket que está atrelado a operação assíncrona.
        Socket clientSocket = (Socket)ar.AsyncState;

        //o método EndReceive retorna o número de bytes recebidos pela operação assíncrona.
        int bytesRead = clientSocket.EndReceive(ar);

        //para garantir que apenas os bytes válidos serão utilizados, criamos uma variável
        //data com o número extato de bytes recebidos.
        byte[] data = new byte[bytesRead];

        //Aqui copiamos os bytes recebidos de "buffer" para "data"
        Array.Copy(buffer, data, bytesRead);

        //Fazemos o encoding da mensagem recebida.
        string message = Encoding.ASCII.GetString(data);
        Debug.Log("Mensagem recebida: " + message);

        //Aqui nós chamamos ReceberMensagem novamente, fazendo com que sempre recebamos mensagens.
        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceberMensagem), clientSocket);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            string message = "SpaceKeyPressed";
            byte[] data = Encoding.ASCII.GetBytes(message);
            clientSocket.Send(data);
            Debug.Log("mensagem enviada...");
        }
    }
}
