using System;
using System.Configuration;
using System.Xml;

namespace Ks.Core.Configuration
{
    /// <summary>
    ///     Represents a KsConfig
    /// </summary>
    public class KsConfig : IConfigurationSectionHandler
    {
        /// <summary>
        ///     Indicates whether we should ignore startup tasks
        /// </summary>
        public bool IgnoreStartupTasks { get; private set; }

        /// <summary>
        ///     Path to database with user agent strings
        /// </summary>
        public string UserAgentStringsPath { get; private set; }

        /// <summary>
        ///     Indicates whether we should use Redis server for caching (instead of default in-memory caching)
        /// </summary>
        public bool RedisCachingEnabled { get; private set; }

        /// <summary>
        ///     Redis connection string. Used when Redis caching is enabled
        /// </summary>
        public string RedisCachingConnectionString { get; private set; }

        /// <summary>
        ///     Indicates whether we should support previous versions (it can slightly improve performance)
        /// </summary>
        public bool SupportPreviousKsVersionsNode { get; private set; }

        /// <summary>
        ///     A value indicating whether the site is run on multiple instances (e.g. web farm, Windows Azure with multiple
        ///     instances, etc).
        ///     Do not enable it if you run on Azure but use one instance only
        /// </summary>
        public bool MultipleInstancesEnabled { get; private set; }

        /// <summary>
        ///     A value indicating whether the site is run on Windows Azure Websites
        /// </summary>
        public bool RunOnAzureWebsites { get; private set; }

        /// <summary>
        ///     Connection string for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageConnectionString { get; private set; }

        /// <summary>
        ///     Container name for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageContainerName { get; private set; }

        /// <summary>
        ///     End point for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageEndPoint { get; private set; }

        /// <summary>
        ///     A value indicating whether a store owner can install sample data during installation
        /// </summary>
        public bool DisableSampleDataDuringInstallation { get; private set; }

        /// <summary>
        ///     By default this setting should always be set to "False" (only for advanced users)
        /// </summary>
        public bool UseFastInstallationService { get; private set; }

        /// <summary>
        ///     A list of plugins ignored during Ks.Commerce installation
        /// </summary>
        public string PluginsIgnoredDuringInstallation { get; private set; }

        /// <summary>
        ///     Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new KsConfig();

            var startupNode = section.SelectSingleNode("Startup");
            if (startupNode != null && startupNode.Attributes != null)
            {
                var attribute = startupNode.Attributes["IgnoreStartupTasks"];
                if (attribute != null)
                    config.IgnoreStartupTasks = Convert.ToBoolean(attribute.Value);
            }

            var redisCachingNode = section.SelectSingleNode("RedisCaching");
            if (redisCachingNode != null && redisCachingNode.Attributes != null)
            {
                var enabledAttribute = redisCachingNode.Attributes["Enabled"];
                if (enabledAttribute != null)
                    config.RedisCachingEnabled = Convert.ToBoolean(enabledAttribute.Value);

                var connectionStringAttribute = redisCachingNode.Attributes["ConnectionString"];
                if (connectionStringAttribute != null)
                    config.RedisCachingConnectionString = connectionStringAttribute.Value;
            }

            var userAgentStringsNode = section.SelectSingleNode("UserAgentStrings");
            if (userAgentStringsNode != null && userAgentStringsNode.Attributes != null)
            {
                var attribute = userAgentStringsNode.Attributes["databasePath"];
                if (attribute != null)
                    config.UserAgentStringsPath = attribute.Value;
            }

            var supportPreviousKsVersionsNode = section.SelectSingleNode("SupportPreviousKs.commerceVersions");
            if (supportPreviousKsVersionsNode != null &&
                supportPreviousKsVersionsNode.Attributes != null)
            {
                var attribute = supportPreviousKsVersionsNode.Attributes["Enabled"];
                if (attribute != null)
                    config.SupportPreviousKsVersionsNode = Convert.ToBoolean(attribute.Value);
            }

