namespace ristorante_backend.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public Menu() { }

        public Menu(int id, string nome)
        {
            this.Id = id;
            this.Nome = nome;
        }
    }
}
