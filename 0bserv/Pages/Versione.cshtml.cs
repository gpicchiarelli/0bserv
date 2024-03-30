using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using System;
using System.Runtime.InteropServices;

namespace _0bserv.Pages
{
    public class PrivacyModel : PageModel
    {
        public string Versione;
        public string hashAssembly;
        public string Applicazione;
        public string Ambiente;
        public string Piattaforma;
        public string SOVer;
        public string Configurazione_build;
        public string RuntimeVer;
        public string ClrVersione;

        private void PageLoad()
        {
            // Ottieni la versione dell'assembly principale
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            string versionString = version.ToString();

            // Calcola l'hash SHA-256 dell'assembly principale
            string assemblyPath = Assembly.GetEntryAssembly().Location;

            using var sha512 = System.Security.Cryptography.SHA512.Create();
            var bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(assemblyPath));
            string sha512Hash =  Convert.ToBase64String(bytes).ToString();

            string environment = "Sviluppo"; // Modifica in base all'ambiente di esecuzione

            // Piattaforma e sistema operativo
            string platform = Environment.OSVersion.Platform.ToString();
            string osVersion = Environment.OSVersion.VersionString;

            // Informazioni sulla compilazione e sulla configurazione
            string buildConfig = "";
#if DEBUG
            buildConfig = "Debug";
#else
        buildConfig = "Release";
#endif

            string runtimeVersion = RuntimeInformation.FrameworkDescription;
            string clrVersion = AppContext.BaseDirectory;

            // Visualizza le informazioni sulla pagina
            Versione = "Versione: " + versionString;
            hashAssembly = "SHA-512 Hash: " + sha512Hash;
            Applicazione = $"Nome dell'applicazione: {Assembly.GetEntryAssembly().GetName().Name}";
            Ambiente = $"Ambiente di esecuzione: {environment}";
            Piattaforma = $"Piattaforma: {platform}";
            SOVer = $"Versione del sistema operativo: {osVersion}";
            Configurazione_build = $"Configurazione di build: {buildConfig}";
            RuntimeVer = $"Versione del runtime .NET: {runtimeVersion}";
            ClrVersione = $"Directory del runtime CLR: {clrVersion}";

        }

        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            PageLoad();
        }
    }
}

