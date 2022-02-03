using System;
using System.Collections.Generic;
using System.Linq;
using CoisasAFazer.Core.Commands;
using CoisasAFazer.Core.Models;
using CoisasAFazer.Infrastructure;
using CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
    public class GerenciaPrazoDasTarefasHandlerExecute
    {
        [Fact]
        public void DadaTarefaEmAtrasoMudarStatus()
        {
            // Given
            // Aqui estamos configurando um dublê do tipo Fake Object, estes são utilizados para simular recursos de forma rápida e leve, neste caso
            // estamos a simular um Banco de Dados utilizando o "InMemory" que simula um banco de dados na memória, dessa forma não 
            // temos problemas com demoras nos testes alem de que os testes não entram em conflito com o banco de produção. 
            // Lembrando que ao utilizarmos do "InMemory" para testes é necessário que algumas mudanças sejam feitas na declaração do contexto
            
            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasTeste")
                .Options;

            var context = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(context);
            var handler = new GerenciaPrazoDasTarefasHandler(repo);
            var comando = new GerenciaPrazoDasTarefas(new DateTime(2022,2,3));

            
            // Apesar de declararmos as categorias, não é ela quem esta em teste e sim as tarefas, porem para que o cadastro de tarefa ocorra é necessario
            // indicar uma categoria, e por esse motivo estamos criando, este é o duble chamado de "Dummy Object", dependendo da regra de negocio
            // do sistema, "null" também poderá ser um Dummy Object.

            var compCateg = new Categoria(1, "Compras");
            var casaCateg = new Categoria(2, "Casa");
            var trabCateg = new Categoria(3, "Trabalho");
            var saudCateg = new Categoria(4, "Saúde");
            var higiCateg = new Categoria(5, "Higiene");

            var tarefas = new List<Tarefa>
            {
                // Atrasadas a partir de 03/02/2022
                new Tarefa(11, "Tirar lixo", casaCateg, new DateTime(2022, 1, 20), null, StatusTarefa.Criada),
                new Tarefa(4, "Fazer Almoço", casaCateg, new DateTime(2021, 12, 5), null, StatusTarefa.Criada),
                new Tarefa(9, "Ir à academia", saudCateg, new DateTime(2022, 2, 1), null, StatusTarefa.Criada),
                new Tarefa(7, "Concluir o relatório", trabCateg, new DateTime(2022, 1, 2), null, StatusTarefa.Pendente),
                new Tarefa(10, "Beber água", saudCateg, new DateTime(2022, 2, 2), null, StatusTarefa.Criada),

                // Dentro do prazo em 03/02/2022
                new Tarefa(8, "Comparecer à reunião", trabCateg, new DateTime(2021, 12, 31), new DateTime(2022, 1, 10), StatusTarefa.Concluida),
                new Tarefa(2, "Arrumar a cama", casaCateg, new DateTime(2022, 6, 10), null, StatusTarefa.Criada),
                new Tarefa(3, "Escovar os dentes", higiCateg, new DateTime(2022, 5, 1), null, StatusTarefa.Criada),
                new Tarefa(5, "Comprar presente do João", compCateg, new DateTime(2022, 10, 8), null, StatusTarefa.Criada),
                new Tarefa(6, "Comprar ração", compCateg, new DateTime(2022, 12, 11), null, StatusTarefa.Criada)
            };
            
            repo.IncluirTarefas(tarefas.ToArray());

            // When
            handler.Execute(comando);

            // Then
            var tarefasEmAtraso = repo.ObtemTarefas(t => t.Status == StatusTarefa.EmAtraso);
            Assert.Equal(5, tarefasEmAtraso.Count());
        }
    }
}