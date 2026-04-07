using System.ComponentModel.DataAnnotations;

namespace oop_s2_1_mvc_83356.ViewModels;

public class BranchIndexViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public int CourseCount { get; set; }
}

public class BranchDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public List<CourseBasicViewModel> Courses { get; set; } = new();
}

public class BranchCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Branch name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Branch name must be between 2 and 100 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters")]
    public string Address { get; set; }

    public string PageTitle => Id == 0 ? "Create Branch" : "Edit Branch";
}

public class CourseBasicViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
