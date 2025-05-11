using data.Domain.UseCase;
using data.RemoteData.RemoteDatabase.DAO;
using domain.Models;
using Microsoft.AspNetCore.Mvc;
using presence_api.Models;

namespace presence_api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminPanelController : ControllerBase
{
    private readonly GroupUseCase _groupUseCase;
    private readonly UserUseCase _userUseCase;
    private readonly PresenceUseCase _presenceUseCase;
    private readonly APIUseCase _apiUseCase;

    public AdminPanelController(GroupUseCase groupUseCase, UserUseCase userUseCase, PresenceUseCase presenceUseCase, APIUseCase apiUseCase)
    {
        _groupUseCase = groupUseCase;
        _userUseCase = userUseCase;
        _presenceUseCase = presenceUseCase;
        _apiUseCase = apiUseCase;
    }

    [HttpPost("groups/add")]
    public IActionResult AddGroup(GroupAPI groupDto)
    {
        try
        {
            _apiUseCase.AddGroup(groupDto);
            return Ok(new { message = "Группа успешно добавлена." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.ToString() });
        }
    }

    [HttpDelete("groups/{groupId}")]
    public IActionResult DeleteGroup(int groupId)
    {
        try
        {
            _groupUseCase.DeleteGroupById(groupId);
            return Ok(new { message = "Группа успешно удалена." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("groups/{groupId}/students")]
    public IActionResult AddStudentsToGroup(int groupId, [FromBody] StudentsDTO studentsDto)
    {
        try
        {
            _apiUseCase.AddStudentsToExistingGroup(groupId, studentsDto.Students);
            return Ok(new { message = "Студенты успешно добавлены в группу." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    

    [HttpGet("presence/{groupId}")]
    public IActionResult GetPresence(int groupId, [FromQuery] DateOnly? date, [FromQuery] int? student)
    {
        try
        {
            var presence = _presenceUseCase.GetPresenceByUserId(groupId, date, student);
            return Ok(presence);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("presence/clear")]
    public IActionResult ClearAllPresence()
    {
        try
        {
            _presenceUseCase.ClearAllPresence();
            return Ok(new { message = "Все записи посещаемости успешно удалены." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("presence")]
    public IActionResult ClearPresenceByGroup([FromQuery] int group)
    {
        try
        {
            _presenceUseCase.DeletePresenceByGroup(group);
            return Ok(new { message = "Записи посещаемости группы успешно удалены." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("presence")]
    public IActionResult AddPresence([FromBody] List<PresenceInputDTO> presenceList)
    {
        try
        {
            _presenceUseCase.AddPresence(presenceList);
            return Ok(new { message = "Посещаемость успешно добавлена." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("presence")]
    public IActionResult UpdatePresence([FromBody] List<UpdateAttendanceDTO> attendanceList)
    {
        try
        {
            _presenceUseCase.UpdatePresence(attendanceList);
            return Ok(new { message = "Посещаемость успешно обновлена." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}