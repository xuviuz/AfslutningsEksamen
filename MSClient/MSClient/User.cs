using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSClient
{
    class User
    {
        public string Scope { get; set; }
        public string Route { get; set; }
        public string Messageid { get; set; }
        public string Json { get; set; }
        public string OrganisationId { get; set; }
        public string UserContextToken { get; set; }
        public string Tracing { get; set; }

        public User(string scope, string route, string messageid, string json, string organisationId, string userContextToken,
            string tracing)
        {
            this.Scope = scope;
            this.Route = route;
            this.Messageid = messageid;
            this.Json = json;
            this.OrganisationId = organisationId;
            this.UserContextToken = userContextToken;
            this.Tracing = tracing;
        }
    }
}
