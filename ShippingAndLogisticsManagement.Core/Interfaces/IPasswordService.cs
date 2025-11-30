namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool VerifyPassword(string hash, string password);
    }
}
