using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CoisasAFazer.WebApp.Controllers;
using CoisasAFazer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CoisasAFazer.Services.Handlers;
using CoisasAFazer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using CoisasAFazer.Core.Models;

namespace Alura.CoisasAFazer.Testes
{
    public class TarefasControllerEndPointCadastraTarefa
    {
        [Fact]
        public void DadaTarefaValidaRetornarOk()
        {
            // Given
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("Dbtaref").Options;
            var context = new DbTarefasContext(options);

            context.Categorias.Add(new Categoria(20, "Estudo"));
            context.SaveChanges();

            var repo = new RepositorioTarefa(context);

            var controlador = new TarefasController(repo, mockLogger.Object);
            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estudar xUnit";
            model.Prazo = new DateTime(2022, 12, 31);
            

            // When
            var retorno = controlador.EndpointCadastraTarefa(model); 
        
            // Then
            Assert.IsType<OkResult>(retorno);
        
        }
        
        [Fact]
        public void QuandoExcecaoForLancadaRetornarStatusCode500()
        {
            // Given
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var mock = new Mock<IRepositorioTarefas>();

            var repo = mock.Object;
            mock.Setup(x => x.ObtemCategoriaPorId(20)).Returns(new Categoria(20, "Estudo"));
            mock.Setup(x => x.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro"));
            
            var controlador = new TarefasController(repo, mockLogger.Object);
            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estudar xUnit";
            model.Prazo = new DateTime(2022, 12, 31);
            

            // When
            var retorno = controlador.EndpointCadastraTarefa(model); 
        
            // Then
            Assert.IsType<StatusCodeResult>(retorno);
            var statusCodeResult = (retorno as StatusCodeResult).StatusCode;
            Assert.Equal(500, statusCodeResult);
        
        }
    }
}