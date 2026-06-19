
namespace GesMgmt.Domain.Entities
{
    public class ValidationMessage
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Message_ESP { get; set; }
        public string Message_ENG { get; set; }
        public string Message_Friendy_ESP { get; set; }
        public string Message_Friendy_ENG { get; set; }
        public string Action { get; set; }
        public string Api { get; set; }
    }
}