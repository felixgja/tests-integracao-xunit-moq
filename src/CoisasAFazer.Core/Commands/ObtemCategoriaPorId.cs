using CoisasAFazer.Core.Models;
using MediatR;

namespace CoisasAFazer.Core.Commands
{
    public class ObtemCategoriaPorId: IRequest<Categoria>
    {
        public ObtemCategoriaPorId(int idCategoria)
        {
            IdCategoria = idCategoria;
        }

        public int IdCategoria { get; }
    }
}
