using System;
using System.IO;
using System.Text.Json;

namespace CovidConfigProgram
{
    public class CovidConfig
    {
        private const string CONFIG_FILE = "covid_config.json";
        private string satuanSuhu;
        private int batasHariDemam;
        private string pesanDitolak;
        private string pesanDiterima;

        public CovidConfig()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(CONFIG_FILE))
                {
                    string jsonContent = File.ReadAllText(CONFIG_FILE);
                    var config = JsonSerializer.Deserialize<JsonElement>(jsonContent);

                    satuanSuhu = config.GetProperty("satuan_suhu").GetString();
                    batasHariDemam = int.Parse(config.GetProperty("batas_hari_demam").GetString());
                    pesanDitolak = config.GetProperty("pesan_ditolak").GetString();
                    pesanDiterima = config.GetProperty("pesan_diterima").GetString();
                }
                else
                {
                    satuanSuhu = "celcius";
                    batasHariDemam = 14;
                    pesanDitolak = "Anda tidak diperbolehkan masuk ke dalam gedung ini";
                    pesanDiterima = "Anda dipersilahkan untuk masuk ke dalam gedung ini";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                satuanSuhu = "celcius";
                batasHariDemam = 14;
                pesanDitolak = "Anda tidak diperbolehkan masuk ke dalam gedung ini";
                pesanDiterima = "Anda dipersilahkan untuk masuk ke dalam gedung ini";
            }
        }
        public double UbahSatuan(double suhu)
        {
            if (satuanSuhu.ToLower() == "celcius")
                return suhu; 
            else if (satuanSuhu.ToLower() == "fahrenheit")
                return (suhu * 9 / 5) + 32;

            return suhu;
        }

        public string GetSatuanSuhu() => satuanSuhu;
        public int GetBatasHariDemam() => batasHariDemam;
        public string GetPesanDitolak() => pesanDitolak;
        public string GetPesanDiterima() => pesanDiterima;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== COVID Screening Application ===");
            CovidConfig config = new CovidConfig();

            Console.Write($"Berapa suhu badan anda saat ini? Dalam nilai {config.GetSatuanSuhu()}: ");
            if (!double.TryParse(Console.ReadLine(), out double suhuInput))
            {
                Console.WriteLine("Input suhu tidak valid.");
                return;
            }

            Console.Write("Berapa hari yang lalu (perkiraan) anda terakhir memiliki gejala demam? ");
            if (!int.TryParse(Console.ReadLine(), out int hariDemam))
            {
                Console.WriteLine("Input hari tidak valid.");
                return;
            }

            bool kondisiSuhu = false;
            string satuanSuhu = config.GetSatuanSuhu().ToLower();

            if (satuanSuhu == "celcius")
                kondisiSuhu = suhuInput >= 36.5 && suhuInput <= 37.5;
            else if (satuanSuhu == "fahrenheit")
                kondisiSuhu = suhuInput >= 97.7 && suhuInput <= 99.5;

            bool kondisiHari = hariDemam < config.GetBatasHariDemam();

            double suhuKonversi = config.UbahSatuan(suhuInput);
            string satuanKonversi = satuanSuhu == "celcius" ? "fahrenheit" : "celcius";

            Console.WriteLine("\n=== Hasil Screening ===");
            Console.WriteLine($"Suhu anda: {suhuInput} {satuanSuhu} ({suhuKonversi} {satuanKonversi})");
            Console.WriteLine($"Hari sejak demam terakhir: {hariDemam} hari");

            if (kondisiSuhu && kondisiHari)
            {
                Console.WriteLine(config.GetPesanDiterima());
            }
            else
            {
                Console.WriteLine(config.GetPesanDitolak());

                if (!kondisiSuhu)
                    Console.WriteLine($"Suhu anda tidak dalam rentang normal");

                if (!kondisiHari)
                    Console.WriteLine($"Anda masih dalam masa pemulihan");
            }

            Console.WriteLine("\nTekan sembarang tombol untuk keluar...");
            Console.ReadKey();
        }
    }
}