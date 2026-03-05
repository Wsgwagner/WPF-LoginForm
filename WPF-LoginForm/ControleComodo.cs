using System.Collections.Generic;

public class Comodo
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Icone { get; set; } // Ex: "🛋️", "🍳", "🛏️"
    public List<Equipamento> Equipamentos { get; set; } = new List<Equipamento>();
}

public class Equipamento
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Tipo { get; set; } // "Luz", "Portão", "Câmera"
    public int Intensidade { get; set; } // 0-100 para luzes
    public bool Status { get; set; } // Ligado/Desligado ou Aberto/Fechado
}