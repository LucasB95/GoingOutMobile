namespace GoingOutMobile.Services
{
    public interface IMercadoPagoService
    {
        Task<List<string>> MPCorto();
        Task<List<string>> preparandoMP();
    }
}