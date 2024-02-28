using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Server : MonoBehaviour
{
    Socket serverSocket;
    public int numerodePlayers = 1;
    byte[] buffer = new byte[1024];

    void Start()
    {
        Debug.Log($"Servidor iniciando em localhost:{12012} ...");
        //Iniciamos o socket do servidor aqui
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //O método Bind especifica o ip e port onde o servidor será iniciado.
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, 12012));

        //Com o listen, nós dizemos quantas pessoas vão poder acessar o servidor ao mesmo tempo.
        serverSocket.Listen(numerodePlayers);

        //Com o BeginAccept, começamos a aceitar as conexões.
        //Repare que AceitarConexao é iniciada sem parâmetro, pois o sistema vai gerar um parâmetro
        //e incluí-lo automaticamente a função.
        serverSocket.BeginAccept(new AsyncCallback(AceitarConexao), null);
        Debug.Log($"Servidor iniciado!");
    }
    private void AceitarConexao(IAsyncResult ar)
    {
        //o método EndAccept conclui a conexão e retorna o socket do cliente.
        Socket clientSocket = serverSocket.EndAccept(ar);
        Debug.Log("Cliente Conectado" + clientSocket.RemoteEndPoint);

        //o método Beginreceive inicia a recepção de dados pelo servidor do cliente.
        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceberMensagem), clientSocket);
        serverSocket.BeginAccept(new AsyncCallback(AceitarConexao), null);
    }
    private void ReceberMensagem(IAsyncResult ar)
    {
        //(Socket)ar.AsyncState indica que nós queremos o socket que está atrelado a operação assíncrona.
        Socket clientSocket = (Socket)ar.AsyncState;

        //o método EndReceive retorna o número de bytes recebidos pela operação assíncrona.
        int bytesRead = clientSocket.EndReceive(ar);
        if (bytesRead > 0)
        {
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
    }
    // Update is called once per frame
    void Update()
    {
    }
}
