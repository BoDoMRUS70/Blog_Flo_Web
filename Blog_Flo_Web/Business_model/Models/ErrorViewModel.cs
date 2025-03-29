namespace Blog_Flo_Web.Business_model.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; } // Идентификатор запроса (может быть null)
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // Показывать ли RequestId
        public bool IsDevelopment { get; set; } // Режим разработки
        public int? StatusCode { get; set; } // Код статуса HTTP (например, 404, 500)
    }
}