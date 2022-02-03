using System;
using System.Linq;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

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
}