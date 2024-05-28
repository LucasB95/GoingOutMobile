namespace GoingOutMobile.Services.Interfaces
{
    public interface IMercadoPagoService
    {
        Task<List<string>> MPCorto();
        Task<List<string>> preparandoMP();
    }
}