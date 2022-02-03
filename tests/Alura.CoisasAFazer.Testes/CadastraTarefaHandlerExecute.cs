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