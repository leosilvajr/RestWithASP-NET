namespace RestWithASPNET.Data.Converter.Contract
{
    public interface IParse< O, D> //O: Origem, D: Destino
    {
        D Parse(O origin); // Recebe um objeto Origem, converte e retorna Destino
        List<D> Parse(List<O> origin);
    }
}
