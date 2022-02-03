using System;
using System.Linq;
using CoisasAFazer.Core.Commands;
using CoisasAFazer.Core.Models;
using CoisasAFazer.Infrastructure;
using CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;

namespace Alura.CoisasAFazer.Testes;

public class CadastraTarefaHandlerExecute
{
    [Fact]
    public void DadaTarefaComInfoValidaIncluirNoBD()
    {
        // Given

        var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), new DateTime(2022, 2, 2));

        // Aqui estamos configurando um Fake Object, estes são utilizados para simular recursos de forma rápida e leve, neste caso
        // estamos a simular um Banco de Dados utilizando o "InMemory" que simula um banco de dados na memória, dessa forma não 
        // temos problemas com demoras nos testes alem de que os testes não entram em conflito com o banco de produção. 
        // Lembrando que ao utilizarmos do "InMemory" para testes é necessário que algumas mudanças sejam feitas na declaração do contexto

        var options = new DbContextOptionsBuilder<DbTarefasContext>()
            .UseInMemoryDatabase("DbTarefasTeste")
            .Options;
        var context = new DbTarefasContext(options);

        var repo = new RepositorioTarefa(context);
        var handler = new CadastraTarefaHandler(repo);


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
        
        // Aqui vemos o dublê Stub, para esse caso estamos utilizando o Mock (Biblioteca Moq) para criar um dublê deste tipo.
        // O Stub permite controlar as entradas de uma unidade a ser testada, fornecendo dados que podem ser dificieis de se obter de forma natural
        // Neste caso configuramos o mock para que ao cadastrarmos uma tarefa, lancemos uma exceção, para que assim o retorno "CommandResult" do 
        // comando "Execute", seja "IsSucess = false"

        var mock = new Mock<IRepositorioTarefas>();

        // Configuração que fizemos para que seja lançada uma exceção no "Execute"
        mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
            .Throws(new Exception("Houve um erro na inclusão de tarefas"));

        var repo = mock.Object;
        var handler = new CadastraTarefaHandler(repo);


        // When
        var resultado = handler.Execute(comando);

        // Then


        Assert.False(resultado.IsSucess);
    }
}