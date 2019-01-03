
namespace MSClient
{
    /// <summary>
    /// Represents class User
    /// </summary>
    class User
    {
        /// <summary>
        /// Gets and sets scope of the message (name of calling program)
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets and sets routing key
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Gets and sets
        /// </summary>
        public string Messageid { get; set; }

        /// <summary>
        /// Gets and sets json
        /// </summary>
        public string Json { get; set; }

        /// <summary>
        /// Gets and sets organisation id
        /// </summary>
        public string OrganisationId { get; set; }

        /// <summary>
        /// Gets and sets user context token
        /// </summary>
        public string UserContextToken { get; set; }

        /// <summary>
        /// Gets and sets tracing level
        /// </summary>
        public string Tracing { get; set; }

        /// <summary>
        /// Initializes a new instance of the User
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="route">routing key</param>
        /// <param name="messageid">message id</param>
        /// <param name="json">json</param>
        /// <param name="organisationId">organisation id</param>
        /// <param name="userContextToken">user context token</param>
        /// <param name="tracing">tracing level</param>
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
