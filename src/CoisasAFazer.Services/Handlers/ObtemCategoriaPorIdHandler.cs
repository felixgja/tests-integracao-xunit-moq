using CoisasAFazer.Core.Commands;
using CoisasAFazer.Core.Models;
using CoisasAFazer.Infrastructure;

namespace CoisasAFazer.Services.Handlers
{
    public class ObtemCategoriaPorIdHandler
    {
        IRepositorioTarefas _repo;

        public ObtemCategoriaPorIdHandler(IRepositorioTarefas repositorio)
        {
            _repo = repositorio;
        }
        public Categoria Execute(ObtemCategoriaPorId comando)
        {
            return _repo.ObtemCategoriaPorId(comando.IdCategoria);
        }
    }
}
