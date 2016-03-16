﻿namespace REstate.Configuration
{
    public interface ICodeWithDatabaseConfiguration
        : ICodeElement
    {
        int? SqlDatabaseDefinitionId { get; }

        string SqlDatabaseName { get; }

        string SqlDatabaseDescription { get; }

        string ConnectionString { get; }

        string ProviderName { get; set; }
    }

    public class CodeWithDatabaseConfiguration 
        : ICodeWithDatabaseConfiguration
    {
        public int CodeElementId { get; set; }
        public string ConnectorKey { get; set; }
        public string CodeElementName { get; set; }
        public string SemanticVersion { get; set; }
        public string CodeElementDescription { get; set; }
        public string CodeBody { get; set; }
        public int? SqlDatabaseDefinitionId { get; set; }
        public string SqlDatabaseName { get; set; }
        public string SqlDatabaseDescription { get; set; }
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }
    }
}