using System;

namespace MSClient
{
    /// <summary>
    /// Contains the function definitions
    /// </summary>
    class FunctionDefinitions
    {
        /// <summary>
        /// User id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Function name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Function data: function or arguments
        /// </summary>
        public string FunctionData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="name">function name</param>
        /// <param name="functionData">function data</param>
        public FunctionDefinitions(string id, string name, string functionData)
        {
            this.Id = id;
            this.Name = name;
            this.FunctionData = functionData;
        }
    }
}
