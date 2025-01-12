namespace OptiTask.DTOs
{
    //API ile client arasında veri taşıma nesneleri oluşturuyoruz
    //Entity'lerin sadece gerekli alanlarını içerir
    public class UserDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
