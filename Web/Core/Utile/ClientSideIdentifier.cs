namespace Web.Core.Utile
{
    public static class ClientSideIdentifier
    {
        private static string _clientSideId_P1 = Guid.NewGuid().ToString();
        private static string _clientSideId = _clientSideId_P1 + ((DateTime.UtcNow).ToString());
        public static string GetClientSideId()
        {
            return _clientSideId;
        }
    }
}