            var webFarmsNode = section.SelectSingleNode("WebFarms");
            if (webFarmsNode != null && webFarmsNode.Attributes != null)
            {
                var multipleInstancesEnabledAttribute = webFarmsNode.Attributes["MultipleInstancesEnabled"];
                if (multipleInstancesEnabledAttribute != null)
                    config.MultipleInstancesEnabled = Convert.ToBoolean(multipleInstancesEnabledAttribute.Value);

                var runOnAzureWebsitesAttribute = webFarmsNode.Attributes["RunOnAzureWebsites"];
                if (runOnAzureWebsitesAttribute != null)
                    config.RunOnAzureWebsites = Convert.ToBoolean(runOnAzureWebsitesAttribute.Value);
            }

            var azureBlobStorageNode = section.SelectSingleNode("AzureBlobStorage");
            if (azureBlobStorageNode != null && azureBlobStorageNode.Attributes != null)
            {
                var azureConnectionStringAttribute = azureBlobStorageNode.Attributes["ConnectionString"];
                if (azureConnectionStringAttribute != null)
                    config.AzureBlobStorageConnectionString = azureConnectionStringAttribute.Value;

                var azureContainerNameAttribute = azureBlobStorageNode.Attributes["ContainerName"];
                if (azureContainerNameAttribute != null)
                    config.AzureBlobStorageContainerName = azureContainerNameAttribute.Value;

                var azureEndPointAttribute = azureBlobStorageNode.Attributes["EndPoint"];
                if (azureEndPointAttribute != null)
                    config.AzureBlobStorageEndPoint = azureEndPointAttribute.Value;
            }

            var installationNode = section.SelectSingleNode("Installation");
            if (installationNode != null && installationNode.Attributes != null)
            {
                var disableSampleDataDuringInstallationAttribute =
                    installationNode.Attributes["DisableSampleDataDuringInstallation"];
                if (disableSampleDataDuringInstallationAttribute != null)
                    config.DisableSampleDataDuringInstallation =
                        Convert.ToBoolean(disableSampleDataDuringInstallationAttribute.Value);

                var useFastInstallationServiceAttribute = installationNode.Attributes["UseFastInstallationService"];
                if (useFastInstallationServiceAttribute != null)
                    config.UseFastInstallationService = Convert.ToBoolean(useFastInstallationServiceAttribute.Value);

                var pluginsIgnoredDuringInstallationAttribute =
                    installationNode.Attributes["PluginsIgnoredDuringInstallation"];
                if (pluginsIgnoredDuringInstallationAttribute != null)
                    config.PluginsIgnoredDuringInstallation = pluginsIgnoredDuringInstallationAttribute.Value;
            }

            return config;
        }
    }

    /*
     <configuration>
            <configSections>
                <section name="KsConfig" type="Ks.Core.Configuration.KsConfig, Ks.Core" requirePermission="false" />
            <\configSections>
     <\configuration>
      
      <KsConfig>
        <!-- Web farm support
        Enable "MultipleInstancesEnabled" if you run multiple instances.
        Enable "RunOnAzureWebsites" if you run on Windows Azure Web sites (not cloud services). -->
        <WebFarms MultipleInstancesEnabled="False" RunOnAzureWebsites="False" />
        <!-- Windows Azure BLOB storage. Specify your connection string, container name, end point for BLOB storage here -->
        <AzureBlobStorage ConnectionString="" ContainerName="" EndPoint="" />
        <!-- Redis support (used by web farms, Azure, etc). Find more about it at https://azure.microsoft.com/en-us/documentation/articles/cache-dotnet-how-to-use-azure-redis-cache/ -->
        <RedisCaching Enabled="false" ConnectionString="localhost" />
        <!-- You can get the latest version of user agent strings at http://user-agent-string.info/ -->
        <UserAgentStrings databasePath="~/App_Data/uas_20140809-02.ini" />
        <!-- Set the setting below to "False" if you did not upgrade from one of the previous versions. It can slightly improve performance -->
        <supportPreviousKsVersionsNode Enabled="True" />
        <!-- Do not edit this element. For advanced users only -->
        <Installation DisableSampleDataDuringInstallation="False" UseFastInstallationService="False" PluginsIgnoredDuringInstallation="" />
      </KsConfig>
     
     */
}