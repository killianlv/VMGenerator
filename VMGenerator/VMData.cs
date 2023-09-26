using Newtonsoft.Json;

namespace VMGenerator
{
    public class VMRequestData
    {
        public string region { get; set; }
        public string plan { get; set; }
        public string hostname { get; set; }
        public string image_id { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class VMResponseData
    {
        public string server_status { get; set; }

        public static VMResponseData FromJson(string json)
        {
            try
            {
                // Utilisez JsonConvert pour désérialiser la réponse JSON en un objet VMResponseData.
                return JsonConvert.DeserializeObject<VMResponseData>(json);
            }
            catch (Exception ex)
            {
                // Gérez les erreurs de désérialisation ici, si nécessaire.
                Console.WriteLine($"Erreur lors de la désérialisation JSON : {ex.Message}");
                return null;
            }
        }
    }
}
