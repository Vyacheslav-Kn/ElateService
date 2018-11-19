namespace ElateService.BLL.ModelsDTO
{
    public class ResponceDTO
    {
        public int ResponceId { get; set; }
        public string ResponceText { get; set; }
        public double? Price { get; set; }
        public int IndentId { get; set; }
        public ExecutorDTO Executor { get; set; }
    }
}
