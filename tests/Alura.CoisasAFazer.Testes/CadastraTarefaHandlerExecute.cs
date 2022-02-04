using System;
using System.Linq;
using CoisasAFazer.Core.Commands;
using CoisasAFazer.Core.Models;
using CoisasAFazer.Infrastructure;
using CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

namespace Alura.CoisasAFazer.Testes;

public class CadastraTarefaHandlerExecute
{
    [Fact]
    public void DadaTarefaComInfoValidaIncluirNoBD()
    {
        // Given

        var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), new DateTime(2022, 2, 2));

        var mock = new Mock<ILogger<CadastraTarefaHandler>>();
        // Aqui estamos configurando um Fake Object, estes são utilizados para simular recursos de forma rápida e leve, neste caso
        // estamos a simular um Banco de Dados utilizando o "InMemory" que simula um banco de dados na memória, dessa forma não 
        // temos problemas com demoras nos testes alem de que os testes não entram em conflito com o banco de produção. 
        // Lembrando que ao utilizarmos do "InMemory" para testes é necessário que algumas mudanças sejam feitas na declaração do contexto

        var options = new DbContextOptionsBuilder<DbTarefasContext>()
            .UseInMemoryDatabase("DbCadastraTarefas")
            .Options;
        var context = new DbTarefasContext(options);

        var repo = new RepositorioTarefa(context);
        var handler = new CadastraTarefaHandler(repo, mock.Object);


        // When
        handler.Execute(comando);

        // Then
        var tarefa = repo.ObtemTarefas(t => t.Titulo == comando.Titulo).FirstOrDefault();

        Assert.NotNull(tarefa);
    }


    [Fact]
    public void QuandoExceptionForLancadaResultadoIsSucessDeveSerFalso()
    {
        // Given

        var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), new DateTime(2022, 2, 2));

        var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

        // Aqui vemos o dublê Stub, para esse caso estamos utilizando o Mock (Biblioteca Moq) para criar um dublê deste tipo.
        // O Stub permite controlar as entradas de uma unidade a ser testada, fornecendo dados que podem ser dificieis de se obter de forma natural
        // Neste caso configuramos o mock para que ao cadastrarmos uma tarefa, lancemos uma exceção, para que assim o retorno "CommandResult" do 
        // comando "Execute", seja "IsSucess = false"

        var mock = new Mock<IRepositorioTarefas>();

        // Configuração que fizemos para que seja lançada uma exceção no "Execute"
        mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
            .Throws(new Exception("Houve um erro na inclusão de tarefas"));

        var repo = mock.Object;
        var handler = new CadastraTarefaHandler(repo, mockLogger.Object);


        // When
        var resultado = handler.Execute(comando);

        // Then


        Assert.False(resultado.IsSucess);
    }

    [Fact]
    public void QuandoExecptionForLancadaDeveLogarAMensagemDaExcecao()
    {
        // Given

        var msgErroEsperada = "Houve um erro na inclusão de tarefas";
        var excecaoEsperada = new Exception(msgErroEsperada);

        var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), new DateTime(2022, 2, 2));
        var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
        var mock = new Mock<IRepositorioTarefas>();

        mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
            .Throws(excecaoEsperada);

        var repo = mock.Object;
        var handler = new CadastraTarefaHandler(repo, mockLogger.Object);


        // When
        var resultado = handler.Execute(comando);

        // Then
        
        mockLogger.Verify(l => l.Log(
            LogLevel.Error, // Nivel de log, no caso um log de erro
            It.IsAny<EventId>(), // Identificador do evento, no caso qualquer um que siga os argumentos passados
            It.IsAny<object>(), // Objeto que será logado
            excecaoEsperada, // Exceção que será logado
            (Func<object, Exception, string>)It.IsAny<object>()), // Função que converte objeto + exceção para string
            Times.Once);

    }

    // Tentativa de exemplo para um dublê Spy, necessario verificar as difirenças de aplicação entre a versão do .NET do curso (2.2) e aque estou aplicando
    // o conhecimento, no caso .NET 6
    /* 
    delegate void CapturaMensagemLog(LogLevel level, EventId eventId, object state, Exception exception, Func<ILogger<CadastraTarefaHandler>, Exception, string> function);

    [Fact]
    public void DadaTarefaComInfoValidaDeveLogar()
    {
       // Given

       string titutoTarefa = "Estudar xUnit";
       var comando = new CadastraTarefa(titutoTarefa, new Categoria(100, "Estudo"), new DateTime(2022, 2, 2));

       var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

       LogLevel levelCapturado = LogLevel.Information;

       string mensagemCapturada = string.Empty;
       CapturaMensagemLog captura = (level, eventId, state, exception, func) =>
       {
           mensagemCapturada = func((ILogger<CadastraTarefaHandler>)state, exception);
       };

       mockLogger.Setup(l =>
           l.Log(
               It.IsAny<LogLevel>(), // Nivel de log, no caso um log de erro
               It.IsAny<EventId>(), // Identificador do evento, no caso qualquer um que siga os argumentos passados
               It.IsAny<ILogger<CadastraTarefaHandler>>(), // Objeto que será logado
               It.IsAny<Exception>(), // Exceção que será logado
               (Func<ILogger<CadastraTarefaHandler>, Exception, string>)It.IsAny<object>() // Função que converte objeto + exceção para string
               )).Callback(captura);


       var mock = new Mock<IRepositorioTarefas>();
       var repo = mock.Object;
       var handler = new CadastraTarefaHandler(repo, mockLogger.Object);


       // When
       handler.Execute(comando);

       // Then
       Assert.Contains(titutoTarefa, mensagemCapturada);
        

    }*/
}