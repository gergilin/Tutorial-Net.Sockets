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

        //O m�todo Bind especifica o ip e port onde o servidor ser� iniciado.
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, 12012));

        //Com o listen, n�s dizemos quantas pessoas v�o poder acessar o servidor ao mesmo tempo.
        serverSocket.Listen(numerodePlayers);

        //Com o BeginAccept, come�amos a aceitar as conex�es.
        //Repare que AceitarConexao � iniciada sem par�metro, pois o sistema vai gerar um par�metro
        //e inclu�-lo automaticamente a fun��o.
        serverSocket.BeginAccept(new AsyncCallback(AceitarConexao), null);
        Debug.Log($"Servidor iniciado!");
    }
    private void AceitarConexao(IAsyncResult ar)
    {
        //o m�todo EndAccept conclui a conex�o e retorna o socket do cliente.
        Socket clientSocket = serverSocket.EndAccept(ar);
        Debug.Log("Cliente Conectado" + clientSocket.RemoteEndPoint);

        //o m�todo Beginreceive inicia a recep��o de dados pelo servidor do cliente.
        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceberMensagem), clientSocket);
        serverSocket.BeginAccept(new AsyncCallback(AceitarConexao), null);
    }
    private void ReceberMensagem(IAsyncResult ar)
    {
        //(Socket)ar.AsyncState indica que n�s queremos o socket que est� atrelado a opera��o ass�ncrona.
        Socket clientSocket = (Socket)ar.AsyncState;

        //o m�todo EndReceive retorna o n�mero de bytes recebidos pela opera��o ass�ncrona.
        int bytesRead = clientSocket.EndReceive(ar);
        if (bytesRead > 0)
        {
            //para garantir que apenas os bytes v�lidos ser�o utilizados, criamos uma vari�vel
            //data com o n�mero extato de bytes recebidos.
            byte[] data = new byte[bytesRead];

            //Aqui copiamos os bytes recebidos de "buffer" para "data"
            Array.Copy(buffer, data, bytesRead);

            //Fazemos o encoding da mensagem recebida.
            string message = Encoding.ASCII.GetString(data);
            Debug.Log("Mensagem recebida: " + message);

            //Aqui n�s chamamos ReceberMensagem novamente, fazendo com que sempre recebamos mensagens.
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceberMensagem), clientSocket);
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}
