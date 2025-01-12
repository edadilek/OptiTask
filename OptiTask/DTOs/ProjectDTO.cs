namespace OptiTask.DTOs
{
    //API ile client arasında veri taşıma nesneleri oluşturuyoruz
    //Entity'lerin sadece gerekli alanlarını içerir
    public class ProjectDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
