using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CoisasAFazer.Core.Commands;
using CoisasAFazer.Services.Handlers;
using CoisasAFazer.Infrastructure;

namespace Alura.CoisasAFazer.Testes
{
    public class ObtemCategoriaPorIdExecute
    {
        [Fact]
        public void QuandoIdForExistenteChamaCategoriaPorIdUmaVez()
        {
            // Given
            var idCategoria = 20;
            var comando = new ObtemCategoriaPorId(idCategoria);
            
            // Na aula anterior ja haviamos utilizado o "Mock", mas neste exemplo o mock é um dublê, sendo assim um "Mock Object"
            // A diferença entre um "Stub Object" e um "Mock Object", é que no stub nos configuramos um valor que sera utilizado para o teste
            // já no mock, nos o utilizamos para verificar um comportamento, a verificação é feita dentro do "Assert/Then", neste 
            // caso estamos verificando se um comando foi executado apenas uma vez
            var mock = new Mock<IRepositorioTarefas>();
            var repo = mock.Object;
            var handler = new ObtemCategoriaPorIdHandler(repo);
            // When
            handler.Execute(comando);
            
            // Then
            mock.Verify(r => r.ObtemCategoriaPorId(idCategoria), Times.Once());
        }
    }
}