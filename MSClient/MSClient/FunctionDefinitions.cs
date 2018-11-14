using System;

namespace MSClient
{
    class FunctionDefinitions
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string FunctionData { get; set; }

        public FunctionDefinitions(string id, string name, string functionData)
        {
            this.Id = id;
            this.Name = name;
            this.FunctionData = functionData;
        }
    }
}
