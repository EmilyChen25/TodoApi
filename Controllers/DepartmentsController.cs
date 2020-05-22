using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments  --//查詢all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            //return await _context.Department.ToListAsync();

            return await _context.Department
                        .Where(d => d.IsDeleted == null || d.IsDeleted == false) // 排除已標記刪除資料
                                    .ToListAsync();
        }

        // GET: api/Departments/5  --//查詢 by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            //var department = await _context.Department.FindAsync(id);

            var department = await _context.Department.Where(department => department.DepartmentId== id).FirstOrDefaultAsync();

            



            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5 //修改
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            var departmentOri = await _context.Department.FindAsync(id);

            if (departmentOri == null)
            {
                return NotFound();
            }

            department.DateModified = DateTime.Now;

            try
            {
                //_context.Entry(department).State = EntityState.Modified; 
                // await _context.SaveChangesAsync();


                //預存程序
                await _context.Database.ExecuteSqlRawAsync("Department_Update @DepartmentID = {0}, @Name = {1}, @Budget = {2}, @StartDate = {3}, @InstructorID = {4}, @RowVersion_Original = {5}"
                    , id, department.Name, department.Budget, department.StartDate, department.InstructorId, departmentOri.RowVersion);


            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        // POST: api/Departments  //新增
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            //_context.Department.Add(department);
            //await _context.SaveChangesAsync();

            //預存程序
            await _context.Database.ExecuteSqlRawAsync("Department_Insert @Name = {0}, @Budget = {1}, @StartDate = {2}, @InstructorID = {3}"
                , department.Name, department.Budget, department.StartDate, department.InstructorId);

            return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        }

        // DELETE: api/Departments/5   //刪除
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            //_context.Department.Remove(department);
            //await _context.SaveChangesAsync();

            ////預存程序
            //await _context.Database.ExecuteSqlRawAsync("Department_Delete @DepartmentID = {0}, @RowVersion_Original = {1}"
            //    , id, department.RowVersion);

            department.IsDeleted = true;
            _context.Department.Update(department);
            await _context.SaveChangesAsync();

            return department;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }


        // GET: api/GetDepartmentCourseCount
        [HttpGet("GetDepartmentCourseCount")]
        public async Task<ActionResult<IEnumerable<VwDepartmentCourseCount>>> GetDepartmentCourseCount()
        {
            return await _context.VwDepartmentCourseCount.FromSqlRaw("SELECT * FROM dbo.VwDepartmentCourseCount").ToListAsync();
        }

        // GET: api/GetDepartmentCourseCount/5  --//查詢 by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartmentCourseCount(int id)
        {
            //var department = await _context.Department.FindAsync(id);

            var department = await _context.Department.Where(d => d.DepartmentId== id).FirstOrDefaultAsync();

            //var department = await _context.VwDepartmentCourseCount.FromSqlRaw($"SELECT * FROM dbo.VwDepartmentCourseCount where DepartmentID={id}").ToListAsync();


            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

    }
}
