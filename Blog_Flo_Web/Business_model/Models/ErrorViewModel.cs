namespace Blog_Flo_Web.Business_model.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; } // ������������� ������� (����� ���� null)
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // ���������� �� RequestId
        public bool IsDevelopment { get; set; } // ����� ����������
        public int? StatusCode { get; set; } // ��� ������� HTTP (��������, 404, 500)
    }
}