using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

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

            // Ottieni il percorso dell'assembly in esecuzione
            string assemblyPath = Assembly.GetEntryAssembly().Location;
            string sha256Hash = string.Empty;
            // Calcola l'hash SHA-256 del contenuto dell'assembly
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(System.IO.File.ReadAllBytes(assemblyPath));
                sha256Hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
            }
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
            hashAssembly = "SHA-256 Hash: " + sha256Hash;
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

