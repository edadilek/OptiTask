namespace OptiTask.DTOs
{
    //API ile client arasında veri taşıma nesneleri oluşturuyoruz
    //Entity'lerin sadece gerekli alanlarını içerir
    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
