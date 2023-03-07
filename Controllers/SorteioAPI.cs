using APISorteio;
using APISorteio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SorteioAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SorteioController : ControllerBase
    {
        private readonly SorteioService sorteioService;

        public SorteioController(SorteioService sorteioService)
        {
            this.sorteioService = sorteioService;
        }

        [HttpPost]
        public async Task<ActionResult<Sorteio>> RealizarSorteio([FromBody] Cliente cliente)
        {
            var sorteio = await sorteioService.RealizarSorteioAsync(cliente.Nome, cliente.Telefone, cliente.CPF, cliente.Email);

            return Ok(sorteio);
        }
    }
    public class SorteioService
    {
        private readonly GeradorNumerosSorteio geradorNumeros;
        private readonly SorteioDbContext dbContext;

        public SorteioService(GeradorNumerosSorteio geradorNumeros, SorteioDbContext dbContext)
        {
            this.geradorNumeros = geradorNumeros;
            this.dbContext = dbContext;
        }

        public async Task<Sorteio> RealizarSorteioAsync(string nome, string telefone, string cpf, string email)
        {
            int numeroSorteado = geradorNumeros.GerarNumeroSorteio();

            var sorteio = new Sorteio
            {
                Numero = numeroSorteado,
                NomeCliente = nome,
                TelefoneCliente = telefone,
                CPFCliente = cpf,
                EmailCliente = email
            };

            dbContext.Sorteios.Add(sorteio);
            await dbContext.SaveChangesAsync();

            SalvarNumeroSorteadoEmArquivo(numeroSorteado);

            return sorteio;
        }

        private void SalvarNumeroSorteadoEmArquivo(int numero)
        {
            string filePath = $"numero_sorteado_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine(numero);
            }
        }

    }
    public class GeradorNumerosSorteio
    {
        private readonly Random random;
        private readonly SorteioDbContext dbContext;

        public GeradorNumerosSorteio(SorteioDbContext dbContext)
        {
            this.random = new Random();
            this.dbContext = dbContext;
        }

        public int GerarNumeroSorteio()
        {
            int numero;
            do
            {
                numero = random.Next(100000);
            } while (dbContext.Sorteios.Any(sorteio => sorteio.Numero == numero));
            return numero;
        }
    }
    public class SorteioDbContext : DbContext
    {
        public DbSet<Sorteio> Sorteios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SorteioDb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sorteio>()
                .HasIndex(s => s.EmailCliente)
                .IsUnique();
        }
    }
}
