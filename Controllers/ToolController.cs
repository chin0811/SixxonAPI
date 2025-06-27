using Microsoft.AspNetCore.Mvc;
using sixxonAPI.Dtos;
using sixxonAPI.Interfaces;
using sixxonAPI.Services;
using SixxonAPI.Dtos;
using SixxonAPI.Services;

namespace sixxonAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToolController : ControllerBase
    {
        private readonly IToolService _toolService;

        public ToolController(IToolService toolService)
        {
            _toolService = toolService;
        }

        // 取得所有工具，含狀態與領用人
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var result = await _toolService.GetAllAsync();
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateToolDto dto)
        {
            try
            {
                await _toolService.InsertToolAsync(dto);
                return Ok(new { message = "新增成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

 }
