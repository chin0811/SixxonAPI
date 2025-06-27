using sixxonAPI.Interfaces;
using SixxonAPI.Dtos;
using SixxonAPI.Interfaces;

namespace SixxonAPI.Services
{
    public class ToolService : IToolService
    {
        private readonly IToolRepository _repo;

        public ToolService(IToolRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ToolDto>> GetAllAsync()
        {
            var rawTools = await _repo.GetAllAsync();

            var result = rawTools.Select(t => new ToolDto
            {
                Category = t.Category,
                Barcode = t.Barcode,
                ToolName = t.Tool_Name,
                ToolCode = t.Tool_Code,
                BorrowDays = t.Borrow_Days,
                Receiver = t.Receiver,
                Remark = t.Remark,
                Status = t.Receiver == null
                    ? "可用"
                    : (t.ExpectedReturnDate.HasValue && t.ExpectedReturnDate.Value < DateTime.Now ? "逾期" : "已借出")
            });
            foreach (var tool in rawTools)
            {
                Console.WriteLine($"BARCODE: {tool.Barcode}, REMARK: {tool.Remark}");
            }
            return result;
        }
        public async Task InsertToolAsync(CreateToolDto dto)
        {
            var exists = await _repo.IsBarcodeExistsAsync(dto.Barcode);
            if (exists)
            {
                throw new Exception("條碼已存在，請確認是否重複。");
            }

            await _repo.InsertAsync(dto);
        }
    }

}
