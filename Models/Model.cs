using System;

namespace Models
{
    public class Evento
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public int Pessoas { get; set; }
    }
}
