using System.Collections.Generic;

public class ComodoAgrupado
{
    public string NomeComodo { get; set; }
    public List<EquipamentoSimples> Equipamentos { get; set; }
}

public class EquipamentoSimples
{
    public string Nome { get; set; }
    public string Tipo { get; set; }
    public string Status { get; set; }
}