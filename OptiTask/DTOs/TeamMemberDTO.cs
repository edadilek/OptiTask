namespace OptiTask.DTOs
{
    //API ile client arasında veri taşıma nesneleri oluşturuyoruz
    //Entity'lerin sadece gerekli alanlarını içerir
    public class TeamMemberDTO
    {
        public int TeamId { get; set; }
        public int UserId { get; set; }
    }
}
