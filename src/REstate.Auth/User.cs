using System.Collections.Generic;

namespace REstate.Auth
{
    public class Principal : IPrincipal
    {
        public int PrincipalId { get; set; }

        public string UserOrApplicationName { get; set; }

        public string PrincipalType { get; set; }

        public ICollection<string> Claims { get; set; }

        public string ApiKey { get; set; }
    }
}