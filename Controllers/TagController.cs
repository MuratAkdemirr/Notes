using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notes.Data;
using Notes.Model.Dto.Tag;
using Notes.Model.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Notes.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[SwaggerTag("Tag CRUD API'ları")]
public class TagController(AppDbContext context) : ControllerBase
{

}